using Plugin.Geolocator;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Xamarin.Forms.Maps;
using Xamarin.Forms;

namespace test
{
    class Client
    {

        private bool isStarted = false;
        private int tpsInterval = 33;//en ms
        private Thread executionClientReceptionThread = null;
        private Thread executionClientDemandeThread = null;
        private Socket socketClient = null;
        private IPEndPoint ipEndpoint = null;

        private readonly Mutex verrou = new Mutex();

        private Position oldPos = new Position();
        private Dictionary<int, Personnage> participant = null;
        private Personnage perso = null;

        public event EventHandler NeedRefreshMap;//creation d'un event
        public event EventHandler NeedToReCenterPos;

        public Client(IPEndPoint addrIp)
        {
            socketClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            ipEndpoint = addrIp;
        }


        private bool Connection()
        {

            try
            {
                if (socketClient == null)
                    socketClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                socketClient.Connect(ipEndpoint);
            }
            catch (Exception e)
            {
                Console.WriteLine("serveur invalide " + e);
                GlobalSingleton.Instance().ButtonClientLabel = "Echec Connexion";
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
                executionClientReceptionThread = null;
                executionClientDemandeThread = null;

                socketClient.Disconnect(false);
                socketClient.Dispose();
                socketClient.Close();
                socketClient = null;
            }
        }


