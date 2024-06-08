using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
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
        List<ServerUser> users = new List<ServerUser>();
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
            users.Add(user);

            SendMsg($"{user.Name} подключился к чату!", 0);

            return user.ID;
        }

        public void Disconnect(int id)
        {
            var user = users.FirstOrDefault(x => x.ID == id);
            if (user == null) return;

            users.Remove(user);
            SendMsg($"{user.Name} покинул чат!", 0);
        }

        public void SendMsg(string msg, int id)
        {
            foreach (var item in users)
            {
                StringBuilder answer = new StringBuilder();
                answer.Append(DateTime.Now.ToShortTimeString());

                var user = users.FirstOrDefault(x => x.ID == id);
                answer.Append($": {user.Name} " ?? "");

                answer.Append(msg);

                item.operationContext.GetCallbackChannel<IServerChatCallBack>().MsgCallBack(answer.ToString());
            }

        }
    }
}
