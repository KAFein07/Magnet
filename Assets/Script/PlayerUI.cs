using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerUI : MonoBehaviour
{
    [Header("プレイヤー")]
    [SerializeField] private PlayerMove player;

    [Header("温度ゲージ")]
    [SerializeField] private Image temperatureGauge;

    [Header("温度表示")]
    [SerializeField] private TMP_Text temperatureText;

    [Header("磁力ON/OFF")]
    [SerializeField] private TMP_Text magnetText;

    [Header("極")]
    [SerializeField] private TMP_Text poleText;

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

        // 温度の数字を更新
        if (temperatureText != null)
        {
            temperatureText.text =
                Mathf.RoundToInt(temperature) + "°C";
        }
    }

    private void UpdateMagnetStatus()
    {
        if (magnetText == null)
            return;

        if (player.MagnetEnabled)
        {
            magnetText.text = "MAGNET ON";
        }
        else
        {
            magnetText.text = "MAGNET OFF";
        }
    }

    private void UpdatePoleStatus()
    {
        if (poleText == null)
            return;

        if (player.IsNorthPole)
        {
            poleText.text = "N POLE";
        }
        else
        {
            poleText.text = "S POLE";
        }
    }
}