        private void TraitementClientDemande()
        {
            var locator = CrossGeolocator.Current;
            locator.DesiredAccuracy = 50;
            var pos = locator.GetPositionAsync().Result;
            GlobalSingleton.Instance().MaPosition = new Position(pos.Latitude, pos.Longitude);
            MapInfoSingleton.Instance().SetPosition(GlobalSingleton.Instance().MaPosition);
            if(NeedToReCenterPos!=null)
                NeedToReCenterPos(this, new EventArgs());


            Stopwatch watch = new Stopwatch();
            
            watch.Start();
            while (IsStarted)
            {
                if(watch.ElapsedMilliseconds > tpsInterval)
                {
                    verrou.WaitOne();
                    try
                    {
                        if (socketClient.Connected)
                        {

                            //recuperation des données de localisation
                            locator = CrossGeolocator.Current;
                            locator.DesiredAccuracy = 50;
                            pos = locator.GetPositionAsync().Result;
                            GlobalSingleton.Instance().MaPosition = new Position(pos.Latitude, pos.Longitude);
                            MapInfoSingleton.Instance().SetPosition(GlobalSingleton.Instance().MaPosition);

                            TraitementDemande();
                            
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

#if DEBUG
            Console.WriteLine("fin thread client demade");
#endif
        }

        private void TraitementDemande()
        {
            byte[] data = new byte[32];

            if (perso == null)
                perso = GlobalSingleton.Instance().Perso;

            //si pas d'id on en demande un
            if (perso.ID == Personnage.INVALID_ID_VALUE)
            {
                //envoie demande d'id
                data = EncodeDecode.EncoderMessage(EncodeDecode.NomMessage.GET_ID, Personnage.INVALID_ID_VALUE, new Position(), new Color(), null);
                socketClient.Send(data);
                return;
            }

            //si position changer alors on informe le serveur
            if (!perso.Position.Equals(oldPos))
            {
                oldPos = perso.Position;
                data = EncodeDecode.EncoderMessage(EncodeDecode.NomMessage.INFO_POS, perso.ID, oldPos, perso.Couleur, perso.Pseudo);
                socketClient.Send(data);
                return;
            }

        }


      
        private void TraitementClientReception()
        {

            Stopwatch watch = new Stopwatch();
            Personnage newParticipant = new Personnage();
            byte[] data = new byte[32];

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
                            TraitementReceptionDonnees(ref data);
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

#if DEBUG
            Console.WriteLine("fin thread client reception");
#endif

        }


        private void TraitementReceptionDonnees(ref byte[] data)
        {
            if (data == null)
                return;

            EncodeDecode.NomMessage idmsg = EncodeDecode.NomMessage.INVALID_VALUE;
            int id = Personnage.INVALID_ID_VALUE;
            Position pos = new Position();
            Color couleur = new Color();
            string pseudo = Personnage.DEFAULT_PSEUDO_VALUE;
            byte[] dataReponse = new byte[32];
            participant = GlobalSingleton.Instance().Participants;

            EncodeDecode.DecoderMessage(data, ref idmsg, ref id, ref pos, ref couleur, ref pseudo);

            switch (idmsg)
            {
                case EncodeDecode.NomMessage.INFO_PERSO: //recuperation donnée perso
                    if(id != GlobalSingleton.Instance().Perso.ID)
                    {
                        //si id!= miens, alors on met a jour les info du participant
                        if (participant == null)
                            participant = GlobalSingleton.Instance().Participants;

                        if (participant.ContainsKey(id))
                        {
                            participant[id].Couleur = couleur;
                            participant[id].Pseudo = pseudo;
                        }
                        else
                        {
                            Personnage p = new Personnage();
                            p.ID = id;
                            p.Pseudo = pseudo;
                            p.Couleur = couleur;
                            participant.Add(id, p);
                        }
                    }
                    break;

                case EncodeDecode.NomMessage.INFO_POS: //recuperation donnée position d'un nouveau participant

                    if (id != GlobalSingleton.Instance().Perso.ID)
                    {
                        if (participant == null)
                            participant = GlobalSingleton.Instance().Participants;

                        if (participant.ContainsKey(id))
                        {
                            participant[id].Position = pos;
                        }
                        else
                        {
                            Personnage p = new Personnage();
                            p.ID = id;
                            p.Position = pos;
                            participant.Add(id, p);
                        }
                    }
                    break;

                case EncodeDecode.NomMessage.GET_ID: //demande d'id : on me demande mon id, je l'envoie
                    dataReponse = EncodeDecode.EncoderMessage(EncodeDecode.NomMessage.SEND_ID, GlobalSingleton.Instance().Perso.ID, pos, couleur, null);
                    socketClient.Send(dataReponse);
                    break;

                case EncodeDecode.NomMessage.SEND_ID: // reception d'id : je recoi mon id, je retourne toutes mes info
                    GlobalSingleton.Instance().Perso.ID = id;
                    pos = GlobalSingleton.Instance().Perso.Position;
                    couleur = GlobalSingleton.Instance().Perso.Couleur;
                    pseudo = GlobalSingleton.Instance().Perso.Pseudo;
                    dataReponse = EncodeDecode.EncoderMessage(EncodeDecode.NomMessage.SEND_ALL_INFO_PERSO, GlobalSingleton.Instance().Perso.ID, pos, couleur, pseudo);
                    socketClient.Send(dataReponse);
                    break;

                case EncodeDecode.NomMessage.GET_ALL_PARTICIPANT: //demande des autres perso dispo
                    //ne rien faire
                    break;

                case EncodeDecode.NomMessage.SEND_ALL_INFO_PERSO: //reception des donnees complete d'une personne
                    if (id != GlobalSingleton.Instance().Perso.ID)
                    {
                        //si id!= miens, alors on met a jour les info du participant
                        if (participant == null)
                            participant = GlobalSingleton.Instance().Participants;

                        if (participant.ContainsKey(id))
                        {
                            participant[id].Couleur = couleur;
                            participant[id].Pseudo = pseudo;
                            participant[id].Position = pos;
                        }
                        else
                        {
                            Personnage p = new Personnage();
                            p.ID = id;
                            p.Pseudo = pseudo;
                            p.Couleur = couleur;
                            p.Position = pos;
                            participant.Add(id, p);
                        }
                    }
                    break;

                default:
                    Console.WriteLine("ERREUR : client.cs : TraitementReceptionDonnees : message inconnue");
                    break;
            }
        }




        public void Stop()
        {
           IsStarted = false;
           GlobalSingleton.Instance().ButtonClientLabel = "Rejoindre";

            executionClientDemandeThread.Abort();
            executionClientReceptionThread.Abort();
            Deconnection();

        }

        public void Start()
        {
            if (!IsStarted)
            {
                if (Connection())
                {
                    IsStarted = true;

                    executionClientDemandeThread = new Thread(TraitementClientDemande);
                    executionClientDemandeThread.Start();

                    executionClientReceptionThread = new Thread(TraitementClientReception);
                    executionClientReceptionThread.Start();
                }
            }
        }


        public bool IsStarted { get { return isStarted; } set { isStarted = value; } }
        public bool IsConnected { get { if (socketClient != null) return socketClient.Connected; else return false; } }


    }
}
