using System;
using System.Collections.Generic;
using Xamarin.Forms;
using System.Text;
using Xamarin.Forms.Maps;

namespace test
{
    class Personnage
    {
        public static int INVALID_ID_VALUE = -1;
        public static string DEFAULT_PSEUDO_VALUE = "NoPseudo";

        private String pseudo = "NoPseudo";
        private int id = INVALID_ID_VALUE;
        private Color color = Color.Red;
        private Position pos;

        public String Pseudo { get { return pseudo; } set { pseudo = value; } }
        public int ID { get { return id; } set { id = value; } }
        public Color Couleur { get { return color; } set { color = value; } }
        public Position Position { get { return pos; } set { pos = value; } }

    }
}
