using UnityEngine;
using UnityStandardAssets.Vehicles.Car;

[RequireComponent(typeof(CarController))]
public class FourWheeledMovement : PlayerMover {

	MovementController mc;
    
    private CarController m_Car; // the car controller we want to use

    private void Awake()
    {
        // get the car controller
        m_Car = GetComponent<CarController>();
    }
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
        float moveHorizontal = mc.rawInputHoriz;
        float moveVertical = mc.rawInputVert;
        float handbrake = mc.handbrake;

        // pass the input to the car!
        m_Car.Move(moveHorizontal, moveVertical, moveVertical, handbrake);
    }
}
