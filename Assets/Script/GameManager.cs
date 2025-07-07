using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private Timer timer;
    [SerializeField]
    private TextMeshProUGUI messageText; // ← 追加

    private bool TimerBefore;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        timer.startTimer();
        TimerBefore = false;
        messageText.text = "START"; // 初期表示
    }

    // Update is called once per frame
    void Update()
    {
        //タイマーが動いてる状態から止まっている状態に切り替わった瞬間を取る
        if (TimerBefore && !timer.GetisTimer())
        {
            Debug.Log("ゲーム終了!");
        }
        TimerBefore = timer.GetisTimer();
    }
}