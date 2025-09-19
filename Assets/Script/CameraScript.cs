using UnityEngine;

public class CameraScript : MonoBehaviour
{
    private Transform parentTransform;

    void Start()
    {
        parentTransform = transform.parent;

        this.transform.position = parentTransform.position;
        this.transform.rotation = transform.parent.rotation;
    }
}
