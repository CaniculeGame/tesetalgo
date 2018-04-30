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
    public partial class MainPageTest : ContentPage
    {

        test.GlobalSingleton labelGlobalInstance = null;

        IPAddress[] address;



        public MainPageTest()
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
           // IpTextServer.Text = address[0].ToString();
            Console.WriteLine(address[0].ToString());

            var networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
            foreach (var elem in networkInterfaces)
                Console.WriteLine(elem.ToString());


            labelGlobalInstance = GlobalSingleton.Instance();
            GlobalSingleton.Instance().RefreshThread = new Thread(Refresh);
            GlobalSingleton.Instance().RefreshThread.Start();
            ClientButton.Clicked += OnButtonClientClicked;

            GlobalSingleton.Instance().ButtonClientLabel = ClientButton.Text;
            Box0.Color = GlobalSingleton.Instance().Perso.Couleur;

        }

        private void Refresh()
        {

            Stopwatch watch = new Stopwatch();
            int interval = 33;//ms
            watch.Start();
            bool continuer = true;
            GlobalSingleton.Instance().Page2 = false;
            while (continuer)
            {
                if (watch.ElapsedMilliseconds > interval)
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        LabelMessageClient.Text = GlobalSingleton.Instance().LabelClient;
                        labelMessageServer.Text = GlobalSingleton.Instance().LabelServer;
                        ClientButton.Text = GlobalSingleton.Instance().ButtonClientLabel;
                      //  ServerButton.Text = GlobalSingleton.Instance().ButtonServerLabel;


                        //si on est co on chage de page
                        if (GlobalSingleton.Instance().Client != null && GlobalSingleton.Instance().Client.IsConnected && !GlobalSingleton.Instance().Page2)
                        {
                            //changer de page
                            Navigation.PushAsync(new MapPage());
                            GlobalSingleton.Instance().Page2 = true;
                            
                        }
                    });

                    watch.Restart();
                }
                else
                {
                    Thread.Sleep(33);
                }
            }

            Console.WriteLine("FIN THREAD MAJ et changement d ecran");
        }

        protected override void OnAppearing()
        {
            //deconnexion
            GlobalSingleton.Instance().Page2 = false;
            Box0.Color = GlobalSingleton.Instance().Perso.Couleur;
        }

        private void OnButtonServerClicked(object sender, EventArgs e)
        {
           /* if (pseudo.Text != null && pseudo.Text != "")
                GlobalSingleton.Instance().Perso.Pseudo = pseudo.Text;
            else */
                GlobalSingleton.Instance().Perso.Pseudo = "NoPseudo";

            if (GlobalSingleton.Instance().Server == null)
            {
                GlobalSingleton.Instance().Server = new test.Server(new IPEndPoint(address[0], 8080), 5);
               
            }

            if (!GlobalSingleton.Instance().Server.IsStarted())
            {
                GlobalSingleton.Instance().Server.Start();
                GlobalSingleton.Instance().ButtonServerLabel = "Stopper Serveur";
            }
            else
            {
                GlobalSingleton.Instance().Server.Stop();
                GlobalSingleton.Instance().ButtonServerLabel = "Demarrer Serveur";
            }
        }


        private void OnButtonClientClicked(object sender, EventArgs e)
        {
            /*if (pseudo.Text != null && pseudo.Text != "")
                GlobalSingleton.Instance().Perso.Pseudo = pseudo.Text;
            else*/
                GlobalSingleton.Instance().Perso.Pseudo = "NoPseudo";

            if (GlobalSingleton.Instance().Client == null && IpTextClient.Text != null)
                GlobalSingleton.Instance().Client = new Client(new IPEndPoint(IPAddress.Parse(IpTextClient.Text), 8080));

            if (GlobalSingleton.Instance().Client != null && !GlobalSingleton.Instance().Client.IsStarted)
            {
                GlobalSingleton.Instance().ButtonClientLabel = "Connection en cours";
                GlobalSingleton.Instance().Client.Start();
            }
            else if (GlobalSingleton.Instance().Client != null && GlobalSingleton.Instance().Client.IsStarted)
            {
                GlobalSingleton.Instance().ButtonClientLabel = "Rejoindre";
                GlobalSingleton.Instance().Client.Stop();
            }
        }


        private void OnbuttonColorCLicked(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            if (button != null)
            {
                if (GlobalSingleton.Instance().Perso != null)
                {
                    GlobalSingleton.Instance().Perso.Couleur = button.TextColor;
                }

                button.BorderColor = Color.AntiqueWhite;
                button.BorderWidth = 2;

                Box0.Color = button.BackgroundColor;

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
