using UnityEngine;

public class RobotArmTargetSetter : MonoBehaviour {

	RobotArmControl robotArmController;

	// Use this for initialization
	void Start () {
		robotArmController = GetComponentInParent<RobotArmControl> ();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		robotArmController.RA_x = transform.localPosition.x;
		robotArmController.RA_y = transform.localPosition.y;
		robotArmController.RA_z = transform.localPosition.z;
		robotArmController.RA_roll = transform.localRotation.eulerAngles.z;
		robotArmController.RA_pitch = transform.localRotation.eulerAngles.x;
		robotArmController.RA_yaw = transform.localRotation.eulerAngles.y;
	}
}
