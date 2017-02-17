using UnityEngine;
using System.Collections;

/// <summary>
/// Generic movement controller for the player. Should be inherited for a specific vehicle type.
/// </summary>
public class MovementController : MonoBehaviour {

	Vector3 initPlayerPos;
    Quaternion initPlayerRot;

    /// <summary>
    /// Raw sideways input from the joystick and keyboard
    /// </summary>
    public float rawInputHoriz;
	/// <summary>
	/// Raw forwards/backwards input from the joystick and keyboard
	/// </summary>
	public float rawInputVert;
	/// <summary>
	/// Raw handbrake input
	/// </summary>
	public float handbrake;

	public Vector3 acceleration;
    public Vector3 linearAcceleration;
    public Vector3 linearVelocity;
    public Vector3 angularAcceleration;
    public Vector3 angularVelocity;
    
    /// Velocity in the rotating reference frame
    //public Vector3 velocity_r;

    public int linearAccelerationSamples = 10;
    public int angularAccelerationSamples = 10;
    public int linearVelocitySamples = 10;
    public int angularVelocitySamples = 10;

    public PlayerMover pm; 

	// Use this for initialization
	void Start () {
		pm = GetComponent<PlayerMover> ();
		initPlayerPos = pm.transform.position;
        initPlayerRot = pm.transform.rotation;
    }
	
	// Update is called once per frame
	void Update () {
		rawInputHoriz = Input.GetAxis ("Horizontal");
		rawInputVert = Input.GetAxis ("Vertical");
		handbrake = Input.GetAxis ("Jump");
	}

	void FixedUpdate() {
        // Calculate acceleration in the rotating reference frame
        Math3d.LinearVelocity(out linearVelocity, transform.position, linearVelocitySamples);
        Math3d.AngularVelocity(out angularVelocity, transform.rotation, angularVelocitySamples);
        Math3d.LinearAcceleration (out linearAcceleration, transform.position, linearAccelerationSamples);
        Math3d.AngularAcceleration(out angularAcceleration, transform.rotation, angularAccelerationSamples);

        //velocity_r = linearVelocity - Vector3.Cross(angularVelocity, transform.position);

        acceleration = linearAcceleration;
        /*
        Vector3 dir;
        float scale = 2f;
        dir = new Vector3(angularVelocity.x, 0, 0);
        dir = Math3d.SetVectorLength(dir, dir.magnitude * scale);
        dir = gameObject.transform.TransformDirection(dir);
        Debug.DrawRay(gameObject.transform.position, dir, Color.red);
        dir = new Vector3(0, angularVelocity.y, 0);
        dir = Math3d.SetVectorLength(dir, dir.magnitude * scale);
        dir = gameObject.transform.TransformDirection(dir);
        Debug.DrawRay(gameObject.transform.position, dir, Color.green);
        dir = new Vector3(0, 0, angularVelocity.z);
        dir = Math3d.SetVectorLength(dir, dir.magnitude * scale);
        dir = gameObject.transform.TransformDirection(dir);
        Debug.DrawRay(gameObject.transform.position, dir, Color.blue);
        */
        if (pm != null)
			pm.MoverFixedUpdate ();
	}

	public void ResetPlayer()
	{
		pm.transform.position = initPlayerPos;
        pm.transform.rotation = initPlayerRot;
    }
}
