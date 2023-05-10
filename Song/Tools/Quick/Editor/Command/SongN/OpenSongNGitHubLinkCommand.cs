using UnityEngine;

namespace Song.Tools.Quick
{
    public class OpenSongNGitHubLinkCommand:IQuickCommand
    {
        public string CommandName()
        {
            return "[@]打开SongN的github界面";
        }

        public string Execute()
        {
            Application.OpenURL("https://github.com/Song-Hong/SongN");
            return "";
        }
    }
}