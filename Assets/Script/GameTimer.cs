using UnityEngine;
using TMPro;

public class GameTimer : MonoBehaviour
{
    [Header("タイマー表示")]
    [SerializeField] private TMP_Text timerText;

    private float elapsedTime = 0f;
    private bool timerStarted = false;

    private void FixedUpdate()
    {
        if (!timerStarted)
            return;

        elapsedTime += Time.fixedDeltaTime;

        UpdateTimerText();
    }

    public void StartTimer()
    {
        if (timerStarted)
            return;

        timerStarted = true;
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