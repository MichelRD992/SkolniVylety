using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Essentials;

namespace SkolniVylety
{
    [Table("Zaznamy"), Popisek("Záznamy")]
    public class Zaznam : IData
    {
        [PrimaryKey, AutoIncrement, SkrytVeFormulari]
        public int ID { get; set; }

        [Popisek("Zeměpisná šířka")]
        public double Latitude { get; set; }

        [Popisek("Zeměpisná délka")]
        public double Longitude { get; set; }

        [Popisek("Čas")]
        public DateTime Cas { get; set; }

        [Reference(typeof(Zajezd)), SkrytVeFormulari]
        public int Zajezd { get; set; }
    }
}
