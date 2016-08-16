using UnityEngine;
using System.Collections;

public class BasicTileObject : MonoBehaviour {

    public LevelGrid grid;
    public GameObject presentTile, nextTile;
    public float offsetFromTile;

    // Use this for initialization
    void Start () {
        grid = GameObject.Find("Grid").GetComponent<LevelGrid>();
        transform.parent = grid.gameObject.transform;
    }
	
	// Update is called once per frame
	void Update () {
        NotMovingUpdate();

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
