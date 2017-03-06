using UnityEngine;
using System.Collections;
using System.Xml.Serialization;
using System.IO;
using Assets.Scripts;
using UnityEngine.Assertions;

public class RobotArmControl : MonoBehaviour {

	private GameObject RobotArm_base;
	private GameObject RobotArm_main_rotor;
	private GameObject RobotArm_lower_arm;
	private GameObject RobotArm_upper_arm_back;
    private GameObject RobotArm_upper_arm_front;
    private GameObject RobotArm_wrist_peice;
	private GameObject RobotArm_seat;

	public float RA_x = 0f;
	public float RA_y = 0.8f;
	public float RA_z = 0.8f;
	public float RA_yaw = 0.0f;
	public float RA_pitch = 0.0f;
	public float RA_roll = 0.0f;

	public float axis_base = 0.0f;
	public float axis_lower = 0.0f;
	public float axis_upper = 0.0f;
	public float axis_wrist = 0.0f;
	public float axis_seat = 0.0f;

	private float axis_base_offset = -90.0f;
	private float axis_lower_offset = 0.0f;
	private float axis_upper_offset = 0.0f;
	private float axis_wrist_offset = 270.0f;
	private float axis_seat_offset = 410.0f;

	private bool RA_complete = false;

	private readonly Vector3 segment1 = new Vector3 (0.0f, 0.38f, 0.0f); // From base of arm to Kuka-Axis 2
	private readonly Vector3 segment2 = new Vector3 (0.8f, 0.0f, 0.0f); // From Kuka-Axis 2 to Kuka-Axis 3
	private readonly Vector3 segment3 = new Vector3 (0.95f, 0.0f, 0.0f); // From Kuka-Axis 3 to Kuka-Axis 5
	private readonly Vector3 segment4 = new Vector3 (0.25f, 0.0f, 0.0f); // From Kuka-Axis 5 to seat origin
    private CarObject car;
    XmlSerializer serializer = new XmlSerializer(typeof(CarObject));
    private FileStream stream;
    //StreamWriter writer = new StreamWriter("robot.xml");
    

    //private scaleByAcceleration sbaScript;

    // Use this for initialization
    void Start () {
        car = new CarObject();
        //serializer = new XmlSerializer(typeof(CarObject));
       // stream = new FileStream("robot.xml", FileMode.OpenOrCreate);
        //writer = new StreamWriter("robot.xml");
        Transform[] RA_parts = GetComponentsInChildren<Transform> ();
		for (int i = 0; i < RA_parts.Length; i++) {
			if (RA_parts[i].name == "base")
				RobotArm_base = RA_parts [i].gameObject;
			else if (RA_parts[i].name == "main_rotor")
				RobotArm_main_rotor = RA_parts [i].gameObject;
			else if (RA_parts[i].name == "lower_arm")
				RobotArm_lower_arm = RA_parts [i].gameObject;
			else if (RA_parts[i].name == "upper_arm_back")
                RobotArm_upper_arm_back = RA_parts [i].gameObject;
            else if (RA_parts[i].name == "upper_arm_front")
                RobotArm_upper_arm_front = RA_parts[i].gameObject;
            else if (RA_parts[i].name == "wrist_piece")
				RobotArm_wrist_peice = RA_parts [i].gameObject;
			else if (RA_parts[i].name == "seat")
				RobotArm_seat = RA_parts[i].gameObject;
            
			
		}
		if (RobotArm_base != null
		    && RobotArm_main_rotor != null
		    && RobotArm_lower_arm != null
		    && RobotArm_upper_arm_back != null
            && RobotArm_upper_arm_front != null
            && RobotArm_wrist_peice != null
		    && RobotArm_seat != null) {

			RA_complete = true;
			//sbaScript = GameObject.FindGameObjectWithTag ("AccelerationController").GetComponent<scaleByAcceleration>();
		} else {
			Debug.LogError ("Robot arm not initialized!");
		}



	}
	
	// Update is called once per frame
	void Update ()
	{
	    stream = new FileStream("robot.xml", FileMode.Append);
        car.x = RA_x;
        car.y = RA_y;
        car.z = RA_z;
        car.anglex = RA_pitch;
        car.angley = RA_roll;
        car.anglez = RA_yaw;
        
        Assert.IsNotNull(stream);
        Assert.IsNotNull(car);
        serializer.Serialize(stream, car);
        stream.Close();

    }

