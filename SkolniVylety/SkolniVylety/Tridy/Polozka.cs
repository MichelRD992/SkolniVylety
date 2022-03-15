using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace SkolniVylety
{
    [Table("Polozky"), Popisek("Položky")]
    public class Polozka : IData
    {
        [PrimaryKey, AutoIncrement, SkrytVeFormulari]
        public int ID { get; set; }

        [Popisek("Činnost")]
        public string Cinnost { get; set; }

        [Popisek("Předpokládaný čas zahájení")]
        public string Zacatek { get; set; }

        [Popisek("Předpokládaný čas ukončení")]
        public string Konec { get; set; }

        [Reference(typeof(Vylet)), SkrytVeFormulari]
        public int Vylet { get; set; }

        [Popisek("Popis a poznámky")]
        public string Popis { get; set; }

        public override string ToString() => $"{Cinnost} ({Zacatek} - {Konec})";
    }
}
