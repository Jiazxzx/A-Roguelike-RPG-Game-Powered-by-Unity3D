using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Exp : MonoBehaviour {

    public float liveTime = 1f;

	// Use this for initialization
	void Start () {
        Destroy(this.gameObject, liveTime);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
