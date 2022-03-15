using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace SkolniVylety
{
    [Table("Zajezdy"), Popisek("Naplánované výlety")]
    public class Zajezd : IData
    {
        [PrimaryKey, AutoIncrement, SkrytVeFormulari]
        public int ID { get; set; }

        [Reference(typeof(Trida)), Popisek("Třída")]
        public int? Trida { get; set; }

        [Reference(typeof(Vylet)), Popisek("Výlet")]
        public int? Vylet { get; set; }

        [Popisek("Datum konání")]
        public DateTime Datum { get; set; }

        [Popisek("Učitelský dozor")]
        public string Ucitel { get; set; }

        public override string ToString() => $"{Datum:dd.MM.yyyy}";
    }
}
