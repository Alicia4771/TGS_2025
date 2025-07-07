using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;       // 追いかける対象（キューブ）
    public Vector3 offset = new Vector3(0, 5, -10); // カメラの位置（高さ・後ろ）

    void LateUpdate()
    {
        if (target != null)
        {
            transform.position = target.position + offset;
            transform.LookAt(target); // キューブの方向を見る
        }
    }
}
