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
        public async void SetData(IData record)
        {
            var typ = record.GetType();
            foreach (var prop in typ.GetProperties())
            {
                if (prop.IsDefined(typeof(SkrytVeFormulariAttribute)))
                    continue;
                View editor = null;
                if (prop.IsDefined(typeof(EnumAttribute)))
                {
                    editor = new Picker()
                    {
                        ItemsSource = "1.,2.,3.,4.,5.,6.,7.,8.,9.".Split(',')
                    };
                    editor.SetBinding(Picker.SelectedItemProperty, prop.Name);
                }
                else if (prop.PropertyType == typeof(int?) && prop.IsDefined(typeof(ReferenceAttribute)))
                {
                    editor = new LookupPicker()
                    {
                        ItemsSource = await DBUtils.DataTabulky(
                            prop.GetCustomAttribute<ReferenceAttribute>().Table) as IList
                    };
                    editor.SetBinding(LookupPicker.IDProperty, prop.Name);
                }
                else if (prop.PropertyType == typeof(string))
                {
                    editor = new Entry();
                    editor.SetBinding(Entry.TextProperty, prop.Name);
                }
                else if (prop.PropertyType == typeof(int))
                {
                    editor = new Entry() { Keyboard = Keyboard.Numeric };
                    editor.SetBinding(Entry.TextProperty, prop.Name);
                }
                else if (prop.PropertyType == typeof(DateTime))
                {
                    editor = new DatePicker() { HorizontalOptions = LayoutOptions.Start };
                    editor.SetBinding(DatePicker.DateProperty, prop.Name);
                }
                else if (prop.PropertyType == typeof(bool))
                {
                    editor = new Switch();
                    editor.SetBinding(Switch.IsToggledProperty, prop.Name);
                }
                if (editor != null)
                {
                    Children.Add(new Label() { Text = DBUtils.Popisek(prop) });
                    editor.Margin = new Thickness(0, 2, 0, 10);
                    Children.Add(editor);
                }
            }
            BindingContext = record;
        }
        public IData GetData() => BindingContext as IData;
    }
}
