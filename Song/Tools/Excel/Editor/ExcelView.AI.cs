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
        private static string _langOutputPath = "Assets/StreamingAssets/Config/Lang/";
        
        ///是否显示ai操作界面
        private bool _isShowAI;

        /// <summary>
        /// 是否正在运行中
        /// </summary>
        private bool _isRuntimeAI = false;
        
        /// <summary>
        /// 显示图标
        /// </summary>
        private void AI()
        {
            if(_isRuntimeAI) return;
            if (_isShowAI)
            {
                ShowAI();
            }
            if(GUI.Button(new Rect(Screen.width-50,Screen.height-70,30,30),_sai))
            {
                _isShowAI = !_isShowAI;
            }
        }
        
        /// <summary>
        /// 显示工具界面
        /// </summary>
        private void ShowAI()
        {
            GUILayout.BeginArea(new Rect(Screen.width - 240, Screen.height - 360, 200, 300), _btnUnselect);
            GUILayout.Space(4);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("文本翻译工具");
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            if (GUILayout.Button("全表格翻译")) { LanguageConversionAll();}
            if(_selectR > 0 && _selectC == -2)
            {
                if (GUILayout.Button($"翻译第{_selectR+1}行")) { LanguageConversionRow(_selectR);}   
            }
            if (_selectR == -2 && _selectC > 1)
            {
                if (GUILayout.Button($"翻译第{_selectC+1}列")) { LanguageConversionCol(_selectC); }
            }
            
            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("语言转换工具");
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            if (GUILayout.Button("存储为语言文件")) { Convert2Lang(); }
            if (_selectR == -2 && _selectC > 0)
            {
                if (GUILayout.Button($"存储第{_selectC+1}列为语言文件")) { Convert2LangCol(_selectC); }
            }

            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("快速填充");
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            if (GUILayout.Button("填充第1列的编号")) { FileCode(); }
            GUILayout.EndArea();
        }

        /// <summary>
        /// 表格文本翻译初始化
        /// </summary>
        /// <param name="text">文本</param>
        /// <param name="dest">目标语言代码</param>
        /// <param name="dests">需要目标语言代码组</param>
        /// <param name="destTips">目标语言名称组</param>
        /// <param name="syncContext">上下文</param>
        private bool LanguageConversionInit(out string text,out string dest,
            out List<string> dests,out List<string> destTips,out List<List<string>> datas,out SynchronizationContext syncContext)
        {
            datas = _excelData[_sheetName];
            _isAuto = true;
            _isShowAI = false;
            _isRuntimeAI = true;
            text = "";
            dest = "";
            Reset();
            dests = new List<string>();
            destTips = new List<string>();
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
            syncContext = SynchronizationContext.Current;
            if (dests.Any()) return true;
            _isAuto = false;
            _isRuntimeAI = false;
            SetTip("无法找到可翻译目标语言");
            return false;
        }

        /// <summary>
        /// 转换整个表格的数据
        /// </summary>
        private void LanguageConversionAll()
        {
            var _isOk = LanguageConversionInit(out string text, out string dest, out List<string> dests,
                out List<string> destTips,out List<List<string>> datas,out SynchronizationContext syncContext);
            if(!_isOk) return;
            var count = datas.Count;
            SetTip("开始翻译");
            var thread = new Thread(delegate(object o)
            {
                for (int i = 1; i < count; i++)
                {
                    text = datas[i][1];
                    if (string.IsNullOrWhiteSpace(text))
                    {
                        _tip = $"{i},{0}为空";
                        continue;
                    }
                    for (int j = 1; j < dests.Count; j++)
                    {
                        var tryIndex = 0;
                        while (tryIndex<3)
                        {
                            dest = dests[j];
                            var data = new PythonRunner().Run(
                                _pythonTranslation,
                                $"{text} {dest}").Trim();
                            var isNull = string.IsNullOrWhiteSpace(data);
                            if (isNull) tryIndex++;
                            syncContext.Post(state =>
                            {
                                if (isNull && tryIndex<2)
                                {
                                    SetTip($"\"{text}\" 翻译为 [{destTips[j]}] 翻译失败,正在重新尝试[{tryIndex}]");
                                }
                                else if (isNull && tryIndex > 2)
                                {
                                    SetTip($"\"{text}\" 翻译为 [{destTips[j]}] 翻译失败,已跳过");
                                }
                                else
                                {
                                    SetTip($"\"{text}\" 翻译为 [{destTips[j]}] \"{data}\" 已完成. 设置至{i+1}行{j+2}列");
                                    Set(i,j+1,data);
                                }
                            }, null);
                            Thread.Sleep(120);
                            if(!isNull) break;
                        }
                    }
                }
                _tip = "翻译完成";
                _isAuto = false;
                _isRuntimeAI = false;
            });
            thread.Start();
            _isAutoButtonClick -= Click;
            void Click()
            {
                SetTip($"已取消翻译");
                thread.Abort();
                _isAutoButtonClick -= Click;
                _isAuto = false;
                _isRuntimeAI = false;
            }

            _isAutoButtonClick += Click;
        }

        /// <summary>
        /// 转换一行数据
        /// </summary>
        private void LanguageConversionRow(int row)
        {
            var _isOk = LanguageConversionInit(out string text, out string dest, out List<string> dests,
                out List<string> destTips,out List<List<string>> datas,out SynchronizationContext syncContext);
            if(!_isOk) return;
            var count = datas[row].Count();
            SetTip($"开始翻译第 {row+1} 行");
            var thread = new Thread(delegate(object o)
            {
                for (int j = 1; j < dests.Count; j++)
                {
                    text = datas[row][1];
                    if (string.IsNullOrWhiteSpace(text))
                    {
                        _tip = $"{row},{0}为空";
                        continue;
                    }
                    var tryIndex = 0;
                    while (tryIndex<3)
                    {
                        dest = dests[j];
                        var data = new PythonRunner().Run(
                            _pythonTranslation,
                            $"{text} {dest}").Trim();
                        var isNull = string.IsNullOrWhiteSpace(data);
                        if (isNull) tryIndex++;
                        syncContext.Post(state =>
                        {
                            if (isNull && tryIndex<2)
                            {
                                SetTip($"\"{text}\" 翻译为 [{destTips[j]}] 翻译失败,正在重新尝试[{tryIndex}]");
                            }
                            else if (isNull && tryIndex > 2)
                            {
                                SetTip($"\"{text}\" 翻译为 [{destTips[j]}] 翻译失败,已跳过");
                            }
                            else
                            {
                                SetTip($"\"{text}\" 翻译为 [{destTips[j]}] \"{data}\" 已完成. 设置至{row+1}行{j+2}列");
                                Set(row,j+1,data);
                            }
                        }, null);
                        Thread.Sleep(120);
                        if(!isNull) break;
                    }
                }
                _tip = $"第 {row+1} 行翻译完成";
                _isAuto = false;
                _isRuntimeAI = false;
            });
            thread.Start();
            _isAutoButtonClick -= Click;
            void Click()
            {
                SetTip($"已取消翻译");
                thread.Abort();
                _isAutoButtonClick -= Click;
                _isAuto = false;
                _isRuntimeAI = false;
            }

            _isAutoButtonClick += Click;
        }

        /// <summary>
        /// 转换一列数据
        /// </summary>
        private void LanguageConversionCol(int col)
        {
            var _isOk =LanguageConversionInit(out string text, out string dest, out List<string> dests,
                out List<string> destTips, out List<List<string>> datas, out SynchronizationContext syncContext);
            if(!_isOk) return;
            if (col > dests.Count())
            {
                _isAuto = false;
                _isRuntimeAI = false;
                SetTip("无法找到可翻译目标语言");
                return;
            }
            col -= 1;
            var count = datas.Count();
            SetTip($"开始翻译第 {col+2} 列");
            var thread = new Thread(delegate(object o) 
            {
                for (int i = 1; i < count; i++)
                {
                    text = datas[i][1];
                    if (string.IsNullOrWhiteSpace(text))
                    {
                        _tip = $"{i},{0}为空";
                        continue;
                    }

                    var tryIndex = 0;
                    while (tryIndex < 3)
                    {
                        dest = dests[col];
                        var data = new PythonRunner().Run(
                            _pythonTranslation,
                            $"{text} {dest}").Trim();
                        var isNull = string.IsNullOrWhiteSpace(data);
                        if (isNull) tryIndex++;
                        syncContext.Post(state =>
                        {
                            if (isNull && tryIndex < 2)
                            {
                                SetTip($"\"{text}\" 翻译为 [{destTips[col]}] 翻译失败,正在重新尝试[{tryIndex}]");
                            }
                            else if (isNull && tryIndex > 2)
                            {
                                SetTip($"\"{text}\" 翻译为 [{destTips[col]}] 翻译失败,已跳过");
                            }
                            else
                            {
                                SetTip($"\"{text}\" 翻译为 [{destTips[col]}] \"{data}\" 已完成. 设置至{i+1}行{col+2}列");
                                Set(i, col+1, data);
                            }
                        }, null);
                        Thread.Sleep(120);
                        if (!isNull) break;
                    }
                }
                _tip = $"第 {col+2} 列翻译完成";
                _isAuto = false;
                _isRuntimeAI = false;
            });
            thread.Start();
            _isAutoButtonClick -= Click;
            void Click()
            {
                SetTip("已取消翻译");
                thread.Abort();
                _isAutoButtonClick -= Click;
                _isAuto = false;
                _isRuntimeAI = false;
            }
            _isAutoButtonClick += Click;
        }
        
        /// <summary>
        /// 转换至语言文件
        /// </summary>
        private void Convert2Lang()
        {
            if(string.IsNullOrWhiteSpace(_sheetName))return;
            var sheets = _excelData[_sheetName];
            if(sheets==null)return;
            _isAuto = true;
            _isRuntimeAI = true;
            var langData = ExcelSupport.DataSet2DictionaryListList(ExcelSupport.ReadToDataSet(_langPath));
            List<List<string>> list = null;
            if (langData != null)
            {
                list = langData["CodeList"];
            }
            else
            {
                _isAuto = false;
                _isRuntimeAI = false;
                SetTip("配置文件读取失败");
                return;
            }
            var dict = new Dictionary<string,int>();
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

            for (var i = 1; i < sheets.Count(); i++)
            {
                var s = sheets[0][i];
                var intPtr = Search(s);
                if (intPtr > 0)
                {
                    dict.Add(list[intPtr][1],i);
                }
            }
            var syncContext = SynchronizationContext.Current;
            var thread = new Thread(delegate(object o)
            {
                foreach (var keyValuePair in dict)
                {
                    string content = "";
                    var dir = $"{_langOutputPath}{keyValuePair.Key}/";
                    if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                    for (int i = 1; i < sheets.Count(); i++)
                    {
                        var s = sheets[i][keyValuePair.Value];
                        var c = sheets[i][0];
                        if(string.IsNullOrWhiteSpace(s))continue;
                        content += ($"{c} : {s}\n");
                    }
                    var path = $"{dir}{_sheetName}.lang";
                    File.WriteAllText(path,content.Trim());
                    syncContext.Post(state =>
                    {
                        SetTip($"{sheets[0][keyValuePair.Value]}存储至{path}");
                    }, null);
                    Thread.Sleep(10);
                }
                _isAuto = false;
                _isRuntimeAI = false;
                _isShowAI = false;
            });
            thread.Start();
            _isAutoButtonClick -= Click;
            void Click()
            {
                SetTip("已取消翻译");
                thread.Abort();
                _isAutoButtonClick -= Click;
                _isAuto = false;
                _isRuntimeAI = false;
            }
            _isAutoButtonClick += Click;
        }

        /// <summary>
        /// 转换一列至语言文件
        /// </summary>
        /// <param name="col">列</param>
        private void Convert2LangCol(int col)
        {
            if(string.IsNullOrWhiteSpace(_sheetName))return;
            var sheets = _excelData[_sheetName];
            if(sheets==null)return;
            _isAuto = true;
            _isRuntimeAI = true;
            var langData = ExcelSupport.DataSet2DictionaryListList(ExcelSupport.ReadToDataSet(_langPath));
            List<List<string>> list = null;
            if (langData != null)
            {
                list = langData["CodeList"];
            }
            else
            {
                _isAuto = false;
                _isRuntimeAI = false;
                SetTip("配置文件读取失败");
                return;
            }
            var dict = new Dictionary<string,int>();
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

            var s = sheets[0][col];
            var intPtr = Search(s);
            if (intPtr > 0)
            {
                dict.Add(list[intPtr][1],col);
            }

            if (dict.Count <= 0)
            {
                _isAuto = false;
                _isRuntimeAI = false;
                _isShowAI = false;
                SetTip("无法存储此列(配置文件中不存在与之匹配的名称)");
                return;
            }
            var syncContext = SynchronizationContext.Current;
            var thread = new Thread(delegate(object o)
            {
                foreach (var keyValuePair in dict)
                {
                    string content = "";
                    var dir = $"{_langOutputPath}{keyValuePair.Key}/";
                    if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                    for (int i = 1; i < sheets.Count(); i++)
                    {
                        var s = sheets[i][keyValuePair.Value];
                        var c = sheets[i][0];
                        if(string.IsNullOrWhiteSpace(s))continue;
                        content += ($"{c} : {s}\n");
                    }
                    var path = $"{dir}{_sheetName}.lang";
                    File.WriteAllText(path,content.Trim());
                    syncContext.Post(state =>
                    {
                        SetTip($"{sheets[0][keyValuePair.Value]}存储至{path}");
                    }, null);
                    Thread.Sleep(10);
                }
                _isAuto = false;
                _isRuntimeAI = false;
                _isShowAI = false;
            });
            thread.Start();
            _isAutoButtonClick -= Click;
            void Click()
            {
                SetTip("已取消翻译");
                thread.Abort();
                _isAutoButtonClick -= Click;
                _isAuto = false;
                _isRuntimeAI = false;
            }
            _isAutoButtonClick += Click;
        }

        /// <summary>
        /// 填充编码
        /// </summary>
        private void FileCode()
        {
            var list = _excelData[_sheetName];
            var s1 = list[1][0];
            int.TryParse(s1, out var code);
            if (code == 0) code = 10000;
            for (var i = 1; i < list.Count(); i++)
            {
                Set(i,0,code.ToString());
                code++;
            }
        }
    }
}