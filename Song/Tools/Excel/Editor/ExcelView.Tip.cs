using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using Song.Runtime.Support;

namespace Song.Tools.Excel
{
    public partial class ExcelView:EditorWindow
    {
        private string _tip  = "";
        private bool _isAuto = false;
        private Action _isAutoButtonClick;
        
        private void Tip()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(_tip);
            GUILayout.Space(10);
            Automatic();
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }
        
        /// <summary>
        /// 设置提示
        /// </summary>
        /// <param name="tip"></param>
        private void SetTip(string tip) => _tip = tip;
        
        /// <summary>
        /// 设置提示数据
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <param name="data"></param>
        private void SetTip(string row,string col,string data)
        {
            _tip = $"{row}行 {col}列 {data}";
        }
        
        /// <summary>
        /// 设置默认值
        /// </summary>
        private void SetTipDefault()
        {
            _tip = "*行 *列 *";
        }
        
        /// <summary>
        /// 自动化
        /// </summary>
        private void Automatic()
        {
            if(!_isAuto) return;
            if (GUILayout.Button("取消",GUILayout.Width(60)))
            {
                _isAuto = false;
                _tip = "";
                _isAutoButtonClick?.Invoke();
            }
        }
    }
}