using System.Net.Sockets;

namespace ChatServer
{
    public class ClientObj
    {
        #region Fields and Properties
        private string id;
        protected internal string Id { get => id; }

        private StreamReader reader;
        protected internal StreamReader Reader { get=> reader; }

        private StreamWriter writer;
        protected internal StreamWriter Writer { get => writer; }

        private TcpClient tcpClient;
        private ServerObj server;
        #endregion

        public ClientObj(TcpClient tcpClient, ServerObj serverObject)
        {
            id = Guid.NewGuid().ToString();

            this.tcpClient = tcpClient;
            server = serverObject;
            
            var stream = this.tcpClient.GetStream();
            reader = new StreamReader(stream);
            writer = new StreamWriter(stream);
        }

        /// <summary>
        /// provides message exhange with the server
        /// </summary>
        /// <returns></returns>
        public async Task ProcessAsync()
        {
            try
            {
                string? userName = await Reader.ReadLineAsync();
                string? message = $"{DateTime.Now}\n{userName} has entered the chat.";

                await server.BroadcastMessageAsync(message, Id); //notifies about new user
                Console.WriteLine(message);

                while (true)                                //endless cycle for getting messages
                {
                    try
                    {
                        message = await Reader.ReadLineAsync();
                        if (message == null) continue;
                        message = $"{DateTime.Now}\n{userName}: {message}";
                        Console.WriteLine(message);
                        await server.BroadcastMessageAsync(message, Id);
                    }
                    catch
                    {
                        message = $"{DateTime.Now}\n{userName} has just left the chat.";
                        Console.WriteLine(message);
                        await server.BroadcastMessageAsync(message, Id);
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                server.RemoveConnection(Id); //release of resources
            }
        }
        
        /// <summary>
        /// closes the connection
        /// </summary>
        public void Close()
        {
            Writer.Close();
            Reader.Close();
            tcpClient.Close();
        }
    }
}
