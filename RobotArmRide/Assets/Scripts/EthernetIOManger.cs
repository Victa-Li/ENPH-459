using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Xml;
using UnityEngine;

public class Utf8StringWriter : StringWriter
{
    public override Encoding Encoding
    {
        get { return Encoding.UTF8; }
    }
}

public class EthernetIOManger : MonoBehaviour
{
    //private int targetPort = 0;
    private const int maxRetries = 100;
    private const int myPort = 59152;
    private const String targetAddress = "192.168.2.100";
    private string timestamp;

    private Thread tid1;
    private bool threadflag;
    public ForceSimulator fs;
    public static string testoutput;
    public static string testsend;

    // Parameters used for limiting
    private const float RSIStep = 0.004f; // in seconds
    private const float maxJerk = 10f; // mm / s^3
    private const float maxAccel = 10f; // mm / s^2
    private const float maxVel = 3f; // mm / cycle
    private const float maxJerkAngle = 10f; // deg / s^3
    private const float maxAccelAngle = 10f; // deg / s^2
    private const float maxVelAngle = 0.05f; // deg / cycle
    //private float currentJerk;
    //private float currentAccel;
    //private float currentVel;
    private const bool useJerk = false;
    private Vector3 lastPosition;
    private Vector3 lastRotation;

    private Vector3 pos_copy;
    private Vector3 rot_copy;
    private Vector3 rot_last = Vector3.zero;
    private Vector3 pos_last = Vector3.zero;

