using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Song.Runtime.Localization
{
    public class Lang
    {
        /// <summary>
        /// 数据
        /// </summary>
        private Dictionary<string, string> _data;

        #region 构造函数
        /// <summary>
        /// 初始化
        /// </summary>
        public Lang() => _data = new Dictionary<string, string>();

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="content">内容</param>
        public Lang(string content):this() => Parsing(content.Split('\n'));

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="contents">内容</param>
        public Lang(string[] contents):this() => Parsing(contents);

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="contents"></param>
        public Lang(List<string> contents):this() => Parsing(contents.ToArray());
        
        /// <summary>
        /// 解析
        /// </summary>
        /// <param name="contents">内容</param>
        private void Parsing(string[] contents)
        {
            foreach (var s in contents)
            {
                if(string.IsNullOrWhiteSpace(s)) continue;
                var parts = s.Split(new char[] { ':' }, 2);
                if (parts.Length != 2) continue;
                _data.Add(parts[0].Trim(),parts[1].Trim());
            }
        }
        #endregion

        #region 存取
        /// <summary>
        ///  语言数据
        /// </summary>
        /// <param name="key">编号</param>
        public string this[string key]
        {
            get
            {
                _data.TryGetValue(key, out var item);
                return item ?? "";
            }
            set
            {
                if(_data.ContainsKey(key))
                    _data[key] = value;
                else
                    _data.Add(key,value);
            }
        }

        /// <summary>
        /// 语言数量
        /// </summary>
        public int Count => _data.Count;
        #endregion
    }
}
