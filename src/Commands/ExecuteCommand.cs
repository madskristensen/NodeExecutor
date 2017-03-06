using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using System;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;

namespace NodeExecutor
{
    internal sealed class ExecuteCommand
    {
        public static string[] FileExtensions { get; } = { ".js", ".es6" };

        private readonly Package package;
        private string _fileName;

        private ExecuteCommand(Package package, OleMenuCommandService commandService)
        {
            this.package = package;

            var cmdId = new CommandID(PackageGuids.guidPrettierPackageCmdSet, PackageIds.ExecuteSolExp);
            var cmd = new OleMenuCommand(Execute, cmdId);
            cmd.BeforeQueryStatus += BeforeQueryStatus;
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

        private void BeforeQueryStatus(object sender, EventArgs e)
        {
            var button = (OleMenuCommand)sender;
            button.Visible = button.Enabled = false;

            var dte = ServiceProvider.GetService(typeof(DTE)) as DTE2;

            if (dte.ActiveWindow.Type == vsWindowType.vsWindowTypeDocument)
            {
                _fileName = dte.ActiveDocument.FullName;
            }
            else if (dte.SelectedItems.Count == 1)
            {
                _fileName = dte.SelectedItems?.Item(1)?.ProjectItem?.FileNames[0];
            }

            if (string.IsNullOrEmpty(_fileName))
                return;

            string ext = Path.GetExtension(_fileName);

            if (FileExtensions.Contains(ext, StringComparer.OrdinalIgnoreCase))
            {
                button.Visible = button.Enabled = true;
            }
        }

        private void Execute(object sender, EventArgs e)
        {
            NodeProcess.ExecuteFile(_fileName);
        }
    }
}
