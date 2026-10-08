using UnityEngine;

public class GoalPos : MonoBehaviour
{
    [Header("ゲームクリア管理")]
    [SerializeField] private GameClearManager gameClearManager;

    private void OnTriggerEnter(Collider other)
    {
        // プレイヤー以外は無視
        if (!other.CompareTag("Player"))
            return;

        // GameClearManagerが設定されていなければ何もしない
        if (gameClearManager == null)
            return;

        // ゲームクリア
        gameClearManager.GameClear();
    }
}