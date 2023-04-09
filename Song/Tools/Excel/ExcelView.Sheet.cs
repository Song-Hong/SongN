using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Song.Tools.Excel
{
    public partial class ExcelView:EditorWindow
    {
        private void ShowSheet(string [,] sheet)
        {
            GUILayout.BeginVertical();
            for (var i = 0; i < sheet.GetLength(0); i++)
            {
                GUILayout.BeginHorizontal();
                for (var j = 0; j < sheet.GetLength(1); j++)
                {
                    GUI.SetNextControlName("sheet");
                    sheet[i, j] = GUILayout.TextField(sheet[i, j],GUILayout.Width(80)); // 修改代码

                    // 检查输入框是否获得了焦点
                    if (GUI.GetNameOfFocusedControl() == "sheet")
                    {
                        _col = (i+1).ToString();
                        _row = (j+1).ToString();
                        _data = sheet[i, j];
                    }
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();
        }

    }
}