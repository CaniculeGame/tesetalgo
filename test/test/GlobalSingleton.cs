using System;
using System.Collections.Generic;
using System.Text;

namespace test
{
    class GlobalSingleton
    {
        private static GlobalSingleton instance = null;
        private string labelClient = "coucou";
        private string labelServer = "coucou";

        private GlobalSingleton() {}


        public static GlobalSingleton Instance()
        {
            if (instance == null)
                instance = new GlobalSingleton();

            return instance;
        }

        public string LabelClient { get { return labelClient; } set { labelClient = value; } }
        public string LabelServer { get { return labelServer; } set { labelServer = value; } }

    }
}
