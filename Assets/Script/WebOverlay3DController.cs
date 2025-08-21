using UnityEngine;
using System.Collections;

public class WebOverlay3DController : MonoBehaviour
{

    // ----- 新しく追加する変数とコンポーネント -----
    public AudioSource windAudioSource; // AudioSourceコンポーネントへの参照
    public AudioClip windSoundEffect;   // 再生したいオーディオクリップ

    public float showTime = 3f;
    private Renderer[] renderers;

    void Awake()
    {
        renderers = GetComponentsInChildren<Renderer>();
        SetVisible(false);
    }

    public void ShowOnce()
    {
        SetVisible(true);
        StartCoroutine(HideAfterSeconds(showTime));
    }

    private IEnumerator HideAfterSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        SetVisible(false);
    }

    private void SetVisible(bool visible)
    {
        foreach (var r in renderers)
        {
            r.enabled = visible;
        }
    }
}
