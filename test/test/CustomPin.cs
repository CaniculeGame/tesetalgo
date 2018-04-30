using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Xamarin.Forms.Maps;

namespace test
{
    class CustomPin : Pin
    {
        //Position carte;


        public Genre TypePin { get; set; }
        public string Url { get; set; }
        public int Dist { get; set; }
        public Position PositionAffichage { get; set; }

    }
}
