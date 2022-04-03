using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace SkolniVylety
{
    [Table("Studenti"), Popisek("Studenti")]
    public class Student : IData
    {
        [PrimaryKey, AutoIncrement, SkrytVeFormulari]
        public int ID { get; set; }

        [Popisek("Jméno")]
        public string Jmeno { get; set; }

        [Popisek("Příjmení")]
        public string Prijmeni { get; set; }

        [Reference(typeof(Trida)), SkrytVeFormulari]
        public int Trida { get; set; }

        [Popisek("Rok narození")]
        public DateTime Narozen { get; set; } = DateTime.Now.AddYears(-13);

        public override string ToString() => $"{Jmeno} {Prijmeni}";
    }
}
