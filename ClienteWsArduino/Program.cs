using System;
using System.Net.WebSockets;
using System.Text;

// See https://aka.ms/new-console-template for more information
Console.WriteLine("Cliente ws!");

string name;

while (true)
{
	Console.WriteLine("Digite su nombre");
	name = Console.ReadLine();
	break;
}

var ws = new ClientWebSocket();

//await ws.ConnectAsync(new Uri("ws://localhost:6969/ws"),
//  CancellationToken.None);
//http://192.168.1.5:22679/http://192.168.1.5:22679/

//await ws.ConnectAsync(new Uri($"ws://192.168.1.5/ws?name={name}"),
  //CancellationToken.None);


await ws.ConnectAsync(new Uri($"wss://localhost/ws?name={name}"),
  CancellationToken.None);

Console.WriteLine("Conectado al servidor!");

var sendTask = Task.Run(async () =>
{
	while (true)
	{
		string message  = Console.ReadLine();

		if (message == "exit")
			break;

        var bytes =  Encoding.UTF8.GetBytes(message);

		await ws.SendAsync(new ArraySegment<byte>(bytes),WebSocketMessageType.Text,true, CancellationToken.None);

	}
});

//await sendTask;

var receiveTask = Task.Run(async () =>
{
	var buffer = new byte[1024];
	while (true)
	{
		var result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

		if (result.MessageType == WebSocketMessageType.Close)
		{
			break;
		}
		var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
		Console.WriteLine("Received: " + message);
	}
});


//await receiveTask;


await Task.WhenAll(receiveTask,sendTask);
if (ws.State == WebSocketState.Closed) {

	await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
}

await Task.WhenAll(receiveTask,sendTask);