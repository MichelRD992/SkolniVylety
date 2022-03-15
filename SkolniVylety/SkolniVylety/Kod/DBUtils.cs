using SQLite;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms.Internals;

namespace SkolniVylety
{
    class DBUtils
    {
        public static SQLiteAsyncConnection DB { get; private set; }

        public static async Task Init()
        {
            DB = new SQLiteAsyncConnection(Path.Combine(FileSystem.AppDataDirectory, "data.db"));
            foreach (var tbl in VsechnyDBTridy)
                await DB.CreateTableAsync(tbl);
        }

        public static string Popisek(MemberInfo mi)
        {
            string name = mi.GetCustomAttribute<PopisekAttribute>()?.Text;
            if (String.IsNullOrEmpty(name))
                name = mi.Name;
            return name;
        }

        static IList<Type> vsechnyDBTridy;
        public static IList<Type> VsechnyDBTridy
        {
            get
            {
                if (vsechnyDBTridy == null)
                    vsechnyDBTridy = typeof(DBUtils).Assembly.GetTypes().Where(t => t.IsDefined(typeof(TableAttribute))).ToList();
                return vsechnyDBTridy;
            }
        }

        public static async Task<IEnumerable> DataTabulky(Type trida)
        {
            var table = DB.GetType().GetMethod("Table").MakeGenericMethod(trida).Invoke(DB, new object[] { });
            var task = (Task)table.GetType().GetMethod("ToListAsync").Invoke(table, new object[] { });
            await task.ConfigureAwait(false);
            var result = task.GetType().GetProperty("Result").GetValue(task);
            return result as IEnumerable;
        }

        public static async Task<bool> Smazat(IData obj)
        {
            var vazby = VsechnyDBTridy.GroupBy(t => t).Select(g => new
            {
                Typ = g.Key,
                Vlastnosti = g.Key.GetProperties()
                .Where(p => p.GetCustomAttribute<ReferenceAttribute>()?.Table == obj.GetType())

            }).Where(v => v.Vlastnosti.Count() > 0);

            foreach (var v in vazby)
            {
                string s = "";
                v.Vlastnosti.ForEach(p => s += (p.GetCustomAttribute<ColumnAttribute>()?.Name ?? p.Name) + $"={obj.ID} or ");
                s = s.Substring(0, s.Length - 4);
                int pocet = await DB.ExecuteScalarAsync<int>(String.Format(
                    "select count(*) from {0} where {1}", v.Typ.GetCustomAttribute<TableAttribute>()?.Name ?? v.Typ.Name, s));
                if (pocet > 0)
                    return false;
            }
            await DB.DeleteAsync(obj);
            return true;
        }
    }
}
