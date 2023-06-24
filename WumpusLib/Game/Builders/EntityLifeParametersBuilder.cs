namespace WumpusLib.Game.Builders;

public class EntityLifeParametersBuilder
{
    private bool _canRespawn;
    private int _lives;
    private int _livesBeforeNoRespawn;
    private bool _shouldEndGame;
    private Type _type;

    public EntityLifeParametersBuilder ForEntity(Type type)
    {
        _type = type;
        return this;
    }

    public EntityLifeParametersBuilder SetLives(int lives)
    {
        _lives = lives;
        return this;
    }

    public EntityLifeParametersBuilder Respawn(int livesBeforeNoRespawn)
    {
        _canRespawn = true;
        _livesBeforeNoRespawn = livesBeforeNoRespawn;
        return this;
    }

    public EntityLifeParametersBuilder ImmediatelyEndGame()
    {
        _shouldEndGame = true;
        return this;
    }

    public void BuildAndAddTo(List<EntityLifeParameters> targetList)
    {
        var parameters = new EntityLifeParameters(_type, _lives, _shouldEndGame, _canRespawn, _livesBeforeNoRespawn);
        targetList.Add(parameters);
    }
}