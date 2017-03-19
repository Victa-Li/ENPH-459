using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Net;
using System.Net.Sockets;

namespace KRCSimulator
{
    class Program
    {
        private const int targetPort = 59152;
        private const String targetAddress = "206.12.45.26"; //"192.168.2.100";

        private static double X = 331.7, Y = -1.2, Z = 852.0, A = -90.0, B = 0.9, C = -90.0;
        private static double sineValue;
        private static long timestamp;
        private static int testOutput = 1;
        private static int delay = 0;

        static void Main(string[] args)
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
            int count = 10;

            while (!done)
            {
                if (count < 0)
                {
                    done = true;
                }
                else
                {
                    // Update internal values:
                    timestamp = DateTime.Now.Second;
                    sineValue = Math.Sin(DateTime.Now.Millisecond * Math.PI / 2000);

                    // Sample send message
                    string replyMessage = "< Rob Type =\"KUKA\">" +
                        "< RIst X =\"" + X + "\" Y=\"" + Y + "\" Z=\"" + Z + "\" A=\"" + A + "\" B=\"" + B + "\" C=\"" + C + "\"/>" +
                        "< RSol X =\"" + X + "\" Y=\"" + Y + "\" Z=\"" + Z + "\" A=\"" + A + "\" B=\"" + B + "\" C=\"" + C + "\"/>" +
                        "< Delay D =\"" + delay + "\"/>" +
                        "< Tech C11 =\"0.0\" C12=\"0.0\" C13=\"0.0\" C14=\"0.0\" C15=\"0.0\" C16=\"0.0\"  C17=\"0.0\" C18=\"0.0\" C19=\"0.0\" C110=\"0.0\"/>" +
                        "< TestOutput > " + testOutput + " < TestOutput />" +
                        "< SineSource > " + sineValue + " </ SineSource >" +
                        "< IPOC > " + timestamp + " </ IPOC >" +
                        "</ Rob >";


                    byte[] send_buffer = Encoding.ASCII.GetBytes(replyMessage);
                    
                    Console.WriteLine("Sending to: {0}",
                    sending_end_point.ToString());
                    try
                    {
                        sending_socket.SendTo(send_buffer, sending_end_point);
                    }
                    catch (Exception send_exception)
                    {
                        exception_thrown = true;
                        Console.WriteLine(send_exception.ToString());
                        break;
                    }

                    // Read reply:
                    try
                    {
                        Console.WriteLine("Waiting for reply");
                        receive_byte_array = listener.Receive(ref receiving_end_point);
                        Console.WriteLine("Received from {0}", receiving_end_point.ToString());
                        received_data = Encoding.ASCII.GetString(receive_byte_array, 0, receive_byte_array.Length);
                        Console.WriteLine("data follows \n\n{0}\n", received_data);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.ToString());
                        break;
                    }
                }
                System.Threading.Thread.Sleep(1000);
                count--;
            } // end of while (!done)

            listener.Close();

            Console.WriteLine("Done. Press enter to continue...");
            Console.ReadLine();
        }
    }
}
