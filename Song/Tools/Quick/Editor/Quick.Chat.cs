using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Song.Tools.Quick
{
    /// <summary>
    /// 消息界面
    /// </summary>
    public partial class Quick:UnityEditor.EditorWindow
    {
        enum ChatType
        {
            My,
            Robot
        }

        /// <summary>
        /// 聊天信息,<%> %对应的操作编号
        /// </summary>
        private List<Tuple<ChatType, string>> _chats = new List<Tuple<ChatType, string>>();

        private Vector2 _chatPo = Vector2.zero;
        
        /// <summary>
        /// 机器人是否在书写
        /// </summary>
        private bool _robotIsWrite;

        /// <summary>
        /// 生成的文本
        /// </summary>
        private string _robotText;
        
        private void ShowChats()
        {
            GUILayout.BeginArea(new Rect(0,0,Screen.width,Screen.height*0.9f));
            _chatPo = GUILayout.BeginScrollView(_chatPo);
            GUILayout.Space(30);
            foreach (var tuple in _chats)
            {
                if (tuple.Item1 == ChatType.My)
                {
                    CreateMyChat(tuple.Item2);
                }
                else
                {
                    ParsingRobotChat(tuple.Item2);
                }
            }

            //当机器人书写时生成临时对话框
            if (_robotIsWrite)
            {
                ParsingRobotChat(_robotText+"|");
            }

            GUILayout.Space(30);
            GUILayout.EndScrollView();
            GUILayout.EndArea();
        }

        /// <summary>
        /// 添加我的消息
        /// </summary>
        private void AddMyChat(string message) => AddChat(ChatType.My, message);

        /// <summary>
        /// 添加机器人消息
        /// </summary>
        private void AddRobotChat(string message) => AddChat(ChatType.Robot, message);

        /// <summary>
        /// 添加聊天
        /// </summary>
        private void AddChat(ChatType type,string message)
        {
            _chats.Add(new Tuple<ChatType, string>(type, message));
        }
        
        /// <summary>
        /// 创建我的聊天信息
        /// </summary>
        /// <param name="message"></param>
        private void CreateMyChat(string message)
        {
            GUILayout.Space(2);
            GUILayout.BeginHorizontal();
            GUILayout.Space(30);
            GUILayout.FlexibleSpace();
            GUILayout.Label(message,_chatBGI);
            GUILayout.Space(10);
            GUILayout.EndHorizontal();
            GUILayout.Space(10);
        }

        /// <summary>
        /// 解析机器人聊天信息
        /// </summary>
        /// <param name="message"></param>
        private void ParsingRobotChat(string message)
        {
            //#  命令
            //^  编辑器窗口
            //@  链接
            //%  百科
            
            foreach (var text in message.Split('\n'))
            {
                if(string.IsNullOrWhiteSpace(text))continue;
                if (text.StartsWith("[") && text.Contains("]"))
                {
                    switch (text.Substring(1, text.IndexOf("]", StringComparison.Ordinal) - 1))
                    {
                        case "#": //命令
                            CreateRobotCommand(text.Replace("[#]",""));
                            break;
                        case "^": //编辑器窗口
                            CreateRobotWindow(text.Replace("[^]",""));
                            break;
                        case "@": //链接
                            CreateRobotLink(text.Replace("[@]",""));
                            break;
                        case "%": //百科
                            CreateRobotPedia(text.Replace("[%]",""));
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    CreateRobotChat(text);
                }
            }
        }

        /// <summary>
        /// 创建机器人语言
        /// </summary>
        private void CreateRobotChat(string message)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(10);
            GUILayout.Label(message,_chatBGI);
            GUILayout.FlexibleSpace();
            GUILayout.Space(30);
            GUILayout.EndHorizontal();
            GUILayout.Space(4);
        }

        /// <summary>
        /// 获取命令编号
        /// </summary>
        /// <param name="message">信息</param>
        /// <returns></returns>
        private void ParsingCommandMessage(in string message,out int commandID,out string robotMessage)
        {
            var startIndex = message.IndexOf("<", StringComparison.Ordinal);
            var endIndex = message.IndexOf(">", StringComparison.Ordinal);
            if (startIndex != -1 && endIndex != -1 && endIndex > startIndex)
            {
                var result = message.Substring(startIndex + 1, endIndex - startIndex - 1);
                commandID = int.Parse(result);
                robotMessage = message.Remove(startIndex, endIndex - startIndex + 1);
            }
            else
            {
                commandID = 0;
                robotMessage = message;
            }
        }

        /// <summary>
        /// 创建机器人命令
        /// </summary>
        private void CreateRobotCommand(string message)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(10);
            ParsingCommandMessage(message,out var commandID,out var robotMessage);
            if (GUILayout.Button(new GUIContent(robotMessage,_logoViewC ), _commandBGI))
            {
                NewRobotTalk("开始执行: "+robotMessage);
                Execute(commandID);
            }
            GUILayout.FlexibleSpace();
            GUILayout.Space(30);
            GUILayout.EndHorizontal();
            GUILayout.Space(4);
        }
        
        /// <summary>
        /// 创建机器人窗口
        /// </summary>
        private void CreateRobotWindow(string message)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(10);
            ParsingCommandMessage(message,out var commandID,out var robotMessage);
            if (GUILayout.Button(new GUIContent(robotMessage,_windowView ), _commandBGI))
            {
                NewRobotTalk("打开窗口: "+robotMessage);
                Execute(commandID);
            }
            GUILayout.FlexibleSpace();
            GUILayout.Space(30);
            GUILayout.EndHorizontal();
            GUILayout.Space(4);
        }
        
        /// <summary>
        /// 创建机器人链接
        /// </summary>
        private void CreateRobotLink(string message)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(10);
            ParsingCommandMessage(message,out var commandID,out var robotMessage);
            if (GUILayout.Button(new GUIContent(robotMessage,_linkView ), _commandBGI))
            {
                NewRobotTalk("打开网页: "+robotMessage);
                Execute(commandID);
            }
            GUILayout.FlexibleSpace();
            GUILayout.Space(30);
            GUILayout.EndHorizontal();
            GUILayout.Space(4);
        }
        
        /// <summary>
        /// 创建机器人百科
        /// </summary>
        private void CreateRobotPedia(string message)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(10);
            ParsingCommandMessage(message,out var commandID,out var robotMessage);
            if (GUILayout.Button(new GUIContent(robotMessage,_pediaView ), _commandBGI))
            {
                NewRobotTalk("显示百科: "+robotMessage);
                Execute(commandID);
            }
            GUILayout.FlexibleSpace();
            GUILayout.Space(30);
            GUILayout.EndHorizontal();
            GUILayout.Space(4);
        }
    }
}