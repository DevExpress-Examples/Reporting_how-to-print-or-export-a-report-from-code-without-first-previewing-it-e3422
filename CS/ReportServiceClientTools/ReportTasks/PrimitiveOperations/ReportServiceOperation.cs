using System;
using System.ComponentModel;
using DevExpress.Xpf.Printing.ServiceModel;

namespace ReportServiceClientTools.ReportTasks.PrimitiveOperations
{
    public abstract class ReportServiceOperation
    {
        readonly IReportServiceClient client;
        public IReportServiceClient Client { get { return client; } }

        public event AsyncCompletedEventHandler Completed;

        public ReportServiceOperation(IReportServiceClient client)
        {
            if (client == null)
                throw new ArgumentNullException("client");
            this.client = client;
        }

        protected bool HasErrorOrCancelled(AsyncCompletedEventArgs args)
        {
            if (args.Error != null || args.Cancelled)
            {
                RaiseCompleted(args);
                return true;
            }

            return false;
        }

        protected void RaiseCompleted(AsyncCompletedEventArgs args)
        {
            if (Completed != null)
            {
                Completed(this, args);
            }
        }
    }
}
