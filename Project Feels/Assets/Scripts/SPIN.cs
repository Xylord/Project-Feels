using UnityEngine;
using System.Collections;

public class SPIN : MonoBehaviour {

    public float rotation, rotSpeed, radius;
    private AuraManager aura;
    private GameObject particleSys;

	// Use this for initialization
	void Start () {
        particleSys = transform.GetChild(0).gameObject;
        aura = transform.parent.gameObject.GetComponent<AuraManager>();
	}
	
	// Update is called once per frame
	void Update () {
        rotation += Time.deltaTime * rotSpeed;
        if (rotation >= 360f)
            rotation = 0f;

        radius = aura.auraRange;
        particleSys.transform.localPosition = new Vector3(radius, 0f, 0f);
        transform.rotation = Quaternion.Euler(0f, rotation, 0f);
	}
}
