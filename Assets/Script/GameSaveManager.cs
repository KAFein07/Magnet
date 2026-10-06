using UnityEngine;

public class GameSaveManager : MonoBehaviour
{
    // 保存データが存在するか確認するためのキー
    private const string SaveKey = "GameSaveData";


    // =========================
    // 保存データ
    // =========================

    [System.Serializable]
    public class SaveData
    {
        // プレイヤー座標
        public float playerX;
        public float playerY;
        public float playerZ;

        // プレイ時間
        public float elapsedTime;
    }


    // =========================
    // ゲームを保存
    // =========================

    public void SaveGame(Transform player, GameTimer gameTimer)
    {
        if (player == null || gameTimer == null)
            return;

        SaveData data = new SaveData();

        // プレイヤー座標を保存
        data.playerX = player.position.x;
        data.playerY = player.position.y;
        data.playerZ = player.position.z;

        // タイマーを保存
        data.elapsedTime = gameTimer.ElapsedTime;

        // JSONに変換
        string json = JsonUtility.ToJson(data);

        // 保存
        PlayerPrefs.SetString(SaveKey, json);
        PlayerPrefs.Save();

        Debug.Log("ゲームを保存しました！");
    }


    // =========================
    // 保存データがあるか
    // =========================

    public bool HasSaveData()
    {
        return PlayerPrefs.HasKey(SaveKey);
    }


    // =========================
    // 保存データを読み込む
    // =========================

    public SaveData LoadGame()
    {
        if (!HasSaveData())
            return null;

        string json = PlayerPrefs.GetString(SaveKey);

        return JsonUtility.FromJson<SaveData>(json);
    }


    // =========================
    // 保存データを削除
    // =========================

    public void DeleteSaveData()
    {
        PlayerPrefs.DeleteKey(SaveKey);
        PlayerPrefs.Save();

        Debug.Log("保存データを削除しました！");
    }
}