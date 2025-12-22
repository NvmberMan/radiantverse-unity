using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    void LateUpdate()
    {
        if (Camera.main == null) return;

        Vector3 dir = transform.position - Camera.main.transform.position;
        transform.rotation = Quaternion.LookRotation(dir);
    }
}
