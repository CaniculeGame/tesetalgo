using System;
using System.Collections.Generic;
using Android.Content;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Graphics;
using Android.Widget;
using test;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Maps.Android;
using Android.Locations;

[assembly: ExportRenderer(typeof(CustomMap), typeof(test.droid.MyMapRenderer))]
namespace test.droid
{
    class MyMapRenderer : MapRenderer, GoogleMap.IInfoWindowAdapter
    {
        private List<CustomPin> customPins;
        private GoogleMap map = null;
        private Bitmap bmp = null;
        private Bitmap resizedBitmap = null;
        private Bitmap flagBitmap = null;
        private Bitmap resizedFlagBitmap = null;
        private readonly static double ERROR_VALUE_LATLONG = 200;
        private readonly Vec2 ERROR_VALUE_VEC_LATLONG = new Vec2(ERROR_VALUE_LATLONG, ERROR_VALUE_LATLONG);



        public MyMapRenderer(Context context) : base(context)
        {
            bmp = BitmapFactory.DecodeResource(Resources, test.Droid.Resource.Drawable.location_icon);
            resizedBitmap = Bitmap.CreateScaledBitmap(bmp, 128, 128, false);

            GlobalSingleton.Instance().Client.NeedRefreshMap += new EventHandler(RefreshPosition);

            flagBitmap = BitmapFactory.DecodeResource(Resources, test.Droid.Resource.Drawable.flag);
            resizedFlagBitmap = Bitmap.CreateScaledBitmap(flagBitmap, 64, 64, false);

        }

        public  void MakePointToReachMarker(object sender, GoogleMap.MapLongClickEventArgs e)
        {
            MapInfoSingleton mapInfo = MapInfoSingleton.Instance();

            LatLng pos = (LatLng)e.Point;

            CustomPin newPin = new CustomPin()
            {
                Type = PinType.Generic,
                Position = new Position(pos.Latitude, pos.Longitude),
                Label = "10",
                Address = "394 Pacific Ave, San Francisco CA",
                Id = "0",
                Dist = 0,
                TypePin = Genre.VISIBLE_REACHPOINT,
                PositionAffichage = new Position(pos.Latitude, pos.Longitude)
            };

            if (mapInfo.Map != null && newPin != null)
            {
                mapInfo.Map.CustomPins.Add(newPin);
                mapInfo.Map.Pins.Add(newPin);
            }
        }

        public global::Android.Views.View GetInfoContents(Marker marker)
        {
            var inflater = Android.App.Application.Context.GetSystemService(Context.LayoutInflaterService) as Android.Views.LayoutInflater;
            if (inflater != null)
            {
                Android.Views.View view;
                view = inflater.Inflate(2130903067, null);


                TextView infoTitle = view.FindViewById<TextView>(2131427456);
                TextView infoSubtitle = view.FindViewById<TextView>(2131427457);

                if (infoTitle != null)
                {
                    infoTitle.Text = "Flore est la plus belle de toute! note bien ca khey";
                    infoTitle.SetTextColor(Android.Graphics.Color.IndianRed);
                }

                if (infoSubtitle != null)
                {
                    infoSubtitle.Text = "500 m";
                    infoTitle.SetTextColor(Android.Graphics.Color.IndianRed);
                }

                return view;
            }
            return null;
        }

        public global::Android.Views.View GetInfoWindow(Marker marker)
        {
            return null;
        }


        protected override void OnElementChanged(Xamarin.Forms.Platform.Android.ElementChangedEventArgs<Map> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null)
            {
                // NativeMap.InfoWindowClick -= Xamarin.Forms.Maps.OnInfoWindowClick;
            }

            if (e.NewElement != null)
            {
                var formsMap = (CustomMap)e.NewElement;
                customPins = formsMap.CustomPins;
                Control.GetMapAsync(this);
            }
        }

        protected override void OnMapReady(GoogleMap map)
        {
            base.OnMapReady(map);
            NativeMap.SetInfoWindowAdapter(this);
            this.map = map;

            if (this.map != null)
                this.map.MapLongClick += MakePointToReachMarker;
        }

        protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            Refresh(map);

