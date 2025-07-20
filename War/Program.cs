using Fleck;
using System.Text.Json;
using War;

// Dictionnaire pour gérer une instance de jeu par connexion
var games = new Dictionary<Guid, WarGame>();

var webSocketAddress = "ws://0.0.0.0:8080/war-csharp"; // Adresse WebSocket par défaut

// Crée le serveur WebSocket qui écoute sur toutes les interfaces au port 8080
var server = new WebSocketServer(webSocketAddress);

Console.WriteLine($"Server starting on {webSocketAddress}");

server.Start(socket =>
{
    // Événement à l'ouverture d'une nouvelle connexion
    socket.OnOpen = () =>
    {
        Console.WriteLine($"Client connected: {socket.ConnectionInfo.Id}");

        // Crée une nouvelle partie pour ce client et la stocke
        var game = new WarGame();
        games[socket.ConnectionInfo.Id] = game;

        // Envoie l'état initial du jeu
        var initialState = game.GetGameState();
        socket.Send(JsonSerializer.Serialize(initialState));
    };

    // Événement à la réception d'un message
    socket.OnMessage = message =>
    {
        Console.WriteLine($"Message received from {socket.ConnectionInfo.Id}: {message}");

        if (!games.TryGetValue(socket.ConnectionInfo.Id, out var game)) return;

        var action = JsonSerializer.Deserialize<PlayerAction>(message);

        if (action?.Action == "play_card")
        {
            game.PlayTurn();
            var newState = game.GetGameState();
            socket.Send(JsonSerializer.Serialize(newState));
        }
    };

    // Événement à la fermeture d'une connexion
    socket.OnClose = () =>
    {
        Console.WriteLine($"Client disconnected: {socket.ConnectionInfo.Id}");
        // Nettoie l'instance de jeu associée
        games.Remove(socket.ConnectionInfo.Id);
    };
});

while (true)
{
    Thread.Sleep(int.MaxValue);
}