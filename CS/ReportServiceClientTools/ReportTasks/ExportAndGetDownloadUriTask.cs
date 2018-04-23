using System;
using System.ComponentModel;
using DevExpress.DocumentServices.ServiceModel.Client;
using DevExpress.DocumentServices.ServiceModel.DataContracts;

namespace ReportServiceClientTools.ReportTasks {
    public class ExportAndGetDownloadUriTask : ExportTask {
        public Uri DownloadUri { get; private set; }

        public ExportAndGetDownloadUriTask(IReportServiceClient client)
            : base(client) {
        }

        protected override void ProcessExportedDocumentAsync(ExportId exportId, object asyncState) {
            DownloadUri = Client.GetDocumentDownloadUri(exportId);
            RaiseCompleted(new AsyncCompletedEventArgs(null, false, asyncState));
        }
    }
}
