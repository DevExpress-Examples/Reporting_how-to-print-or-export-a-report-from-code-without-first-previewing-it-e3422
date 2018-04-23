using System;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using DevExpress.Xpf.Printing.Service;
using DevExpress.XtraReports.Service;
using DevExpress.XtraReports.UI;

namespace E3422.Web
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "DemoReportService" in code, svc and config file together.
    [SilverlightFaultBehavior]
    public class DemoReportService : DevExpress.XtraReports.Service.ReportService
    {
        // Uncomment this method if you need to extend the base functionality.
        // protected override XtraReport CreateReportByName(string reportName)
        // {
        //	return base.CreateReportByName(reportName);
        // }
    }
}
