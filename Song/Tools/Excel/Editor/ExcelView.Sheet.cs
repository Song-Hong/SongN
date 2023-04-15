using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Song.Tools.Excel
{
    public partial class ExcelView:EditorWindow
    {
        private int _selectR = -2;
        private int _selectC = -2;

        private void Reset(int r = -2, int c = -2)
        {
            _selectR = r;
            _selectC = c;
        }

        /// <summary>
        /// 显示表格数据
        /// </summary>
        /// <param name="sheet"></param>
        private void ShowSheet(List<List<string>> sheet)
        {
            GUILayout.BeginVertical();
            if (sheet.Count > 0)
            {
                GUILayout.BeginHorizontal();
                if(GUILayout.Button("A",GUILayout.Width(26)))
                {
                    Reset(-1, -1);
                }
                for (int j = 0; j  < sheet[0].Count; j++)
                {
                    if (GUILayout.Button((j+1).ToString(), GUILayout.Width(80)))
                    {
                        SetTip("*", (j + 1).ToString(), "*");
                        GUIUtility.keyboardControl = 0;
                        Reset(c:j);
                    }
                }
                GUILayout.EndHorizontal();
            }
            for (var i = 0; i < sheet.Count; i++)
            {
                GUILayout.BeginHorizontal();
                if (GUILayout.Button((i+1).ToString(), GUILayout.Width(26)))
                {
                    SetTip((i+1).ToString(), "*", "*");
                    GUIUtility.keyboardControl = 0;
                    Reset(i);
                }
                for (var j = 0; j < sheet[i].Count; j++)
                {
                    GUI.SetNextControlName("sheet");
                    if (i == _selectR || j == _selectC || (_selectR == -1 && _selectC == -1))
                        sheet[i][j] = GUILayout.TextField(sheet[i][j],_btnSelect, GUILayout.Width(80));
                    else
                        sheet[i][j] = GUILayout.TextField(sheet[i][j], GUILayout.Width(80));
                    // 检查输入框是否获得了焦点
                    if (GUI.GetNameOfFocusedControl() == "sheet")
                    {
                        SetTip((i+1).ToString(), (j+1).ToString(), sheet[i][j]);
                        Reset();
                    }
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();
        }

        
    }
}