using System.Text.RegularExpressions;

namespace Mentions
{
    public static class Tools
    {
        #region ReplaceText

        public static int ReplaceText(Regex regex, ref string text, Func<Match, string>? formatting = null)
        {
            MatchCollection collection = regex.Matches(text);
            foreach (Match match in collection.Reverse())
            {
                text = text.Remove(match.Index, match.Length);
                if (formatting != null)
                    text = text.Insert(match.Index, formatting(match));
            }

            return collection.Count;
        }
        #endregion
    }
}

