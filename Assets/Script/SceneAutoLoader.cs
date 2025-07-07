using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneAutoLoader : MonoBehaviour
{
    [SerializeField, Tooltip("切り替えるまでの秒数")]
    private float waitTime = 10f;

    [SerializeField, Tooltip("切り替えるシーン名")]
    private string targetSceneName = "Start";

    void Start()
    {
        // 一定時間後にシーンを切り替える
        Invoke("LoadTargetScene", waitTime);
    }

    void LoadTargetScene()
    {
        SceneManager.LoadScene(targetSceneName);
    }
}
