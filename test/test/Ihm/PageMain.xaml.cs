using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace test
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class PageMain : ContentPage
	{
   
        public PageMain ()
		{
			InitializeComponent ();

            Quitter.Clicked += OnButtonQuitClicked;
            Rejoindre.Clicked += OnRejoindreClicked;
            Credit.Clicked += OnCreditClicked;
            Option.Clicked += OnOptionClicked;
            Pub.Clicked += OnPubClicked;
            ChangeProfil.Clicked += OnProfilClicked;
            Creer.Clicked += OnCreateClicked;

        }

        private void OnCreateClicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new MakePage());
        }

        private void OnProfilClicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new ProfilPage());
        }

        private void OnPubClicked(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void OnOptionClicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new OptionPage());
        }

        private void OnCreditClicked(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void OnRejoindreClicked(object sender, EventArgs e)
        {
           Navigation.PushAsync(new MainPageTest()); ;
        }


        protected override void OnAppearing()
        {
            ChangeProfil.BackgroundColor = GlobalSingleton.Instance().Perso.Couleur;
            Rejoindre.BackgroundColor = GlobalSingleton.Instance().Perso.Couleur;
            Creer.BackgroundColor = GlobalSingleton.Instance().Perso.Couleur;
            Pub.BackgroundColor = GlobalSingleton.Instance().Perso.Couleur;
            Credit.BackgroundColor = GlobalSingleton.Instance().Perso.Couleur;
            Option.BackgroundColor = GlobalSingleton.Instance().Perso.Couleur;
            Quitter.BackgroundColor = GlobalSingleton.Instance().Perso.Couleur;

        }


        private void OnButtonQuitClicked(object sender, EventArgs e)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {

#if __ANDROID__
                    var activity = (Android.App.Activity)Forms.Context;
                    activity.FinishAffinity();
#endif               
            });
        }
    }
}