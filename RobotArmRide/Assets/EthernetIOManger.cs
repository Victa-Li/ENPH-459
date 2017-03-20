using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Xml;
using Assets.Scripts;
using UnityEngine;
using UnityEngine.Assertions;

public class EthernetIOManger : MonoBehaviour
{

    private const int targetPort = 59152;
    private const String targetAddress = "192.168.2.100";

    private int myport = 59152;

    private Thread tid1;
    private bool threadflag;
    RobotArmControl robotArm;
    // Use this for initialization
    void Start()
    {
        robotArm = GameObject.Find("Robot Arm").GetComponent<RobotArmControl>();
        threadflag = true;
        tid1 = new Thread(Thread1);
        tid1.Start();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Thread1()
    {
        Boolean done = false;
        Boolean exception_thrown = false;

        Socket sending_socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        IPAddress send_to_address = IPAddress.Parse(targetAddress);
        IPEndPoint sending_end_point = new IPEndPoint(send_to_address, targetPort);

        UdpClient listener = new UdpClient(targetPort);
        listener.Client.ReceiveTimeout = 4; // in milliseconds
        IPEndPoint receiving_end_point = new IPEndPoint(IPAddress.Any, targetPort);
        string received_data;
        byte[] receive_byte_array;

        Console.WriteLine("Simulator started on port " + targetPort);

    
            XmlWriterSettings Settings = new XmlWriterSettings();
            Settings.OmitXmlDeclaration = true;
            Settings.ConformanceLevel = ConformanceLevel.Fragment;
            Settings.NewLineOnAttributes = true;
            String position = "RKorr X=\"" + robotArm.RA_x + "\" Y=\"" + robotArm.RA_y + "\" Z=\"" + robotArm.RA_z +
                              "\" A=\"" + robotArm.RA_pitch + "\" B=\"" + robotArm.RA_roll + "\" C=\"" + robotArm.RA_yaw +
                              "\"";
            using (XmlWriter writer = XmlWriter.Create("robot.xml", Settings))
            {
                //XmlWriterSettings writer.Settings = settings;
                //writer.Settings.OmitXmlDeclaration=true;
                //writer.Settings.ConformanceLevel=ConformanceLevel.Fragment;

                //writer.WriteStartDocument();
                writer.WriteStartElement("Sen");
                writer.WriteAttributeString(null, "Type", null, "ImFree");
                writer.WriteElementString("EStr", "Message from RSI TestServer");
                writer.WriteElementString(
                    "Tech T21=\"1.09\" T22=\"2.08\" T23=\"3.07\" T24=\"4.06\" T25=\"5.05\" T26=\"6.04\" T27=\"7.03\" T28=\"8.02\" T29=\"9.01\" T210=\"10.00\"",
                    "");
                writer.WriteElementString(position, "");
                writer.WriteElementString("TestOutput", "1");
                writer.WriteElementString("IPOC", "398220");
                //                    writer.WriteElementString("Salary", employee.Salary.ToString());
                //
                //writer.WriteEndElement();
                //                }
                //
                writer.WriteEndElement();
                writer.WriteEndDocument();
            }

            string text;
            var fileStream = new FileStream("robot.xml", FileMode.Open, FileAccess.Read);
            using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
            {
                text = streamReader.ReadToEnd();
            }
            fileStream.Close();
           // Console.WriteLine("Sending to: {0}",
            //    sending_end_point.ToString());
            byte[] send_buffer = Encoding.ASCII.GetBytes(text);
            Debug.Log("waiting");
            int count = 0;
            // Read reply:

            while (listener.Available == 0 && threadflag)
            {  }
        
       
        /*
        do
        {
            try
            {
                Console.WriteLine("Waiting for reply");
                receive_byte_array = listener.Receive(ref receiving_end_point);

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());

            }
        } while (listener.Available == 0&&threadflag);
        */
        Debug.Log("receive");
            try
            {
            
            sending_socket.SendTo(send_buffer, sending_end_point);
            }
            catch (Exception send_exception)
            {
                exception_thrown = true;
                Console.WriteLine(send_exception.ToString());
                //Assert.IsTrue(exception_thrown);

            }

            do
            {
                
                    try
                    {
                        Console.WriteLine("Waiting for reply");
                        receive_byte_array = listener.Receive(ref receiving_end_point);
                        Console.WriteLine("Received from {0}", receiving_end_point.ToString());
                        received_data = Encoding.ASCII.GetString(receive_byte_array, 0, receive_byte_array.Length);
                        Console.WriteLine("data follows \n\n{0}\n", received_data);
                        count = 0;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.ToString());
                        //break;
                        count++;
                        continue;
                        
                    }
                
                try
                {
                String position1 = "RKorr X=\"" + robotArm.RA_x + "\" Y=\"" + robotArm.RA_y + "\" Z=\"" + robotArm.RA_z +
                       "\" A=\"" + robotArm.RA_pitch + "\" B=\"" + robotArm.RA_roll + "\" C=\"" + robotArm.RA_yaw +
                       "\"";
                using (XmlWriter writer = XmlWriter.Create("robot.xml", Settings))
                {
                    //XmlWriterSettings writer.Settings = settings;
                    //writer.Settings.OmitXmlDeclaration=true;
                    //writer.Settings.ConformanceLevel=ConformanceLevel.Fragment;

                    //writer.WriteStartDocument();
                    writer.WriteStartElement("Sen");
                    writer.WriteAttributeString(null, "Type", null, "ImFree");
                    writer.WriteElementString("EStr", "Message from RSI TestServer");
                    writer.WriteElementString(
                        "Tech T21=\"1.09\" T22=\"2.08\" T23=\"3.07\" T24=\"4.06\" T25=\"5.05\" T26=\"6.04\" T27=\"7.03\" T28=\"8.02\" T29=\"9.01\" T210=\"10.00\"",
                        "");
                    writer.WriteElementString(position1, "");
                    writer.WriteElementString("TestOutput", "1");
                    writer.WriteElementString("IPOC", "398220");
                    //                    writer.WriteElementString("Salary", employee.Salary.ToString());
                    //
                    //writer.WriteEndElement();
                    //                }
                    //
                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                }

                string text1;
                var fileStream1 = new FileStream("robot.xml", FileMode.Open, FileAccess.Read);
                using (var streamReader = new StreamReader(fileStream1, Encoding.UTF8))
                {
                    text1 = streamReader.ReadToEnd();
                }
                fileStream.Close();
                Console.WriteLine("Sending to: {0}",
                    sending_end_point.ToString());
                byte[] send_buffer1 = Encoding.ASCII.GetBytes(text1);
                sending_socket.SendTo(send_buffer1, sending_end_point);
                }
                catch (Exception send_exception)
                {
                    exception_thrown = true;
                    Console.WriteLine(send_exception.ToString());
                    count++;
                }
            } while (count<100 && threadflag);

        
        //buildconnect.Connect("127.0.0.1", text);
        //192.168.2.100

        listener.Close();

    }



    void OnApplicationQuit()
    {
        threadflag = false;
        tid1.Join();
        tid1.Abort();
    }

}