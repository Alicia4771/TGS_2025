using UnityEngine;

public class CharacterControllerByDistance : MonoBehaviour
{
    public float speedScale = 0.05f; // センサーの動きをゲーム速度に変換する倍率
    public float noiseThreshold = 5f; // ノイズ閾値
    public float minDelta = 1f;       // 移動とみなす最小値

    // パーティクルシステムへの参照
    public ParticleSystem windParticles;

    private float prevDistance;

    void Start()
    {
        prevDistance = DistanceSensorReader1.distance;

        // ゲーム開始時にパーティクルを停止
        if (windParticles != null)
        {
            windParticles.Stop();
        }
    }

    void Update()
    {
        float currentDistance = DistanceSensorReader1.distance;
        float rawDelta = prevDistance - currentDistance;

        // 小さい変化はノイズとして無視
        if (Mathf.Abs(rawDelta) < noiseThreshold)
        {
            rawDelta = 0;
        }

        float delta = rawDelta / Time.deltaTime;
        delta = Mathf.Clamp(delta, -100f, 100f); // 異常値制御

        // 一定以上の変化があったときだけ前進
        if (delta > minDelta)
        {
            float moveAmount = delta * speedScale * Time.deltaTime;
            transform.Translate(Vector3.forward * moveAmount);
            // 動いているとき、再生
            windParticles.Play();
            Debug.Log("風吹く");
        }
        else {
            // 停止しているとき、停止
            windParticles.Stop();

        }

        prevDistance = currentDistance;
    }
}
