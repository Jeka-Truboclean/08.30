using System.Net.Sockets;
using System.Net;
using System.Text;

namespace _08._30
{
    internal class Program
    {
        static char[] board = { '1', '2', '3', '4', '5', '6', '7', '8', '9' };
        static int currentPlayer = 1;
        static bool gameEnded = false;

        static void Main(string[] args)
        {
            UdpClient udpServer = new UdpClient(5000);
            IPEndPoint remoteEP = null;

            Console.WriteLine("Server is launched, waiting for players...");

            while (!gameEnded)
            {
                byte[] data = udpServer.Receive(ref remoteEP);
                string move = Encoding.UTF8.GetString(data);

                int playerMove;
                if (int.TryParse(move, out playerMove) && playerMove >= 1 && playerMove <= 9 && board[playerMove - 1] != 'X' && board[playerMove - 1] != 'O')
                {
                    board[playerMove - 1] = currentPlayer == 1 ? 'X' : 'O';
                    SendBoardToPlayers(udpServer, remoteEP);

                    if (CheckForWinner())
                    {
                        string winnerMessage = $"Player {currentPlayer} won!";
                        SendToAllPlayers(udpServer, winnerMessage, remoteEP);
                        gameEnded = true;
                    }
                    else if (CheckForDraw())
                    {
                        string drawMessage = "Tie!";
                        SendToAllPlayers(udpServer, drawMessage, remoteEP);
                        gameEnded = true;
                    }
                    else
                    {
                        currentPlayer = (currentPlayer == 1) ? 2 : 1;
                    }
                }
            }

            udpServer.Close();
            Console.WriteLine("Game over. Server shut down.");
        }

        static void SendBoardToPlayers(UdpClient udpServer, IPEndPoint remoteEP)
        {
            string boardState = GetBoard();
            SendToAllPlayers(udpServer, boardState, remoteEP);
        }

        static string GetBoard()
        {
            return $"\n {board[0]} | {board[1]} | {board[2]} \n---+---+---\n {board[3]} | {board[4]} | {board[5]} \n---+---+---\n {board[6]} | {board[7]} | {board[8]} \n";
        }

        static bool CheckForWinner()
        {
            int[,] winConditions = {
            {0, 1, 2}, {3, 4, 5}, {6, 7, 8}, // rows
            {0, 3, 6}, {1, 4, 7}, {2, 5, 8}, // columns
            {0, 4, 8}, {2, 4, 6}  // diagonals
        };

            for (int i = 0; i < winConditions.GetLength(0); i++)
            {
                if (board[winConditions[i, 0]] == board[winConditions[i, 1]] && board[winConditions[i, 1]] == board[winConditions[i, 2]])
                {
                    return true;
                }
            }

            return false;
        }

        static bool CheckForDraw()
        {
            foreach (char cell in board)
            {
                if (cell != 'X' && cell != 'O')
                {
                    return false;
                }
            }
            return true;
        }

        static void SendToAllPlayers(UdpClient udpServer, string message, IPEndPoint remoteEP)
        {
            byte[] data = Encoding.UTF8.GetBytes(message);
            udpServer.Send(data, data.Length, remoteEP);
        }
    }
}
