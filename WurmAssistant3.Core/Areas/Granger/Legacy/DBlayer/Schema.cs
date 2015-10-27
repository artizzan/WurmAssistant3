using System.Data.SQLite;
using AldursLab.WurmAssistant3.Core.Areas.Granger.Legacy.Utils;

namespace AldursLab.WurmAssistant3.Core.Areas.Granger.Legacy.DBlayer
{
    static class DBSchema
    {
        static string _ConnectionString = null;
        public static string ConnectionString { get { return _ConnectionString; } }

        public static void SetConnectionString(string DBPath)
        {
            _ConnectionString = 
                  "DbLinqProvider=Sqlite;"
                + "DbLinqConnectionType=System.Data.SQLite.SQLiteConnection, System.Data.SQLite, Version=1.0.61.0, Culture=neutral, PublicKeyToken=db937bc2d44ff139;"
                + "Data Source="+DBPath+";"
                + "Pooling=True;"
                + "Max Pool Size=100;";
        }

        public const string HorsesTableName = "horses";
        public static SQLiteHelper.SQLiteFieldDef[] HorsesSchema;
        public const string TraitValuesTableName = "traitvalues";
        public static SQLiteHelper.SQLiteFieldDef[] TraitValuesSchema;
        public const string HerdsTableName = "herds";
        public static SQLiteHelper.SQLiteFieldDef[] HerdsSchema;

        static DBSchema()
        {
            HorsesSchema = new SQLiteHelper.SQLiteFieldDef[] { 
                new SQLiteHelper.SQLiteFieldDef() { fieldName = "id", fieldParams = "INTEGER PRIMARY KEY"},
                new SQLiteHelper.SQLiteFieldDef() { fieldName = "herd"},
                new SQLiteHelper.SQLiteFieldDef() { fieldName = "name"},
                new SQLiteHelper.SQLiteFieldDef() { fieldName = "fathername"},
                new SQLiteHelper.SQLiteFieldDef() { fieldName = "mothername"},
                new SQLiteHelper.SQLiteFieldDef() { fieldName = "traits"},
                new SQLiteHelper.SQLiteFieldDef() { fieldName = "notinmood"},
                new SQLiteHelper.SQLiteFieldDef() { fieldName = "pregnantuntil"},
                new SQLiteHelper.SQLiteFieldDef() { fieldName = "groomedon"},
                new SQLiteHelper.SQLiteFieldDef() { fieldName = "ismale"},
                new SQLiteHelper.SQLiteFieldDef() { fieldName = "takencareofby"},
                new SQLiteHelper.SQLiteFieldDef() { fieldName = "traitsinspectedatskill"},
                new SQLiteHelper.SQLiteFieldDef() { fieldName = "epiccurve"},
                new SQLiteHelper.SQLiteFieldDef() { fieldName = "age"},
                new SQLiteHelper.SQLiteFieldDef() { fieldName = "color"},
                new SQLiteHelper.SQLiteFieldDef() { fieldName = "comments"},
                new SQLiteHelper.SQLiteFieldDef() { fieldName = "specialtags"},
                new SQLiteHelper.SQLiteFieldDef() { fieldName = "pairedwith"},
                new SQLiteHelper.SQLiteFieldDef() { fieldName = "brandedfor"},
                new SQLiteHelper.SQLiteFieldDef() { fieldName = "birthdate"}
                };

            TraitValuesSchema = new SQLiteHelper.SQLiteFieldDef[] {
                new SQLiteHelper.SQLiteFieldDef() { fieldName = "id", fieldParams = "INTEGER PRIMARY KEY"},
                new SQLiteHelper.SQLiteFieldDef() { fieldName = "valuemapid"},
                new SQLiteHelper.SQLiteFieldDef() { fieldName = "traitid"},
                new SQLiteHelper.SQLiteFieldDef() { fieldName = "traitvalue"}
            };

            HerdsSchema = new SQLiteHelper.SQLiteFieldDef[] {
                new SQLiteHelper.SQLiteFieldDef() { fieldName = "id_herdname", fieldParams = "PRIMARY KEY"},
                new SQLiteHelper.SQLiteFieldDef() { fieldName = "selected"}
            };
        }

        public static SQLiteConnection GenerateNewConnection()
        {
            return new SQLiteConnection(ConnectionString);
        }
    }
}
