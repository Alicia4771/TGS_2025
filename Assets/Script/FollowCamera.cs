//using UnityEngine;
//using System.Collections;

//public class FollowCamera : MonoBehaviour
//{
//    private Transform parentTransform;
//    private Vector3 initialPosition;
//    private Coroutine shakeCoroutine;

//    public float shakeDuration = 0.15f; // 揺れの継続時間
//    public float shakeMagnitude = 0.2f; // 揺れの強さ

//    void Start()
//    {
//        parentTransform = transform.parent;

//        this.transform.position = parentTransform.position;
//    }


//    public void ShakeCamera()
//    {
//        if (shakeCoroutine != null)
//        {
//            StopCoroutine(shakeCoroutine);
//        }
//        shakeCoroutine = StartCoroutine(DoShake());
//    }

//    private IEnumerator DoShake()
//    {
//        float timer = 0f;

//        while (timer < shakeDuration)
//        {
//            timer += Time.deltaTime;

//            // ランダムな方向に小さいオフセットを加える
//            float x = Random.Range(-1f, 1f) * shakeMagnitude;
//            float y = Random.Range(-1f, 1f) * shakeMagnitude;

//            transform.localPosition = initialPosition + new Vector3(x, y, 0); // ローカル座標を基準に揺らす

//            yield return null;
//        }

//        transform.localPosition = initialPosition; // 揺れが終わったら元の位置に戻す
//        shakeCoroutine = null;
//    }




//    // Update is called once per frame
//    void Update()
//    {

//    }
//}












using UnityEngine;
using System.Collections;

public class FollowCamera : MonoBehaviour
{
    private Transform parentTransform;
    private Vector3 initialLocalPosition; // ローカル座標を保存するように修正
    private Coroutine shakeCoroutine;

    public float shakeDuration = 0.15f; // 揺れの継続時間
    public float shakeMagnitude = 0.2f; // 揺れの強さ

    void Start()
    {
        parentTransform = transform.parent;
        // カメラの初期ローカル座標を保存
        initialLocalPosition = transform.localPosition;
    }

    void Update()
    {
        // 常に親オブジェクト（プレイヤー）に追従する
        // ローカル座標は、親オブジェクトからの相対位置を維持するため、
        // 揺れ処理と追従処理が両立する
        if (shakeCoroutine == null)
        {
            transform.localPosition = initialLocalPosition;
        }
    }

    public void ShakeCamera()
    {
        if (shakeCoroutine != null)
        {
            StopCoroutine(shakeCoroutine);
        }
        shakeCoroutine = StartCoroutine(DoShake());
    }

    private IEnumerator DoShake()
    {
        float timer = 0f;

        while (timer < shakeDuration)
        {
            timer += Time.deltaTime;

            // ランダムな方向に小さいオフセットを親のローカル座標に加える
            float x = Random.Range(-1f, 1f) * shakeMagnitude;
            float y = Random.Range(-1f, 1f) * shakeMagnitude;

            transform.localPosition = initialLocalPosition + new Vector3(x, y, 0);

            yield return null;
        }

        // 揺れが終わったら、元のローカル座標に戻す
        transform.localPosition = initialLocalPosition;
        shakeCoroutine = null;
    }
}