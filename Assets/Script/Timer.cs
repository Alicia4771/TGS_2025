


using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement; // シーン切り替えに必要

public class Timer : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI TimerText;

    [SerializeField, Tooltip("制限時間(秒)")]
    private float time_limit = 60f;

    private float currentTime;
    private bool isTimer = false;
    private bool isGoal = false; // ゴール済みフラグ

    // ゴールした時の効果音
    public AudioSource goalAudioSource; // AudioSourceコンポーネントへの参照
    public AudioClip goalSoundEffect;   // 再生したいオーディオクリップ

    void Start()
    {
        currentTime = time_limit;
        TimerText.text = currentTime.ToString("F0");
        startTimer();
    }

    void Update()
    {
        if (isTimer && !isGoal)
        {
            currentTime -= Time.deltaTime;

            if (currentTime <= 0)
            {
                currentTime = 0;
                isTimer = false;
                // ゴールしてないまま時間切れ
                ShowResultByTime();
            }

            TimerText.text = currentTime.ToString("F0");
        }
    }

    public void startTimer()
    {
        currentTime = time_limit;
        isTimer = true;
        isGoal = false;
    }

    public bool GetisTimer()
    {
        return isTimer;
    }

    // ✅ ゴールしたときに呼び出す
    public void OnGoal()
    {
        if (!isTimer || isGoal) return;

        isGoal = true;
        isTimer = false;
        ShowResultByTime();
    }

    // ✅ 経過時間に応じてリザルト画面を表示
    private void ShowResultByTime()
    {
        float elapsedTime = time_limit - currentTime;

        // 効果音を再生
        if (goalAudioSource != null && goalSoundEffect != null)
        {
            goalAudioSource.PlayOneShot(goalSoundEffect);
        }



        if (elapsedTime <= (time_limit / 2))
        {
            SceneManager.LoadScene("GoodResult");
        }
        else if (elapsedTime < time_limit)
        {
            SceneManager.LoadScene("NormalResult");
        }
        else
        {
            SceneManager.LoadScene("BadResult");
        }
    }
}
