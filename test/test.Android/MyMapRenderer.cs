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
using Android.Views;
using Android;
using Android.Util;


[assembly: ExportRenderer(typeof(CustomMap), typeof(test.droid.MyMapRenderer))]
namespace test.droid
{
    class MyMapRenderer : MapRenderer, GoogleMap.IInfoWindowAdapter
    {
        List<CustomPin> customPins;
        GoogleMap map = null;
        Bitmap bmp = null;
        Bitmap resizedBitmap = null;

        public MyMapRenderer(Context context) : base(context)
        {
            bmp = BitmapFactory.DecodeResource(Resources, test.Droid.Resource.Drawable.location_icon);
            resizedBitmap = Bitmap.CreateScaledBitmap(bmp, 128, 128, false);
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
        }

        protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            Refresh(map);
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

                    float rayon =(float) DistanceKm(posNear, posFar); //en km
                    Console.WriteLine("r = " + rayon);
                    BitmapDescriptor bm = GetTextMarker(MyPin.Label, MyPin.Dist, Android.Graphics.Color.Red, 45f);
                    //MyPin.PositionAffichage = CalculPosition(MapInfoSingleton.Instance().GetPosition(), pin.Position, rayon);




                    Position centerCameraMap = new Position(map.Projection.VisibleRegion.LatLngBounds.Center.Latitude, map.Projection.VisibleRegion.LatLngBounds.Center.Longitude);
                        //MyPin.PositionAffichage = CalculPosition(pin.Position, centerCameraMap, map.Projection.VisibleRegion.LatLngBounds);
                    MyPin.PositionAffichage = CalculNouvellePosition(pin.Position, centerCameraMap, map.Projection.VisibleRegion.LatLngBounds);

                    Console.WriteLine(map.Projection.VisibleRegion.LatLngBounds.ToString());


                    Console.WriteLine("("+MyPin.Position.Latitude+" , " +pin.Position.Longitude+")   "+MyPin.PositionAffichage.Latitude +"  "+ MyPin.PositionAffichage.Longitude);
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



        private Position CalculNouvellePosition(Position A, Position B, LatLngBounds zone)
        {
            //creation des points
            Vec2 E = new Vec2(zone.Southwest.Latitude + 0.001, zone.Northeast.Longitude - 0.001);
            Vec2 F = new Vec2(zone.Northeast.Latitude - 0.001, zone.Northeast.Longitude - 0.001);
            Vec2 G = new Vec2(zone.Northeast.Latitude - 0.001, zone.Southwest.Longitude + 0.001);
            Vec2 H = new Vec2(zone.Southwest.Latitude + 0.001, zone.Southwest.Longitude + 0.001);
            //formation des vecteurs
            Vec2 AB = new Vec2(A.Longitude - B.Longitude, A.Latitude - B.Latitude);
            Vec2 BA = new Vec2(A.Latitude - B.Latitude, A.Longitude - B.Longitude);
            Vec2 EF = new Vec2(F.X - E.X, F.Y - E.Y);
            Vec2 FG = new Vec2(G.X - F.X, G.Y - F.Y);
            Vec2 GH = new Vec2(H.X - G.X, H.Y - G.Y);
            Vec2 HE = new Vec2(E.X - H.X, E.Y - H.Y);


            Vec2 Seg1 = new Vec2();
            Vec2 Seg2 = new Vec2();
            double dst1 = 0;
            double dst2 = 0;
            //choix du quart de l'ecran ( on doit trouvé 2 demi segment a partir du centre )
            if (AB.X >= 0 && AB.Y >= 0)//++
            {
                //chercher point d'intersection avec les segments precedents
                Seg1 = IntesectionPoints(B, A, E, F);
                Seg2 = IntesectionPoints(B, A, F, G);
            }
            else if (AB.X <= 0 && AB.Y >= 0)//-+
            {
                Seg1 = IntesectionPoints(B, A, E, F);
                Seg2 = IntesectionPoints(B, A, E, H);
            }
            else if (AB.X <= 0 && AB.Y <= 0)//--
            {
                Seg1 = IntesectionPoints(B, A, E, H);
                Seg2 = IntesectionPoints(B, A, H, G);
            }
            else if (AB.X >= 0 && AB.Y <= 0)//+-
            {
                Seg1 = IntesectionPoints(B, A, H, G);
                Seg2 = IntesectionPoints(B, A, G, F);
            }

            //chercher la distance entre le centre et les points trouvés
            dst1 = Distance(BA, Seg1);
            dst2 = Distance(BA, Seg2);

            //prendre le plus proche et retourner la nouvelle position calculée
            if (dst1 <= dst2)
                return new Position(Seg1.X,Seg1.Y);
            else
                return new Position(Seg2.X, Seg2.Y);
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

            return new Vec2(C.X + m * CD.X, C.Y + m * CD.Y);
        }

        private Vec2 IntesectionPoints(Position A, Position B, Vec2 C, Vec2 D)
        {
            Vec2 AB = new Vec2(B.Latitude - A.Latitude, B.Longitude - A.Longitude);
            Vec2 CD = new Vec2(D.X - C.X, D.Y - C.Y);

            double m = 0;
            double diviseur = (AB.X * CD.Y) - (AB.Y * CD.X);

            if (diviseur != 0)
            {
                m = (AB.X * A.Longitude
                     - AB.X * C.Y
                     - AB.Y * A.Latitude
                     + AB.Y * C.X
                    ) / diviseur;
            }

            return new Vec2(C.X + m * CD.X, C.Y + m * CD.Y);
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

            //parametre
            paint.TextSize = 64;
            int offset = 30;
            paint.TextAlign = Paint.Align.Left;

            Bitmap bmp = BitmapFactory.DecodeResource(Resources, test.Droid.Resource.Drawable.icon);
            Bitmap img = Bitmap.CreateScaledBitmap(bmp, ((int)paint.TextSize / 2 + offset) * 2, ((int)paint.TextSize / 2 + offset) * 2, true);


            //definiton de la taille du canvas qui va contenir les informations

            //taille total du canvas  texte + img + fleche + offset
            int widthTotal = (int)paint.MeasureText(pseudo) + img.Width + offset * 4 + ((int)paint.TextSize / 2 + offset);
            int heightTotal = img.Width + offset * 4;
            //taille texte + image
            int width = (int)paint.MeasureText(pseudo) + img.Width + offset * 2;
            int height = img.Width + offset * 2;


            // Creation de l'image que l'on vas cosntuiore pas a pas
            Bitmap image = Bitmap.CreateBitmap(widthTotal, heightTotal, Bitmap.Config.Argb8888);
            Canvas canvas = new Canvas(image);


            //zone de dessin : que l'on colorie pour voir les delimitations
            //  canvas.DrawColor(Android.Graphics.Color.White);

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


     

        static void Refresh(Object state)
        {
            MapInfoSingleton mapInfo = MapInfoSingleton.Instance();

            GoogleMap carte = (GoogleMap)state;
            if (carte == null)
                return;

            LatLngBounds bounds = carte.Projection.VisibleRegion.LatLngBounds;

           // Console.Write("bounds = "+bounds.ToString() );

            if (mapInfo == null)
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
                if (pin.TypePin != Genre.OWN)
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
                else { Console.WriteLine("OWN"); }


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
