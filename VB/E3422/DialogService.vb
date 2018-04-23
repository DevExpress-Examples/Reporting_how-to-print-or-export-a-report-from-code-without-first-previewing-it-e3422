Imports System
Imports System.IO
Imports System.Windows
Imports System.Windows.Browser
Imports System.Windows.Controls
Imports DevExpress.DocumentServices.ServiceModel.Native
Imports DevExpress.Xpf.Printing
Imports DevExpress.Xpf.Printing.Native

Namespace E3422
    Public Class DialogService
        Implements IDialogService

        Public Sub ShowPreview(ByVal serviceUri As String, ByVal reportName As String) Implements IDialogService.ShowPreview
            Dim model As New ReportServicePreviewModel(serviceUri)
            model.ReportName = reportName
            Dim preview As New DocumentPreviewWindow() With {.Model = model}
            model.CreateDocument()
            preview.ShowDialog()
        End Sub

        Public Function ShowSaveFileDialog(ByVal filter As String) As Stream Implements IDialogService.ShowSaveFileDialog
            Dim dialog As New SaveFileDialog()
            dialog.Filter = filter

            If dialog.ShowDialog() <> True Then
                Return Nothing
            End If

            Return dialog.OpenFile()
        End Function

        Public Sub ShowMessage(ByVal caption As String, ByVal message As String) Implements IDialogService.ShowMessage
            MessageBox.Show(message, If(caption, String.Empty), MessageBoxButton.OK)
        End Sub

        Public Sub OpenBrowserWindow(ByVal uri As Uri) Implements IDialogService.OpenBrowserWindow
            HtmlPage.Window.Navigate(uri, "_blank", "toolbar=0,menubar=0,resizable=1,scrollbars=1")
        End Sub

        Public Sub AsyncRequestPrintingConfirmation(ByVal continuePrinting As Action(Of Boolean)) Implements IDialogService.AsyncRequestPrintingConfirmation
            Dim window As New LoadingPrintDataWindow()

            Dim model As New LoadingPrintDataViewModel()
            model.SetStatus(PrintingStatus.Generated)
            AddHandler model.Continue, Sub(s, a)
                window.Hide()
                continuePrinting(True)
            End Sub
            AddHandler model.Cancel, Sub(s, a)
                window.Hide()
                continuePrinting(False)
            End Sub

            window.ViewModel = model
            window.Show()
        End Sub
    End Class
End Namespace
