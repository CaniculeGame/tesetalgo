using System;
using System.Net;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace test
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class MakePage : ContentPage
	{
        public enum ChoixCarteImage
        {
            CARTE=0, 
            PLAN=1, 
            INVALIDE=-1
        }


        ChoixCarteImage choix = ChoixCarteImage.INVALIDE;
        IPAddress[] address;

        public MakePage ()
		{
			InitializeComponent ();
            Refresh();

            address = Dns.GetHostAddresses(Dns.GetHostName());
            IpServer.Text = address[0].ToString();

        }


        private void Refresh()
        {
            ButtonPlan.BackgroundColor = GlobalSingleton.Instance().Perso.Couleur;
            ButtonCarte.BackgroundColor = GlobalSingleton.Instance().Perso.Couleur;
            BoxMakePage0.Color = GlobalSingleton.Instance().Perso.Couleur;
            BoxMakePage1.Color = GlobalSingleton.Instance().Perso.Couleur;

            if (choix == ChoixCarteImage.CARTE)
            {
                ButtonPlan.BorderColor = GlobalSingleton.Instance().Perso.Couleur;
                ButtonCarte.BorderColor = Color.WhiteSmoke;
            }
            else if (choix == ChoixCarteImage.PLAN)
            {
                ButtonPlan.BorderColor = Color.WhiteSmoke;
                ButtonCarte.BorderColor = GlobalSingleton.Instance().Perso.Couleur;
            }
            else if (choix == ChoixCarteImage.INVALIDE)
            {
                ButtonPlan.BorderColor = GlobalSingleton.Instance().Perso.Couleur;
                ButtonCarte.BorderColor = GlobalSingleton.Instance().Perso.Couleur;
            }
        }

        private void OnButtonImageClicked(object sender, EventArgs e)
        {
            ButtonPlan.BorderColor = Color.WhiteSmoke;
            ButtonCarte.BorderColor = GlobalSingleton.Instance().Perso.Couleur;
            choix = ChoixCarteImage.PLAN;
        }

        private void OnButtonCarteClicked(object sender, EventArgs e)
        {
            ButtonCarte.BorderColor = Color.WhiteSmoke;
            ButtonPlan.BorderColor = GlobalSingleton.Instance().Perso.Couleur;
            choix = ChoixCarteImage.CARTE;
        }

        protected override void OnAppearing()
        {
            Refresh();
        }
    }
}