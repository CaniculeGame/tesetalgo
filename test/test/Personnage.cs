using System;
using System.Collections.Generic;
using Xamarin.Forms;
using System.Text;

namespace test
{
    class Personnage
    {
        private String pseudo = "NoPseudo";
        private int id = 0;
        private Color color = Color.Red;

        public String Pseudo { get { return pseudo; } set { pseudo = value; } }
        public int ID { get { return id; } set { id = value; } }
        public Color Couleur { get { return color; } set { color = value; } }

    }
}
