

namespace ChatServer
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            ServerObj server = new ServerObj();
            await server.ListenAsync(); 
        }
    }
    
    
}