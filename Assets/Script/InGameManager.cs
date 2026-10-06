using UnityEngine;

public class InGameManager : MonoBehaviour
{
    [Header("プレイヤー")]
    [SerializeField] private Transform player;

    [Header("ゲーム開始位置")]
    [SerializeField] private Transform playerStartPoint;

    [Header("ゲームタイマー")]
    [SerializeField] private GameTimer gameTimer;

    [Header("セーブ")]
    [SerializeField] private GameSaveManager saveManager;

    private void Start()
    {
        if (GameStartManager.IsTutorialEnabled)
            return;

        if (GameStartManager.IsContinue)
        {
            LoadSavedGame();
            return;
        }

        StartNormalGame();
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

    public void StartNormalGame()
    {
        Debug.Log("通常ゲーム開始");
        ResetPlayer();
        ResetGameTimer();
        // 通常ゲーム処理
    }
    private void ResetPlayer()
    {
        if (player == null || playerStartPoint == null)
            return;

        player.position = playerStartPoint.position;
        player.rotation = playerStartPoint.rotation;
    }

    private void ResetGameTimer()
    {
        if (gameTimer == null)
            return;

        gameTimer.ResetTimer();
    }

    public void SaveGame()
    {
        if (saveManager == null)
            return;

        saveManager.SaveGame(player, gameTimer);
    }

    private void LoadSavedGame()
    {
        if (saveManager == null || !saveManager.HasSaveData())
        {
            StartNormalGame();
            return;
        }

        GameSaveManager.SaveData data = saveManager.LoadGame();

        if (data == null)
        {
            StartNormalGame();
            return;
        }

        // プレイヤー座標を復元
        player.position = new Vector3(
            data.playerX,
            data.playerY,
            data.playerZ
        );

        // タイマーを復元
        gameTimer.SetElapsedTime(data.elapsedTime);

        Debug.Log("保存データからゲームを再開しました！");
    }
}