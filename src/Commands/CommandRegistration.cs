using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.Utilities;
using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;

namespace NodeExecutor
{
    [Export(typeof(IVsTextViewCreationListener))]
    [ContentType("TypeScript")]
    [ContentType("JavaScript")]
    [TextViewRole(PredefinedTextViewRoles.PrimaryDocument)]
    internal sealed class CommandRegistration : IVsTextViewCreationListener
    {
        public static string[] FileExtensions { get; } = { ".js", ".es6" };

        [Import]
        private IVsEditorAdaptersFactoryService AdaptersFactory { get; set; }

        [Import]
        private ITextDocumentFactoryService DocumentService { get; set; }

        public void VsTextViewCreated(IVsTextView textViewAdapter)
        {
            IWpfTextView view = AdaptersFactory.GetWpfTextView(textViewAdapter);

            if (!DocumentService.TryGetTextDocument(view.TextBuffer, out ITextDocument doc))
                return;

            string ext = Path.GetExtension(doc.FilePath);

            if (!FileExtensions.Contains(ext, StringComparer.OrdinalIgnoreCase))
                return;

            AddCommandFilter(textViewAdapter, new ExecuteCommand(view, doc.FilePath));
        }

        private void AddCommandFilter(IVsTextView textViewAdapter, BaseCommand command)
        {

            textViewAdapter.AddCommandFilter(command, out var next);
            command.Next = next;
        }
    }
}
