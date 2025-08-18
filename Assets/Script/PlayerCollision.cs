using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Spider"))
        {
            Debug.Log("🕷️ 蜘蛛に当たった！");
            // ここにダメージ処理 or スローダウン処理を書く
        }

        if (other.CompareTag("SpiderWeb"))
        {
            Debug.Log("🕸️ 蜘蛛の巣に引っかかった！");
            // ここに移動速度を落とす処理を書く
        }
    }
}
