using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    public WebOverlay3DController webController; // Inspectorでドラッグしてアサイン

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Spider"))
        {
            Debug.Log("🕷️ 蜘蛛に当たった！");
            // ここにダメージ処理 or スローダウン処理
        }

        if (other.CompareTag("SpiderWeb"))
        {
            Debug.Log("🕸️ 蜘蛛の巣に引っかかった！");
            if (webController != null)
            {
                webController.ShowOnce();
            }
            else
            {
                Debug.LogError("webController がアサインされていません！");
            }
        }
    }
}
