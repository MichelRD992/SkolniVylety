using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
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
        protected override void OnAppearing()
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
            if (data.Count == 0)
                sSeznam.Children.Add(new Label() { Text = "Prozatím tu nejsou žádné záznamy", LineBreakMode = LineBreakMode.WordWrap });
            else
            {
                PridejZaznam(data[0], null, null, null);
                if (data.Count > 1)
                {
                    double rychlost = 1;
                    double rychlost2;
                    for (int i = 1; i < data.Count; i++)
                    {
                        double vzdalenost = Location.CalculateDistance(new Location(data[i - 1].Latitude, data[i - 1].Longitude), new Location(data[i].Latitude, data[i].Longitude), DistanceUnits.Kilometers);
                        rychlost2 = vzdalenost / data[i].Cas.Subtract(data[i - 1].Cas).TotalHours;
                        if (i == 1)
                            PridejZaznam(data[i], rychlost2, vzdalenost, null);
                        else
                        {
                            double zmena;
                            if (rychlost == 0 && rychlost2 == 0)
                                zmena = 0;
                            else
                                zmena = Math.Sign(rychlost - rychlost2)*(((100 / Math.Max(rychlost, rychlost2))* Math.Min(rychlost, rychlost2)) - 100);
                            PridejZaznam(data[i], rychlost2, vzdalenost, zmena);
                        }
                        rychlost = rychlost2;
                    }
                }
            }
        }

        public void PridejZaznam(Zaznam zaznam, double? rychlost, double? vzdalenost, double? zmena)
        {
            Grid gg = new Grid
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = GridLength.Auto },
                    new ColumnDefinition { Width = GridLength.Star },
                    new ColumnDefinition { Width = GridLength.Star }
                }
            };
            gg.Children.Add(new Label() { Text = zaznam.Latitude.ToString("N3") + "; " + zaznam.Longitude.ToString("N3"), FontAttributes = FontAttributes.Bold }, 0, 0);
            gg.Children.Add(new Label() { Text = zaznam.Cas.ToString("HH:mm:ss"), HorizontalTextAlignment = TextAlignment.End }, 2, 0);
            if (rychlost != null)
            {
                gg.Children.Add(new Label() { Text = ((float)rychlost).ToString("N3") + " km/h", HorizontalTextAlignment = TextAlignment.End }, 2, 1);
                gg.Children.Add(new Label() { Text = "+" + ((float)vzdalenost).ToString("N3") + " km", HorizontalTextAlignment = TextAlignment.Center }, 1, 0);
            }
            if (zmena != null)
            {
                string sipka = (zmena < 0) ? "\u2198" : "\u2197";
                Color pismo = (zmena < 0) ? Color.Red : Color.Green;
                gg.BackgroundColor = (zmena < 0) ? Color.LightCoral : Color.LightGreen;
                gg.Children.Add(new Label() { Text = sipka, TextColor = pismo, FontAttributes = FontAttributes.Bold, VerticalTextAlignment = TextAlignment.Start }, 0, 1);
                gg.Children.Add(new Label() { Text = ((float)zmena).ToString("N2") + " %", TextColor = pismo, HorizontalTextAlignment = TextAlignment.Center }, 1, 1);
            }
            else
                gg.BackgroundColor = Color.LightGray;
            sSeznam.Children.Add(gg);
        }

        private async void bPridat_Clicked(object sender, EventArgs e)
        {
            Position position = null;
            try
            {
                var locator = CrossGeolocator.Current;
                locator.DesiredAccuracy = 5;

                if (!locator.IsGeolocationAvailable)
                    sSeznam.Children.Add(new Label() { Text = "Zjišťování polohy není k dispozici, prosím zkontrolujte, zda jím váš telefon disponuje, a pokud ano, povolte jej v nastavení aplikací", LineBreakMode = LineBreakMode.WordWrap });
                else if (!locator.IsGeolocationEnabled)
                    sSeznam.Children.Add(new Label() { Text = "Zjišťování polohy není povoleno, prosím zapněte jej", LineBreakMode = LineBreakMode.WordWrap });
                else
                {
                    position = await locator.GetPositionAsync(TimeSpan.FromSeconds(5), null, true);
                    Zaznam zaznam = new Zaznam();
                    zaznam.Latitude = position.Latitude;
                    zaznam.Longitude = position.Longitude;
                    zaznam.Cas = DateTime.Now;
                    zaznam.Zajezd = id;
                    await DBUtils.DB.InsertAsync(zaznam);
                    Statistika();
                }
            }
            catch (Exception ex)
            {
                sSeznam.Children.Add(new Label() { Text = "Nastala chyba: " + ex, LineBreakMode = LineBreakMode.WordWrap });
            }
        }

        private async void bSmazat_Clicked(object sender, EventArgs e)
        {
            await DBUtils.DB.QueryAsync<Zaznam>("delete from Zaznamy where Zajezd = ?", id);
            Statistika();
        }
    }
}