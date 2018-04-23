using DevExpress.Data.Utils.ServiceModel;
using DevExpress.DocumentServices.ServiceModel.Client;
using DevExpress.DocumentServices.ServiceModel.DataContracts;

namespace ReportServiceClientTools.ReportTasks
{
    public class ExportAndGetFileDataTask : ExportTask
    {
        public byte[] ExportedData { get; private set; }

        public ExportAndGetFileDataTask(IReportServiceClient client)
            : base(client)
        {
        }

        protected override void ProcessExportedDocumentAsync(ExportId exportId, object asyncState)
        {
            Client.GetExportedDocumentCompleted += Client_GetExportedDocumentCompleted;
            Client.GetExportedDocumentAsync(exportId, asyncState);
        }

        void Client_GetExportedDocumentCompleted(object sender, ScalarOperationCompletedEventArgs<byte[]> e)
        {
            Client.GetExportedDocumentCompleted -= Client_GetExportedDocumentCompleted;

            if (HasErrorOrCancelled(e))
                return;

            ExportedData = e.Result;
            RaiseCompleted(e);
        }
    }
}
