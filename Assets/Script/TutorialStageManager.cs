using UnityEngine;
using UnityEngine.UI;

public class TutorialStageManager : MonoBehaviour
{
    [SerializeField, Tooltip("画像をはめるオブジェクト")]
    private Image image;

    [SerializeField, Tooltip("表示する画像")]
    private Sprite sprite;

    [SerializeField, Tooltip("画像を表示するかどうか")]
    private bool showOnStart = true;


    private void Awake()
    {
        if (!image) image = GetComponent<Image>();
        image.preserveAspect = true;
    }

    void Start()
    {
        
    }

    
    void Update()
    {
        
    }
}
