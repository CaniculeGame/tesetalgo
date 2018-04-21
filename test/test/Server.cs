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
    internal class Server
    {
        internal class Param
        {
            private Socket socket_ = null;
            private int id_ = -1;

            public Socket socket { get { return socket; } set { socket = value; } }
            public int id { get { return id; } set { if (value < 0) id = -1; else id = value; } }
        }


        private Socket serverSocket = null;
        private IPEndPoint ipAdressServer = null;
        private bool isStarted = false;
        private int connectionMaxSimultane = 10;
        private Thread traitementConnectionThread = null;
        private ArrayList acceptList = null;
        private Dictionary<int,Personnage> clientDonnees = null;
        private static readonly Object verrou = new Object();

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

                lock (verrou)
                {
                    Console.WriteLine("Connection entrante");
                    //on a un client; on créé un thread de communication
                    Thread newThread = new Thread(TraitementServiceServer);
                    Param prm = new Param();
                    prm.id = GenererID();
                    prm.socket = newClient;
                    newThread.Start(acceptList[acceptList.Count - 1]);
                    acceptList.Add(newThread);
                }
            }

            isStarted = false;
            serverSocket.Shutdown(SocketShutdown.Both);
            serverSocket.Close();
        }




        public Server(IPEndPoint ip, int connectionMaxSimultane)
        {
            ipAdressServer = ip;
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            clientDonnees = new Dictionary<int, Personnage>();

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

        private void TraitementServiceServer(Object param)
        {
            Param parametres = (Param)param;
            Socket socket = parametres.socket;
            int id = parametres.id;
            int idmsg = 0;
            Stopwatch watch = new Stopwatch();
            Position position = new Position();
            Color couleur = new Color();
            string pseudo = "";
            int tpsInterval = 2000; //ms mise a jour des données
            Boolean estEnMarche = true;
            byte[] message = new byte[32];


            watch.Start();
            while (estEnMarche)
            {
                if (watch.ElapsedMilliseconds > tpsInterval)
                {
                    //verrification que le client est connecté
                    if (socket != null && socket.Connected)
                    {
                        //traitement reception : on recoi des demande client et on traite
                        socket.Receive(message);
                        DecoderMessage(message, ref idmsg, ref id, ref position, ref couleur, ref pseudo);
                        Repondre(idmsg, id, position, couleur, pseudo);
                    }
                    else
                    {
                        estEnMarche = false;
                    }

                    watch.Restart();
                }
                else
                {
                    Thread.Sleep(1000);
                }

            }

        }

        private void Repondre(int idmsg, int id, Position position, Color couleur, string pseudo)
        {

            switch (idmsg)
            {
                case 0:
                    DemandeDeMajDuPseudoClient(id, couleur,pseudo);
                    break;

                case 1:
                    DemandeMajPositionClient(id, position);
                    break;

                case 2:
                   DemandeIdClient();
                    break;

                case 3:
                    // n'est pas recu
                    break;

                case 4:
                    DemandeMajDonneesParticipants(id);
                    break;

                case 5:
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
                    byte[] message = EncoderMessage(5, p.Value.ID, p.Value.Position, p.Value.Couleur, p.Value.Pseudo);
                    serverSocket.Send(message);
                }
            }
        }

        private void DemandeIdClient()
        {
            Personnage p = new Personnage();
            int ident = GenererID();
            p.ID = ident;
            byte[] message = EncoderMessage(3, ident, new Position(), new Color(), "");
            serverSocket.Send(message);
            clientDonnees.Add(ident, p);
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
                        byte[] message = EncoderMessage(3, ident, new Position(), new Color(), "");
                        serverSocket.Send(message);
                    }
                }
            }
        }

        public static string GetHexString(Color color)
        {
            var red = (int)(color.R * 255);
            var green = (int)(color.G * 255);
            var blue = (int)(color.B * 255);
            var alpha = (int)(color.A * 255);
            var hex = $"#{alpha:X2}{red:X2}{green:X2}{blue:X2}";

            return hex;
        }

        private byte[] EncodeColor(Color couleur)
        {
            if (couleur != null)
                return Encoding.UTF8.GetBytes(GetHexString(couleur));
            else
                return Encoding.UTF8.GetBytes(GetHexString(Color.Gray));
        }


        //idMessage 0(int) ; id(int) ; couleur Xamarin.Form.Color, int sizepseudoo ; string[] pseudo : info perso client : message serveur et client encode/decode
        //idMessage 1(int) ; id(int); lat(double) long(double)  ; info position client : message server et client encode/decode
        //idMessage 2(int) ; demande d'id par le client : message client decode
        //idmessage 3(int) ; id(int) //envoi de l'id au client demandeur
        //idmessage 4(int) ; id(int) //demande id ts client
        //idmessage 5(int) ; envoie toutes les données

        //demande client maj position ts clients
        //demande : idmessage 4
        //reponse : idmessage 5

        //demande client maj sa position
        //demande message 1

        //demande client maj pseudo
        //demande message 0

        //demande client demande son id
        //demande message 2
        //reponse message 3


        //idmessage = 0 : maj Perso
        //idmessage = 1 : maj position
        private byte[] EncoderMessage(int idMsg, int id, Position position, Color couleur, String pseudo)
        {
            int index = 0;
            byte[] idMsgByte     = BitConverter.GetBytes(idMsg); 
            byte[] idByte        = BitConverter.GetBytes(id);
            byte[] latitudeByte  = BitConverter.GetBytes(position.Latitude);
            byte[] longitudeByte = BitConverter.GetBytes(position.Longitude);
            byte[] colorByte     = EncodeColor(couleur);
            
            

            byte[] data = new byte[32];
            idMsgByte.CopyTo(data, index);
            index += idMsgByte.Length;
            idByte.CopyTo(data, index);
            index += idByte.Length;
            switch (idMsg)
            {
                case 0:
                    colorByte.CopyTo(data, index);
                    index += colorByte.Length;
                    byte[] sizepseudo = BitConverter.GetBytes(pseudo.Length);
                    sizepseudo.CopyTo(data, index);
                    index += sizepseudo.Length;
                    byte[] pseudoByte = Encoding.UTF8.GetBytes(pseudo);
                    pseudoByte.CopyTo(data, index);
                    break;

                case 1:
                    latitudeByte.CopyTo(data, index);
                    index += latitudeByte.Length;
                    longitudeByte.CopyTo(data, index);
                    break;

                case 2:
                    //rien de plus
                    break;

                case 3:
                    //rien de plus
                    break;

                case 4:
                    //rien de plus
                    break;


                case 5:
                    latitudeByte.CopyTo(data, index);
                    index += latitudeByte.Length;
                    longitudeByte.CopyTo(data, index);
                    colorByte.CopyTo(data, index);
                    index += colorByte.Length;
                    byte[] sizepseudo2 = BitConverter.GetBytes(pseudo.Length);
                    sizepseudo2.CopyTo(data, index);
                    index += sizepseudo2.Length;
                    byte[] pseudoByte2 = Encoding.UTF8.GetBytes(pseudo);
                    pseudoByte2.CopyTo(data, index);
                    break;


                default:
                    break;
            }

            return data;
        }


        private void DecoderMessage(byte[] message, ref int idmsg , ref int id, ref Position position, ref Color couleur, ref String pseudo)
        {
            int index = 0;
            int tailleCouleur = GetHexString(Color.Red).Length;
            int idMsgType = -1;
            int idType = -1;
            double latitudeType = -1;
            double longitudeType = -1;
            string colorType = null;
            string pseudotype = null;


            Console.WriteLine(" TAILE MESSAGE RECU = " + message.Length);
            idMsgType = BitConverter.ToInt32(message, index);
            index += sizeof(int);
            idType = BitConverter.ToInt32(message, index);
            index += sizeof(int);

            switch (idMsgType)
            {
                case 0:
                    colorType = Encoding.UTF8.GetString(message,index, tailleCouleur);
                    index += tailleCouleur;
                    int sizePseudo = BitConverter.ToInt32(message, index);
                    index += sizeof(int);
                    pseudotype = Encoding.UTF8.GetString(message, index, sizePseudo);
                    break;

                case 1:
                    latitudeType = BitConverter.ToDouble(message, index);
                    index += sizeof(double);
                    longitudeType = BitConverter.ToDouble(message, index);
                    break;

                case 2:
                    //rien de plus
                    break;

                case 3:
                    //rien de plus
                    break;

                case 4:
                    //rien de plus
                    break;

                case 5:
                    latitudeType = BitConverter.ToDouble(message, index);
                    index += sizeof(double);
                    longitudeType = BitConverter.ToDouble(message, index);
                    colorType = Encoding.UTF8.GetString(message, index, tailleCouleur);
                    index += tailleCouleur;
                    int sizePseudo2 = BitConverter.ToInt32(message, index);
                    index += sizeof(int);
                    pseudotype = Encoding.UTF8.GetString(message, index, sizePseudo2);
                    break;

                default:
                    break;
            }

            id = idType;
            position = new Position(latitudeType, longitudeType);
            if(colorType != null)
                couleur = Color.FromHex(colorType);
            pseudo = pseudotype;

            Console.WriteLine("Decoder MESSAGE");
            Console.WriteLine("Decoder id : " + id);
            Console.WriteLine("Decoder pos : " + position.Latitude+"  "+position.Longitude);
            if (colorType != null)
                Console.WriteLine("Decoder couleur : " + couleur);
            if (pseudo != null)
                Console.WriteLine("Decoder pseudo : " + pseudo);
        }



        private int GenererID()
        {
            throw new NotImplementedException();
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

                }
            }
        }



        public bool IsStarted(){  return isStarted; }
        public int NbConnectionMaxSimulatanne() { return connectionMaxSimultane; }



