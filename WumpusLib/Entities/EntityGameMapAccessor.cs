using WumpusLib.Entities.Events;
using WumpusLib.Game;
using WumpusLib.Map;
using WumpusLib.Map.Events;
using WumpusLib.Map.Results;

namespace WumpusLib.Entities;

public class EntityGameMapAccessor
{
    private bool _isLinkedToEntity;


    public EntityGameMapAccessor(GameMap map)
    {
        GameMap = map;
        map.OnGameMapEvent += Map_OnGameMapEvent;
        map.OnEntitySpawned += Map_OnEntitySpawned;
        map.OnGameStarted += Map_OnGameStarted;
        map.OnGameEnded += Map_OnGameEnded;

        RoomLinks = new List<int>();
        NearbyEntities = new List<NearbyEntitySenseInfo>();
        EntitiesInRoom = new List<Entity>();
    }

    public EntityGameMapAccessor(WumpusGame game, GameMap map)
    {
        GameMap = map;

        map.OnGameMapEvent += Map_OnGameMapEvent;
        map.OnEntitySpawned += Map_OnEntitySpawned;
        map.OnGameStarted += Map_OnGameStarted;
        map.OnGameEnded += Map_OnGameEnded;
        map.OnEntityKilled += Map_OnEntityKilled;

        RoomLinks = new List<int>();
        NearbyEntities = new List<NearbyEntitySenseInfo>();
        EntitiesInRoom = new List<Entity>();

        // Assign WumpusGame and subscribe to OnPlayerTurnStarted event
        WumpusGame = game;
        game.OnPlayerTurnStarted += WumpusGame_OnPlayerTurnStarted;
    }

    public int TurnCounter { get; private set; }

    public int CurrentRoom { get; private set; }
    public int AmountOfRooms => GameMap.AmountOfRooms;
    public List<int> RoomLinks { get; private set; }
    public List<Entity> EntitiesInRoom { get; private set; }
    public List<NearbyEntitySenseInfo> NearbyEntities { get; private set; }

    public bool GameStarted => GameMap.GameStarted;

    protected Entity Entity { get; set; }

    protected GameMap GameMap { get; }
    protected WumpusGame WumpusGame { get; }
    public bool IsPlayersTurn => WumpusGame.IsPlayerTurn(Entity as Player.Player);

    public event EventHandler<EventArgs>? OnTurnStarted;

    public bool LinkEntityToAccessor(Entity entity)
    {
        if (_isLinkedToEntity) return false;

        Entity = entity;

        _isLinkedToEntity = true;

        UpdateRoomLinksAndEntities();

        return _isLinkedToEntity;
    }

    public event EventHandler<EventArgs>? OnGameStarted;
    public event EventHandler<EventArgs>? OnGameEnded;

    public event EventHandler<EntityMovedArgs>? OnMove;
    public event EventHandler<EntityDiedArgs>? OnKilled;
    public event EventHandler<EntityDeployArgs>? OnDeployedToMap;
    public event EventHandler<EntityUpdateArgs>? OnRemovedFromMap;

    public event EventHandler<EntityMovedByEntityArgs>? OnMovedByOtherEntity;
    public event EventHandler<EntityDiedArgs>? OnOtherEntityKilled;

    public event EventHandler<EntityUpdateArgs>? OnEntityEnterCurrentRoom;
    public event EventHandler<EntityUpdateArgs>? OnEntityLeaveCurrentRoom;
    public event EventHandler<EntityUpdateArgs>? OnEntityEnterNearbyRoom;
    public event EventHandler<EntityUpdateArgs>? OnEntityLeaveNearbyRoom;

    private void Map_OnGameEnded(object? sender, EventArgs e)
    {
        if (!_isLinkedToEntity)
            return;
        OnGameEnded?.Invoke(this, EventArgs.Empty);
    }

    private void Map_OnGameStarted(object? sender, EventArgs e)
    {
        if (!_isLinkedToEntity)
            return;
        OnGameStarted?.Invoke(this, EventArgs.Empty);
    }

    private void Map_OnGameMapEvent(object? sender, GameMapUpdateArgs e)
    {
        if (!_isLinkedToEntity)
            return;

        //ensure entity updates/game information is updated
        UpdateRoomLinksAndEntities();

        if (e.EntityThatMoved == Entity)
            //if the one that moved was us, see how we moved
            HandleLinkedEntityMoved(e);
        else
            //otherwise check if any entities that moved are related to us
            HandleOtherEntityMoved(e);
    }

    private void Map_OnEntityKilled(object? sender, EntityDiedArgs e)
    {
        if (!_isLinkedToEntity)
            return;

        HandleEntityKilled(e);
    }

