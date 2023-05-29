using System.Net.Sockets;
using System.Net;


namespace ChatServer
{
    public class ServerObj
    {
        #region Fields
        private TcpListener tcpListener;
        private List<ClientObj> users;
        #endregion

        public ServerObj()
        {
            tcpListener = new TcpListener(IPAddress.Any, 8888);
            users = new List<ClientObj>();
        }

        /// <summary>
        /// listens incomin connections
        /// </summary>
        /// <returns></returns>
        public async Task ListenAsync()
        {
            try
            {
                tcpListener.Start();
                Console.WriteLine("Server started. Waiting for connections...");

                while (true)
                {
                    TcpClient tcpClient = await tcpListener.AcceptTcpClientAsync();
                    ClientObj clientObj = new ClientObj(tcpClient, this);
                    users.Add(clientObj);

                    Task.Run(clientObj.ProcessAsync); // message exhange 
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                Disconnect();
            }
        }

        /// <summary>
        /// broadcasts messages to connected users
        /// </summary>
        /// <param name="message"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task BroadcastMessageAsync(string message, string id)
        {
            foreach (var user in users)
            {
                if (user.Id != id) 
                {
                    await user.Writer.WriteLineAsync(message); 
                    await user.Writer.FlushAsync();
                }
            }
        }

        /// <summary>
        /// removes the connection with certain user
        /// </summary>
        /// <param name="id"></param>
        public void RemoveConnection(string id)
        {
            ClientObj? client = users.FirstOrDefault(c => c.Id == id);
            if (client != null) users.Remove(client);
            client?.Close();
        }

        /// <summary>
        /// disconnects all the users
        /// </summary>
        public void Disconnect()
        {
            foreach (var client in users)
            {
                client.Close(); 
            }
            tcpListener.Stop(); 
        }
    }
}
