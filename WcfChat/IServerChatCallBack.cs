using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;

namespace WcfChat
{
    public interface IServerChatCallBack
    {
        [OperationContract(IsOneWay = true)]
        void MsgCallBack(string msg);

        /// <summary>
        /// Пользователь вошел в чат
        /// </summary>
        /// <param name="user"></param>
        [OperationContract(IsOneWay = true)]
        void ArrivedUserCallBack(ClientUser user);

        /// <summary>
        /// Пользователь покинул чат
        /// </summary>
        /// <param name="user"></param>
        [OperationContract(IsOneWay = true)]
        void GoneUserCallBack(ClientUser user);

        /// <summary>
        /// Обновился пользовательский список
        /// </summary>
        /// <param name="user"></param>
        [OperationContract(IsOneWay = true)]
        void UserListUpdatedCallBack(List<ClientUser> users);

    }
}
