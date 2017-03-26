﻿using UnityEngine;

public class ForceSimulator : MonoBehaviour {

	public Vector3 testVector;

	Vector3 simulatedVector;
	public MovementController mc;
	float g;
	public float exaggeration = 1.0f;
	/// Reality factor threshold
	public float RF_threshold = 0.95f;
	/// Acceleration mapping reality factor
	public float AM_RF;

    /// Simulated - percieved acceleration
    public float d_acceleration;

	public float pitch;
	public float roll;
	public float yaw;

    public Vector3 localAccel;
    public Vector3 accelerationLimit = new Vector3(15, 20, 10);

    private Vector3 initialPosition;
	// Use this for initialization
	void Start () {
		initialPosition = transform.position;
		
	}
	
	// Update is called once per frame
	void Update () {
		// Get the local accelerations:
		localAccel = mc.transform.InverseTransformDirection(mc.acceleration);
        // Limit the acceleration
        if (localAccel.x > accelerationLimit.x)
            localAccel.x = accelerationLimit.x;
        if (localAccel.x < -accelerationLimit.x)
            localAccel.x = -accelerationLimit.x;
        if (localAccel.y > accelerationLimit.y)
            localAccel.y = accelerationLimit.y;
        if (localAccel.y < -accelerationLimit.y)
            localAccel.y = -accelerationLimit.y;
        if (localAccel.z > accelerationLimit.z)
            localAccel.z = accelerationLimit.z;
        if (localAccel.z < -accelerationLimit.z)
            localAccel.z = -accelerationLimit.z;

        AM_RF = Physics.gravity.magnitude / (mc.acceleration * exaggeration + Physics.gravity).magnitude;

        Vector3 AM_from = Physics.gravity;
        Vector3 AM_to = Physics.gravity + new Vector3( localAccel.z, 0, -localAccel.x);
        Quaternion AM_rotation = Quaternion.FromToRotation(AM_from, AM_to);
        
        transform.position = initialPosition;
        Vector3 temp = mc.transform.rotation.eulerAngles;
        Quaternion rotation_no_y = Quaternion.Euler(new Vector3(temp.z, 0, -temp.x));
        transform.rotation = rotation_no_y * AM_rotation;

		testVector = new Vector3 (transform.rotation.eulerAngles.x % 360.0f, transform.rotation.eulerAngles.y % 360.0f, transform.rotation.eulerAngles.z % 360.0f);
	}
}
