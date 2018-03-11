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
using System.Net.NetworkInformation;
using System.Diagnostics;

namespace test
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : ContentPage
    {
        /*  Position MyPosition;
          readonly Position FirstPosition = new Position(43.1166700, 5.9333300); //lat , long
          readonly double FirstSliderZoom = 50; //en metre
          MapInfoSingleton mapInfo = MapInfoSingleton.Instance();*/


        test.Server server = null;
        test.Client client = null;
        test.GlobalSingleton labelGlobalInstance = null;
        Thread refreshThread = null;

        IPAddress[] address;

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

            address = Dns.GetHostAddresses(Dns.GetHostName());
            IpTextServer.Text = address[0].ToString();
            Console.WriteLine(address[0].ToString());

           var networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
           foreach ( var elem in networkInterfaces)
                 Console.WriteLine(elem.ToString());


            labelGlobalInstance = GlobalSingleton.Instance();
            refreshThread = new Thread(Refresh);
            refreshThread.Start();
            ServerButton.Clicked += OnButtonServerClicked;
            ClientButton.Clicked += OnButtonClientClicked;

        }

        private void Refresh()
        {

            Stopwatch watch = new Stopwatch();
            int interval = 33;//ms
            watch.Start();
            bool continuer = true;
            while (continuer)
            {
                if (watch.ElapsedMilliseconds > interval)
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        LabelMessageClient.Text = GlobalSingleton.Instance().LabelClient;
                        labelMessageServer.Text = GlobalSingleton.Instance().LabelServer;
                    });

                    watch.Restart();
                }
                else
                {
                    Thread.Sleep(33);
                }
            }
        }

        private void OnButtonServerClicked(object sender, EventArgs e)
        {
            if (server == null)
            {
                server = new test.Server(new IPEndPoint(address[0], 8080), 5);
            }

            if (!server.IsStarted())
            {
                server.Start();
                ServerButton.Text = "Stopper Serveur";
            }
            else
            {
                server.Stop();
                ServerButton.Text = "Demarrer Serveur";
            }
        }


        private void OnButtonClientClicked(object sender, EventArgs e)
        {
            if (client == null)
                client = new Client(new IPEndPoint(IPAddress.Parse(IpTextClient.Text), 8080));

            if (client != null && !client.IsStarted)
            {
                ClientButton.Text = "Deconnexion" ;
                client.Start();
            }
            else if (client != null && client.IsStarted)
            {
                ClientButton.Text = "Rejoindre";
                client.Stop();
            }
        }




        private void OnButtonCliked(object sender, EventArgs e)
        {
            /* mapInfo.Map.MoveToRegion(MapSpan.FromCenterAndRadius(FirstPosition, Distance.FromMeters(FirstSliderZoom)));
             MyLabelSlider.Text = FirstSliderZoom.ToString();
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
