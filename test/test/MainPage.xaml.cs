using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Forms.Maps;
using System.Collections.Generic;
using System.Threading;
using System.Net;

namespace test
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : ContentPage
    {
        Position MyPosition;
        readonly Position FirstPosition = new Position(43.1166700, 5.9333300); //lat , long
        readonly double FirstSliderZoom = 50; //en metre
        MapInfoSingleton mapInfo = MapInfoSingleton.Instance();

        public MainPage()
        {
            InitializeComponent();
            //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////// Partie map test
            /*   MyPosition = new Position(FirstPosition.Latitude, FirstPosition.Longitude);
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
   //            MyMap.CustomPins = new List<CustomPin> { MyPin };



               CustomPin MyPin2 = new CustomPin()
               {
                   Type = PinType.Generic,
                   Position = new Position(43.1117831, 5.9368637000000035),
                   Label = "2",
                   Address = "394 Pacific Ave, San Francisco CA",
                   Id = "0",
                   Dist = 0,
                   TypePin = Genre.VISIBLE,
                   PositionAffichage = new Position(43.1117831, 5.9368637000000035)
               };

               if (mapInfo.Map != null && MyPin2 != null)
               {
                   mapInfo.Map.CustomPins.Add(MyPin2);
                   mapInfo.Map.Pins.Add(MyPin2);
               }

               CustomPin MyPin3 = new CustomPin()
               {
                   Type = PinType.Generic,
                   Position = new Position(43.117135, 5.920857),
                   Label = "3",
                   Address = "394 Pacific Ave, San Francisco CA",
                   Id = "0",
                   Dist = 0,
                   TypePin = Genre.VISIBLE,
                   PositionAffichage = new Position(43.117135, 5.920857)
               };

               if (mapInfo.Map != null && MyPin3 != null)
               {
                   mapInfo.Map.CustomPins.Add(MyPin3);
                   mapInfo.Map.Pins.Add(MyPin3);
               }

               CustomPin MyPin4 = new CustomPin()
               {
                   Type = PinType.Generic,
                   Position = new Position(43.127847, 5.904292),
                   Label = "4",
                   Address = "394 Pacific Ave, San Francisco CA",
                   Id = "0",
                   Dist = 0,
                   TypePin = Genre.VISIBLE,
                   PositionAffichage = new Position(43.127847, 5.904292)
               };

               if (mapInfo.Map != null && MyPin4 != null)
               {
                   mapInfo.Map.CustomPins.Add(MyPin4);
                   mapInfo.Map.Pins.Add(MyPin4);
               }

               CustomPin MyPin5 = new CustomPin()
               {
                   Type = PinType.Generic,
                   Position = new Position(43.141824, 5.942325),
                   Label = "5",
                   Address = "394 Pacific Ave, San Francisco CA",
                   Id = "0",
                   Dist = 0,
                   TypePin = Genre.VISIBLE,
                   PositionAffichage = new Position(43.141824, 5.942325)
               };

               if (mapInfo.Map != null && MyPin5 != null)
               {
                   mapInfo.Map.CustomPins.Add(MyPin5);
                   mapInfo.Map.Pins.Add(MyPin5);
               }

               MyMap.MoveToRegion(MapSpan.FromCenterAndRadius(MyPosition, Distance.FromMeters(FirstSliderZoom)));
               mapInfo.Rect = new Rectangle();
               mapInfo.CamPos = new Position();
               mapInfo.MapSpan = MyMap.VisibleRegion;
               mapInfo.Map = MyMap;

               MyMap.CustomPins = new List<CustomPin> { MyPin, MyPin2, MyPin3, MyPin4, MyPin5 };

               MySliderZoom.ValueChanged += OnSliderZoomValueChange;
               MyLabelSlider.Text = MySliderZoom.Value.ToString();

               MySliderLat.ValueChanged += OnSliderLatValueChange;
               MySliderLong.ValueChanged += OnSliderLongValueChange;
               MyReinitButton.Clicked += OnButtonCliked;
               MyPinButton.Clicked += OnButtonAddClicked;

               //    TimerCallback timerDelegate = new TimerCallback(Refresh);
               //    Timer timer = new Timer(timerDelegate, MyMap, 1000, 2000);

               //Task taskA = Task.Factory.StartNew(() => Refresh(MyMap));
               */

            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////Partie client server test

            IPAddress[] address = Dns.GetHostAddresses(Dns.GetHostName());
            IpTextServer.Text = address[0].ToString();



        }

        private void OnButtonCliked(object sender, EventArgs e)
        {
            mapInfo.Map.MoveToRegion(MapSpan.FromCenterAndRadius(FirstPosition, Distance.FromMeters(FirstSliderZoom)));
           /* MyLabelSlider.Text = FirstSliderZoom.ToString();
            MySliderZoom.Value = FirstSliderZoom;
            MySliderLat.Value = FirstPosition.Latitude;
            MySliderLong.Value = FirstPosition.Longitude;*/
        }

        private void OnSliderZoomValueChange(object sender, ValueChangedEventArgs e)
        {
           /* mapInfo.Map.MoveToRegion(MapSpan.FromCenterAndRadius(MyPosition, Distance.FromMeters(MySliderZoom.Value)));
            MyLabelSlider.Text = MySliderZoom.Value.ToString();*/
        }

        private void OnSliderLatValueChange(object sender, ValueChangedEventArgs e)
        {
           /* Position pos = new Position(MyPosition.Latitude + MySliderLat.Value, MyPosition.Longitude);
            mapInfo.Map.MoveToRegion(MapSpan.FromCenterAndRadius(pos, Distance.FromMeters(MySliderZoom.Value)));*/
           // mapInfo.Map.Clear();
        }

        private void OnSliderLongValueChange(object sender, ValueChangedEventArgs e)
        {
           /* Position pos = new Position(MyPosition.Latitude, MyPosition.Longitude + +MySliderLong.Value);
            mapInfo.Map.MoveToRegion(MapSpan.FromCenterAndRadius(pos, Distance.FromMeters(MySliderZoom.Value)));*/
        }

        private void OnButtonAddClicked(object sender, EventArgs e)
        {
          /*  CustomPin MyPin = new CustomPin()
            {
                Type = PinType.Generic,
                Position = new Position(MyPosition.Latitude + MySliderLat.Value, MyPosition.Longitude + MySliderLong.Value),
                Label = "Pseudo",
                Address = "394 Pacific Ave, San Francisco CA",
                Id = "0",
                Dist = 0,
                TypePin = Genre.VISIBLE,
                PositionAffichage = new Position(MyPosition.Latitude + MySliderLat.Value, MyPosition.Longitude + MySliderLong.Value)
            };

            if (mapInfo.Map != null && MyPin != null)
            {
                mapInfo.Map.CustomPins.Add(MyPin);
                mapInfo.Map.Pins.Add(MyPin);
            }*/


        }

    }
}
