using UnityEngine;

public class keyCommand : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))  // 数字の1が押されたとき
        {
            Debug.Log("1が押されました！");
            // ここに処理を書く
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))  // 数字の2が押されたとき
        {
            Debug.Log("2が押されました！");
            // ここに別の処理を書く
        }
    }
}