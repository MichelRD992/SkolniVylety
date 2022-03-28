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
    public partial class GPSPage : ContentPage
    {
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
                    float rychlost;
                    float rychlost2;
                    for(int i = 1; i < data.Count; i++)
                    {

                    }
                }
            }
        }

        public void PridejZaznam(Zaznam zaznam, float? rychlost, float? zmena)
        {
            StackLayout sl = new StackLayout() { HorizontalOptions = LayoutOptions.FillAndExpand, Orientation = StackOrientation.Vertical };
            sl.Children.Add(new Label() { Text = zaznam.Latitude.ToString("N3") + "; " + zaznam.Longitude.ToString("N3") });
            sl.Children.Add(new Label() { Text = zaznam.Cas.ToString("HH:mm:ss"), HorizontalTextAlignment = TextAlignment.End });
            StackLayout sll = new StackLayout() { HorizontalOptions = LayoutOptions.FillAndExpand, Orientation = StackOrientation.Vertical };
            if(zmena != null)
            {
                string sipka = (zmena < 0) ? "\u2198" : "\u2197";
                Color pozadi = (zmena < 0) ? Color.LightCoral : Color.LightGreen;
                Color pismo = (zmena < 0) ? Color.Red : Color.Green;
                sl.BackgroundColor = pozadi;
                sll.BackgroundColor = pozadi;
                sll.Children.Add(new Label() { Text = sipka, TextColor = pismo });
                sll.Children.Add(new Label() { Text = zmena.ToString() + " %", TextColor = pismo });
            } else
            {
                sl.BackgroundColor = Color.LightGray;
                sll.BackgroundColor = Color.LightGray;
            }
            sSeznam.Children.Add(sl);
            if (rychlost != null)
            {
                sll.Children.Add(new Label() { Text = rychlost.ToString() });
                sSeznam.Children.Add(sll);
            }
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
    }
}