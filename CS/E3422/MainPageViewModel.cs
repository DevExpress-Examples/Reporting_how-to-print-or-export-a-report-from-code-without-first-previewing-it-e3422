using System;
using System.ComponentModel;
using System.IO;
using System.Linq.Expressions;
using System.ServiceModel;
using System.Windows;
using System.Windows.Input;
using DevExpress.DocumentServices.ServiceModel.Client;
using DevExpress.DocumentServices.ServiceModel.DataContracts;
using DevExpress.Utils;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Commands;
using DevExpress.Xpf.Printing.Native;
using DevExpress.XtraPrinting;
using ReportServiceClientTools.ReportTasks;

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

        public MainPageViewModel() : this(new DialogService()) {
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
            PrintTask printTask = new PrintTask(CreateClient());
            printTask.Completed += printTask_Completed;
            printTask.ExecuteAsync(ReportName, CreateParameters(), null);
            IsBusy = true;
        }

        void printTask_Completed(object sender, AsyncCompletedEventArgs e) {
            PrintTask task = (PrintTask)sender;
            task.Completed -= printTask_Completed;
            IsBusy = false;

            if(e.Error != null) {
                dialogService.ShowMessage("Error", e.Error.Message);
            } else {
                dialogService.AsyncRequestPrintingConfirmation(continuePrinting => {
                    if(continuePrinting) {
                        DocumentPrinter documentPrinter = new DocumentPrinter();
                        documentPrinter.Print(new XamlDocumentPaginator(task.PrintData), "Print Document Name");
                    }
                });
            }
        }

        private bool CanExportToWindow(object arg) {
            return !IsBusy && Application.Current.Host.Settings.EnableHTMLAccess;
        }

        private void ExportToWindow(object obj) {
            ExportAndGetDownloadUriTask exportToWindowTask = new ExportAndGetDownloadUriTask(CreateClient());
            exportToWindowTask.Completed += exportToWindowTask_Completed;
            exportToWindowTask.ExecuteAsync(ReportName, new PdfExportOptions(), CreateParameters(), null);
            IsBusy = true;
        }

        void exportToWindowTask_Completed(object sender, AsyncCompletedEventArgs e) {
            ExportAndGetDownloadUriTask task = (ExportAndGetDownloadUriTask)sender;
            task.Completed -= exportToWindowTask_Completed;
            IsBusy = false;

            if(e.Error != null) {
                dialogService.ShowMessage("Error", e.Error.Message);
            } else {
                dialogService.OpenBrowserWindow(task.DownloadUri);
            }
        }

        private bool CanExport(object arg) {
            return !isBusy;
        }

        private void Export(object obj) {
            Stream stream = dialogService.ShowSaveFileDialog("PDF files (*.pdf)|*.pdf");
            
            if(stream == null)
                return;
            
            ExportAndGetFileDataTask exportTask = new ExportAndGetFileDataTask(CreateClient());
            exportTask.Completed += exportTask_Completed;
            exportTask.ExecuteAsync(ReportName, new PdfExportOptions(), CreateParameters(), stream);
            IsBusy = true;
        }

        void exportTask_Completed(object sender, AsyncCompletedEventArgs e) {
            ExportAndGetFileDataTask task = (ExportAndGetFileDataTask)sender;
            task.Completed -= exportTask_Completed;
            IsBusy = false;

            using(Stream stream = (Stream)e.UserState) {
                if(e.Error != null) {
                    dialogService.ShowMessage("Error", e.Error.Message);
                } else {
                    stream.Write(task.ExportedData, 0, task.ExportedData.Length);
                }
            }
        }

        private bool CanShowPreview(object arg) {
            return true;
        }

        private void ShowPreview(object obj) {
            dialogService.ShowPreview(ReportServiceUri, ReportName);
        }
    }
}
