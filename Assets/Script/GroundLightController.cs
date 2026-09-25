using UnityEngine;

public class GroundLightController : MonoBehaviour
{
    [Header("プレイヤー")]
    [SerializeField] private Transform player;

    [Header("Directional Light")]
    [SerializeField] private Light directionalLight;

    [Header("地上に出る高さ")]
    [SerializeField] private float groundHeight = 10f;

    private bool hasReachedGround = false;

    private void FixedUpdate()
    {
        if (player == null || directionalLight == null)
            return;

        // すでに地上に出ていたら何もしない
        if (hasReachedGround)
            return;

        // プレイヤーが地上の高さに到達
        if (player.position.y >= groundHeight)
        {
            hasReachedGround = true;

            directionalLight.enabled = true;

            Debug.Log("地上に出た！Directional Light ON");
        }
    }
}