            //temporaire: affichage de ligne 
            try
            {

               /* Vec2 E = new Vec2(map.Projection.VisibleRegion.LatLngBounds.Southwest.Latitude , map.Projection.VisibleRegion.LatLngBounds.Northeast.Longitude );
                Vec2 F = new Vec2(map.Projection.VisibleRegion.LatLngBounds.Northeast.Latitude , map.Projection.VisibleRegion.LatLngBounds.Northeast.Longitude );
                Vec2 G = new Vec2(map.Projection.VisibleRegion.LatLngBounds.Northeast.Latitude , map.Projection.VisibleRegion.LatLngBounds.Southwest.Longitude );
                Vec2 H = new Vec2(map.Projection.VisibleRegion.LatLngBounds.Southwest.Latitude , map.Projection.VisibleRegion.LatLngBounds.Southwest.Longitude );
                PolygonOptions polygonOption = new PolygonOptions();
                polygonOption.Add(new LatLng(E.X,E.Y), new LatLng(F.X, F.Y), new LatLng(G.X, G.Y), new LatLng(H.X, H.Y));

                polygonOption.InvokeStrokeColor(Android.Graphics.Color.Orange);


                map.AddPolygon(polygonOption);*/
                

            /*    Vec2 E1 = new Vec2(map.Projection.VisibleRegion.LatLngBounds.Southwest.Latitude , map.Projection.VisibleRegion.LatLngBounds.Northeast.Longitude );
                Vec2 F1 = new Vec2(map.Projection.VisibleRegion.LatLngBounds.Northeast.Latitude  , map.Projection.VisibleRegion.LatLngBounds.Northeast.Longitude);
                Vec2 G1 = new Vec2(map.Projection.VisibleRegion.LatLngBounds.Northeast.Latitude , map.Projection.VisibleRegion.LatLngBounds.Southwest.Longitude );
                Vec2 H1 = new Vec2(map.Projection.VisibleRegion.LatLngBounds.Southwest.Latitude , map.Projection.VisibleRegion.LatLngBounds.Southwest.Longitude );

                Vec2 E2 = MovePoint(E1.X, E1.Y, 50, 315);
                Vec2 F2 = MovePoint(F1.X, F1.Y, 50, 225);
                Vec2 G2 = MovePoint(G1.X, G1.Y, 50, 135);
                Vec2 H2 = MovePoint(H1.X, H1.Y, 50, 45);

                PolygonOptions polygonOption1 = new PolygonOptions(); 
                polygonOption1.Add(new LatLng(E2.X, E2.Y), new LatLng(F2.X, F2.Y), new LatLng(G2.X, G2.Y), new LatLng(H2.X, H2.Y));

                polygonOption1.InvokeStrokeColor(Android.Graphics.Color.Violet);

                map.AddPolygon(polygonOption1);*/
                

                /* PolylineOptions polyline1 = new PolylineOptions();
                   polyline1.Add(new LatLng(0,0), new LatLng(0,0));
                   polyline1.InvokeColor(Android.Graphics.Color.Red);
                   polyline1.InvokeWidth(25);

                   map.AddPolyline(polyline1);

                   PolylineOptions polyline2 = new PolylineOptions();
                   polyline2.Add(new LatLng(0,0), new LatLng(0,0));
                   polyline2.InvokeColor(Android.Graphics.Color.White);
                   polyline2.InvokeWidth(25);

                   map.AddPolyline(polyline2);*/

            }
            catch(Exception exc) { }


        }


        protected override MarkerOptions CreateMarker(Pin pin)
        {
            MarkerOptions CustomMarker = new MarkerOptions();

            CustomPin MyPin = FindCustomPin(pin);
            if (MyPin != null)
            {
                if (MyPin.TypePin == Genre.PAS_VISIBLE) // marker personaliser
                {

                    Position posNear = new Position(map.Projection.VisibleRegion.NearRight.Latitude, map.Projection.VisibleRegion.NearRight.Longitude);
                    Position posFar = new Position(map.Projection.VisibleRegion.FarRight.Latitude, map.Projection.VisibleRegion.FarRight.Longitude);

                    float rayon = (float)DistanceKm(posNear, posFar); //en km
                    Console.WriteLine("r = " + rayon);
                    BitmapDescriptor bm = GetTextMarker(MyPin.Label, MyPin.Dist, Android.Graphics.Color.Red, 45f);
                    //MyPin.PositionAffichage = CalculPosition(MapInfoSingleton.Instance().GetPosition(), pin.Position, rayon);



                    Position centerCameraMap = new Position(map.Projection.VisibleRegion.LatLngBounds.Center.Latitude, map.Projection.VisibleRegion.LatLngBounds.Center.Longitude);
                    //MyPin.PositionAffichage = CalculPosition(pin.Position, centerCameraMap, map.Projection.VisibleRegion.LatLngBounds);
                    MyPin.PositionAffichage = CalculNouvellePosition(centerCameraMap, pin.Position, map.Projection.VisibleRegion.LatLngBounds, pin.Label);

                    Console.WriteLine(map.Projection.VisibleRegion.LatLngBounds.ToString());


                    Console.WriteLine("(" + MyPin.Position.Latitude + " , " + pin.Position.Longitude + ")   " + MyPin.PositionAffichage.Latitude + "  " + MyPin.PositionAffichage.Longitude);
                    CustomMarker.SetPosition(new LatLng(MyPin.PositionAffichage.Latitude, MyPin.PositionAffichage.Longitude));
                    //  CustomMarker.SetPosition(new LatLng(pin.Position.Latitude,pin.Position.Longitude));
                    CustomMarker.SetIcon(bm);
                }
                else if (MyPin.TypePin == Genre.VISIBLE) // marker fleche
                {
                    CustomMarker.SetPosition(new LatLng(pin.Position.Latitude, pin.Position.Longitude));
                    CustomMarker.SetIcon(BitmapDescriptorFactory.FromBitmap(resizedBitmap));
                }
                else if (MyPin.TypePin == Genre.OWN) // own
                {
                    CustomMarker.SetPosition(new LatLng(pin.Position.Latitude, pin.Position.Longitude));
                    CustomMarker.SetIcon(BitmapDescriptorFactory.FromBitmap(resizedBitmap));
                }
                else if (MyPin.TypePin == Genre.VISIBLE_REACHPOINT)
                {
                    CustomMarker.SetPosition(new LatLng(pin.Position.Latitude, pin.Position.Longitude));
                    CustomMarker.SetIcon(BitmapDescriptorFactory.FromBitmap(resizedFlagBitmap));
                }
                else if (MyPin.TypePin == Genre.PAS_VISIBLE_REACHPOINT)
                {
                    CustomMarker.SetPosition(new LatLng(pin.Position.Latitude, pin.Position.Longitude));
                    CustomMarker.SetIcon(BitmapDescriptorFactory.FromBitmap(resizedFlagBitmap));
                }

                return CustomMarker;
            }
            return null;
        }

        /*
         *  CalculPosition
         *  
         *  Retourne la position au bord de l'ecran d'une position à l'exterieur de l'ecran
         *  
         * Soit A, un point
         * Soit B, un point
         * Soit AB le vecteur representant le segment AB
         * Soit zone, la zone composé des points, formant des segments
         * 
         * Algo
         * On va chercher à savoir quelle droite de la zone, le segment AB coupe
         * Pour cela on crée les points et on forme les vecteurs de la zone
         * Puis on va chercher  si oui ou non , le segment intersecte un des coté
         * 
         * PB : valide que pour les point qui sont exactement sur le mm axe sinon cmarche pas :(
         */
        private Position CalculPosition(Position A, Position B, LatLngBounds zone)
        {

            //creation des points
            Vec2 E = new Vec2(zone.Southwest.Latitude, zone.Northeast.Longitude);
            Vec2 F = new Vec2(zone.Northeast.Latitude, zone.Northeast.Longitude);
            Vec2 G = new Vec2(zone.Northeast.Latitude, zone.Southwest.Longitude);
            Vec2 H = new Vec2(zone.Southwest.Latitude,zone.Southwest.Longitude);
            //formation des vecteurs
            Vec2 AB = new Vec2(B.Latitude - A.Latitude, B.Longitude - A.Longitude);
            Vec2 EF = new Vec2(F.X - E.X, F.Y - E.Y);
            Vec2 FG = new Vec2(G.X - F.X, G.Y - F.Y);
            Vec2 GH = new Vec2(H.X - G.X, H.Y - G.Y);
            Vec2 HE = new Vec2(E.X - H.X, E.Y - H.Y );


            //recherche de l'emplacement du point calculé
            Byte flag = 0;
            if (Instersection(EF, new Vec2(A.Latitude - F.X, A.Longitude - F.Y), new Vec2(B.Latitude - F.X, B.Longitude - F.Y)))
                flag += 1;
             if (Instersection(FG, new Vec2(A.Latitude - G.X, A.Longitude - G.Y), new Vec2(B.Latitude - G.X, B.Longitude - G.Y)))
                flag += 2;
            if (Instersection(GH, new Vec2(A.Latitude - H.X, A.Longitude - H.Y), new Vec2(B.Latitude - H.X, B.Longitude - H.Y)))
                flag += 4;
            if (Instersection(HE, new Vec2(A.Latitude - E.X, A.Longitude - E.Y), new Vec2(B.Latitude - E.X, B.Longitude - E.Y)))
                flag += 8;


            if ((flag & 0b1) !=0)
                return new Position(zone.Northeast.Latitude, A.Longitude); //ok
            if ((flag & 0b10) != 0)
                return new Position(A.Latitude, zone.Northeast.Longitude); //ok
            if ((flag & 0b100) != 0)
                return new Position(zone.Southwest.Latitude, A.Longitude);//ok?
            if ((flag & 0b1000) != 0)
                return new Position(A.Latitude, zone.Southwest.Longitude); //ok?

            return A;

        }



        private Position CalculNouvellePosition(Position A, Position B, LatLngBounds zone, string label)
        {
            //creation des points
            Vec2 E = new Vec2(zone.Southwest.Latitude , zone.Northeast.Longitude );
            Vec2 F = new Vec2(zone.Northeast.Latitude , zone.Northeast.Longitude );
            Vec2 G = new Vec2(zone.Northeast.Latitude , zone.Southwest.Longitude );
            Vec2 H = new Vec2(zone.Southwest.Latitude , zone.Southwest.Longitude );

            Vec2 E2 = MovePoint(E.X, E.Y, 50, 315);
            Vec2 F2 = MovePoint(F.X, F.Y, 50, 225);
            Vec2 G2 = MovePoint(G.X, G.Y, 50, 135);
            Vec2 H2 = MovePoint(H.X, H.Y, 50, 45);

            float[] result = new float[3];
            Location.DistanceBetween(E.X, E.Y, F.X, F.Y, result);

            double brng = CalculateBearing(A, B);
            double dist = result[0] / 2;
            Vec2 newPos = MovePoint(A.Latitude, A.Longitude, dist, brng);


            try
                {
                    PolylineOptions polyline = new PolylineOptions();
                    polyline.Add(new LatLng(B.Latitude, B.Longitude), new LatLng(A.Latitude, A.Longitude));
                    polyline.InvokeColor(Android.Graphics.Color.Black);
                    polyline.InvokeWidth(15);

                    map.AddPolyline(polyline);
                }
                catch (Exception exc)
                { Console.WriteLine(exc); }

            return new Position(newPos.X, newPos.Y);

        }


        private Vec2 IntesectionPoints(Vec2 A, Vec2 B, Vec2 C, Vec2 D)
        {
            Vec2 AB = new Vec2(B.X - A.X, B.Y - A.Y);
            Vec2 CD = new Vec2(D.X - C.X, D.Y - C.Y);

            double m = 0;
            double diviseur = (AB.X * CD.Y) - (AB.Y * CD.X);

            if (diviseur != 0)
            {
                m = (AB.X * A.Y
                     - AB.X * C.Y
                     - AB.Y * A.X
                     + AB.Y * C.X
                    ) / diviseur;
            }

            if (m < 0 && m < 1)
                return new Vec2(C.X + m * CD.X, C.Y + m * CD.Y);
            else
                return ERROR_VALUE_VEC_LATLONG;
        }

        private Vec2 IntesectionPoints(Position A, Position B, Vec2 C, Vec2 D)
        {
            Vec2 AB = new Vec2(B.Latitude - A.Latitude, B.Longitude - A.Longitude);
            Vec2 CD = new Vec2(D.X - C.X, D.Y - C.Y);

            double m = 0;
            double k = 0;
            double diviseur = (AB.X * CD.Y) - (AB.Y * CD.X);

            if (diviseur != 0)
            {
                m = (AB.X * A.Longitude
                     - AB.X * C.Y
                     - AB.Y * A.Latitude
                     + AB.Y * C.X
                    ) / diviseur;

                k = (CD.X * A.Longitude
                 - CD.X * C.Y
                 - CD.Y * A.Latitude
                 + CD.Y * C.X
                ) / diviseur;
            }

            if (k > 0 && k < 1)
                return new Vec2(A.Latitude + k * AB.X, A.Longitude + k * AB.Y);
            else
                return ERROR_VALUE_VEC_LATLONG;
        }


        private bool IntersecteLeSegment(Vec2 A, Vec2 B, Vec2 C, Vec2 D)
        {
            Vec2 seg = IntesectionPoints(A, B, C, D);
            if (seg.Equals(ERROR_VALUE_VEC_LATLONG))
                return false;
            else
                return true;
        }

        private bool IntersecteLeSegment(Position A, Position B, Vec2 C, Vec2 D)
        {
            Vec2 seg = IntesectionPoints(A, B, C, D);
            if (seg.Equals(ERROR_VALUE_VEC_LATLONG))
                return false;
            else
                return true;
        }


        private double Distance(Vec2 A, Vec2 B)
        {
            double X = Math.Pow(B.X - A.X, 2);
            double Y = Math.Pow(B.Y - A.Y, 2);
            return Math.Sqrt(X + Y);
        }

        private double Distance(Position A, Position B)
        {
            double X = Math.Pow(B.Latitude - A.Latitude, 2);
            double Y = Math.Pow(B.Longitude - A.Longitude, 2);
            return Math.Sqrt(X + Y);
        }

        //Calcul simple du derterminant
        private double Determiant(Vec2 A, Vec2 B)
        {
            return (A.X * B.Y) - (B.X * A.Y);
        }

        // Pour savoir si un segment coupe un autre, il suffit qu'il y ai un point de chaque coté de la droite a tester
        // on applique donc 2 determiant et si le resultat est < 0 c'est gagné
        private Boolean Instersection(Vec2 A, Vec2 B, Vec2 C)
        {
            if (Determiant(A, B) * Determiant(A, C) < 0)
                return true;
            else
                return false;
        }

        private CustomPin FindCustomPin(Pin pin)
        {
            var position = new Position(pin.Position.Latitude, pin.Position.Longitude);
            foreach (var cspin in customPins)
            {
                if (cspin.Position == position)
                {
                    return cspin;
                }
            }
            return null;
        }

        private BitmapDescriptor GetTextMarker(String pseudo, int distance, Android.Graphics.Color couleur, float angle)
        {

            Paint paint = new Paint();
            String taillePseudoMax = "pseudo";

            //parametres
            paint.TextSize = 64;
            int offset = 30;
            paint.TextAlign = Paint.Align.Left;

            Bitmap bmp = BitmapFactory.DecodeResource(Resources, test.Droid.Resource.Drawable.icon);
            Bitmap img = Bitmap.CreateScaledBitmap(bmp, ((int)paint.TextSize / 2 + offset) * 2, ((int)paint.TextSize / 2 + offset) * 2, true);


            //definiton de la taille du canvas qui va contenir les informations

            //taille total du canvas  texte + img + fleche + offset
            int widthTotal = (int)paint.MeasureText(taillePseudoMax) + img.Width + offset * 4 + ((int)paint.TextSize / 2 + offset);
            int heightTotal = img.Width + offset * 4;
            //taille texte + image
            int width = (int)paint.MeasureText(taillePseudoMax) + img.Width + offset * 2;
            int height = img.Width + offset * 2;


            // Creation de l'image que l'on vas constuire pas a pas
            Bitmap image = Bitmap.CreateBitmap(widthTotal, heightTotal, Bitmap.Config.Argb8888);
            Canvas canvas = new Canvas(image);
            //canvas.DrawColor(Android.Graphics.Color.White);

            //hop hop hop !!! on dessine 
            paint.Color = new Android.Graphics.Color(220, 220, 220, 100);
            canvas.DrawRect(new Rect(offset * 2, offset * 2, offset * 2 + width, height), paint);

            //paint.Color = new Android.Graphics.Color(128,128,128,255);
            paint.Color = couleur;
            paint.Alpha = 255;

            float[] pts = new float[16];
            pts[0] = offset * 2;
            pts[1] = offset * 2;
            pts[2] = offset * 2 + (float)width;
            pts[3] = offset * 2;

            pts[4] = offset * 2 + (float)width;
            pts[5] = offset * 2 + 2f;
            pts[6] = offset * 2 + (float)width - 2;
            pts[7] = (float)height - 2;

            pts[8] = offset * 2 + (float)width - 2;
            pts[9] = (float)height - 2;
            pts[10] = offset * 2 + 2f;
            pts[11] = (float)height - 2;

            pts[12] = offset * 2 + 2f;
            pts[13] = (float)height - 2;
            pts[14] = offset * 2 + 2f;
            pts[15] = offset * 2;
            canvas.DrawLines(pts, paint);


            Path triangle = MakePath(angle, width, height, widthTotal, heightTotal, offset);
            paint.Alpha = 128;
            paint.SetStyle(Paint.Style.Fill);
            canvas.DrawPath(triangle, paint);


            paint.Color = Android.Graphics.Color.Black;
            canvas.DrawText(pseudo, offset * 2 + img.Width + offset + 10, paint.TextSize / 2 + offset + offset * 2, paint);
            float s = paint.TextSize;

            paint.TextSize = 60;
            paint.Color = new Android.Graphics.Color(255, 171, 13, 255);
            canvas.DrawText(distance + " Km", offset * 2 + img.Width + offset + 10, s + offset * 2 + offset * 2, paint);

            canvas.DrawBitmap(img, offset * 2 + offset, offset * 2, paint);


            return BitmapDescriptorFactory.FromBitmap(image);
        }

        private Path MakePath(float angle, int width, int height, int widthTotal, int heightTotal, int size)
        {
            Path triangle = new Path();

            //Est
            if ((angle >= 0 && angle < 45) || (angle >= 315 && angle <= 0))
            {
                triangle.MoveTo(width + size, 0f);
                triangle.LineTo(widthTotal, height / 2);
                triangle.LineTo(width + size, height);
                triangle.LineTo(width + size, 0f);
            }
            //Nord
            else if (angle >= 45 && angle < 135)
            {
                triangle.MoveTo(widthTotal / 2 + height / 2, size * 2);
                triangle.LineTo(widthTotal / 2, 0);
                triangle.LineTo(widthTotal / 2 - height / 2, size * 2);
                triangle.LineTo(widthTotal / 2 + height / 2, size * 2);
            }
            //Ouest
            else if (angle >= 135 && angle < 225)
            {
                triangle.MoveTo(size, 0f);
                triangle.LineTo(0, height / 2);
                triangle.LineTo(size, height);
                triangle.LineTo(size, 0f);

            }
            //Sud
            else if (angle >= 225 && angle < 315)
            {
                triangle.MoveTo(widthTotal / 2 + height / 2, height + size);
                triangle.LineTo(widthTotal / 2, heightTotal);
                triangle.LineTo(widthTotal / 2 - height / 2, heightTotal - size);
                triangle.LineTo(widthTotal / 2 + height / 2, height + size);

            }


            return triangle;
        }

        private double DegreeToRadian(double angleDegre)
        {
            return Math.PI * angleDegre / 180.0;
        }

        private double RadToDegree(double angleRad)
        {
            return 180 * angleRad / Math.PI;
        }

        private Vec2 MovePoint(double latitude, double longitude, double distanceInMetres, double bearing)
        {
            double brngRad = DegreeToRadian(bearing);
            double latRad  = DegreeToRadian(latitude);
            double lonRad  = DegreeToRadian(longitude);
            double earthRadiusInMetres = 6371000.0;
            double distFrac = distanceInMetres / earthRadiusInMetres;

            double latitudeResult = Math.Asin(Math.Sin(latRad) * Math.Cos(distFrac) + Math.Cos(latRad) * Math.Sin(distFrac) * Math.Cos(brngRad));
            double a = Math.Atan2(Math.Sin(brngRad) * Math.Sin(distFrac) * Math.Cos(latRad), Math.Cos(distFrac) - Math.Sin(latRad) * Math.Sin(latitudeResult));
            double longitudeResult = (lonRad + a + 3 * Math.PI) % (2 * Math.PI) - Math.PI;

            return new Vec2(RadToDegree(latitudeResult), RadToDegree(longitudeResult));
        }

        private double CalculateBearing(Vec2 A, Vec2 B)
        {
            double λ1 = A.Y;
            double φ1 = A.X;

            double λ2 = B.Y;
            double φ2 = B.X;

            double y = Math.Sin(λ2 - λ1) * Math.Cos(φ2);
            double x = Math.Cos(φ1) * Math.Sin(φ2) -
                    Math.Sin(φ1) * Math.Cos(φ2) * Math.Cos(λ2 - λ1);

            double brng = Math.Atan2(y, x);

            return RadToDegree(brng);
        }

        private double CalculateBearing(Position A, Position B)
        {
            double λ1 = A.Longitude;
            double φ1 = A.Latitude;

            double λ2 = B.Longitude;
            double φ2 = B.Latitude;

            double y = Math.Sin(λ2 - λ1) * Math.Cos(φ2);
            double x = Math.Cos(φ1) * Math.Sin(φ2) -
                    Math.Sin(φ1) * Math.Cos(φ2) * Math.Cos(λ2 - λ1);

            double brng = Math.Atan2(y, x);
            brng = RadToDegree(brng);

            if (brng < 0)
                return 360 + brng;
            else
                return brng;

        }



        public void RefreshPosition(object sender, EventArgs e)
        {
            MapInfoSingleton.Instance().SetPosition(GlobalSingleton.Instance().MaPosition);
            MapInfoSingleton mapInfo = MapInfoSingleton.Instance();

            Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
            {
                Refresh(map);
            });
           
        }


        static void Refresh(Object state)
        {
            MapInfoSingleton mapInfo = MapInfoSingleton.Instance();

            GoogleMap carte = (GoogleMap)state;
            if (carte == null)
                return;

            try
            {
                if (carte.Projection == null && carte.Projection.VisibleRegion == null && carte.Projection.VisibleRegion.LatLngBounds == null)
                    return;
            }catch(Exception e)
            {
                Console.WriteLine("ERREUR   mapRenderer : " + e);
                return;
            }

            LatLngBounds bounds = carte.Projection.VisibleRegion.LatLngBounds;


            if (mapInfo == null)
                return;

            if (mapInfo.Map == null)
                return;

            if (mapInfo.Map.VisibleRegion == null)
                return;

            mapInfo.MapSpan = mapInfo.Map.VisibleRegion;


            //copie les valeurs
            List<CustomPin> cPin = new List<CustomPin>(mapInfo.Map.CustomPins);

            if (mapInfo.Map.Pins != null)
                mapInfo.Map.Pins.Clear();
            if (mapInfo.Map.CustomPins != null)
                mapInfo.Map.CustomPins.Clear();

            foreach (var pin in cPin)
            {
                if (pin.TypePin != Genre.OWN && pin.TypePin != Genre.PAS_VISIBLE_REACHPOINT && pin.TypePin != Genre.VISIBLE_REACHPOINT)
                {

                    double dst = DistanceKm(mapInfo.GetPosition(), pin.Position);
                    //Console.WriteLine(pin.Position.Longitude+"  "+ pin.Position.Latitude+"  bounds = "+ bounds.ToString());
                    if (!EstEnCollision(pin.Position, bounds)) //hors carte =1
                    {
                        pin.TypePin = Genre.PAS_VISIBLE;
                        Console.WriteLine("PAS_VISIBLE  = " + dst);
                    }
                    else
                    {
                        pin.TypePin = Genre.VISIBLE;
                        Console.WriteLine("VISIBLE  = " + dst);
                    }
                }
                 else if(pin.TypePin == Genre.OWN)
                {
                    Console.WriteLine("OWN");
                    pin.Position = mapInfo.GetPosition();
                }
                


                if (mapInfo != null && pin != null)
                {
                    mapInfo.Map.CustomPins.Add(pin);
                    mapInfo.Map.Pins.Add(pin);
                }
            }
        }

        private static double DistanceKm(Position pos1, Position pos2)
        {
            float[] results = new float[3];
            Android.Locations.Location.DistanceBetween(pos1.Latitude, pos1.Longitude, pos2.Latitude, pos2.Longitude, results);

            return results[0]/1000;

        }

        private static bool EstEnCollision(Position pos, LatLngBounds viewPortLatLng)
        {
            double x1 = viewPortLatLng.Southwest.Longitude;
            double y1 = viewPortLatLng.Southwest.Latitude;
            
            double x2 = viewPortLatLng.Northeast.Longitude;
            double y2 = viewPortLatLng.Northeast.Latitude;

            return ((pos.Longitude >= x1 && pos.Longitude <= x2) && (pos.Latitude >= y1 && pos.Latitude <= y2));
        }
    }
}
