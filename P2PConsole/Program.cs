using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace P2PConsole
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var r = new Random();
            var port = r.Next(9000, 9999);
            Peer myPeer = new Peer(Guid.NewGuid().ToString(), new IPEndPoint(IPAddress.Parse("192.168.1.105"), port));

            Console.WriteLine($"My address:\nHANDSHAKE {myPeer.EndPoint.Address}:{port}");
            Console.WriteLine("Usage: send <file_path> <recipient_ip>:<recipient_port>" + "\n" +
            "Usage: request <file_name> <recipient_ip>:<recipient_port>" + "\n" +
            "Usage: search <file_name>" + "\n" +
            "Usage: addpeer <ip>:<port>" + "\n" +
            "Usage: handshake <ip>:<port>"
                );
            myPeer.ListenThread.Start();

            while (true)
            {
                Console.Write("> ");
                string input = Console.ReadLine();

                string[] command = input.Split(' ');
                if (command[0].ToLower() == "send")
                {
                    if (command.Length != 3)
                    {
                        Console.WriteLine("Usage: send <file_path> <recipient_ip>:<recipient_port>");
                        continue;
                    }

                    string[] recipient = command[2].Split(':');
                    myPeer.SendFile(command[1], new IPEndPoint(IPAddress.Parse(recipient[0]), int.Parse(recipient[1])));
                }
                else if (command[0].ToLower() == "request")
                {
                    if (command.Length != 3)
                    {
                        Console.WriteLine("Usage: request <file_name> <recipient_ip>:<recipient_port>");
                        continue;
                    }

                    string[] recipient = command[2].Split(':');
                    myPeer.RequestFile(
                        command[1], new IPEndPoint(IPAddress.Parse(recipient[0]), int.Parse(recipient[1])));
                }
                else if (command[0].ToLower() == "search")
                {
                    if (command.Length != 2)
                    {
                        Console.WriteLine("Usage: search <file_name>");
                        continue;
                    }

                    myPeer.SearchForFile(command[1]);
                }
                else if (command[0].ToLower() == "addpeer")
                {
                    if (command.Length != 2)
                    {
                        Console.WriteLine("Usage: addpeer <ip>:<port>");
                        continue;
                    }

                    string[] peer = command[1].Split(':');
                    myPeer.AddKnownPeer(new IPEndPoint(IPAddress.Parse(peer[0]), int.Parse(peer[1])));
                }
                else if (command[0].ToLower() == "removepeer")
                {
                    if (command.Length != 2)
                    {
                        Console.WriteLine("Usage: removepeer <ip>:<port>");
                        continue;
                    }

                    string[] peer = command[1].Split(':');
                    myPeer.RemoveKnownPeer(new IPEndPoint(IPAddress.Parse(peer[0]), int.Parse(peer[1])));
                }
                else if (command[0].ToLower() == "handshake")
                {
                    if (command.Length != 2)
                    {
                        Console.WriteLine("Usage: handshake <ip>:<port>");
                        continue;
                    }

                    string[] peer = command[1].Split(':');
                    myPeer.InitiateHandshake(new IPEndPoint(IPAddress.Parse(peer[0]), int.Parse(peer[1])));
                }
                else { Console.WriteLine("Unknown command. Type 'help' for help."); }
            }
        }
    }
}
