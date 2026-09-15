using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("追従するプレイヤー")]
    [SerializeField] private Transform target;

    [Header("カメラ位置")]
    [SerializeField]
    private Vector3 offset =
        new Vector3(15f, 2f, 0f);

    [Header("追従速度")]
    [SerializeField] private float followSpeed = 5f;

    private Rigidbody targetRb;

    private void Awake()
    {
        if (target != null)
        {
            targetRb =
                target.GetComponent<Rigidbody>();
        }
    }

    private void FixedUpdate()
    {
        if (target == null)
            return;

        FollowTarget();
    }

    private void FollowTarget()
    {
        Vector3 targetPosition =
            target.position + offset;

        Vector3 newPosition =
            Vector3.Lerp(
                transform.position,
                targetPosition,
                followSpeed *
                Time.fixedDeltaTime
            );

        transform.position = newPosition;
    }
}