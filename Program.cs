



using System;
using System.Text;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;
using System.Threading.Tasks;
using BookClassLibrary;
using Newtonsoft.Json;

namespace MandatoryAssignmentTCPServer
{
    public class Program
    {
        public static List<Book> BookList = new List<Book>()
        {

            new Book("New Moon", "Robert Lynistar", 400, "12346geh63dg1"),
            new Book("Harry Potter", "J:K Rolling", 800, "162gSuw72gwV3"),
            new Book("You Can Win", "Shiva Khera", 300, "7493GS832YHc2"),
            new Book("Capital", "Karl Marx", 250, "2648gd837GBS4"),
            new Book("Muna Madan", "Laxmi Prashad Devkota", 900, "LA12xm19I45po")


        };
        static void Main(string[] args)
        {
            try
            {
                // set the TcpListener on port 4646
                int port = 4646;
                TcpListener server = new TcpListener(IPAddress.Any, port);
                TcpClient client;
                // Start listening for client requests
                server.Start();

                int clientNumber = 0;

                //Enter the listening loop
                while (true)
                {
                    Console.Write("Waiting for a connection... ");
                    client = server.AcceptTcpClient();
                    ThreadProc(client, ref clientNumber);
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: {0}", e);
            }

            Console.Read();
        }
        static void ThreadProc(object obj, ref int clientNumber)
        {
            byte[] bytes = new byte[1024];
            string data;
            clientNumber = 1;
            var client = (TcpClient)obj;
            Console.WriteLine("Connected!");

            // Get a stream object for reading and writing
            NetworkStream stream = client.GetStream();

            int i;

            // Loop to receive all the data sent by the client.
            i = stream.Read(bytes, 0, bytes.Length);
            string message = "";
            while (i != 0)
            {
                // Translate data bytes to a ASCII string.
                data = Encoding.ASCII.GetString(bytes, 0, i);
                Console.WriteLine(string.Format("Received: {0}", data));
                string mess = "not valid command";
                string[] words = data.ToLower().Split(' ');
                if (words[0].Trim() == "GetAll")
                {
                    mess = JsonConvert.SerializeObject(BookList);
                }
                if (words[0].Trim() == "get")
                {
                    mess = JsonConvert.SerializeObject(BookList.Find(e => e.ISBN == words[1]));
                }
                if (words[0].Trim() == "save")
                {
                    BookList.Add(JsonConvert.DeserializeObject<Book>(words[1]));
                    mess = "";
                }

                client.Close();
                Console.WriteLine("Client Disconnected");

                byte[] msg = Encoding.ASCII.GetBytes(message);

                stream.Write(msg, 0, msg.Length);
                Console.WriteLine(string.Format("Sent: {0}", message));

                Thread.Sleep(1000);

                Console.WriteLine("Sent: {0}", mess);
            }
            client.Close();
            clientNumber--;


        }

    }
}
