using UnityEngine;

public class CameraScript : MonoBehaviour
{
    private Transform parentTransform;

    void Start()
    {
        parentTransform = transform.parent;

        this.transform.position = parentTransform.position;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
