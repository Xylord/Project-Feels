using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class BasicTile : MonoBehaviour {

    public enum Orientation
    {
        Directionless,
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
    public Mesh foundation, top;

    private int presentHeight, nextHeight;
    private float continuousHeight, heightVariationProgress, randomHeightVariation;
    private bool isOccupied;
    private bool isUnderwater;
    
    

	// Use this for initialization
	void Start () {
        
	}

	// Update is called once per frame
	void Update ()
    {
        //height = transform.localPosition.magnitude;
        if(isOccupied == true)
        {
            gameObject.GetComponent<MeshRenderer>().material.color = Color.red;
        }
        else
        {
            gameObject.GetComponent<MeshRenderer>().material.color = Color.white;
        }
        
        if (presentHeight != nextHeight)
        {
            continuousHeight = Mathf.Lerp(presentHeight, nextHeight, heightVariationProgress);
            heightVariationProgress += Time.deltaTime;
            if (heightVariationProgress >= 1f)
            {
                heightVariationProgress = 0f;
                presentHeight = nextHeight;
            }
        }
        
        transform.localPosition = new Vector3(xPosition * grid.GridScale, continuousHeight * grid.VerticalScale + randomHeightVariation, yPosition * grid.GridScale);
    }

    public void InitializeTile()
    {
        grid = GameObject.Find("Grid").GetComponent<LevelGrid>();
        presentHeight = baseHeight;
        heightVariationProgress = 0f;
        randomHeightVariation = Random.Range(-randomHeightVariationRange, randomHeightVariationRange);
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

    public bool IsOccupied
    {
        get
        {
            return isOccupied;
        }
        set
        {
            isOccupied = value;
        }
    }
}
