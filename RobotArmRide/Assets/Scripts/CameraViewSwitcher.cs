using UnityEngine;
using System.Collections;

public class CameraViewSwitcher : MonoBehaviour {

	Vector3 PosFirstPerson = new Vector3 (0.0f, 1.0f, 1.4f);
	Vector3 PosThirdPerson = new Vector3 (0.0f, 2.5f, -6.0f);

	bool inFirstPerson = true;

	// Use this for initialization
	void Start () {
		//PosFirstPerson = transform.localPosition;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void SwitchView()
	{
		if (inFirstPerson) {
			inFirstPerson = false;
			transform.localPosition = PosThirdPerson;
		} else {
			inFirstPerson = true;
			transform.localPosition = PosFirstPerson;
		}
	}
}
