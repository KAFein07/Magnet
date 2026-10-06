using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    [Header("説明表示時間")]
    [SerializeField] private float explanationDisplayTime = 3f;

    private float explanationTimer = 0f;
    private bool waitingForNextStep = false;


    [Header("最後の温度説明表示時間")]
    [SerializeField] private float temperatureDisplayTime = 15f;

    [Header("ゲーム管理")]
    [SerializeField] private InGameManager inGameManager;


    // =========================
    // チュートリアルのステップ
    // =========================

    private enum TutorialStep
    {
        AD説明,
        磁力ON説明,
        壁登り説明,
        磁力切り替え説明,
        温度説明, 
        チュートリアル終了表示,
        終了
    }


    [Header("プレイヤー")]
    [SerializeField] private PlayerMove player;

    [Header("ゲームタイマー")]
    [SerializeField] private GameTimer gameTimer;

    [Header("説明画像")]
    [SerializeField] private GameObject adImage;
    [SerializeField] private GameObject magnetOnImage;
    [SerializeField] private GameObject wallClimbImage;
    [SerializeField] private GameObject magnetOffImage;
    [SerializeField] private GameObject temperatureImage;

    [Header("チュートリアル終了")]
    [SerializeField] private GameObject tutorialEndImage;
    [SerializeField] private TMP_Text countdownText;
    [SerializeField] private float tutorialEndDisplayTime = 5f;


    private TutorialStep currentStep;


    // =========================
    // 開始
    // =========================

    private void Start()
    {
        // チュートリアルをプレイしない場合
        if (!GameStartManager.IsTutorialEnabled)
        {
            // チュートリアル画像を全部非表示
            HideAllImages();

            // TutorialManagerを停止
            enabled = false;

            return;
        }

        // チュートリアル開始
        StartTutorial();
    }


    // =========================
    // 毎物理フレーム
    // =========================

    private void FixedUpdate()
    {
        if (!waitingForNextStep)
            return;

        explanationTimer += Time.fixedDeltaTime;

        float displayTime = explanationDisplayTime;

        // 温度説明は15秒
        if (currentStep == TutorialStep.温度説明)
        {
            displayTime = temperatureDisplayTime;
        }

        // チュートリアル終了表示は5秒
        if (currentStep == TutorialStep.チュートリアル終了表示)
        {
            displayTime = tutorialEndDisplayTime;

            // 残り秒数を計算
            int remainingTime =
                Mathf.CeilToInt(displayTime - explanationTimer);

            if (countdownText != null)
            {
                countdownText.text = remainingTime.ToString();
            }
        }

        if (explanationTimer >= displayTime)
        {
            Debug.Log("説明時間終了！");

            waitingForNextStep = false;
            explanationTimer = 0f;

            // カウントダウン表示を消す
            if (countdownText != null)
            {
                countdownText.text = "";
            }

            GoToNextTutorialStep();
        }
    }


    // =========================
    // チュートリアル開始
    // =========================

    private void StartTutorial()
    {
        // ゲームタイマーをリセット
        if (gameTimer != null)
        {
            gameTimer.ResetTimer();
        }

        // A / D だけ使用可能
        player.SetInputPermission(
            true,   // A/D
            false,  // W/S
            false,  // 極変更
            false   // 磁力ON/OFF
        );

        // 最初のステップ
        currentStep = TutorialStep.AD説明;

        // UIを更新
        UpdateTutorialUI();
    }


    // =========================
    // A / D入力通知
    // =========================

    public void OnADInput()
    {
        // A/D説明中ではない場合は無視
        if (currentStep != TutorialStep.AD説明)
            return;

        Debug.Log("A/D入力確認！");

        // A/Dを押した瞬間から3秒カウント
        if (!waitingForNextStep)
        {
            explanationTimer = 0f;
            waitingForNextStep = true;
        }
    }


    // =========================
    // 左クリック通知
    // =========================

    public void OnLeftClick()
    {
        // 磁力ON説明中ではない場合は無視
        if (currentStep != TutorialStep.磁力ON説明)
            return;

        Debug.Log("左クリック確認！");

        // 磁力ON/OFFを許可
        player.SetInputPermission(
            true,   // A/D
            false,  // W/S
            false,  // 極変更
            true    // 磁力ON/OFF
        );

        // 左クリックした瞬間から3秒カウント
        if (!waitingForNextStep)
        {
            explanationTimer = 0f;
            waitingForNextStep = true;
        }
    }

    // =========================
    // W / S入力通知
    // =========================

    public void OnWSInput()
    {
        if (currentStep != TutorialStep.壁登り説明)
            return;

        Debug.Log("W/S入力確認！");

        // 3秒カウント開始
        if (!waitingForNextStep)
        {
            explanationTimer = 0f;
            waitingForNextStep = true;
        }
    }


    // =========================
    // 次のステップへ
    // =========================

    private void GoToNextTutorialStep()
    {
        switch (currentStep)
        {
            // =========================
            // A/D説明
            // =========================

            case TutorialStep.AD説明:

                currentStep = TutorialStep.磁力ON説明;

                break;


            // =========================
            // 磁力ON説明
            // =========================

            case TutorialStep.磁力ON説明:

                currentStep = TutorialStep.壁登り説明;

                player.SetInputPermission(
                    true,   // A/D
                    true,   // W/S
                    false,  // 極変更
                    true    // 磁力ON/OFF
                );

                break;


            // =========================
            // 壁登り説明
            // =========================

            case TutorialStep.壁登り説明:

                currentStep = TutorialStep.磁力切り替え説明;

                // A/D と W/S はそのまま使用可能
                // 右クリック（極変更）を許可
                // 磁力ON/OFFも許可したまま
                player.SetInputPermission(
                    true,   // A/D
                    true,   // W/S
                    true,   // 極変更
                    true    // 磁力ON/OFF
                );

                break;


            // =========================
            // 磁力切り替え説明
            // =========================

            case TutorialStep.磁力切り替え説明:

                currentStep = TutorialStep.温度説明;

                player.SetInputPermission(
                    true,   // A/D
                    true,   // W/S
                    true,   // 極変更
                    true    // 磁力ON/OFF
                ); 
                if (!waitingForNextStep)
                {
                    explanationTimer = 0f;
                    waitingForNextStep = true;
                }

                break;


            // =========================
            // 温度説明
            // =========================

            case TutorialStep.温度説明:

                // 温度説明は15秒表示してから終了
                currentStep = TutorialStep.チュートリアル終了表示;

                player.SetInputPermission(
                    true,
                    true,
                    true,
                    true
                );
                if (!waitingForNextStep)
                {
                    explanationTimer = 0f;
                    waitingForNextStep = true;
                }

                break;
            case TutorialStep.チュートリアル終了表示:

                currentStep = TutorialStep.終了;

                player.SetInputPermission(
                    true,
                    true,
                    true,
                    true
                );

                // 通常ゲーム開始
                if (inGameManager != null)
                {
                    inGameManager.StartNormalGame();
                }

                break;

            default:
                return;
        }

        Debug.Log("次のチュートリアル：" + currentStep);

        UpdateTutorialUI();
    }

    public void OnRightClick()
    {
        if (currentStep != TutorialStep.磁力切り替え説明)
            return;

        Debug.Log("右クリック確認！");

        // 3秒カウント開始
        if (!waitingForNextStep)
        {
            explanationTimer = 0f;
            waitingForNextStep = true;
        }
    }


    // =========================
    // チュートリアルUI更新
    // =========================

    private void UpdateTutorialUI()
    {
        switch (currentStep)
        {
            // =========================
            // A/D説明
            // =========================

            case TutorialStep.AD説明:

                HideAllImages();

                if (adImage != null)
                    adImage.SetActive(true);

                break;

            // =========================
            // 磁力ON説明
            // =========================

            case TutorialStep.磁力ON説明:

                HideAllImages();

                if (magnetOnImage != null)
                    magnetOnImage.SetActive(true);

                break;

            // =========================
            // 壁登り説明
            // =========================

            case TutorialStep.壁登り説明:

                HideAllImages();

                if (wallClimbImage != null)
                    wallClimbImage.SetActive(true);

                break;



            // =========================
            // 磁力切り替え説明
            // =========================

            case TutorialStep.磁力切り替え説明:

                HideAllImages();

                if (magnetOffImage != null)
                    magnetOffImage.SetActive(true);

                break;



            // =========================
            // 温度説明
            // =========================

            case TutorialStep.温度説明:

                HideAllImages();

                if (temperatureImage != null)
                    temperatureImage.SetActive(true);

                break;

            case TutorialStep.チュートリアル終了表示:

                HideAllImages();

                if (tutorialEndImage != null)
                    tutorialEndImage.SetActive(true);

                // カウントダウン開始
                if (countdownText != null)
                    countdownText.text = "5";

                break;


            // =========================
            // 終了
            // =========================

            case TutorialStep.終了:

                HideAllImages();

                break;
        }
    }


    // =========================
    // 画像を全部非表示
    // =========================

    private void HideAllImages()
    {
        if (adImage != null)
            adImage.SetActive(false);

        if (magnetOnImage != null)
            magnetOnImage.SetActive(false);

        if (wallClimbImage != null)
            wallClimbImage.SetActive(false);

        if (magnetOffImage != null)
            magnetOffImage.SetActive(false);

        if (temperatureImage != null)
            temperatureImage.SetActive(false); 
        if (tutorialEndImage != null)
            tutorialEndImage.SetActive(false);
    }
}