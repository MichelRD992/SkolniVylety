using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Xamarin.Forms;

namespace SkolniVylety
{
    public class DetailForm : StackLayout
    {
        public void SetData(IData record)
        {
            rec = record;
            var typ = record.GetType();
            foreach (var prop in typ.GetProperties())
            {
                var val = prop.GetValue(record);
                if (prop.IsDefined(typeof(SkrytVeFormulariAttribute)))
                    continue;
                View editor = null;
                if (prop.PropertyType == typeof(int?) && prop.IsDefined(typeof(ReferenceAttribute)))
                    continue;
                else if (prop.PropertyType == typeof(string) || prop.PropertyType == typeof(int))
                    editor = new Label() { Text = (string)val };
                else if (prop.PropertyType == typeof(DateTime))
                    editor = new Label() { Text = DateTime.Parse(val.ToString()).ToString("dd.MM.yyyy") };
                if (editor != null)
                {
                    Children.Add(new Label() { Text = DBUtils.Popisek(prop) + ":" });
                    editor.Margin = new Thickness(0, 2, 0, 10);
                    Children.Add(editor);
                }
            }
        }
        private IData rec { get; set; }
        public IData GetData() => rec;
    }
}
