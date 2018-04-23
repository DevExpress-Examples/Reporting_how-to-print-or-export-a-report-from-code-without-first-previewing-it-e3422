Imports System
Imports System.IO

Namespace E3422
    Public Interface IDialogService
        Sub ShowPreview(ByVal serviceUri As String, ByVal reportName As String)
        Function ShowSaveFileDialog(ByVal filter As String) As Stream
        Sub ShowMessage(ByVal caption As String, ByVal message As String)
        Sub OpenBrowserWindow(ByVal uri As Uri)
        Sub AsyncRequestPrintingConfirmation(ByVal continuePrinting As Action(Of Boolean))
    End Interface
End Namespace
