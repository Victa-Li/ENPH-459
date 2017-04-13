using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;

public class TextfileOutput : MonoBehaviour {
    
    public string outfile;
    public MovementController mc;
    public ForceSimulator fs;
    // Use this for initialization
    void Start()
    {
        outfile = "AccelLog-" + String.Format("{0:d-M-yyyy-HH-mm-ss}", DateTime.Now) + ".log";
    }

    // Update is called once per frame
    void Update()
    {

        string newtext = "Timestamp, " + Time.time.ToString() + ", " +
            "Target Accel, " + string.Format("{0:0.00}", fs.localAccel.x) + ", "
            + string.Format("{0:0.00}", fs.localAccel.y) + ", " + string.Format("{0:0.00}", fs.localAccel.z) + ", " +
            "Sim Accel, " + string.Format("{0:0.00}", fs.feltAccel.x) + ", "
            + string.Format("{0:0.00}", fs.feltAccel.y) + ", " + string.Format("{0:0.00}", fs.feltAccel.z) + ", " +
            "Seat Accel, " + string.Format("{0:0.00}", fs.seatAccet_rotFrame.x) + ", "
            + string.Format("{0:0.00}", fs.seatAccet_rotFrame.y) + ", " + string.Format("{0:0.00}", fs.seatAccet_rotFrame.z) + ", " +
            "Total Accel, " + string.Format("{0:0.00}", fs.seatAccet_rotFrame.x + fs.feltAccel.x) + ", "
            + string.Format("{0:0.00}", fs.seatAccet_rotFrame.y + fs.feltAccel.y) + ", " + string.Format("{0:0.00}", fs.seatAccet_rotFrame.z + fs.feltAccel.z);

        using (System.IO.StreamWriter file =
            new System.IO.StreamWriter(outfile, true))
        {
            file.WriteLine(newtext);
        }
    }
}
