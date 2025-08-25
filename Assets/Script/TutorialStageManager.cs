using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TutorialStageManager : MonoBehaviour
{
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

    [Header("ステップ設定")]
    [SerializeField] private TutorialStep[] tutorialSteps;

    private int currentStep = 0;
    private bool waitingForInput = false;

    void Start()
    {
        if (tutorialSteps.Length > 0)
        {
            ShowStep(0);
        }
    }

    void Update()
    {
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
        Debug.Log("currentStep" + currentStep);
        Debug.Log("steps.Length" + tutorialSteps.Length);
        if (currentStep < tutorialSteps.Length)
        {
            ShowStep(currentStep);
        }
        else
        {
            Debug.Log("Tutorial Finished → GameStart!");
            tutorialImage.gameObject.SetActive(false);

            // TODO: ここで本編のゲーム開始処理を呼ぶ
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
