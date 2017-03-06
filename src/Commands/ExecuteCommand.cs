using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Text.Editor;
using System;
using System.IO;

namespace NodeExecutor
{
    internal sealed class ExecuteCommand : BaseCommand
    {
        private Guid _commandGroup = PackageGuids.guidPrettierPackageCmdSet;
        private const uint _commandId = PackageIds.PrettierCommandId;

        private IWpfTextView _view;
        private NodeProcess _node;
        private string _filePath;

        public ExecuteCommand(IWpfTextView view, NodeProcess node, string filePath)
        {
            _view = view;
            _node = node;
            _filePath = filePath;
        }

        public override int Exec(ref Guid pguidCmdGroup, uint nCmdID, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut)
        {
            if (pguidCmdGroup == _commandGroup && nCmdID == _commandId)
            {
                if (_node != null)
                {
                    _node.ExecuteFile(_filePath);
                }

                return VSConstants.S_OK;
            }

            return Next.Exec(pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut);
        }

        public override int QueryStatus(ref Guid pguidCmdGroup, uint cCmds, OLECMD[] prgCmds, IntPtr pCmdText)
        {
            if (pguidCmdGroup == _commandGroup && prgCmds[0].cmdID == _commandId)
            {
                prgCmds[0].cmdf = (uint)OLECMDF.OLECMDF_ENABLED | (uint)OLECMDF.OLECMDF_SUPPORTED;
                return VSConstants.S_OK;
            }

            return Next.QueryStatus(pguidCmdGroup, cCmds, prgCmds, pCmdText);
        }
    }
}