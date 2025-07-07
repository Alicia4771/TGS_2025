using UnityEngine;

public class SwingObstacle : MonoBehaviour
{
    public float swingSpeed = 2f;       // 揺れるスピード
    public float swingAmount = 2f;      // 揺れる幅

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        float offsetX = Mathf.Sin(Time.time * swingSpeed) * swingAmount;
        transform.position = startPos + new Vector3(offsetX, 0f, 0f);
    }
}
