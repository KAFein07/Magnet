using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using static RankingManager;
using static UnityEngine.LowLevelPhysics2D.PhysicsLayers;

public class GameClearManager : MonoBehaviour
{
    [Header("リザルト画面")]
    [SerializeField] private GameObject resultPanel;

    [Header("クリアタイム")]
    [SerializeField] private TMP_Text clearTimeText;

    [Header("ゲームタイマー")]
    [SerializeField] private GameTimer gameTimer;

    [Header("プレイヤー")]
    [SerializeField] private PlayerMove player;

    [Header("ランキング")]
    [SerializeField] private string gasUrl;
    [SerializeField] private string playerName = "Player";

    [Header("プレイヤー名")]
    [SerializeField] private TMP_InputField playerNameInput;


    private bool isCleared = false;
    private bool SendRank = false;

    private const string TrophyKey = "PlayerTrophyCount";

    // 所持トロフィー数
    public static int TrophyCount
    {
        get { return PlayerPrefs.GetInt(TrophyKey, 0); }
    }

    // トロフィーを1個獲得
    private void AddTrophy()
    {
        int count = TrophyCount + 1;

        PlayerPrefs.SetInt(TrophyKey, count);
        PlayerPrefs.Save();

        Debug.Log("トロフィー獲得！ 所持数：" + count);
    }

    private void Start()
    {
        // 最初はリザルト画面を非表示
        if (resultPanel != null)
        {
            resultPanel.SetActive(false);
        }
    }

    public void ReturnToTitle()
    {
        if (SendRank)
            return;
        SendRank = true;
        AudioManager.Instance.PlayClearSE();
        // 入力されたプレイヤー名を取得
        string playerName = "";

        if (playerNameInput != null)
        {
            playerName = playerNameInput.text;
        }

        // 名前が空の場合
        if (string.IsNullOrWhiteSpace(playerName))
        {
            playerName = "noname";
        }

        // クリアタイムを取得
        float clearTime = 0f;

        if (gameTimer != null)
        {
            clearTime = gameTimer.ElapsedTime;
        }

        // ランキング送信
        StartCoroutine(SendRanking(playerName, clearTime));
    }

    //
    public void GameClear()
    {
        // すでにクリア済みなら何もしない
        if (isCleared)
            return;

        SendRank=false;
        isCleared = true;

        AddTrophy();
        // クリアしたので、続きから用のセーブデータを削除
        PlayerPrefs.DeleteKey("GameSaveData");
        PlayerPrefs.Save();

        Debug.Log("クリアしたためセーブデータを削除しました！");

        // クリアタイムを取得
        float clearTime = 0f;

        if (gameTimer != null)
        {
            clearTime = gameTimer.ElapsedTime;
        }




        // クリアタイムを表示
        if (clearTimeText != null)
        {
            int minutes = Mathf.FloorToInt(clearTime / 60f);
            int seconds = Mathf.FloorToInt(clearTime % 60f);

            clearTimeText.text =
                minutes.ToString("00") + ":" +
                seconds.ToString("00");
        }

        // リザルト画面を表示
        if (resultPanel != null)
        {
            resultPanel.SetActive(true);
        }

        // プレイヤー操作を停止
        if (player != null)
        {
            player.SetInputPermission(false, false, false, false);
        }

        // ゲームを停止
        Time.timeScale = 0f;

        Debug.Log("ゲームクリア！");
        Debug.Log("クリアタイム：" + clearTime);
    }


    public static bool TrySpendTrophies(int amount)
    {
        if (amount < 0)
            return false;

        int currentCount = TrophyCount;

        // 所持数が足りなければ購入できない
        if (currentCount < amount)
            return false;

        // トロフィーを消費して保存
        PlayerPrefs.SetInt(TrophyKey, currentCount - amount);
        PlayerPrefs.Save();

        return true;
    }


    private IEnumerator SendRanking(string playerName, float clearTime)
    {
        // GASへ送るデータ
        RankingData data = new RankingData();

        data.playerName = playerName;
        data.clearTime = clearTime;

        // JSONに変換
        string json = JsonUtility.ToJson(data);

        // JSONをバイトデータに変換
        byte[] body = System.Text.Encoding.UTF8.GetBytes(json);

        // POSTリクエストを作成
        using (UnityWebRequest request =
            new UnityWebRequest(gasUrl, "POST"))
        {
            request.uploadHandler =
                new UploadHandlerRaw(body);

            request.downloadHandler =
                new DownloadHandlerBuffer();

            request.SetRequestHeader(
                "Content-Type",
                "application/json"
            );

            Debug.Log("ランキング送信開始！");
            Debug.Log("名前：" + playerName);
            Debug.Log("タイム：" + clearTime);

            yield return request.SendWebRequest();

            // 送信結果を確認
            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(
                    "ランキング送信失敗：" +
                    request.error
                );

                yield break;
            }

            Debug.Log("ランキング送信成功！");
            Debug.Log("GASからの返答：" + request.downloadHandler.text);

            // 送信が終わってからタイトルへ戻る
            Time.timeScale = 1f;
            SceneManager.LoadScene("Title");
        }
    }



    [System.Serializable]
    private class RankingData
    {
        public string playerName;
        public float clearTime;
    }
}