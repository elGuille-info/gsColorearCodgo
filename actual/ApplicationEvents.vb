Namespace My

    ' The following events are availble for MyApplication:
    ' 
    ' Startup: Raised when the application starts, before the startup form is created.
    ' Shutdown: Raised after all application forms are closed.  This event is not raised if the application terminates abnormally.
    ' UnhandledException: Raised if the application encounters an unhandled exception.
    ' StartupNextInstance: Raised when launching a single-instance application and the application is already active. 
    ' NetworkAvailabilityChanged: Raised when the network connection is connected or disconnected.
    Partial Friend Class MyApplication

        ' Si se inicia otra instancia y está minimizada u oculta,   (04/Feb/07)
        ' mostrarla
        Private Sub MyApplication_StartupNextInstance( _
                    ByVal sender As Object, _
                    ByVal e As Microsoft.VisualBasic.ApplicationServices.StartupNextInstanceEventArgs) _
                    Handles Me.StartupNextInstance
            My.Forms.fColorear.Size = My.Forms.fColorear.RestoreBounds.Size
            My.Forms.fColorear.Location = My.Forms.fColorear.RestoreBounds.Location

            e.BringToForeground = True
            My.Forms.fColorear.Show()

        End Sub
    End Class

End Namespace

