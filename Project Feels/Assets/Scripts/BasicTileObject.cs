using UnityEngine;
using System.Collections;

public class BasicTileObject : MonoBehaviour {

    public LevelGrid grid;
    public GameObject presentTile, nextTile;
    public float offsetFromTile;
    public BasicTile.Orientation orientation;
    private float rotSlerpFctr;

    // Use this for initialization
    void Start () {
        grid = GameObject.Find("Grid").GetComponent<LevelGrid>();
        transform.parent = grid.gameObject.transform;
    }
	
	// Update is called once per frame
	void Update () {
        NotMovingUpdate();
        RotationUpdate();

    }

    public void RotationUpdate()
    {
        Quaternion newRotation = Quaternion.identity;
        switch (orientation)
        {
            case BasicTile.Orientation.Forward:
                newRotation = Quaternion.Euler(0f, 90f, 0f);
                break;
            case BasicTile.Orientation.ForwardRight:
                newRotation = Quaternion.Euler(0f, 45f, 0f);
                break;
            case BasicTile.Orientation.Right:
                newRotation = Quaternion.Euler(0f, 0f, 0f);
                break;
            case BasicTile.Orientation.BackwardRight:
                newRotation = Quaternion.Euler(0f, 315f, 0f);
                break;
            case BasicTile.Orientation.Backward:
                newRotation = Quaternion.Euler(0f, 270f, 0f);
                break;
            case BasicTile.Orientation.BackwardLeft:
                newRotation = Quaternion.Euler(0f, 225f, 0f);
                break;
            case BasicTile.Orientation.Left:
                newRotation = Quaternion.Euler(0f, 180f, 0f);
                break;
            case BasicTile.Orientation.ForwardLeft:
                newRotation = Quaternion.Euler(0f, 135f, 0f);
                break;
        }

        transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, rotSlerpFctr);
        rotSlerpFctr += Time.deltaTime * 0.01f;

        if(rotSlerpFctr >= 1f)
        {
            rotSlerpFctr = 0f;
        }
    }

    public void NotMovingUpdate()
    {
        if (nextTile == null)
        {
            switch (presentTile.GetComponent<BasicTile>().type)
            {
                case BasicTile.TileKind.Flat:
                    transform.localPosition = presentTile.transform.position + new Vector3(0f, offsetFromTile, 0f);
                    break;

                case BasicTile.TileKind.Stair:
                    transform.localPosition = presentTile.transform.position + new Vector3(0f, offsetFromTile - 0.25f, 0f);
                    break;
            }
        }
    }
}
