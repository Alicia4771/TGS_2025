using UnityEngine;

public class SpiderPendulumScript : MonoBehaviour
{
    public float maxSwingAngle = 30f;      // 最大振れ角
    public float swingFrequency = 0.5f;    // 揺れる速さ

    private float _prevAngle = 0f;
    private Vector3 pivotPoint;            // 支点（糸の付け根）

    void Start()
    {
        // 親の位置を支点に自動設定
        if (transform.parent != null)
        {
            pivotPoint = transform.parent.position;
        }
        else
        {
            // 親がいない場合は自身の位置を支点に
            pivotPoint = transform.position;
            Debug.LogWarning("SpiderPendulumScript: 親がいないため、自身の位置を支点に設定");
        }
    }

    void Update()
    {
        float angle = Mathf.Sin(Time.time * Mathf.PI * 2f * swingFrequency) * maxSwingAngle;

        // 前回との差分で回転
        float delta = angle - _prevAngle;
        transform.RotateAround(pivotPoint, Vector3.forward, delta); // 横揺れならZ軸
        _prevAngle = angle;
    }
}
