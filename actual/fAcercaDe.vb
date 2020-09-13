'------------------------------------------------------------------------------
' Formulario Acerca de "el Guille"                                  (12/Jul/06)
' Adaptado del AcercaDe del visor de imágenes.
'
'
' ©Guillermo 'guille' Som, 2006, 2020
'------------------------------------------------------------------------------
Option Explicit On
Option Strict On

Imports Microsoft.VisualBasic
Imports vb = Microsoft.VisualBasic
Imports System
Imports System.Windows.Forms
Imports System.Drawing

Imports clickOnce = System.Deployment
Imports System.Text.RegularExpressions

Public Class fAcercaDe

    ' Datos a configurar en cada uso de este formulario:
    'Const laURL As String = "http://www.elguille.info/NET/vs2005/utilidades/gsColorearCodigo.htm"
    ' Nueva URL de la utilidad                                      (10/Sep/20)
    Const laURL As String = "http://elguille.info/NET/utilidades/gsColorearCodigo.aspx"
    ' Para mover la ventana
    Private ratonPulsado As Boolean
    Private pX, pY As Integer
    Private bugInfo As String
    Private fvi As System.Diagnostics.FileVersionInfo
    '
    Private Sub fAcercaDe_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        'Configure el texto del cuadro de diálogo en tiempo de ejecución según la información del ensamblado de la aplicación.  

        'TODO: Personalice la información del ensamblado de la aplicación en el panel "Aplicación" del cuadro de diálogo 
        '  propiedades del proyecto (bajo el menú "Proyecto").

        ' Título de la aplicación
        If My.Application.Info.Title <> "" Then
            labelTitulo.Text = My.Application.Info.Title
        Else
            ' Si falta el título de la aplicación, utilice el nombre de la aplicación sin la extensión
            labelTitulo.Text = System.IO.Path.GetFileNameWithoutExtension(My.Application.Info.AssemblyName)
        End If

        'Dé formato a la información de versión usando el texto establecido en el control de versión en tiempo de diseño como
        '  cadena de formato. Esto le permite una localización efectiva si lo desea.
        '  Se pudo incluir la información de generación y revisión usando el siguiente código y cambiando el 
        '  texto en tiempo de diseño del control de versión a "Versión {0}.{1:00}.{2}.{3}" o algo parecido. Consulte
        '  String.Format() en la Ayuda para obtener más información.
        '
        '    Version.Text = System.String.Format(Version.Text, My.Application.Info.Version.Major, My.Application.Info.Version.Minor, My.Application.Info.Version.Build, My.Application.Info.Version.Revision)

        'Version.Text = My.Application.Info.Title & " v" & My.Application.Info.Version.ToString()

        Dim ensamblado As System.Reflection.Assembly = System.Reflection.Assembly.GetExecutingAssembly
        fvi = System.Diagnostics.FileVersionInfo.GetVersionInfo(ensamblado.Location)
        bugInfo = labelTitulo.Text & " v" & fvi.FileVersion

        ' Información de Copyright
        labelAutor.Text = My.Application.Info.Copyright


        '' Mostrar la versión de la DLL de colorear                  (09/Sep/20)
        'labelDescripcion.Text = labelTitulo.Text &
        '                " v" & My.Application.Info.Version.ToString() &
        '                " (" & fvi.FileVersion & ")" & vbCrLf & vbCrLf &
        '                My.Application.Info.Description.Replace("  ", vbCrLf) & vbCrLf &
        '                ".NET Framework 4.8" & vbCrLf & vbCrLf &
        '                gsColorearNET.Colorear.Version(True)

        ' Mostrar más información de gsColorearCodigo               (12/Sep/20)
        labelDescripcion.Text = labelTitulo.Text &
                        " v" & My.Application.Info.Version.ToString() &
                        " (" & fvi.FileVersion & ")" & vbCrLf & vbCrLf &
                        My.Application.Info.Description.Replace("  ", vbCrLf) & vbCrLf &
                        My.Application.Info.Trademark.Replace("  ", vbCrLf) & vbCrLf & vbCrLf &
                        gsColorearNET.Colorear.Version(True)

        Me.labelWeb.Text = "Comprobando si hay actualizaciones..."
        timerWeb.Enabled = True
    End Sub

    Private Sub pMouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) _
                    Handles Me.MouseDown, TableLayoutPanel1.MouseDown, _
                            labelTitulo.MouseDown, labelDescripcion.MouseDown, labelAutor.MouseDown
        ' Mover el formulario mientras se mantenga el ratón pulsado
        ratonPulsado = True
        pX = e.X
        pY = e.Y
    End Sub
    Private Sub pMouseMove(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) _
                    Handles Me.MouseMove, TableLayoutPanel1.MouseMove, _
                            labelTitulo.MouseMove, labelDescripcion.MouseMove, labelAutor.MouseMove
        If ratonPulsado Then
            Me.Left += e.X - pX
            Me.Top += e.Y - pY
        End If
    End Sub
    Private Sub pMouseUp(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) _
                    Handles Me.MouseUp, TableLayoutPanel1.MouseUp, _
                            labelTitulo.MouseUp, labelDescripcion.MouseUp, labelAutor.MouseUp
        ratonPulsado = False
    End Sub

    Private Sub LinkURL_LinkClicked(sender As System.Object,
                                    e As System.Windows.Forms.LinkLabelLinkClickedEventArgs) Handles linkURL.LinkClicked
        ' Comprobar si tenemos conexión a Internet                  (26/Abr/06)
        ' Aunque podemos tener conexión a la red, pero no a Internet.
        If My.Computer.Network.IsAvailable Then
            System.Diagnostics.Process.Start(linkURL.Text)
        End If
    End Sub

    Private Sub btnAceptar_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAceptar.Click
        Me.Close()
    End Sub

    Private Sub linkBug_LinkClicked(ByVal sender As Object, ByVal e As System.Windows.Forms.LinkLabelLinkClickedEventArgs) Handles linkBug.LinkClicked
        ' Enviar un bug o mejora                                    (13/Jul/06)
        If My.Computer.Network.IsAvailable Then
            Dim sb As New System.Text.StringBuilder
            sb.AppendFormat("http://www.elguille.info/elguille_bugsmejoras.asp?subject=Bug o mejora en {0}", bugInfo)
            System.Diagnostics.Process.Start(sb.ToString)
        End If
    End Sub
    '
    Private Function versionWeb() As String
        Try
            Dim request As System.Net.WebRequest = System.Net.WebRequest.Create(laURL)
            Dim response As System.Net.WebResponse
            Dim reader As System.IO.StreamReader
            ' Obtener la respuesta.
            response = request.GetResponse()
            ' Abrir el stream de la respuesta recibida.
            reader = New System.IO.StreamReader(response.GetResponseStream())
            ' Leer el contenido.
            Dim s As String = reader.ReadToEnd()
            ' Cerrar los streams abiertos.
            reader.Close()
            response.Close()
            '
            ' Comprobar el valor de <meta name="version" 
            ' Usar esta expresión regular: <meta name="version" content="(\d.\d.\d.\d)" />
            ' En Groups(1) estará la versión
            Dim r As New Regex("<meta name=""version"" content=""(\d{1,}.\d{1,}.\d{1,}.\d{1,})"" />")
            For Each m As Match In r.Matches(s)
                If m.Groups.Count > 1 Then
                    Return m.Groups(1).Value
                End If
            Next
        Catch ex As Exception
            Return ""
        End Try
        Return ""
    End Function
    '
    Private Sub timerWeb_Tick(ByVal sender As Object, ByVal e As System.EventArgs) Handles timerWeb.Tick
        timerWeb.Enabled = False

        Dim sb As New System.Text.StringBuilder
        If clickOnce.Application.ApplicationDeployment.IsNetworkDeployed Then
            sb.Append("Aplicación instalada con ClickOnce")
            sb.AppendLine()
            Dim inf As clickOnce.Application.UpdateCheckInfo
            inf = My.Application.Deployment.CheckForDetailedUpdate
            If inf.UpdateAvailable Then
                sb.AppendFormat("Hay una nueva versión disponible: v{0}", inf.AvailableVersion.ToString)
            Else
                sb.Append("Esta es la versión más reciente, no hay nuevas actualizaciones.")
            End If
        Else
            sb.Append("Aplicación instalada directamente (sin usar ClickOnce)")
            sb.AppendLine()
            Dim vWeb As String = versionWeb()
            ' Por si la versión está vacía                          (17/Abr/07)
            If String.IsNullOrEmpty(vWeb) Then
                vWeb = "0.0.0.0"
            End If
            ' Solo funcionará bien con valores de 1 cifra           (14/Feb/07)
            ' ya que 1.0.3.11 será menor que 1.0.3.9 aunque no sea así...
            ' Convertirlo en cadena de números de dos cifras
            Dim aWeb() As String = vWeb.Split("."c)
            Dim aFic() As String = fvi.FileVersion.Split("."c)
            Dim wMayor As Boolean = False
            vWeb = ""
            Dim sFic As String = ""
            ' Para que se muestre bien, añadir puntos               (17/Abr/07)
            For i As Integer = 0 To aWeb.Length - 1
                vWeb &= CInt(aWeb(i)).ToString("00") & "."
                'sFic &= CInt(aFic(i)).ToString("00")
            Next
            For i As Integer = 0 To aFic.Length - 1
                sFic &= CInt(aFic(i)).ToString("00") & "."
            Next
            If vWeb > sFic Then ' fvi.FileVersion Then
                sb.AppendFormat("Hay una nueva versión disponible en mi sitio: v{0}", vWeb)
                'sb.AppendFormat("La última versión en la Web es: v{0}, la actual es: v{1}", vWeb, fvi.FileVersion)
            Else
                sb.Append("Esta es la versión más reciente, no hay nuevas actualizaciones.")
            End If
        End If

        Me.labelWeb.Text = sb.ToString
    End Sub

    Private Sub linkPrograma_LinkClicked(ByVal sender As System.Object,
                                         ByVal e As System.Windows.Forms.LinkLabelLinkClickedEventArgs) Handles linkPrograma.LinkClicked
        ' Ir a la página de la utilidad                             (14/Feb/07)
        If My.Computer.Network.IsAvailable Then
            System.Diagnostics.Process.Start(laURL)
        End If
    End Sub
End Class
