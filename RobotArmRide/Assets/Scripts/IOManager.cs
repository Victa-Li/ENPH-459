using System;
using System.IO.Ports;
using System.Threading;

using UnityEngine;
using System.Collections;

public class IOManager : MonoBehaviour {

	bool _continue;
	SerialPort _serialPort;
	string message;
	StringComparer stringComparer;
	Thread readThread;
	bool lightOn = false;
	byte byteToSend;
	byte[] sendBuffer = new byte[7];
	bool _connected = false;

	float timeLast = 0.0f;
	float updateTime = 0.02f;

//	private const float accelerationXscale = 60.0f;
//	private const float accelerationZscale = 60.0f;
//	private const float accelerationXoffset = 60.0f;
//	private const float accelerationZoffset = 60.0f;

	public float exaggerationScale = 1.0f;

	public int axis_base_trim = 7;
	public int axis_lower_trim = -26;
	public int axis_upper_trim = 68;
	public int axis_wrist_trim = -14;
	public int axis_seat_trim = 37;

	public int servo_base = 90;
	public int servo_lower = 90;
	public int servo_upper = 90;
	public int servo_wrist = 90;
	public int servo_seat = 90;

	GameObject movementController;
	//GameObject accelerationController;
	RobotArmControl robotArm;

	// Use this for initialization
	void Start () {

		//stringComparer = StringComparer.OrdinalIgnoreCase;
		readThread = new Thread(Read);

		// Create a new SerialPort object with default settings.
		_serialPort = new SerialPort();

		// Allow the user to set the appropriate properties.
		_serialPort.PortName = _serialPort.PortName;
		_serialPort.BaudRate = 115200;
		_serialPort.Parity = _serialPort.Parity;
		_serialPort.DataBits = _serialPort.DataBits;
		_serialPort.StopBits = _serialPort.StopBits;
		_serialPort.Handshake = _serialPort.Handshake;

		_serialPort.NewLine = "\r\n";

		// Set the read/write timeouts
		_serialPort.ReadTimeout = 500;
		_serialPort.WriteTimeout = 500;

		_serialPort.Open ();
		_continue = true;

		readThread.Start();
		_connected = true;

		//movementController = GameObject.FindGameObjectWithTag ("MovementController");
		//accelerationController = GameObject.FindGameObjectWithTag ("AccelerationController");

		robotArm = GameObject.Find("Robot Arm").GetComponent<RobotArmControl>();

//		while (true) {
//			// Start read coroutine
//			StartCoroutine
//			(
//				AsynchronousReadFromArduino
//				(   (string s) => Debug.Log(s),     // Callback
//					() => Debug.LogError("Error!"), // Error callback
//					10f                             // Timeout (seconds)
//				)
//			);
//		}
	}
	
	// Update is called once per frame
	void Update () {
		if (!_connected) {
//			// Wait for connetion to establish
//			while (true)
//			{
//				try
//				{
//					char[] inputBuffer = new char[3];
//					_serialPort.Read(inputBuffer, 0, 3);
//					Debug.Log("Read: \"" + inputBuffer[0] + "\"");
//					if (inputBuffer[0] == 'A')
//					{
//						_serialPort.Write("B");
//						_serialPort.BaseStream.Flush ();
//						_connected = true;
//						break;
//					}
//				}
//				catch (TimeoutException) {}
//			}
		} else {
			if (timeLast + updateTime < Time.time) {
				
				timeLast = Time.time;

//				Leaning leaningScript = movementController.GetComponent<Leaning>();
//				scaleByAcceleration sbaScript = accelerationController.GetComponent<scaleByAcceleration>();

				servo_base = Math.Max(Math.Min((int)Mathf.Floor (robotArm.axis_base) + axis_base_trim, 180), 0);
				servo_lower = Math.Max(Math.Min((int)Mathf.Floor (robotArm.axis_lower) + axis_lower_trim, 180), 0);
				servo_upper = Math.Max(Math.Min((int)Mathf.Floor (robotArm.axis_upper) + axis_upper_trim, 180), 0);
				servo_wrist = Math.Max(Math.Min((int)Mathf.Floor (robotArm.axis_wrist) + axis_wrist_trim, 180), 0);
				servo_seat = Math.Max(Math.Min((int)Mathf.Floor (robotArm.axis_seat) + axis_seat_trim, 180), 0);

				sendBuffer [0] = (byte)'S';
//				sendBuffer [1] = (byte) (int)Math.Floor(leaningScript.afkRotX);
//				sendBuffer [2] = (byte) (int)Math.Floor(leaningScript.afkRotZ);
//				sendBuffer [1] = (byte) (int)Math.Floor(sbaScript.playerAcceleration.x * accelerationXscale + accelerationXoffset);
//				sendBuffer [2] = (byte) (int)Math.Floor(sbaScript.playerAcceleration.z * accelerationZscale + accelerationZoffset);
				sendBuffer [1] = (byte) servo_base;
				sendBuffer [2] = (byte) servo_lower;
				sendBuffer [3] = (byte) servo_upper;
				sendBuffer [4] = (byte) servo_wrist;
				sendBuffer [5] = (byte) servo_seat;
				sendBuffer [6] = (byte)'\n';
				_serialPort.Write (sendBuffer, 0, sendBuffer.Length);
				//_serialPort.BaseStream.Flush ();
				//Debug.Log ("Sent angle: " + sendBuffer [1]);
			}
		}
	}

