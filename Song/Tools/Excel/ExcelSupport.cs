using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using Excel;
using OfficeOpenXml;
using UnityEngine;

namespace Song.Runtime.Support
{
    /// <summary>
    /// Excel支持
    /// </summary>
    public static class ExcelSupport
    {
        #region 存储
        /// <summary>
        /// 存储Excel
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <param name="datas">Excel数据</param>
        /// <param name="IsCreate">为空创建</param>
        /// <param name="IsCover">是否覆盖</param>
        public static bool Save(string filePath, ExcelData datas, bool IsCover = true, bool IsCreate = true)
        {
            if (!filePath.Contains(".xlsx")) filePath += ".xlsx";
            if (!IsCreate && !File.Exists(filePath)) return false;
            if (!IsCover && File.Exists(filePath)) return false;
            FileInfo excelName = new FileInfo(filePath);
            if (excelName.Exists)
            {
                excelName.Delete();
                excelName = new FileInfo(filePath);
            }
            using (ExcelPackage package = new ExcelPackage(excelName))
            {
                foreach (var item in datas.Sheets())
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(item.name);
                    for (int i = 0; i < item.RowCount; i++)
                    {
                        for (int k = 0; k < item.ColCount; k++)
                        {
                            worksheet.Cells[i + 1, k + 1].Value = item.Get(i, k);
                        }
                    }
                }
                package.Save();
            }
            return true;
        }

        /// <summary>
        /// 存储Excel
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <param name="data">Excel 工作表数据</param>
        /// <param name="IsCreate">为空创建</param>
        /// <param name="IsCover">是否覆盖</param>
        public static bool Save(string filePath,SheetData data, bool IsCover = true, bool IsCreate = true)
        {
            if (!filePath.Contains(".xlsx")) filePath += ".xlsx";
            if (!IsCreate && !File.Exists(filePath)) return false;
            if (!IsCover && File.Exists(filePath)) return false;
            FileInfo excelName = new FileInfo(filePath);
            if (excelName.Exists)
            {
                excelName.Delete();
                excelName = new FileInfo(filePath);
            }
            using (ExcelPackage package = new ExcelPackage(excelName))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(data.name);
                for (int i = 0; i < data.RowCount; i++)
                {
                    for (int k = 0; k < data.ColCount; k++)
                    {
                        worksheet.Cells[i + 1, k + 1].Value = data.Get(i,k);
                    }
                }
                package.Save();
            }
            return true;
        }
        #endregion

        #region 读取
        /// <summary>
        /// 读取Excel表至StringList
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns>字符表</returns>
        public static List<string> ReadToStringList(string filePath)
            => DataSet2StringList(ReadToDataSet(filePath));

        /// <summary>
        /// 读取Excel表至Excel数据
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns>Excel数据</returns>
        public static ExcelData ReadToExcelData(string filePath)
            => DataSet2ExcelData(ReadToDataSet(filePath));

        /// <summary>
        /// 读取Excel表至DataSet
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns>Excel数据</returns>
        public static DataSet ReadToDataSet(string filePath)
        {
            var dataSet = new DataSet();
            var sheetNames = new List<string>();
            using (var package = new ExcelPackage(new FileInfo(filePath)))
            {
                foreach (var worksheet in package.Workbook.Worksheets)
                {
                    sheetNames.Add(worksheet.Name);
                }
            }
            var stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            var excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
            dataSet = excelReader.AsDataSet();
            foreach (var sheetName in sheetNames)
            {
                if (dataSet.Tables.Contains(sheetName)) continue;
                else dataSet.Tables.Add(sheetName);
            }
            return dataSet;
        }



        /// <summary>
        /// Excel数据转StringList
        /// </summary>
        /// <param name="result">Excel数据</param>
        /// <returns>StringList</returns>
        public static List<string>  DataSet2StringList(DataSet result)
        {
            List<string> datas = new List<string>();
            for (int i = 0; i < result.Tables.Count; i++)
            {
                DataRowCollection DRC = result.Tables[i].Rows;
                int columnCount = result.Tables[i].Columns.Count;
                int rowCount = result.Tables[i].Rows.Count;
                for (int j = 0; j < rowCount; j++)
                {
                    for (int k = 0; k < columnCount; k++)
                    {
                        datas.Add(DRC[j][k].ToString());
                    }
                }
            }
            return datas;
        }

        /// <summary>
        /// DataSet转Dictionary<string,string[,]>
        /// </summary>
        /// <param name="result">DataSet</param>
        /// <returns>Dictionary<string,string[,]></returns>
        public static Dictionary<string, string[,]> DataSet2DictionaryStringArrayTwo(DataSet result)
        {
            var dictionary = new Dictionary<string, string[,]>();

            if (result != null && result.Tables != null)
            {
                foreach (DataTable table in result.Tables)
                {
                    if (table != null && table.Rows != null && table.Columns != null)
                    {
                        var data = new string[table.Rows.Count, table.Columns.Count];
                        for (var i = 0; i < table.Rows.Count; i++)
                        {
                            var row = table.Rows[i];
                            for (var j = 0; j < table.Columns.Count; j++)
                            {
                                data[i, j] = row[j].ToString();
                            }
                        }
                        dictionary.Add(table.TableName, data);
                    }
                    else
                    {
                        dictionary.Add(table.TableName, new string[0,0]);
                    }
                }
            }

            return dictionary;
        }


        /// <summary>
        /// DataSet转ExcelData
        /// </summary>
        /// <param name="result">DataSet</param>
        /// <returns>ExcelData</returns>
        public static ExcelData DataSet2ExcelData(DataSet result)
        {
            ExcelData datas = new ExcelData();
            for (int i = 0; i < result.Tables.Count; i++)
            {
                DataRowCollection DRC = result.Tables[i].Rows;
                int rowCount = result.Tables[i].Rows.Count;
                int columnCount = result.Tables[i].Columns.Count;
                string TableName = result.Tables[i].TableName;
                string[,] Sheetdatas = new string[rowCount, columnCount];
                for (int j = 0; j < rowCount; j++)
                {
                    for (int k = 0; k < columnCount; k++)
                    {
                        Sheetdatas[j, k] = (DRC[j][k].ToString());
                    }
                }
                SheetData sheetData = new SheetData(Sheetdatas,TableName);
                datas.Add(sheetData);
            }
            return datas;
        }
        #endregion
    }
}
