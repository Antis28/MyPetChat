using CommonLibrary;
using CommonLibrary.Interfaces;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using static SQLite.SQLite3;

namespace CommonLibraryStandart.Other
{
    public class TodoItemDatabase<T> : IDataSettingsService<T> where T : class, IDataItem, new()
    {
        SQLiteAsyncConnection Database;

        public TodoItemDatabase()
        {
        }

        async Task Init()
        {
            if (Database is not null)
                return;

            Database = new SQLiteAsyncConnection(Constants.DatabasePath, Constants.Flags);
            var result = await Database.CreateTableAsync<T>();
        }

        public async Task<List<T>> GetItemsAsync()
        {
            await Init();
            return await Database.Table<T>().ToListAsync();
        }

        public async Task<List<T>> GetItemsNotDoneAsync()
        {
            await Init();
            return await Database.Table<T>().Where(t => true).ToListAsync();

            // SQL queries are also possible
            //return await Database.QueryAsync<TodoItem>("SELECT * FROM [TodoItem] WHERE [Done] = 0");
        }

        public async Task<T> GetItemAsync(int id)
        {
            await Init();
            return await Database.Table<T>().Where(i => i.ID == id).FirstOrDefaultAsync();
        }

        public async Task<int> SaveItemAsync(T item)
        {
            await Init();
            if (item.ID != 0)
               { return await Database.UpdateAsync(item); }
            else
               { return await Database.InsertAsync(item); }
        }

        public async Task<int> DeleteItemAsync(T item)
        {
            await Init();
            return await Database.DeleteAsync(item);
        }


        public T LoadSetting()
        {
            T result = null;
            Task.Run(async () => {
               result =  await GetItemAsync(0);

            });
            Task.Delay(1000);
            return result;
        }

        public T Load(string jsonString)
        {
            throw new NotImplementedException();
        }

        public T LoadOrCreateSetting(T defaultSettings)
        {
            T result = defaultSettings;
            var t = Task.Run(() => {
                var t =  LoadSetting();
                if (t == null)
                {
                    Save(defaultSettings);                    
                }
            });
            t.Wait();
            return result;
        }

        public void Save(object clientObject)
        {
            var a = SaveItemAsync(clientObject as T);
        }
    }
}
