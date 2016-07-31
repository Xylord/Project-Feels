using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class ComputerUnit : TileObject {

	// Use this for initialization
	void Start () {
        InitializeTileObject();
    }
	
	// Update is called once per frame
	void Update () {
        NotMovingUpdate();

    }

	
}
