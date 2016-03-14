using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace TreinoUDP
{
    class Program
    {
        static Thread threadListener,
            threadSender;
        static Socket socket;
        static bool aliveSender,
            aliveListener;

        static void Main(string[] args)
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            socket.ReceiveTimeout = 8000;
            socket.SendTimeout = 8000;

            Console.Write("1-Emissor\n2-Receptor\n\nEscolha: ");

            switch (int.Parse(Console.ReadLine()))
            {
                case 1:
                    threadSender = new Thread(SenderDefinition);
                    threadSender.Start();
                    aliveSender = true;
                    break;
                case 2:
                    threadListener = new Thread(ListenerDefinition);
                    threadListener.Start();
                    aliveListener = true;
                    break;
            }
        }

        static void ListenerDefinition()
        {
            IPEndPoint endpoint = new IPEndPoint(IPAddress.Any, 7654);
            byte[] buffer = new byte[1024];

            socket.Bind(endpoint);

            Console.WriteLine("\nConectado! Aguardando mensagem...");

            while (aliveListener)
            {
                try
                {
                    buffer = new byte[1024];
                    int qtRecebida = socket.Receive(buffer);

                    Console.WriteLine("Mensagem recebida: {0}", Encoding.UTF8.GetString(buffer, 0, qtRecebida));
                }
                catch (SocketException ex)
                {
                    if (ex.ErrorCode == 10060) { }
                    else throw ex;
                }
            }
        }

        static void SenderDefinition()
        {
            IPEndPoint endpoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 7654);
            byte[] buffer = new byte[1024];

            Console.WriteLine();

            while (aliveSender)
            {
                try
                {
                    Console.Write("Mensagem a enviar: ");
                    buffer = Encoding.UTF8.GetBytes(Console.ReadLine());

                    socket.SendTo(buffer, endpoint);
                }
                catch (SocketException ex)
                {
                    if (ex.ErrorCode == 10060) { }
                    else throw ex;
                }
            }
        }
    }
}