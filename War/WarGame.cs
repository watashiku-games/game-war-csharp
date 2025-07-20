using System.Security.Cryptography;

namespace War;

public class WarGame
{
    private Queue<Card> _player1Deck = new();
    private Queue<Card> _player2Deck = new();
    private List<Card> _battleZoneCards = new();
    private string _status = "Game just started";

    public WarGame()
    {
        InitializeDecks();
    }

    private void InitializeDecks()
    {
        var suits = new[] { "hearts", "diamonds", "clubs", "spades" };
        var values = new[] { "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K", "A" };
        var fullDeck = suits.SelectMany(suit => values.Select(value => new Card(suit, value))).ToList();

        // Mélange simple pour ce mock
        var shuffledDeck = fullDeck.OrderBy(c => RandomNumberGenerator.GetInt32(int.MaxValue)).ToList();

        // Distribue les cartes
        for (var i = 0; i < shuffledDeck.Count; i++)
        {
            if (i % 2 == 0) _player1Deck.Enqueue(shuffledDeck[i]);
            else _player2Deck.Enqueue(shuffledDeck[i]);
        }
    }

    private int GetCardValue(Card card)
    {
        return card.Value switch
        {
            "J" => 11,
            "Q" => 12,
            "K" => 13,
            "A" => 14,
            _ => int.Parse(card.Value)
        };
    }

    public void PlayTurn()
    {
        if (_player1Deck.Count == 0 || _player2Deck.Count == 0)
        {
            _status = _player1Deck.Count == 0 ? "Player 2 wins the game!" : "Player 1 wins the game!";
            return;
        }

        var p1Card = _player1Deck.Dequeue();
        var p2Card = _player2Deck.Dequeue();

        _battleZoneCards.Add(p1Card);
        _battleZoneCards.Add(p2Card);

        var p1Value = GetCardValue(p1Card);
        var p2Value = GetCardValue(p2Card);

        if (p1Value > p2Value)
        {
            foreach (var card in _battleZoneCards) _player1Deck.Enqueue(card);
            _status = "Player 1 wins the round";
        }
        else if (p2Value > p1Value)
        {
            foreach (var card in _battleZoneCards) _player2Deck.Enqueue(card);
            _status = "Player 2 wins the round";
        }
        else
        {
            // Logique de "Bataille" simplifiée : les cartes sont mises de côté pour le prochain tour
            _status = "War!";
        }

        // La bataille est terminée pour ce tour, on vide la zone pour le prochain envoi JSON.
        if (p1Value != p2Value)
        {
            _battleZoneCards.Clear();
        }
    }

    public GameState GetGameState()
    {
        // On ne montre que les 2 dernières cartes jouées dans la zone de bataille
        var currentBattle = _battleZoneCards.TakeLast(2).ToList();

        return new GameState(
            GameType: "bataille",
            Players: new List<Player>
            {
            new("player1", "Joueur 1", _player1Deck.Count),
            new("player2", "Joueur 2", _player2Deck.Count)
            },
            BattleZone: new List<BattleCard>
            {
            new("player1", currentBattle.FirstOrDefault()),
            new("player2", currentBattle.LastOrDefault())
            },
            Status: _status,
            CanPlay: _player1Deck.Count > 0 && _player2Deck.Count > 0
        );
    }
}