using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace test
{
    class MapInfoSingleton
    {
        private static MapInfoSingleton instance = null;

        private Position pos; // ma position sur la map
        private Position camera;
        private Rectangle ViewPort;
        private MapSpan mapSpan;
        private CustomMap map;

        public static MapInfoSingleton Instance()
        {
            if (instance == null)
                instance = new MapInfoSingleton();

            return instance;
        }

        public CustomMap Map
        {
            get { return map; }
            set { map = value; }
        }

        public void SetPosition(Position p)
        {
            pos = p;
        }

        public Position GetPosition() { return pos; }

        public Rectangle Rect
        {
            get { return ViewPort; }
            set { ViewPort = value; }
        }

        public Position CamPos
        {
            get { return camera; }
            set { camera = value; }
        }

        public MapSpan MapSpan
        {
            get { return mapSpan; }
            set { mapSpan = value; }
        }
    }
}
