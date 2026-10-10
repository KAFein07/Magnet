using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using System.Collections;

public class RankingManager : MonoBehaviour
{
    [Header("ランキング")]
    [SerializeField] private string gasUrl;
    [SerializeField] private GameObject rankingPanel;

    private bool isRankingOpen = false;

    [Header("ランキング表示")]
    [SerializeField] private TMP_Text rankingText;


    private void OnEnable()
    {
        StartCoroutine(GetRanking());
        if (rankingPanel != null)
        {
            rankingPanel.SetActive(false);
        }
    }

    public void ToggleRanking()
    {
        StartCoroutine(GetRanking());
        isRankingOpen = !isRankingOpen;

        if (rankingPanel != null)
        {
            rankingPanel.SetActive(isRankingOpen);
            if (isRankingOpen)
            {
                AudioManager.Instance.PlayDecisionSE();
            }
            else
            {
                AudioManager.Instance.PlayCancelSE();
            }
        }
    }

    private IEnumerator GetRanking()
    {
        // GASへアクセス
        using (UnityWebRequest request =
            UnityWebRequest.Get(gasUrl))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(
                    "ランキング取得失敗：" +
                    request.error
                );

                rankingText.text =
                    "ランキングを取得できませんでした。";

                yield break;
            }

            // GASから受け取ったJSON
            string json = request.downloadHandler.text;

            Debug.Log("ランキング取得：" + json);

            // JSONを解析
            RankingData[] ranking =
                JsonHelper.FromJson<RankingData>(json);

            // 表示
            ShowRanking(ranking);
        }
    }

    private void ShowRanking(RankingData[] ranking)
    {
        rankingText.text = "";

        for (int i = 0; i < ranking.Length; i++)
        {
            int minutes =
                Mathf.FloorToInt(ranking[i].time / 60f);

            int seconds =
                Mathf.FloorToInt(ranking[i].time % 60f);

            rankingText.text +=
                (i + 1) + ". " +
                ranking[i].name + "  " +
                minutes.ToString("00") + ":" +
                seconds.ToString("00") +
                "\n";
        }
    }

    [System.Serializable]
    public class RankingData
    {
        public string name;
        public float time;
    }
}

public static class JsonHelper
{
    public static T[] FromJson<T>(string json)
    {
        string wrappedJson =
            "{\"items\":" + json + "}";

        Wrapper<T> wrapper =
            JsonUtility.FromJson<Wrapper<T>>(wrappedJson);

        return wrapper.items;
    }

    [System.Serializable]
    private class Wrapper<T>
    {
        public T[] items;
    }
}