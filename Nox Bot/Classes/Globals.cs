namespace Nox_Bot.Classes
{
    public static class Globals
    {

        public static string twitchChannelId;

        static Globals()
        {
            var c = "noxscourge";
            var t = TwitchLib.TwitchAPI.Users.v5.GetUserByNameAsync(c);
            var t2 = t.Result;
            var t3 = t2.Matches;
            var t4 = t3[0];
            var t5 = t4.Id;
            twitchChannelId = t5;
        }
    }
}
