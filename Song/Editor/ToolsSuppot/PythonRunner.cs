using System;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Debug = UnityEngine.Debug;

namespace Song.Editor.ToolsSuppot
{
    /// <summary>
    /// 系统
    /// </summary>
    public class PythonRunner
    {
        private readonly string _pythonPath = "/Library/Frameworks/Python.framework/Versions/3.11/bin/python3";

        public PythonRunner()
        {
        }

        public PythonRunner(string pythonPath)
        {
            _pythonPath = pythonPath ?? _pythonPath;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="arguments"></param>
        /// <param name="controll"></param>
        /// <returns></returns>
        /// <exception cref="OperationCanceledException"></exception>
        public string Run(string fileName, string arguments)
        {
            using var process = new Process
            {
                StartInfo =
                {
                    FileName = _pythonPath,
                    Arguments = $"{fileName} {arguments}",
                    UseShellExecute = false,
                    RedirectStandardOutput = true
                }
            };

            var output = new StringBuilder();
            var outputLock = new object();

            process.OutputDataReceived += (sender, args) =>
            {
                if (args.Data != null)
                {
                    lock (outputLock)
                    {
                        output.AppendLine(args.Data);
                    }
                }
            };

            process.EnableRaisingEvents = true;

            var tcs = new TaskCompletionSource<int>();

            process.Exited += (sender, args) =>
            {
                try
                {
                    tcs.TrySetResult(process.ExitCode);
                }
                catch (Exception ex)
                {
                    tcs.TrySetException(ex);
                }
            };

            try
            {
                process.Start();
                process.BeginOutputReadLine();

                tcs.Task.Wait();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return output.ToString();
        }
    }
}