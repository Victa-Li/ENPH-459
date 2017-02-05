using UnityEngine;
using System.Collections;

/// <summary>
/// Generic movement controller for the player. Should be inherited for a specific vehicle type.
/// </summary>
public class MovementController : MonoBehaviour {

	Vector3 initPlayerPos;

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

	public int accelerationSamples = 10;

	public PlayerMover pm; 

	// Use this for initialization
	void Start () {
		pm = GetComponent<PlayerMover> ();
		initPlayerPos = pm.transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		rawInputHoriz = Input.GetAxis ("Horizontal");
		rawInputVert = Input.GetAxis ("Vertical");
		handbrake = Input.GetAxis ("Jump");
	}

	void FixedUpdate() {
		Math3d.LinearAcceleration (out acceleration, transform.position, accelerationSamples);
		if (pm != null)
			pm.MoverFixedUpdate ();
	}

	public void ResetPlayer()
	{
		pm.transform.position = initPlayerPos;
	}
}
