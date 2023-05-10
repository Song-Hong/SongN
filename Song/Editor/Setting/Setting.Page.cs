using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Song.Editor.Settings
{
    public partial class Setting : EditorWindow
    {
        private static GUIStyle _localizationBtn;
        private static GUIStyle _localizationBg;
        private static GUIStyle _searchBackground;
        private static GUIStyle _searchBtn;
        private static GUIStyle _tagsBtn;
        private static GUIStyle _pluginBtn;
        
        private int _selectedIndex = 0;
        
        private bool _showPopup = false;
        
        private Vector2 _locationPosition = Vector2.zero;
        private Vector2 _tagsPosition = Vector2.zero;
        private Vector2 _pluginsPosition = Vector2.zero;
        
        private string _searchInfo;
        
        private string[] _options = { "中文(简体)", "中文(繁体)", "English"};
        private List<string> _tags = new List<string>();
        
        /// <summary>
        /// 配置文件设置信息
        /// </summary>
        private static List<Tuple<string, string, Texture, string, string, string>> _tools;

        /// <summary>
        /// 当前筛选的标签
        /// </summary>
        private string nowSelectTag = "";
        
        private void Logo()
        {
            GUI.Label(new Rect(26,26,96,96),_logo);
        }

        private void Localization()
        {
            if (GUI.Button(new Rect(138, 34, 120, 34),"", _localizationBg))
            {
                _showPopup = !_showPopup;
            }
            GUI.Label(new Rect(172, 34, 86, 34),_options[_selectedIndex]);
        }

        private void LocalizationPopup()
        {
            if (!_showPopup) return;
            GUILayout.BeginArea(new Rect(138, 62, 120, 125), _popup);
            _locationPosition = GUILayout.BeginScrollView(_locationPosition);
            for (int i = 0; i < _options.Length; i++)
            {
                if (GUILayout.Button(_options[i],_localizationBtn))
                {
                    _selectedIndex = i;
                    _showPopup = false;
                }
            }
            GUILayout.EndScrollView();
            GUILayout.EndArea();
        }

        private void Version()
        {
            GUI.Label(new Rect(138,81,120,34),_version);
            GUI.Label(new Rect(172, 81, 86, 34),"0.00.01P");
        }
        
        private void Search()
        {
            _searchInfo = GUI.TextField(new Rect(29,134,172,24),
                _searchInfo,_searchBackground);

            if (GUI.Button(new Rect(207, 134, 24, 24),"",_searchBtn))
            {
                Debug.Log("search btn clicked");
            }
        }

        private void Tags()
        {
            GUILayout.BeginArea(new Rect(243, 135, Screen.width - 243, 22));
            _tagsPosition = GUILayout.BeginScrollView(_tagsPosition);
            GUILayout.BeginHorizontal();
            foreach (var tag in _tags)
            {
                if (GUILayout.Button(tag,_tagsBtn,GUILayout.Width(42),GUILayout.Height(20)))
                {
                    nowSelectTag = tag;
                }
            }
            GUILayout.EndHorizontal();
            GUILayout.EndScrollView();
            GUILayout.EndArea();
        }

        private void Plugins()
        {
            GUILayout.BeginArea(new Rect(34, 176, Screen.width - 34, Screen.height - 176));
            _pluginsPosition = GUILayout.BeginScrollView(_pluginsPosition, GUILayout.ExpandHeight(true),
                GUILayout.ExpandWidth(true));
            
            var buttonsPerRow = Mathf.FloorToInt((Screen.width - 34) / 70f)-2;
            var buttonIndex = 0;

            var startindex = 0;
            var endindex = 0;
            
            foreach (var tuple in _tools)
            {
                if (buttonIndex == 0)
                {
                    GUILayout.BeginHorizontal();
                    startindex++;
                }

                if (!string.IsNullOrWhiteSpace(nowSelectTag) && nowSelectTag != tuple.Item4) continue;
                GUILayout.BeginVertical();
                if (GUILayout.Button(new GUIContent(tuple.Item3), _pluginBtn, GUILayout.Width(64),
                        GUILayout.Height(64)))
                {
                    if (string.IsNullOrWhiteSpace(tuple.Item6)) return;
                    var type = Type.GetType(tuple.Item6);
                    var instance = ScriptableObject.CreateInstance(type);
                    var method = type.GetMethod("Setting");
                    method.Invoke(instance, null);
        
                }
                GUILayout.Label(tuple.Item1);
                GUILayout.Space(6);
                GUILayout.EndVertical();

                buttonIndex++;
                if (buttonIndex <= buttonsPerRow) continue;
                buttonIndex = 0;
                endindex++;
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }

            if (endindex < startindex)
            {
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }

            GUILayout.EndScrollView();
            GUILayout.EndArea();
        }
    }
}