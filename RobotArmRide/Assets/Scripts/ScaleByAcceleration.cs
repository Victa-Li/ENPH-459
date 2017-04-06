using UnityEngine;

public class ScaleByAcceleration : MonoBehaviour {

	private float scaleFactor = 1.0f;

	MovementController mc;

	// Use this for initialization
	void Start () {
		mc = GameObject.FindGameObjectWithTag ("Player").GetComponent<MovementController> ();
	}

	// Update is called once per frame
	void Update () {
		transform.localScale = new Vector3 (1.0f, 1.0f, mc.acceleration.magnitude * scaleFactor);

		if (mc.acceleration.magnitude > 0.001) {
			transform.rotation = Quaternion.LookRotation (mc.acceleration, Vector3.up); //.Euler (new Vector3 (0.0f, rotationY, rotationZ));
		}
	}

	void FixedUpdate()
	{

	}
}
