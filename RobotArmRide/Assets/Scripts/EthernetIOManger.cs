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
    // Use this for initialization
    void Start()
    {
        threadflag = true;
        tid1 = new Thread(Thread1);
        tid1.Priority = System.Threading.ThreadPriority.Highest;
        tid1.Start();
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
        String position = "RKorr X=\"" + fs.transform.position.x + "\" Y=\"" + fs.transform.position.z + "\" Z=\"" + fs.transform.position.y +
                        "\" A=\"" + (fs.transform.eulerAngles.x) + "\" B=\"" + (fs.transform.eulerAngles.y) + "\" C=\"" + (fs.transform.eulerAngles.x) +
                        "\"";
        string text;
        using (TextWriter textWriter = new Utf8StringWriter())
        {
            using (XmlWriter xmlWriter = XmlWriter.Create(textWriter, Settings))
            {
           
                xmlWriter.WriteStartElement("Sen");
                xmlWriter.WriteAttributeString(null, "Type", null, "ImFree");
                xmlWriter.WriteElementString("EStr", "Message from RSI TestServer");
                xmlWriter.WriteElementString(
                "Tech T21=\"1.09\" T22=\"2.08\" T23=\"3.07\" T24=\"4.06\" T25=\"5.05\" T26=\"6.04\" T27=\"7.03\" T28=\"8.02\" T29=\"9.01\" T210=\"10.00\"",
                "");
                xmlWriter.WriteElementString(position, "");
                xmlWriter.WriteElementString("TestOutput", "1");
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
            }
            catch (Exception)
            {
                Debug.LogWarning("Failed to recieve packet!");
                count++;
                continue;
            }

            try
            {

                String position1 = "RKorr X=\"" + fs.transform.position.x + "\" Y=\"" + fs.transform.position.z + "\" Z=\"" + fs.transform.position.y +
                        "\" A=\"" + (fs.transform.eulerAngles.x) + "\" B=\"" + (fs.transform.eulerAngles.y) + "\" C=\"" + (fs.transform.eulerAngles.x) +
                        "\"";
                string text1;
                using (TextWriter textWriter = new Utf8StringWriter())
                {
                    using (XmlWriter xmlWriter = XmlWriter.Create(textWriter, Settings))
                    {
                        xmlWriter.WriteStartElement("Sen");
                        xmlWriter.WriteAttributeString(null, "Type", null, "ImFree");
                        xmlWriter.WriteElementString("EStr", "Message from RSI TestServer");
                        xmlWriter.WriteElementString(
                            "Tech T21=\"1.09\" T22=\"2.08\" T23=\"3.07\" T24=\"4.06\" T25=\"5.05\" T26=\"6.04\" T27=\"7.03\" T28=\"8.02\" T29=\"9.01\" T210=\"10.00\"",
                            "");
                        xmlWriter.WriteElementString(position1, "");
                        xmlWriter.WriteElementString("TestOutput", "1");
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

}