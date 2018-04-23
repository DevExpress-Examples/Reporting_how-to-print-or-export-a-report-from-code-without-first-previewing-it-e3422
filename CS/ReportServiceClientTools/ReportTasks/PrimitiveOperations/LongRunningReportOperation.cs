using System;
using System.ComponentModel;
using DevExpress.Xpf.Printing;
using DevExpress.Xpf.Printing.ServiceModel;
using DevExpress.Xpf.Printing.ServiceModel.DataContracts;

namespace ReportServiceClientTools.ReportTasks.PrimitiveOperations
{
    public abstract class LongRunningReportOperation : ReportServiceOperation
    {
        protected static readonly TimeSpan DefaultUpdateStatusInterval = TimeSpan.FromMilliseconds(250);

        readonly Delayer delayer;
        protected Delayer Delayer { get { return delayer; } }

        public LongRunningReportOperation(IReportServiceClient client, TimeSpan updateStatusInterval)
            : base(client)
        {
            delayer = new Delayer(updateStatusInterval);
        }

        protected void QueryOperationStatusCompleted(AsyncCompletedEventArgs args, Func<TaskStatus> getTaskStatus, ServiceFault serviceFault)
        {
            if (HasErrorOrCancelled(args))
                return;

            switch (getTaskStatus())
            {
                case TaskStatus.InProgress:
                    Delayer.Execute(() => QueryOperationStatusAsync(args.UserState));
                    break;

                case TaskStatus.Complete:
                    RaiseCompleted(args);
                    break;

                case TaskStatus.Fault:
                    RaiseCompleted(new AsyncCompletedEventArgs(ExceptionFromFault(serviceFault), false, args.UserState));
                    break;

                default:
                    throw new NotSupportedException("Unexpected status: " + getTaskStatus());
            }
        }

        protected abstract void QueryOperationStatusAsync(object asyncState);

        protected static Exception ExceptionFromFault(ServiceFault fault)
        {
            string message = fault != null ? fault.Message : "Unspecified fault on the server";
            Exception innerException = fault != null ? new Exception(fault.FullMessage) : null;
            return new Exception(message, innerException);
        }
    }
}
