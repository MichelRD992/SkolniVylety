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
        public DetailPage(IData obj, Type Trida)
        {
            InitializeComponent();
            Title = obj.ToString();
            SetDataAsync(obj);
            lObsah = Form;
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
            }
            id = obj.ID;
            typ = Trida;
            objekt = obj;
        }

        public IData objekt { get; set; }

        public int id { get; set; }

        public Type typ { get; set; }

        public Type Target { get; set; }

        public StackLayout Form { get; set; }

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

        public async void SetDataAsync(IData record)
        {
            var typ = record.GetType();
            bool reference = false;
            foreach (var prop in typ.GetProperties())
            {
                var val = prop.GetValue(record);
                if (prop.IsDefined(typeof(SkrytVeFormulariAttribute)))
                    continue;
                View editor = null;
                if (prop.PropertyType == typeof(string) || prop.PropertyType == typeof(int))
                    editor = new Label() { Text = (string)val };
                else if (prop.PropertyType == typeof(int?) && prop.IsDefined(typeof(ReferenceAttribute)))
                {
                    string nazev;
                    if (!reference)
                    {
                        List<Trida> trida = await DBUtils.DB.Table<Trida>().Where(x => x.ID == (int)val).ToListAsync();
                        nazev = trida[0].Nazev;
                        reference = true;
                    }
                    else
                    {
                        List<Vylet> vylet = await DBUtils.DB.Table<Vylet>().Where(x => x.ID == (int)val).ToListAsync();
                        nazev = vylet[0].Nazev;
                    }
                    editor = new Label() { Text = nazev };
                }
                else if (prop.PropertyType == typeof(DateTime))
                    editor = new Label() { Text = DateTime.Parse((string)val).ToString("d.m.yyyy") };
                if (editor != null)
                {
                    Form.Children.Add(new Label() { Text = DBUtils.Popisek(prop) + ":" });
                    editor.Margin = new Thickness(0, 2, 0, 10);
                    Form.Children.Add(editor);
                }
            }
        }
    }
}