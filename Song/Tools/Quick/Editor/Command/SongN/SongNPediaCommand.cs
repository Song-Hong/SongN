namespace Song.Tools.Quick
{
    public class SongNPediaCommand:IQuickCommand
    {
        public string CommandName()
        {
            return "[%]显示SongN介绍";
        }

        public string Execute()
        {
            return "SongN介绍\n" +
                   "SongN是一款基于Unity Mono和UGUI的工具集\n" +
                   "其中包含了许多插件\n" +
                   "例如:SongExcel,可以帮助开发者快速部署本地化\n" +
                   "SongQuick,可以帮助开发者快速执行一些操作\n";
        }
    }
}