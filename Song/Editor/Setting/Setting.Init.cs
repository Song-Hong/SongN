using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEditor;
using UnityEngine;

namespace Song.Editor.Settings
{
    public partial class Setting : EditorWindow
    {
        private static Texture2D _logo;
        private static Texture2D _localization;
        private static Texture2D _version;
        private static Texture2D _search;
        private static Texture2D _searchBg;
        private static Texture2D _tag;
        private static Texture2D _plugin;
        private static Texture2D _popup;
        
        [MenuItem("Song/Setting")]
        public static void ShowSetting()
        {
            var wnd = GetWindow<Setting>();
            wnd.titleContent = new GUIContent("设置");
            wnd.minSize = new Vector2(540, 340);
            
            LoadImg();
            InitStyle();
            InitTools();
            
            wnd.Show();
        }

        private static void InitStyle()
        {
            _localizationBg = new GUIStyle()
            {
                normal =
                {
                    background = _localization
                },
            };
            _localizationBtn = new GUIStyle()
            {
                normal =
                {
                    background = _tag,
                    textColor = Color.white
                },
                alignment = TextAnchor.MiddleCenter,
                margin = new RectOffset(2,2,4,4),
            };
            _searchBackground = new GUIStyle()
            {
                normal =
                {
                    background = _searchBg,
                    textColor = Color.white
                },
                alignment = TextAnchor.MiddleCenter,
            };
            _searchBtn = new GUIStyle()
            {
                normal =
                {
                    background = _search,
                },
            };
            _tagsBtn = new GUIStyle()
            {
                normal =
                {
                    background = _tag,
                    textColor = Color.white
                },
                alignment = TextAnchor.MiddleCenter,
                margin = new RectOffset(2,10,0,0),
            };
            _pluginBtn = new GUIStyle()
            {
                normal =
                {
                    background = _tag,
                    textColor = Color.white
                },
                alignment = TextAnchor.MiddleCenter,
                margin = new RectOffset(2,10,0,0),
            };
        }

        private static void LoadImg()
        {
            _logo         = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Song/Editor/Img/Logo.png");
            _localization = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Song/Editor/Img/Localization.png");
            _version      = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Song/Editor/Img/Version.png");
            _search       = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Song/Editor/Img/Search.png");
            _tag          = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Song/Editor/Img/Tag.png");
            _plugin       = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Song/Editor/Img/Plugin.png");
            _searchBg     = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/song/Editor/Img/SearchBG.png");
            _popup        = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/song/Editor/Img/Popup.png");
        }

        /// <summary>
        /// 初始化工具
        /// </summary>
        private static void InitTools()
        {
            _tools = new List<Tuple<string, string, Texture, string, string, string>>();
            foreach (var directory in Directory.GetDirectories("Assets/Song/Tools"))
            {
                var editorConfigXML = directory + "/Editor/config.xml";
                if (!File.Exists(editorConfigXML)) continue;
                
                var doc = new XmlDocument();
                doc.Load(editorConfigXML);
                var root = doc.SelectSingleNode("SongNPlugin");
                if (root != null)
                    _tools.Add(new Tuple<string, string, Texture, string, string, string>(
                        root.SelectSingleNode("PName")?.InnerText,
                        root.SelectSingleNode("Version")?.InnerText,
                        AssetDatabase.LoadAssetAtPath<Texture2D>(root.SelectSingleNode("Logo")?.InnerText),
                        root.SelectSingleNode("Author")?.InnerText,
                        root.SelectSingleNode("License")?.InnerText,
                        root.SelectSingleNode("Setting")?.InnerText
                    ));
            }
        }

        private void OnGUI () 
        {
            Logo();
            Localization();
            Version();
            Search();
            Tags();
            Plugins();
            LocalizationPopup();
        }
    }
}
