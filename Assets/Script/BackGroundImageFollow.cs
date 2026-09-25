using UnityEngine;

public class BackgroundImageFollow : MonoBehaviour
{
    [Header("プレイヤー")]
    [SerializeField] private Transform player;

    [Header("追従")]
    [SerializeField] private float followRate = 0.3f;

    [SerializeField] private Vector3 targetPosition;

    
    [SerializeField] private float startPlayerY;
    [SerializeField] private Vector3 startPosition;

    

    private void Start()
    {
        //startPlayerY = player.position.y;
        //startPosition = transform.localPosition;
    }

    private void FixedUpdate()
    {
        if (player == null)
            return;

        float playerY =
            player.position.y - startPlayerY;

        targetPosition = startPosition;

        // プレイヤーの上昇量の一部だけ背景を動かす
        targetPosition.y -= playerY * followRate;

        transform.localPosition = Vector3.Lerp(
            transform.localPosition,
            targetPosition,
            3f * Time.fixedDeltaTime
        );
    }
}