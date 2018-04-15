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
                Label = "Xamarin San Francisco Office",
                Address = "394 Pacific Ave, San Francisco CA",
                Id = "0",
                Dist = 0,
                TypePin = Genre.OWN
            };


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
            MyMap.MoveToRegion(MapSpan.FromCenterAndRadius(MapInfoSingleton.Instance().GetPosition(),mapInfo.MapSpan.Radius));
        }

        protected override void OnDisappearing()
        {
            //deconnexion
            if (GlobalSingleton.Instance().Client != null)
                GlobalSingleton.Instance().Client.Stop();

            GlobalSingleton.Instance().Page2 = false;

            Navigation.PopAsync();
        }
    }
}