using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
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
            var sheetN = _sheetName;
            foreach (var sheetName in _excelData.Keys)
            {
                // var style = _btnUnselect;
                // if (sheetName == sheetN) style = _btnSelect;
                if (sheetName == sheetN)
                {
                    sheetN = GUILayout.TextField(sheetN,_btnSelect, GUILayout.Width(60));
                    GUILayout.Space(5);
                    continue;
                }
                if (GUILayout.Button(sheetName,_btnUnselect, GUILayout.Width(60)))
                {
                    _sheetName = sheetName;
                    SetTipDefault();
                }
                GUILayout.Space(6);
            }
            if (GUILayout.Button("+",_btnUnselect,GUILayout.Width(60)))
            {
                if(_excelData.ContainsKey("New")) return;
                var sheet = new List<List<string>>();
                _excelData.Add("New",sheet);
            }
            GUILayout.EndHorizontal();
            GUILayout.EndScrollView();

            if (sheetN != _sheetName)
            {
                if(_excelData.ContainsKey(sheetN)) return;
                var list = new List<List<string>>();
                foreach (var value in _excelData[_sheetName])
                {
                    list.Add(value);
                }
                _excelData.Remove(_sheetName);
                _excelData.Add(sheetN,list);
                _sheetName = sheetN;
            }
        }
    }
}