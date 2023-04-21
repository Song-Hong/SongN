using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            if (GUILayout.Button(new GUIContent(_save,"保存"),GUILayout.Width(40),GUILayout.Height(20))) Save();
            if (GUILayout.Button(new GUIContent(_saveAs,"另存为"),GUILayout.Width(40),GUILayout.Height(20))) SaveAs();
            if (_selectR > -1)
            {
                if (GUILayout.Button(new GUIContent("行^+"), GUILayout.Width(40), GUILayout.Height(20))) AddRow(_selectR);
                if (GUILayout.Button(new GUIContent("行v+"), GUILayout.Width(40), GUILayout.Height(20))) AddRow(_selectR+1);
                if (GUILayout.Button(new GUIContent("行-"), GUILayout.Width(40), GUILayout.Height(20))) RemoveRow(_selectR);
            }
            else if (_selectC > -1)
            {
                if (GUILayout.Button(new GUIContent("列<+"), GUILayout.Width(40), GUILayout.Height(20))) AddCol(_selectC);
                if (GUILayout.Button(new GUIContent("列>+"), GUILayout.Width(40), GUILayout.Height(20))) AddCol(_selectC+1);
                if (GUILayout.Button(new GUIContent("列-"), GUILayout.Width(40), GUILayout.Height(20))) RemoveCol(_selectC);
            }
            else
            {
                if (GUILayout.Button(new GUIContent("行+"), GUILayout.Width(40), GUILayout.Height(20))) AddRow(-2);
                if (GUILayout.Button(new GUIContent("行-"), GUILayout.Width(40), GUILayout.Height(20))) RemoveRow(-2);
                if (GUILayout.Button(new GUIContent("列+"), GUILayout.Width(40), GUILayout.Height(20))) AddCol(-2);
                if (GUILayout.Button(new GUIContent("列-"), GUILayout.Width(40), GUILayout.Height(20))) RemoveCol(-2);
            }
            if (GUILayout.Button(new GUIContent("[-]", "删除表格"), GUILayout.Width(40), GUILayout.Height(20))) RemoveSheet();
            GUILayout.EndHorizontal();
            GUILayout.EndScrollView();
        }
        
        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="path">存储路径</param>
        private void Save(string path = "")
        {
            if (string.IsNullOrWhiteSpace(path)) return;
            ExcelSupport.Save(path,_excelData);
            ChangeName2Save();
        }
        
        /// <summary>
        /// 保存
        /// </summary>
        private void Save()
        {
            Save(_filePath);
        }
        
        /// <summary>
        /// 存储为
        /// </summary>
        private void SaveAs()
        {
            Save(EditorUtility.SaveFilePanel("存储为","",_fileName,"xlsx"));
        }
        
        /// <summary>
        /// 添加行
        /// </summary>
        /// <param name="row">行数</param>
        private void AddRow(int row = -1)
        {
            var list = _excelData[_sheetName];
            var item = new List<string>();
            for (int i = 0; i < list[0].Count(); i++)
            {
                item.Add("");
            }
            if (row < 0)
            {
                list.Add(item);
                return;
            }
            list.Insert(row,item);
        }

        /// <summary>
        /// 添加列
        /// </summary>
        /// <param name="col">列数</param>
        private void AddCol(int col = -1)
        {
            var list = _excelData[_sheetName];
            if (col < 0)
            {
                foreach (var t in list)
                {
                    t.Add("");
                }
                return;
            }
            foreach (var t in list)
            {
                t.Insert(col,"");
            }
        }

        /// <summary>
        /// 移除行
        /// </summary>
        /// <param name="row">行数</param>
        private void RemoveRow(int row = -1)
        {
            var list = _excelData[_sheetName];
            if (row < 0)
            {
                list.RemoveAt(list.Count()-1);
                return;
            }
            list.RemoveAt(row);
        }

        /// <summary>
        /// 移除列
        /// </summary>
        /// <param name="col">列数</param>
        private void RemoveCol(int col = -1)
        {
            var list = _excelData[_sheetName];
            if (col < 0)
            {
                foreach (var t in list)
                {
                    t.RemoveAt(t.Count()-1);
                }

                return;
            }
            for (var i = 0; i < list.Count; i++)
            {
                list[i].RemoveAt(col);
            }
        }

        /// <summary>
        /// 移除表格
        /// </summary>
        private void RemoveSheet()
        {
            if (_excelData.Count()<=1)
            {
                SetTip("无法移除最后一个表格");
                return;
            }

            _excelData.Remove(_sheetName);
            _sheetName = _excelData.First().Key;
        }
    }
}