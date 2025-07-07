using UnityEngine;
using UnityEngine.SceneManagement;


public class keyCommand : MonoBehaviour
{
    [SerializeField, Tooltip("moveSample")]
    moveSample moveSampleScript;

    void Update()
    {
    

        if (Input.GetKeyDown(KeyCode.Alpha1))  // 数字の1が押されたとき
        {
            // 倍率を1.0に設定
            moveSampleScript.SetMoveRatio(1.0);
            SceneManager.LoadScene("SampleScene"); // ← 実際のシーン名に変更
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))  // 数字の2が押されたとき
        {
            // 倍率を1.5に設定
            moveSampleScript.SetMoveRatio(1.5);
            SceneManager.LoadScene("SampleScene"); // ← 実際のシーン名に変更
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))  // 数字の3が押されたとき
        {
            // 倍率を2.0に設定
            moveSampleScript.SetMoveRatio(2.0);
            SceneManager.LoadScene("SampleScene"); // ← 実際のシーン名に変更
        }
    }
}