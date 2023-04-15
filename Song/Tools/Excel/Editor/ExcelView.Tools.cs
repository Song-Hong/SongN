using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using Song.Runtime.Support;

namespace Song.Tools.Excel
{
    public partial class ExcelView:EditorWindow
    {
        private Vector2 _toolsPosition = Vector2.zero;

        private static Texture2D _save;
        private static Texture2D _saveAs;
        
        private void Tools()
        {
            _toolsPosition = GUILayout.BeginScrollView(_toolsPosition);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button(new GUIContent(_save,"保存"),GUILayout.Width(40),GUILayout.Height(20))) { Save();}
            if (GUILayout.Button(new GUIContent(_saveAs,"另存为"),GUILayout.Width(40),GUILayout.Height(20))) {SaveAs();}
            GUILayout.EndHorizontal();
            GUILayout.EndScrollView();
        }

        private void Save(string path = "")
        {
            if (string.IsNullOrWhiteSpace(path)) return;
            ExcelSupport.Save(path,_excelData);
            ChangeName2Save();
        }

        private void Save()
        {
            Save(_filePath);
        }
        
        private void SaveAs()
        {
            Save(EditorUtility.SaveFilePanel("存储为","",_fileName,"xlsx"));
        }
        
    }
}