#if DEBUG
        private void testEncodeDecode()
        {
            int id = -1;
            int idmsg = -1;
            string pseudo ="";
            Color couleur = new Color();
            Position pos = new Position();

            byte[] tabByteMsg0 = EncoderMessage(0,35,new Position(),Color.Orange,"abcdefghij");
            byte[] tabByteMsg1 = EncoderMessage(1, 38, new Position(43,5.3), new Color(), null);
            byte[] tabByteMsg2 = EncoderMessage(2, 45, new Position(), new Color(), null);

            Console.WriteLine(" -----------------------------------");
            DecoderMessage(tabByteMsg0, ref idmsg, ref id, ref pos, ref couleur, ref pseudo);
            Console.WriteLine(" -----------------------------------");
            DecoderMessage(tabByteMsg1, ref idmsg, ref id, ref pos, ref couleur, ref  pseudo);
            Console.WriteLine(" -----------------------------------");
            DecoderMessage(tabByteMsg2, ref idmsg, ref id, ref pos, ref couleur, ref pseudo);
            Console.WriteLine(" -----------------------------------");
            Console.WriteLine(" "+ idmsg+" "+id+" "+pos.Latitude+" "+pos.Longitude+" "+couleur.ToString()+" "+pseudo);
            Console.WriteLine(" -----------------------------------");
        }
#endif

    }
}
