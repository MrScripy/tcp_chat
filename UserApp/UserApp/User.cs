using System.Net.Sockets;


namespace UserApp
{
    internal class User
    {
        #region Fields and properties
        private readonly string host = "127.0.0.1";
        private readonly int port = 8888;
        private string? userName;
        private StreamReader? reader;
        private StreamWriter? writer;
        #endregion

        /// <summary>
        /// provides message exchange
        /// </summary>
        /// <returns></returns>
        public async Task Start()
        {            
            using TcpClient client = new TcpClient();
            Console.Write("What is your name? ");
            userName = Console.ReadLine();     
            
            try
            {
                client.Connect(host, port); 

                var stream = client.GetStream();
                reader = new StreamReader(stream);
                writer = new StreamWriter(stream);

                if (writer is null || reader is null) return;
                Task.Run(() => ReceiveMessageAsync(reader)); // sgetting messages
                await SendMessageAsync(writer);              // recieving messages
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            writer?.Close();                                // release of resources
            reader?.Close();
        }

        /// <summary>
        /// sends messages
        /// </summary>
        /// <param name="writer"></param>
        /// <returns></returns>
        async Task SendMessageAsync(StreamWriter writer)
        {
            await writer.WriteLineAsync(userName);              // sends name for creating a new user
            await writer.FlushAsync();
            Console.WriteLine("Write your message and press Enter for sending.");

            while (true)                                       // sends messages
            {
                string? message = Console.ReadLine();
                await writer.WriteLineAsync(message);           
                await writer.FlushAsync();
            }
        }

        /// <summary>
        /// gets messages
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        async Task ReceiveMessageAsync(StreamReader reader)
        {
            while (true)
            {
                try
                {
                    string? message = await reader.ReadLineAsync();
                    if (string.IsNullOrEmpty(message)) continue;        // checks if the messages is empty
                    Print(message);
                }
                catch
                {
                    break;
                }
            }
        }

        /// <summary>
        /// predicts mixing recieved message with sending message
        /// </summary>
        /// <param name="message"></param>
        void Print(string message)
        {
            if (OperatingSystem.IsWindows())    
            {
                var position = Console.GetCursorPosition(); 
                int left = position.Left;   
                int top = position.Top;    
                                            
                Console.MoveBufferArea(0, top, left, 1, 0, top + 1); // coping the sending message in buffer
                Console.SetCursorPosition(0, top);
                Console.WriteLine(message);
                Console.SetCursorPosition(left, top + 1);
            }
            else Console.WriteLine(message);
        }
    }
}
