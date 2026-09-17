using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerUI : MonoBehaviour
{
    [Header("プレイヤー")]
    [SerializeField] private PlayerMove player;

    [Header("温度ゲージ")]
    [SerializeField] private Image temperatureGauge;

    [Header("磁力ON/OFF")]
    [SerializeField] private Image magnetImage;

    [Header("極")]
    [SerializeField] private Image poleImage;

    private void FixedUpdate()
    {
        if (player == null)
            return;

        UpdateTemperature();
        UpdateMagnetStatus();
        UpdatePoleStatus();
    }

    private void UpdateTemperature()
    {
        // 0～100を0～1に変換
        float temperature =
            player.Temperature;

        float fillAmount =
            Mathf.Clamp01(
                temperature / 100f
            );

        // Imageのゲージを更新
        if (temperatureGauge != null)
        {
            temperatureGauge.fillAmount =
                fillAmount;
        }
    }

    private void UpdateMagnetStatus()
    {
        if (magnetImage == null)
            return;

        if (player.MagnetEnabled)
        {
            magnetImage.enabled = true;
        }
        else
        {
            magnetImage.enabled = false;
        }
    }

    private void UpdatePoleStatus()
    {
        if (poleImage == null)
            return;

        if (player.IsNorthPole)
        {
            poleImage.enabled = true;
        }
        else
        {
            poleImage.enabled = false;
        }
    }
}