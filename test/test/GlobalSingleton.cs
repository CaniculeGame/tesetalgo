using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace test
{
    class GlobalSingleton
    {
        private static GlobalSingleton instance = null;
        private string labelClient = "";
        private string labelServer = "";
        private string boutonServerLabel = null;
        private string boutonClientLabel = null;
        private Position myPosition;
        private List<Position> positionsAmis = null;
        private Client client = null;
        private Server server = null;
        private Thread refreshThread = null;
        private bool page2 = false;
        private Personnage perso = null;
        private List<Personnage> participants = null;

        private GlobalSingleton()
        {
            perso = new Personnage();
            participants = new List<Personnage>();
        }


        public static GlobalSingleton Instance()
        {
            if (instance == null)
                instance = new GlobalSingleton();

            return instance;
        }

        public string LabelClient { get { return labelClient; } set { labelClient = value; } }
        public string LabelServer { get { return labelServer; } set { labelServer = value; } }
        public string ButtonClientLabel { get { return boutonClientLabel; } set { boutonClientLabel = value; } }
        public string ButtonServerLabel { get { return boutonServerLabel; } set { boutonServerLabel = value; } }
        public Position MaPosition { get { return myPosition; } set { myPosition = value; } }
        public List<Position> PositionsAmis { get { return positionsAmis; } set { positionsAmis = value; } }
        public Client Client { get { return client; } set { client = value; } }
        public Server Server { get { return server; } set { server = value; } }
        public Thread RefreshThread { get { return refreshThread; } set { refreshThread = value; } }
        public bool Page2 { get { return page2; } set { page2 = value; } }
        public Personnage Perso { get { return perso; } set { perso = value; } }
        public List<Personnage> Participants { get { return participants; } set { participants = value; } }
    }
}
