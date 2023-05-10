using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Song.Tools.Quick
{
    public partial class Quick : UnityEditor.EditorWindow
    {
        private static string _logoPath   = "Assets/Song/Tools/Quick/Editor/img/QuickL.png";
        private static string _logoCPath  = "Assets/Song/Tools/Quick/Editor/img/QuickC.png";
        private static string _linkPath   = "Assets/Song/Tools/Quick/Editor/img/Link.png";
        private static string _winodwPath = "Assets/Song/Tools/Quick/Editor/img/Window.png";
        private static string _pediaPath  = "Assets/Song/Tools/Quick/Editor/img/Pedia.png";
        private static string _clearPath  = "Assets/Song/Tools/Quick/Editor/img/Clear.png";
        
        private static Texture2D _logoView;
        private static Texture2D _logoViewC;
        private static Texture2D _linkView;
        private static Texture2D _windowView;
        private static Texture2D _pediaView;
        private static Texture2D _clearView;

        private static GUIStyle _chatBGI;
        private static GUIStyle _commandBGI;
        
        /// <summary>
        /// 输入文本
        /// </summary>
        private string _inputText = "";
        
        [MenuItem("Song/Tools/Quick")]
        public static void ShowQuick()
        {
            InitImg();
            InitStyle();
            
            var wnd = GetWindow<Quick>();
            wnd.titleContent = new GUIContent("Quick",_logoView);
            wnd.Show();
        }

        private void OnGUI()
        {
            //显示聊天
            ShowChats();
            
            //检测回车键按下
            if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Return)
            {
                if (!string.IsNullOrWhiteSpace(_inputText) && !_robotIsWrite)
                {
                    Submit(_inputText);
                }
            }
            
            
            GUILayout.FlexibleSpace();
            
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (_robotIsWrite && GUILayout.Button("停止生成"))
            {
                _stopGenerate?.Invoke();
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(6);
            
            //输入框
            GUILayout.BeginHorizontal();
            GUILayout.Space(10);
            if (GUILayout.Button(new GUIContent(_clearView), GUILayout.Width(30),GUILayout.Height(20)))
            {
                _chats.Clear();   
            }
            _inputText = GUILayout.TextField(_inputText);
            GUILayout.Space(10);
            GUILayout.EndHorizontal();
            GUILayout.Space(20);
        }
        
        /// <summary>
        /// 初始化图片
        /// </summary>
        private static void InitImg()
        {
            _logoView   = AssetDatabase.LoadAssetAtPath<Texture2D>(_logoPath);
            _logoViewC  = AssetDatabase.LoadAssetAtPath<Texture2D>(_logoCPath);
            _linkView   = AssetDatabase.LoadAssetAtPath<Texture2D>(_linkPath);
            _windowView = AssetDatabase.LoadAssetAtPath<Texture2D>(_winodwPath);
            _pediaView  = AssetDatabase.LoadAssetAtPath<Texture2D>(_pediaPath);
            _clearView  = AssetDatabase.LoadAssetAtPath<Texture2D>(_clearPath);
        }
        
        /// <summary>
        /// 初始化样式
        /// </summary>
        private static void InitStyle()
        {
            GUIStyle NewStyle(Color color)
            {
                var texture2D = new Texture2D(1,1);
                texture2D.SetPixel(1,1,color);
                texture2D.Apply();

                var guiStyle = new GUIStyle
                {
                    normal =
                    {
                        background = texture2D,
                        textColor = Color.white
                    }
                };
                return guiStyle;
            };

            _chatBGI = NewStyle(new Color(0.176f, 0.176f, 0.176f));
            _chatBGI.padding = new RectOffset(4, 4, 4, 4);

            _commandBGI = NewStyle(new Color(0.176f, 0.176f, 0.176f));
            _commandBGI.padding = new RectOffset(4, 12, 4, 4);
        }

        /// <summary>
        /// 提交
        /// </summary>
        private void Submit(string message)
        {
            AddMyChat(message);
            _inputTextCache = _inputText;
            _inputText = "";
            GenerateRobot();
        }
    }
}
