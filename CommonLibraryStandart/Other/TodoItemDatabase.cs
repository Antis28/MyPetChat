using CommonLibrary;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibraryStandart.Other
{
    public class TodoItemDatabase : IDataSettingsService
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
            var result = await Database.CreateTableAsync<ServerSettings>();
        }

        public async Task<List<ServerSettings>> GetItemsAsync()
        {
            await Init();
            return await Database.Table<ServerSettings>().ToListAsync();
        }

        public async Task<List<ServerSettings>> GetItemsNotDoneAsync()
        {
            await Init();
            return await Database.Table<ServerSettings>().Where(t => true).ToListAsync();

            // SQL queries are also possible
            //return await Database.QueryAsync<TodoItem>("SELECT * FROM [TodoItem] WHERE [Done] = 0");
        }

        public async Task<ServerSettings> GetItemAsync(int id)
        {
            await Init();
            return await Database.Table<ServerSettings>().Where(i => i.ID == id).FirstOrDefaultAsync();
        }

        public async Task<int> SaveItemAsync(ServerSettings item)
        {
            await Init();
            if (item.ID != 0)
                return await Database.UpdateAsync(item);
            else
                return await Database.InsertAsync(item);
        }

        public async Task<int> DeleteItemAsync(ServerSettings item)
        {
            await Init();
            return await Database.DeleteAsync(item);
        }
    }
}
