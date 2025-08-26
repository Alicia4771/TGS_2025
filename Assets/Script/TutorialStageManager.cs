using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;


public class TutorialStageManager : MonoBehaviour
{
    private string nextSceneName = "game";

    public enum WaitType
    {
        None,   // 時間経過で進む
        Charge, // チャージ動作待ち
        Jump    // ジャンプ動作待ち
    }

    [System.Serializable]
    public class TutorialStep
    {
        public Sprite sprite;       // 表示する画像
        public float duration = 0f; // 時間で進む場合の待機秒
        public WaitType waitType = WaitType.None;
    }

    [Header("UI")]
    [SerializeField] private Image tutorialImage;

    [Header("難易度表示用の画像")]
    [SerializeField] private Image nanidoImage;

    [Header("普通用の画像")]
    [SerializeField] private Image nomalImage;

    [Header("簡単用の画像")]
    [SerializeField] private Image easyImage;

    [Header("ステップ設定")]
    [SerializeField] private TutorialStep[] tutorialSteps;

    private int currentStep = 0;
    private bool waitingForInput = false;

    // 距離センサーの値
    private float distance_value;
    private float normalDistance = 40f;    // 普通難易度までの距離
    private float easyDistance = 65f;     // 簡単難易度までの距離
    private float standardDistance = 90f;     // 基準距離

    // 難易度選択用フラグ
    bool sceneChanged = false;

    bool isEasyCandidate;
    bool isNormalCandidate;

    // 2. 基準距離より大きいかを判別
    bool aboveStandard;

    // 2. 難易度選択中かを判別
    bool isNanido = false;

    void Start()
    {
        // チュートリアル説明用の文字画像の初期化
        tutorialImage.gameObject.SetActive(true);

        // 難易度説明用の文字画像の初期化
        nanidoImage.enabled = false;
        nomalImage.enabled = false;
        easyImage.enabled = false;
        sceneChanged = false;
        isNanido = false;

        if (tutorialSteps.Length > 0)
        {
            ShowStep(0);
        }
    }

    void Update()
    {
        // 距離センサーからの値を取得
        distance_value = DistanceSensorReader1.distance;

        // 1. 距離が簡単距離より大きいか、普通距離より大きいかを判別
        isEasyCandidate = distance_value > easyDistance;
        isNormalCandidate = distance_value > normalDistance;

        // 2. 基準距離より大きいかを判別
        aboveStandard = distance_value > standardDistance;

        // 入力待ち中はここで判定
        if (waitingForInput)
        {
            if (tutorialSteps[currentStep].waitType == WaitType.Charge)
            {
                if (PlayerMoveFromSensor.Charged) // ← プレイヤー側から静的フラグをもらう
                {
                    waitingForInput = false;
                    NextStep();
                }
            }
            else if (tutorialSteps[currentStep].waitType == WaitType.Jump)
            {
                if (PlayerMoveFromSensor.Jumped) // ← プレイヤー側から静的フラグをもらう
                {
                    waitingForInput = false;
                    NextStep();
                    PlayerMoveFromSensor.Jumped = false;

                }
            }
        }

        // 難易度を選択する
        if (isNanido)
        {
            Debug.Log("nanido選択");
            // 3. 難易度を決定してシーン切り替え
            if (!sceneChanged)
            {
                if (isEasyCandidate && aboveStandard)
                {
                    Debug.Log("簡単モードを選択");
                    nanidoImage.enabled = false;
                    easyImage.enabled = true;
                    PlayerMoveFromSensor.move_diameter = 2.0f;
                    //SceneManager.LoadScene(nextSceneName);
                    sceneChanged = true;
                    StartCoroutine(LoadSceneWithDelay(3f)); // ← 3秒後にシーン切り替え
                }
                else if (isNormalCandidate && aboveStandard)
                {
                    Debug.Log("普通モードを選択");
                    nanidoImage.enabled = false;
                    nomalImage.enabled = true;
                    PlayerMoveFromSensor.move_diameter = 1.0f;
                    //SceneManager.LoadScene(nextSceneName);
                    sceneChanged = true;
                    StartCoroutine(LoadSceneWithDelay(3f)); // ← 3秒後にシーン切り替え
                }
                //else
                //{
                //    Debug.Log("ハードモードを選択");
                //    PlayerMoveFromSensor.move_diameter = 0.5f;
                //    SceneManager.LoadScene(nextSceneName);
                //    sceneChanged = true;
                //}
            }
        }
    }

    //private void ShowStep(int step)
    //{
    //    if (step < 0 || step >= steps.Length) return;

    //    TutorialStep s = steps[step];
    //    tutorialImage.sprite = s.sprite;
    //    tutorialImage.preserveAspect = true;

    //    Debug.Log($"Show step: {step}, sprite={s.sprite?.name ?? "null"}");

    //    Debug.Log("Show step: " + step);

    //    if (s.waitType == WaitType.None)
    //    {
    //        if (s.duration > 0)
    //        {
    //            StartCoroutine(AutoStepRoutine(s.duration));
    //        }
    //    }
    //    else
    //    {
    //        waitingForInput = true;
    //    }
    //}

    // 時間が立ってからシーンを切り替える用のコルーチン
    private IEnumerator LoadSceneWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(nextSceneName);
    }



    private Coroutine autoStepRoutine;



    private void ShowStep(int step)
    {
        if (step < 0 || step >= tutorialSteps.Length) return;

        TutorialStep s = tutorialSteps[step];
        tutorialImage.sprite = s.sprite;
        tutorialImage.preserveAspect = true;

        Debug.Log("Show step: " + step);

        // 古いコルーチンを止める
        if (autoStepRoutine != null)
        {
            StopCoroutine(autoStepRoutine);
            autoStepRoutine = null;
        }

        if (s.waitType == WaitType.None && s.duration > 0)
        {
            autoStepRoutine = StartCoroutine(AutoStepRoutine(s.duration));
        }
        else
        {
            waitingForInput = true;
        }
    }


    private void NextStep()
    {
        currentStep++;
        //Debug.Log("currentStep" + currentStep);
        //Debug.Log("steps.Length" + tutorialSteps.Length);
        if (currentStep < tutorialSteps.Length)
        {
            ShowStep(currentStep);
        }
        else
        {
            //Debug.Log("Tutorial Finished → GameStart!");
            tutorialImage.gameObject.SetActive(false);

            //難易度選択が動かなかった時の保険
            //SceneManager.LoadScene(nextSceneName);



            // TODO: ここで本編のゲーム開始処理を呼ぶ
            nanidoImage.enabled = true;
            isNanido = true;


            
        }
    }

    private IEnumerator AutoStepRoutine(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        if (!waitingForInput) // 入力待ちに切り替わってなければ進む
        {
            NextStep();
        }
    }
}
