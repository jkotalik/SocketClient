using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SocketClient
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            try
            {
                while (true)
                {
                    // Setting up address stuff
                    var ipAddress = IPAddress.Loopback;
                    var ipe = new IPEndPoint(ipAddress, 50690);
                    var socket = new Socket(ipe.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                    socket.Connect(ipe);

                    var testString = "Hello the server should deadlock from this";
                    var request = $"POST / HTTP/1.0\r\n" +
                            $"Host: localhost\r\n" +
                            $"Content-Length: {testString.Length}" +
                            "\r\n\r\n";
                    var bodyBytesToSend = Encoding.ASCII.GetBytes(testString);
                    var requestBytes = Encoding.ASCII.GetBytes(request);

                    var bytes = 0;
                    while ((bytes += socket.Send(requestBytes, bytes, 1, SocketFlags.None)) < requestBytes.Length)
                    {
                    }
                    Console.WriteLine("Done with headers, requesting bytes.");

                    // Expect bytes back
                    var buffer = new byte[1];
                    // Should deadlock here if the server is not canceling the read.
                    while (socket.Receive(buffer) != 0)
                    {
                        Console.Write((char)buffer[0]);
                        Console.Out.Flush();
                    }

                    Console.Write((char)buffer[0]);
                    Console.Out.Flush();

                    bytes = 0;
                    while ((bytes += socket.Send(bodyBytesToSend, bytes, 1, SocketFlags.None)) < bodyBytesToSend.Length)
                    {
                        Console.WriteLine($"Wrote a byte! {(char)bodyBytesToSend[bytes - 1]}");
                    }

                    while (socket.Receive(buffer) != 0)
                    {
                        Console.Write((char)buffer[0]);
                        Console.Out.Flush();
                    }
                    Console.WriteLine("Done reading stream");
                    Console.ReadLine();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");

            }
        }
    }
}
