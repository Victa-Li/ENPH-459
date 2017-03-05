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

    /// Percieved acceleration - Simulated acceleration
    //public float d_acceleration;

	public float pitch;
	public float roll;
	public float yaw;

    public Vector3 localAccel;
    public Vector3 accelerationLimit = new Vector3(15, 20, 10);

    public Vector3 simulatedAccel;
    public float h1;
    public float h2;

    private Vector3 initialPosition;
	// Use this for initialization
	void Start () {
		initialPosition = transform.position;
		//mc = GameObject.FindGameObjectWithTag ("Player").GetComponent<MovementController>();
		//g = Physics.gravity.magnitude;
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

        // Acceleration Mapping
        //pitch = -Mathf.Rad2Deg * Mathf.Atan2 (localAccel.z, g);
        //roll = Mathf.Rad2Deg * Mathf.Atan2 (localAccel.x, g);
        //yaw = Mathf.Rad2Deg * Mathf.Atan2 (localAccel.x, transform.localPosition.z);

        //AM_RF = Physics.gravity.magnitude / (mc.acceleration * exaggeration + Physics.gravity).magnitude;
        //Quaternion AM_rotation = Quaternion.Euler (exaggeration * pitch, 0.0f, exaggeration * roll);
        // Testing a better rotation method:
        Vector3 AM_from = Physics.gravity;
        Vector3 AM_to = Physics.gravity + new Vector3( localAccel.z, 0, -localAccel.x);
        Quaternion AM_rotation = Quaternion.FromToRotation(AM_from, AM_to);

        // Calculate the expected "felt" acceleration:
        h1 = (new Vector2(localAccel.x, localAccel.z)).magnitude;
        h2 = (new Vector2(h1, Physics.gravity.magnitude)).magnitude;
        simulatedAccel = new Vector3(localAccel.x, 0, localAccel.z) / h2 * Physics.gravity.magnitude;
        //d_acceleration = (Physics.gravity.magnitude - AM_to.magnitude);
        // Impulse simulation

        // Small


        // Large


        // Rotation


        // Set transform
        transform.position = initialPosition;
        Vector3 temp = mc.transform.rotation.eulerAngles;
        Quaternion rotation_no_y = Quaternion.Euler(new Vector3(temp.z, 0, -temp.x));
        transform.rotation = rotation_no_y * AM_rotation;

		testVector = new Vector3 (transform.rotation.eulerAngles.x % 360.0f, transform.rotation.eulerAngles.y % 360.0f, transform.rotation.eulerAngles.z % 360.0f);
	}
}
