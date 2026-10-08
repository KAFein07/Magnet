using UnityEngine;
using UnityEngine.Audio;

public class MainMenuSettingManager : MonoBehaviour
{
    [Header("設定画面")]
    [SerializeField] private GameObject settingPanel;

    [Header("削除確認")]
    [SerializeField] private GameObject deleteConfirmPanel;

    [Header("続きから")]
    [SerializeField] private GameObject continueButton;

    [Header("音量")]
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private UnityEngine.UI.Slider bgmSlider;
    [SerializeField] private UnityEngine.UI.Slider seSlider;



    private void Start()
    {
        // 設定画面を閉じる
        if (settingPanel != null)
        {
            settingPanel.SetActive(false);
        }

        // 保存されている音量を読み込む
        float bgmVolume = PlayerPrefs.GetFloat("BGMVolume", 1f);
        float seVolume = PlayerPrefs.GetFloat("SEVolume", 1f);

        if (bgmSlider != null)
        {
            bgmSlider.value = bgmVolume;
        }

        if (seSlider != null)
        {
            seSlider.value = seVolume;
        }

        SetBGMVolume(bgmVolume);
        SetSEVolume(seVolume);
    }

    public void SetBGMVolume(float value)
    {
        audioMixer.SetFloat(
            "BGMVolume",
            Mathf.Log10(Mathf.Max(value, 0.0001f)) * 20f
        );
        Debug.Log("BGMVolume :" + value);
        PlayerPrefs.SetFloat("BGMVolume", value);
        PlayerPrefs.Save();
    }

    public void SetSEVolume(float value)
    {
        audioMixer.SetFloat(
            "SEVolume",
            Mathf.Log10(Mathf.Max(value, 0.0001f)) * 20f
        );
        Debug.Log("SEVolume :" + value);

        PlayerPrefs.SetFloat("SEVolume", value);
        PlayerPrefs.Save();
    }


    // 「セーブデータを削除」ボタン
    public void OpenDeleteConfirm()
    {
        if (deleteConfirmPanel != null)
        {
            deleteConfirmPanel.SetActive(true);
        }
    }


    // 「はい」ボタン
    // 「はい」ボタン
    public void DeleteSaveData()
    {
        PlayerPrefs.DeleteKey("GameSaveData");
        PlayerPrefs.Save();

        // 確認画面を閉じる
        if (deleteConfirmPanel != null)
        {
            deleteConfirmPanel.SetActive(false);
        }
        // 削除確認画面は最初は非表示
        if (continueButton != null)
        {
            continueButton.SetActive(false);
        }

        Debug.Log("セーブデータを削除しました！");
    }


    // 「いいえ」ボタン
    public void CancelDelete()
    {
        if (deleteConfirmPanel != null)
        {
            deleteConfirmPanel.SetActive(false);
        }
    }
    // 設定画面を開く
    public void OpenSetting()
    {
        if (settingPanel != null)
        {
            settingPanel.SetActive(true);
        }
    }

    // 設定画面を閉じる
    public void CloseSetting()
    {
        if (settingPanel != null)
        {
            settingPanel.SetActive(false);
        }
    }
}