	void OnApplicationQuit() {
		if (_continue && _connected) {
			_continue = false;
			readThread.Join();
			_serialPort.Close();
		}
		Debug.Log("Application ending after " + Time.time + " seconds");
	}

	public void buttonPressed()
	{
		if (!lightOn) {
			byteToSend = 1;
			lightOn = true;
		} else {
			byteToSend = 0;
			lightOn = false;
		}
		sendBuffer [0] = byteToSend;
		sendBuffer [1] = 10;
		_serialPort.Write (sendBuffer, 0, 2);
		_serialPort.BaseStream.Flush ();
		Debug.Log ("sent message: \"" + byteToSend + "\"");

//		if (!lightOn) {
//			message = "ON";
//			lightOn = true;
//		} else {
//			message = "OFF";
//			lightOn = false;
//		}
//		_serialPort.WriteLine (message);
//		Debug.Log ("sent message: \"" + message + "\"");
	}

	public void stopSerial()
	{
		_continue = false;
		readThread.Join();
		_serialPort.Close();
		Debug.Log ("Stopped serial connection");
	}

	public void Read()
	{
		while (_continue)
		{
			if (_connected) {
				try
				{
					string inputString = _serialPort.ReadLine();
					Debug.Log("Read: \"" + inputString + "\"");
//					int inputByte = _serialPort.ReadByte();
//					Debug.Log("Read byte: \"" + inputByte.ToString() + "\"");
				}
				catch (TimeoutException) { }
			}
		}
	}

	public IEnumerator AsynchronousReadFromArduino(Action<string> callback, Action fail = null, float timeout = float.PositiveInfinity) {
		DateTime initialTime = DateTime.Now;
		DateTime nowTime;
		TimeSpan diff = default(TimeSpan);

		string dataString = null;

		do {
			try {
				dataString = _serialPort.ReadLine();
			}
			catch (TimeoutException) {
				dataString = null;
			}

			if (dataString != null)
			{
				callback(dataString);
				yield return null;
			} else
				yield return new WaitForSeconds(0.05f);

			nowTime = DateTime.Now;
			diff = nowTime - initialTime;

		} while (diff.Milliseconds < timeout);

		if (fail != null)
			fail();
		yield return null;
	}

	public void Update_Base_Trim(float newValue)
	{
		axis_base_trim = (int)newValue;
	}
	public void Update_Lower_Trim(float newValue)
	{
		axis_lower_trim = (int)newValue;
	}
	public void Update_Upper_Trim(float newValue)
	{
		axis_upper_trim = (int)newValue;
	}
	public void Update_Wrist_Trim(float newValue)
	{
		axis_wrist_trim = (int)newValue;
	}
	public void Update_Seat_Trim(float newValue)
	{
		axis_seat_trim = (int)newValue;
	}
}

