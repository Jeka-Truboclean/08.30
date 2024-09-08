using System.Net.Sockets;
using System.Net;
using System.Text;

namespace ClientTicTacToe
{
    internal class Program
    {
        static void Main(string[] args)
        {
            UdpClient udpClient = new UdpClient();
            udpClient.Connect("localhost", 5000);

            IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);

            Console.WriteLine("Connecting server...");

            while (true)
            {
                byte[] data = udpClient.Receive(ref remoteEP);
                string boardState = Encoding.UTF8.GetString(data);
                Console.WriteLine(boardState);

                if (boardState.Contains("won") || boardState.Contains("Tie"))
                {
                    break;
                }

                Console.Write("Enter the cell number for the move (1-9): ");
                string move = Console.ReadLine();
                data = Encoding.UTF8.GetBytes(move);
                udpClient.Send(data, data.Length);

                data = udpClient.Receive(ref remoteEP);
                boardState = Encoding.UTF8.GetString(data);
                Console.WriteLine(boardState);
            }

            udpClient.Close();
            Console.WriteLine("Lost connection with server.");
        }
    }
}
