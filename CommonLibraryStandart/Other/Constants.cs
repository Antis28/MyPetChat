using System.IO;
using Xamarin.Essentials;

namespace CommonLibraryStandart.Other
{
    internal class Constants
    {
        public static string DatabaseFilename => "ChatMauiSQLite.db3";

        public static SQLite.SQLiteOpenFlags Flags =>
            // подключение может считывать и записывать данные.
            SQLite.SQLiteOpenFlags.ReadWrite |
            // подключение автоматически создаст файл базы данных, если он не существует.
            SQLite.SQLiteOpenFlags.Create |
            // подключение будет участвовать в общем кэше, если оно включено.
            SQLite.SQLiteOpenFlags.SharedCache |
            // файл базы данных не зашифрован.
            SQLite.SQLiteOpenFlags.ProtectionNone;

        public static string DatabasePath =>
            Path.Combine(FileSystem.AppDataDirectory, DatabaseFilename);
    }
}
