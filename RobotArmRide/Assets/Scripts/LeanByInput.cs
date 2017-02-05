using UnityEngine;

public class LeanByInput : MonoBehaviour {

	private const float visualScaleX  = 60.0f;
	private const float visualScaleZ  = 60.0f;
	private const float visualOffsetX = 00.0f;
	private const float visualOffsetZ = 00.0f;

	public float visualRotZ;
	public float visualRotX;

	MovementController mc;

	// Use this for initialization
	void Start () {
		mc = GameObject.FindGameObjectWithTag ("Player").GetComponent<MovementController> ();
	}
	
	// Update is called once per frame
	void Update () {
		visualRotX = mc.rawInputHoriz * visualScaleX + visualOffsetX;
		visualRotZ = mc.rawInputVert * visualScaleZ + visualOffsetZ;

		Vector3 rawIn = new Vector3 (visualRotX, 0.0f, visualRotZ);

		transform.rotation = Quaternion.LookRotation (rawIn); // * Quaternion.LookRotation (mc.transform.forward, mc.transform.up);
	}

	void FixedUpdate()
	{

	}
}
