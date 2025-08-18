using UnityEngine;

public class CharacterControllerByDistance : MonoBehaviour
{
    public float speedScale = 0.05f; // センサーの動きをゲーム速度に変換する倍率

    private float prevDistance;

    void Start()
    {
        prevDistance = DistanceSensorReader1.distance;
    }

    void Update()
    {
        float currentDistance = DistanceSensorReader1.distance;

        // センサーの変化量を時間で割って「速さ」にする
        float delta = (prevDistance - currentDistance) / Time.deltaTime;

        if (delta > 0)
        {
            // 距離が短くなっているときだけ前進
            float moveAmount = delta * speedScale * Time.deltaTime;
            transform.Translate(Vector3.forward * moveAmount);
        }
        else
        {
            // 戻すとき or 止まってるとき → キャラも止まる（何もしない）
        }

        prevDistance = currentDistance;
    }
}
