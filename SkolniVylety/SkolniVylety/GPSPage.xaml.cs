using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SkolniVylety
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GPSPage : ContentPage
    {
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            Statistika();
        }
        public GPSPage(string zajezd)
        {
            InitializeComponent();
            Title = "Sledování pohybu - " + zajezd;
        }
        public int id { get; set; }

        public async void Statistika()
        {
            sSeznam.Children.Clear();
            List<Zaznam> data = await DBUtils.DB.Table<Zaznam>().Where(x => x.Zajezd == id).ToListAsync();
            if(data.Count == 0)
                sSeznam.Children.Add(new Label() { Text = "Prozatím tu nejsou žádné záznamy", LineBreakMode = LineBreakMode.WordWrap });
            else
            {
                PridejZaznam(data[0], null, null);
                if(data.Count > 1)
                {
                    double rychlost = 1;
                    double rychlost2;
                    for(int i = 1; i < data.Count; i++)
                    {
                        rychlost2 = Location.CalculateDistance(new Location(data[i - 1].Latitude, data[i - 1].Longitude), new Location(data[i].Latitude, data[i].Longitude), DistanceUnits.Kilometers)/ data[i].Cas.Subtract(data[i - 1].Cas).TotalHours;
                        if (i == 1)
                            PridejZaznam(data[i], (float?)rychlost2, null);
                        else
                            PridejZaznam(data[i], (float?)rychlost2, (float?)((100 / rychlost) * rychlost2) - 100);
                        rychlost = rychlost2;
                    }
                }
            }
        }

        public void PridejZaznam(Zaznam zaznam, float? rychlost, float? zmena)
        {
            Grid gg = new Grid
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = GridLength.Star },
                    new ColumnDefinition { Width = GridLength.Star },
                    new ColumnDefinition { Width = GridLength.Star }
                }
            };
            gg.Children.Add(new Label() { Text = zaznam.Latitude.ToString("N3") + "; " + zaznam.Longitude.ToString("N3"), FontAttributes = FontAttributes.Bold }, 0, 0);
            gg.Children.Add(new Label() { Text = zaznam.Cas.ToString("HH:mm:ss"), HorizontalTextAlignment = TextAlignment.End }, 2, 0);
            if (rychlost != null)
                gg.Children.Add(new Label() { Text = rychlost.ToString() + " km/h", HorizontalTextAlignment = TextAlignment.End }, 2, 1);
            if(zmena != null)
            {
                string sipka = (zmena < 0) ? "\u2198" : "\u2197";
                Color pismo = (zmena < 0) ? Color.Red : Color.Green;
                gg.BackgroundColor = (zmena < 0) ? Color.LightCoral : Color.LightGreen;
                gg.Children.Add(new Label() { Text = sipka, TextColor = pismo, FontAttributes = FontAttributes.Bold, VerticalTextAlignment = TextAlignment.Start }, 0, 1);
                gg.Children.Add(new Label() { Text = zmena.ToString() + " %", TextColor = pismo, HorizontalTextAlignment = TextAlignment.Center }, 1, 1);
            } else
                gg.BackgroundColor = Color.LightGray;
            sSeznam.Children.Add(gg);
        }

        private async void bPridat_Clicked(object sender, EventArgs e)
        {
            Zaznam zaznam = new Zaznam();
            zaznam.Latitude = random.Next(0, 10) + random.NextDouble();
            zaznam.Longitude = random.Next(0, 10) + random.NextDouble();
            zaznam.Cas = DateTime.Now;
            zaznam.Zajezd = id;
            await DBUtils.DB.InsertAsync(zaznam);
            Statistika();
        }

        Random random = new Random();

        private async void bSmazat_Clicked(object sender, EventArgs e)
        {
            await DBUtils.DB.QueryAsync<Zaznam>("delete from Zaznamy where Zajezd = ?", id);
            Statistika();
        }
    }
}