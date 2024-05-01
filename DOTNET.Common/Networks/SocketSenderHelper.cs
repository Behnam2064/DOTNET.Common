using DOTNET.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DOTNET.Common.Networks
{
    public class SocketSenderHelper 
    {
        public SocketSenderHelper()
        {

        }

        public async Task SendAsync(SocketSenderArguments arguments)
        {
            await arguments.WebSocket.SendAsync(new ArraySegment<byte>(arguments.Data), arguments.WebSocketMessageType, true, arguments.CancellationToken);
        }


        public async Task SendAsync(SocketSenderArguments arguments,object Data)
        {
            if (arguments.Data != null)
                throw new Exception($"Send your object with '{nameof(Data)}' arguments"); 

            var data = arguments.Encoding.GetBytes(arguments.ISecurityText.EncryptText(JsonSerializer.Serialize(Data)));

            arguments.Data = data;

            await SendAsync(arguments);


            //await arguments.WebSocket.SendAsync(new ArraySegment<byte>(data), arguments.WebSocketMessageType, true, arguments.CancellationToken);
        }

    }


    public class SocketSenderArguments 
    {
        public byte[]? Data { get; set; }

        public required CancellationToken CancellationToken;
        public required WebSocket WebSocket { get; set; }
        public required WebSocketMessageType WebSocketMessageType { get; set; }
        public ISecurityText? ISecurityText { get; set; }

        public Encoding Encoding { get; set; }

        public SocketSenderArguments()
        {
                Encoding = Encoding.UTF8;
        }
    }
}
