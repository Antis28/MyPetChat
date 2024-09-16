namespace CommonLibraryStandart.Interfaces
{
    public interface IDataSettingsService<T> where T : class, IDataItem, new()
    {
        void Save(object clientObject);
        public T LoadSetting();
        public T Load(string jsonString);
        public T LoadOrCreateSetting(T defaultSettings);
    }
}
