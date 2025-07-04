using UnityEngine;

public class keyCommand : MonoBehaviour
{
    void Update()
    {
        [SerializeField, Tooltip("moveSample")]
        moveSample moveSampleScript; ;

        if (Input.GetKeyDown(KeyCode.Alpha1))  // 数字の1が押されたとき
        {
            // 倍率を1.0に設定
            moveSampleScript.SetMoveRatio(1.0);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))  // 数字の2が押されたとき
        {
            // 倍率を1.5に設定
            moveSampleScript.SetMoveRatio(1.5);
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))  // 数字の3が押されたとき
        {
            // 倍率を2.0に設定
            moveSampleScript.SetMoveRatio(2.0);
        }
    }
}