//using UnityEngine;
//using UnityEngine.SceneManagement;

//public class SceneChangerByDistance : MonoBehaviour
//{
//    // センサーの値を判断するための閾値
//    public float thresholdDown = 20f;
//    public float thresholdUp = 100f;
//    private string nextSceneName = "tutorial";

//    // センサー値が安定するまでの待機時間（秒）
//    public float startupDelay = 2.0f;

//    // センサーの状態を管理
//    private enum State
//    {
//        Initializing, // 初期化中
//        Waiting,      // 待機中
//        Down,         // 下がっている
//        Up            // 上がっている
//    }

//    private State currentState = State.Initializing;
//    private float startTime;

//    void Start()
//    {
//        // ゲーム開始時の時間を記録
//        startTime = Time.time;
//    }

//    void Update()
//    {
//        // 現在の距離を取得
//        float currentDistance = DistanceSensorReader1.distance;

//        // 初期化状態のチェック
//        if (currentState == State.Initializing)
//        {
//            // 指定した時間が経過したら、待機状態に移行
//            if (Time.time - startTime > startupDelay)
//            {
//                currentState = State.Waiting;
//                Debug.Log("初期化完了。腹筋ローラーの動きを待機しています。");
//            }
//            // 初期化中は以下のロジックをスキップ
//            return;
//        }

//        // ------------------ ここから既存のロジック ------------------
//        switch (currentState)
//        {
//            case State.Waiting:
//                if (currentDistance < thresholdDown)
//                {
//                    currentState = State.Down;
//                    Debug.Log("下がった");
//                }
//                break;

//            case State.Down:
//                if (currentDistance > thresholdUp)
//                {
//                    Debug.Log("あがった");
//                    currentState = State.Up;
//                    Debug.Log("腹筋ローラーの1回の動きを検知しました。シーンを切り替えます。");
//                    SceneManager.LoadScene(nextSceneName);
//                }
//                break;
//        }
//    }
//}















//using UnityEngine;
//using UnityEngine.SceneManagement;

//public class SceneChangerByDistance : MonoBehaviour
//{
//    // センサーの値を判断するための閾値（差分）
//    [Header("しきい値(基準からの差)")]
//    public float downOffset = -20f;   // 基準より20cm下がったらDown
//    public float upOffset = 80f;      // 基準より80cm上がったらUp
//    public float baseDistance = 60f;   // 起動時の基準距離

//    [Header("その他設定")]
//    public string nextSceneName = "tutorial";
//    public float startupDelay = 2.0f; // センサー値が安定するまでの待機時間

//    private enum State { Initializing, Waiting, Down }
//    private State currentState = State.Initializing;

//    private float startTime;

//    void Start()
//    {
//        startTime = Time.time;
//        Debug.Log($"基準距離を記録しました: {baseDistance}");
//    }

//    void Update()
//    {
//        float currentDistance = DistanceSensorReader1.distance;
//        float diff = currentDistance - baseDistance;   // 基準との差分
//        //Debug.Log($"state:{currentState} dist:{currentDistance} diff:{diff}");

//        switch (currentState)
//        {
//            case State.Initializing:
//                // センサー値が安定するまで待つ
//                if (Time.time - startTime > startupDelay)
//                {
//                    currentState = State.Waiting;
//                    Debug.Log("初期化完了。腹筋ローラーの動きを待機しています。");
//                }
//                break;

//            case State.Waiting:
//                if (diff < downOffset)
//                {
//                    currentState = State.Down;
//                    Debug.Log("ローラーが下がりました。");
//                }
//                break;

//            case State.Down:
//                if (diff > upOffset)
//                {
//                    Debug.Log("ローラーが上がりました。シーンを切り替えます。");
//                    SceneManager.LoadScene(nextSceneName);
//                }
//                break;

//        }
//    }
//}

















using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 距離センサーの値を平均化し、
/// 腹筋ローラーが「下がって→戻った」動作を検知してシーンを切り替える
/// </summary>
public class SceneChangerByDistance : MonoBehaviour
{
    [Header("センサー設定")]
    public float baseDistance = 60f;       // 起動時の基準距離（cm）
    public float downOffset = -20f;        // 基準より下がったらDown
    public float upOffset = 10f;           // Downのあと、基準より上がったらUp
    public float startupDelay = 2.0f;      // センサー安定待ち時間

    [Header("ノイズ対策")]
    [Tooltip("移動平均を取るサンプル数")]
    public int sampleCount = 20;

    [Header("シーン名")]
    public string nextSceneName = "tutorial";

    private enum State { Initializing, Waiting, Down }
    private State currentState = State.Initializing;

    private float startTime;

    // ==== 平均計算用 ====
    private float[] history;
    private int index = 0;
    private bool historyFilled = false;
    private bool justEnteredWaiting = false;

    void Start()
    {
        startTime = Time.time;

        history = new float[sampleCount];
        for (int i = 0; i < sampleCount; i++) history[i] = baseDistance;

        Debug.Log($"基準距離を記録しました: {baseDistance}");
    }

    void Update()
    {
        // センサーから現在の距離を取得
        float raw = DistanceSensorReader1.distance;

        // 履歴に格納（リングバッファ）
        history[index] = raw;
        index = (index + 1) % sampleCount;
        if (index == 0) historyFilled = true;

        // 平均値を計算
        int validCount = historyFilled ? sampleCount : index;
        float sum = 0f;
        for (int i = 0; i < validCount; i++) sum += history[i];
        float avg = sum / validCount;

        float diff = avg - baseDistance; // 基準との差分

        //Debug.Log($"raw:{raw:F1} avg:{avg:F1} diff:{diff:F1} state:{currentState}");

        switch (currentState)
        {
            case State.Initializing:
                if (Time.time - startTime > startupDelay)
                {
                    currentState = State.Waiting;
                    justEnteredWaiting = true;
                    Debug.Log("初期化完了。ローラーの動きを待っています。");
                }
                break;

            case State.Waiting:
                if (justEnteredWaiting)
                {
                    justEnteredWaiting = false;
                    break; // このフレームは判定しない
                }

                if (diff < downOffset)
                {
                    currentState = State.Down;
                    Debug.Log("ローラーが下がりました。");
                }
                break;

            case State.Down:
                // Down のあと平均値が Up しきい値を超えたらシーン切替
                if (diff > upOffset)
                {
                    Debug.Log("ローラーが上がりました。シーンを切り替えます。");
                    SceneManager.LoadScene(nextSceneName);
                }
                break;
        }
    }
}
