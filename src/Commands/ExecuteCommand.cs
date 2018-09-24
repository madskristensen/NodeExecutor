using System;
using System.ComponentModel.Design;
using EnvDTE;
using Microsoft.VisualStudio.Shell;

namespace NodeExecutor
{
    internal sealed class ExecuteCommand
    {
        private readonly Package package;

        private ExecuteCommand(Package package, OleMenuCommandService commandService)
        {
            this.package = package;

            var cmdId = new CommandID(PackageGuids.guidPrettierPackageCmdSet, PackageIds.ExecuteSolExp);
            var cmd = new OleMenuCommand(Execute, cmdId)
            {
                Supported = false
            };

            commandService.AddCommand(cmd);
        }

        public static ExecuteCommand Instance
        {
            get;
            private set;
        }

        private IServiceProvider ServiceProvider
        {
            get { return package; }
        }

        public static void Initialize(Package package, OleMenuCommandService commandService)
        {
            Instance = new ExecuteCommand(package, commandService);
        }

        private void Execute(object sender, EventArgs e)
        {
            var dte = ServiceProvider.GetService(typeof(DTE)) as DTE;
            string fileName = GetFileName(dte);

            if (string.IsNullOrEmpty(fileName))
                return;

            NodeProcess.ExecuteFile(fileName);
        }

        private static string GetFileName(DTE dte)
        {
            string fileName = null;

            if (dte.ActiveWindow.Type == vsWindowType.vsWindowTypeDocument)
            {
                fileName = dte.ActiveDocument.FullName;
            }
            else if (dte.SelectedItems.Count == 1)
            {
                fileName = dte.SelectedItems?.Item(1)?.ProjectItem?.FileNames[0];
            }

            return fileName;
        }
    }
}
