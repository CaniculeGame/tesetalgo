using System;
using System.Collections;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace test
{
    class Server
    {

        private Socket serverSocket = null;
        private IPEndPoint ipAdressServer = null;
        private bool isStarted = false;
        private int connectionMaxSimultane = 10;
        Thread traitementConnectionThread = null;
        Thread traitementServiceThread = null;
        private ArrayList acceptList = null;



        private void TraitementConnectionServer()
        {
            if (serverSocket == null || ipAdressServer == null)
                return;

            acceptList = new ArrayList();
            isStarted = true;
            while (isStarted)
            {
                serverSocket.Listen(connectionMaxSimultane);
                Socket newClient = serverSocket.Accept();
                Console.WriteLine("Connection entrante");
                //on a un client; on créé un thread de communication
                acceptList.Add(newClient);
            }

            isStarted = false;
            serverSocket.Shutdown(SocketShutdown.Both);
            serverSocket.Close();
        }




        public Server(IPEndPoint ip, int connectionMaxSimultane)
        {
            ipAdressServer = ip;
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        }



        public void Start()
        {
            if (serverSocket == null || ipAdressServer == null)
                return;

            serverSocket.Bind(ipAdressServer);

            traitementConnectionThread = new Thread(TraitementConnectionServer);
            traitementConnectionThread.Start();

            traitementServiceThread = new Thread(TraitementServiceServer);
            traitementServiceThread.Start();
        }

        private void TraitementServiceServer()
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            int tpsInterval = 60000; //ms
            while (isStarted)
            {
                if (watch.ElapsedMilliseconds > tpsInterval)
                {
                    //verrification que le client est connecté
                    ArrayList newListSocket = new ArrayList();
                    foreach (Socket elem in acceptList)
                    {
                        if (elem.Poll(10, SelectMode.SelectRead) && elem.Available != 0)
                        {
                            newListSocket.Add(elem);
                        }

                    }
                    acceptList = newListSocket;

                    //traitement reception
                    byte[] bytes = new byte[256];
                    foreach (Socket elem in acceptList)
                    {
                        elem.Receive(bytes);
                        GlobalSingleton.Instance().LabelServer = "Server a recu = " + Encoding.UTF8.GetString(bytes);
                    }
                    //traitement emission
                    foreach (Socket elem in acceptList)
                    {
                        bytes = Encoding.UTF8.GetBytes("J'ai bien recu à : "+ DateTime.Now.ToShortTimeString());
                        elem.Send(bytes);

                    }

                    watch.Restart();
                }
                else
                {
                    Thread.Sleep(1000);
                }

            }

        }




        public void Stop()
        {
            if (serverSocket == null || ipAdressServer == null)
                return;

            if (traitementConnectionThread != null && traitementConnectionThread.IsAlive)
            {
               isStarted = false;
            }
        }



        public bool IsStarted(){  return isStarted; }
        public int NbConnectionMaxSimulatanne() { return connectionMaxSimultane; }

    }
}
