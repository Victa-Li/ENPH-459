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
        private const int listenPort = 59152;
        private int replyport = 53453; // This value should be updated every packet, it may or may not change
        private const String targetAddress = "192.168.2.100";

        static void Main(string[] args)
        {
            Boolean done = false;
            Boolean exception_thrown = false;

            Socket sending_socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            IPAddress send_to_address = IPAddress.Parse(targetAddress);
            IPEndPoint sending_end_point = new IPEndPoint(send_to_address, listenPort);

            UdpClient listener = new UdpClient(listenPort);
            IPEndPoint groupEP = new IPEndPoint(IPAddress.Any, listenPort);
            string received_data;
            byte[] receive_byte_array;

            Console.WriteLine("Enter text to broadcast via UDP.");
            Console.WriteLine("Enter a blank line to exit the program.");
            while (!done)
            {
                Console.WriteLine("Enter text to send, blank line to quit");
                string text_to_send = Console.ReadLine();
                if (text_to_send.Length == 0)
                {
                    done = true;
                }
                else
                {
                    byte[] send_buffer = Encoding.ASCII.GetBytes(text_to_send);

                    // Remind the user of where this is going.
                    Console.WriteLine("sending to address: {0} port: {1}",
                    sending_end_point.Address,
                    sending_end_point.Port);
                    try
                    {
                        sending_socket.SendTo(send_buffer, sending_end_point);
                    }
                    catch (Exception send_exception)
                    {
                        exception_thrown = true;
                        Console.WriteLine(" Exception {0}", send_exception.Message);
                    }
                    if (exception_thrown == false)
                    {
                        Console.WriteLine("Message has been sent to address: " + targetAddress);
                    }
                    else
                    {
                        exception_thrown = false;
                        Console.WriteLine("The exception indicates the message was not sent.");
                    }
                }
            } // end of while (!done)

            /*
            try
            {
                while (!done)
                {
                    Console.WriteLine("Waiting for broadcast");
                    // this is the line of code that receives the broadcase message.
                    // It calls the receive function from the object listener (class UdpClient)
                    // It passes to listener the end point groupEP.
                    // It puts the data from the broadcast message into the byte array
                    // named received_byte_array.
                    // I don't know why this uses the class UdpClient and IPEndPoint like this.
                    // Contrast this with the talker code. It does not pass by reference.
                    // Note that this is a synchronous or blocking call.
                    receive_byte_array = listener.Receive(ref groupEP);
                    Console.WriteLine("Received a broadcast from {0}", groupEP.ToString());
                    received_data = Encoding.ASCII.GetString(receive_byte_array, 0, receive_byte_array.Length);
                    Console.WriteLine("data follows \n{0}\n\n", received_data);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            listener.Close();
            */
        }
    }
}
