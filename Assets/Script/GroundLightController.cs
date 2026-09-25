using UnityEngine;

public class GroundLightController : MonoBehaviour
{
    [Header("プレイヤー")]
    [SerializeField] private Transform player;

    [Header("Directional Light")]
    [SerializeField] private Light directionalLight;

    [Header("地上に出る高さ")]
    [SerializeField] private float groundHeight = 10f;

    private void FixedUpdate()
    {
        if (player == null || directionalLight == null)
            return;

        if (player.position.y >= groundHeight)
        {
            // 地上
            directionalLight.enabled = true;
        }
        else
        {
            // 地下
            directionalLight.enabled = false;
        }
    }
}