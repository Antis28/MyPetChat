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

            SendMsg($"{user.Name} подключился к чату!");

            return user.ID;
        }

        public void Disconnect(int id)
        {
            throw new NotImplementedException();
        }

        public void SendMsg(string msg)
        {
            throw new NotImplementedException();
        }
    }
}
