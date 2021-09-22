using DarkRift.Server;
namespace MeatInc.ActionGunnersServer.Connections
{
    public abstract class AbstractClientConnection
    {
        public IClient Client { get; }

        public AbstractClientConnection(IClient client)
        {
            Client = client;
        }
    }
}