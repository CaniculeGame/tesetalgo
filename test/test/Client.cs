using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace test
{
    class Client
    {

        private bool isStarted = false;
        private int tpsInterval = 33;//en ms
        private Thread executionClientThread = null;
        private Socket socketClient = null;
        private IPEndPoint ipEndpoint = null;

        public Client(IPEndPoint addrIp)
        {
            socketClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            ipEndpoint = addrIp;

        }

        private void TraitementClient()
        {
            int i = 0;
            Stopwatch watch = new Stopwatch();
            byte[] data = new byte[256];

            try
            {
                socketClient.Connect(ipEndpoint);
            }catch(Exception e)
            {
                Console.WriteLine("serveur invalide");
                return;
            }
            
            
            if (!socketClient.Connected)
            {
                IsStarted = false;
                Console.WriteLine("echec connection");
            }
            else
            {
                Console.WriteLine("connection etablie");
            }

            watch.Start();
            while (IsStarted)
            {
                if(watch.ElapsedMilliseconds > tpsInterval)
                {
                    if (socketClient.Connected)
                    {
                        //envoi de donnée
                        data = Encoding.UTF8.GetBytes("coucou!! iteration client numero = "+i);
                        i++;
                        socketClient.Send(data);


                        //reception de donnée
                        socketClient.Receive(data);
                        GlobalSingleton.Instance().LabelClient = Encoding.UTF8.GetString(data);

                        //restart compteur
                        watch.Restart();
                    }
                    else
                    {
                        IsStarted = false;
                    }
                }
                else
                {
                    Thread.Sleep(33);
                }

            }

            if (socketClient != null)
                socketClient.Close();

            Stop();
        }



        public void Stop()
        {
            if(isStarted)
            {
                isStarted = false;
            }
        }

        public void Start()
        {
            if (!isStarted)
            {
                isStarted = true;
                executionClientThread = new Thread(TraitementClient);
                executionClientThread.Start();
            }
        }


        public bool IsStarted { get { return isStarted; } set { isStarted = value; } }

    }
}
