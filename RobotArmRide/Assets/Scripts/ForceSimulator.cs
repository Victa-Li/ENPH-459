using UnityEngine;

public class ForceSimulator : MonoBehaviour {

	public Vector3 testVector;
    
	public MovementController mc;
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

    public Vector3 feltAccel;

    public float ISverticalReturnSpeed = 0.01f;
    public float ISverticalMultiplier = 0.5f;
    public float ISverticalThreshold = 1;

    private Rigidbody rb;
    private Vector3 initialPosition;
	// Use this for initialization
	void Start () {
		initialPosition = transform.position;
        rb = GetComponent<Rigidbody>();
		//mc = GameObject.FindGameObjectWithTag ("Player").GetComponent<MovementController>();
	}
	
	void FixedUpdate () {
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
        Vector3 AM_from = Physics.gravity;
        Vector3 AM_to = Physics.gravity + new Vector3( localAccel.z, 0, -localAccel.x);
        Quaternion AM_rotation = Quaternion.FromToRotation(AM_from, AM_to);

        //d_acceleration = (Physics.gravity.magnitude - AM_to.magnitude);

        // Impulse simulation up-down:
        if (Mathf.Abs(localAccel.y) > ISverticalThreshold)
            rb.AddForce(transform.up * localAccel.y * ISverticalMultiplier, ForceMode.Acceleration);
        else
        {
            Vector3 returnStep = Vector3.zero;
            float d_x = transform.position.x - initialPosition.x;
            float d_y = transform.position.y - initialPosition.y;
            float d_z = transform.position.z - initialPosition.z;

            if (Mathf.Abs(d_x) > ISverticalReturnSpeed / 2)
                //    returnStep.x = -Mathf.Sign(d_x);
                //else
                returnStep.x = Mathf.Lerp(-Mathf.Sign(d_x), 0, 0.5f);
            if (Mathf.Abs(d_y) > ISverticalReturnSpeed / 2)
                //    returnStep.y = -Mathf.Sign(d_y);
                //else
                returnStep.y = Mathf.Lerp(-Mathf.Sign(d_y), 0, 0.5f);
            if (Mathf.Abs(d_z) > ISverticalReturnSpeed / 2)
                //    returnStep.z = -Mathf.Sign(d_z);
                //else
                returnStep.z = Mathf.Lerp(-Mathf.Sign(d_z), 0, 0.5f);

            transform.Translate(returnStep * ISverticalReturnSpeed);
        }

        // Rotation


        // Set transform
        Vector3 temp = mc.transform.rotation.eulerAngles;
        Quaternion rotation_no_y = Quaternion.Euler(new Vector3(temp.z, 0, -temp.x));
        transform.rotation = rotation_no_y * AM_rotation;

        // Calculate the expected "felt" acceleration:
        float h1 = (new Vector2(localAccel.x, localAccel.z)).magnitude;
        float h2 = (new Vector2(h1, Physics.gravity.magnitude)).magnitude;
        feltAccel = new Vector3(localAccel.x, 0, localAccel.z) / h2 * Physics.gravity.magnitude;

        testVector = new Vector3 (transform.rotation.eulerAngles.x % 360.0f, transform.rotation.eulerAngles.y % 360.0f, transform.rotation.eulerAngles.z % 360.0f);
	}
}
