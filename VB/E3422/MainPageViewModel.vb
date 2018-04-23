Imports System
Imports System.ComponentModel
Imports System.IO
Imports System.Linq.Expressions
Imports System.ServiceModel
Imports System.Threading.Tasks
Imports System.Windows
Imports System.Windows.Input
Imports DevExpress.DocumentServices.ServiceModel.Client
Imports DevExpress.DocumentServices.ServiceModel.DataContracts
Imports DevExpress.Mvvm
Imports DevExpress.DocumentServices.ServiceModel
Imports DevExpress.Utils
Imports DevExpress.Xpf.Core
Imports DevExpress.Xpf.Printing.Native
Imports DevExpress.XtraPrinting

Namespace E3422
    Public Class MainPageViewModel
        Implements INotifyPropertyChanged

        Private ReadOnly dialogService As IDialogService

        Private ReadOnly showPreviewCommand_Renamed As DelegateCommand(Of Object)

        Private ReadOnly exportCommand_Renamed As DelegateCommand(Of Object)

        Private ReadOnly exportToWindowCommand_Renamed As DelegateCommand(Of Object)

        Private ReadOnly printCommand_Renamed As DelegateCommand(Of Object)

        Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

        Public Property ReportName() As String
        Public Property ReportServiceUri() As String


        Private parameterValue_Renamed As String
        Public Property ParameterValue() As String
            Get
                Return parameterValue_Renamed
            End Get
            Set(ByVal value As String)
                If value <> parameterValue_Renamed Then
                    parameterValue_Renamed = value
                    RaisePropertyChanged(Function() ParameterValue)
                End If
            End Set
        End Property

        Public ReadOnly Property PrintCommand() As ICommand
            Get
                Return printCommand_Renamed
            End Get
        End Property
        Public ReadOnly Property ExportCommand() As ICommand
            Get
                Return exportCommand_Renamed
            End Get
        End Property
        Public ReadOnly Property ExportToWindowCommand() As ICommand
            Get
                Return exportToWindowCommand_Renamed
            End Get
        End Property
        Public ReadOnly Property ShowPreviewCommand() As ICommand
            Get
                Return showPreviewCommand_Renamed
            End Get
        End Property


        Private isBusy_Renamed As Boolean
        Private Property IsBusy() As Boolean
            Get
                Return isBusy_Renamed
            End Get
            Set(ByVal value As Boolean)
                If value <> isBusy_Renamed Then
                    isBusy_Renamed = value
                    printCommand_Renamed.RaiseCanExecuteChanged()
                    exportCommand_Renamed.RaiseCanExecuteChanged()
                    exportToWindowCommand_Renamed.RaiseCanExecuteChanged()
                End If
            End Set
        End Property

        Public Sub New()
            Me.New(New DialogService())
        End Sub

        Public Sub New(ByVal dialogService As IDialogService)
            Guard.ArgumentNotNull(dialogService, "dialogService")
            Me.dialogService = dialogService

            printCommand_Renamed = New DelegateCommand(Of Object)(AddressOf Print, AddressOf CanPrint)
            exportCommand_Renamed = New DelegateCommand(Of Object)(AddressOf Export, AddressOf CanExport)
            exportToWindowCommand_Renamed = New DelegateCommand(Of Object)(AddressOf ExportToWindow, AddressOf CanExportToWindow)
            showPreviewCommand_Renamed = New DelegateCommand(Of Object)(AddressOf ShowPreview, AddressOf CanShowPreview)
        End Sub

        Private Function CreateClient() As IReportServiceClient
            Dim factory As New ReportServiceClientFactory(New EndpointAddress(ReportServiceUri))
            Return factory.Create()
        End Function

        Private Function CreateParameters() As ReportParameter()
            Return New ReportParameter() { _
                New ReportParameter() With {.Path = "stringParameter", .Value = ParameterValue} _
            }
        End Function

        Private Sub RaisePropertyChanged(Of T)(ByVal [property] As Expression(Of Func(Of T)))
            PropertyExtensions.RaisePropertyChanged(Me, PropertyChangedEvent, [property])
        End Sub

        Private Function CanPrint(ByVal arg As Object) As Boolean
            Return Not IsBusy
        End Function

        Private Sub Print(ByVal obj As Object)
            IsBusy = True
            Dim printTask As Task(Of String()) = Task.Factory.PrintReportAsync(CreateClient(), ReportName, CreateParameters(), Nothing)
            printTask.ContinueWith(AddressOf PrintReportCompleted, TaskScheduler.FromCurrentSynchronizationContext())
        End Sub

        Private Sub PrintReportCompleted(ByVal task As Task(Of String()))
            IsBusy = False
            If TaskIsFauledOrCancelled(task, "Print") Then
                Return
            End If
            dialogService.AsyncRequestPrintingConfirmation(Sub(continuePrinting)
                If continuePrinting Then
                    Dim documentPrinter As New DocumentPrinter()
                    documentPrinter.Print(New XamlDocumentPaginator(task.Result), "Print Document Name")
                End If
            End Sub)
        End Sub

        Private Function CanExportToWindow(ByVal arg As Object) As Boolean
            Return (Not IsBusy) AndAlso Application.Current.Host.Settings.EnableHTMLAccess
        End Function

        Private Sub ExportToWindow(ByVal obj As Object)
            IsBusy = True
            Dim exportTask As Task(Of Uri) = Task.Factory.ExportReportForDownloadAsync(CreateClient(), ReportName, New PdfExportOptions(), CreateParameters(), Nothing)
            exportTask.ContinueWith(AddressOf ExportReportForDownloadCompleted, TaskScheduler.FromCurrentSynchronizationContext())
        End Sub

        Private Sub ExportReportForDownloadCompleted(ByVal task As Task(Of Uri))
            IsBusy = False
            If TaskIsFauledOrCancelled(task, "Export") Then
                Return
            End If
            dialogService.OpenBrowserWindow(task.Result)
        End Sub

        Private Function CanExport(ByVal arg As Object) As Boolean
            Return Not isBusy_Renamed
        End Function

        Private Sub Export(ByVal obj As Object)
            Dim stream As Stream = dialogService.ShowSaveFileDialog("PDF files (*.pdf)|*.pdf")
            If stream Is Nothing Then
                Return
            End If
            IsBusy = True
            Dim exportTask As Task(Of Byte()) = Task.Factory.ExportReportAsync(CreateClient(), ReportName, New PdfExportOptions(), CreateParameters(), stream)
            exportTask.ContinueWith(AddressOf ExportReportCompleted, TaskScheduler.FromCurrentSynchronizationContext())
        End Sub

        Private Sub ExportReportCompleted(ByVal task As Task(Of Byte()))
            IsBusy = False
            Using stream As Stream = CType(task.AsyncState, Stream)
                If TaskIsFauledOrCancelled(task, "Export") Then
                    Return
                End If
                stream.Write(task.Result, 0, task.Result.Length)
            End Using
        End Sub

        Private Function CanShowPreview(ByVal arg As Object) As Boolean
            Return True
        End Function

        Private Sub ShowPreview(ByVal obj As Object)
            dialogService.ShowPreview(ReportServiceUri, ReportName)
        End Sub

        Private Function TaskIsFauledOrCancelled(ByVal task As Task, ByVal caption As String) As Boolean
            If task.IsFaulted Then
                dialogService.ShowMessage(caption, task.Exception.Message)
                Return True
            End If

            If task.IsCanceled Then
                dialogService.ShowMessage(caption, "Operation has been cancelled")
                Return True
            End If

            Return False
        End Function
    End Class
End Namespace
