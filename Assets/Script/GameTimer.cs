using UnityEngine;
using TMPro;

public class GameTimer : MonoBehaviour
{
    [Header("タイマー表示")]
    [SerializeField] private TMP_Text timerText;

    private float elapsedTime = 0f;
    private bool timerStarted = false;
    public float ElapsedTime => elapsedTime;

    private void FixedUpdate()
    {
        if (!timerStarted)
            return;

        elapsedTime += Time.fixedDeltaTime;

        UpdateTimerText();
    }
    public void SetElapsedTime(float time)
    {
        elapsedTime = time;
        timerStarted = true;

        UpdateTimerText();
    }

    public void StartTimer()
    {
        if (timerStarted)
            return;

        timerStarted = true;
    }

    public void ResetTimer()//タイマーリセット(初期化)
    {
        elapsedTime = 0f;
        timerStarted = false;

        UpdateTimerText();
    }

    private void UpdateTimerText()
    {
        int minutes =
            Mathf.FloorToInt(elapsedTime / 60f);

        int seconds =
            Mathf.FloorToInt(elapsedTime % 60f);

        timerText.text =
            minutes.ToString("00") + ":" +
            seconds.ToString("00");
    }
}