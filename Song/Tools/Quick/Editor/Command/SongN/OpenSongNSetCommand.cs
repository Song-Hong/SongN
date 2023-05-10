using UnityEditor;
using Song.Editor.Settings;
using UnityEngine;

namespace Song.Tools.Quick
{
    public class OpenSongNSetCommand:IQuickCommand
    {
        public string CommandName()
        {
            return "[^]打开SongN设置界面";
        }

        public string Execute()
        {
            Setting.ShowSetting();
            return "";
        }
    }
}