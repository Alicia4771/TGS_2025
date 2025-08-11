using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public Transform target; // 追いかけるキャラクター
    public Vector3 offset = new Vector3(0, 5, -8); // 位置のオフセット
    public float smoothSpeed = 5f; // 追従スムーズさ

    void LateUpdate()
    {
        if (target == null) return;

        // 目標位置を計算
        Vector3 desiredPosition = target.position + offset;

        // スムーズに補間して追従
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.position = smoothedPosition;

        // 常にターゲットを見る
        transform.LookAt(target);
    }
}
