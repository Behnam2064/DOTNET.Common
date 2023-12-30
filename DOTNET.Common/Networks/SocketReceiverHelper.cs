using DOTNET.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace DOTNET.Common.Networks
{
    public class SocketReceiverHelper
    {
        public SocketReceiverHelper()
        {

        }

        /// <summary>
        /// https://stackoverflow.com/questions/49299333/websockets-in-net-core-2-0-receiving-messages-in-chunks
        /// Receiving messages in chunks
        /// </summary>
        /// <param name="ws"></param>
        /// <param name="arguments"></param>
        /// <returns></returns>
        public async Task<byte[]> GetAsBytesAsync(SocketReceiverArguments arguments)
        {
            var compoundBuffer = new List<byte>();
            var messageReceiveResult = new WebSocketReceiveResult(0, WebSocketMessageType.Close, false);
            byte[] buffer = new byte[arguments.BufferSize];

            while (!messageReceiveResult.EndOfMessage)
            {
                // read one message until the end
                messageReceiveResult = await arguments.WebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), arguments.CancellationToken);
                if (messageReceiveResult.MessageType == arguments.WebSocketMessageType)
                {
                    byte[] readBytes = new byte[messageReceiveResult.Count];
                    Array.Copy(buffer, readBytes, messageReceiveResult.Count);
                    compoundBuffer.AddRange(readBytes);
                }

                /*if (messageReceiveResult.MessageType == WebSocketMessageType.Text)
                {
                    byte[] readBytes = new byte[messageReceiveResult.Count];
                    Array.Copy(buffer, readBytes, messageReceiveResult.Count);
                    compoundBuffer.AddRange(readBytes);
                }*/
            }
            // get string message

            return compoundBuffer.ToArray();
        }

        /// <summary>
        /// The unencrypted string is sent to this method
        /// </summary>
        /// <param name="arguments"></param>
        /// <returns></returns>
        public async Task<string> GetAsStringAsync(SocketReceiverArguments arguments)
            => await Task.Run(async () => arguments.Encoding.GetString(await GetAsBytesAsync(arguments)));

        /// <summary>
        /// An encrypted string is sent to this method
        /// </summary>
        /// <param name="arguments"></param>
        /// <returns></returns>
        public async Task<string> GetISecurityTextAsStringAsync(SocketReceiverArguments arguments)
            => await Task.Run(async () => arguments.ISecurityText.DecryptText(arguments.Encoding.GetString(await GetAsBytesAsync(arguments))));

        public async Task<TEntity?> GetEntityAsync<TEntity>(SocketReceiverArguments arguments) where TEntity : class
        => await Task.Run(async () =>JsonSerializer.Deserialize<TEntity>(await GetAsStringAsync(arguments)));

        public async Task<TEntity?> GetISecurityTextAsEntityAsync<TEntity>(SocketReceiverArguments arguments) where TEntity : class
        => await Task.Run(async () =>
        {
            string txt = await GetAsStringAsync(arguments);
            txt = arguments.ISecurityText.DecryptText(txt);
            return JsonSerializer.Deserialize<TEntity>(txt);


        });
    }


    public class SocketReceiverArguments
    {
        public required int BufferSize { get; set; }
        public required CancellationToken CancellationToken;
        public required WebSocket WebSocket { get; set; }
        public required WebSocketMessageType WebSocketMessageType { get; set; }
        public ISecurityText? ISecurityText { get; set; }
        public Encoding Encoding { get; set; }

        public SocketReceiverArguments()
        {
            this.Encoding = Encoding.UTF8;
        }
    }
}
