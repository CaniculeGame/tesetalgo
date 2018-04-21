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

        private readonly Mutex verrou = new Mutex();

        public event EventHandler NeedRefreshMap;//creation d'un event
        public event EventHandler NeedToReCenterPos;

        public Client(IPEndPoint addrIp)
        {
            socketClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            ipEndpoint = addrIp;

#if DEBUG
            testEncodeDecode();
#endif
        }


        private bool Connection()
        {


            try
            {
                socketClient.Connect(ipEndpoint);
            }
            catch (Exception e)
            {
                Console.WriteLine("serveur invalide " + e);
                return false;
            }

            if (!socketClient.Connected)
            {
                IsStarted = false;
                Console.WriteLine("echec connection ");
                GlobalSingleton.Instance().ButtonClientLabel = "Echec Connexion";
                return false;
            }
            else
            {
                Console.WriteLine("connection etablie");
                GlobalSingleton.Instance().ButtonClientLabel = "Deconnexion";
                return true;
            }
        }

        private void Deconnection()
        {
            if (socketClient != null)
            {
                socketClient.Disconnect(false);
                socketClient.Dispose();
                socketClient.Close();
                socketClient = null;
            }
        }


        private void TraitementClientEnvoi()
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
            byte[] data = new byte[64];


            watch.Start();

            //premier envoi
            // idmsg id color pseudo => idmsgidcolorpseudo
            data = Encoding.UTF8.GetBytes("0"+ GlobalSingleton.Instance().Perso.ID+GlobalSingleton.Instance().Perso.Couleur.ToString()+ GlobalSingleton.Instance().Perso.Pseudo);
            socketClient.Send(data);
            while (IsStarted)
            {
                if(watch.ElapsedMilliseconds > tpsInterval)
                {
                    verrou.WaitOne();
                    try
                    {
                        if (socketClient.Connected)
                        {
                            if (GlobalSingleton.Instance().Perso.ID != Personnage.INVALID_ID_VALUE)
                            {
                                //recuperation des données de localisation
                                locator = CrossGeolocator.Current;
                                locator.DesiredAccuracy = 50;
                                pos = locator.GetPositionAsync().Result;
                                GlobalSingleton.Instance().MaPosition = new Position(pos.Latitude, pos.Longitude);
                                MapInfoSingleton.Instance().SetPosition(GlobalSingleton.Instance().MaPosition);

                                //envoi de ma position msg id = 1 : idmsg:id:lat:long 
                                data = Encoding.UTF8.GetBytes("1" + GlobalSingleton.Instance().Perso.ID + GlobalSingleton.Instance().MaPosition.Latitude + GlobalSingleton.Instance().MaPosition.Longitude);
                                i++;
                                socketClient.Send(data);
                            }
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
                    finally
                    {
                        verrou.ReleaseMutex();
                    }
                }
                else
                {
                    Thread.Sleep(33);
                }
            }
        }


        private void TraitementClientReception()
        {

            Stopwatch watch = new Stopwatch();
            Personnage newParticipant = new Personnage();
            byte[] data = new byte[64];

            watch.Start();

            while (IsStarted)
            {
                if (watch.ElapsedMilliseconds > tpsInterval)
                {
                    verrou.WaitOne();
                    try
                    {
                        if (socketClient.Connected)
                        {
                            //reception des données
                            socketClient.Receive(data);
                            int id = (int)data[0];
                            //decodage en focntion du message
                            if (id == 0) // pseudo des autres 
                            {
                                newParticipant.ID = (int)data[1];
                                if (GlobalSingleton.Instance().Participants != null && !GlobalSingleton.Instance().Participants.Contains(newParticipant))
                                    GlobalSingleton.Instance().Participants.Add(newParticipant);
                            }
                            else if (id == 1) // position  des autres
                            {

                            }
                            else if (id == 2 ) // notre id aupres du serveur
                            {

                            }
                            // GlobalSingleton.Instance().LabelClient = Encoding.UTF8.GetString(data);
                        }
                        else
                        {
                            IsStarted = false;
                        }
                    }
                    finally
                    {
                        verrou.ReleaseMutex();
                    }
                }
                else
                {
                    Thread.Sleep(33);
                }

            }

        }


        public void Stop()
        {
           isStarted = false;
           GlobalSingleton.Instance().ButtonClientLabel = "Rejoindre";
        }

        public void Start()
        {
            if (!isStarted)
            {
                if (Connection())
                {
                    isStarted = true;

                    executionClientThread = new Thread(TraitementClientEnvoi);
                    executionClientThread.Start();

                    executionClientThread = new Thread(TraitementClientReception);
                    executionClientThread.Start();
                }
            }
        }


        public bool IsStarted { get { return isStarted; } set { isStarted = value; } }
        public bool IsConnected { get { if (socketClient != null) return socketClient.Connected; else return false; } }



#if DEBUG
        private void testEncodeDecode()
        {
           

        }
#endif

    }
}
