using Song.Tools.Excel;
using UnityEditor;

namespace Song.Tools.Quick.SongExcel
{
    public class OpenSongExcelCommand:IQuickCommand
    {
        public string CommandName()
        {
            return "[^]SongExcel,";
        }

        public string Execute()
        {
            //获取当前选中的文件
            var selectedObject = Selection.activeObject;
            //获取当前选中文件位置
            var assetPath = AssetDatabase.GetAssetPath(selectedObject);

            if (string.IsNullOrWhiteSpace(assetPath))
            {
                ExcelView.ShowExcelViewMenu();
                return "";
            }
            else
            {
                ExcelView.ShowExcelViewAssets();
                return $"打开文件 {assetPath}";    
            }
        }
    }
}