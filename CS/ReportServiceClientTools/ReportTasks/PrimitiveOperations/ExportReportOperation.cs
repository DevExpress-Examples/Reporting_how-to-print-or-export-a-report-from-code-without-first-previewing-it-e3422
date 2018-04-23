using System;
using DevExpress.Data.Utils.ServiceModel;
using DevExpress.Xpf.Printing.ServiceModel;
using DevExpress.Xpf.Printing.ServiceModel.DataContracts;
using DevExpress.XtraPrinting;
using DevExpress.XtraReports.ServiceModel.DataContracts;

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
            DocumentExportArgs exportArgs = new DocumentExportArgs() { Format = exportFormat, SerializedExportOptions = exportOptions };
            Client.StartExportAsync(documentId, exportArgs, asyncState);
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
