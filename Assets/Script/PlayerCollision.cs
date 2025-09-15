//using UnityEngine;

//public class PlayerCollision : MonoBehaviour
//{
//    public WebOverlay3DController webController; // Inspectorでドラッグしてアサイン
//    public FollowCamera FollowCamera; // Inspectorでドラッグしてアサイン


//    [SerializeField]
//    PlayerMoveFromSensor player;

//    [SerializeField, Tooltip("デバフ時間")]
//    private double time;

//    [SerializeField, Tooltip("スロー")]
//    private float slow;

//    private void OnTriggerEnter(Collider other)
//    {
//        if (other.CompareTag("Spider"))
//        {
//            //Debug.Log("🕷️ 蜘蛛に当たった！");

//            // 当たり判定がとられたところにこれを書く
//            player.collision(time, slow);

//            // 画面を揺らす処理
//            if (FollowCamera != null)
//            {
//                FollowCamera.ShakeCamera();
//            }
//            else
//            {
//                Debug.LogError("CameraScript がアサインされていません！");
//            }
//            // ここにダメージ処理 or スローダウン処理
//            Debug.Log("🕷️ 蜘蛛に当たった！");
//        }

//        if (other.CompareTag("SpiderWeb"))
//        {
//            // 当たり判定がとられたところにこれを書く
//            player.collision(time, slow);
//            Debug.Log("🕸️ 蜘蛛の巣に引っかかった！");
//            if (webController != null)
//            {
//                webController.ShowOnce();
//            }
//            else
//            {
//                Debug.LogError("webController がアサインされていません！");
//            }
//        }
//    }
//}
















using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    public WebOverlay3DController webController; // Inspectorでドラッグしてアサイン
    public FollowCamera FollowCamera; // Inspectorでドラッグしてアサイン


    [SerializeField]
    PlayerMoveFromSensor player;

    [SerializeField, Tooltip("デバフ時間")]
    private double time;

    [SerializeField, Tooltip("スロー")]
    private float slow;

    [SerializeField, Tooltip("デバフ解除後何秒間無敵時間にするか[s]")]
    private double invincible_time;

    // デバフが付与された瞬間の時間
    private double debuff_start_time;

    public AudioSource spiderAudioSource; // AudioSourceコンポーネントへの参照
    public AudioClip spiderSoundEffect;   // 再生したいオーディオクリップ

    private void Start()
    {
        debuff_start_time = -10;
    }

    private void OnTriggerEnter(Collider other)
    {
        // 今の時間
        double now_time = Time.time;

        if ((debuff_start_time + time + invincible_time) < now_time)
        {
            // デバフが付与された時間として今の時間をセット
            debuff_start_time = now_time;

            if (other.CompareTag("Spider"))
            {
                //Debug.Log("🕷️ 蜘蛛に当たった！");

                // 当たり判定がとられたところにこれを書く
                player.collision(2, 0.1f);

                // 効果音を再生
                if (spiderAudioSource != null && spiderSoundEffect != null)
                {
                    spiderAudioSource.PlayOneShot(spiderSoundEffect);
                }

                // 画面を揺らす処理
                if (FollowCamera != null)
                {
                    FollowCamera.ShakeCamera();
                }
                else
                {
                    Debug.LogError("CameraScript がアサインされていません！");
                }
                // ここにダメージ処理 or スローダウン処理
                //Debug.Log("🕷️ 蜘蛛に当たった！");
            }

            if (other.CompareTag("SpiderWeb"))
            {
                // 当たり判定がとられたところにこれを書く
                player.collision(time, slow);
                //Debug.Log("🕸️ 蜘蛛の巣に引っかかった！");
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
}
