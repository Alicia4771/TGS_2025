using UnityEngine;
using UnityEngine.SceneManagement;

public class tutorialGoalTrigger : MonoBehaviour
{
    public string nextSceneName = "Level2";
    public AudioSource goalAudioSource;
    public AudioClip goalSoundEffect;

    // ゴールに到達したことを示すフラグ
    private bool hasReachedGoal = false;

    private void Start()
    {
        hasReachedGoal = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        // プレイヤーに接触したとき、かつまだゴールに到達していない場合
        if (other.CompareTag("Player") && !hasReachedGoal)
        {
            Debug.Log("ゴールに到達しました！");

            // ゴール音を再生
            if (goalAudioSource != null && goalSoundEffect != null)
            {
                goalAudioSource.clip = goalSoundEffect;
                goalAudioSource.Play();
            }

            // ゴールに到達したフラグを立てる
            hasReachedGoal = true;
        }
    }

    void Update()
    {
        // ゴールに到達済みで、かつ効果音が再生されていない場合
        if (hasReachedGoal && goalAudioSource != null && !goalAudioSource.isPlaying)
        {
            Debug.Log("効果音の再生が終了しました。次のシーンに切り替えます。");
            SceneManager.LoadScene(nextSceneName);
        }
    }
}