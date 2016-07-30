using UnityEngine;
using System.Collections;


//This script takes in charge to animate the camera to always move to the a selected object 
//NOTE : the object must contain an empty gameObject of the name "FocusPoint".

public class AnimateCamera : MonoBehaviour
{
    public float moveSpeed = 35.0f;
    public GameObject targetObject;
    private bool movingTowardsTarget = false;

    private float lerpSpeed = 0.025f;
    private Transform initialRot;
    private Transform finalRot;
    private bool inPosition;


	// Use this for initialization
	void Start ()
    {

	}
	
	// Update is called once per frame
	void Update ()
    { 
        //Activated by pressing E
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (movingTowardsTarget == true)
            {
                movingTowardsTarget = false;
            }
            else
            {
                initialRot = gameObject.transform;
                finalRot = targetObject.transform;
                movingTowardsTarget = true;
            }
        }

        if (movingTowardsTarget)
        {
            MoveTowardsTarget(targetObject);
        }
	}

    public void MoveTowardsTarget(GameObject target)
    {
        transform.position = Vector3.MoveTowards(transform.position, target.transform.position, moveSpeed * Time.deltaTime);

        //Move the camera to the target position.
        if (Vector3.Distance(transform.position,target.transform.position) < 0.1)
        {
            transform.position = target.transform.position;
            inPosition = true;

            //movingTowardsTarget = false;

        }

        //Rotate the camera towards the object.
        if (inPosition)
        {
            if (Mathf.Abs(initialRot.localEulerAngles.y - finalRot.localEulerAngles.y) < 3)
            {
                //reset boolean variables
                transform.rotation = target.transform.rotation;

                movingTowardsTarget = false;
                inPosition = false;
            }
            else
            {
                transform.rotation = Quaternion.Lerp(initialRot.rotation, finalRot.rotation, Time.time * lerpSpeed);
            }
        }
    }
}
