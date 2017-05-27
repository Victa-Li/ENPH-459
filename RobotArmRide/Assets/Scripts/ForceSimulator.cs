using UnityEngine;

public class ForceSimulator : MonoBehaviour {

	public Vector3 testVector;
    
	public MovementController mc;
	public float exaggeration = 1.0f;

    public Vector3 localAccel;
    public Vector3 accelerationLimit = new Vector3(15, 20, 10);

    public Vector3 feltAccel;
    public Vector3 seatAccel;
    public Vector3 seatAccet_rotFrame; // Seat acceleration in the rotated frame due to orientation of the car

    public Target_helper target_helper;
    public Quaternion rotation_no_y;
        
    // Synchronization lock due to sharing of x, y, z, A, B, C values between unity main thread and ethernet communication thread
    public Object pos_rot_Lock = new Object();
    public Vector3 current_position;
    public Quaternion current_rotation;

    //***************************************//
    //*** Acceleration Mapping Parameters ***//
    //***************************************//
    public float leftRightSensitivity = 0.5f;
    public float forwardBackSensitivity = 1.0f;

    //*************************************//
    //*** Impusle Simulation Parameters ***//
    //*************************************//
    public float ISverticalReturnSpeed = 0.01f;
    public float ISverticalMultiplier = 0.5f;
    public float ISverticalThreshold = 1;
    public float ISlimitMultiplier = 1;
    // IS Limits:
    private float[] r_limits = { 0.06f, 0.08f}; // defined in meters
    //private float[] theta_limits = { -130f, -110f, 110f, 130f }; // defined in degrees
    //private float[] phi_limits = { 5f, 15f, 125f, 135f }; // defined in degrees
    // position in spherical coordinates
    public float r;
    //public float theta;
    //public float phi;

    private float[] heightLimits = { 0.0f, 0.1f }; // defined in meters

    private Rigidbody rb;
    private RobotArmControl RobotArm;
    public Vector3 returnPosition;
    /// Flag to enable adjusting the return position relative to the height of the car in the map
    public bool enableHeightAdjust = true;

    // LinearAcceleration variables
    private static Vector3[] positionRegister;
    private static float[] posTimeRegister;
    private static int positionSamplesTaken = 0;
    public int seatAccelSamples = 10;

    // Copied form math3d.cs since we need to have a local buffer of positions and times
    public static bool LinearAcceleration(out Vector3 vector, Vector3 position, int samples)
    {
        Vector3 averageSpeedChange = Vector3.zero;
        vector = Vector3.zero;
        Vector3 deltaDistance;
        float deltaTime;
        Vector3 speedA;
        Vector3 speedB;

        //Clamp sample amount. In order to calculate acceleration we need at least 2 changes
        //in speed, so we need at least 3 position samples.
        if (samples < 3)
        {
            samples = 3;
        }
        //Initialize
        if (positionRegister == null)
        {
            positionRegister = new Vector3[samples];
            posTimeRegister = new float[samples];
        }
        //Fill the position and time sample array and shift the location in the array to the left
        //each time a new sample is taken. This way index 0 will always hold the oldest sample and the
        //highest index will always hold the newest sample. 
        for (int i = 0; i < positionRegister.Length - 1; i++)
        {
            positionRegister[i] = positionRegister[i + 1];
            posTimeRegister[i] = posTimeRegister[i + 1];
        }
        positionRegister[positionRegister.Length - 1] = position;
        posTimeRegister[posTimeRegister.Length - 1] = Time.time;
        positionSamplesTaken++;
        //The output acceleration can only be calculated if enough samples are taken.
        if (positionSamplesTaken >= samples)
        {
            //Calculate average speed change.
            for (int i = 0; i < positionRegister.Length - 2; i++)
            {
                deltaDistance = positionRegister[i + 1] - positionRegister[i];
                deltaTime = posTimeRegister[i + 1] - posTimeRegister[i];
                //If deltaTime is 0, the output is invalid.
                if (deltaTime == 0)
                {
                    return false;
                }
                speedA = deltaDistance / deltaTime;
                deltaDistance = positionRegister[i + 2] - positionRegister[i + 1];
                deltaTime = posTimeRegister[i + 2] - posTimeRegister[i + 1];
                if (deltaTime == 0)
                {
                    return false;
                }
                speedB = deltaDistance / deltaTime;
                //This is the accumulated speed change at this stage, not the average yet.
                averageSpeedChange += speedB - speedA;
            }
            //Now this is the average speed change.
            averageSpeedChange /= positionRegister.Length - 2;
            //Get the total time difference.
            float deltaTimeTotal = posTimeRegister[posTimeRegister.Length - 1] - posTimeRegister[0];
            //Now calculate the acceleration, which is an average over the amount of samples taken.
            vector = averageSpeedChange / (deltaTimeTotal / (posTimeRegister.Length - 1));
            return true;
        }
        else
        {
            return false;
        }
    }

    // Use this for initialization
    void Start () {
        returnPosition = transform.position;
        rb = GetComponent<Rigidbody>();
        RobotArm = GetComponentInParent<RobotArmControl>();
	}
	
