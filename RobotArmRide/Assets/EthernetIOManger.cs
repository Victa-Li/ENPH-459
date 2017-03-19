using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Xml;
using Assets.Scripts;
using UnityEngine;

public class EthernetIOManger : MonoBehaviour {
 
    private Thread tid1;
    private bool threadflag;
    RobotArmControl robotArm;
    // Use this for initialization
    void Start () {
        robotArm= GameObject.Find("Robot Arm").GetComponent<RobotArmControl>();
        threadflag = true;
        tid1=new Thread(Thread1);
        tid1.Start();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Thread1()
    {

        while (threadflag)
        {
            XmlWriterSettings Settings = new XmlWriterSettings();
            Settings.OmitXmlDeclaration = true;
            Settings.ConformanceLevel = ConformanceLevel.Fragment;

            String position = "RKorr X=\"" + robotArm.RA_x + "\" Y=\"" + robotArm.RA_y + "\" Z=\"" + robotArm.RA_z + "\" A=\"" + robotArm.RA_pitch + "\" B=\"" + robotArm.RA_roll + "\" C=\"" + robotArm.RA_yaw + "\"";
            using (XmlWriter writer = XmlWriter.Create("robot.xml", Settings))
            {
                //XmlWriterSettings writer.Settings = settings;
                //writer.Settings.OmitXmlDeclaration=true;
                //writer.Settings.ConformanceLevel=ConformanceLevel.Fragment;

                //writer.WriteStartDocument();
                writer.WriteStartElement("Sen");
                writer.WriteAttributeString(null, "Type", null, "ImFree");
                writer.WriteElementString("EStr", "Message from RSI TestServer");
                writer.WriteElementString("Tech T21=\"1.09\" T22=\"2.08\" T23=\"3.07\" T24=\"4.06\" T25=\"5.05\" T26=\"6.04\" T27=\"7.03\" T28=\"8.02\" T29=\"9.01\" T210=\"10.00\"", "");
                writer.WriteElementString(position, "");
                writer.WriteElementString("DiO", "125");
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

            buildconnect.Connect("127.0.0.1", text);
            //192.168.2.100
            fileStream.Close();
        }

    }

    void OnApplicationQuit()
    {
        threadflag = false;
        tid1.Join();
        tid1.Abort();
    }
}
