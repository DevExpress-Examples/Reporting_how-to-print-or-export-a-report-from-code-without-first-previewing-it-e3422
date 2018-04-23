using System;
using System.IO;

namespace E3422 {
    public interface IDialogService {
        void ShowPreview(string serviceUri, string reportName);
        Stream ShowSaveFileDialog(string filter);
        void ShowMessage(string caption, string message);
        void OpenBrowserWindow(Uri uri);
        void AsyncRequestPrintingConfirmation(Action<bool> continuePrinting);
    }
}
