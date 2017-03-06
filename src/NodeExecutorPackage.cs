using Microsoft.VisualStudio.Shell;
using System;
using System.ComponentModel.Design;
using System.Runtime.InteropServices;
using System.Threading;

namespace NodeExecutor
{
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", Vsix.Version, IconResourceID = 400)]
    [Guid(PackageGuids.guidPrettierPackageString)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideAutoLoad(UIContextRule)]
    [ProvideUIContextRule(UIContextRule,
        name: "Test auto load",
        expression: "js",
        termNames: new[] { "js" },
        termValues: new[] { "HierSingleSelectionName:.js$" })]
    public sealed class NodeExecutorPackage : AsyncPackage
    {
        private const string UIContextRule = "b6b8bfd2-2af9-4dec-9dfb-4c1297eca6e7";

        protected override async System.Threading.Tasks.Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            if (await GetServiceAsync(typeof(IMenuCommandService)) is OleMenuCommandService commandService)
            {
                SolutionExplorerCommand.Initialize(this, commandService);
            }
        }
    }
}