	void FixedUpdate() {
      
		if (RA_complete) {

			//setAxes (sbaScript.playerAcceleration.x * 90.0f, sbaScript.playerAcceleration.z * 90.0f, sbaScript.playerAcceleration.x * 90.0f, sbaScript.playerAcceleration.x * 90.0f, sbaScript.playerAcceleration.x * 90.0f);
			setCoords(-RA_x, RA_z, RA_y, RA_pitch, RA_roll, RA_yaw);
			setAxes (axis_base, axis_lower, axis_upper, axis_wrist, axis_seat);

			// Fix axes to go from 0 to 360 and centred at 90:

			if (axis_seat < 180.0f)
				axis_seat += 360.0f;
			if (axis_wrist < 0.0f)
				axis_wrist += 360.0f;

			axis_seat *= -1.0f;
			axis_wrist *= -1.0f;

			axis_base += axis_base_offset;
			axis_lower += axis_lower_offset;
			axis_upper += axis_upper_offset;
			axis_wrist += axis_wrist_offset;
			axis_seat += axis_seat_offset;
		}
	    
       
	}

	// All axes are in degrees
	public void setAxes(float set_rotor, float set_lower, float set_upper, float set_wrist, float set_seat) {

		Vector3 localForward = Math3d.GetForwardVector (transform.rotation);
		Vector3 localRight = Vector3.Cross (localForward, Vector3.down);

		RobotArm_main_rotor.transform.localPosition = Vector3.zero;
		RobotArm_main_rotor.transform.localRotation = Quaternion.AngleAxis(set_rotor, Vector3.up);
		RobotArm_lower_arm.transform.localPosition = segment1;
		RobotArm_lower_arm.transform.localRotation = Quaternion.AngleAxis (set_lower, localForward);
        RobotArm_upper_arm_back.transform.localPosition = segment2;
        RobotArm_upper_arm_back.transform.localRotation = Quaternion.AngleAxis (set_upper, localForward);
        // RobotArm_upper_arm_front.transform.localPosition = segmentX;
        // RobotArm_upper_arm_front.transform.localRotation = Quaternion.AngleAxis(set_upperX, localForward);
        RobotArm_wrist_peice.transform.localPosition = segment3;
		RobotArm_wrist_peice.transform.localRotation =  Quaternion.AngleAxis (180 - set_wrist, localForward);
		RobotArm_seat.transform.localPosition = segment4;
		RobotArm_seat.transform.localRotation =  Quaternion.AngleAxis (180 + set_seat, localRight);

	}
	public void setCoords (float x, float y, float z, float roll, float pitch, float yaw) {

		float pitch_r = Mathf.Deg2Rad * pitch;

		// Inverse kinematics formulae;

		float axis1 = Mathf.Atan2(y,x);

		float param_A = x - segment4.x * Mathf.Cos (axis1) * Mathf.Cos (pitch_r);
		float param_B = y - segment4.x * Mathf.Sin (axis1) * Mathf.Cos (pitch_r);
		float param_C = z - segment1.y - segment4.x * Mathf.Sin (pitch_r);

		float axis3 = Mathf.Acos ((Mathf.Pow (param_A, 2.0f) + Mathf.Pow (param_B, 2.0f) + Mathf.Pow (param_C, 2.0f) - Mathf.Pow (segment2.x, 2.0f) - Mathf.Pow (segment3.x, 2.0f)) / (2 * segment2.x * segment3.x));

		float param_a = segment3.x * Mathf.Sin (axis3);
		float param_b = segment2.x + segment3.x * Mathf.Cos (axis3);
		float param_c = z - segment1.y - segment4.x * Mathf.Sin (pitch_r);
		float param_r = Mathf.Sqrt (Mathf.Pow (param_a, 2.0f) + Mathf.Pow (param_b, 2.0f));

		// Param_r2 can be multiplied by + or - 
		float param_r2 = Mathf.Sqrt (Mathf.Pow (param_r, 2.0f) - Mathf.Pow (param_c, 2.0f));

		float axis2 = Mathf.Atan2 (param_c, -param_r2) - Mathf.Atan2 (param_a, param_b);

		float axis4 = pitch_r + axis2 + axis3;

		if (!float.IsNaN (axis3) && !float.IsNaN (axis2)) {
			axis_base = Mathf.Rad2Deg * axis1;
			axis_lower = Mathf.Rad2Deg * axis2;
			axis_upper = Mathf.Rad2Deg * axis3;
			axis_wrist = Mathf.Rad2Deg * axis4;
			axis_seat = roll;
		}
	}
}
