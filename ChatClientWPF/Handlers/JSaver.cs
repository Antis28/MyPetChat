using ChatClientWPF.Models;
using ChatClientWPF.ServiceChat;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatClientWPF.Handlers
{
    internal class JSaver
    {
        static string path = "Settings.json";

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
                // TODO: Переделать в логгер ECS(DI)
                var message = "Файл настроек команд не существует - settings.json";
                //Main.Logger.ShowMessage(message);
                throw new Exception(message);
               // return commandSettings;
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
