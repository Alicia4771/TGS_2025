//using UnityEngine;

//public class GoalTrigger : MonoBehaviour
//{
//    void OnTriggerEnter(Collider other)
//    {
//        // Playerタグがついているオブジェクトと接触したら
//        if (other.CompareTag("Player"))
//        {
//            Debug.Log("Game Over!");
//            // ここにゲーム終了処理を追加可能
//        }
//    }
//}






using UnityEngine;

public class GoalTrigger : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        // Playerタグがついているオブジェクトと接触したら
        if (other.CompareTag("Player"))
        {
            Debug.LogError("🚨 ゴール判定 🚨");


            // Timerスクリプトを探して、ゴール処理を呼び出す
            Timer timer = FindObjectOfType<Timer>();
            if (timer != null)
            {
                timer.OnGoal(); // ← ここでリザルト画面に切り替え
            }
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            Debug.Log("テスト用：Gキーでゴール処理実行");

            Timer timer = FindObjectOfType<Timer>();
            if (timer != null)
            {
                timer.OnGoal();
            }
        }
    }

}
