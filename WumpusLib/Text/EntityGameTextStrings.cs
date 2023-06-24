using WumpusLib.Entities;

namespace WumpusLib.Text;

public static class EntityGameTextStrings
{
    public static readonly Dictionary<Type, GameTextString> GameTextStrings = new()
    {
        {
            typeof(SuperBats), new GameTextString
            {
                NearbyText = "*rustle* *rustle* (must be bats nearby)",
                WalkedIntoRoomText = "*flap*  *flap*  *flap*  (humongous bats pick you up and move you!"
            }
        },
        {
            typeof(Wumpus), new GameTextString
            {
                NearbyText = "*sniff* (I can smell the evil Wumpus nearby!)",
                WalkedIntoRoomText =
                    "*ROAR* *chomp* *snurfle* *chomp*!\\n\\\r\nMuch to the delight of the Wumpus, you walked right into his mouth,\\n\\\r\nmaking you one of the easiest dinners he's ever had!  For you, however,\\n\\\r\nit's a rather unpleasant death.  The only good thing is that it's been\\n\\\r\nso long since the evil Wumpus cleaned his teeth that you immediately\\n\\\r\npassed out from the stench!",
                KilledText =
                    "*thwock!* *groan* *crash*\\n\\n\\\r\nA horrible roar fills the cave, and you realize, with a smile, that you\\n\\\r\nhave slain the evil Wumpus and won the game!  You don't want to tarry for\\n\\\r\nlong, however, because not only is the Wumpus famous, but the stench of\\n\\\r\ndead Wumpus is also quite well known, a stench plenty enough to slay the\\n\\\r\nmightiest adventurer at a single whiff!!"
            }
        },
        {
            typeof(BottomlessPit), new GameTextString
            {
                NearbyText = "*whoosh* (I feel a draft from some pits).",
                DiedToText =
                    "*AAAUUUUGGGGGHHHHHhhhhhhhhhh...*\\n\\\r\nThe whistling sound and updraft as you walked into this room of the\\n\\\r\ncave apparently wasn't enough to clue you in to the presence of the\\n\\\r\nbottomless pit.  You have a lot of time to reflect on this error as\\n\\\r\nyou fall many miles to the core of the earth.  Look on the bright side;\\n\\\r\nyou can at least find out if Jules Verne was right..."
            }
        }
    };

    public static string GetKilledText(Type entityType)
    {
        return GameTextStrings.FirstOrDefault(x => x.Key == entityType).Value.KilledText;
    }

    public static string GetKilledByText(Type entityType)
    {
        return GameTextStrings.FirstOrDefault(x => x.Key == entityType).Value.DiedToText;
    }

    public static string GetWalkedIntoRoomText(Type entityType)
    {
        return GameTextStrings.FirstOrDefault(x => x.Key == entityType).Value.WalkedIntoRoomText;
    }

    public static string GetNearbyText(Type entityType)
    {
        return GameTextStrings.FirstOrDefault(x => x.Key == entityType).Value.NearbyText;
    }
}