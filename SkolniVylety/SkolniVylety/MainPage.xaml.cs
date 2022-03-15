using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace SkolniVylety
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private void BTrida_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new VypisPage() { Trida = typeof(Trida) });
        }

        private void BVylet_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new VypisPage() { Trida = typeof(Vylet) });
        }

        private void BZajezd_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new VypisPage() { Trida = typeof(Zajezd) });
        }
    }
}
