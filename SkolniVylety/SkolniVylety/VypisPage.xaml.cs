using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SkolniVylety
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class VypisPage : ContentPage
    {
        public Type Trida { get; set; }

        public VypisPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            Title = DBUtils.Popisek(Trida);
            lSeznam.ItemsSource = await DBUtils.DataTabulky(Trida);
        }

        private void BNovy_Clicked(object sender, EventArgs e)
        {
            var obj = Activator.CreateInstance(Trida) as IData;
            Navigation.PushAsync(new EditPage(obj, Trida));
        }

        private async void LSeznam_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            IData obj = e.Item as IData;
            await Navigation.PushAsync(new DetailPage(e.Item as IData, Trida) { id = obj.ID, typ = obj.GetType(), objekt = obj });
        }
    }
}