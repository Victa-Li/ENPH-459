using UnityEngine;

public class followPlayer : MonoBehaviour {

	public GameObject playerObject;

	// Use this for initialization
	void Start () {
		//playerObject = GameObject.FindGameObjectWithTag ("Player");
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void LateUpdate () {
		transform.position = playerObject.transform.position;
	}
}
