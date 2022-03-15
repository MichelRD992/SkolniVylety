using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace SkolniVylety
{
    [Table("Tridy"), Popisek("Třídy")]
    public class Trida : IData
    {
        [PrimaryKey, AutoIncrement, SkrytVeFormulari]
        public int ID { get; set; }

        [Popisek("Název")]
        public string Nazev { get; set; }

        [Popisek("Třídní učitel")]
        public string Ucitel { get; set; }

        [Enum, Popisek("Ročník")]
        public string Rocnik { get; set; }

        public override string ToString() => $"{Nazev} ({Ucitel})";
    }
}
