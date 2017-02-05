using UnityEngine;

public class EllipticalMover : MonoBehaviour {

	public float xFrequency = 1.0f;
	public float yFrequency = 1.0f;
	public float zFrequency = 0.0f;

	public float xAmplidude = 1.0f;
	public float yAmplidude = 1.0f;
	public float zAmplidude = 1.0f;

	private Vector3 initialPosition;
	// Use this for initialization
	void Start () {
		initialPosition = transform.position;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		transform.position = initialPosition + new Vector3 (xAmplidude * Mathf.Sin (Time.fixedTime * xFrequency * 2 * Mathf.PI),
			yAmplidude * Mathf.Cos (Time.fixedTime * yFrequency * 2 * Mathf.PI),
			zAmplidude * Mathf.Sin (Time.fixedTime * zFrequency * 2 * Mathf.PI));
	}
}
