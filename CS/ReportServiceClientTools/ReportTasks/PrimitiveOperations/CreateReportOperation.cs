using System;
using DevExpress.Data.Utils.ServiceModel;
using DevExpress.DocumentServices.ServiceModel.Client;
using DevExpress.DocumentServices.ServiceModel.DataContracts;

namespace ReportServiceClientTools.ReportTasks.PrimitiveOperations
{
    class CreateReportOperation : LongRunningReportOperation
    {
        public DocumentId DocumentId { get; private set; }

        public CreateReportOperation(IReportServiceClient client)
            : this(client, LongRunningReportOperation.DefaultUpdateStatusInterval)
        {
        }

        public CreateReportOperation(IReportServiceClient client, TimeSpan updateStatusInterval)
            : base(client, updateStatusInterval)
        {
        }

        public void ExecuteAsync(string reportName, ReportParameter[] parameters, object asyncState) {
            if(string.IsNullOrEmpty(reportName))
                throw new ArgumentException("reportName");

            Client.StartBuildCompleted += client_StartBuildCompleted;
            Client.StartBuildAsync(new ReportNameIdentity(reportName), new ReportBuildArgs() { Parameters = parameters }, asyncState);
        }

        void client_StartBuildCompleted(object sender, ScalarOperationCompletedEventArgs<DocumentId> e)
        {
            Client.StartBuildCompleted -= client_StartBuildCompleted;

            if (HasErrorOrCancelled(e))
                return;

            DocumentId = e.Result;
            QueryOperationStatusAsync(e.UserState);
        }

        void client_GetBuildStatusCompleted(object sender, ScalarOperationCompletedEventArgs<BuildStatus> e)
        {
            Client.GetBuildStatusCompleted -= client_GetBuildStatusCompleted;
            QueryOperationStatusCompleted(e, () => e.Result.Status, e.Result.Fault);
        }

        protected override void QueryOperationStatusAsync(object asyncState)
        {
            Client.GetBuildStatusCompleted += client_GetBuildStatusCompleted;
            Client.GetBuildStatusAsync(DocumentId, asyncState);
        }
    }
}
