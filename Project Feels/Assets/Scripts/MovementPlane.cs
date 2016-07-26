using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class MovementPlane : MonoBehaviour
{

    public LevelGrid grid;
    public GameObject presentTile;
    public float offsetFromTile;

    // Use this for initialization
    void Start()
    {
        InitializeMovementPlane();
    }

    public void InitializeMovementPlane()
    {
        grid = GameObject.Find("Grid").GetComponent<LevelGrid>();
        transform.parent = grid.gameObject.transform;
    }

    // Update is called once per frame
    void Update()
    {
        transform.localPosition = presentTile.transform.position + new Vector3(0f, offsetFromTile, 0f);


    }

    
}
