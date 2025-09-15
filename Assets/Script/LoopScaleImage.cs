using UnityEngine;
using UnityEngine.UI;

public class LoopScaleImage : MonoBehaviour
{
    [Header("対象のImage")]
    public Image targetImage;

    [Header("スケール設定")]
    public float minScale = 0.8f;   // 最小倍率
    public float maxScale = 1.2f;   // 最大倍率
    public float speed = 2f;        // 拡縮スピード

    void Update()
    {
        // 0〜1を行き来する値を作る
        float t = (Mathf.Sin(Time.time * speed) + 1f) / 2f;

        // min〜max の間を補間
        float scale = Mathf.Lerp(minScale, maxScale, t);

        // RectTransform にスケールを反映
        targetImage.rectTransform.localScale = new Vector3(scale, scale, 1f);
    }
}
