using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Song.Runtime.Support;
using UnityEngine.UIElements;

namespace Song.Tools.Excel
{
    public partial class ExcelView:EditorWindow
    {
        private static Dictionary<string,string[,]> _excelData = new Dictionary<string, string[,]>();
        private static string _sheetName;

        private static GUIStyle _btnSelect;
        private static GUIStyle _btnUnselect;
        
        [MenuItem("Assets/SongTools/ExcelView")]
        public static void ShowExcelView()
        {
            //获取当前选中的文件
            var selectedObject = Selection.activeObject;
            //获取当前选中文件位置
            string assetPath = AssetDatabase.GetAssetPath(selectedObject);
            
            if(!string.IsNullOrWhiteSpace(assetPath))
                _excelData = ExcelSupport.DataSet2DictionaryStringArrayTwo(ExcelSupport.ReadToDataSet(assetPath));
            
            InitGUIStyles();

            if (_excelData.Keys.Count > 0)
            {
                _sheetName = _excelData.Keys.First();
            }

            var window = EditorWindow.GetWindow<ExcelView>();
            window.titleContent = new GUIContent("ExcelView");
            window.Show();
        }

        private void OnGUI()
        {
             Event evt = Event.current;
             if (evt.type == EventType.DragUpdated)//拖动到该窗口上，所每帧执行的事件
             {
                 DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
             }
             else if (evt.type == EventType.DragPerform)//拖动结束事件
             {
                 DragAndDrop.AcceptDrag();
                 foreach (string path in DragAndDrop.paths)// 拖动导入的文件路径
                 {
                     _excelData = ExcelSupport.DataSet2DictionaryStringArrayTwo(ExcelSupport.ReadToDataSet(path));
                     break;
                 }
             }
            
             // 工具栏
             GUILayout.BeginArea(new Rect(10,10,Screen.width,26));
             Tools();
             GUILayout.EndArea();
            
            //显示表格
            GUILayout.BeginArea(new Rect(10,36,Screen.width,Screen.height-98));
            if (!string.IsNullOrWhiteSpace(_sheetName))
            {
                ShowSheet(_excelData[_sheetName]);
            }
            GUILayout.EndArea();
            
            //全部表格
            GUILayout.BeginArea(new Rect(10,Screen.height-72,Screen.width,26));
            SheetNames();
            GUILayout.EndArea();
            
            //提示栏
            GUILayout.BeginArea(new Rect(10,Screen.height-46,Screen.width,26));
            Tip();
            GUILayout.EndArea();
        }
        
        //初始化样式
        private static void InitGUIStyles()
        {
            // 定义_btnSelect的样式
            _btnSelect = new GUIStyle()
            {
                normal =
                {
                    background = MakeTex(new Color(0.3486f, 0.5873f, 0.6944f)), // 天蓝色背景
                    textColor = Color.white,
                },
                alignment = TextAnchor.MiddleCenter
            };
            
            // 定义_btnUnselect的样式
            _btnUnselect = new GUIStyle()
            {
                normal =
                {
                    background = MakeTex(new Color(0.301f, 0.301f, 0.301f)),
                    textColor = Color.white // 字体颜色为黑色
                },
                alignment = TextAnchor.MiddleCenter
            };
        }

        //设置背景
        private static Texture2D MakeTex(Color color) {
            var result = new Texture2D(1,1);
            result.SetPixel(1,1,color);
            result.Apply();
            return result;
        }

    }
}