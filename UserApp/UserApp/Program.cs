using System.Net.Sockets;

namespace UserApp
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            User user = new User();
            await user.Start();
        }
    }
}