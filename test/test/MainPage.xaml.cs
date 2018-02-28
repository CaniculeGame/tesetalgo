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
            MyMap.CustomPins = new List<CustomPin> { MyPin };

            MyMap.MoveToRegion(MapSpan.FromCenterAndRadius(MyPosition, Distance.FromMeters(FirstSliderZoom)));
            mapInfo.Rect = new Rectangle();
            mapInfo.CamPos = new Position();
            mapInfo.MapSpan = MyMap.VisibleRegion;
            mapInfo.Map = MyMap;


            MySliderZoom.ValueChanged += OnSliderZoomValueChange;
            MyLabelSlider.Text = MySliderZoom.Value.ToString();

            MySliderLat.ValueChanged += OnSliderLatValueChange;
            MySliderLong.ValueChanged += OnSliderLongValueChange;
            MyReinitButton.Clicked += OnButtonCliked;
            MyPinButton.Clicked += OnButtonAddClicked;

            //    TimerCallback timerDelegate = new TimerCallback(Refresh);
            //    Timer timer = new Timer(timerDelegate, MyMap, 1000, 2000);

            //Task taskA = Task.Factory.StartNew(() => Refresh(MyMap));

        }

        private void OnButtonCliked(object sender, EventArgs e)
        {
            mapInfo.Map.MoveToRegion(MapSpan.FromCenterAndRadius(FirstPosition, Distance.FromMeters(FirstSliderZoom)));
            MyLabelSlider.Text = FirstSliderZoom.ToString();
            MySliderZoom.Value = FirstSliderZoom;
            MySliderLat.Value = FirstPosition.Latitude;
            MySliderLong.Value = FirstPosition.Longitude;
        }

        private void OnSliderZoomValueChange(object sender, ValueChangedEventArgs e)
        {
            mapInfo.Map.MoveToRegion(MapSpan.FromCenterAndRadius(MyPosition, Distance.FromMeters(MySliderZoom.Value)));
            MyLabelSlider.Text = MySliderZoom.Value.ToString();
        }

        private void OnSliderLatValueChange(object sender, ValueChangedEventArgs e)
        {
            Position pos = new Position(MyPosition.Latitude + MySliderLat.Value, MyPosition.Longitude);
            mapInfo.Map.MoveToRegion(MapSpan.FromCenterAndRadius(pos, Distance.FromMeters(MySliderZoom.Value)));
           // mapInfo.Map.Clear();
        }

        private void OnSliderLongValueChange(object sender, ValueChangedEventArgs e)
        {
            Position pos = new Position(MyPosition.Latitude, MyPosition.Longitude + +MySliderLong.Value);
            mapInfo.Map.MoveToRegion(MapSpan.FromCenterAndRadius(pos, Distance.FromMeters(MySliderZoom.Value)));
        }

        private void OnButtonAddClicked(object sender, EventArgs e)
        {
            CustomPin MyPin = new CustomPin()
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
            }


        }

    }
}
