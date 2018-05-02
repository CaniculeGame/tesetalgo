using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace test 
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class OptionPage : ContentPage
	{
        bool checkBoxNotif = false;
        bool checkBoxSon = false;
        ImageSource on;
        ImageSource off;

        ImageSource pays0_actif;
        ImageSource pays0_inactif;
        ImageSource pays1_actif;
        ImageSource pays1_inactif;
        ImageSource pays2_actif;
        ImageSource pays2_inactif;

        public enum Pays { FR= 0, USA =1, ALL=2}
        Pays paysChoisie = Pays.FR;

        public OptionPage ()
		{
			InitializeComponent ();
            on = ImageSource.FromFile("on.png");
            off = ImageSource.FromFile("off.png");

            pays0_actif = ImageSource.FromFile("allemagne.png");
            pays0_inactif = ImageSource.FromFile("allemagne_off.png");

            pays1_actif = ImageSource.FromFile("france.png");
            pays1_inactif = ImageSource.FromFile("france_off.png");

            pays2_actif = ImageSource.FromFile("usa.png");
            pays2_inactif = ImageSource.FromFile("usa_off.png");

            Refresh();
        }

        private void OnPaysALLClicked(object sender, EventArgs e)
        {
            paysChoisie = Pays.ALL;
            PaysButton0.Source = pays0_actif;
            PaysButton1.Source = pays1_inactif;
            PaysButton2.Source = pays2_inactif;
        }

        private void OnPaysFRClicked(object sender, EventArgs e)
        {
            paysChoisie = Pays.FR;
            PaysButton0.Source = pays0_inactif;
            PaysButton1.Source = pays1_actif;
            PaysButton2.Source = pays2_inactif;
        }

        private void OnPaysUSAClicked(object sender, EventArgs e)
        {
            paysChoisie = Pays.USA;
            PaysButton0.Source = pays0_inactif;
            PaysButton1.Source = pays1_inactif;
            PaysButton2.Source = pays2_actif;
        }

        private void OnCheckBoxSonClicked(object sender, EventArgs e)
        {
            checkBoxSon = !checkBoxSon;
            if (checkBoxSon)
                CheckBoxSonButton.Source = on;
            else
                CheckBoxSonButton.Source = off;
        }

        private void OnCheckBoxNotifClicked(object sender, EventArgs e)
        {
            checkBoxNotif = !checkBoxNotif;
            if(checkBoxNotif)
                CheckBoxNotifButton.Source = on;
            else
                CheckBoxNotifButton.Source = off;
        }

        protected override void OnAppearing()
        {
            Refresh();
        }

        private void Refresh()
        {
            Box0.Color = GlobalSingleton.Instance().Perso.Couleur;
            Box1.Color = GlobalSingleton.Instance().Perso.Couleur;

            switch (paysChoisie)
            {
                case Pays.ALL:
                    PaysButton0.Source = pays0_actif;
                    PaysButton1.Source = pays1_inactif;
                    PaysButton2.Source = pays2_inactif;
                    break;

                case Pays.FR:
                    PaysButton0.Source = pays0_inactif;
                    PaysButton1.Source = pays1_actif;
                    PaysButton2.Source = pays2_inactif;
                    break;

                case Pays.USA:
                    PaysButton0.Source = pays0_inactif;
                    PaysButton1.Source = pays1_inactif;
                    PaysButton2.Source = pays2_actif;
                    break;
            }
        }
    }
}