using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SkolniVylety
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DetailPage : ContentPage
    {
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            if (typ == typeof(Zajezd))
            {
                Zajezd zajezd = objekt as Zajezd;
                List<Trida> trida = await DBUtils.DB.Table<Trida>().Where(x => x.ID == zajezd.Trida).ToListAsync();
                List<Vylet> vylet = await DBUtils.DB.Table<Vylet>().Where(x => x.ID == zajezd.Vylet).ToListAsync();
                lReference.IsVisible = true;
                lReference.Children.Add(new Label() { Text = "Třída:" });
                lReference.Children.Add(new Label() { Text = trida[0].Nazev, Margin = new Thickness(0, 2, 0, 10) });
                lReference.Children.Add(new Label() { Text = "Výlet:" });
                lReference.Children.Add(new Label() { Text = vylet[0].Nazev, Margin = new Thickness(0, 2, 0, 10) });
            }
            dForm.SetData(objekt);
            lReference.Children.Clear();
        }
        public DetailPage(IData obj, Type Trida)
        {
            InitializeComponent();
            Title = obj.ToString();
            if (Trida == typeof(Trida) || Trida == typeof(Vylet))
                sLayout.IsVisible = true;
            if (Trida == typeof(Vylet))
            {
                bNovaCinnost.IsVisible = true;
                bProgram.IsVisible = true;
            }
            else if (Trida == typeof(Trida))
            {
                bNovyZak.IsVisible = true;
                bZaci.IsVisible = true;
                bVylety.IsVisible = true;
            } else if (Trida == typeof(Zajezd))
            {
                bGPS.IsVisible = true;
            }
        }

        public IData objekt { get; set; }

        public int id { get; set; }

        public Type typ { get; set; }

        public Type Target { get; set; }

        private async void BZaci_Clicked(object sender, EventArgs e)
        {
            lNadpis.Text = "Žáci:";
            lZaci.ItemsSource = await DBUtils.DB.Table<Student>().Where(x => x.Trida == id).OrderBy(x => x.Prijmeni).ToListAsync();
            Target = typeof(Student);
        }

        private async void BVylety_Clicked(object sender, EventArgs e)
        {
            lNadpis.Text = "Naplánované výlety:";
            lZaci.ItemsSource = await DBUtils.DB.Table<Zajezd>().Where(x => x.Trida == id).ToListAsync();
            Target = typeof(Zajezd);
        }

        private void LZaci_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            Navigation.PushAsync(new DetailPage(e.Item as IData, Target));
        }

        private async void BSmazat_Clicked(object sender, EventArgs e)
        {
            if (await DisplayAlert("Smazat", "Skutečně si přejete tento záznam smazat?", "Ano", "Ne"))
            {
                if (await DBUtils.Smazat(objekt))
                    await Navigation.PopAsync();
                else
                    await DisplayAlert("Smazat", "Záznam není možné vymazat, protože se na něj odkazují záznamy jiných tabulek", "OK");
            }
        }

        private void BNovyZak_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new EditPage(new Student() { Trida = id }, typeof(Student)));
        }

        private void BNovaCinnost_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new EditPage(new Polozka() { Vylet = id }, typeof(Polozka)));
        }

        private async void BProgram_Clicked(object sender, EventArgs e)
        {
            lNadpis.Text = "Program:";
            lZaci.ItemsSource = await DBUtils.DB.Table<Polozka>().Where(x => x.Vylet == id).ToListAsync();
            Target = typeof(Polozka);
        }

        private void bUpravit_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new EditPage(objekt, typ));
        }

        private void bZpet_Clicked(object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }

        private void bGPS_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new GPSPage(objekt.ToString()) { id = id });
        }
    }
}