	void FixedUpdate () {
        if (enableHeightAdjust)
            returnPosition.y = RobotArm.transform.position.y + Mathf.Lerp(heightLimits[0], heightLimits[1], (mc.transform.position.y + 0.78f) / 40);

        //*********************************//
        //*** Impulse Simulation Limits ***//
        //*********************************//
        r = (transform.position - RobotArm.transform.position).magnitude;
        //theta = Mathf.Atan2(transform.position.z - RobotArm.transform.position.z, transform.position.x - RobotArm.transform.position.x) * Mathf.Rad2Deg;
        //phi = Mathf.Acos((transform.position.y - RobotArm.transform.position.y) / r) * Mathf.Rad2Deg;
        ISlimitMultiplier = 1;
        if ( r < r_limits[1]) // && r > r_limits[0]
        {
            if (r > r_limits[1])
                ISlimitMultiplier = Mathf.Min( Mathf.Lerp(1, 0, (r - r_limits[0]) / (r_limits[1] - r_limits[0])), ISlimitMultiplier);
        }
        else
            ISlimitMultiplier = 0;
        //if (theta > theta_limits[0] && theta < theta_limits[3])
        //{
        //    if (theta < theta_limits[1])
        //        ISlimitMultiplier = Mathf.Min(Mathf.Lerp(0, 1, (theta - theta_limits[0]) / (theta_limits[1] - theta_limits[0])), ISlimitMultiplier);
        //    else if (theta > theta_limits[2])
        //        ISlimitMultiplier = Mathf.Min(Mathf.Lerp(1, 0, (theta - theta_limits[2]) / (theta_limits[3] - theta_limits[2])), ISlimitMultiplier);
        //}
        //else
        //    ISlimitMultiplier = 0;
        //if (phi > phi_limits[0] && phi < phi_limits[3])
        //{
        //    if (phi < phi_limits[1])
        //        ISlimitMultiplier = Mathf.Min(Mathf.Lerp(0, 1, (phi - phi_limits[0]) / (phi_limits[1] - phi_limits[0])), ISlimitMultiplier);
        //    else if (phi > phi_limits[2])
        //        ISlimitMultiplier = Mathf.Min(Mathf.Lerp(1, 0, (phi - phi_limits[2]) / (phi_limits[3] - phi_limits[2])), ISlimitMultiplier);
        //}
        //else
        //    ISlimitMultiplier = 0;

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

        //****************************//
        //*** Acceleration Mapping ***//
        //****************************//
        Vector3 AM_from = Physics.gravity;
        Vector3 AM_to = Physics.gravity + new Vector3( localAccel.z * forwardBackSensitivity, 0, -localAccel.x * leftRightSensitivity);
        Quaternion AM_rotation = Quaternion.FromToRotation(AM_from, AM_to);

        //**********************************//
        //*** Impulse simulation up-down ***//
        //**********************************//
        if (Mathf.Abs(localAccel.y) > ISverticalThreshold)
            // rb.AddForce(target_helper.transform.up * localAccel.y * ISverticalMultiplier * ISlimitMultiplier, ForceMode.Acceleration);
            rb.AddForce(Vector3.up * localAccel.y * ISverticalMultiplier * ISlimitMultiplier, ForceMode.Acceleration);

        Vector3 returnStep = Vector3.zero;
        float d_x = transform.position.x - returnPosition.x;
        float d_y = transform.position.y - returnPosition.y;
        float d_z = transform.position.z - returnPosition.z;

        if (Mathf.Abs(d_x) > ISverticalReturnSpeed / 2)
            returnStep.x = Mathf.Lerp(-Mathf.Sign(d_x), 0, 0.5f);
        //else
        //    returnStep.x = -d_x;
        if (Mathf.Abs(d_y) > ISverticalReturnSpeed / 2)
            returnStep.y = Mathf.Lerp(-Mathf.Sign(d_y), 0, 0.5f);
        //else
        //    returnStep.y = -d_y;
        if (Mathf.Abs(d_z) > ISverticalReturnSpeed / 2)
            returnStep.z = Mathf.Lerp(-Mathf.Sign(d_z), 0, 0.5f);
        //else
        //    returnStep.z = -d_z;

        // Update position and rotation
        transform.Translate(returnStep * ISverticalReturnSpeed);
        //****************//
        //*** Rotation ***//
        //****************//
        Vector3 temp = mc.transform.rotation.eulerAngles;
        rotation_no_y = Quaternion.Euler(new Vector3(temp.z, 0, -temp.x));
        transform.rotation = rotation_no_y * AM_rotation;

        // Calculate the expected "felt" acceleration:
        float h1 = (new Vector2(localAccel.x, localAccel.z)).magnitude;
        float h2 = (new Vector2(h1, Physics.gravity.magnitude)).magnitude;
        feltAccel = new Vector3(localAccel.x, 0, localAccel.z) / h2 * Physics.gravity.magnitude;
        LinearAcceleration(out seatAccel, transform.position, seatAccelSamples);

        testVector = new Vector3 (transform.rotation.eulerAngles.x % 360.0f, transform.rotation.eulerAngles.y % 360.0f, transform.rotation.eulerAngles.z % 360.0f);
        
        //lock (pos_rot_Lock)
        //{
            current_position = transform.localPosition;
            current_rotation = transform.localRotation;
        //}

        rb.transform.localPosition = new Vector3(0f,rb.transform.localPosition.y, 0f);
    }
}


