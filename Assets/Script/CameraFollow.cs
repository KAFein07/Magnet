using UnityEngine;
using UnityEngine.InputSystem;

public class CameraFollow : MonoBehaviour
{
    [Header("追従するプレイヤー")]
    [SerializeField] private Transform target;

    [Header("カメラ位置")]
    [SerializeField]
    private Vector3 offset = new Vector3(15f, 2f, 0f);

    [Header("追従速度")]
    [SerializeField] private float followSpeed = 5f;

    [Header("ズーム")]
    [SerializeField] private float wheelSpeed = 0.02f;
    [SerializeField] private float minXOffset = 10f;
    [SerializeField] private float maxXOffset = 20f;
    [SerializeField] private float zoomSmoothTime = 0.15f;

    [SerializeField] private PlayerMove playerMove;

    private float targetXOffset;
    private float zoomVelocity;

    // マウスホイール入力を一時保存
    private float scrollInput = 0f;

    private void Awake()
    {
        targetXOffset = offset.x;
    }

    // Input Systemから呼ばれる
    public void OnCameraZoom(InputValue value)
    {
        Vector2 input = value.Get<Vector2>();

        // 入力を蓄積
        scrollInput += input.y;
        Debug.Log(scrollInput);
    }

    private void FixedUpdate()
    {
        if (target == null)
            return;

        HandleZoom();
        FollowTarget();
    }

    private void HandleZoom()
    {
        if (playerMove == null)
            return;

        float scrollInput =
            playerMove.ConsumeCameraZoom();

        if (Mathf.Abs(scrollInput) > 0.001f)
        {
            targetXOffset -=
                scrollInput * wheelSpeed;

            targetXOffset = Mathf.Clamp(
                targetXOffset,
                minXOffset,
                maxXOffset
            );
        }

        offset.x = Mathf.SmoothDamp(
            offset.x,
            targetXOffset,
            ref zoomVelocity,
            zoomSmoothTime
        );
    }

    private void FollowTarget()
    {
        Vector3 targetPosition =
            target.position + offset;

        Vector3 newPosition =
            Vector3.Lerp(
                transform.position,
                targetPosition,
                followSpeed * Time.fixedDeltaTime
            );

        transform.position = newPosition;
    }
}