﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITextUpdater : MonoBehaviour {

    public Text targetVectorText;
    public MovementController mc;
    public ForceSimulator fs;
    //public EthernetIOManger connect;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        targetVectorText.text = "Target Accel: " + string.Format("{0:0.00}", fs.localAccel.x) + ", "
            + string.Format("{0:0.00}", fs.localAccel.y) + ", " + string.Format("{0:0.00}", fs.localAccel.z) + "\n" +
            "Sim Accel:     " + string.Format("{0:0.00}", fs.feltAccel.x) + ", "
            + string.Format("{0:0.00}", fs.feltAccel.y) + ", " + string.Format("{0:0.00}", fs.feltAccel.z) + "\n" +
            "Seat Accel:    " + string.Format("{0:0.00}", fs.seatAccel.x) + ", "
            + string.Format("{0:0.00}", fs.seatAccel.y) + ", " + string.Format("{0:0.00}", fs.seatAccel.z) + "\n" +
            "Total Accel:   " + string.Format("{0:0.00}", fs.seatAccel.x+fs.feltAccel.x) + ", "
            + string.Format("{0:0.00}", fs.seatAccel.y+fs.feltAccel.y) + ", " + string.Format("{0:0.00}", fs.seatAccel.z+fs.seatAccel.z)+"\n"
            + "output(receive):   "+ string.Format(EthernetIOManger.testoutput)+"\n"
            + "output(send):   " + string.Format(EthernetIOManger.testsend);

    }
}
