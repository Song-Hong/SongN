using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using Song.Runtime.Support;

namespace Song.Tools.Excel
{
    public partial class ExcelView:EditorWindow
    {
        private Vector2 _sheetNamesPosition = Vector2.zero;
        
        private void SheetNames()
        {
            _sheetNamesPosition = GUILayout.BeginScrollView(_sheetNamesPosition);
            GUILayout.BeginHorizontal();
            GUILayout.Space(4);
            foreach (var sheetName in _excelData.Keys)
            {
                var style = _btnUnselect;
                if (sheetName == _sheetName) style = _btnSelect;
                if (GUILayout.Button(sheetName,style, GUILayout.Width(60)))
                {
                    _sheetName = sheetName;
                    SetTipDefault();
                }
                GUILayout.Space(6);
            }
            GUILayout.EndHorizontal();
            GUILayout.EndScrollView();
        }
    }
}