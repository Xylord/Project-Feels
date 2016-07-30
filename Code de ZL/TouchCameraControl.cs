using UnityEngine;
using System.Collections;

public class TouchCameraControl : MonoBehaviour
{

    public float moveSensitivityX = 1.0f;
    public float moveSensitivtyY = 1.0f;

    public bool updateZoomSensitivy = true;

    public float orthographicZoomSpeed = 0.05f;

    public float minZoom = 1.0f;
    public float maxZoom = 20.0f;

    public bool invertMoveX = false;
    public bool invertMoveY = false;

    private Camera _camera;

	// Use this for initialization
	void Start ()
    {
        _camera = Camera.main;
	}
	
	// Update is called once per frame
	void Update ()
    {
	    if(updateZoomSensitivy)
        {
            moveSensitivityX = _camera.orthographicSize / 5.0f;
            moveSensitivityX = _camera.orthographicSize / 5.0f;
        }

        Touch[] touches = Input.touches;

        if (touches.Length > 0)
        {
            //Single touch 
            if (touches.Length == 1)
            {
                if (touches[0].phase == TouchPhase.Moved)
                {
                    Vector2 delta = touches[0].deltaPosition;

                    //Math for X
                    float positionX = delta.x * moveSensitivityX * Time.deltaTime;       

                    //Invertion adjustments    
                    positionX = invertMoveX ? positionX : positionX * -1;

                    //Math for Y
                    float positionY = delta.y * moveSensitivtyY * Time.deltaTime;

                    //Invertion adjustments    
                    positionY = invertMoveY ? positionY : positionY * -1;

                    //Update position of the camera
                    _camera.transform.position += new Vector3(positionX, positionY, 0);

                }
            }

            // Double  touch 
            if (touches.Length == 2)
            {
                Touch toucheOne = touches[0];
                Touch touchTwo = touches[1];

                //to find the previous touch of in the previous frame
                Vector2 touchOnePreviousPos = toucheOne.position - toucheOne.deltaPosition;
                Vector2 touchTwoPreviousPos = touchTwo.position - touchTwo.deltaPosition;

                float prevTouchDeltaMagnitude = (touchOnePreviousPos - touchTwoPreviousPos).magnitude;

                float touchDeltaMagnitude = (toucheOne.position - touchTwo.position).magnitude;

                float deltaMagDiff = prevTouchDeltaMagnitude - touchDeltaMagnitude;

                _camera.orthographicSize += deltaMagDiff * orthographicZoomSpeed;
                _camera.orthographicSize = Mathf.Clamp( _camera.orthographicSize, minZoom, maxZoom );

            }
        }
	}
}
