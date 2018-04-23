using System;
using DevExpress.XtraPrinting;
using DevExpress.Xpf.Printing.Service.DataContracts;
using DevExpress.Xpf.Printing.Service;

namespace ReportServiceClientTools.ReportTasks.PrimitiveOperations
{
    class ExportReportOperation : LongRunningReportOperation
    {
        public ExportId ExportId { get; private set; }

        public ExportReportOperation(IReportServiceClient client)
            : this(client, LongRunningReportOperation.DefaultUpdateStatusInterval)
        {
        }

        public ExportReportOperation(IReportServiceClient client, TimeSpan updateStatusInterval)
            : base(client, updateStatusInterval)
        {
        }

        public void ExecuteAsync(DocumentId documentId, ExportFormat exportFormat, byte[] exportOptions, object asyncState)
        {
            Client.StartExportCompleted += client_StartExportCompleted;
            Client.StartExportAsync(documentId, exportFormat, exportOptions, asyncState);
        }

        void client_StartExportCompleted(object sender, ScalarOperationCompletedEventArgs<ExportId> e)
        {
            Client.StartExportCompleted -= client_StartExportCompleted;

            if (HasErrorOrCancelled(e))
                return;

            ExportId = e.Result;
            QueryOperationStatusAsync(e.UserState);
        }

        void client_GetExportStatusCompleted(object sender, ScalarOperationCompletedEventArgs<ExportStatus> e)
        {
            Client.GetExportStatusCompleted -= client_GetExportStatusCompleted;
            QueryOperationStatusCompleted(e, () => e.Result.Status, e.Result.Fault);
        }

        protected override void QueryOperationStatusAsync(object asyncState)
        {
            Client.GetExportStatusCompleted += client_GetExportStatusCompleted;
            Client.GetExportStatusAsync(ExportId, asyncState);
        }
    }
}
