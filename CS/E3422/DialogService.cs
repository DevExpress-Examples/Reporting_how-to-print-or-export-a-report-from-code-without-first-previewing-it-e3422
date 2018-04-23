using System;
using System.IO;
using System.Windows;
using System.Windows.Browser;
using System.Windows.Controls;
using DevExpress.DocumentServices.ServiceModel.Native;
using DevExpress.Xpf.Printing;
using DevExpress.Xpf.Printing.Native;

namespace E3422 {
    public class DialogService : IDialogService {
        public void ShowPreview(string serviceUri, string reportName) {
            ReportPreviewModel model = new ReportPreviewModel(serviceUri);
            model.ReportName = reportName;
            DocumentPreviewWindow preview = new DocumentPreviewWindow() { Model = model };
            model.CreateDocument();
            preview.ShowDialog();
        }

        public Stream ShowSaveFileDialog(string filter) {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = filter;
       
            if(dialog.ShowDialog() != true)
                return null;

            return dialog.OpenFile();
        }

        public void ShowMessage(string caption, string message) {
            MessageBox.Show(message, caption ?? string.Empty, MessageBoxButton.OK);
        }

        public void OpenBrowserWindow(Uri uri) {
            HtmlPage.Window.Navigate(uri, "_blank", "toolbar=0,menubar=0,resizable=1,scrollbars=1");
        }

        public void AsyncRequestPrintingConfirmation(Action<bool> continuePrinting) {
            LoadingPrintDataWindow window = new LoadingPrintDataWindow();

            LoadingPrintDataViewModel model = new LoadingPrintDataViewModel();
            model.SetStatus(PrintingStatus.Generated);
            model.Continue += (s, a) => {
                window.Hide();
                continuePrinting(true);
            };
            model.Cancel += (s, a) => {
                window.Hide();
                continuePrinting(false);
            };

            window.ViewModel = model;
            window.Show();
        }
    }
}
