using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class BasicTile : MonoBehaviour {

    public enum Orientation
    {
        Forward,
        Backward,
        Right,
        Left
    }

    public enum TileKind
    {
        Empty,
        Flat,
        Stair,
        SteepStair,
        Impassable
    }

    [SerializeField]
    private int xPosition, yPosition;

    public int baseHeight;
    public LevelGrid grid;
    public TileKind Type;
    public Orientation orientation;
    public float randomHeightVariationRange;
    public bool isSpawn;

    private float height, randomHeightVariation;
    private bool characterStepping;
    private bool isUnderwater;
    
    

	// Use this for initialization
	void Start () {
        grid = GameObject.Find("Grid").GetComponent<LevelGrid>();
        height = baseHeight;
        randomHeightVariation = Random.Range(-randomHeightVariationRange, randomHeightVariationRange);
	}

	// Update is called once per frame
	void Update () {
        //height = transform.localPosition.magnitude;
        if(isSpawn == true)
        {
            gameObject.GetComponent<MeshRenderer>().material.color = Color.red;
        }
        transform.localPosition = new Vector3(xPosition * grid.GridScale, height * grid.VerticalScale + randomHeightVariation, yPosition * grid.GridScale);

        
    }

    public int XPosition
    {
        get
        {
            return xPosition;
        }
        set
        {
            xPosition = value;
        }
    }

    public int YPosition
    {
        get
        {
            return yPosition;
        }
        set
        {
            yPosition = value;
        }
    }

    public bool IsSpawn
    {
        get
        {
            return isSpawn;
        }
        set
        {
            isSpawn = value;
        }
    }
}
