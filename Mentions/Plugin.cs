#region Using

using System.Text.RegularExpressions;

using Terraria;
using TerrariaApi.Server;

using TShockAPI;
using TShockAPI.Hooks;
using TShockAPI.Configuration;

#endregion

namespace Mentions
{
    [ApiVersion(2, 1)]
    public class MentionsPlugin : TerrariaPlugin
    {
        #region Data

        public override string Author => "Zoom L1";
        public override string Name => "Mentions";
        public override Version Version => new Version(1, 0, 0, 0);
        public MentionsPlugin(Main game) : base(game) { /* base.Order = 1; */ }

        public static ConfigFile<ConfigSettings> Config = new ConfigFile<ConfigSettings>();
        public static readonly Regex mention = new Regex(@"@(?<prefix>\W{0,3})?(?<text>""(?<exact>[^""]+)""|\S+)");

        public static event Action<MentionedHandledEventArgs>? Mentioned;

        #endregion
        #region Initialize

        public override void Initialize()
        {
            OnReload(new ReloadEventArgs(null));
            GeneralHooks.ReloadEvent += OnReload;
            PlayerHooks.PlayerChat += OnPlayerChat;
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                GeneralHooks.ReloadEvent -= OnReload;
                PlayerHooks.PlayerChat -= OnPlayerChat;
            }
            base.Dispose(disposing);
        }

        #endregion

        #region OnPlayerChat

        public void OnPlayerChat(PlayerChatEventArgs args)
        {
            if (args.Handled || !args.Player.HasPermission("mentions.canping"))
                return;

            string text = args.RawText;
            string formattedText = args.TShockFormattedText.Substring(0, args.TShockFormattedText.Length - text.Length);

            List<TSPlayer> mentioned = new List<TSPlayer>();

            Tools.ReplaceText(mention, ref text, (match) =>
            {
                System.Text.RegularExpressions.Group group = match.Groups["exact"];
                string name = match.Groups["text"].Value;
                if (group.Success)
                    name = group.Value;
                else
                {
                    while (char.IsPunctuation(name.Last()) && name.Length > 1)
                        name = name.Remove(name.Length - 1, 1);
                }

                name = name.Replace("\"", "").Replace("\\", "");

                string? result = null;
                TSPlayer? player = TSPlayer.FindByNameOrID(name).FirstOrDefault();
                if (player != null)
                {
                    result = $"[c/{Config.Settings.HEX}:@{player.Name}]";
                    mentioned.Add(player);
                }

                MentionedHandledEventArgs e = new MentionedHandledEventArgs(args.Player, match, name, mentioned);
                Mentioned?.Invoke(e);
                if (e.Handled)
                    result = e.Result;

                return result ?? match.ToString();
            });

            args.RawText = text;
            args.TShockFormattedText = formattedText + text;
        }

        #endregion

        #region OnReload

        public void OnReload(ReloadEventArgs args)
        {
            string path = Path.Combine(TShock.SavePath, Name + ".json");
            Config = new ConfigFile<ConfigSettings>();
            Config.Read(path, out bool write);
            if (write)
                Config.Write(path);

            args.Player?.SendSuccessMessage("["+ Name + "] Reloaded.");
        }

        #endregion
    }
}