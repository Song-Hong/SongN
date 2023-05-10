using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Song.Tools.Quick
{
    public partial class Quick : UnityEditor.EditorWindow
    {
        /// <summary>
        /// 输入消息缓存
        /// </summary>
        private string _inputTextCache;

        /// <summary>
        /// 快捷操作集合预输入
        /// </summary>
        private Dictionary<int, List<string>> _commnadsTip;

        /// <summary>
        /// 快捷操作集合缓存
        /// </summary>
        private Dictionary<int, string> _commands;

        /// <summary>
        /// 机器说话队列
        /// </summary>
        private List<string> _robotTalks = new List<string>();

        /// <summary>
        /// 说话间隔
        /// </summary>
        private readonly int _talkWait = 26;
        
        private event Action _stopGenerate;
        
        /// <summary>
        /// 初始化全部命令
        /// </summary>
        private void InitCommand()
        {
            var _commandCount = 1000000;
            _commands = new Dictionary<int, string>();
            _commnadsTip = new Dictionary<int, List<string>>();
            var assembly = Assembly.GetExecutingAssembly();
            var types = assembly.GetTypes();
            foreach (var type in types)
            {
                if (type.IsClass && !type.IsAbstract && typeof(IQuickCommand).IsAssignableFrom(type))
                {
                    var command = Type.GetType(type.FullName);
                    var instance = Activator.CreateInstance(command);
                    var method = type.GetMethod("CommandName");
                    var commandTips = method.Invoke(instance, null);
                    var tips = commandTips.ToString().Split(',').ToList();
                    _commands.Add(_commandCount,type.FullName);
                    _commnadsTip.Add(_commandCount,tips);
                    _commandCount ++;
                }
            }
        }
        
        /// <summary>
        /// 生成对话
        /// </summary>
        private void GenerateRobot()
        {
            _robotIsWrite = true;
            var syncContext = SynchronizationContext.Current;
            var thread = new Thread(delegate(object o)
            {
                void Talk(string key,int count)
                {
                    foreach (var c in key)
                    {
                        syncContext.Post((_) =>
                        {
                            _robotText += c;
                            if (count >= 30)
                            {
                                _robotText += "\n";
                                count = 0;
                            }
                            Repaint();
                        }, null);
                        count++;
                        Thread.Sleep(_talkWait);
                    } 
                }
                
                float ComputeSimilarity(string s1, string s2)
                {
                    int maxLength = Math.Max(s1.Length, s2.Length);
                    if (maxLength == 0) return 1f;

                    int distance = ComputeLevenshteinDistance(s1, s2);

                    return (float)(maxLength - distance) / maxLength;
                }

                int ComputeLevenshteinDistance(string s1, string s2)
                {
                    int[,] d = new int[s1.Length + 1, s2.Length + 1];

                    for (int i = 0; i <= s1.Length; i++)
                    {
                        d[i, 0] = i;
                    }

                    for (int j = 0; j <= s2.Length; j++)
                    {
                        d[0, j] = j;
                    }

                    for (int j = 1; j <= s2.Length; j++)
                    {
                        for (int i = 1; i <= s1.Length; i++)
                        {
                            if (s1[i - 1] == s2[j - 1])
                            {
                                d[i, j] = d[i - 1, j - 1];
                            }
                            else
                            {
                                d[i, j] = Math.Min(Math.Min(d[i - 1, j], d[i, j - 1]), d[i - 1, j - 1]) + 1;
                            }
                        }
                    }

                    return d[s1.Length, s2.Length];
                }
                
                if(_commands==null) InitCommand();
                
                var similarityThreshold = 0.25f; 

                var mostSimilarCommands = new Dictionary<string, int>();

                foreach (var kvp in _commnadsTip)
                {
                    var key = kvp.Key;
                    var commandList = kvp.Value;

                    var mostSimilarCommand = "";
                    var highestSimilarityScore = 0.0f;

                    foreach (var command in commandList)
                    {
                        var similarityScore = ComputeSimilarity(command, _inputTextCache);
                        if (similarityScore >= similarityThreshold && similarityScore > highestSimilarityScore)
                        {
                            mostSimilarCommand = command;
                            highestSimilarityScore = similarityScore;
                        }
                    }

                    if (!string.IsNullOrEmpty(mostSimilarCommand))
                    {
                        mostSimilarCommands.Add(mostSimilarCommand, key);
                    }
                }

                var sortedCommands = mostSimilarCommands.OrderByDescending(x => ComputeSimilarity(x.Key, _inputTextCache)).ToList();

                if (sortedCommands.Count > 0)
                {
                    Talk("为你找到以下结果:\n", 0);
                    foreach (var command in sortedCommands)
                    {
                        _robotText += _robotText.EndsWith("\n") ? "" : "\n";
                        Talk($"{command.Key}<{command.Value}>\n", 0);
                    }
                }
                else
                {
                    Talk("未找到相关命令", 0);
                }
                
                syncContext.Post((_) =>
                {
                    _robotIsWrite = false;
                    AddRobotChat(_robotText);
                    _robotText = "";
                    _inputTextCache = "";
                    Repaint();
                }, null);
            });
            thread.Start();
            _stopGenerate += Stop;

            void Stop()
            {
                thread.Abort();
                _robotIsWrite = false;
                AddRobotChat(_robotText);
                _robotText = "";
                _inputTextCache = "";
                Repaint();
                _stopGenerate -= Stop;
            }
        }

        /// <summary>
        /// 执行
        /// </summary>
        private void Execute(int commandName)
        {
            if(!_commands.ContainsKey(commandName)) return;
            var command = Type.GetType(_commands[commandName]);
            var instance = Activator.CreateInstance(command);
            var method = command.GetMethod("Execute");
            var request = method.Invoke(instance, null);
            // if(string.IsNullOrWhiteSpace()) return;
            NewRobotTalk(request.ToString());
        }

        /// <summary>
        /// 创建新的机器对话
        /// </summary>
        /// <param name="message">所说的消息</param>
        private void NewRobotTalk(string message)
        {
            if (_robotIsWrite)
            {
                _robotTalks.Add(message);
                return;
            }
            _robotIsWrite = true;
            var _autoLineFeed = !message.Contains("\n");
            var syncContext = SynchronizationContext.Current;
            var thread = new Thread(delegate(object o)
            {
                var count = 0;
                foreach (var c in message)
                {
                    syncContext.Post((_) =>
                    {
                        _robotText += c;
                        if (_autoLineFeed && count >= 30)
                        {
                            _robotText += "\n";
                            count = 0;
                        }
                        Repaint();
                    }, null);
                    count++;
                    Thread.Sleep(_talkWait);
                }
                
                syncContext.Post((_) =>
                {
                    _robotIsWrite = false;
                    AddRobotChat(_robotText);
                    _robotText = "";
                    Repaint();
                    
                    if (_robotTalks.Count == 0) return;
                    NewRobotTalk(_robotTalks[0]);
                    _robotTalks.RemoveAt(0);
                }, null);
            });
            thread.Start();
            
            _stopGenerate += Stop;

            void Stop()
            {
                thread.Abort();
                _robotIsWrite = false;
                AddRobotChat(_robotText);
                _robotText = "";
                _inputTextCache = "";
                Repaint();
                _stopGenerate -= Stop;
            }
        }
    }
}