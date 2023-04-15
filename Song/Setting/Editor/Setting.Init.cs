using System;
using System.Collections;
using System.Collections.Generic;
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

        private static Texture2D test;
        
        [MenuItem("Song/Setting")]
        public static void ShowSetting()
        {
            var wnd = GetWindow<Setting>();
            wnd.titleContent = new GUIContent("设置");
            wnd.minSize = new Vector2(540, 340);
            
            LoadImg();
            InitStyle();
            
            wnd.Show();
        }

        private static void InitStyle()
        {
            // Texture2D SetTex(Color color)
            // {
            //     var tex = new Texture2D(1, 1);
            //     tex.SetPixel(1,1,color);
            //     tex.Apply();
            //     return tex;
            // }

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
            _logo         = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Song/Img/Logo.png");
            _localization = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Song/Img/Localization.png");
            _version      = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Song/Img/Version.png");
            _search       = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Song/Img/Search.png");
            _tag          = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Song/Img/Tag.png");
            _plugin       = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Song/Img/Plugin.png");
            _searchBg     = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/song/Img/SearchBG.png");
            _popup         = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/song/Img/Popup.png");

            test = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Song/Tools/Excel/Editor/Img/SongExcel.png");
        }
        
        void OnGUI () {
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
