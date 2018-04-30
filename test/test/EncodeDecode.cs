using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

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



namespace test
{
    class EncodeDecode
    {
        public enum NomMessage
        {
            INVALID_VALUE = -1,
            INFO_PERSO =0,
            INFO_POS =1,
            GET_ID = 2,
            SEND_ID = 3,
            GET_ALL_PARTICIPANT = 4,
            SEND_ALL_INFO_PERSO = 5
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

        private static  byte[] EncodeColor(Color couleur)
        {
            if (couleur != null)
                return Encoding.UTF8.GetBytes(GetHexString(couleur));
            else
                return Encoding.UTF8.GetBytes(GetHexString(Color.Gray));
        }





        public static byte[] EncoderMessage(EncodeDecode.NomMessage idMsg, int id, Position position, Color couleur, String pseudo)
        {
            int index = 0;
            byte[] idMsgByte = BitConverter.GetBytes((int)idMsg);
            byte[] idByte = BitConverter.GetBytes(id);
            byte[] latitudeByte = BitConverter.GetBytes(position.Latitude);
            byte[] longitudeByte = BitConverter.GetBytes(position.Longitude);
            byte[] colorByte = EncodeColor(couleur);



            byte[] data = new byte[32];
            idMsgByte.CopyTo(data, index);
            index += idMsgByte.Length;
            idByte.CopyTo(data, index);
            index += idByte.Length;
            switch (idMsg)
            {
                case EncodeDecode.NomMessage.INFO_PERSO:
                    colorByte.CopyTo(data, index);
                    index += colorByte.Length;
                    byte[] sizepseudo = BitConverter.GetBytes(pseudo.Length);
                    sizepseudo.CopyTo(data, index);
                    index += sizepseudo.Length;
                    byte[] pseudoByte = Encoding.UTF8.GetBytes(pseudo);
                    pseudoByte.CopyTo(data, index);
                    break;

                case EncodeDecode.NomMessage.INFO_POS:
                    latitudeByte.CopyTo(data, index);
                    index += latitudeByte.Length;
                    longitudeByte.CopyTo(data, index);
                    break;

                case EncodeDecode.NomMessage.GET_ID:
                    //rien de plus
                    break;

                case EncodeDecode.NomMessage.SEND_ID:
                    //rien de plus
                    break;

                case EncodeDecode.NomMessage.GET_ALL_PARTICIPANT:
                    //rien de plus
                    break;


                case EncodeDecode.NomMessage.SEND_ALL_INFO_PERSO:
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


        public static void DecoderMessage(byte[] message, ref EncodeDecode.NomMessage idmsg, ref int id, ref Position position, ref Color couleur, ref String pseudo)
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

            switch ((EncodeDecode.NomMessage)idMsgType)
            {
                case EncodeDecode.NomMessage.INFO_PERSO:
                    colorType = Encoding.UTF8.GetString(message, index, tailleCouleur);
                    index += tailleCouleur;
                    int sizePseudo = BitConverter.ToInt32(message, index);
                    index += sizeof(int);
                    pseudotype = Encoding.UTF8.GetString(message, index, sizePseudo);
                    break;

                case EncodeDecode.NomMessage.INFO_POS:
                    latitudeType = BitConverter.ToDouble(message, index);
                    index += sizeof(double);
                    longitudeType = BitConverter.ToDouble(message, index);
                    break;

                case EncodeDecode.NomMessage.GET_ID:
                    //rien de plus
                    break;

                case EncodeDecode.NomMessage.SEND_ID:
                    //rien de plus
                    break;

                case EncodeDecode.NomMessage.GET_ALL_PARTICIPANT:
                    //rien de plus
                    break;

                case EncodeDecode.NomMessage.SEND_ALL_INFO_PERSO:
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
            if (colorType != null)
                couleur = Color.FromHex(colorType);
            pseudo = pseudotype;

            Console.WriteLine("Decoder MESSAGE");
            Console.WriteLine("Decoder id : " + id);
            Console.WriteLine("Decoder pos : " + position.Latitude + "  " + position.Longitude);
            if (colorType != null)
                Console.WriteLine("Decoder couleur : " + couleur);
            if (pseudo != null)
                Console.WriteLine("Decoder pseudo : " + pseudo);
        }

    }
}
