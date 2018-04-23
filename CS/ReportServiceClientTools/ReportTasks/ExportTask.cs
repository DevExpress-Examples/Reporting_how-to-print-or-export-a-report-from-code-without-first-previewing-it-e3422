using System;
using System.ComponentModel;
using System.IO;
using DevExpress.DocumentServices.ServiceModel.Client;
using DevExpress.DocumentServices.ServiceModel.DataContracts;
using DevExpress.Utils.Serializing;
using DevExpress.XtraPrinting;
using ReportServiceClientTools.ReportTasks.PrimitiveOperations;

namespace ReportServiceClientTools.ReportTasks
{
    public abstract class ExportTask : ReportServiceOperation
    {
        ExportFormat exportFormat;
        byte[] serializedExportOptions;

        public ExportTask(IReportServiceClient client)
            : base(client)
        {
        }

        public void ExecuteAsync(string reportName, ExportOptionsBase exportOptions, object asyncState) {
            ExecuteAsync(reportName, exportOptions, null, asyncState);
        }

        public void ExecuteAsync(string reportName, ExportOptionsBase exportOptions, ReportParameter[] parameters, object asyncState) {
            if (exportOptions == null)
                throw new ArgumentNullException("exportOptions");

            exportFormat = GetExportFormat(exportOptions);
            serializedExportOptions = Serialize(exportOptions);

            CreateReportOperation createReport = new CreateReportOperation(Client);
            createReport.Completed += createReport_Completed;
            createReport.ExecuteAsync(reportName, parameters, asyncState);
        }

        void createReport_Completed(object sender, AsyncCompletedEventArgs e)
        {
            CreateReportOperation createReport = (CreateReportOperation)sender;
            createReport.Completed -= createReport_Completed;

            if (HasErrorOrCancelled(e))
                return;

            ExportReportOperation exportReport = new ExportReportOperation(Client);
            exportReport.Completed += exportReport_Completed;
            exportReport.ExecuteAsync(createReport.DocumentId, exportFormat, serializedExportOptions, e.UserState);
        }

        void exportReport_Completed(object sender, AsyncCompletedEventArgs e)
        {
            ExportReportOperation exportReport = (ExportReportOperation)sender;
            exportReport.Completed -= exportReport_Completed;

            if (HasErrorOrCancelled(e))
                return;

            ProcessExportedDocumentAsync(exportReport.ExportId, e.UserState);
        }

        protected abstract void ProcessExportedDocumentAsync(ExportId exportId, object asyncState);

        private static ExportFormat GetExportFormat(ExportOptionsBase exportOptions)
        {
            if (exportOptions is CsvExportOptions)
                return ExportFormat.Csv;
            if (exportOptions is HtmlExportOptions)
                return ExportFormat.Htm;
            if (exportOptions is ImageExportOptions)
                return ExportFormat.Image;
            if (exportOptions is MhtExportOptions)
                return ExportFormat.Mht;
            if (exportOptions is PdfExportOptions)
                return ExportFormat.Pdf;
            if (exportOptions is RtfExportOptions)
                return ExportFormat.Rtf;
            if (exportOptions is TextExportOptions)
                return ExportFormat.Txt;
            if (exportOptions is XlsExportOptions)
                return ExportFormat.Xls;
            if (exportOptions is XlsxExportOptions)
                return ExportFormat.Xlsx;
            if (exportOptions is XpsExportOptions)
                return ExportFormat.Xps;

            throw new NotSupportedException(exportOptions.ToString());
        }

        private static byte[] Serialize(ExportOptionsBase exportOptions)
        {
            using (var stream = new MemoryStream())
            {
                var serializer = new XmlXtraSerializer();
                serializer.SerializeObject(exportOptions, stream, typeof(ExportOptions).Name);
                return stream.ToArray();
            }
        }
    }
}
