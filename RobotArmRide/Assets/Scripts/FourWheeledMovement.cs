using UnityEngine;
using System.Collections;

public class FourWheeledMovement : PlayerMover {

	MovementController mc;

	// Use this for initialization
	void Start () {
		mc = GetComponent<MovementController> ();
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


	}
}
