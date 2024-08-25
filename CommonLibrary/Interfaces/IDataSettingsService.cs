using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibrary.Interfaces
{
    public interface IDataSettingsService<T> where T : class, IDataItem, new()
    {
        void Save(object clientObject);
        public T LoadSetting();
        public T Load(string jsonString);
        public T LoadOrCreateSetting(T defaultSettings);
    }
}
