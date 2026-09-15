using System;
using System.Diagnostics;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace D365VsTools.VisualStudio
{
    public static class Logger
    {
        private static IVsOutputWindow _outputWindow;

        /// <summary>
        /// Initialize Logger output window
        /// </summary>
        public static void Initialize()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            _outputWindow = Package.GetGlobalService(typeof(SVsOutputWindow)) as IVsOutputWindow;
            Guid windowGuid = ProjectGuids.OutputWindow;
            string windowTitle = "D365-VS-Tools";
            _outputWindow?.CreatePane(ref windowGuid, windowTitle, 1, 1);
        }

        /// <summary>
        /// Adds a timestamp and line feed to message and writes it to output window
        /// </summary>
        /// <param name="message">text message to write</param>
        /// <param name="print">print or ignore call using for extended logging</param>
        public static void WriteLine(string message, bool print = true)
        {
            if (print)
                Write($"[{DateTime.Now:HH:mm:ss}] {message}\r\n");
        }

        /// <summary>
        /// Writes message to output window. Safe to call from a background thread (commands can now run
        /// their CRM/file work off the UI thread; this marshals back onto it as needed).
        /// </summary>
        /// <param name="message">Text message to write</param>
        public static void Write(string message)
        {
            ProjectHelper.RunOnUIThread(() =>
            {
                try
                {
                    Debug.Print(message);

                    Guid windowGuid = ProjectGuids.OutputWindow;
                    IVsOutputWindowPane pane;
                    _outputWindow.GetPane(ref windowGuid, out pane);
                    pane.Activate();
                    pane.OutputStringThreadSafe(message);
                }
                catch (Exception e)
                {
                    Debug.Print("Error writting Log: " + e.Message);
                }
            });
        }

        public static void Clear()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var windowGuid = ProjectGuids.OutputWindow;
            IVsOutputWindowPane pane;
            _outputWindow.GetPane(ref windowGuid, out pane);
            pane.Clear();
        }

        // WriteLine now timestamps every line itself; kept for existing callers.
        public static void WriteLineWithTime(string message, bool print = true) => WriteLine(message, print);
    }
}
