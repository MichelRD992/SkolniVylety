using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace SkolniVylety
{
    [Table("Vylety"), Popisek("Programy výletů")]
    public class Vylet : IData
    {
        [PrimaryKey, AutoIncrement, SkrytVeFormulari]
        public int ID { get; set; }

        [Popisek("Název")]
        public string Nazev { get; set; }

        [Popisek("Místo určení")]
        public string Misto { get; set; }

        [Enum, Popisek("Doporučovaný ročník")]
        public string Rocnik { get; set; }

        [Popisek("Předpokládaný čas odjezdu")]
        public string Odjezd { get; set; }

        [Popisek("Předpokládaný čas návratu")]
        public string Navrat { get; set; }

        [Popisek("Dopravní prostředek")]
        public string Doprava { get; set; }

        public override string ToString() => $"{Nazev}";
    }
}
