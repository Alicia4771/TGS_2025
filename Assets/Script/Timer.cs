//using System.Collections;
//using System.Collections.Generic;
//using System.Threading.Tasks;
//using System.Threading;
//using UnityEngine;
//using UnityEngine.UI; //UIを使う時に必要
//using System;
//using TMPro; // ← 追加

//public class Timer : MonoBehaviour
//{
//    [SerializeField]
//    private TextMeshProUGUI TimerText; // ← 型を変える
//    // float limitTime = 60; //制限時間
//    [SerializeField, Tooltip("制限時間(秒)")]
//    private int time_limit; //制限時間
//    private int limitTime;

//    private bool isTimer = false;

//    // Start is called the first frame update
//    void Start()
//    {
//        limitTime = time_limit;
//        // isTimer = false;
//        Debug.Log("S:" + isTimer);
//    }

//    // Update is called once per frame
//    void Update()
//    {
//        // Debug.Log("Update");
//        // Debug.Log(isTimer);


//        if (isTimer)
//        {
//            // Debug.Log("ifif");

//            limitTime -= 1;

//            if (limitTime < 0)
//            {
//                limitTime = 0;
//                isTimer = false;
//            }

//            try
//            {
//                // TimerText.text = limitTime.ToString("F0"); //残り時間を整数で表示
//                TimerText.text = limitTime.ToString("F0"); //残り時間を整数で表示
//                                                           //Task.Delay(1000);
//                Thread.Sleep(1000);
//            }
//            catch
//            {
//                // if (TimerText == null) {
//                //     Debug.Log("nullでした。");
//                // }
//            }

//        }
//    }

//    public void startTimer()
//    {
//        isTimer = true;
//        Debug.Log("suta--to:" + isTimer);
//    }

//    public bool GetisTimer()
//    {
//        return isTimer;
//    }
//}





//using UnityEngine;
//using TMPro; // ← 追加！

//public class Timer : MonoBehaviour
//{
//    [SerializeField]
//    private TextMeshProUGUI TimerText; // ← Text から変更！

//    [SerializeField, Tooltip("制限時間(秒)")]
//    private float time_limit = 10f;

//    private float currentTime;
//    private bool isTimer = false;

//    void Start()
//    {
//        currentTime = time_limit;
//        TimerText.text = currentTime.ToString("F0");
//    }

//    void Update()
//    {
//        if (isTimer)
//        {
//            currentTime -= Time.deltaTime;

//            if (currentTime <= 0)
//            {
//                currentTime = 0;
//                isTimer = false;
//            }

//            TimerText.text = currentTime.ToString("F0");
//        }
//    }

//    public void startTimer()
//    {
//        currentTime = time_limit;
//        isTimer = true;
//    }

//    public bool GetisTimer()
//    {
//        return isTimer;
//    }
//}













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

    void Start()
    {
        currentTime = time_limit;
        TimerText.text = currentTime.ToString("F0");
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
