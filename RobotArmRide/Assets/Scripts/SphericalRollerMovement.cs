using UnityEngine;

public class SphericalRollerMovement : PlayerMover {

	public float forceMultiplier = 5;

	MovementController mc;
	Rigidbody rb;

	// Use this for initialization
	void Start () {
		mc = GetComponent<MovementController> ();
		rb = GetComponent<Rigidbody> ();
	}

	// Update is called once per frame
	void Update () {

	}
	/// <summary>
	/// Called by the movement controller at the end of every fixed update call
	/// </summary>
	public override void MoverFixedUpdate()
	{
		base.MoverFixedUpdate ();

		float moveHorizontal = mc.rawInputHoriz;
		float moveVertical = mc.rawInputVert;

		Vector3 movement = new Vector3 (moveHorizontal, 0.0f, moveVertical);
		rb.AddForce (movement * forceMultiplier);
	}
}
