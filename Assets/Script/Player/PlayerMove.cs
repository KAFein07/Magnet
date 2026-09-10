using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
    [Header("通常移動")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("電磁石")]
    [SerializeField] private bool magnetEnabled = true;
    [SerializeField] private bool isNorthPole = true;

    [Header("温度")]
    [SerializeField] private float temperature = 0f;
    [SerializeField] private float maxTemperature = 100f;
    [SerializeField] private float heatSpeed = 20f;
    [SerializeField] private float coolSpeed = 30f;

    [Header("磁力")]
    [SerializeField] private float magneticRange = 3f;
    [SerializeField] private float magneticForce = 30f;
    [SerializeField] private float repulsionForce = 50f;

    [Header("吸着移動")]
    [SerializeField] private float wallMoveSpeed = 4f;
    [SerializeField] private float surfaceDistance = 1.2f;

    [Header("極変更時の反発")]
    [SerializeField] private float switchRepulsionForce = 35f;
    [SerializeField] private float switchRepulsionDuration = 0.3f;
    [SerializeField] private float launchUpForce = 0.5f;

    private Rigidbody rb;

    // ========================================
    // 入力
    // ========================================

    private Vector2 moveInput;

    // ========================================
    // 吸着
    // ========================================

    private bool isAttached = false;

    // 吸着している面の法線
    private Vector3 surfaceNormal;

    // 吸着している岩
    private Collider attachedRock;

    // ========================================
    // 極変更時の反発
    // ========================================

    private float repulsionTimer = 0f;

    private Vector3 repulsionDirection;

    // ========================================
    // 外部公開
    // ========================================

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

    // ========================================
    // 初期化
    // ========================================

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    // ========================================
    // Physics
    // ========================================

    private void FixedUpdate()
    {
        // 温度
        HandleTemperature();

        // 極変更直後の反発
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
    }

    // ========================================
    // 入力
    // ========================================

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    // ========================================
    // N / S切り替え
    // ========================================

    public void OnSwitchPole(InputValue value)
    {
        if (!value.isPressed)
            return;

        // ------------------------------------
        // 吸着中の場合
        // ------------------------------------

        if (isAttached && attachedRock != null)
        {
            Vector3 closestPoint =
                attachedRock.ClosestPoint(
                    transform.position
                );

            // 岩から離れる方向
            Vector3 awayDirection =
                (
                    transform.position -
                    closestPoint
                ).normalized;

            // --------------------------------
            // 地面 / 壁 / 天井を判定
            // --------------------------------

            bool isGround =
                surfaceNormal.y > 0.7f;

            bool isCeiling =
                surfaceNormal.y < -0.7f;

            // --------------------------------
            // 反発方向
            // --------------------------------

            if (isGround)
            {
                // 地面では大きく上に飛ばさない
                repulsionDirection =
                    Vector3.ProjectOnPlane(
                        awayDirection,
                        Vector3.up
                    ).normalized;

                if (repulsionDirection == Vector3.zero)
                {
                    repulsionDirection =
                        Vector3.forward;
                }
            }
            else if (isCeiling)
            {
                // --------------------------------
                // 天井
                // --------------------------------
                // 真下へ猛烈に飛ばすのではなく
                // 天井に沿う方向を強くする

                Vector3 surfaceForward =
                    Vector3.ProjectOnPlane(
                        Vector3.forward,
                        surfaceNormal
                    ).normalized;

                if (surfaceForward == Vector3.zero)
                {
                    surfaceForward =
                        Vector3.right;
                }

                repulsionDirection =
                    (
                        awayDirection * 0.3f +
                        surfaceForward * 0.7f
                    ).normalized;
            }
            else
            {
                // --------------------------------
                // 壁
                // --------------------------------

                Vector3 surfaceUp =
                    Vector3.ProjectOnPlane(
                        Vector3.up,
                        surfaceNormal
                    ).normalized;

                if (surfaceUp == Vector3.zero)
                {
                    surfaceUp =
                        Vector3.up;
                }

                // 壁から離れる
                // ＋ 面に沿って少し上
                repulsionDirection =
                    (
                        awayDirection +
                        surfaceUp * launchUpForce
                    ).normalized;
            }

            // --------------------------------
            // 反発開始
            // --------------------------------

            repulsionTimer =
                switchRepulsionDuration;

            // 吸着解除
            ReleaseFromSurface();
        }

        // ------------------------------------
        // 極を変更
        // ------------------------------------

        isNorthPole = !isNorthPole;

        Debug.Log(
            isNorthPole
                ? "N極"
                : "S極"
        );
    }

    // ========================================
    // 極変更時の反発
    // ========================================

    private void HandleSwitchRepulsion()
    {
        if (repulsionTimer <= 0f)
            return;

        // タイマーを減らす
        repulsionTimer -=
            Time.fixedDeltaTime;

        // 現在の磁力
        float force =
            switchRepulsionForce *
            MagnetStrength;

        // 短時間だけ押し続ける
        rb.AddForce(
            repulsionDirection * force,
            ForceMode.Force
        );
    }

    // ========================================
    // 磁力 ON / OFF
    // ========================================

    public void OnMagnetToggle(InputValue value)
    {
        if (!value.isPressed)
            return;

        magnetEnabled =
            !magnetEnabled;

        if (!magnetEnabled)
        {
            ReleaseFromSurface();
        }

        Debug.Log(
            magnetEnabled
                ? "磁力ON"
                : "磁力OFF"
        );
    }

    // ========================================
    // 通常移動
    // ========================================

    private void MoveNormal()
    {
        Vector3 velocity =
            rb.linearVelocity;

        // A / Dだけ使用
        velocity.z =
            moveInput.x *
            moveSpeed;

        rb.linearVelocity =
            velocity;
    }

    // ========================================
    // 磁力処理
    // ========================================

    private void HandleMagnetism()
    {
        Collider[] nearbyRocks =
            Physics.OverlapSphere(
                transform.position,
                magneticRange
            );

        float closestDistance =
            Mathf.Infinity;

        Collider closestRock =
            null;

        foreach (Collider rock in nearbyRocks)
        {
            // 自分自身を無視
            if (rock.transform.root == transform.root)
                continue;

            bool attracts = false;

            // --------------------------------
            // N極
            // --------------------------------

            if (rock.CompareTag("N極"))
            {
                attracts =
                    !isNorthPole;
            }

            // --------------------------------
            // S極
            // --------------------------------

            else if (rock.CompareTag("S極"))
            {
                attracts =
                    isNorthPole;
            }

            // --------------------------------
            // 同極
            // --------------------------------

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

            // --------------------------------
            // 異極
            // --------------------------------

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
                closestDistance =
                    distance;

                closestRock =
                    rock;
            }

            // 吸着方向へ引っ張る
            ApplyAttraction(rock);
        }

        // ------------------------------------
        // 吸着
        // ------------------------------------

        if (
            closestRock != null &&
            closestDistance <= surfaceDistance &&
            MagnetStrength > 0.1f
        )
        {
            AttachToSurface(
                closestRock
            );
        }
    }

    // ========================================
    // 吸着力
    // ========================================

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

    // ========================================
    // 通常の反発
    // ========================================

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

    // ========================================
    // 面に吸着
    // ========================================

    private void AttachToSurface(Collider rock)
    {
        attachedRock =
            rock;

        Vector3 closestPoint =
            rock.ClosestPoint(
                transform.position
            );

        surfaceNormal =
            (
                transform.position -
                closestPoint
            ).normalized;

        if (surfaceNormal == Vector3.zero)
            return;

        isAttached = true;

        // ------------------------------------
        // 壁へ向かう速度だけ消す
        // ------------------------------------

        Vector3 velocity =
            rb.linearVelocity;

        velocity -=
            Vector3.Project(
                velocity,
                -surfaceNormal
            );

        rb.linearVelocity =
            velocity;
    }

    // ========================================
    // 壁・天井移動
    // ========================================

    private void MoveOnSurface()
    {
        if (attachedRock == null)
        {
            ReleaseFromSurface();
            return;
        }

        Vector3 normal =
            surfaceNormal;

        // ------------------------------------
        // 面に沿った上方向
        // ------------------------------------

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

        // ------------------------------------
        // 面に沿った右方向
        // ------------------------------------

        Vector3 surfaceRight =
            Vector3.Cross(
                surfaceUp,
                normal
            ).normalized;

        // ------------------------------------
        // 移動方向
        // ------------------------------------

        Vector3 moveDirection =
            surfaceRight *
            moveInput.x
            +
            surfaceUp *
            moveInput.y;

        moveDirection =
            Vector3.ClampMagnitude(
                moveDirection,
                1f
            );

        Vector3 targetVelocity =
            moveDirection *
            wallMoveSpeed;

        // ------------------------------------
        // 面に垂直な速度
        // ------------------------------------

        Vector3 normalVelocity =
            Vector3.Project(
                rb.linearVelocity,
                normal
            );

        rb.linearVelocity =
            targetVelocity +
            normalVelocity;

        // ------------------------------------
        // 面に押し付ける
        // ------------------------------------

        float stickForce =
            magneticForce *
            MagnetStrength;

        rb.AddForce(
            -normal *
            stickForce,
            ForceMode.Force
        );
    }

    // ========================================
    // 吸着解除
    // ========================================

    private void ReleaseFromSurface()
    {
        isAttached = false;

        attachedRock = null;

        surfaceNormal =
            Vector3.zero;

        // 速度は消さない
        // → 極変更時の反発を維持する
    }

    // ========================================
    // 温度
    // ========================================

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

    // ========================================
    // デバッグ
    // ========================================

    private void OnDrawGizmosSelected()
    {
        // 磁力範囲
        Gizmos.DrawWireSphere(
            transform.position,
            magneticRange
        );

        // 吸着面の法線
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