using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameStartManager : MonoBehaviour
{
    [Header("チュートリアル")]
    [SerializeField] private Toggle tutorialToggle;

    [Header("続きから")]
    [SerializeField] private GameObject continueButton;

    private GameSaveManager saveManager;

    private const string SaveKey = "GameSaveData";

    public static bool IsTutorialEnabled { get; private set; }
    public static bool IsContinue { get; private set; }

    private void Start()
    {
        if (continueButton != null)
        {
            continueButton.SetActive(
                PlayerPrefs.HasKey(SaveKey)
            );
        }
    }

    public void StartGame()
    {
        // チュートリアルのON/OFFを保存
        IsTutorialEnabled =
            tutorialToggle != null &&
            tutorialToggle.isOn;

        // 新規ゲーム
        IsContinue = false;

        SceneManager.LoadScene("InGame");
    }

    public void ContinueGame()
    {
        // 続きから
        IsContinue = true;

        // 続きからの場合はセーブデータ側で判断する
        SceneManager.LoadScene("InGame");
    }
}