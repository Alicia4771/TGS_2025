using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChangerByDistance : MonoBehaviour
{
    // センサーの値を判断するための閾値
    public float thresholdDown = 20f;
    public float thresholdUp = 100f;
    public string nextSceneName = "game";

    // センサー値が安定するまでの待機時間（秒）
    public float startupDelay = 2.0f;

    // センサーの状態を管理
    private enum State
    {
        Initializing, // 初期化中
        Waiting,      // 待機中
        Down,         // 下がっている
        Up            // 上がっている
    }

    private State currentState = State.Initializing;
    private float startTime;

    void Start()
    {
        // ゲーム開始時の時間を記録
        startTime = Time.time;
    }

    void Update()
    {
        // 現在の距離を取得
        float currentDistance = DistanceSensorReader1.distance;

        // 初期化状態のチェック
        if (currentState == State.Initializing)
        {
            // 指定した時間が経過したら、待機状態に移行
            if (Time.time - startTime > startupDelay)
            {
                currentState = State.Waiting;
                Debug.Log("初期化完了。腹筋ローラーの動きを待機しています。");
            }
            // 初期化中は以下のロジックをスキップ
            return;
        }

        // ------------------ ここから既存のロジック ------------------
        switch (currentState)
        {
            case State.Waiting:
                if (currentDistance < thresholdDown)
                {
                    currentState = State.Down;
                    Debug.Log("下がった");
                }
                break;

            case State.Down:
                if (currentDistance > thresholdUp)
                {
                    Debug.Log("あがった");
                    currentState = State.Up;
                    Debug.Log("腹筋ローラーの1回の動きを検知しました。シーンを切り替えます。");
                    SceneManager.LoadScene(nextSceneName);
                }
                break;
        }
    }
}