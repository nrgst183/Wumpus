namespace WumpusLib.Map.Results;

public enum MoveResult
{
    Success,
    EntityAlreadyOnMap,
    EntityNotOnMap,
    EntityNotInRoom,
    EntityCannotMoveOtherEntities,
    EntityNotAllowedToMove,
    CannotMoveToSameRoom,
    NoCompatibleRoomsToMoveTo,
    InvalidRoomNumber,
    NotPlayersTurn,
    IllegalMove
}