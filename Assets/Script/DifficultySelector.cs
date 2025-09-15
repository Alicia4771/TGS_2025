//using UnityEngine;
//using UnityEngine.SceneManagement;

///// <summary>
///// 腹筋ローラーの動きから難易度を決定し、シーンを切り替える
///// </summary>
//public class DifficultySelector : MonoBehaviour
//{
//    [Header("センサー設定")]
//    public float baseDistance = 60f;
//    public float downOffset = -20f;      // 下方向しきい値
//    public float easyUpOffset = 10f;     // 簡単モード判定の上しきい値
//    public float normalUpOffset = 40f;   // 普通モード判定の上しきい値
//    public float startupDelay = 2f;

//    [Header("ノイズ対策")]
//    public int sampleCount = 20;

//    [Header("シーン名")]
//    public string easyScene = "EasyScene";
//    public string normalScene = "NormalScene";

//    private enum State { Initializing, Waiting, Down }
//    private State currentState = State.Initializing;

//    private float startTime;
//    private float[] history;
//    private int index;
//    private bool historyFilled;
//    private bool justEnteredWaiting;

//    // 判定用フラグ
//    private bool normalFlag = false;
//    private bool easyFlag = false;

//    void Start()
//    {
//        startTime = Time.time;
//        history = new float[sampleCount];
//        for (int i = 0; i < sampleCount; i++) history[i] = baseDistance;
//    }

//    void Update()
//    {
//        float raw = DistanceSensorReader1.distance;

//        // ---- 移動平均 ----
//        history[index] = raw;
//        index = (index + 1) % sampleCount;
//        if (index == 0) historyFilled = true;

//        int valid = historyFilled ? sampleCount : index;
//        float sum = 0f;
//        for (int i = 0; i < valid; i++) sum += history[i];
//        float avg = sum / valid;
//        float diff = avg - baseDistance;

//        switch (currentState)
//        {
//            case State.Initializing:
//                if (Time.time - startTime > startupDelay)
//                {
//                    currentState = State.Waiting;
//                    justEnteredWaiting = true;
//                }
//                break;

//            case State.Waiting:
//                if (justEnteredWaiting)
//                {
//                    justEnteredWaiting = false;
//                    break;
//                }
//                if (diff < downOffset)
//                {
//                    currentState = State.Down;
//                    normalFlag = false;
//                    easyFlag = false;
//                    Debug.Log("ローラーが下がりました");
//                }
//                break;

//            case State.Down:
//                if (diff < normalUpOffset)
//                {
//                    normalFlag = true;
//                }
//                else if (diff < easyUpOffset)
//                {
//                    easyFlag = true;
//                }

//                // 戻ってきたタイミングでシーン切り替え
//                if (diff > easyUpOffset)
//                {
//                    if (normalFlag)
//                    {
//                        Debug.Log("普通モードに決定");
//                        SceneManager.LoadScene(normalScene);
//                    }
//                    else if (easyFlag)
//                    {
//                        Debug.Log("簡単モードに決定");
//                        SceneManager.LoadScene(easyScene);
//                    }
//                }
//                break;
//        }
//    }
//}












using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 腹筋ローラーの動きから難易度を決定し、シーンを切り替える
/// </summary>
public class DifficultySelector : MonoBehaviour
{
    [Header("センサー設定")]
    public float baseDistance = 60f;
    public float downOffset = -20f;      // 基準より下がったらDown
    public float easyUpOffset = 10f;     // 簡単モードの戻り閾値
    public float normalUpOffset = 40f;   // 普通モードの戻り閾値
    public float startupDelay = 2f;      // センサー起動後の待機時間
    public float readyTolerance = 5f;    // 基準付近とみなす許容値
    public float readyHoldTime = 1f;     // その位置を何秒キープしたらOKか

    [Header("ノイズ対策")]
    public int sampleCount = 20;

    [Header("シーン名")]
    public string easyScene = "EasyScene";
    public string normalScene = "NormalScene";

    private enum State { Initializing, ReadyCheck, Waiting, Down }
    private State currentState = State.Initializing;

    private float startTime;
    private float[] history;
    private int index;
    private bool historyFilled;

    // Ready判定用
    private float readyStart = 0f;

    // 判定フラグ
    private bool normalFlag = false;
    private bool easyFlag = false;

    void Start()
    {
        startTime = Time.time;
        history = new float[sampleCount];
        for (int i = 0; i < sampleCount; i++) history[i] = baseDistance;
    }

    void Update()
    {
        float raw = DistanceSensorReader1.distance;

        // ---- 移動平均 ----
        history[index] = raw;
        index = (index + 1) % sampleCount;
        if (index == 0) historyFilled = true;

        int valid = historyFilled ? sampleCount : index;
        float sum = 0f;
        for (int i = 0; i < valid; i++) sum += history[i];
        float avg = sum / valid;
        float diff = avg - baseDistance;

        switch (currentState)
        {
            case State.Initializing:
                // センサー安定待ち
                if (Time.time - startTime > startupDelay)
                {
                    currentState = State.ReadyCheck;
                    readyStart = 0f;
                    Debug.Log("基準位置チェックを開始");
                }
                break;

            case State.ReadyCheck:
                // 基準距離 ±readyTolerance の範囲にあるか？
                if (Mathf.Abs(diff) <= readyTolerance)
                {
                    if (readyStart == 0f) readyStart = Time.time;
                    if (Time.time - readyStart >= readyHoldTime)
                    {
                        currentState = State.Waiting;
                        Debug.Log("ローラーが基準付近に安定。難易度選択開始");
                    }
                }
                else
                {
                    readyStart = 0f; // 範囲外に出たらリセット
                }
                break;

            case State.Waiting:
                if (diff < downOffset)
                {
                    currentState = State.Down;
                    normalFlag = false;
                    easyFlag = false;
                    Debug.Log("ローラーが下がりました");
                }
                break;

            case State.Down:
                // 戻り動作のしきい値をチェック
                if (diff < normalUpOffset)
                {
                    normalFlag = true; // 普通モード優先
                }
                else if (diff > easyUpOffset)
                {
                    easyFlag = true;
                }

                // 「上がってきた」状態になったらシーン切替
                if (diff < easyUpOffset)
                {
                    if (normalFlag)
                    {
                        Debug.Log("普通モードに決定");
                        SceneManager.LoadScene(normalScene);
                    }
                    else if (easyFlag)
                    {
                        Debug.Log("簡単モードに決定");
                        SceneManager.LoadScene(easyScene);
                    }
                }
                break;
        }
    }
}