    // Use this for initialization
    void Start()
    {
        threadflag = true;
        tid1 = new Thread(Thread1);
        tid1.Priority = System.Threading.ThreadPriority.Highest;
        tid1.Start();
        testoutput = "0";
        testsend = "0";
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Thread1()
    {
        Debug.Log("Thread started");
        
        IPAddress send_to_address = IPAddress.Parse(targetAddress);
        IPEndPoint sending_end_point = new IPEndPoint(send_to_address, 0);

        UdpClient udpServer = new UdpClient(myPort);
        udpServer.Client.ReceiveTimeout = 10; // in milliseconds
        
        string received_data;
        byte[] receive_byte_array;

        Debug.Log("Listening on port " + sending_end_point.Port);

        
        int count = 0;
        // Read reply:

        while (udpServer.Available == 0 && threadflag)
        {  }

        string[] stringSeparators = new string[] { "<IPOC>", "</IPOC>" };
        string[] stringSeparators1 = new string[] { "<TestOutput>", "</TestOutput>" };

        try
        {
            receive_byte_array = udpServer.Receive(ref sending_end_point);
            received_data = Encoding.ASCII.GetString(receive_byte_array, 0, receive_byte_array.Length);
            
            string[] temp = received_data.Split(stringSeparators, StringSplitOptions.None);
            timestamp = temp[1];

            Debug.Log("received");
        }
        catch (Exception e)
        {
            Debug.LogError(e.ToString());
        }

        XmlWriterSettings Settings = new XmlWriterSettings();
        Settings.OmitXmlDeclaration = true;
        Settings.ConformanceLevel = ConformanceLevel.Fragment;
        Settings.NewLineOnAttributes = true;
        Settings.Indent = true;
        Settings.IndentChars = "";
        String position = "RKorr X=\"" + 0 + "\" Y=\"" + 0 + "\" Z=\"" + 0 +
                           "\" A=\"" + 0 + "\" B=\"" + 0 + "\" C=\"" + 0 +
                           "\"";
        string text;
        using (TextWriter textWriter = new Utf8StringWriter())
        {
            using (XmlWriter xmlWriter = XmlWriter.Create(textWriter, Settings))
            {
           
                xmlWriter.WriteStartElement("Sen");
                xmlWriter.WriteAttributeString(null, "Type", null, "ImFree");
                xmlWriter.WriteElementString("EStr", "Message from Unity");
                xmlWriter.WriteElementString(
                "Tech T21=\"1.09\" T22=\"2.08\" T23=\"3.07\" T24=\"4.06\" T25=\"5.05\" T26=\"6.04\" T27=\"7.03\" T28=\"8.02\" T29=\"9.01\" T210=\"10.00\"",
                "");
                xmlWriter.WriteElementString(position, "");
                xmlWriter.WriteElementString("TestOutput", testsend);
                xmlWriter.WriteElementString("IPOC", timestamp);
                
                xmlWriter.WriteEndElement();
            }
            text = textWriter.ToString();
        }

        byte[] send_buffer = Encoding.ASCII.GetBytes(text);

        try
        {
            udpServer.Send(send_buffer, send_buffer.Length, sending_end_point);
        
        }
        catch (Exception e)
        {
            Debug.LogError(e.ToString());
        }

        do
        {

            try
            {
                
                receive_byte_array = udpServer.Receive(ref sending_end_point);
                
                received_data = Encoding.ASCII.GetString(receive_byte_array, 0, receive_byte_array.Length);
                
                count = 0;
                string[] temp = received_data.Split(stringSeparators, StringSplitOptions.None);
                timestamp = temp[1];
                string[] temp1 = received_data.Split(stringSeparators1, StringSplitOptions.None);
                testoutput = temp1[1];

                XmlDocument doc = new XmlDocument();
                doc.LoadXml(received_data);
                XmlNode newPos = doc.DocumentElement.SelectSingleNode("/Rob/RIst");
                FeedBackSimulateOnCube.x = (float)Convert.ToDouble(newPos.Attributes["X"].Value);
                FeedBackSimulateOnCube.z = (float)Convert.ToDouble(newPos.Attributes["Y"].Value);
                FeedBackSimulateOnCube.y = (float)Convert.ToDouble(newPos.Attributes["Z"].Value);
                FeedBackSimulateOnCube.angleA = (float)Convert.ToDouble(newPos.Attributes["A"].Value);
                FeedBackSimulateOnCube.angleB = (float)Convert.ToDouble(newPos.Attributes["B"].Value);
                FeedBackSimulateOnCube.angleC = (float)Convert.ToDouble(newPos.Attributes["C"].Value);
            }
            catch (Exception)
            {
                Debug.LogWarning("Failed to recieve packet!");
                count++;
                continue;
            }

            try
            {
                //lock (fs.pos_rot_Lock)
                //{
                    pos_copy = fs.current_position;
                    pos_copy.y -= 0.89f;
                    pos_copy *= 200;
                    rot_copy = fs.current_rotation.eulerAngles;
                //}
                // Limit position and rotation updates
                Vector3 newPosition;
                Vector3 newRotation;
                if (false)
                {
                    newPosition = pos_copy;
                    newRotation = rot_copy;
                    // TODO: implement this once inverse kinematics is done
                }
                else
                {
                    newPosition = Vector3.MoveTowards(lastPosition, pos_copy, maxVel);
                    //if (!(Mathf.Approximately(newPosition.x, lastPosition.x) &&
                    //    Mathf.Approximately(newPosition.y, lastPosition.y) &&
                    //    Mathf.Approximately(newPosition.z, lastPosition.z)))
                    //{
                    //    Debug.LogWarning("Limited position!");
                    //}
                    // Rotate angles, MoveTowardsAngle should take of rotation of angles around 360
                    if (Mathf.Abs(rot_copy.x - rot_last.x) > 180)
                        rot_copy.x -= 360 * Mathf.Sign(rot_copy.x - rot_last.x);
                    if (Mathf.Abs(rot_copy.y - rot_last.y) > 180)
                        rot_copy.y -= 360 * Mathf.Sign(rot_copy.y - rot_last.y);
                    if (Mathf.Abs(rot_copy.z - rot_last.z) > 180)
                        rot_copy.z -= 360 * Mathf.Sign(rot_copy.z - rot_last.z);
                    rot_last = rot_copy;
                    newRotation = Vector3.zero;
                    newRotation.x = Mathf.MoveTowardsAngle(lastRotation.x, rot_copy.x, maxVelAngle);
                    newRotation.y = Mathf.MoveTowardsAngle(lastRotation.y, rot_copy.y, maxVelAngle);
                    newRotation.z = Mathf.MoveTowardsAngle(lastRotation.z, rot_copy.z, maxVelAngle);

                    //if (!(Mathf.Approximately(newRotation.x, lastRotation.x) &&
                    //    Mathf.Approximately(newRotation.y, lastRotation.y) &&
                    //    Mathf.Approximately(newRotation.z, lastRotation.z)))
                    //{
                    //    Debug.LogWarning("Limited rotation!");
                    //}
                    lastPosition = newPosition;
                    lastRotation = newRotation;
                }
                // Generate string to send
                String position1 = "RKorr X=\"" + string.Format("{0:0.00}", newPosition.x) + "\" Y=\"" + string.Format("{0:0.00}", newPosition.z) + "\" Z=\"" + string.Format("{0:0.00}", newPosition.y) +
                           "\" A=\"" + string.Format("{0:0.00}", newRotation.x) + "\" B=\"" + string.Format("{0:0.00}", newRotation.y) + "\" C=\"" + string.Format("{0:0.00}", newRotation.z) +
                           "\"";

                string text1;
                using (TextWriter textWriter = new Utf8StringWriter())
                {
                    using (XmlWriter xmlWriter = XmlWriter.Create(textWriter, Settings))
                    {
                        xmlWriter.WriteStartElement("Sen");
                        xmlWriter.WriteAttributeString(null, "Type", null, "ImFree");
                        xmlWriter.WriteElementString("EStr", "Message from Unity");
                        xmlWriter.WriteElementString(
                            "Tech T21=\"1.09\" T22=\"2.08\" T23=\"3.07\" T24=\"4.06\" T25=\"5.05\" T26=\"6.04\" T27=\"7.03\" T28=\"8.02\" T29=\"9.01\" T210=\"10.00\"",
                            "");
                        xmlWriter.WriteElementString(position1, "");
                        xmlWriter.WriteElementString("TestOutput", testsend);
                        xmlWriter.WriteElementString("IPOC", timestamp);
                      
                        xmlWriter.WriteEndElement();
                    }
                    text1 = textWriter.ToString();
                }
                Debug.Log("Sending to: " + sending_end_point.ToString());
                byte[] send_buffer1 = Encoding.ASCII.GetBytes(text1);
                udpServer.Send(send_buffer1, send_buffer1.Length, sending_end_point);
               
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
                count++;
            }
        } while (threadflag);

   

        udpServer.Close();

    }



    void OnApplicationQuit()
    {
        threadflag = false;
        tid1.Join();
        tid1.Abort();
    }

    public void setTestOutput()
    {
        if (testsend.Equals("0"))
        {
            testsend = "1";
        }
        else
        {
            testsend = "0";
        }
    }

}