using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Xamarin.Forms.Maps;
using Xamarin.Forms;
using System.Text;

namespace test
{
    class Server
    {

        private Socket serverSocket = null;
        private IPEndPoint ipAdressServer = null;
        private bool isStarted = false;
        private int connectionMaxSimultane = 10;
        private Thread traitementConnectionThread = null;
        private List<Thread> acceptList = null;
        private Dictionary<int,Personnage> clientDonnees = null;
        private static readonly Object verrou = new Object();

        public event EventHandler<ThreadEventEnd> DeconnectionClient;//creation d'un event

        private void TraitementConnectionServer()
        {
            if (serverSocket == null || ipAdressServer == null)
                return;

            acceptList = new List<Thread>();
            isStarted = true;
            while (isStarted)
            {
                serverSocket.Listen(connectionMaxSimultane);
                Socket newClient = serverSocket.Accept();

                lock (verrou)
                {
                    Console.WriteLine("Connection entrante");
                    //on a un client; on créé un thread de communication
                    ParamThreadConnection prm = new ParamThreadConnection();
                    prm.Id = GenererID(); // on genere l'ident a la connection du client et il gardera le meme
                    prm.SocketClient = newClient;
                    Thread newThread = new Thread(() => TraitementServiceServer(prm));
                    acceptList.Add(newThread);
                    newThread.Start();
                }
            }

            isStarted = false;
            serverSocket.Shutdown(SocketShutdown.Both);
            serverSocket.Close();
        }


        private bool CheckIfAlwayConnectedClient(Socket sock)
        {

            if (sock != null)
            {
                if (!sock.Poll(10, SelectMode.SelectRead))
                    return false;
                else
                    return true;
            }

            return false;
        }

        public Server(IPEndPoint ip, int connectionMaxSimultane)
        {
            ipAdressServer = ip;
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            clientDonnees = new Dictionary<int, Personnage>();

            DeconnectionClient += new EventHandler<ThreadEventEnd>(EndConnection);
#if DEBUG
            testEncodeDecode();
#endif

        }



        public void Start()
        {
            if (serverSocket == null || ipAdressServer == null)
                return;

            serverSocket.Bind(ipAdressServer);

            traitementConnectionThread = new Thread(TraitementConnectionServer);
            traitementConnectionThread.Start();

        }

        private void TraitementServiceServer(ParamThreadConnection param)
        {
            ParamThreadConnection parametres = param;
            Socket socket = parametres.SocketClient;
            int idClient = parametres.Id;
            int idreception = Personnage.INVALID_ID_VALUE;
            EncodeDecode.NomMessage idmsg = EncodeDecode.NomMessage.INVALID_VALUE;
            Stopwatch watch = new Stopwatch();
            Position position = new Position();
            Color couleur = new Color();
            string pseudo = Personnage.DEFAULT_PSEUDO_VALUE;
            int tpsInterval = 2000; //ms mise a jour des données
            Boolean estEnMarche = true;
            byte[] message = new byte[32];


            watch.Start();
            while (estEnMarche)
            {
                if (watch.ElapsedMilliseconds > tpsInterval)
                {
                    //verrification que le client est connecté
                    if (socket != null && socket.Connected && CheckIfAlwayConnectedClient(socket))
                    {
                        //traitement reception : on recois des demandes client et on traite
                        socket.Receive(message);
                        EncodeDecode.DecoderMessage(message, ref idmsg, ref idreception, ref position, ref couleur, ref pseudo);
                        Repondre(idmsg, idClient, position, couleur, pseudo);
                    }
                    else
                    {
                        estEnMarche = false;
                    }

                    watch.Restart();
                }
                else
                {
                    Thread.Sleep(33);
                }
            }

            if (DeconnectionClient != null)
                DeconnectionClient(this, new ThreadEventEnd(Thread.CurrentThread));

#if DEBUG
            Console.WriteLine("Fin  connection client thread");
#endif
        }

        private void EndConnection(object sender, ThreadEventEnd e)
        {
            if(acceptList != null)
            {
                if (acceptList.Contains(e.Data))
                {
                    e.Data.Abort();
                    acceptList.Remove(e.Data);
#if DEBUG
                    Console.WriteLine("end deconection");
#endif
                }
            }
        }

        private void Repondre(EncodeDecode.NomMessage idmsg, int id, Position position, Color couleur, string pseudo)
        {

            switch (idmsg)
            {
                case EncodeDecode.NomMessage.INFO_PERSO:
                    DemandeDeMajDuPseudoClient(id, couleur,pseudo);
                    break;

                case EncodeDecode.NomMessage.INFO_POS:
                    DemandeMajPositionClient(id, position);
                    break;

                case EncodeDecode.NomMessage.GET_ID:
                   DemandeIdClient(id);
                    break;

                case EncodeDecode.NomMessage.SEND_ID:
                    // n'est pas recu
                    break;

                case EncodeDecode.NomMessage.GET_ALL_PARTICIPANT:
                    DemandeMajDonneesParticipants(id);
                    break;

                case EncodeDecode.NomMessage.SEND_ALL_INFO_PERSO:
                    // n'est pas recu
                    break;

                default:
                    break;
            }

        }

