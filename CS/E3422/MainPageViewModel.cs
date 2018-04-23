using System;
using System.ComponentModel;
using System.IO;
using System.Linq.Expressions;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using DevExpress.DocumentServices.ServiceModel.Client;
using DevExpress.DocumentServices.ServiceModel.DataContracts;
using DevExpress.Mvvm;
using DevExpress.Printing.DocumentServices.ServiceModel;
using DevExpress.Printing.DocumentServices;
using DevExpress.Utils;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Printing.Native;
using DevExpress.XtraPrinting;

namespace E3422 {
    public class MainPageViewModel : INotifyPropertyChanged {
        readonly IDialogService dialogService;
        readonly DelegateCommand<object> showPreviewCommand;
        readonly DelegateCommand<object> exportCommand;
        readonly DelegateCommand<object> exportToWindowCommand;
        readonly DelegateCommand<object> printCommand;

        public event PropertyChangedEventHandler PropertyChanged;

        public string ReportName { get; set; }
        public string ReportServiceUri { get; set; }

        string parameterValue;
        public string ParameterValue {
            get { return parameterValue; }
            set {
                if(value != parameterValue) {
                    parameterValue = value;
                    RaisePropertyChanged(() => ParameterValue);
                }
            }
        }

        public ICommand PrintCommand { get { return printCommand; } }
        public ICommand ExportCommand { get { return exportCommand; } }
        public ICommand ExportToWindowCommand { get { return exportToWindowCommand; } }
        public ICommand ShowPreviewCommand { get { return showPreviewCommand; } }

        bool isBusy;
        private bool IsBusy {
            get { return isBusy; }
            set {
                if(value != isBusy) {
                    isBusy = value;
                    printCommand.RaiseCanExecuteChanged();
                    exportCommand.RaiseCanExecuteChanged();
                    exportToWindowCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public MainPageViewModel()
            : this(new DialogService()) {
        }

        public MainPageViewModel(IDialogService dialogService) {
            Guard.ArgumentNotNull(dialogService, "dialogService");
            this.dialogService = dialogService;

            printCommand = new DelegateCommand<object>(Print, CanPrint);
            exportCommand = new DelegateCommand<object>(Export, CanExport);
            exportToWindowCommand = new DelegateCommand<object>(ExportToWindow, CanExportToWindow);
            showPreviewCommand = new DelegateCommand<object>(ShowPreview, CanShowPreview);
        }

        IReportServiceClient CreateClient() {
            ReportServiceClientFactory factory = new ReportServiceClientFactory(new EndpointAddress(ReportServiceUri));
            return factory.Create();
        }

        ReportParameter[] CreateParameters() {
            return new ReportParameter[] {
                new ReportParameter() { Path = "stringParameter", Value = ParameterValue  }
            };
        }

        void RaisePropertyChanged<T>(Expression<Func<T>> property) {
            PropertyExtensions.RaisePropertyChanged(this, PropertyChanged, property);
        }

        private bool CanPrint(object arg) {
            return !IsBusy;
        }

        private void Print(object obj) {
            IsBusy = true;
            Task<string[]> printTask = Task.Factory.PrintReportAsync(CreateClient(), ReportName, CreateParameters(), null);
            printTask.ContinueWith(PrintReportCompleted, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void PrintReportCompleted(Task<string[]> task) {
            IsBusy = false;
            if(TaskIsFauledOrCancelled(task, "Print"))
                return;
            dialogService.AsyncRequestPrintingConfirmation(continuePrinting => {
                if(continuePrinting) {
                    DocumentPrinter documentPrinter = new DocumentPrinter();
                    documentPrinter.Print(new XamlDocumentPaginator(task.Result), "Print Document Name");
                }
            });
        }

        private bool CanExportToWindow(object arg) {
            return !IsBusy && Application.Current.Host.Settings.EnableHTMLAccess;
        }

        private void ExportToWindow(object obj) {
            IsBusy = true;
            Task<Uri> exportTask = Task.Factory.ExportReportForDownloadAsync(CreateClient(), ReportName, new PdfExportOptions(), CreateParameters(), null);
            exportTask.ContinueWith(ExportReportForDownloadCompleted, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void ExportReportForDownloadCompleted(Task<Uri> task) {
            IsBusy = false;
            if(TaskIsFauledOrCancelled(task, "Export"))
                return;
            dialogService.OpenBrowserWindow(task.Result);
        }

        private bool CanExport(object arg) {
            return !isBusy;
        }

        private void Export(object obj) {
            Stream stream = dialogService.ShowSaveFileDialog("PDF files (*.pdf)|*.pdf");
            if(stream == null)
                return;
            IsBusy = true;
            Task<byte[]> exportTask = Task.Factory.ExportReportAsync(CreateClient(), ReportName, new PdfExportOptions(), CreateParameters(), stream);
            exportTask.ContinueWith(ExportReportCompleted, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void ExportReportCompleted(Task<byte[]> task) {
            IsBusy = false;
            using(Stream stream = (Stream)task.AsyncState) {
                if(TaskIsFauledOrCancelled(task, "Export"))
                    return;
                stream.Write(task.Result, 0, task.Result.Length);
            }
        }

        private bool CanShowPreview(object arg) {
            return true;
        }

        private void ShowPreview(object obj) {
            dialogService.ShowPreview(ReportServiceUri, ReportName);
        }

        private bool TaskIsFauledOrCancelled(Task task, string caption) {
            if(task.IsFaulted) {
                dialogService.ShowMessage(caption, task.Exception.Message);
                return true;
            }

            if(task.IsCanceled) {
                dialogService.ShowMessage(caption, "Operation has been cancelled");
                return true;
            }

            return false;
        }
    }
}
