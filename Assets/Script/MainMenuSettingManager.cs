using UnityEngine;
using UnityEngine.Audio;

public class MainMenuSettingManager : MonoBehaviour
{
    [Header("設定画面")]
    [SerializeField] private GameObject settingPanel;

    [Header("ショップ")]
    [SerializeField] private GameObject shopPanel;
    [SerializeField] private TMPro.TMP_Text trophyCountText;

    private bool isShopOpen = false;

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

        if (shopPanel != null)
        {
            shopPanel.SetActive(false);
        }

        UpdateTrophyCount();

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
            AudioManager.Instance.PlayDecisionSE();
            deleteConfirmPanel.SetActive(true);
        }
    }


    // 「はい」ボタン
    public void DeleteSaveData()
    {
        PlayerPrefs.DeleteKey("GameSaveData");
        PlayerPrefs.Save();

        AudioManager.Instance.PlayDecisionSE();
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
            AudioManager.Instance.PlayCancelSE();
            deleteConfirmPanel.SetActive(false);
        }
    }
    // 設定画面を開く
    public void OpenSetting()
    {
        if (settingPanel != null)
        {
            AudioManager.Instance.PlayDecisionSE();
            settingPanel.SetActive(true);
        }
    }

    // 設定画面を閉じる
    public void CloseSetting()
    {
        if (settingPanel != null)
        {
            AudioManager.Instance.PlayCancelSE();
            settingPanel.SetActive(false);
        }
    }

    public void ExitGame()
    {
        
        // ゲームを終了
        Application.Quit();
    }

    // ショップ画面を開閉
    public void ToggleShop()
    {
        isShopOpen = !isShopOpen;

        if (shopPanel != null)
        {
            shopPanel.SetActive(isShopOpen);
        }

        UpdateTrophyCount();
    }

    // ショップ画面の閉じるボタン用
    public void CloseShop()
    {
        isShopOpen = false;

        if (shopPanel != null)
        {
            shopPanel.SetActive(false);
        }
    }

    // 所持トロフィー数を表示
    public void UpdateTrophyCount()
    {
        if (trophyCountText == null)
            return;

        trophyCountText.text =
    TrophyManager.TrophyCount.ToString();
    }


}