//public class PortChat
//{
//	static bool _continue;
//	static SerialPort _serialPort;
//
//	public static void Main()
//	{
//		string name;
//		string message;
//		StringComparer stringComparer = StringComparer.OrdinalIgnoreCase;
//		Thread readThread = new Thread(Read);
//
//		// Create a new SerialPort object with default settings.
//		_serialPort = new SerialPort();
//
//		// Allow the user to set the appropriate properties.
//		_serialPort.PortName = SetPortName(_serialPort.PortName);
//		_serialPort.BaudRate = SetPortBaudRate(_serialPort.BaudRate);
//		_serialPort.Parity = SetPortParity(_serialPort.Parity);
//		_serialPort.DataBits = SetPortDataBits(_serialPort.DataBits);
//		_serialPort.StopBits = SetPortStopBits(_serialPort.StopBits);
//		_serialPort.Handshake = SetPortHandshake(_serialPort.Handshake);
//
//		// Set the read/write timeouts
//		_serialPort.ReadTimeout = 500;
//		_serialPort.WriteTimeout = 500;
//
//		_serialPort.Open();
//		_continue = true;
//		readThread.Start();
//
//		Console.Write("Name: ");
//		name = "";//Console.ReadLine();
//
//		Console.WriteLine("Type QUIT to exit");
//
//		while (_continue)
//		{
//			message = "";//Console.ReadLine();
//
//			if (stringComparer.Equals("quit", message))
//			{
//				_continue = false;
//			}
//			else
//			{
//				_serialPort.WriteLine(
//					String.Format("<{0}>: {1}", name, message));
//			}
//		}
//
//		readThread.Join();
//		_serialPort.Close();
//	}
//
//	public static void Read()
//	{
//		while (_continue)
//		{
//			try
//			{
//				string message = _serialPort.ReadLine();
//				Console.WriteLine(message);
//			}
//			catch (TimeoutException) { }
//		}
//	}
//
//	// Display Port values and prompt user to enter a port.
//	public static string SetPortName(string defaultPortName)
//	{
//		string portName;
//
//		Console.WriteLine("Available Ports:");
//		foreach (string s in SerialPort.GetPortNames())
//		{
//			Console.WriteLine("   {0}", s);
//		}
//
//		Console.Write("Enter COM port value (Default: {0}): ", defaultPortName);
//		portName = Console.ReadLine();
//
//		if (portName == "" || !(portName.ToLower()).StartsWith("com"))
//		{
//			portName = defaultPortName;
//		}
//		return portName;
//	}
//	// Display BaudRate values and prompt user to enter a value.
//	public static int SetPortBaudRate(int defaultPortBaudRate)
//	{
//		string baudRate;
//
//		Console.Write("Baud Rate(default:{0}): ", defaultPortBaudRate);
//		baudRate = Console.ReadLine();
//
//		if (baudRate == "")
//		{
//			baudRate = defaultPortBaudRate.ToString();
//		}
//
//		return int.Parse(baudRate);
//	}
//
//	// Display PortParity values and prompt user to enter a value.
//	public static Parity SetPortParity(Parity defaultPortParity)
//	{
//		string parity;
//
//		Console.WriteLine("Available Parity options:");
//		foreach (string s in Enum.GetNames(typeof(Parity)))
//		{
//			Console.WriteLine("   {0}", s);
//		}
//
//		Console.Write("Enter Parity value (Default: {0}):", defaultPortParity.ToString(), true);
//		parity = Console.ReadLine();
//
//		if (parity == "")
//		{
//			parity = defaultPortParity.ToString();
//		}
//
//		return (Parity)Enum.Parse(typeof(Parity), parity, true);
//	}
//	// Display DataBits values and prompt user to enter a value.
//	public static int SetPortDataBits(int defaultPortDataBits)
//	{
//		string dataBits;
//
//		Console.Write("Enter DataBits value (Default: {0}): ", defaultPortDataBits);
//		dataBits = Console.ReadLine();
//
//		if (dataBits == "")
//		{
//			dataBits = defaultPortDataBits.ToString();
//		}
//
//		return int.Parse(dataBits.ToUpperInvariant());
//	}
//
//	// Display StopBits values and prompt user to enter a value.
//	public static StopBits SetPortStopBits(StopBits defaultPortStopBits)
//	{
//		string stopBits;
//
//		Console.WriteLine("Available StopBits options:");
//		foreach (string s in Enum.GetNames(typeof(StopBits)))
//		{
//			Console.WriteLine("   {0}", s);
//		}
//
//		Console.Write("Enter StopBits value (None is not supported and \n" +
//			"raises an ArgumentOutOfRangeException. \n (Default: {0}):", defaultPortStopBits.ToString());
//		stopBits = Console.ReadLine();
//
//		if (stopBits == "" )
//		{
//			stopBits = defaultPortStopBits.ToString();
//		}
//
//		return (StopBits)Enum.Parse(typeof(StopBits), stopBits, true);
//	}
//	public static Handshake SetPortHandshake(Handshake defaultPortHandshake)
//	{
//		string handshake;
//
//		Console.WriteLine("Available Handshake options:");
//		foreach (string s in Enum.GetNames(typeof(Handshake)))
//		{
//			Console.WriteLine("   {0}", s);
//		}
//
//		Console.Write("Enter Handshake value (Default: {0}):", defaultPortHandshake.ToString());
//		handshake = Console.ReadLine();
//
//		if (handshake == "")
//		{
//			handshake = defaultPortHandshake.ToString();
//		}
//
//		return (Handshake)Enum.Parse(typeof(Handshake), handshake, true);
//	}
//}