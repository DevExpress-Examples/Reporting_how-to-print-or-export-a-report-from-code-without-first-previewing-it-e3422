//using System;
//using System.ComponentModel;
//using DevExpress.Xpf.Printing.Service;
//using DevExpress.Xpf.Printing.Service.DataContracts;

//namespace ReportServiceClientTools.ReportTasks {
//    public class ExportAndGetDownloadUriTask : ExportTask {
//        public Uri DownloadUri { get; private set; }

//        public ExportAndGetDownloadUriTask(IReportServiceClient client)
//            : base(client) {
//        }

//        protected override void ProcessExportedDocumentAsync(ExportId exportId, object asyncState) {
//            DownloadUri = new Uri(string.Format("{0}/{1}/Get?id={2}", serviceUri, "rest", exportId.Value), UriKind.RelativeOrAbsolute);
//            RaiseCompleted(new AsyncCompletedEventArgs(null, false, asyncState));
//        }
//    }
//}
