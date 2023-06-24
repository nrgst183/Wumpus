using WumpusLib.Entities;
using WumpusLib.Entities.Events;
using WumpusLib.Entities.Player;
using WumpusLib.Entities.Player.Bot;
using WumpusLib.Game.GameModes;
using WumpusLib.Map;
using WumpusLib.Map.Results;

namespace WumpusLib.Game;

public class WumpusGame
{
    private readonly Queue<Player> _turnQueue = new();

    public Dictionary<Entity, int> EntityDeathCountMapping = new();
    public List<EntityKillInformation> EntityKills = new();

    public Dictionary<int, Player> Players = new();
    public Dictionary<Team, List<Player>> PlayerTeamMapping = new();

    public WumpusGame(GameMap map, GameMode gameMode)
    {
        GameMode = gameMode;
        GameMap = map;
        map.OnEntityKilled += Map_OnEntityKilled;

        if (!GameMap.IsMapGenerated) GameMap.InitializeGameMap(gameMode);
    }

    public GameMode GameMode { get; }
    public GameMap GameMap { get; }

    public DateTime TimeStarted { get; private set; }
    public DateTime TimeEnded { get; private set; }

    public bool IsGameEnded => GameMode.IsGameEndCondition(this);


    public TimeSpan TotalTimeElapsed => DateTime.Now - TimeStarted;

    public event EventHandler<Player> OnPlayerTurnStarted;

    public AddPlayerResult CreateAndAddNewPlayer(string playerName, BotSkillLevel? skillLevel = null)
    {
        if (GameMode.MaximumPlayers <= Players.Count)
            return AddPlayerResult.TooManyPlayers;

        var playerAccessor = new EntityGameMapAccessor(this, GameMap);

        List<Arrow> arrowList = new();

        for (var counter = 0; counter < GameMode.GameModeOptions.AmountOfStartingArrows; counter++)
        {
            var arrowAccessor = new EntityGameMapAccessor(this, GameMap);
            arrowList.Add(new Arrow(arrowAccessor));
        }

        var player = skillLevel.HasValue
            ? new BotPlayer(skillLevel.Value, playerName, arrowList, playerAccessor, GameMode)
            : new Player(playerName, arrowList, playerAccessor);

        Players.Add(Players.Count + 1, player);

        GameMap.AddEntityToRandomRoom(player, 1);

        _turnQueue.Enqueue(player);

        return AddPlayerResult.Success;
    }

    public AssignPlayerToTeamResult AssignPlayerToTeam(int playerId, Team team)
    {
        try
        {
            if (!Players.ContainsKey(playerId)) return AssignPlayerToTeamResult.PlayerNotFound;

            if (!PlayerTeamMapping.ContainsKey(team)) PlayerTeamMapping[team] = new List<Player>();

            PlayerTeamMapping[team].Add(Players[playerId]);
            return AssignPlayerToTeamResult.Success;
        }
        catch
        {
            return AssignPlayerToTeamResult.UnknownError;
        }
    }

    public void StartNextTurn()
    {
        if (_turnQueue.Count == 0)
            // If the turn queue is empty, there are no players left to move
            return;

        // Start the turn for the player at the front of the queue
        var currentPlayer = _turnQueue.Peek();
        OnPlayerTurnStarted(this, currentPlayer);
    }

    public void AdvanceTurn(Player player)
    {
        // Check if the current player is the one ending their turn
        if (_turnQueue.Peek() == player)
        {
            // Dequeue the player from the front and enqueue them at the end
            _turnQueue.Dequeue();
            _turnQueue.Enqueue(player);

            // Start the turn for the next player
            StartNextTurn();
        }
        else
        {
            // Handle an unexpected turn end, e.g., by throwing an exception or logging an error
            throw new InvalidOperationException("A player tried to end their turn out of order.");
        }
    }

    public bool IsPlayerTurn(Player player)
    {
        if (_turnQueue.Count == 0)
            return false;
        var currentPlayer = _turnQueue.Peek();
        return currentPlayer == player;
    }

    public void StartGame()
    {
        GameMap.GameStarted = true;

        // Ensure that all players have been added to the game
        if (Players.Count == 0)
            throw new InvalidOperationException("No players have been added to the game.");

        TimeStarted = DateTime.Now;

        StartNextTurn();
    }

    public Dictionary<Entity, int> GetKillCountsByEntityTypeKilled<T>()
    {
        var dict = new Dictionary<Entity, int>();

        foreach (var entity in EntityKills.Where(x => x.EntityThatWasKilled is T))
            if (!dict.ContainsKey(entity.EntityThatKilled))
                dict.Add(entity.EntityThatKilled, 1);
            else
                dict[entity.EntityThatKilled]++;

        return dict;
    }

    private void Map_OnEntityKilled(object? sender, EntityDiedArgs e)
    {
        EntityKills.Add(new EntityKillInformation(e.EntityThatKilled, e.EntityThatDied));

        if (EntityDeathCountMapping.ContainsKey(e.EntityThatDied))
            EntityDeathCountMapping[e.EntityThatDied]++;
        else
            EntityDeathCountMapping.Add(e.EntityThatDied, 1);
    }
}