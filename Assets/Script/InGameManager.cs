using UnityEngine;

public class InGameManager : MonoBehaviour
{
    private void Start()
    {
        if (GameStartManager.IsContinue)
        {
            ContinueGame();
        }
        else
        {
            StartNewGame();
        }
    }

    private void StartNewGame()
    {
        Debug.Log("新しくゲーム開始");

        if (GameStartManager.IsTutorialEnabled)
        {
            StartTutorial();
        }
        else
        {
            StartNormalGame();
        }
    }

    private void ContinueGame()
    {
        Debug.Log("続きから開始");

        // セーブデータを読み込む
    }

    private void StartTutorial()
    {
        Debug.Log("チュートリアル開始");

        // チュートリアル処理
    }

    private void StartNormalGame()
    {
        Debug.Log("通常ゲーム開始");

        // 通常ゲーム処理
    }
}