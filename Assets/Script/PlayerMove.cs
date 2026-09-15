using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
    [Header("通常移動")]
    [SerializeField] private float moveSpeed = 5f;
    // 地面にいるときのA/Dの移動速度

    [Header("電磁石")]
    [SerializeField] private bool magnetEnabled = true;
    // 電磁石がONかOFFか
    // true = ON、false = OFF
    [SerializeField] private bool isNorthPole = true;
    // 現在の極
    // true = N極、false = S極

    [Header("温度")]
    [SerializeField] private float temperature = 0f;
    // 現在の電磁石の温度

    [SerializeField] private float maxTemperature = 100f;
    // 電磁石の最大温度

    [SerializeField] private float heatSpeed = 10f;
    // 電磁石ON時に温度が上昇する速さ

    [SerializeField] private float coolSpeed = 20f;
    // 電磁石OFF時に温度が下がる速さ

    [Header("磁力")]
    [SerializeField] private float magneticRange = 3f;
    // 磁力が届く範囲

    [SerializeField] private float magneticForce = 30f;
    // 異なる極の岩を引き寄せる力

    [SerializeField] private float repulsionForce = 50f;
    // 同じ極の岩から受ける反発力

    [Header("吸着移動")]
    [SerializeField] private float wallMoveSpeed = 4f;
    // 壁や天井に吸着しているときの基本移動速度

    [SerializeField] private float wallHorizontalSpeedMultiplier = 0.5f;
    // 壁や天井に吸着しているときのA/D移動速度の倍率
    // 0.5 = 通常の50%の速度

    [SerializeField] private float surfaceDistance = 1.2f;
    // 岩にどれくらい近づいたら吸着するか

    [Header("極変更時の反発")]
    [SerializeField] private float switchRepulsionForce = 35f;
    // N極⇔S極を切り替えたときに発生する反発力

    [SerializeField] private float switchRepulsionDuration = 0.3f;
    // 極を切り替えたときの反発が続く時間

    [SerializeField] private float launchUpForce = 0.5f;
    // 壁に吸着している状態で極変更したときに
    // 少し上方向へ飛ばすための補正値

    [Header("アニメーション")]
    [SerializeField] private Animator animator;
    // プレイヤーのアニメーションを制御するAnimator

    [Header("見た目の向き")]
    [SerializeField] private Transform modelTransform;
    // プレイヤーの見た目（モデル）の向きを変更するためのTransform

    [Header("電磁石の見た目")]
    [SerializeField] private Material coilMaterial;
    // 電磁石のコイル部分に使用するマテリアル

    [SerializeField] private Material lampMaterial;
    // 電磁石のランプ部分に使用するマテリアル

    [SerializeField] private Color northColor = Color.red;
    // N極のときのコイルの色

    [SerializeField] private Color southColor = Color.blue;
    // S極のときのコイルの色

    [SerializeField] private Color lampOnColor = Color.white;
    // 電磁石ON時のランプの色

    [SerializeField] private Color lampOffColor = Color.black;
    // 電磁石OFF時のランプの色

    [Header("電磁石の光")]
    [SerializeField] private Light coilLight;
    // コイルから出す光

    [SerializeField] private Light lampLight;
    // ランプから出す光

    [SerializeField] private float coilLightIntensity = 2f;
    // コイルの光の明るさ

    [SerializeField] private float lampLightIntensity = 3f;
    // ランプの光の明るさ

    [SerializeField] private float lightRange = 3f;
    // 光が届く距離
    private Rigidbody rb;

    private Vector2 moveInput;

    private bool isAttached = false;

    private Vector3 surfaceNormal;

    private Collider attachedRock;

    private float repulsionTimer = 0f;

    private Vector3 repulsionDirection;

    // 地面判定
    private bool isGrounded = false;


    // =========================
    // 外部公開
    // =========================

    public bool MagnetEnabled => magnetEnabled;

    public bool IsNorthPole => isNorthPole;

    public float Temperature => temperature;

    public bool IsAttached => isAttached;

    public float MagnetStrength
    {
        get
        {
            return 1f - temperature / maxTemperature;
        }
    }


    // =========================
    // 初期化
    // =========================

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }

        if (modelTransform == null && animator != null)
        {
            modelTransform = animator.transform;
        }

        UpdateElectromagnetVisual();
    }


    // =========================
    // 物理更新
    // =========================

    private void FixedUpdate()
    {
        // 温度
        HandleTemperature();

        // 極変更時の反発
        HandleSwitchRepulsion();

        // 磁力
        if (magnetEnabled)
        {
            HandleMagnetism();
        }
        else
        {
            ReleaseFromSurface();
        }

        // 移動
        if (isAttached)
        {
            MoveOnSurface();
        }
        else
        {
            MoveNormal();
        }

        // アニメーション
        UpdateAnimation();

        // 電磁石の見た目
        UpdateElectromagnetVisual();
    }


    // =========================
    // 入力
    // =========================

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }


    public void OnSwitchPole(InputValue value)
    {
        if (!value.isPressed)
            return;

        // 吸着中なら極変更時に反発
        if (isAttached && attachedRock != null)
        {
            Vector3 closestPoint =
                attachedRock.ClosestPoint(transform.position);

            Vector3 awayDirection =
                (transform.position - closestPoint).normalized;

            bool isGround = surfaceNormal.y > 0.7f;
            bool isCeiling = surfaceNormal.y < -0.7f;

            if (isGround)
            {
                repulsionDirection =
                    Vector3.ProjectOnPlane(
                        awayDirection,
                        Vector3.up
                    ).normalized;

                if (repulsionDirection == Vector3.zero)
                {
                    repulsionDirection = Vector3.forward;
                }
            }
            else if (isCeiling)
            {
                Vector3 surfaceForward =
                    Vector3.ProjectOnPlane(
                        Vector3.forward,
                        surfaceNormal
                    ).normalized;

                if (surfaceForward == Vector3.zero)
                {
                    surfaceForward = Vector3.right;
                }

                repulsionDirection =
                    (
                        awayDirection * 0.3f +
                        surfaceForward * 0.7f
                    ).normalized;
            }
            else
            {
                Vector3 surfaceUp =
                    Vector3.ProjectOnPlane(
                        Vector3.up,
                        surfaceNormal
                    ).normalized;

                if (surfaceUp == Vector3.zero)
                {
                    surfaceUp = Vector3.up;
                }

                repulsionDirection =
                    (
                        awayDirection +
                        surfaceUp * launchUpForce
                    ).normalized;
            }

            repulsionTimer =
                switchRepulsionDuration;

            ReleaseFromSurface();
        }

        // N ⇔ S
        isNorthPole = !isNorthPole;

        Debug.Log(
            isNorthPole
                ? "N極"
                : "S極"
        );

        UpdateElectromagnetVisual();
    }


    public void OnMagnetToggle(InputValue value)
    {
        if (!value.isPressed)
            return;

        magnetEnabled = !magnetEnabled;

        if (!magnetEnabled)
        {
            ReleaseFromSurface();
        }

        Debug.Log(
            magnetEnabled
                ? "磁力ON"
                : "磁力OFF"
        );

        UpdateElectromagnetVisual();
    }


    // =========================
    // 通常移動
    // =========================

    private void MoveNormal()
    {
        Vector3 velocity = rb.linearVelocity;

        if (isGrounded)
        {
            // 地上ではA/Dで自由に横移動
            velocity.z = moveInput.x * moveSpeed;
        }
        // 空中ではvelocity.zを一切変更しない
        // → 地上で持っていた横方向の慣性がそのまま残る

        rb.linearVelocity = velocity;
    }


    // =========================
    // 地面判定
    // =========================

    private void OnCollisionStay(Collision collision)
    {
        foreach (ContactPoint contact in collision.contacts)
        {
            // 上向きの面 = 地面
            if (contact.normal.y > 0.7f)
            {
                isGrounded = true;
                return;
            }
        }
    }


    private void OnCollisionExit(Collision collision)
    {
        // いったん解除
        isGrounded = false;
    }


    // =========================
    // 磁力
    // =========================

    private void HandleMagnetism()
    {
        Collider[] nearbyRocks =
            Physics.OverlapSphere(
                transform.position,
                magneticRange
            );

        float closestDistance =
            Mathf.Infinity;

        Collider closestRock = null;

        foreach (Collider rock in nearbyRocks)
        {
            if (rock.transform.root == transform.root)
                continue;

            bool attracts = false;

            // N極の岩
            if (rock.CompareTag("N極"))
            {
                attracts = !isNorthPole;
            }

            // S極の岩
            else if (rock.CompareTag("S極"))
            {
                attracts = isNorthPole;
            }

            // 引き寄せない場合
            if (!attracts)
            {
                if (
                    rock.CompareTag("N極") ||
                    rock.CompareTag("S極")
                )
                {
                    ApplyRepulsion(rock);
                }

                continue;
            }

            Vector3 closestPoint =
                rock.ClosestPoint(
                    transform.position
                );

            float distance =
                Vector3.Distance(
                    transform.position,
                    closestPoint
                );

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestRock = rock;
            }

            ApplyAttraction(rock);
        }

        // 一番近い岩に吸着
        if (
            closestRock != null &&
            closestDistance <= surfaceDistance &&
            MagnetStrength > 0.1f
        )
        {
            AttachToSurface(closestRock);
        }
    }


    private void ApplyAttraction(Collider rock)
    {
        Vector3 closestPoint =
            rock.ClosestPoint(
                transform.position
            );

        Vector3 direction =
            (
                closestPoint -
                transform.position
            ).normalized;

        float force =
            magneticForce *
            MagnetStrength;

        rb.AddForce(
            direction * force,
            ForceMode.Force
        );
    }


    private void ApplyRepulsion(Collider rock)
    {
        Vector3 closestPoint =
            rock.ClosestPoint(
                transform.position
            );

        Vector3 direction =
            (
                transform.position -
                closestPoint
            ).normalized;

        float force =
            repulsionForce *
            MagnetStrength;

        rb.AddForce(
            direction * force,
            ForceMode.Force
        );
    }


    // =========================
    // 吸着
    // =========================

    private void AttachToSurface(Collider rock)
    {
        Vector3 closestPoint =
            rock.ClosestPoint(
                transform.position
            );

        Vector3 normal =
            (
                transform.position -
                closestPoint
            ).normalized;

        if (normal == Vector3.zero)
            return;

        // 地面には吸着しない
        if (normal.y > 0.7f)
        {
            return;
        }

        attachedRock = rock;

        surfaceNormal = normal;

        isAttached = true;
    }


    // =========================
    // 吸着中の移動
    // =========================

    private void MoveOnSurface()
    {
        if (attachedRock == null)
        {
            ReleaseFromSurface();
            return;
        }

        Vector3 normal = surfaceNormal;

        Vector3 surfaceUp =
            Vector3.ProjectOnPlane(
                Vector3.up,
                normal
            ).normalized;

        if (surfaceUp == Vector3.zero)
        {
            surfaceUp =
                Vector3.ProjectOnPlane(
                    Vector3.forward,
                    normal
                ).normalized;
        }

        Vector3 surfaceRight =
            Vector3.Cross(
                surfaceUp,
                normal
            ).normalized;

        // A/Dは弱める
        float horizontalSpeed =
            wallMoveSpeed *
            wallHorizontalSpeedMultiplier;

        Vector3 targetVelocity =
            surfaceRight *
            moveInput.x *
            horizontalSpeed
            +
            surfaceUp *
            moveInput.y *
            wallMoveSpeed;

        Vector3 normalVelocity =
            Vector3.Project(
                rb.linearVelocity,
                normal
            );

        rb.linearVelocity =
            targetVelocity +
            normalVelocity;

        // 吸着力
        float stickForce =
            magneticForce *
            MagnetStrength;

        rb.AddForce(
            -normal *
            stickForce,
            ForceMode.Force
        );
    }


    private void ReleaseFromSurface()
    {
        isAttached = false;

        attachedRock = null;

        surfaceNormal =
            Vector3.zero;
    }


    // =========================
    // 極変更時の反発
    // =========================

    private void HandleSwitchRepulsion()
    {
        if (repulsionTimer <= 0f)
            return;

        repulsionTimer -=
            Time.fixedDeltaTime;

        float force =
            switchRepulsionForce *
            MagnetStrength;

        rb.AddForce(
            repulsionDirection *
            force,
            ForceMode.Force
        );
    }


    // =========================
    // 温度
    // =========================

    private void HandleTemperature()
    {
        if (magnetEnabled)
        {
            temperature +=
                heatSpeed *
                Time.fixedDeltaTime;
        }
        else
        {
            temperature -=
                coolSpeed *
                Time.fixedDeltaTime;
        }

        temperature =
            Mathf.Clamp(
                temperature,
                0f,
                maxTemperature
            );
    }


    // =========================
    // アニメーション
    // =========================

    private void UpdateAnimation()
    {
        if (animator == null)
            return;

        // A/Dを押している
        bool isWalking =
            Mathf.Abs(
                moveInput.x
            ) > 0.01f;

        // 吸着中 + W/S
        bool isClimbing =
            isAttached &&
            Mathf.Abs(
                moveInput.y
            ) > 0.01f;

        animator.SetBool(
            "IsWalking",
            isWalking &&
            !isClimbing
        );

        animator.SetBool(
            "IsClimbing",
            isClimbing
        );

        // A/Dで見た目の向きを変更
        if (isWalking)
        {
            if (moveInput.x > 0f)
            {
                FaceRight();
            }
            else if (moveInput.x < 0f)
            {
                FaceLeft();
            }
        }
    }


    private void FaceRight()
    {
        if (modelTransform == null)
            return;

        modelTransform.localRotation =
            Quaternion.Euler(
                0f,
                90f,
                0f
            );
    }


    private void FaceLeft()
    {
        if (modelTransform == null)
            return;

        modelTransform.localRotation =
            Quaternion.Euler(
                0f,
                -90f,
                0f
            );
    }


    // =========================
    // 電磁石の見た目
    // =========================

    private void UpdateElectromagnetVisual()
    {
        // コイル
        if (coilMaterial != null)
        {
            if (magnetEnabled)
            {
                Color coilColor =
                    isNorthPole
                        ? northColor
                        : southColor;

                coilMaterial.color =
                    coilColor;

                coilMaterial.EnableKeyword(
                    "_EMISSION"
                );

                coilMaterial.SetColor(
                    "_EmissionColor",
                    coilColor * 2f
                );
            }
            else
            {
                coilMaterial.color =
                    Color.black;

                coilMaterial.DisableKeyword(
                    "_EMISSION"
                );

                coilMaterial.SetColor(
                    "_EmissionColor",
                    Color.black
                );
            }
        }


        // コイルの光
        if (coilLight != null)
        {
            Color coilColor =
                isNorthPole
                    ? northColor
                    : southColor;

            coilLight.color =
                coilColor;

            coilLight.intensity =
                coilLightIntensity;

            coilLight.range =
                lightRange;

            coilLight.enabled =
                magnetEnabled;
        }


        // ランプ
        if (lampMaterial != null)
        {
            if (magnetEnabled)
            {
                lampMaterial.color =
                    lampOnColor;

                lampMaterial.EnableKeyword(
                    "_EMISSION"
                );

                lampMaterial.SetColor(
                    "_EmissionColor",
                    lampOnColor * 2f
                );
            }
            else
            {
                lampMaterial.color =
                    lampOffColor;

                lampMaterial.DisableKeyword(
                    "_EMISSION"
                );

                lampMaterial.SetColor(
                    "_EmissionColor",
                    Color.black
                );
            }
        }


        // ランプの光
        if (lampLight != null)
        {
            lampLight.color =
                lampOnColor;

            lampLight.intensity =
                lampLightIntensity;

            lampLight.range =
                lightRange;

            lampLight.enabled =
                magnetEnabled;
        }
    }


    // =========================
    // Gizmos
    // =========================

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(
            transform.position,
            magneticRange
        );

        if (isAttached)
        {
            Gizmos.DrawLine(
                transform.position,
                transform.position +
                surfaceNormal
            );
        }
    }
}