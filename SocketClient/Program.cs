using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SocketClient
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            try
            {
                var ipAddress = IPAddress.Loopback;
                var ipe = new IPEndPoint(ipAddress, 50690);
                var socket = new Socket(ipe.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                socket.Connect(ipe);
                var testString = "hello world";
                var request = $"POST / HTTP/1.0\r\n" +
                        $"Host: localhost\r\n" +
                        $"Content-Length: {testString.Length}" +
                        "\r\n\r\n" +
                        testString;
                var bytesToSend = Encoding.ASCII.GetBytes(request);
                var bytes = 0;
                while ((bytes += socket.Send(bytesToSend, bytes, 1, SocketFlags.None)) < bytesToSend.Length)
                {
                    Console.WriteLine($"Wrote a byte! {(char)bytesToSend[bytes - 1]}");
                    await Task.Delay(100);
                }

                var buffer = new byte[1];
                while (socket.Receive(buffer) != 0)
                {
                    Console.Write((char)buffer[0]);
                    Console.Out.Flush();
                }
                Console.WriteLine("Done reading stream");
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Connection Limit Hit: {ex.Message}");

            }
        }
    }
}
