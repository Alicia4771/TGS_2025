using UnityEngine;

public class CharacterControllerByDistance : MonoBehaviour
{
    public float moveSpeed = 2f; // 前進速度
    public float threshold = 30f; // この距離以下で動く

    void Update()
    {
        // DistanceSensorReader1.distance から取得
        float currentDistance = DistanceSensorReader1.distance;

        if (currentDistance <= threshold)
        {
            // 前進
            transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
        }
        else
        {
            // 停止（何もしない）
        }
    }
}
