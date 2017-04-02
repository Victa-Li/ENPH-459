using UnityEngine;

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


	//private scaleByAcceleration sbaScript;

	// Use this for initialization
	void Start () {
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
	void Update () {

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

		setAllAngels (0, 0, 0, 0, 0, 0);
	}


	// This is the inverse kinematics to compute all the angles for the 6 segments
	// @param: x,y,z,roll,pitch,yaw
	// 		as the position and rotation of the end-of-tooltip coordinates. 
	public void setAllAngels(float x, float y, float z, float roll, float pitch, float yaw) {
	
		// ----------------------------------------- Task 1: setup ------------------------------------------
		// length of each segmanet 
		// @to-do: adjust to real lengths
		float l1 = 10;
		float l2 = 10;
		float l3 = 10;
		float l4 = 10;
		float l5 = 10;


		// thetas are the angles of rotation for each axis: indexed as 1 to 6
		float theta1 = 0;
		float theta2 = 0;
		float theta3 = 0;
		float theta4 = 0;
		float theta5 = 0;
		float theta6 = 0;

		// create the DH parameter matrix 
		// @to-do: right now it is just dummy numbers; change this later
		float [,] DHMatrix = new float[7,4];
		// a: distance along rotated x-axis 
		float a1 = 350;
		float a2 = 0;
		float a3 = 850;
		float a4 = 145;
		float a5 = 0;
		float a6 = 0;

		// alpha: angle rotating along the new x-axis
		float alpha1 = 0;
		float alpha2 = Mathf.PI/2;
		float alpha3 = 0;
		float alpha4 = Mathf.PI/2;
		float alpha5 = -Mathf.PI/2;
		float alpha6 = Mathf.PI/2;

		// d: depth along previous z-axis to common normal
		float d1 = 0;
		float d2 = -815;
		float d3 = 0;
		float d4 = 0;
		float d5 = -820;
		float d6 = 0;
		float d7 = 170;

		// thetaX_offset: angle about previous z to align its x-axis with the new origin
		float theta1_offset= 0;
		float theta2_offset = 0;
		float theta3_offset = -Mathf.PI/2;
		float theta4_offset = 0;
		float theta5_offset = Mathf.PI;
		float theta6_offset = Mathf.PI;
		float theta7_offset = 0;
	
		// ----------------------------------------- Task 2: Joint 1 ------------------------------------------
		// Target Transformation T_0_G
		// @to-do: right now it is just dummy numbers. Get these numbers from read() method
		Matrix4x4 T_0_G = new Matrix4x4();
		T_0_G [0, 0] = 0;		T_0_G [0, 1] = 0;		T_0_G [0, 2] = 0;		T_0_G [0, 3] = 0;
		T_0_G [1, 0] = 0;		T_0_G [1, 1] = 0;		T_0_G [1, 2] = 0;		T_0_G [1, 3] = 0;
		T_0_G [2, 0] = 0;		T_0_G [2, 1] = 0;		T_0_G [2, 2] = 0;		T_0_G [2, 3] = 0;
		T_0_G [3, 0] = 0;		T_0_G [3, 1] = 0;		T_0_G [3, 2] = 0;		T_0_G [3, 3] = 1;

		// Target Transformation T_0_2, absolute position of Joint 2, using forward kinematics, with assuming theta2 is zero. 
		// 		this is to use analyzing Joint 3's position vector. 
		// @to-do: right now it is just dummy numbers. Get these numbers from read() method
		Matrix4x4 T_0_2 = new Matrix4x4();
		T_0_2 [0, 0] = 0;		T_0_2 [0, 1] = 0;		T_0_2 [0, 2] = 0;		T_0_2 [0, 3] = 0;
		T_0_2 [1, 0] = 0;		T_0_2 [1, 1] = 0;		T_0_2 [1, 2] = 0;		T_0_2 [1, 3] = 0;
		T_0_2 [2, 0] = 0;		T_0_2 [2, 1] = 0;		T_0_2 [2, 2] = 0;		T_0_2 [2, 3] = 0;
		T_0_2 [3, 0] = 0;		T_0_2 [3, 1] = 0;		T_0_2 [3, 2] = 0;		T_0_2 [3, 3] = 1;

		// Target Transformation T_0_4, absolute position of Joint 4, using forward kinematics, with assuming theta4 is zero. 
		// 		this is to use analyzing Joint 5's position vector. 
		// @to-do: right now it is just dummy numbers. Get these numbers from read() method
		Matrix4x4 T_0_4 = new Matrix4x4();
		T_0_4 [0, 0] = 0;		T_0_4 [0, 1] = 0;		T_0_4 [0, 2] = 0;		T_0_4 [0, 3] = 0;
		T_0_4 [1, 0] = 0;		T_0_4 [1, 1] = 0;		T_0_4 [1, 2] = 0;		T_0_4 [1, 3] = 0;
		T_0_4 [2, 0] = 0;		T_0_4 [2, 1] = 0;		T_0_4 [2, 2] = 0;		T_0_4 [2, 3] = 0;
		T_0_4 [3, 0] = 0;		T_0_4 [3, 1] = 0;		T_0_4 [3, 2] = 0;		T_0_4 [3, 3] = 1;

		// Asbolute Seat Orientation vector Nk0_0_6, relative to the base's origin
		Vector3 Nk0_0_6 = new Vector3 ();
		Nk0_0_6 [0] = T_0_G [0, 2];
		Nk0_0_6 [1] = T_0_G [1, 2];
		Nk0_0_6 [2] = T_0_G [2, 2];

		// Absolute Seat position vector Pk0_0_6, relative to base's origin
		Vector3 Pk0_0_6 = new Vector3 ();
		Pk0_0_6 [0] = T_0_G [0, 3];
		Pk0_0_6 [1] = T_0_G [1, 3];
		Pk0_0_6 [2] = T_0_G [2, 3];

		// ==> Relative orientation vector from seat to previous joint
		Vector3 Pk0_4_6 = d6 * Nk0_0_6;

		// ==> Absolute position of previous joint, joint 4
		Vector3 Pk0_0_4 = Pk0_0_6 - Pk0_4_6;

		// @to-do
		// Theta1 is either this value, or off by +PI
		theta1 = Mathf.Atan2 (T_0_G [1, 3] - d6 * T_0_G [1, 2], T_0_G [0, 3] - d6 * T_0_G [0, 2]);


		// ----------------------------------------- Task 3: Joint 3 ------------------------------------------
		// yes... we do not do joint 2 right after joint 1

		// Absolute Joint 2 position vector Pk0_0_2, relative to origin
		Vector3 Pk0_0_2 = new Vector3 ();
		Pk0_0_2 [0] = T_0_2 [0, 3];
		Pk0_0_2 [1] = T_0_2 [1, 3];
		Pk0_0_2 [2] = T_0_2 [2, 3];

		// Relative position Joint 3 to Joint 2
		// This length is used to compute the simple geometric relations 
		Vector3 Pk0_2_4 = Pk0_0_4 - Pk0_0_2;
		float phi = Mathf.Asin ( (Mathf.Pow(l1,2) - Mathf.Pow(a2,2) + Mathf.Pow(Pk0_2_4.magnitude,2)) / 
			(2 * Pk0_2_4.magnitude * l1)) + 
			Mathf.Asin( (Pk0_2_4.magnitude - (Mathf.Pow(l1,2) - Mathf.Pow(a2,2) + Mathf.Pow(Pk0_2_4.magnitude,2)) / (2 * Pk0_2_4.magnitude)) / a2);

		float alpha = Mathf.Atan2(-d4,a3);

		// theta 3 is either this value or off from plus phi to minus phi
		theta3 = Mathf.PI + phi - alpha;

		// ----------------------------------------- Task 4: Joint 2 ------------------------------------------
		// I don't see why 3 x 3 matirx multiplication... so it is such a pain to work with the math

		// relative position vector from joint 2 to 4
		Vector3 Pk2_2_4 = new Vector3 ();
		Pk2_2_4 [0] = T_0_2 [0, 0] * Pk0_2_4 [0] + T_0_2 [0, 1] * Pk0_2_4 [1] + T_0_2 [0, 2] * Pk0_2_4 [2];
		Pk2_2_4 [1] = T_0_2 [1, 0] * Pk0_2_4 [0] + T_0_2 [1, 1] * Pk0_2_4 [1] + T_0_2 [1, 2] * Pk0_2_4 [2];
		Pk2_2_4 [2] = T_0_2 [2, 0] * Pk0_2_4 [0] + T_0_2 [2, 1] * Pk0_2_4 [1] + T_0_2 [2, 2] * Pk0_2_4 [2];

		float beta1 = Mathf.Atan2 (Pk2_2_4[0], Pk2_2_4[1]);
		float beta2 = Mathf.Asin ((Mathf.Pow (a2, 2) - Mathf.Pow (Pk0_2_4.magnitude,2) + Mathf.Pow (l1, 2)) / (2 * l1 * a2)) +
			Mathf.Asin ( (l1 - (Mathf.Pow(a2,2) - Mathf.Pow(Pk0_2_4.magnitude,2) + Mathf.Pow(l1,2)) / (2 * l1)) / Pk0_2_4.magnitude);

		// theta 2 is either this value or another one --> please see the document
		theta2 = Mathf.PI/2 - (Mathf.Abs(beta1) + beta2);

		// ----------------------------------------- Task 5: Joint 5 ------------------------------------------
		// Now obtain the T_0_4 matrix from results so far, plus assuming theta 4 to be zero

		// absolute Orentation of joint 4
		Vector3 Nk0_0_4 = new Vector3 ();
		Nk0_0_4 [0] = T_0_4 [0, 2];
		Nk0_0_4 [1] = T_0_4 [1, 2];
		Nk0_0_4 [2] = T_0_4 [2, 2];

		float dotProductResult = Nk0_0_4 [0] * Nk0_0_6 [0] + Nk0_0_4 [1] * Nk0_0_6 [1] + Nk0_0_4 [2] * Nk0_0_6 [2];
		theta5 = Mathf.PI - Mathf.Acos (dotProductResult);

		// ----------------------------------------- Task 6: Joint 4 and Joint 6 ------------------------------------------
		// Now obtain the R_4_6 rotation matrix from results so far
		// but only needs the _2_3, _1_3, _3_2, and _3_1 components
		Matrix4x4 R_4_6 = new Matrix4x4();
		R_4_6 [0, 2] = 0;
		R_4_6 [1, 2] = 0;
		R_4_6 [2, 1] = 0;
		R_4_6 [2, 2] = 0;

		// Rotation matrix R_4_6 is used to compute theta4 and theta6, this computation very trivial, but I am not sure R_4_6 is correct
		// @to-do review this

		theta4 = Mathf.Atan2 (-R_4_6 [1, 2], R_4_6 [0, 2]);
		theta6 = Mathf.Atan2 (-R_4_6 [2, 1], R_4_6 [2, 0]);
	}
}
