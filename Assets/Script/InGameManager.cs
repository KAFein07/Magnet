using UnityEngine;
using System.Collections.Generic;

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

    [Header("使用可能なエフェクト")]
    [SerializeField]
    private List<EffectData> effects = new List<EffectData>();

    private const string EquippedKey = "EquippedEffect";

    private PlayerMove playerMove;

    private void Awake()
    {
        // プレイヤーの移動スクリプトを取得
        if (player != null)
        {
            playerMove = player.GetComponent<PlayerMove>();

            if (playerMove == null)
            {
                Debug.LogError(
                    "プレイヤーにPlayerMoveが見つかりません。"
                );
            }
        }
        else
        {
            Debug.LogError(
                "InGameManagerのPlayer参照が設定されていません。"
            );
        }
    }

    private void Start()
    {
        // チュートリアルでも装備を反映
        ApplyEquippedEffect();

        if (GameStartManager.IsTutorialEnabled)
            return;

        if (GameStartManager.IsContinue)
        {
            LoadSavedGame();
            return;
        }

        StartNormalGame();
    }

    // 保存されている装備エフェクトをプレイヤーに適用
    public void ApplyEquippedEffect()
    {
        if (playerMove == null)
            return;

        string equippedId =
            PlayerPrefs.GetString(EquippedKey, "");

        EffectData data = effects.Find(effect =>
            effect != null &&
            effect.effectId == equippedId
        );

        playerMove.SetEffect(data);
    }

    // 通常ゲームを開始
    public void StartNormalGame()
    {
        Debug.Log("通常ゲーム開始");

        ApplyEquippedEffect();
        ResetPlayer();
        ResetGameTimer();
    }

    // プレイヤーを開始位置に戻す
    private void ResetPlayer()
    {
        if (player == null || playerStartPoint == null)
            return;

        player.position = playerStartPoint.position;
        player.rotation = playerStartPoint.rotation;
    }

    // タイマーをリセット
    private void ResetGameTimer()
    {
        if (gameTimer == null)
            return;

        gameTimer.ResetTimer();
    }

    // ゲームを保存
    public void SaveGame()
    {
        if (saveManager == null)
            return;

        saveManager.SaveGame(player, gameTimer);
    }

    // セーブデータから再開
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
        if (player != null)
        {
            player.position = new Vector3(
                data.playerX,
                data.playerY,
                data.playerZ
            );
        }

        // タイマーを復元
        if (gameTimer != null)
        {
            gameTimer.SetElapsedTime(data.elapsedTime);
        }

        // 再開時にも装備を反映
        ApplyEquippedEffect();

        Debug.Log("保存データからゲームを再開しました！");
    }
}