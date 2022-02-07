/// Most functionality in this file is taken from
/// astyle-extension by lukamicoder
///
/// https://github.com/lukamicoder/astyle-extension
/// Licensed under Apache License Version 2.0

global using Community.VisualStudio.Toolkit;
global using Microsoft.VisualStudio.Shell;
global using System;
global using Task = System.Threading.Tasks.Task;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.VisualStudio;
using EnvDTE;
using System.ComponentModel.Design;

namespace BN_Astyle
{
    public enum Language
    {
        NA,
        Cpp
    }

    [Guid(PackageGuids.BN_AstyleString)]
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [InstalledProductRegistration(Vsix.Name, Vsix.Description, Vsix.Version)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideOptionPage(typeof(OptionsProvider.GeneralOptions), "BN_Astyle", "General", 0, 0, true)]
    // [ProvideProfile(typeof(OptionsProvider.GeneralOptions), "BN_Astyle", "General", 0, 0, true)] // Save options with user profile
    [ProvideAutoLoad(VSConstants.UICONTEXT.NoSolution_string, PackageAutoLoadFlags.BackgroundLoad)]
    [ProvideAutoLoad(VSConstants.UICONTEXT.SolutionExists_string, PackageAutoLoadFlags.BackgroundLoad)]
    [ProvideAutoLoad(VSConstants.UICONTEXT.SolutionHasSingleProject_string, PackageAutoLoadFlags.BackgroundLoad)]
    [ProvideAutoLoad(VSConstants.UICONTEXT.SolutionHasMultipleProjects_string, PackageAutoLoadFlags.BackgroundLoad)]
    public sealed class BN_AstylePackage : ToolkitPackage
    {
        private DTE _dte;
        private DocumentEventListener _documentEventListener;
        private OleMenuCommand _formatDocMenuCommand;

        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            await this.RegisterCommandsAsync();

            await JoinableTaskFactory.SwitchToMainThreadAsync();

            var mcs = await GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (null != mcs)
            {
                var id = new CommandID(PackageGuids.BN_Astyle, (int)PackageIds.FormatDocumentCommand);
                _formatDocMenuCommand = new OleMenuCommand(FormatDocumentCallback, id);
                mcs.AddCommand(_formatDocMenuCommand);
                _formatDocMenuCommand.BeforeQueryStatus += OnBeforeQueryStatus;
            }

            DTE dte_tmp = (DTE)await GetServiceAsync(typeof(DTE));
            if (dte_tmp == null)
            {
                await VS.MessageBox.ShowWarningAsync( "BN_AStyle Error", "Failed to get DTE service" );
                return;
            }
            _dte = dte_tmp;

            _documentEventListener = new DocumentEventListener(this);
            _documentEventListener.BeforeSave += OnBeforeDocumentSave;
        }

        private int OnBeforeDocumentSave(uint docCookie)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            if (!General.Instance.CppFormatOnSave)
            {
                return VSConstants.S_OK;
            }

            Document doc = null;
            string find_name = _documentEventListener.GetDocumentName(docCookie);
            foreach ( Document this_doc in _dte.Documents )
            {
                if (this_doc.FullName == find_name)
                {
                    doc = this_doc;
                    break;
                }
            }

            Language language = GetLanguage( doc );

            FormatDocument(GetTextDocument(doc), language);

            return VSConstants.S_OK;
        }

        private void OnBeforeQueryStatus(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var cmd = (OleMenuCommand)sender;
            var language = GetLanguage(_dte.ActiveDocument);

            if (language != Language.Cpp)
            {
                cmd.Visible = false;
            }
            else
            {
                cmd.Visible = true;
            }

            cmd.Enabled = cmd.Visible;
        }

        private void FormatDocumentCallback(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var language = GetLanguage(_dte.ActiveDocument);
            var textDoc = GetTextDocument(_dte.ActiveDocument);

            FormatDocument(textDoc, language);
        }

        private void FormatDocument(TextDocument textDoc, Language language)
        {
            if (textDoc == null || language == Language.NA)
            {
                return;
            }

            ThreadHelper.ThrowIfNotOnUIThread();

            EditPoint sp = textDoc.StartPoint.CreateEditPoint();
            EditPoint ep = textDoc.EndPoint.CreateEditPoint();
            string text = sp.GetText(ep);

            if (String.IsNullOrEmpty(text))
            {
                return;
            }

            string formattedText = Format(text, language);
            if (!String.IsNullOrEmpty(formattedText))
            {
                sp.ReplaceText(ep, formattedText, (int)vsEPReplaceTextOptions.vsEPReplaceTextKeepMarkers);
            }
        }

        private TextDocument GetTextDocument(Document doc)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            if (doc == null || doc.ReadOnly)
            {
                return null;
            }

            var textDoc = doc.Object("TextDocument") as TextDocument;

            return textDoc;
        }

        private Language GetLanguage(Document doc)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var language = Language.NA;

            string lang = doc.Language.ToLower();
            if (lang == "gcc" || lang == "avrgcc" || lang == "c/c++")
            {
                language = Language.Cpp;
            }

            return language;
        }

        private string Format(string text, Language language)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var aStyle = new AStyleInterface();
            aStyle.ErrorRaised += OnAStyleErrorRaised;

            if (language == Language.Cpp) {
                return aStyle.FormatSource(text, General.Instance.CppOptions);
            } else {
                throw new NotImplementedException();
            }
        }

        private void OnAStyleErrorRaised(object source, AStyleErrorArgs args)
        {
            VS.MessageBox.ShowWarning("AStyle Formatter Error", args.Message);
        }
    }
}