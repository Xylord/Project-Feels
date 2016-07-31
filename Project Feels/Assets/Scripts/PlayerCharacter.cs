using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class PlayerCharacter : TileObject {

	// Use this for initialization
	void Start () {
        InitializeTileObject();
    }
	
	// Update is called once per frame
	void Update () {
        NotMovingUpdate();
        Selecting();

        if (Input.GetKeyDown(KeyCode.M))
        {
            print("Showing moves");
            ShowMoves();
        }

    }

	
}
