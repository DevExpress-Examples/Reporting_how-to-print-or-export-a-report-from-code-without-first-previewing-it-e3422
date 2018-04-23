Namespace Reports
    Partial Public Class SampleReport
        ''' <summary>
        ''' Required designer variable.
        ''' </summary>
        Private components As System.ComponentModel.IContainer = Nothing

        ''' <summary> 
        ''' Clean up any resources being used.
        ''' </summary>
        ''' <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        Protected Overrides Sub Dispose(ByVal disposing As Boolean)
            If disposing AndAlso (components IsNot Nothing) Then
                components.Dispose()
            End If
            MyBase.Dispose(disposing)
        End Sub

        #Region "Designer generated code"

        ''' <summary>
        ''' Required method for Designer support - do not modify
        ''' the contents of this method with the code editor.
        ''' </summary>
        Private Sub InitializeComponent()
            Me.Detail = New DevExpress.XtraReports.UI.DetailBand()
            Me.TopMargin = New DevExpress.XtraReports.UI.TopMarginBand()
            Me.xrLabel1 = New DevExpress.XtraReports.UI.XRLabel()
            Me.BottomMargin = New DevExpress.XtraReports.UI.BottomMarginBand()
            Me.stringParameter = New DevExpress.XtraReports.Parameters.Parameter()
            Me.xrLabel2 = New DevExpress.XtraReports.UI.XRLabel()
            DirectCast(Me, System.ComponentModel.ISupportInitialize).BeginInit()
            ' 
            ' Detail
            ' 
            Me.Detail.Controls.AddRange(New DevExpress.XtraReports.UI.XRControl() { Me.xrLabel2})
            Me.Detail.HeightF = 33.00001F
            Me.Detail.Name = "Detail"
            Me.Detail.Padding = New DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F)
            Me.Detail.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft
            ' 
            ' TopMargin
            ' 
            Me.TopMargin.Controls.AddRange(New DevExpress.XtraReports.UI.XRControl() { Me.xrLabel1})
            Me.TopMargin.Name = "TopMargin"
            Me.TopMargin.Padding = New DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F)
            Me.TopMargin.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft
            ' 
            ' xrLabel1
            ' 
            Me.xrLabel1.Font = New System.Drawing.Font("Verdana", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, (CByte(0)))
            Me.xrLabel1.LocationFloat = New DevExpress.Utils.PointFloat(10.00001F, 10.00001F)
            Me.xrLabel1.Name = "xrLabel1"
            Me.xrLabel1.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F)
            Me.xrLabel1.SizeF = New System.Drawing.SizeF(629.9999F, 64.66666F)
            Me.xrLabel1.StylePriority.UseFont = False
            Me.xrLabel1.Text = "A Sample Report"
            ' 
            ' BottomMargin
            ' 
            Me.BottomMargin.Name = "BottomMargin"
            Me.BottomMargin.Padding = New DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F)
            Me.BottomMargin.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft
            ' 
            ' stringParameter
            ' 
            Me.stringParameter.Description = "This is my string parameter"
            Me.stringParameter.Name = "stringParameter"
            Me.stringParameter.ValueInfo = "string parameter default value"
            ' 
            ' xrLabel2
            ' 
            Me.xrLabel2.DataBindings.AddRange(New DevExpress.XtraReports.UI.XRBinding() { New DevExpress.XtraReports.UI.XRBinding(Me.stringParameter, "Text", "")})
            Me.xrLabel2.LocationFloat = New DevExpress.Utils.PointFloat(10.00001F, 10.00001F)
            Me.xrLabel2.Name = "xrLabel2"
            Me.xrLabel2.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 96F)
            Me.xrLabel2.SizeF = New System.Drawing.SizeF(629.9998F, 23F)
            Me.xrLabel2.Text = "xrLabel2"
            ' 
            ' SampleReport
            ' 
            Me.Bands.AddRange(New DevExpress.XtraReports.UI.Band() { Me.Detail, Me.TopMargin, Me.BottomMargin})
            Me.Parameters.AddRange(New DevExpress.XtraReports.Parameters.Parameter() { Me.stringParameter})
            Me.Version = "12.2"
            DirectCast(Me, System.ComponentModel.ISupportInitialize).EndInit()

        End Sub

        #End Region

        Private Detail As DevExpress.XtraReports.UI.DetailBand
        Private TopMargin As DevExpress.XtraReports.UI.TopMarginBand
        Private BottomMargin As DevExpress.XtraReports.UI.BottomMarginBand
        Private xrLabel1 As DevExpress.XtraReports.UI.XRLabel
        Private stringParameter As DevExpress.XtraReports.Parameters.Parameter
        Private xrLabel2 As DevExpress.XtraReports.UI.XRLabel
    End Class
End Namespace
