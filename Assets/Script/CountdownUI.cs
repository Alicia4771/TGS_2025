using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CountdownUI : MonoBehaviour
{
    [SerializeField] private Image countdownImage;
    [SerializeField] private Sprite[] countdownSprites; // 3,2,1,GO
    [SerializeField] private float interval = 1f;

    [Header("BGM設定")]
    [SerializeField] private AudioSource bgmSource; // 再生用AudioSource
    [SerializeField] private AudioClip bgmClip;     // 再生したいBGM

    void Start()
    {
        StartCountdown();
    }

    public void StartCountdown()
    {
        StartCoroutine(CountdownRoutine());
    }

    private IEnumerator CountdownRoutine()
    {
        // 3,2,1,GO の順に表示
        for (int i = 0; i < countdownSprites.Length; i++)
        {
            countdownImage.sprite = countdownSprites[i];
            countdownImage.enabled = true;

            yield return new WaitForSeconds(interval);
        }

        // カウントダウン終了
        countdownImage.enabled = false;

        // 🎵 BGM再生
        if (bgmSource != null && bgmClip != null)
        {
            bgmSource.clip = bgmClip;
            bgmSource.Play();
        }

        Debug.Log("Race Start!");
        // GameManager.Instance.StartRace();
    }
}