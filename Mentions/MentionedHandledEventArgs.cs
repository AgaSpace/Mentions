#region Using

using System.ComponentModel;
using System.Text.RegularExpressions;

using TShockAPI;

#endregion

namespace Mentions
{
    public class MentionedHandledEventArgs : HandledEventArgs
    {
        #region Data

        public readonly TSPlayer Author;
        public readonly Match Match;

        public readonly string Name;
        public readonly List<TSPlayer> Mentioned;

        public string? Result;

        #endregion
        #region Constructor

        public MentionedHandledEventArgs(TSPlayer author, Match match, string name, List<TSPlayer> mentioned)
        {
            Author = author;
            Match = match;
            Name = name;
            Mentioned = mentioned;
        }

        #endregion
    }
}
