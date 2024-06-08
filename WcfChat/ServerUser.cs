
using System.ServiceModel;

namespace WcfChat
{
    public class ServerUser
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public OperationContext operationContext { get; set; }

        public static implicit operator ClientUser(ServerUser su)
        {
            var cu = new ClientUser() { ID = su.ID, Name = su.Name};
            return cu;
        }
        public static explicit operator ServerUser(ClientUser cu)
        {
            var su = new ServerUser() { ID = cu.ID, Name = cu.Name };
            return su;
        }
    }
}
