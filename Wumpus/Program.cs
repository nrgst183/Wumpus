using System;
using WumpusLib.Entities.Player;
using WumpusLib.Game;
using WumpusLib.Map;
using WumpusLib.Text;
using System.Threading;
using WumpusLib.Entities.Player.Bot;
using WumpusLib.Game.GameModes;

namespace Wumpus;

internal class Program
{
    public static void Main(string[] args)
    {
        Console.Clear();

        var map = new DodecahedronGameMap(20);

        var gameModeOptions = new GameModeOptions
        {
            AmountOfStartingArrows = 3,
            IsAmountOfLivesBasedGameType = true
        };

        var game = new WumpusGame(map, new ClassicGameMode(gameModeOptions, 20));

        Console.Write("Choose player type (regular or bot): ");
        string playerType = Console.ReadLine().ToLower();

        if (playerType == "regular")
        {
            game.CreateAndAddNewPlayer("Player");
            //RunGameLoop(game);
        }
        else if (playerType == "bot")
        {
            game.CreateAndAddNewPlayer("BotPlayer", BotSkillLevel.Perfect);
            RunBotGameLoop(game);
        }
        else
        {
            Console.WriteLine("Invalid player type. Please enter 'regular' or 'bot'.");
            return;
        }

        var killInfo = game.EntityKills.FirstOrDefault();

        Console.WriteLine(killInfo?.EntityThatKilled is Player
            ? EntityGameTextStrings.GetKilledText(killInfo.EntityThatWasKilled.GetType())
            : EntityGameTextStrings.GetKilledByText(killInfo.EntityThatKilled.GetType()));

    }

    private static void RunBotGameLoop(WumpusGame game)
    {
        game.StartGame();

        /*

        while (!game.IsGameEnded)
        {
            var player = game.Players.Values.First() as BotPlayer;

            var roomNumber = game.GameMap.GetRoomNumberForEntity(player);
            var roomLinks = game.GameMap.GetRoomLinksForEntity(player);

            if (player.NearbyEntities.Any())
            {
                foreach (var entity in player.NearbyEntities)
                {
                    Console.WriteLine($"{EntityGameTextStrings.GetNearbyText(entity.Entity)}");
                }
            }

            Console.WriteLine($"Bot player is in room {roomNumber} of the cave");
            Console.WriteLine($"The tunnels in front of the bot player lead to {string.Join(", ", roomLinks)}");
            Console.WriteLine($"Arrows left: {player.AmountOfArrows}");

            var action = player.DoBotThinkingAndActions();

            switch (action.Action)
            {
                case PlayerActionType.Move:
                    player.Move(action.Room);
                    break;
                case PlayerActionType.Shoot:
                    player.Shoot(action.Room);
                    break;
                default:
                    Console.WriteLine("Invalid action. Please enter M or S.");
                    break;
            }

            // Wait a short amount of time before the next move
            Thread.Sleep(1000);
        }
        */
    }
}
