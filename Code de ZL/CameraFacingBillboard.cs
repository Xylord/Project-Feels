using UnityEngine;
using System.Collections;


//Attach this to a canvas
public class CameraFacingBillboard : MonoBehaviour
{
    public Camera mainCamera;

    void Start()
    {
        //if ever you're too lazy to attach a camera manually... cuz ya know.
        if (mainCamera == null)
            mainCamera = GameObject.FindObjectOfType<Camera>();
    }

    void Update()
    {
        transform.LookAt(transform.position + mainCamera.transform.rotation * Vector3.forward,
            mainCamera.transform.rotation * Vector3.up);

    }
}