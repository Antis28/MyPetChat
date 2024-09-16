
using Newtonsoft.Json;
using System;
using System.IO;

namespace ChatClientWPF.Handlers
{
    public class JSaver
    {
        private static string path = "Settings.json";

        public static bool SettingExists()
        {
            return File.Exists(path);
        }


        public static void Save(object? clientObject)
        {
            var jsString = JsonConvert.SerializeObject(clientObject, new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                TypeNameHandling = TypeNameHandling.Auto,
            });
            using (var sw = new StreamWriter(path)) { sw.Write(jsString); }
        }

        public static T LoadSetting<T>() where T : new()
        {
            T commandSettings = new T();

            if (!SettingExists())
            {
                var message = "Файл настроек команд не существует - settings.json";
                throw new Exception(message);
            }

            // deserialize JSON directly from a file
            var text = System.IO.File.ReadAllText(path);
            commandSettings = Load<T>(text);

            return commandSettings;
        }
        public static T Load<T>(string jsonString) where T : new()
        {
            var commandSettings = JsonConvert.DeserializeObject<T>(jsonString, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            });

            return commandSettings;
        }
    }
}
