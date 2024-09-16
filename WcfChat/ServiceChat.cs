using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace WcfChat
{
    /// <summary>
    /// InstanceContextMode.Single - создает наш сервис в единственном экземпляре
    /// </summary>
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class ServiceChat : IServiceChat
    {
        readonly List<ServerUser> serverUsers = new List<ServerUser>();
        readonly List<string> meseges = new List<string>();
        int nextId = 1;

        public int Connect(string name)
        {
            var user = new ServerUser()
            {
                ID = nextId,
                Name = name,
                operationContext = OperationContext.Current
            };
            nextId++;   
            SendMsg($" {user.Name} подключился к чату!", 0);
            serverUsers.Add(user);
            serverUsers = serverUsers.OrderBy(x => x.Name.ToLower()).ToList();
            Console.WriteLine($"{user.Name} подключился к чату!");
            NotificationClients(user.ID);
            return user.ID;
        }

        public void Disconnect(int id)
        {
            var user = serverUsers.FirstOrDefault(x => x.ID == id);
            if (user == null) return;

            serverUsers.Remove(user);
            SendMsg($" {user.Name} покинул чат!", 0);
            Console.WriteLine($"{user.Name} покинул чат!");
            
            NotificationClients(id);
        }

        public List<ClientUser> GetUsers()
        {            
            List<ClientUser> cu = ConvertUsersList();
            return cu;
        }

        private List<ClientUser> ConvertUsersList()
        {
            var cu = new List<ClientUser>();
            foreach (var item in serverUsers)
            {
                cu.Add(item);
            }

            return cu;
        }

        public void SendMsg(string msg, int id)
        {
            foreach (var item in serverUsers)
            {
                StringBuilder answer = new StringBuilder();
                answer.Append(DateTime.Now.ToShortTimeString());

                var user = serverUsers.FirstOrDefault(x => x.ID == id);
                if (user != null)
                {
                    answer.Append($": {user.Name} ");
                }

                answer.Append(msg);

                meseges.Add(answer.ToString());

                item.operationContext.GetCallbackChannel<IServerChatCallBack>().MsgCallBack(answer.ToString());
            }

        }

        public void SendMsgPrivate(string msg, int fromId, int toId)
        {
            StringBuilder answer = new StringBuilder();
            answer.Append(DateTime.Now.ToShortTimeString());
            var user = serverUsers.FirstOrDefault(x => x.ID == toId);
            if (user == null) return;
            
            answer.Append($": {user.Name} {msg}");

            user.operationContext.GetCallbackChannel<IServerChatCallBack>().MsgCallBack(answer.ToString());
        }

        private void NotificationClients(int ID)
        {
            var cu = ConvertUsersList();            
            foreach (var item in serverUsers)
            {
                if (ID == item.ID) continue;
                
                item.operationContext.GetCallbackChannel<IServerChatCallBack>().UserListUpdatedCallBack(cu.ToArray());
            }
        }

        public List<string> GetMesseges()
        {
            return meseges;
        }
    }
}
