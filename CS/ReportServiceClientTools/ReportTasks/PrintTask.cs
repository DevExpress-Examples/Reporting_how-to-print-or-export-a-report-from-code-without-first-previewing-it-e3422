using System.ComponentModel;
using DevExpress.Data.Utils.ServiceModel;
using DevExpress.Xpf.Printing.ServiceModel;
using DevExpress.XtraReports.ServiceModel.DataContracts;
using ReportServiceClientTools.ReportTasks.PrimitiveOperations;

namespace ReportServiceClientTools.ReportTasks {
    public class PrintTask : ReportServiceOperation {
        public string[] PrintData { get; private set; }

        public PrintTask(IReportServiceClient client)
            : base(client) {
        }

        public void ExecuteAsync(string reportName, object asyncState) {
            ExecuteAsync(reportName, null, asyncState);
        }

        public void ExecuteAsync(string reportName, ReportParameter[] parameters, object asyncState) {
            CreateReportOperation createReport = new CreateReportOperation(Client);
            createReport.Completed += createReport_Completed;
            createReport.ExecuteAsync(reportName, parameters, asyncState);
        }

        void createReport_Completed(object sender, AsyncCompletedEventArgs e) {
            CreateReportOperation createReport = (CreateReportOperation)sender;
            createReport.Completed -= createReport_Completed;

            if(HasErrorOrCancelled(e))
                return;

            PrintReportOperation printOperation = new PrintReportOperation(Client);
            printOperation.Completed += printOperation_Completed;
            printOperation.ExecuteAsync(createReport.DocumentId, e.UserState);
        }

        void printOperation_Completed(object sender, AsyncCompletedEventArgs e) {
            PrintReportOperation printOperation = (PrintReportOperation)sender;
            printOperation.Completed -= printOperation_Completed;

            if(HasErrorOrCancelled(e))
                return;

            Client.GetPrintDocumentCompleted += Client_GetPrintDocumentCompleted;
            Client.GetPrintDocumentAsync(printOperation.PrintId, e.UserState);
        }

        void Client_GetPrintDocumentCompleted(object sender, ScalarOperationCompletedEventArgs<byte[]> e) {
            Client.GetPrintDocumentCompleted -= Client_GetPrintDocumentCompleted;

            if(HasErrorOrCancelled(e))
                return;

            PrintData = DevExpress.Xpf.Printing.Native.Helper.DeserializePages(e.Result);
            RaiseCompleted(e);
        }
    }
}
