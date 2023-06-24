namespace WumpusLib.Entities.Player.Bot;

public static class BotSkillLevelExtensions
{
    public static int GetMemoryCapacity(this BotSkillLevel skillLevel)
    {
        return skillLevel switch
        {
            BotSkillLevel.Beginner => 5,
            BotSkillLevel.Intermediate => 10,
            BotSkillLevel.Advanced => 15,
            BotSkillLevel.Perfect => int.MaxValue,
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}