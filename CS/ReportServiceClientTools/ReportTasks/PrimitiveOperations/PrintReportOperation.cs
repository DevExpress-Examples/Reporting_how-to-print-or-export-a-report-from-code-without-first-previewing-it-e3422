using System;
using DevExpress.Data.Utils.ServiceModel;
using DevExpress.Xpf.Printing.ServiceModel;
using DevExpress.Xpf.Printing.ServiceModel.DataContracts;
using DevExpress.XtraReports.ServiceModel.DataContracts;

namespace ReportServiceClientTools.ReportTasks.PrimitiveOperations {
    public class PrintReportOperation : LongRunningReportOperation {
        public PrintId PrintId { get; private set; }

        public PrintReportOperation(IReportServiceClient client)
            : this(client, LongRunningReportOperation.DefaultUpdateStatusInterval) {
        }

        public PrintReportOperation(IReportServiceClient client, TimeSpan updateStatusInterval)
            : base(client, updateStatusInterval) {
        }

        public void ExecuteAsync(DocumentId documentId, object asyncState) {
            Client.StartPrintCompleted += Client_StartPrintCompleted;
            Client.StartPrintAsync(documentId, PageCompatibility.Silverlight, asyncState);
        }

        void Client_StartPrintCompleted(object sender, ScalarOperationCompletedEventArgs<PrintId> e) {
            Client.StartPrintCompleted -= Client_StartPrintCompleted;

            if(HasErrorOrCancelled(e))
                return;

            PrintId = e.Result;
            QueryOperationStatusAsync(e.UserState);
        }

        protected override void QueryOperationStatusAsync(object asyncState) {
            Client.GetPrintStatusCompleted += Client_GetPrintStatusCompleted;
            Client.GetPrintStatusAsync(PrintId, asyncState);
        }

        void Client_GetPrintStatusCompleted(object sender, ScalarOperationCompletedEventArgs<PrintStatus> e) {
            Client.GetPrintStatusCompleted -= Client_GetPrintStatusCompleted;
            QueryOperationStatusCompleted(e, () => e.Result.Status, e.Result.Fault);
        }
    }
}
