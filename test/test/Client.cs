using Plugin.Geolocator;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Xamarin.Forms.Maps;

namespace test
{
    class Client
    {

        private bool isStarted = false;
        private int tpsInterval = 33;//en ms
        private Thread executionClientThread = null;
        private Socket socketClient = null;
        private IPEndPoint ipEndpoint = null;

        public event EventHandler NeedRefreshMap;//creation d'un event
        public event EventHandler NeedToReCenterPos;

        public Client(IPEndPoint addrIp)
        {
            socketClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            ipEndpoint = addrIp;

        }

        private void TraitementClient()
        {
            var locator = CrossGeolocator.Current;
            locator.DesiredAccuracy = 50;
            var pos = locator.GetPositionAsync().Result;
            GlobalSingleton.Instance().MaPosition = new Position(pos.Latitude, pos.Longitude);
            MapInfoSingleton.Instance().SetPosition(GlobalSingleton.Instance().MaPosition);
            if(NeedToReCenterPos!=null)
                NeedToReCenterPos(this, new EventArgs());


            int i = 0;
            Stopwatch watch = new Stopwatch();
            byte[] data = new byte[256];

            try
            {
                socketClient.Connect(ipEndpoint);
            }catch(Exception e)
            {
                Console.WriteLine("serveur invalide " + e);
                return;
            }
            
            
            if (!socketClient.Connected)
            {
                IsStarted = false;
                Console.WriteLine("echec connection ");
                GlobalSingleton.Instance().ButtonClientLabel = "Echec Connexion";
            }
            else
            {
                Console.WriteLine("connection etablie");
                GlobalSingleton.Instance().ButtonClientLabel = "Deconnexion";
                
            }

            watch.Start();
            while (IsStarted)
            {
                if(watch.ElapsedMilliseconds > tpsInterval)
                {
                    if (socketClient.Connected)
                    {

                        //recuperation des donnée
                        locator = CrossGeolocator.Current;
                        locator.DesiredAccuracy = 50;
                        pos = locator.GetPositionAsync().Result;
                        GlobalSingleton.Instance().MaPosition  = new Position(pos.Latitude,pos.Longitude);
                        MapInfoSingleton.Instance().SetPosition(GlobalSingleton.Instance().MaPosition);

                        //envoi de ma position msg id = 0 : id:couleur:pseudo:lat:long 
                        data = Encoding.UTF8.GetBytes(GlobalSingleton.Instance().MaPosition.Latitude+":"+GlobalSingleton.Instance().MaPosition.Longitude);
                        i++;
                        socketClient.Send(data);


                        //reception de donnée
                        socketClient.Receive(data);
                        GlobalSingleton.Instance().LabelClient = Encoding.UTF8.GetString(data);

                        //restart compteur
                        watch.Restart();

                        //lance event pour rechargement position carte
                        if (NeedRefreshMap != null)
                            NeedRefreshMap(this, new EventArgs());
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

           socketClient.Disconnect(false);
            socketClient.Dispose();
           socketClient.Close();
           Stop();
        }



        public void Stop()
        {
            if(isStarted)
            {
                isStarted = false;
                GlobalSingleton.Instance().ButtonClientLabel = "Rejoindre";
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
        public bool IsConnected { get { return socketClient.Connected; } }

    }
}
