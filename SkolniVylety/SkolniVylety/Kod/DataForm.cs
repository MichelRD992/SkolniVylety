using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Xamarin.Forms;

namespace SkolniVylety
{
    public class DataForm : StackLayout
    {
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
                    Children.Add(new Label() { Text = DBUtils.Popisek(prop) + ":" });
                    editor.Margin = new Thickness(0, 2, 0, 10);
                    Children.Add(editor);
                }
            }
        }
        //public IData GetData() => BindingContext as IData;
    }
}
