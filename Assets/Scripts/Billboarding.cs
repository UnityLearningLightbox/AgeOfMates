using UnityEngine;

public class Billboarding : MonoBehaviour
{
    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;//Se aplica a la camara activa
    }

    private void Update()
    {
        LookAtToCamera();
    }

    void LookAtToCamera()
    {
        if (mainCamera == null) return;

        transform.LookAt(mainCamera.transform);

        transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f);
    }
}


