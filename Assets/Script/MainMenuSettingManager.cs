using UnityEngine;

public class MainMenuSettingManager : MonoBehaviour
{
    [Header("設定画面")]
    [SerializeField] private GameObject settingPanel;

    [Header("削除確認")]
    [SerializeField] private GameObject deleteConfirmPanel;



    private void Start()
    {
        // 削除確認画面は最初は非表示
        if (deleteConfirmPanel != null)
        {
            deleteConfirmPanel.SetActive(false);
        }
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