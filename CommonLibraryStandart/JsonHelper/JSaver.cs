
using CommonLibraryStandart.Interfaces;
using Newtonsoft.Json;
using System;
using System.IO;

namespace ChatClientWPF.Handlers
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
            T commandSettings;

            if (!SettingExists())
            {
                var message = "Файл настроек команд не существует - settings.json";
                throw new Exception(message);
            }

            // deserialize JSON directly from a file
            var text = System.IO.File.ReadAllText(path);
            commandSettings = Load(text);
            if (commandSettings != null)
            {
                return commandSettings;
            }
            else
            {
                return new T();
            }
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
            throw new NotImplementedException();
        }
    }
}
