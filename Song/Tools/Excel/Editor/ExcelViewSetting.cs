using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Song.Tools.Excel
{
    //设置界面
    public class ExcelViewSetting:EditorWindow
    {
        public static void Setting()
        {
            var excelViewSetting = GetWindow<ExcelViewSetting>();
            excelViewSetting.titleContent = new GUIContent("SongExcelView设置");
            excelViewSetting.Show();
        }

        private void OnGUI()
        {
            
        }
    }
}
