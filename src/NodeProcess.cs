using System;
using System.Diagnostics;
using System.IO;

namespace NodeExecutor
{
    internal class NodeProcess
    {
        public void ExecuteFile(string filePath)
        {
            var start = new ProcessStartInfo("cmd", $"/k node.exe {Path.GetFileName(filePath)}")
            {
                WorkingDirectory = Path.GetDirectoryName(filePath),
                UseShellExecute = false,
                CreateNoWindow = false,
            };

            ModifyPathVariable(start);

            try
            {
                using (var proc = Process.Start(start))
                {
                    proc.WaitForExit();
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }

        private static void ModifyPathVariable(ProcessStartInfo start)
        {
            string path = start.EnvironmentVariables["PATH"];

            var process = Process.GetCurrentProcess();
            string ideDir = Path.GetDirectoryName(process.MainModule.FileName);

            if (Directory.Exists(ideDir))
            {
                string parent = Directory.GetParent(ideDir).Parent.FullName;

                string rc2Preview1Path = new DirectoryInfo(Path.Combine(parent, @"Web\External")).FullName;

                if (Directory.Exists(rc2Preview1Path))
                {
                    path += ";" + rc2Preview1Path;
                }
                else
                {
                    path += ";" + Path.Combine(ideDir, @"Extensions\Microsoft\Web Tools\External");
                    path += ";" + Path.Combine(ideDir, @"Extensions\Microsoft\Web Tools\External\git");
                }
            }

            start.EnvironmentVariables["PATH"] = path;
        }
    }
}
