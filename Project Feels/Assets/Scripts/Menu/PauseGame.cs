using UnityEngine;
using System.Collections;

public class PauseGame : MonoBehaviour {

    public bool paused;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update ()
    {

	}

    public void Pause()
    {
        paused = !paused;
        if (paused) { Time.timeScale = 0; }
        else if (!paused) { Time.timeScale = 1; }
    }
}
