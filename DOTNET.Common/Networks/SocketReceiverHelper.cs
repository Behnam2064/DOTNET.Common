using DOTNET.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        public async Task<SocketReceivedMessageResult> GetAsBytesAsync(SocketReceiverArguments arguments)
        {
            var compoundBuffer = new List<byte>();
            var messageReceiveResult = new WebSocketReceiveResult(0, WebSocketMessageType.Close, false);
            //byte[] buffer = new byte[arguments.BufferSize];
            SocketReceivedMessageResult result = new SocketReceivedMessageResult() 
            {
                Result = new byte[arguments.BufferSize],
            };
            while (!messageReceiveResult.EndOfMessage)
            {
                // read one message until the end
                messageReceiveResult = await arguments.WebSocket.ReceiveAsync(new ArraySegment<byte>(result.Result), arguments.CancellationToken);

                result.WebSocketMessageType = messageReceiveResult.MessageType;

                byte[] readBytes = new byte[messageReceiveResult.Count];
                Array.Copy(result.Result, readBytes, messageReceiveResult.Count);
                compoundBuffer.AddRange(readBytes);


                /*if (messageReceiveResult.MessageType == WebSocketMessageType.Text)
                {
                    byte[] readBytes = new byte[messageReceiveResult.Count];
                    Array.Copy(buffer, readBytes, messageReceiveResult.Count);
                    compoundBuffer.AddRange(readBytes);
                }*/
            }
            // get string message
            //result.Result = buffer;
            result.Result = compoundBuffer.ToArray();
            return result; //compoundBuffer.ToArray();
        }

        /// <summary>
        /// The unencrypted string is sent to this method
        /// </summary>
        /// <param name="arguments"></param>
        /// <returns></returns>
        public async Task<string> GetAsStringAsync(SocketReceiverArguments arguments)
            => await Task.Run(async () =>
            {
                
                var result = await GetAsBytesAsync(arguments);
/*
                if(result.WebSocketMessageType != WebSocketMessageType.Text)
                {
                    throw new InvalidOperationException($"Cannot convert a binary file to text.(Received WebSocketMessageType:{result.WebSocketMessageType})");
                }*/


                return arguments.Encoding.GetString(result.Result);
            });

        /// <summary>
        /// An encrypted string is sent to this method
        /// </summary>
        /// <param name="arguments"></param>
        /// <returns></returns>
        public async Task<string> GetISecurityTextAsStringAsync(SocketReceiverArguments arguments)
            => await Task.Run(async () => 
            {
                var result = await GetAsBytesAsync(arguments);

                /*
                if (result.WebSocketMessageType != WebSocketMessageType.Text)
                {
                    throw new InvalidOperationException($"Cannot convert a binary file to text.(Received WebSocketMessageType:{result.WebSocketMessageType})");
                }
                */

                return arguments.ISecurityText.DecryptText(arguments.Encoding.GetString(result.Result));

            });

        public async Task<TEntity?> GetEntityAsync<TEntity>(SocketReceiverArguments arguments) where TEntity : class
        => await Task.Run(async () => JsonSerializer.Deserialize<TEntity>(await GetAsStringAsync(arguments)));

        public async Task<TEntity?> GetISecurityTextAsEntityAsync<TEntity>(SocketReceiverArguments arguments) where TEntity : class
        => await Task.Run(async () =>
        {
            string txt = await GetAsStringAsync(arguments);
            if (string.IsNullOrEmpty(txt) || string.IsNullOrWhiteSpace(txt)) return null;
            txt = arguments.ISecurityText.DecryptText(txt);
            return JsonSerializer.Deserialize<TEntity>(txt);


        });
    }

    public class SocketReceivedMessageResult
    {
        public WebSocketMessageType WebSocketMessageType { get; set; }
        public byte[]? Result { get; set; }
    }

    public class SocketReceiverArguments
    {
        public required int BufferSize { get; set; }

        public required CancellationToken CancellationToken;
        public required WebSocket WebSocket { get; set; }
        //public required WebSocketMessageType WebSocketMessageType { get; set; }
        public ISecurityText? ISecurityText { get; set; }
        public Encoding Encoding { get; set; }

        public SocketReceiverArguments()
        {
            this.Encoding = Encoding.UTF8;
        }
    }
}
