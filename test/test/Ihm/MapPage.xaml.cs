using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Xaml;

namespace test
{


	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class MapPage : ContentPage
    {
         Position MyPosition;
         readonly Position FirstPosition = GlobalSingleton.Instance().MaPosition;
         readonly double FirstSliderZoom = 50; //en metre
         MapInfoSingleton mapInfo = MapInfoSingleton.Instance();


        public MapPage ()
		{
			InitializeComponent ();

            MyPosition = new Position(FirstPosition.Latitude, FirstPosition.Longitude);
            mapInfo.SetPosition(MyPosition);


            CustomPin MyPin = new CustomPin()
            {
                Type = PinType.Generic,
                Position = MyPosition,
                Label = GlobalSingleton.Instance().Perso.Pseudo,
                Address = EncodeDecode.GetHexString(GlobalSingleton.Instance().Perso.Couleur),
                Id = GlobalSingleton.Instance().Perso.ID,
                Dist = 0,
                TypePin = Genre.OWN
            };

            Box0.Color = GlobalSingleton.Instance().Perso.Couleur;
            Box1.Color = GlobalSingleton.Instance().Perso.Couleur;
            MyMap.Pins.Add(MyPin);
 
            MyMap.MoveToRegion(MapSpan.FromCenterAndRadius(MyPosition, Distance.FromMeters(FirstSliderZoom)));
            mapInfo.Rect = new Rectangle();
            mapInfo.CamPos = new Position();
            mapInfo.MapSpan = MyMap.VisibleRegion;
            mapInfo.Map = MyMap;

            MyMap.CustomPins = new List<CustomPin> { MyPin };

            MyReinitButton.Clicked += ReCentrerPosition;
            GlobalSingleton.Instance().Client.NeedToReCenterPos += new EventHandler(ReCentrerPosition);

        }


        public  void ReCentrerPosition(object sender, EventArgs e)
        {
            try
            { 
            if (MyMap != null)
                    MyMap.MoveToRegion(MapSpan.FromCenterAndRadius(MapInfoSingleton.Instance().GetPosition(), mapInfo.MapSpan.Radius));
            }catch(Exception exc) { Console.WriteLine(exc); }
        }

        protected override void OnDisappearing()
        {
            //deconnexion
            //GlobalSingleton.Instance().Page2 = false;
            if (GlobalSingleton.Instance().Client != null)
                GlobalSingleton.Instance().Client.Stop();

            //destroy all
            MyMap = null;
        }
    }
}