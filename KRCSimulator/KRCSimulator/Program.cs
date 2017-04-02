using System;
using System.Text;
using System.Xml;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace KRCSimulator
{
    public class Utf8StringWriter : StringWriter
    {
        public override Encoding Encoding
        {
            get { return Encoding.UTF8; }
        }
    }

    class Program
    {
        private const int targetPort = 59152;
        private const int myPort = 55566;
        private const String targetAddress = "192.168.2.200";
        //private const String targetAddress = "206.87.211.245";
        //private const String targetAddress = "127.0.0.1";

        private static double X = 331.7, Y = -1.2, Z = 852.0, A = -90.0, B = 0.9, C = -90.0;
        private static double sineValue;
        private static long timestamp;
        private static int testOutput = 1;
        private static int delay = 0;

        static void Main(string[] args)
        {
            Boolean done = false;
            UdpClient udpServer = new UdpClient(myPort);
            //Socket sending_socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            IPAddress send_to_address = IPAddress.Parse(targetAddress);
            IPEndPoint sending_end_point = new IPEndPoint(send_to_address, targetPort);
            //UdpClient sender = new UdpClient();
            //sender.ExclusiveAddressUse = false;
            //sender.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            //sender.Client.Bind(sending_end_point);

            
            //listener.ExclusiveAddressUse = false;
            //listener.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            udpServer.Client.ReceiveTimeout = 1000; // in milliseconds
            //IPEndPoint receiving_end_point = new IPEndPoint(IPAddress.Any, targetPort);
            //listener.Client.Bind(sending_end_point);

            string received_data;
            byte[] receive_byte_array = new byte[2048];

            Console.WriteLine("Simulator started on port " + targetPort);
            int count = 10;

            while (!done)
            {
                if (false)
                {
                    done = true;
                }
                else
                {
                    // Update internal values:
                    timestamp = DateTime.Now.Millisecond;
                    sineValue = Math.Sin(DateTime.Now.Ticks * Math.PI / 20000000);

                    // Sample send message
                    /*
                    string replyMessage = "< Rob Type =\"KUKA\">" +
                        "< RIst X =\"" + X + "\" Y=\"" + Y + "\" Z=\"" + Z + "\" A=\"" + A + "\" B=\"" + B + "\" C=\"" + C + "\"/>" +
                        "< RSol X =\"" + X + "\" Y=\"" + Y + "\" Z=\"" + Z + "\" A=\"" + A + "\" B=\"" + B + "\" C=\"" + C + "\"/>" +
                        "< Delay D =\"" + delay + "\"/>" +
                        "< Tech C11 =\"0.0\" C12=\"0.0\" C13=\"0.0\" C14=\"0.0\" C15=\"0.0\" C16=\"0.0\"  C17=\"0.0\" C18=\"0.0\" C19=\"0.0\" C110=\"0.0\"/>" +
                        "< TestOutput > " + testOutput + " < TestOutput />" +
                        "< SineSource > " + sineValue + " </ SineSource >" +
                        "< IPOC > " + timestamp + " </ IPOC >" +
                        "</ Rob >";
                    */
                    //
                    string replyMessage = "";

                    XmlWriterSettings Settings = new XmlWriterSettings();
                    Settings.OmitXmlDeclaration = true;
                    Settings.ConformanceLevel = ConformanceLevel.Fragment;
                    Settings.NewLineOnAttributes = true;

                    string position = "X =\"" + X + "\" Y=\"" + Y + "\" Z=\"" + Z + "\" A=\"" + A + "\" B=\"" + B + "\" C=\"" + C + "\"";
                    using (TextWriter textWriter = new Utf8StringWriter())
                    {
                        using (XmlWriter xmlWriter = XmlWriter.Create(textWriter, Settings))
                        {
                            xmlWriter.WriteStartElement("Rob");
                            xmlWriter.WriteAttributeString(null, "Type", null, "KUKA");
                            xmlWriter.WriteStartElement("RIst");
                            xmlWriter.WriteAttributeString(null, "X", null, X.ToString());
                            xmlWriter.WriteAttributeString(null, "Y", null, Y.ToString());
                            xmlWriter.WriteAttributeString(null, "Z", null, Z.ToString());
                            xmlWriter.WriteAttributeString(null, "A", null, A.ToString());
                            xmlWriter.WriteAttributeString(null, "B", null, B.ToString());
                            xmlWriter.WriteAttributeString(null, "C", null, C.ToString());
                            xmlWriter.WriteEndElement();
                            xmlWriter.WriteStartElement("RSol");
                            xmlWriter.WriteAttributeString(null, "X", null, X.ToString());
                            xmlWriter.WriteAttributeString(null, "Y", null, Y.ToString());
                            xmlWriter.WriteAttributeString(null, "Z", null, Z.ToString());
                            xmlWriter.WriteAttributeString(null, "A", null, A.ToString());
                            xmlWriter.WriteAttributeString(null, "B", null, B.ToString());
                            xmlWriter.WriteAttributeString(null, "C", null, C.ToString());
                            xmlWriter.WriteEndElement();
                            xmlWriter.WriteStartElement("Delay");
                            xmlWriter.WriteAttributeString(null, "D", null, delay.ToString());
                            xmlWriter.WriteEndElement();
                            xmlWriter.WriteStartElement("Tech");
                            xmlWriter.WriteAttributeString(null, "C11", null, "0.0");
                            xmlWriter.WriteAttributeString(null, "C12", null, "0.0");
                            xmlWriter.WriteAttributeString(null, "C13", null, "0.0");
                            xmlWriter.WriteAttributeString(null, "C14", null, "0.0");
                            xmlWriter.WriteAttributeString(null, "C15", null, "0.0");
                            xmlWriter.WriteAttributeString(null, "C16", null, "0.0");
                            xmlWriter.WriteAttributeString(null, "C17", null, "0.0");
                            xmlWriter.WriteAttributeString(null, "C18", null, "0.0");
                            xmlWriter.WriteAttributeString(null, "C19", null, "0.0");
                            xmlWriter.WriteAttributeString(null, "C110", null, "0.0");
                            xmlWriter.WriteEndElement();
                            xmlWriter.WriteElementString("TestOutput", testOutput.ToString());
                            xmlWriter.WriteElementString("SineSource", string.Format("{0:0.00}", sineValue) );
                            xmlWriter.WriteElementString("IPOC", timestamp.ToString());

                            xmlWriter.WriteEndElement();
                        }
                        replyMessage = textWriter.ToString();
                    }

                    byte[] send_buffer = Encoding.ASCII.GetBytes(replyMessage);
                    
                    Console.WriteLine("Sending to: {0}",
                    sending_end_point.ToString());
                    try
                    {
                        //sending_socket.SendTo(send_buffer, sending_end_point);
                        udpServer.Send(send_buffer, send_buffer.Length, sending_end_point);
                    }
                    catch (Exception send_exception)
                    {
                        Console.WriteLine(send_exception.ToString());
                        break;
                    }

                    // Read reply:
                    try
                    {
                        Console.WriteLine("Waiting for reply");
                        receive_byte_array = udpServer.Receive(ref sending_end_point);
                        Console.WriteLine("Received from {0}", sending_end_point.ToString());
                        received_data = Encoding.ASCII.GetString(receive_byte_array, 0, receive_byte_array.Length);
                        Console.WriteLine("data follows \n\n{0}\n", received_data);

                        XmlDocument doc = new XmlDocument();
                        doc.LoadXml(received_data);
                        XmlNode newPos = doc.DocumentElement.SelectSingleNode("/Sen/RKorr");
                        X = Convert.ToDouble(newPos.Attributes["X"].Value);
                        Y = Convert.ToDouble(newPos.Attributes["Y"].Value);
                        Z = Convert.ToDouble(newPos.Attributes["Z"].Value);
                        A = Convert.ToDouble(newPos.Attributes["A"].Value);
                        B = Convert.ToDouble(newPos.Attributes["B"].Value);
                        C = Convert.ToDouble(newPos.Attributes["C"].Value);
                        XmlNode newTestOutput = doc.DocumentElement.SelectSingleNode("/Sen/TestOutput");
                        Console.WriteLine("New value: " + newTestOutput.InnerText);
                        Console.WriteLine("other values: " + X + " " + Y + " " + Z);
                        testOutput = Convert.ToInt32(newTestOutput.InnerText);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.ToString());
                        break;
                    }
                }
                System.Threading.Thread.Sleep(1);
                count--;
            } // end of while (!done)

            udpServer.Close();

            Console.WriteLine("Done. Press enter to continue...");
            Console.ReadLine();
        }
    }
}
