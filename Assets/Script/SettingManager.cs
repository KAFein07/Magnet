using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Audio;

public class SettingManager : MonoBehaviour
{
    [Header("設定画面")]
    [SerializeField] private GameObject settingPanel;

    [Header("音量")]
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private UnityEngine.UI.Slider bgmSlider;
    [SerializeField] private UnityEngine.UI.Slider seSlider;

    [Header("ゲーム管理")]
    [SerializeField] private InGameManager inGameManager;

    private bool isSettingOpen = false;

    private void Start()
    {
        // 最初は設定画面を閉じる
        if (settingPanel != null)
        {
            settingPanel.SetActive(false);
        }
    }

    public void OnPause(InputValue value)
    {
        if (!value.isPressed)
            return;

        ToggleSetting();
    }

    private void ToggleSetting()
    {
        isSettingOpen = !isSettingOpen;

        if (settingPanel != null)
        {
            settingPanel.SetActive(isSettingOpen);
        }

        if (isSettingOpen)
        {
            // ゲームを一時停止
            Time.timeScale = 0f;
        }
        else
        {
            // ゲームを再開
            Time.timeScale = 1f;
        }
    }

    public void SetBGMVolume(float value)
    {
        audioMixer.SetFloat("BGMVolume", Mathf.Log10(Mathf.Max(value, 0.0001f)) * 20f);
    }

    public void SetSEVolume(float value)
    {
        audioMixer.SetFloat("SEVolume", Mathf.Log10(Mathf.Max(value, 0.0001f)) * 20f);
    }

    public void ReturnToTitle()
    {
        // 現在のゲームをセーブ
        if (inGameManager != null)
        {
            inGameManager.SaveGame();
        }

        // ゲームを再開状態に戻す
        Time.timeScale = 1f;

        // タイトルシーンへ移動
        UnityEngine.SceneManagement.SceneManager.LoadScene("Title");
    }

    public void SaveAndExit()
    {
        // 現在のゲームをセーブ
        if (inGameManager != null)
        {
            inGameManager.SaveGame();
        }

        // ゲームを終了するため、一時停止を解除
        Time.timeScale = 1f;

        // ゲームを終了
        Application.Quit();

        Debug.Log("ゲームをセーブして終了しました！");
    }
}