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
        
        private void Tools()
        {
            _toolsPosition = GUILayout.BeginScrollView(_toolsPosition);
            GUILayout.BeginHorizontal();
            GUILayout.Space(4);
            GUILayout.EndHorizontal();
            GUILayout.EndScrollView();
        }
    }
}