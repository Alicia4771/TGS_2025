


//using UnityEngine;
//using TMPro;
//using UnityEngine.SceneManagement; // シーン切り替えに必要

//public class Timer : MonoBehaviour
//{
//    [SerializeField]
//    private TextMeshProUGUI TimerText;

//    [SerializeField, Tooltip("制限時間(秒)")]
//    private float time_limit = 60f;

//    private float currentTime;
//    private bool isTimer = false;
//    private bool isGoal = false; // ゴール済みフラグ

//    // ゴールした時の効果音
//    public AudioSource goalAudioSource; // AudioSourceコンポーネントへの参照
//    public AudioClip goalSoundEffect;   // 再生したいオーディオクリップ

//    // ゴールに到達したことを示すフラグ
//    private bool hasReachedGoal = false;

//    void Start()
//    {
//        currentTime = time_limit;
//        TimerText.text = currentTime.ToString("F0");
//        startTimer();
//    }

//    void Update()
//    {
//        if (isTimer && !isGoal)
//        {
//            currentTime -= Time.deltaTime;

//            if (currentTime <= 0)
//            {
//                currentTime = 0;
//                isTimer = false;
//                // ゴールしてないまま時間切れ
//                ShowResultByTime();
//            }

//            TimerText.text = currentTime.ToString("F0");
//        }
//    }

//    public void startTimer()
//    {
//        currentTime = time_limit;
//        isTimer = true;
//        isGoal = false;
//    }

//    public bool GetisTimer()
//    {
//        return isTimer;
//    }

//    // ✅ ゴールしたときに呼び出す
//    public void OnGoal()
//    {
//        if (!isTimer || isGoal) return;

//        isGoal = true;
//        isTimer = false;
//        ShowResultByTime();
//    }

//    // ✅ 経過時間に応じてリザルト画面を表示
//    private void ShowResultByTime()
//    {
//        float elapsedTime = time_limit - currentTime;

//        // 効果音を再生
//        if (goalAudioSource != null && goalSoundEffect != null)
//        {
//            goalAudioSource.PlayOneShot(goalSoundEffect);
//        }




//        if (elapsedTime <= (time_limit / 2))
//        {
//            SceneManager.LoadScene("GoodResult");
//        }
//        else if (elapsedTime < time_limit)
//        {
//            SceneManager.LoadScene("NormalResult");
//        }
//        else
//        {
//            SceneManager.LoadScene("BadResult");
//        }
//    }
//}






using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class Timer : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI TimerText;

    [SerializeField, Tooltip("制限時間(秒)")]
    private float time_limit = 60f;

    private float currentTime;
    private bool isTimerRunning = false;
    private bool hasReachedGoal = false;
    private bool hasTimeExpired = false;
    private bool hasTimeExpiredAudioFlug = false;

    // ゴールした時の効果音
    public AudioSource goalAudioSource;
    public AudioClip goalSoundEffect;

    // 経過時間
    private float elapsedTime = 0f;

    void Start()
    {
        currentTime = time_limit;
        TimerText.text = currentTime.ToString("F0");
        isTimerRunning = true;
        hasReachedGoal = false;
        hasTimeExpired = false;
        hasTimeExpiredAudioFlug = false;
    }

    void Update()
    {
        if (isTimerRunning)
        {
            currentTime -= Time.deltaTime;
            elapsedTime += Time.deltaTime; // 経過時間を正確に計測

            if (currentTime <= 0)
            {
                currentTime = 0;
                isTimerRunning = false;
                hasTimeExpired = true;
                // 時間切れになったことを示すフラグを立てる
            }

            TimerText.text = currentTime.ToString("F0");
        }

        // ゴールまたは時間切れ後のシーン切り替え処理
        if (hasReachedGoal)
        {
            // 効果音が鳴り終わった後にシーン切り替え
            if (goalAudioSource != null && !goalAudioSource.isPlaying)
            {
                LoadResultScene();
            }
        }
        else if (hasTimeExpired)
        {
            if(!hasTimeExpiredAudioFlug)
            {
                // ゴール時の効果音を再生
                if (goalAudioSource != null && goalSoundEffect != null)
                {
                    goalAudioSource.PlayOneShot(goalSoundEffect);
                    hasTimeExpiredAudioFlug = true;
                }
            }


            // 効果音が鳴り終わった後にシーン切り替え
            if (goalAudioSource != null && !goalAudioSource.isPlaying && hasTimeExpiredAudioFlug)
            {
                LoadResultScene();
            }
        }
    }

    // ゴールに到達したときに外部から呼び出す
    public void OnGoal()
    {
        if (!isTimerRunning || hasReachedGoal) return;

        isTimerRunning = false;
        hasReachedGoal = true;

        // ゴール時の効果音を再生
        if (goalAudioSource != null && goalSoundEffect != null)
        {
            goalAudioSource.PlayOneShot(goalSoundEffect);
        }
    }

    // 結果シーンをロードする共通メソッド
    private void LoadResultScene()
    {
        // 既にシーンが切り替わっている場合は何もしない
        if (SceneManager.GetActiveScene().name != "GoodResult" &&
            SceneManager.GetActiveScene().name != "NormalResult" &&
            SceneManager.GetActiveScene().name != "BadResult")
        {
            if (hasTimeExpired || elapsedTime > time_limit)
            {
                SceneManager.LoadScene("BadResult");
            }
            else if (elapsedTime <= (time_limit * 0.75f))
            {
                SceneManager.LoadScene("GoodResult");
            }
            else if (elapsedTime < time_limit)
            {
                SceneManager.LoadScene("NormalResult");
            }
        }
    }
}