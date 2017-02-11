using UnityEngine;

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
		// Acceleration Mapping
		//pitch = -Mathf.Rad2Deg * Mathf.Atan2 (localAccel.z, g);
		//roll = Mathf.Rad2Deg * Mathf.Atan2 (localAccel.x, g);
		//yaw = Mathf.Rad2Deg * Mathf.Atan2 (localAccel.x, transform.localPosition.z);

		AM_RF = Physics.gravity.magnitude / (mc.acceleration * exaggeration + Physics.gravity).magnitude;
        //Quaternion AM_rotation = Quaternion.Euler (exaggeration * pitch, 0.0f, exaggeration * roll);
        // Testing a better rotation method:
        Vector3 AM_from = Physics.gravity;
        Vector3 AM_to = Physics.gravity + new Vector3( localAccel.z, 0, -localAccel.x);
        Quaternion AM_rotation = Quaternion.FromToRotation(AM_from, AM_to);
        // Impulse simulation

        // Small


        // Large


        // Rotation


        // Set transform
        transform.position = initialPosition;
        Vector3 temp = mc.transform.rotation.eulerAngles;
        Quaternion rotation_no_y = Quaternion.Euler(new Vector3(temp.x, 0, temp.z));
        transform.rotation = rotation_no_y * AM_rotation;

		testVector = new Vector3 (transform.rotation.eulerAngles.x % 360.0f, transform.rotation.eulerAngles.y % 360.0f, transform.rotation.eulerAngles.z % 360.0f);
	}
}
