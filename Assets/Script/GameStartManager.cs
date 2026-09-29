using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameStartManager : MonoBehaviour
{
    [Header("チュートリアル")]
    [SerializeField] private Toggle tutorialToggle;

    public static bool IsTutorialEnabled { get; private set; }
    public static bool IsContinue { get; private set; }

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