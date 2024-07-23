using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

if(args.Length < 1)
{
    Console.WriteLine("Usage: SocketClient <message> [<ip>] [<port>]");
    return;
}
// Fetch message
var message = args[0];//"Hi friends 👋!<|EOM|>";

// Fetch ip
var ipString = "127.0.0.1";
if (1 < args.Length) ipString = args[1];
var ip = IPAddress.Parse(ipString);
// Fetch port
var port = 8080;
if(2 < args.Length) port = int.Parse(args[2]);

// Prepare socket client
IPEndPoint ipEndPoint = new (ip, port);
using Socket client = new(
    ipEndPoint.AddressFamily, 
    SocketType.Stream, 
    ProtocolType.Tcp);

await client.ConnectAsync(ipEndPoint);
while (true)
{
    // Send message.
    var messageBytes = Encoding.UTF8.GetBytes(message);
    _ = await client.SendAsync(messageBytes, SocketFlags.None);
    Console.WriteLine($"Socket client sent message: \"{message}\"");

    // Receive ack.
    var buffer = new byte[1_024];
    var received = await client.ReceiveAsync(buffer, SocketFlags.None);
    var response = Encoding.UTF8.GetString(buffer, 0, received);
    if (response == "<|ACK|>")
    {
        Console.WriteLine(
            $"Socket client received acknowledgment: \"{response}\"");
        break;
    }
    // Sample output:
    //     Socket client sent message: "Hi friends 👋!<|EOM|>"
    //     Socket client received acknowledgment: "<|ACK|>"
}

client.Shutdown(SocketShutdown.Both);