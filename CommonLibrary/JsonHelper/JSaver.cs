using CommonLibrary.Interfaces;
using Newtonsoft.Json;
using System;
using System.IO;

namespace CommonLibrary
{
    public class JSaver<T> : IDataSettingsService<T> where T : class, IDataItem, new()
    {
        private static string path = "Settings.json";

        public static bool SettingExists()
        {
            return File.Exists(path);
        }

        public void Save(object clientObject)
        {
            var jsString = JsonConvert.SerializeObject(clientObject, new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                TypeNameHandling = TypeNameHandling.Auto,
            });
            using (var sw = new StreamWriter(path)) { sw.Write(jsString); }
        }

        public T LoadSetting()
        {
            T commandSettings = new T();

            if (!SettingExists())
            {
                var message = "Файл настроек команд не существует - settings.json";
                throw new Exception(message);
            }

            // deserialize JSON directly from a file
            var text = System.IO.File.ReadAllText(path);
            commandSettings = Load(text);

            return commandSettings;
        }
        public T Load(string jsonString)
        {
            var commandSettings = JsonConvert.DeserializeObject<T>(jsonString, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            });

            return commandSettings;
        }

        public T LoadOrCreateSetting(T defaultSettings)
        {
            T settings;

            if (SettingExists())
            {
                settings = LoadSetting();
            }
            else
            {
                settings = defaultSettings;
                Save(settings);
            }
            return settings;
        }


    }
}
