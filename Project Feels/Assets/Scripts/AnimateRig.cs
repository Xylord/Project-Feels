using UnityEngine;
using System.Collections;

public class AnimateRig : MonoBehaviour {

    public Animator animator;
    
    void Start () {
        animator = GetComponent<Animator>();
	}
	
	void Update () {
        if (Input.GetKeyDown("1"))
        {
            animator.Play("Armature|Walk_blocking", -1, 0f);
        }
	}
    
}
