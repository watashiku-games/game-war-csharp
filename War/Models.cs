using System.Text.Json.Serialization;

namespace War;

// Représente une carte à jouer
public record Card(
    [property: JsonPropertyName("suit")] string Suit,
    [property: JsonPropertyName("value")] string Value
);

// Représente un joueur dans l'état du jeu
public record Player(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("cardCount")] int CardCount
);

// Représente une carte jouée dans la zone de bataille
public record BattleCard(
    [property: JsonPropertyName("player")] string Player,
    [property: JsonPropertyName("card")] Card Card
);

// L'objet principal envoyé au client
public record GameState(
    [property: JsonPropertyName("gameType")] string GameType,
    [property: JsonPropertyName("players")] List<Player> Players,
    [property: JsonPropertyName("battleZone")] List<BattleCard> BattleZone,
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("canPlay")] bool CanPlay
);

// L'action reçue du client
public record PlayerAction(
    [property: JsonPropertyName("action")] string Action
);