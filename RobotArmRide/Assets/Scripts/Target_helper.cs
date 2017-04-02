using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target_helper : MonoBehaviour {

    public ForceSimulator fs;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        transform.position = fs.transform.position;
        transform.rotation = fs.rotation_no_y;
        fs.seatAccet_rotFrame = transform.InverseTransformDirection(fs.seatAccel);
    }
}
