using UnityEngine;
using System.Collections;


//This script takes in charge to animate the camera to always move to the a selected object 
//NOTE : the object must contain an empty gameObject of the name "FocusPoint".

public class AnimateCamera : MonoBehaviour
{
    public float moveSpeed = 35.0f;
    private bool movingTowardsTarget = false; 

    private GameObject targetObject;
    public Vector3 springArmOffset;

    float cameraDistanceMax = 20f;
    float cameraDistanceMin = 5f;
    float cameraDistance = 10f;
    float scrollSpeed = 3.0f;


    void Start()
    {
        springArmOffset.Set(-10, 10, 0);
       
        //Set default targetObject as the SpawnCameraDefault
        targetObject = GameObject.Find("SpawnCameraDefault");
        //By default (once the game has started for the first time) it will move to the SpawnCameraDefault
        MoveTowardsTarget(targetObject);
    }

    void Update()
    {
        CameraZoom();
        MoveTowardsTarget(targetObject);
    }

    public void MoveTowardsTarget(GameObject target)
    {
        //Condition: Check if the position of the camera is set at the appropriate location.
        if (Camera.main.transform.position != target.transform.position + springArmOffset)
        {
            //if it is the case we want to set the variable below to true in order to move the Camera.
            movingTowardsTarget = true;
        }
        else
            //else we want to stop the camera from moving.
            movingTowardsTarget = false;


        if (movingTowardsTarget)
        {
            //the following line execute the movement to move towards a selected object. Note: it is doing a spherical interpolation to smooth the transition.
            transform.position = Vector3.Slerp(transform.position, target.transform.position + springArmOffset, moveSpeed * Time.deltaTime);
        }

    }

    private void CameraZoom()
    {
        cameraDistance -= Input.GetAxis("Mouse ScrollWheel") * scrollSpeed;
        cameraDistance = Mathf.Clamp(cameraDistance, cameraDistanceMin, cameraDistanceMax);

        springArmOffset.Set(-cameraDistance, cameraDistance, 0);
    }

    public GameObject TargetObject
    {
        get
        {
            return targetObject;
        }
        set
        {
            targetObject = value;
        }
    }
}
