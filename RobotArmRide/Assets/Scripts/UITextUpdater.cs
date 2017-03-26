using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITextUpdater : MonoBehaviour {

    public Text targetVectorText;
    public MovementController mc;
    public ForceSimulator fs;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        targetVectorText.text = "Target Accel: " + string.Format("{0:0.00}", fs.localAccel.x) + ", "
            + string.Format("{0:0.00}", fs.localAccel.y) + ", " + string.Format("{0:0.00}", fs.localAccel.z) + "\n" +
            "Sim Accel:     " + string.Format("{0:0.00}", fs.feltAccel.x) + ", "
            + string.Format("{0:0.00}", fs.feltAccel.y) + ", " + string.Format("{0:0.00}", fs.feltAccel.z) + "\n" +
            "Seat Accel:    " + string.Format("{0:0.00}", fs.seatAccet_rotFrame.x) + ", "
            + string.Format("{0:0.00}", fs.seatAccet_rotFrame.y) + ", " + string.Format("{0:0.00}", fs.seatAccet_rotFrame.z) + "\n" +
            "Total Accel:   " + string.Format("{0:0.00}", fs.seatAccet_rotFrame.x+fs.feltAccel.x) + ", "
            + string.Format("{0:0.00}", fs.seatAccet_rotFrame.y+fs.feltAccel.y) + ", " + string.Format("{0:0.00}", fs.seatAccet_rotFrame.z+fs.feltAccel.z);

    }
}
