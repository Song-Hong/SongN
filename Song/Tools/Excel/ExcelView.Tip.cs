using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using Song.Runtime.Support;

namespace Song.Tools.Excel
{
    public partial class ExcelView:EditorWindow
    {
        private string _col  = "*";
        private string _row  = "*";
        private string _data = "*";
        
        private void Tip()
        {
            GUILayout.Label($"{_row}行  {_col}列  {_data}");
        }
        
        //设置为默认值
        private void SetTipDefault()
        {
            _col  = "*";
            _row  = "*";
            _data = "*";
        }
    }
}