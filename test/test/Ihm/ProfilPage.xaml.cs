using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace test 
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ProfilPage : ContentPage
    {
        Button bouttonSelectionne = null;

        public ProfilPage()
        {
            InitializeComponent();
            Refresh();
        }


        private void OnColorClicked(object sender, EventArgs e) 
        {
            if (bouttonSelectionne != null)
                bouttonSelectionne.BorderColor = bouttonSelectionne.BackgroundColor;

            bouttonSelectionne = (Button)sender;
            GlobalSingleton.Instance().Perso.Couleur = bouttonSelectionne.BackgroundColor;
            bouttonSelectionne.BorderColor = Color.WhiteSmoke;
            Refresh();
        }

        private void Refresh()
        {
            Box.Color = GlobalSingleton.Instance().Perso.Couleur;
            Box1.Color = GlobalSingleton.Instance().Perso.Couleur;
        }
    }
}