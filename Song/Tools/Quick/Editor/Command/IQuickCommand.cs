namespace Song.Tools.Quick
{
    /// <summary>
    /// 快捷命令
    /// </summary>
    public interface IQuickCommand
    {
        /// <summary>
        /// 命令名称
        /// </summary>
        /// <returns>命令名</returns>
        public string CommandName();
        
        /// <summary>
        /// 执行
        /// </summary>
        /// <param name="item">参数</param>
        /// <returns>执行结果</returns>
        public string Execute();
    }
}