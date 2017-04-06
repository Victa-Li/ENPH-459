using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeedBackSimulateOnCube : MonoBehaviour
{
    //private GameObject Cube;
	// Use this for initialization
    public static float x = 0;
    public static float y = 0;
    public static float z = 0;
    public static float anglex = 0;
    public static float angley = 0;
    public static float anglez = 0;
    private Vector3 position ;
    private Vector3 axisx= new Vector3(1,0,0);
    private Vector3 axisy= new Vector3(0,1,0);
    private Vector3 axisz= new Vector3(0,0,1);
    void Start ()
	{
	    transform.localPosition= Vector3.zero;
	    
     
	}
	
	// Update is called once per frame
	void Update () {
        position=new Vector3(x,y,z);
	    transform.localPosition = position;
	    transform.localRotation = Quaternion.AngleAxis(anglez, axisy);
        transform.localRotation = Quaternion.AngleAxis( angley,axisz)* Quaternion.AngleAxis(anglex, axisx);
    }
}
