using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[ExecuteInEditMode]
public class PlayerCharacter : TileObject
{

    //public bool mouseOver;

    //public Image healthBar;

    //Player property
    public int AtkRange = 1;
    public int AtkPower = 3;
    public int AoeRange = 8;

    // Use this for initialization
    void Start ()
    {
        InitializeTileObject();

    }
	
	// Update is called once per frame
	void Update () {
     
        if (turnManager.SelectedUnit == this)
        {
            gameObject.GetComponent<MeshRenderer>().material.SetColor("_Color", Color.blue);
        }

        else if (turnManager.mouseOverObject == gameObject)
        {
            gameObject.GetComponent<MeshRenderer>().material.SetColor("_Color", Color.red);
        }

        else
        {
            gameObject.GetComponent<MeshRenderer>().material.SetColor("_Color", Color.white);
        }


        if (!isMoving)
        {
            NotMovingUpdate();

            if (turnManager.PlayerTurn)
            {
                Selecting();

                /*if (Input.GetKeyDown(KeyCode.K) && turnManager.SelectedUnit == this)
                {
                    for(int i = 0; i < 300; i++)
                    {
                        Debug.Log("Dank memes");
                    }
                    
                }*/

                if (Input.GetKeyDown(KeyCode.B) && turnManager.SelectedUnit == this)
                {
                    turnManager.PlayerTurnEnd();
                }

                if (Input.GetKeyDown(KeyCode.A) && turnManager.SelectedUnit == this)
                {
                    DisplayAttacks(AtkRange, AtkPower, AoeRange, 9);
                }

                if (Input.GetKeyDown(KeyCode.M) && turnManager.SelectedUnit == this)
                {
                    ShowMoves();
                }
                
                else if (Input.GetKeyDown(KeyCode.N))
                {
                    FinishedMoving();
                }
            }
        }
        RotationUpdate();

        UpdateEffects();

    }

    void Selecting()
    {
        RaycastHit hitInfo = new RaycastHit();
        int mask = ~(1 << 8) ;
        bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo, Mathf.Infinity, mask);

        if (hit)
        {
            if (Input.GetKey(KeyCode.Mouse0))
            {
                if (hitInfo.transform.gameObject.GetComponent<MovementPlane>() != null && turnManager.SelectedUnit == this)
                {
                    MovementPlane movePlane = hitInfo.transform.gameObject.GetComponent<MovementPlane>();
                    if (movePlane.IsAttack)
                    {
                        movePlane.ExecuteAttack();
                    }
                    else
                    {
                        isMoving = true;
                        GameObject move = hitInfo.transform.gameObject;
                        StartCoroutine(FollowRoute(TileObject.TruncateRoute(move.GetComponent<MovementPlane>().Route, 32, false)));
                    }
                    turnManager.SelectedUnit = null;
                    ClearMoves();
                }
            }
            else
            {
                if (hitInfo.transform.gameObject.GetComponent<MovementPlane>() != null)
                {
                    if (hitInfo.transform.gameObject.GetComponent<MovementPlane>().IsAttack)
                    {
                        turnManager.mouseOverObject = hitInfo.transform.gameObject;
                    }
                }
            }
        }
    }

    public override void FinishedMoving()
    {

    }

    
}