        private void DemandeMajDonneesParticipants(int id)
        {
            foreach (KeyValuePair<int,Personnage> p in clientDonnees)
            {
                if (p.Key != id)
                {
                    byte[] message = EncodeDecode.EncoderMessage(EncodeDecode.NomMessage.SEND_ALL_INFO_PERSO, p.Value.ID, p.Value.Position, p.Value.Couleur, p.Value.Pseudo);
                    serverSocket.Send(message);
                }
            }
        }

        private void DemandeIdClient(int id)
        {
            if (clientDonnees == null)
                clientDonnees = new Dictionary<int, Personnage>();

            if (!clientDonnees.ContainsKey(id))
            {
                Personnage p = new Personnage();
                p.ID = id;
                clientDonnees.Add(id, p);
            }

            byte[] message = EncodeDecode.EncoderMessage(EncodeDecode.NomMessage.SEND_ID, id, new Position(), new Color(), "");
            serverSocket.Send(message);
        }

        private void DemandeMajPositionClient(int id, Position position)
        {
            Personnage p = new Personnage();

            if (clientDonnees != null)
            {
                if (clientDonnees.ContainsKey(id))
                {
                    if (clientDonnees.TryGetValue(id, out p))
                    {
                        if (position != null)
                            p.Position = position;
                        clientDonnees[id] = p;
                    }
                }
            }
        }

        private void DemandeDeMajDuPseudoClient(int id, Color couleur, string pseudo)
        {
            Personnage p = new Personnage();
            if (pseudo != null)
                p.Pseudo = pseudo;
            if (couleur != null)
                p.Couleur = couleur;

            if (clientDonnees != null)
            {
                if (clientDonnees.ContainsKey(id))
                {
                    if (clientDonnees.TryGetValue(id, out p))
                    {
                        if (pseudo != null)
                            p.Pseudo = pseudo;
                        if (couleur != null)
                            p.Couleur = couleur;
                        clientDonnees[id] = p;
                    }
                }
                else
                {
                    if (pseudo != null)
                        p.Pseudo = pseudo;
                    if (couleur != null)

                        p.Couleur = couleur;
                    if (id != -1)
                        clientDonnees.Add(id, p);
                    else
                    {
                        int ident = GenererID();
                        clientDonnees.Add(ident, p);
                        byte[] message = EncodeDecode.EncoderMessage(EncodeDecode.NomMessage.SEND_ID, ident, new Position(), new Color(), "");
                        serverSocket.Send(message);
                    }
                }
            }
        }


        private int GenererID()
        {
            return 1;
        }



        public void Stop()
        {
            if (serverSocket == null || ipAdressServer == null)
                return;

            lock (verrou)
            {
                if (traitementConnectionThread != null && traitementConnectionThread.IsAlive)
                {
                    isStarted = false;
                    foreach (Thread thread in acceptList)
                    {
                        thread.Abort();
                    }

                    acceptList.Clear();
                    traitementConnectionThread.Abort();
                    traitementConnectionThread = null;

                    serverSocket.Disconnect(false);
                    serverSocket.Close();
                    serverSocket = null;
                }
            }
        }



        public bool IsStarted(){  return isStarted; }
        public int NbConnectionMaxSimulatanne() { return connectionMaxSimultane; }



#if DEBUG
        private void testEncodeDecode()
        {
            int id = -1;
            EncodeDecode.NomMessage idmsg = EncodeDecode.NomMessage.INVALID_VALUE;
            string pseudo ="";
            Color couleur = new Color();
            Position pos = new Position();

            byte[] tabByteMsg0 = EncodeDecode.EncoderMessage(EncodeDecode.NomMessage.INFO_PERSO, 35,new Position(),Color.Orange,"abcdefghij");
            byte[] tabByteMsg1 = EncodeDecode.EncoderMessage(EncodeDecode.NomMessage.INFO_POS, 38, new Position(43,5.3), new Color(), null);
            byte[] tabByteMsg2 = EncodeDecode.EncoderMessage(EncodeDecode.NomMessage.GET_ID, 45, new Position(), new Color(), null);

            Console.WriteLine(" -----------------------------------");
            EncodeDecode.DecoderMessage(tabByteMsg0, ref idmsg, ref id, ref pos, ref couleur, ref pseudo);
            Console.WriteLine(" -----------------------------------");
            EncodeDecode.DecoderMessage(tabByteMsg1, ref idmsg, ref id, ref pos, ref couleur, ref  pseudo);
            Console.WriteLine(" -----------------------------------");
            EncodeDecode.DecoderMessage(tabByteMsg2, ref idmsg, ref id, ref pos, ref couleur, ref pseudo);
            Console.WriteLine(" -----------------------------------");
            Console.WriteLine(" "+ idmsg+" "+id+" "+pos.Latitude+" "+pos.Longitude+" "+couleur.ToString()+" "+pseudo);
            Console.WriteLine(" -----------------------------------");
        }
#endif

    }

    public class ThreadEventEnd : EventArgs
    {
        public Thread Data { get; set; }
        public ThreadEventEnd(Thread data)
        {
            Data = data;
        }
    }
}
