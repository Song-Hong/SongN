using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Song.Runtime.Support;

namespace Song.Tools.Excel
{
    public partial class ExcelView : EditorWindow
    {
        private static Dictionary<string, List<List<string>>> _excelData = new Dictionary<string, List<List<string>>>();
        
        private static GUIStyle _btnSelect;
        private static GUIStyle _btnUnselect;

        private static Texture2D _sai;
        private static Texture2D _logoView;
        
        private static string _logoPath = "Assets/Song/Tools/Excel/Editor/Img/SongExcelL.png";
        private static string _fileName = "ExcelView";
        private static string _filePath = null;
        private static string _sheetName;
        
        private static bool _isNoFile = false;
        
        
        
        [MenuItem("Song/Tools/ExcelView")]
        public static void ShowExcelViewMenu()
        {
            _logoView = AssetDatabase.LoadAssetAtPath<Texture2D>(_logoPath);
            _isNoFile = true;
            GetWindow<ExcelView>().titleContent = new GUIContent("Null File");
            GetWindow<ExcelView>().Show();
        }

        [MenuItem("Assets/SongTools/ExcelView")]
        public static void ShowExcelViewAssets()
        {
            _logoView = AssetDatabase.LoadAssetAtPath<Texture2D>(_logoPath);

            //获取当前选中的文件
            var selectedObject = Selection.activeObject;
            //获取当前选中文件位置
            var assetPath = AssetDatabase.GetAssetPath(selectedObject);

            if (Directory.Exists(assetPath))
            {
                _isNoFile = true;
                GetWindow<ExcelView>().titleContent = new GUIContent("Null File");
                GetWindow<ExcelView>().Show();
                return;
            }

            Init(assetPath);
        }

        private static void Init(string path)
        {
            if(string.IsNullOrWhiteSpace(path)) return;
            
            _excelData = ExcelSupport.DataSet2DictionaryListList(ExcelSupport.ReadToDataSet(path));
            _fileName  = Path.GetFileName(path);
            _filePath  = path;
            _isNoFile  = false;
            
            InitGUIStyles();
            InitFile();
            
            if (_excelData.Keys.Count > 0)
            {
                _sheetName = _excelData.Keys.First();
            }
            
            var window = GetWindow<ExcelView>();
            ChangeName2Save();
            window.Show();
        }

        private void OnGUI()
        {
            var evt = Event.current;
            if (evt.type == EventType.DragUpdated) //拖动到该窗口上，所每帧执行的事件
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
            }
            else if (evt.type == EventType.DragPerform) //拖动结束事件
            {
                DragAndDrop.AcceptDrag();
                foreach (string path in DragAndDrop.paths) // 拖动导入的文件路径
                {
                    if (!path.EndsWith(".xlsx")) continue;
                    Init(path);
                    break;
                }
            }

            if (_isNoFile)
            {
                UnFilePage();
                return;
            }

            // 工具栏
            GUILayout.BeginArea(new Rect(10, 10, Screen.width, 26));
            Tools();
            GUILayout.EndArea();

            //显示表格
            GUILayout.BeginArea(new Rect(10, 36, Screen.width-20, Screen.height - 98));
            if (!string.IsNullOrWhiteSpace(_sheetName))
            {
                ShowSheet(_excelData[_sheetName]);
            }

            GUILayout.EndArea();

            //全部表格
            GUILayout.BeginArea(new Rect(10, Screen.height - 72, Screen.width, 26));
            SheetNames();
            GUILayout.EndArea();

            //提示栏
            GUILayout.BeginArea(new Rect(10, Screen.height - 46, Screen.width, 26));
            Tip();
            GUILayout.EndArea();

            //便捷工具
            AI();
        }

        /// <summary>
        /// 未导入文件显示提示界面
        /// </summary>
        private void UnFilePage()
        {
            GUILayout.BeginVertical();
            GUILayout.FlexibleSpace();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("拖动导入文件,或点击下方按钮导入文件");
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("导入", GUILayout.Width(60), GUILayout.Height(20)))
            {
                string path = EditorUtility.OpenFilePanel("导入excel文件", "", "xlsx");
                Init(path);
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.FlexibleSpace();
            GUILayout.EndVertical();
        }

        //初始化文件
        private static void InitFile()
        {
            _sai = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Song/Editor/Img/SAI.png");
            _save = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Song/Editor/Img/Save.png");
            _saveAs = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Song/Editor/Img/SaveAs.png");
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

            _selectSheetBg = new GUIStyle()
            {
                normal =
                {
                    background = MakeTex(new Color(0.368f, 0.607f, 0.704f)),
                    textColor = Color.white,
                },
                margin = new RectOffset(3,3,4,0)
            };
        }

        /// <summary>
        /// 设置背景
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        private static Texture2D MakeTex(Color color) {
            var result = new Texture2D(1,1);
            result.SetPixel(1,1,color);
            result.Apply();
            return result;
        }

        /// <summary>
        /// 设置数据 不设置为未保存
        /// </summary>
        /// <param name="row">行</param>
        /// <param name="col">列</param>
        /// <param name="data">数据</param>
        public void SetData(int row, int col, string data)
        {
            if (string.IsNullOrWhiteSpace(_sheetName))
                return;

            var datas = _excelData[_sheetName];

            // 检查字典中是否存在键 row
            if (datas.Count < row + 1)
            {
                for (int i = datas.Count; i < row + 1; i++)
                {
                    datas.Add(new List<string>());
                }
            }

            // 获取 row 键的值，即包含所有列的列表
            var rowValues = datas[row];

            // 检查列表中是否存在索引 col
            if (rowValues.Count < col + 1)
            {
                for (int i = rowValues.Count; i < col + 1; i++)
                {
                    rowValues.Add("");
                }
            }

            datas[row][col] = data;
        }

        /// <summary>
        /// 设置数据 设置为未保存
        /// </summary>
        /// <param name="row">行</param>
        /// <param name="col">列</param>
        /// <param name="data">数据</param>
        public void Set(int row, int col, string data)
        {
            SetData(row, col, data);
            ChangeName2UnSave();
        }
        
        /// <summary>
        /// 设置名称为未保存
        /// </summary>
        public static void ChangeName2UnSave() 
            => GetWindow<ExcelView>().titleContent = new GUIContent(_fileName + "*",_logoView);
        
        /// <summary>
        /// 设置名称为已保存
        /// </summary>
        public static void ChangeName2Save() 
            => GetWindow<ExcelView>().titleContent = new GUIContent(_fileName,_logoView);
    }
}