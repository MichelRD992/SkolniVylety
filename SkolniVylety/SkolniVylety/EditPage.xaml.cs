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
    public partial class EditPage : ContentPage
    {
        public EditPage(IData obj, Type Trida)
        {
            InitializeComponent();
            dForm.SetData(obj);
            Title = obj.ID == 0 ? $"Nový záznam do tabulky {DBUtils.Popisek(Trida)}" : obj.ToString();
        }
        private async void BUlozit_Clicked(object sender, EventArgs e)
        {
            var obj = dForm.GetData();
            if (obj.ID == 0)
                await DBUtils.DB.InsertAsync(obj);
            else
                await DBUtils.DB.UpdateAsync(obj);
            await Navigation.PopAsync();
        }

        private void BZrusit_Clicked(object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }
    }
}