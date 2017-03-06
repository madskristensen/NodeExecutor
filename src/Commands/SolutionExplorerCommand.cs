using System;
using System.ComponentModel.Design;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using EnvDTE;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace NodeExecutor
{
    internal sealed class SolutionExplorerCommand
    {
        private readonly Package package;
        private string _fileName;

        private SolutionExplorerCommand(Package package, OleMenuCommandService commandService)
        {
            this.package = package;

            var cmdId = new CommandID(PackageGuids.guidPrettierPackageCmdSet, PackageIds.ExecuteSolExp);
            var cmd = new OleMenuCommand(Execute, cmdId);
            cmd.BeforeQueryStatus += BeforeQueryStatus;
            commandService.AddCommand(cmd);
        }

        public static SolutionExplorerCommand Instance
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
            Instance = new SolutionExplorerCommand(package, commandService);
        }

        private void BeforeQueryStatus(object sender, EventArgs e)
        {
            var button = (OleMenuCommand)sender;
            button.Visible = button.Enabled = false;

            var item = GetSelectedItem() as ProjectItem;

            if (item == null)
                return;

            string ext = Path.GetExtension(item.FileNames[0]);
            if (CommandRegistration.FileExtensions.Contains(ext, StringComparer.OrdinalIgnoreCase))
            {
                _fileName = item.FileNames[0];
                button.Visible = button.Enabled = true;
            }
        }

        private void Execute(object sender, EventArgs e)
        {
            NodeProcess.ExecuteFile(_fileName);
        }

        public static object GetSelectedItem()
        {
            object selectedObject = null;

            var monitorSelection = (IVsMonitorSelection)Package.GetGlobalService(typeof(SVsShellMonitorSelection));

            try
            {
                monitorSelection.GetCurrentSelection(out IntPtr hierarchyPointer,
                                                 out uint itemId,
                                                 out IVsMultiItemSelect multiItemSelect,
                                                 out IntPtr selectionContainerPointer);

                if (Marshal.GetTypedObjectForIUnknown(hierarchyPointer, typeof(IVsHierarchy)) is IVsHierarchy selectedHierarchy)
                {
                    ErrorHandler.ThrowOnFailure(selectedHierarchy.GetProperty(itemId, (int)__VSHPROPID.VSHPROPID_ExtObject, out selectedObject));
                }

                Marshal.Release(hierarchyPointer);
                Marshal.Release(selectionContainerPointer);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Write(ex);
            }

            return selectedObject;
        }
    }
}
