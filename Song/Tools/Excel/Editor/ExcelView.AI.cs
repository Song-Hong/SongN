using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Song.Runtime.Support;
using System.Threading;
using NUnit.Framework;
using Song.Editor.ToolsSuppot;
using Debug = UnityEngine.Debug;

namespace Song.Tools.Excel
{
    public partial class ExcelView:EditorWindow
    {
        /// <summary>
        /// python 翻译文件路径
        /// </summary>
        private string _pythonTranslation = "Assets/Song/Tools/Excel/Editor/Py/translate.py";
        private static string _langPath = "Assets/Song/Tools/Excel/Editor/lang.xlsx";
        
        ///是否显示ai操作界面
        private bool _isShowAI;
        
        //AI
        private void AI()
        {
            if (_isShowAI)
            {
                ShowAI();
            }
            if(GUI.Button(new Rect(Screen.width-50,Screen.height-70,30,30),_sai))
            {
                _isShowAI = !_isShowAI;
            }
        }
        
        //显示AI
        private void ShowAI()
        {
            GUILayout.BeginArea(new Rect(Screen.width - 240, Screen.height - 360, 200, 300), _btnUnselect);
            if (GUILayout.Button("语言转换")) { LanguageConversion(); }
            GUILayout.EndArea();
        }
        
        //文字转换
        private void LanguageConversion()
        {
            _isAuto = true;
            _isShowAI = false;
            string text,dest = "";
            var dests = new List<string>();
            var destTips = new List<string>();
            GUIUtility.keyboardControl = 0;
            if (File.Exists(_langPath))
            {
                SetTip("正在进行初始化配置");
                var langData = ExcelSupport.DataSet2DictionaryListList(ExcelSupport.ReadToDataSet(_langPath));
                List<List<string>> list = null;
                if (langData != null)
                {
                    list = langData["CodeList"];
                } 
                
                int Search(string search)
                {
                    for (var i = 1; i < list.Count; i++)
                    {
                        for (var j = 1; j < list[i].Count; j++)
                        {
                            var value = list[i][j];
                            if(string.IsNullOrWhiteSpace(value)) continue;
                            if (string.CompareOrdinal(value, search) == 0)
                                return i;
                        }
                    }
                    return -1;
                }

                var langTList = _excelData[_sheetName];
                for (var i = 1; i < langTList[0].Count; i++)
                {
                    var search = langTList[0][i];
                    var r = Search(search);
                    if (r > 0)
                    {
                        destTips.Add(search);
                        dests.Add(list[r][0]);
                    }
                }
            }
            
            var syncContext = SynchronizationContext.Current;
            var count = _excelData[_sheetName].Count;
            SetTip("开始翻译");
            var thread = new Thread(delegate(object o)
            {
                for (int i = 1; i < count; i++)
                {
                    text = _excelData[_sheetName][i][0];
                    if (string.IsNullOrWhiteSpace(text))
                    {
                        _tip = $"{i},{0}为空";
                        continue;
                    }
                    var tryIndex = 0;
                    for (int j = 0; j < dests.Count; j++)
                    {
                        dest = dests[j];
                        var data = new PythonRunner().Run(
                            _pythonTranslation,
                            $"{text} {dest}").Trim();
                        if (string.IsNullOrWhiteSpace(data) && tryIndex<2)
                        {
                            tryIndex++;
                            j--;
                        }
                        syncContext.Post(state =>
                        {
                            if (string.IsNullOrWhiteSpace(data) && tryIndex<2)
                            {
                                SetTip($"\"{text}\" 翻译为 [{destTips[j]}] 翻译失败,正在重新尝试[{tryIndex}]");
                            }
                            else if (string.IsNullOrWhiteSpace(data) && tryIndex > 2)
                            {
                                SetTip($"\"{text}\" 翻译为 [{destTips[j]}] 翻译失败,已跳过");
                            }
                            else
                            {
                                GUIUtility.keyboardControl = 0;
                                SetTip($"\"{text}\" 翻译为 [{destTips[j]}] \"{data}\" 已完成. 设置至{i}行{j}列");
                                Set(i,j+1,data);
                            }
                        }, null);
                        Thread.Sleep(120);
                    }
                }
                _tip = "翻译完成";
                _isAuto = false;
            });
            thread.Start();
            _isAutoButtonClick += delegate
            {
                SetTip($"已取消翻译");
                thread.Abort();
            };
        }
    }
}