    private void HandleLinkedEntityMoved(GameMapUpdateArgs e)
    {
        if (IsEntityMovedByAnother(e))
            OnMovedByOtherEntity?.Invoke(this, new EntityMovedByEntityArgs(e.ControllingEntity, e.EntityThatMoved, e.OriginalRoom, e.NewRoom));

        else if (IsEntityMovedItself(e))
            OnMove?.Invoke(this, new EntityMovedArgs(e.OriginalRoom, e.NewRoom));

        else if (IsEntityDeployedToMap(e))
            OnDeployedToMap?.Invoke(this, new EntityDeployArgs(e.ControllingEntity, e.NewRoom));

        else if (IsEntityRemovedFromMap(e))
            OnRemovedFromMap?.Invoke(this, new EntityUpdateArgs(e.ControllingEntity));
    }

    private void HandleOtherEntityMoved(GameMapUpdateArgs e)
    {
        if (IsEntityEnteredCurrentRoom(e))
            OnEntityEnterCurrentRoom?.Invoke(this, new EntityUpdateArgs(e.EntityThatMoved));

        else if (IsEntityLeftCurrentRoom(e))
            OnEntityLeaveCurrentRoom?.Invoke(this, new EntityUpdateArgs(e.EntityThatMoved));

        else if (IsEntityEnteredNearbyRoom(e))
            OnEntityEnterNearbyRoom?.Invoke(this, new EntityUpdateArgs(e.EntityThatMoved));

        else if (IsEntityLeftNearbyRoom(e))
            OnEntityLeaveNearbyRoom?.Invoke(this, new EntityUpdateArgs(e.EntityThatMoved));
    }

    private void HandleEntityKilled(EntityDiedArgs e)
    {
        if (e.EntityThatDied == Entity)
            OnKilled?.Invoke(this, e);
        else
            OnOtherEntityKilled?.Invoke(this, e);
    }

    private void Map_OnEntitySpawned(object? sender, EntityUpdateArgs e)
    {
        if (e.Entity != Entity)
            return;

        //we were spawned in a map, update information
        UpdateRoomLinksAndEntities();
    }

    public virtual MoveResult MoveTo(int room)
    {
        if (Entity is not Player.Player player) return GameMap.MoveTo(Entity, room);
        return !WumpusGame.IsPlayerTurn(player) ? MoveResult.NotPlayersTurn : GameMap.MoveTo(Entity, room);
    }

    public virtual MoveResult MoveEntityTo(Entity entityToMove, int room)
    {
        return GameMap.MoveEntityTo(Entity, entityToMove, room);
    }

    public virtual MoveResult DeployEquippableEntity(Entity entityToDeploy)
    {
        return GameMap.DeployEntity(Entity, entityToDeploy);
    }

    public virtual EquipResult RemoveEquippableEntity(Entity entityToEquip)
    {
        return GameMap.RemoveEntity(Entity, entityToEquip);
    }

    private void UpdateRoomLinksAndEntities()
    {
        CurrentRoom = GameMap.GetRoomNumberForEntity(Entity);
        RoomLinks = GameMap.GetRoomLinksForEntity(Entity).ToList();

        if (CurrentRoom == 0)
            return;

        NearbyEntities = GameMap.GetNearbyEntitiesAndBroadcastStrength(CurrentRoom);
        EntitiesInRoom = GameMap.GetEntitiesInCurrentRoom(Entity);
    }

    private void WumpusGame_OnPlayerTurnStarted(object? sender, Player.Player e)
    {
        if (e != Entity)
            return;

        TurnCounter++;

        UpdateRoomLinksAndEntities();
        OnTurnStarted?.Invoke(this, EventArgs.Empty);
    }

    // Helper methods for conditions
    private bool IsEntityMovedByAnother(GameMapUpdateArgs e) =>
        e.ControllingEntity != null && e.OriginalRoom != 0 && e.NewRoom != 0;

    private bool IsEntityMovedItself(GameMapUpdateArgs e) =>
        e.ControllingEntity == null && e.OriginalRoom != 0 && e.NewRoom != 0;

    private bool IsEntityDeployedToMap(GameMapUpdateArgs e) =>
        e.EntityThatMoved == Entity && e.ControllingEntity != null && e.OriginalRoom == 0 && e.NewRoom != 0;

    private bool IsEntityRemovedFromMap(GameMapUpdateArgs e) =>
        e.EntityThatMoved == Entity && e.ControllingEntity != null && e.OriginalRoom != 0 && e.NewRoom == 0;

    private bool IsEntityEnteredCurrentRoom(GameMapUpdateArgs e) =>
        e.NewRoom == CurrentRoom;

    private bool IsEntityLeftCurrentRoom(GameMapUpdateArgs e) =>
        e.OriginalRoom == CurrentRoom && e.NewRoom != CurrentRoom;

    private bool IsEntityEnteredNearbyRoom(GameMapUpdateArgs e) =>
        RoomLinks.Contains(e.NewRoom) && !RoomLinks.Contains(e.OriginalRoom) && e.OriginalRoom != CurrentRoom && e.NewRoom != CurrentRoom;

    private bool IsEntityLeftNearbyRoom(GameMapUpdateArgs e) =>
        !RoomLinks.Contains(e.NewRoom) && RoomLinks.Contains(e.OriginalRoom) && e.OriginalRoom != CurrentRoom && e.NewRoom != CurrentRoom;

}