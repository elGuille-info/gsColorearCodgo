'------------------------------------------------------------------------------
' Utilidad para colorear el c�digo                                  (25/Ago/06)
' Usando la librer�a gsColorear
'
' Opciones de configuraci�n basadas en el de gsEditorVB
' Formulario de configuraci�n                                       (14/Nov/05)
'
' Revisi�n 1.35 Cambio en la librer�a de colorear                   (16/Ene/07)
' Revisi�n 3.15 Recordar texto, guardar RTF con su formato, etc.    (31/Mar/07)
'
' Versi�n 1.0.8.0                                                   (10/Sep/20)
'       Compilado para .NET 4.8
'       Nuevas opciones en configuraci�n para coloreado
'
' Versi�n 1.0.8.2                                                   (11/Sep/20)
'       Utiliza la DLL ColorearNET compilada con .NET Standard 2.0
'       Tambi�n utiliza nombre seguro (tanto la DLL como la utilidad)
'
' versi�n 1.0.8.3   Usando el paquete de NuGet de gsColorearNET     (11/Sep/20)
' versi�n 1.0.8.4   Por actualizaci�n de gsColorearNET              (12/Sep/20)
'   Esta versi�n la convierto a .NET 5.0 Preview 8
' versi�n 1.0.8.5   Por actualizaci�n de gsColorearNET              (12/Sep/20)
' versi�n 1.0.8.6   Por actualizaci�n de gsColorearNET              (13/Sep/20)
' versi�n 1.0.8.7   Por actualizaci�n de gsColorearNET              (16/Sep/20)
' versi�n 1.0.8.8   Por actualizaci�n de gsColorearNET              (18/Sep/20)
'
' �Guillermo 'guille' Som, 2005-2007, 2020
'
' Para ClickOnce, que se borra y no siempre lo recuerda:
' E:\gsCodigo_00\VS2005\clickonce_pub\gsColorearCodigo\
' http://www.elguille.info/NET/clickonce_pub/gsColorearCodigo/
'------------------------------------------------------------------------------
Option Strict On
Option Explicit On
Option Infer On

Imports System
Imports Microsoft.VisualBasic
Imports System.Windows.Forms
Imports System.Drawing
Imports System.Linq

Imports System.Collections.Generic

' Para no tener conflictos con otras DLL m�as,                      (26/Ago/06)
' todas las clases est�n en este espacio de nombres.
Imports gsColorearNET ' elGuille.Util.Developer

Public Class fColorear

    Private usarTemaOscuro As CheckState = CheckState.Unchecked

    Private m_textoSin As String
    Private Property textoSin() As String
        Get
            Return m_textoSin
        End Get
        Set(ByVal value As String)
            If String.IsNullOrEmpty(value) = False Then
                My.Settings.ultimoTexto = value
            End If
            m_textoSin = value
        End Set
    End Property
    Private sincronizando As Boolean = True
    Private contextFic As New ContextMenu
    Private cambiarCase As Boolean
    '
    Private lenguaje As Lenguajes = Lenguajes.dotNet
    Private inicializando As Boolean = True
    Private indentar As Integer = 4
    Private cfg As gsColorearNET.Config
    Private fvi As System.Diagnostics.FileVersionInfo
    Private ni As NotifyIcon
    Private minimizarTray As Boolean '= False
    ' Solo para usarlo en aplicar de config                         (31/Mar/07)
    Private recordarUltimoTexto As Boolean
    '
    '
    Private m_kColorDeFondo As KnownColor             ' Color de fondo a usar en las ventanas
    '
    Public Property kColorDeFondo() As KnownColor
        Get
            Return m_kColorDeFondo
        End Get
        Set(ByVal value As KnownColor)
            m_kColorDeFondo = value
            ' Aplicar el color a los paneles
            Me.BackColor = colorDeFondo
        End Set
    End Property

    Private Function nombreColorDeFondo() As String
        Return System.Enum.GetName(m_kColorDeFondo.GetType, kColorDeFondo)
    End Function

    Private Function knownColorFromName(ByVal nombre As String) As KnownColor
        Return CType(System.Enum.Parse(m_kColorDeFondo.GetType, nombre), KnownColor)
    End Function

    Private Property colorDeFondo() As Color
        Get
            Return Color.FromKnownColor(m_kColorDeFondo)
        End Get
        Set(ByVal value As Color)
            m_kColorDeFondo = value.ToKnownColor
        End Set
    End Property
    '
    '
    Public Sub New()

        ni = New NotifyIcon
        ' No guardar autom�ticamente los datos al asignarlos    (21/Feb/06)
        cfg = New gsColorearNET.Config(Application.ExecutablePath & ".cfg", False)
        fvi = System.Diagnostics.FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly.Location)

        'El Dise�ador de Windows Forms requiere esta llamada.
        InitializeComponent()

        ' Agregue cualquier inicializaci�n despu�s de la llamada a InitializeComponent().

    End Sub
    '
    Private Sub abrir(ByVal fic As String)
        '
        Using sr As New System.IO.StreamReader(fic, System.Text.Encoding.Default, True)
            Me.rtEditor.Text = sr.ReadToEnd()
        End Using
        '
        Me.Text = Me.Tag.ToString & " [" & System.IO.Path.GetFileName(fic) & "]"
        '
        ' Marcar el lenguaje seg�n la extensi�n
        Dim ext As String = System.IO.Path.GetExtension(fic).ToLower() & ";"
        Dim mnu As ToolStripMenuItem
        For Each mnu In mnuSintax.DropDownItems
            mnu.Checked = False
        Next
        ' Empezar por el "Ninguno" (el 3)                           (29/Ago/06)
        For i As Integer = 3 To tsbSintax.DropDownItems.Count - 1
            mnu = TryCast(tsbSintax.DropDownItems(i), ToolStripMenuItem)
            If mnu IsNot Nothing Then
                mnu.Checked = False
            End If
        Next
        '
        Dim le As Lenguajes
        Dim s As String
        If PalabrasClave.Extension(Lenguajes.CS) <> "" _
                AndAlso (PalabrasClave.Extension(Lenguajes.CS) & ";").IndexOf(ext) > -1 Then
            le = Lenguajes.CS
            s = "mnuSintax_CS"
        ElseIf PalabrasClave.Extension(Lenguajes.VB) <> "" _
                AndAlso (PalabrasClave.Extension(Lenguajes.VB) & ";").IndexOf(ext) > -1 Then
            le = Lenguajes.VB
            s = "mnuSintax_VB"
        ElseIf PalabrasClave.Extension(Lenguajes.VB6) <> "" _
                AndAlso (PalabrasClave.Extension(Lenguajes.VB6) & ";").IndexOf(ext) > -1 Then
            le = Lenguajes.VB6
            s = "mnuSintax_VB6"
        ElseIf PalabrasClave.Extension(Lenguajes.dotNet) <> "" _
                AndAlso (PalabrasClave.Extension(Lenguajes.dotNet) & ";").IndexOf(ext) > -1 Then
            le = Lenguajes.dotNet
            s = "mnuSintax_dotNet"
        ElseIf PalabrasClave.Extension(Lenguajes.Java) <> "" _
                AndAlso (PalabrasClave.Extension(Lenguajes.Java) & ";").IndexOf(ext) > -1 Then
            le = Lenguajes.Java
            s = "mnuSintax_Java"
        ElseIf PalabrasClave.Extension(Lenguajes.FSharp) <> "" _
                AndAlso (PalabrasClave.Extension(Lenguajes.FSharp) & ";").IndexOf(ext) > -1 Then
            le = Lenguajes.FSharp
            s = "mnuSintax_FSharp"
        ElseIf PalabrasClave.Extension(Lenguajes.SQL) <> "" _
                AndAlso (PalabrasClave.Extension(Lenguajes.SQL) & ";").IndexOf(ext) > -1 Then
            le = Lenguajes.SQL
            s = "mnuSintax_SQL"
        ElseIf PalabrasClave.Extension(Lenguajes.CPP) <> "" _
                AndAlso (PalabrasClave.Extension(Lenguajes.CPP) & ";").IndexOf(ext) > -1 Then
            le = Lenguajes.CPP
            s = "mnuSintax_CPP"
        ElseIf PalabrasClave.Extension(Lenguajes.Pascal) <> "" _
                AndAlso (PalabrasClave.Extension(Lenguajes.Pascal) & ";").IndexOf(ext) > -1 Then
            le = Lenguajes.Pascal
            s = "mnuSintax_Pascal"
        ElseIf PalabrasClave.Extension(Lenguajes.IL) <> "" _
                AndAlso (PalabrasClave.Extension(Lenguajes.IL) & ";").IndexOf(ext) > -1 Then
            le = Lenguajes.IL
            s = "mnuSintax_IL"
        ElseIf PalabrasClave.Extension(Lenguajes.XML) <> "" _
                AndAlso (PalabrasClave.Extension(Lenguajes.XML) & ";").IndexOf(ext) > -1 Then
            le = Lenguajes.XML
            s = "mnuSintax_XML"
        Else
            le = Lenguajes.Ninguno
            s = "mnuSintax_Ninguno"
        End If
        lenguaje = le
        '
        DirectCast(mnuSintax.DropDownItems(s), ToolStripMenuItem).Checked = True
        DirectCast(tsbSintax.DropDownItems(s), ToolStripMenuItem).Checked = True
        '
        Me.statusSintax.Text = lenguaje.ToString()
        '
    End Sub
    '
    Private Sub guardar(ByVal fic As String)
        Using sw As New System.IO.StreamWriter(fic, False, System.Text.Encoding.Default)
            sw.WriteLine(Me.rtEditor.Text)
        End Using
    End Sub
    '
    ' Nuevas opciones de ver y colorear                             (08/Feb/07)
    Private Sub verRTF(ByVal sender As Object, ByVal e As EventArgs)
        ' Mostrar el c�digo RTF del editor
        Dim fv As New fVerRTF
        fv.Texto = rtEditor.Rtf
        fv.Show()
        fv.BringToFront()
    End Sub
    Private Sub convertirDeRTF(ByVal sender As Object, ByVal e As EventArgs) Handles mnuFicColorearDeRTF.Click
        ' Convertir el c�digo RTF en coloreado de SPAN
        Me.statusInfo.Text = "Coloreando el c�digo..."
        Me.statusStrip1.Refresh()
        '
        If String.IsNullOrEmpty(rtEditor.Rtf) = False _
                AndAlso rtEditor.Rtf.TrimStart().StartsWith("{\rtf") Then
            textoSin = Me.rtEditor.Rtf
        Else
            textoSin = Me.rtEditor.Text
            Me.statusInfo.Text = "El c�digo debe estar en formato RTF."
            Me.statusStrip1.Refresh()
            Exit Sub
        End If
        Me.btnTextoNormal.Enabled = True

        ' Para colorear en la misma ventana
        ' NO usar el valor de indentar, que se l�a
        ' ya que tiene las etiquetas <span.
        'If Me.chkIndentar.Checked Then
        '    Me.rtEditor.Text = Colorear.RTFaSPAN( _
        '                Me.rtEditor.Rtf, _
        '                Me.indentar, _
        '                Me.chkQuitarEspacios.Checked)
        'Else
        '    Me.rtEditor.Text = Colorear.RTFaSPAN( _
        '                Me.rtEditor.Rtf, _
        '                0, _
        '                Me.chkQuitarEspacios.Checked)
        'End If
        Me.rtEditor.Text = Colorear.RTFaSPAN( _
                    Me.rtEditor.Rtf, _
                    0, _
                    Me.chkQuitarEspacios.Checked)
        guardarTEMP(Me.rtEditor.Text)
        Me.statusStrip1.Refresh()
        '
        ''My.Forms.fVerRTF.Texto = Colorear.RTFaSPAN(Me.rtEditor.Rtf)
        ''My.Forms.fVerRTF.Show()
        'Dim fv As New fVerRTF
        'fv.Texto = Me.rtEditor.Text ' Colorear.RTFaSPAN(Me.rtEditor.Rtf)
        'fv.Show()
        'fv.BringToFront()
    End Sub
    '
    Private Sub inicializar()
        inicializando = True
        '
        Me.cboFuentes.Text = Colorear.Fuente
        Me.cboTamFuente.Text = Colorear.FuenteTam
        'me.Recientes

        ' Las opciones de tsbSintax para los lenguajes
        For i As Integer = 0 To Me.mnuSintax.DropDownItems.Count - 1
            Dim tsi As ToolStripItem = mnuSintax.DropDownItems(i)
            If TypeOf tsi Is ToolStripMenuItem Then
                Me.tsbSintax.DropDownItems.Add(clonarToolStripMenuItem( _
                                DirectCast(tsi, ToolStripMenuItem), _
                                AddressOf mnuSintax_Click))
            End If
        Next

        Dim rtContext As New ContextMenuStrip
        rtContext.Items.Add(clonarToolStripMenuItem(Me.mnuEdiDeshacer, _
                        AddressOf mnuEdiDeshacer_Click)) ' AddressOf mnuEdi_Select))
        rtContext.Items.Add("-")
        rtContext.Items.Add(clonarToolStripMenuItem(Me.mnuEdiCortar, _
                        AddressOf mnuEdiCortar_Click)) ' AddressOf mnuEdi_Select))
        rtContext.Items.Add(clonarToolStripMenuItem(Me.mnuEdiCopiar, _
                        AddressOf mnuEdiCopiar_Click)) ' AddressOf mnuEdi_Select))
        rtContext.Items.Add(clonarToolStripMenuItem(Me.mnuEdiPegar, _
                        AddressOf mnuEdiPegar_Click)) ' AddressOf mnuEdi_Select))
        rtContext.Items.Add("-")
        rtContext.Items.Add(clonarToolStripMenuItem(Me.mnuEdiSeleccionarTodo, _
                        AddressOf mnuEdiSeleccionarTodo_Click)) ' AddressOf mnuEdi_Select))
        '
        ' Mostrar el c�digo RTF del texto                           (08/Feb/07)
        rtContext.Items.Add("-")
        Dim tsit As ToolStripItem
        tsit = rtContext.Items.Add("Ver RTF", Nothing, AddressOf verRTF)
        tsit.Name = "mnuVerRTF"
        tsit = rtContext.Items.Add("Colorear desde RTF", Nothing, AddressOf convertirDeRTF)
        tsit.Name = "mnuColorearDeRTF"
        AddHandler rtContext.Opening, AddressOf mnuEdi_Opening
        rtEditor.ContextMenuStrip = rtContext
        '
        ' A�adir men� contextual al icono
        rtContext = New ContextMenuStrip
        rtContext.Items.Add("&Restaurar", Nothing, AddressOf restaurarForm)
        ' A�adir la opci�n de ocultar al minimizar                  (17/Nov/06)
        rtContext.Items.Add("-")
        Dim mnuTSi As New ToolStripMenuItem("Ocultar al minimizar", Nothing, AddressOf mnuOcultarMini_Click, "mnuOcultarMini")
        mnuTSi.Checked = minimizarTray
        rtContext.Items.Add(mnuTSi)
        rtContext.Items.Add("-")
        rtContext.Items.Add(clonarToolStripMenuItem(Me.mnuFicAcerca, AddressOf mnuFicAcerca_Click))
        rtContext.Items.Add("-")
        rtContext.Items.Add(clonarToolStripMenuItem(Me.mnuFicSalir, AddressOf mnuFicSalir_Click))
        rtContext.Items(0).Font = New Font(rtContext.Items(0).Font, FontStyle.Bold) ' Default
        ni.ContextMenuStrip = rtContext
        ' Asignar el evento DobleClick
        AddHandler ni.DoubleClick, AddressOf restaurarForm
        ni.Text = "gsColorearCodigo"
        ni.Icon = Me.Icon
        ni.Visible = True ' False
        '
        ' Asignar el tama�o y �ltima posici�n
        Me.Left = cfg.GetValue("Ventana", "Left", Me.Left)
        Me.Top = cfg.GetValue("Ventana", "Top", Me.Top)
        Me.Height = cfg.GetValue("Ventana", "Height", Me.Height)
        Me.Width = cfg.GetValue("Ventana", "Width", Me.Width)
        '
        inicializando = False
    End Sub
    '
    Private Sub mnuOcultarMini_Click(ByVal sender As Object, ByVal e As EventArgs)
        Dim mnuOcultarMini As ToolStripMenuItem
        mnuOcultarMini = DirectCast(ni.ContextMenuStrip.Items("mnuOcultarMini"), ToolStripMenuItem)
        mnuOcultarMini.Checked = Not mnuOcultarMini.Checked
        minimizarTray = mnuOcultarMini.Checked
        Me.chkNotify.Checked = minimizarTray
        If Me.WindowState = FormWindowState.Minimized Then
            If mnuOcultarMini.Checked Then
                Me.Hide()
            Else
                Me.Show()
                ' Sin el bringToFront no se muestra en la barra de tareas
                Me.BringToFront()
            End If
        End If
    End Sub
    '
    ' Los tres botones para los datos de configuraci�n.             (26/Ago/06)
    ' Se usar�n para todas las fichas de configuraci�n.
    Private Sub btnCfgAplicar_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCfgAplicar.Click
        ' Guardar los valores en la configuraci�n
        guardarCfg_tpColores()
        datosCambiados()
    End Sub

    Private Sub btnCfgDeshacer_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCfgDeshacer.Click
        ' Leer los valores de la configuraci�n y asignarlos
        leerCfg_tpColores()
        datosCambiados()
    End Sub

    Private Sub btnCfgRestablecer_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCfgRestablecer.Click
        restablecerCfg_tpColores()
        datosCambiados()
    End Sub

    Private Sub datosCambiados()
        Dim b As Boolean = False
        If sincronizando Then
            Me.btnCfgAplicar.Enabled = False
            Me.btnCfgDeshacer.Enabled = False
            Return
        End If

        If chkRecordarUltimoTexto.Checked <> recordarUltimoTexto Then b = True

        If cboColores.Text <> nombreColorDeFondo() Then b = True

        If Me.chkNotify.Checked <> minimizarTray Then b = True
        If chkSyntaxMayusc.Checked <> cambiarCase Then b = True

        If Colorear.ColorInstrucciones <> txtColorKeywords.Text Then b = True
        If Colorear.ColorComentarios <> txtColorRem.Text Then b = True
        If Colorear.ColorTexto <> txtColorTexto.Text Then b = True
        If Colorear.ColorDocXML <> txtColorXML.Text Then b = True
        If Colorear.ColorClases <> txtColorClases.Text Then b = True
        If Colorear.PreTag <> cboPre.Text Then b = True

        If cboFuentes.Text <> Colorear.Fuente Then b = True
        If cboTamFuente.Text <> Colorear.FuenteTam Then b = True
        If Colorear.UsarSpanStyle <> chkUsarSpanStyle.Checked Then b = True

        If chkUsarTemaOscuro.CheckState <> usarTemaOscuro Then b = True

        Me.btnCfgAplicar.Enabled = b
        Me.btnCfgDeshacer.Enabled = b
        ' Restablecer siempre est� disponible
    End Sub
    '
    Private Sub restablecerCfg_tpColores()
        minimizarTray = cfg.GetValue("General", "minimizarTray", minimizarTray)
        Me.chkNotify.Checked = minimizarTray
        cambiarCase = cfg.GetValue("Colorear", "CambiarCase", False)
        Me.chkSyntaxMayusc.Checked = cambiarCase
        '
        Me.kColorDeFondo = Me.knownColorFromName("Lavender")
        cboColores.Text = nombreColorDeFondo()
        '
        chkUsarSpanStyle.Checked = Colorear.UsarSpanStylePre

        chkUsarTemaOscuro.CheckState = usarTemaOscuro

        If chkUsarTemaOscuro.CheckState = CheckState.Checked Then
            Me.txtColorKeywords.Text = Colorear.ColorInstruccionesOscuroPre.Substring(2)
            Me.txtColorRem.Text = Colorear.ColorComentariosOscuroPre.Substring(2)
            Me.txtColorTexto.Text = Colorear.ColorTextoOscuroPre.Substring(2)
            Me.txtColorXML.Text = Colorear.ColorDocXMLOscuroPre.Substring(2)
            Me.txtColorClases.Text = Colorear.ColorClasesOscuroPre.Substring(2)
            Me.cboPre.Text = Colorear.PreTagOscuroPre
        ElseIf chkUsarTemaOscuro.CheckState = CheckState.Unchecked Then
            Me.txtColorKeywords.Text = Colorear.ColorInstruccionesPre.Substring(2)
            Me.txtColorRem.Text = Colorear.ColorComentariosPre.Substring(2)
            Me.txtColorTexto.Text = Colorear.ColorTextoPre.Substring(2)
            Me.txtColorXML.Text = Colorear.ColorDocXMLPre.Substring(2)
            Me.txtColorClases.Text = Colorear.ColorClasesPre.Substring(2)
            Me.cboPre.Text = Colorear.PreTagPre
        Else
            Me.txtColorKeywords.Text = Colorear.ColorInstrucciones
            Me.txtColorRem.Text = Colorear.ColorComentarios
            Me.txtColorTexto.Text = Colorear.ColorTexto
            Me.txtColorXML.Text = Colorear.ColorDocXML
            Me.txtColorClases.Text = Colorear.ColorClases
            Me.cboPre.Text = Colorear.PreTag
        End If

        cboFuentes.Text = Colorear.FuentePre
        cboTamFuente.Text = Colorear.FuenteTamPre
        ' Guardar los valores en la configuraci�n
        guardarCfg_tpColores()
        datosCambiados()
    End Sub
    Private Sub guardarCfg_tpColores()
        minimizarTray = Me.chkNotify.Checked
        ' Actualizar el men� de minimizar                           (04/Feb/07)
        Dim mnuOcultarMini As ToolStripMenuItem
        mnuOcultarMini = DirectCast(ni.ContextMenuStrip.Items("mnuOcultarMini"), ToolStripMenuItem)
        mnuOcultarMini.Checked = minimizarTray
        '
        cambiarCase = Me.chkSyntaxMayusc.Checked
        '
        kColorDeFondo = knownColorFromName(cboColores.Text)
        '
        Colorear.ColorInstrucciones = txtColorKeywords.Text
        Colorear.ColorComentarios = txtColorRem.Text
        Colorear.ColorTexto = txtColorTexto.Text
        Colorear.ColorDocXML = txtColorXML.Text
        Colorear.ColorClases = txtColorClases.Text
        Colorear.PreTag = cboPre.Text

        Colorear.Fuente = cboFuentes.Text
        Colorear.FuenteTam = cboTamFuente.Text
        Colorear.UsarSpanStyle = chkUsarSpanStyle.Checked

        usarTemaOscuro = chkUsarTemaOscuro.CheckState
        datosCambiados()
    End Sub
    Private Sub leerCfg_tpColores()
        inicializando = True

        Me.chkNotify.Checked = minimizarTray
        Me.chkSyntaxMayusc.Checked = cambiarCase
        '
        cboColores.Text = nombreColorDeFondo()
        '
        txtColorKeywords.Text = Colorear.ColorInstrucciones
        txtColorRem.Text = Colorear.ColorComentarios
        txtColorTexto.Text = Colorear.ColorTexto
        txtColorXML.Text = Colorear.ColorDocXML
        txtColorClases.Text = Colorear.ColorClases
        cboPre.Text = Colorear.PreTag

        cboFuentes.Text = Colorear.Fuente
        cboTamFuente.Text = Colorear.FuenteTam
        chkUsarSpanStyle.Checked = Colorear.UsarSpanStyle

        chkUsarTemaOscuro.CheckState = usarTemaOscuro
        datosCambiados()

        inicializando = False
    End Sub
    '
    Private Sub guardarCfg()
        cfg.SetValue("General", "minimizarTray", minimizarTray)
        indentar = CInt(Me.txtIndentar.Value)
        cfg.SetValue("General", "indentar", indentar)
        '
        cfg.SetValue("General", "colorDeFondo", Me.nombreColorDeFondo)
        '
        cfg.SetValue("General", "chkQuitarEspacios", Me.chkQuitarEspacios.Checked)
        cfg.SetValue("General", "chkIndentar", Me.chkIndentar.Checked)
        '
        cfg.SetValue("Colorear", "CambiarCase", cambiarCase)
        '
        cfg.RemoveSection("Colorear")
        cfg.SetKeyValue("Colorear", "Lenguaje", lenguaje.ToString)
        cfg.SetKeyValue("Colorear", "FormatoRTF", Me.tbSintaxColorearRTF.Checked)
        '
        cfg.SetKeyValue("Colorear_" & Lenguajes.CS.ToString, "Seleccionado", Me.mnuSintax_CS.Checked)
        cfg.SetKeyValue("Colorear_" & Lenguajes.dotNet.ToString, "Seleccionado", Me.mnuSintax_dotNet.Checked)
        cfg.SetKeyValue("Colorear_" & Lenguajes.VB.ToString, "Seleccionado", Me.mnuSintax_VB.Checked)
        cfg.SetKeyValue("Colorear_" & Lenguajes.VB6.ToString, "Seleccionado", Me.mnuSintax_VB6.Checked)
        cfg.SetKeyValue("Colorear_" & Lenguajes.Java.ToString, "Seleccionado", Me.mnuSintax_Java.Checked)
        cfg.SetKeyValue("Colorear_" & Lenguajes.FSharp.ToString, "Seleccionado", Me.mnuSintax_FSharp.Checked)
        cfg.SetKeyValue("Colorear_" & Lenguajes.SQL.ToString, "Seleccionado", Me.mnuSintax_SQL.Checked)
        cfg.SetKeyValue("Colorear_" & Lenguajes.CPP.ToString, "Seleccionado", Me.mnuSintax_CPP.Checked)
        cfg.SetKeyValue("Colorear_" & Lenguajes.Pascal.ToString, "Seleccionado", Me.mnuSintax_Pascal.Checked)
        cfg.SetKeyValue("Colorear_" & Lenguajes.IL.ToString, "Seleccionado", Me.mnuSintax_IL.Checked)
        cfg.SetKeyValue("Colorear_" & Lenguajes.XML.ToString, "Seleccionado", Me.mnuSintax_XML.Checked)
        '
        '
        ' Usar el tema oscuro                                       (10/Sep/20)
        ' Para usar la DLL de .NET Standard 2.0                     (11/Sep/20)
        cfg.SetValue("Colorear", "UsarTemaOscuro", chkUsarTemaOscuro.CheckState.ToString)

        ' Los colores se guardan en el formato de HTML
        ' hacer esta asignaci�n para que se usen los colores definidos
        ' ya que si el formato fuese RTF se devolver�a en el formato \redDD\greenDD\blueDD
        Colorear.FormatoColoreado = Colorear.FormatosColoreado.HTML
        ' Para usar <span style en lugar de <font color        (06/Abr/06)
        cfg.SetKeyValue("Tags", "UsarSpanStyle", Colorear.UsarSpanStyle)

        ' Guardar los colores en Tag
        cfg.SetKeyValue("Tags", "ColorInstrucciones", Colorear.ColorInstrucciones)
        cfg.SetKeyValue("Tags", "ColorComentarios", Colorear.ColorComentarios)
        cfg.SetKeyValue("Tags", "ColorTexto", Colorear.ColorTexto)
        cfg.SetKeyValue("Tags", "ColorDocXML", Colorear.ColorDocXML)
        cfg.SetKeyValue("Tags", "ColorClases", Colorear.ColorClases)
        cfg.SetKeyValue("Tags", "TagPre", Colorear.PreTag)

        ' Guardar los temas claro y oscuro para tenerlo como copia
        cfg.SetKeyValue("TemaOscuro", "ColorInstrucciones", Colorear.ColorInstruccionesOscuroPre.Substring(2))
        cfg.SetKeyValue("TemaOscuro", "ColorComentarios", Colorear.ColorComentariosOscuroPre.Substring(2))
        cfg.SetKeyValue("TemaOscuro", "ColorTexto", Colorear.ColorTextoOscuroPre.Substring(2))
        cfg.SetKeyValue("TemaOscuro", "ColorDocXML", Colorear.ColorDocXMLOscuroPre.Substring(2))
        cfg.SetKeyValue("TemaOscuro", "ColorClases", Colorear.ColorClasesOscuroPre.Substring(2))
        cfg.SetKeyValue("TemaOscuro", "TagPre", Colorear.PreTagOscuroPre)

        cfg.SetKeyValue("TemaClaro", "ColorInstrucciones", Colorear.ColorInstruccionesPre.Substring(2))
        cfg.SetKeyValue("TemaClaro", "ColorComentarios", Colorear.ColorComentariosPre.Substring(2))
        cfg.SetKeyValue("TemaClaro", "ColorTexto", Colorear.ColorTextoPre.Substring(2))
        cfg.SetKeyValue("TemaClaro", "ColorDocXML", Colorear.ColorDocXMLPre.Substring(2))
        cfg.SetKeyValue("TemaClaro", "ColorClases", Colorear.ColorClasesPre.Substring(2))
        cfg.SetKeyValue("TemaClaro", "TagPre", Colorear.PreTagPre)

        cfg.SetKeyValue("Fuente", "Family", Colorear.Fuente)
        cfg.SetKeyValue("Fuente", "Size", Colorear.FuenteTam)

        ' El �ltimo texto se guarda en My.Settings.ultimoTexto      (31/Mar/07)
        cfg.SetValue("General", "RecordarUltimoTexto", Me.chkRecordarUltimoTexto.Checked)

        ' Los elementos del combo de los <pre>                      (10/Sep/20)
        cfg.SetValue("PreItems", "Total", cboPre.Items.Count.ToString)
        For i = 0 To cboPre.Items.Count - 1
            cfg.SetValue("PreItems", $"n{i}", cboPre.Items(i).ToString)
        Next

        cfg.Save()
    End Sub

    Private Sub leerCfg()
        ' Leer el fichero de configuraci�n para saber el idioma seleccionado
        minimizarTray = cfg.GetValue("General", "minimizarTray", minimizarTray)
        Me.chkNotify.Checked = minimizarTray
        indentar = cfg.GetValue("General", "indentar", 4)
        Me.txtIndentar.Value = indentar
        '
        Me.kColorDeFondo = Me.knownColorFromName(cfg.GetValue("General", "colorDeFondo", "Lavender"))
        '
        Me.chkQuitarEspacios.Checked = cfg.GetValue("General", "chkQuitarEspacios", False)
        Me.chkIndentar.Checked = cfg.GetValue("General", "chkIndentar", False)
        '
        cambiarCase = cfg.GetValue("Colorear", "CambiarCase", False)
        Me.chkSyntaxMayusc.Checked = cambiarCase
        '
        lenguaje = CType(System.Enum.Parse(GetType(Lenguajes), cfg.GetValue("Colorear", "Lenguaje", lenguaje.ToString)), Lenguajes)
        Me.statusSintax.Text = lenguaje.ToString()
        '
        Me.tbSintaxColorearRTF.Checked = cfg.GetValue("Colorear", "FormatoRTF", False)
        ' Opci�n de Colorear en RTF en el men� fichero              (12/Sep/20)
        Me.mnuFicColorearEnRTF.Checked = Me.tbSintaxColorearRTF.Checked
        If Me.tbSintaxColorearRTF.Checked Then
            Me.btnColorear.Text = "Colorear en RTF"
        Else
            Me.btnColorear.Text = "Colorear en HTML"
        End If
        Me.tbSintaxColorearHTML.Text = Me.btnColorear.Text
        Me.mnuFicColorear.Text = Me.btnColorear.Text
        '
        mnuSintax_dotNet.Checked = cfg.GetValue("Colorear_" & Lenguajes.dotNet.ToString, "Seleccionado", True)
        mnuSintax_CS.Checked = cfg.GetValue("Colorear_" & Lenguajes.CS.ToString, "Seleccionado", False)
        mnuSintax_VB.Checked = cfg.GetValue("Colorear_" & Lenguajes.VB.ToString, "Seleccionado", False)
        mnuSintax_VB6.Checked = cfg.GetValue("Colorear_" & Lenguajes.VB6.ToString, "Seleccionado", False)
        mnuSintax_Java.Checked = cfg.GetValue("Colorear_" & Lenguajes.Java.ToString, "Seleccionado", False)
        mnuSintax_FSharp.Checked = cfg.GetValue("Colorear_" & Lenguajes.FSharp.ToString, "Seleccionado", False)
        mnuSintax_SQL.Checked = cfg.GetValue("Colorear_" & Lenguajes.SQL.ToString, "Seleccionado", False)
        mnuSintax_CPP.Checked = cfg.GetValue("Colorear_" & Lenguajes.CPP.ToString, "Seleccionado", False)
        mnuSintax_Pascal.Checked = cfg.GetValue("Colorear_" & Lenguajes.Pascal.ToString, "Seleccionado", False)
        mnuSintax_IL.Checked = cfg.GetValue("Colorear_" & Lenguajes.IL.ToString, "Seleccionado", False)
        mnuSintax_XML.Checked = cfg.GetValue("Colorear_" & Lenguajes.XML.ToString, "Seleccionado", False)
        asignarPalabrasClave()
        '
        '
        ' Si se usa el tema oscuro                                  (10/Sep/20)
        'chkUsarTemaOscuro.CheckState = cfg.GetValue("Colorear", "UsarTemaOscuro", CheckState.Unchecked)
        ' Para usar la DLL de .NET Standard 2.0                     (11/Sep/20)
        Dim temOsc = cfg.GetValue("Colorear", "UsarTemaOscuro", CheckState.Unchecked.ToString)
        Select Case temOsc
            Case CheckState.Checked.ToString
                chkUsarTemaOscuro.CheckState = CheckState.Checked
            Case CheckState.Unchecked.ToString
                chkUsarTemaOscuro.CheckState = CheckState.Unchecked
            Case CheckState.Indeterminate.ToString
                chkUsarTemaOscuro.CheckState = CheckState.Indeterminate
        End Select
        ' Esto da error ya que es una cadena
        'chkUsarTemaOscuro.CheckState = CType(cfg.GetValue("Colorear", "UsarTemaOscuro", CheckState.Unchecked.ToString), CheckState)

        ' Los colores se guardan en el formato de HTML
        Colorear.FormatoColoreado = Colorear.FormatosColoreado.HTML
        Colorear.UsarSpanStyle = cfg.GetValue("Tags", "UsarSpanStyle", Colorear.UsarSpanStylePre)

        Colorear.ColorInstrucciones = cfg.GetValue("Tags", "ColorInstrucciones", Colorear.ColorInstruccionesPre.Substring(2))
        Colorear.ColorComentarios = cfg.GetValue("Tags", "ColorComentarios", Colorear.ColorComentariosPre.Substring(2))
        Colorear.ColorTexto = cfg.GetValue("Tags", "ColorTexto", Colorear.ColorTextoPre.Substring(2))
        Colorear.ColorDocXML = cfg.GetValue("Tags", "ColorDocXML", Colorear.ColorDocXMLPre.Substring(2))
        Colorear.ColorClases = cfg.GetValue("Tags", "ColorClases", Colorear.ColorClasesPre.Substring(2))
        Colorear.PreTag = cfg.GetValue("Tags", "TagPre", Colorear.PreTagPre)

        Colorear.Fuente = cfg.GetValue("Fuente", "Family", Colorear.FuentePre)
        Colorear.FuenteTam = cfg.GetValue("Fuente", "Size", Colorear.FuenteTamPre)
        Me.cboFuentes.Text = Colorear.Fuente
        Me.cboTamFuente.Text = Colorear.FuenteTam

        ' El �ltimo texto se guarda en My.Settings.ultimoTexto      (31/Mar/07)
        Me.chkRecordarUltimoTexto.Checked = cfg.GetValue("General", "RecordarUltimoTexto", True)
        recordarUltimoTexto = Me.chkRecordarUltimoTexto.Checked

        ' Los elementos del combo de los <pre>                      (10/Sep/20)
        Dim tPre = cfg.GetValue("PreItems", "Total", 0)
        ' dejar los items que haya, solo a�adir los que no est�n ya
        ' con idea de que no se borren los predeterminados en el programa
        For i = 0 To tPre - 1 'cboPre.Items.Count - 1
            Dim sPre = cfg.GetValue("PreItems", $"n{i}", "")
            If cboPre.Items.Contains(sPre) = False Then
                cboPre.Items.Add(sPre)
            End If
        Next

    End Sub
    '
    '
    Private Sub restaurarForm(ByVal sender As Object, ByVal e As EventArgs)
        Show()
        WindowState = FormWindowState.Normal
        BringToFront()
    End Sub
    '
    Private Sub fColorear_DragDrop( _
                    ByVal sender As Object, ByVal e As DragEventArgs) _
                    Handles Me.DragDrop, rtEditor.DragDrop
        If e.Data.GetDataPresent(DataFormats.FileDrop) Then
            Dim sFic As String
            sFic = CType(e.Data.GetData("FileDrop", True), String())(0)
            abrir(sFic)
        End If
    End Sub

    Private Sub fColorear_DragEnter( _
                    ByVal sender As Object, ByVal e As DragEventArgs) _
                    Handles Me.DragEnter, rtEditor.DragEnter
        ' Drag & Drop, comprobar con DataFormats
        If e.Data.GetDataPresent(DataFormats.FileDrop) Then
            e.Effect = DragDropEffects.Copy
        End If
    End Sub

    Private Sub fColorear_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        inicializando = True
        guardarCfg()
        ni.Visible = False
    End Sub
    '
    Private Sub fColorear_Move(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Move
        If inicializando = False AndAlso Me.WindowState = FormWindowState.Normal Then
            cfg.SetKeyValue("Ventana", "Left", Me.Left)
            cfg.SetKeyValue("Ventana", "Top", Me.Top)
            cfg.SetKeyValue("Ventana", "Height", Me.Height)
            cfg.SetKeyValue("Ventana", "Width", Me.Width)
        End If
    End Sub

    Private Sub fColorear_Resize(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Resize
        If Me.WindowState = FormWindowState.Minimized AndAlso minimizarTray Then
            'ni.Visible = True
            Hide()
        Else
            If inicializando = False AndAlso Me.WindowState = FormWindowState.Normal Then
                cfg.SetKeyValue("Ventana", "Left", Me.Left)
                cfg.SetKeyValue("Ventana", "Top", Me.Top)
                cfg.SetKeyValue("Ventana", "Height", Me.Height)
                cfg.SetKeyValue("Ventana", "Width", Me.Width)
            End If
            'ni.Visible = False
        End If
    End Sub
    '
    Private Sub fColorear_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        ' centrar el formulario al ancho de la pantalla
        Me.Left = (Screen.PrimaryScreen.WorkingArea.Width - Me.Width) \ 2
        Me.rtEditor.AllowDrop = True
        '
        Dim sb As New System.Text.StringBuilder("�Guillermo 'guille' Som, 2006")
        If DateTime.Now.Year > 2006 Then
            sb.AppendFormat("-{0}", DateTime.Now.Year)
        End If
        sb.AppendFormat(" - {0} - v{1}", fvi.FileDescription, fvi.FileVersion)
        statusStrip1.Text = sb.ToString
        '
        Me.Text = String.Format("Utilidad para colorear el c�digo - gsColorearCodigo v{0}", fvi.FileVersion)
        Me.Tag = Me.Text
        '
        ' Asignar los colores que se pueden seleccionar             (27/Ago/06)
        cboColores.Items.AddRange(System.Enum.GetNames(kColorDeFondo.GetType))
        '
        asignarPalabrasClave()
        '
        '
        ' A�adir los valores predeterminados
        sincronizando = True
        '
        Me.txtColorKeywords.AutoCompleteCustomSource = txtColorTexto.AutoCompleteCustomSource
        Me.txtColorRem.AutoCompleteCustomSource = txtColorTexto.AutoCompleteCustomSource
        Me.txtColorXML.AutoCompleteCustomSource = txtColorTexto.AutoCompleteCustomSource
        '
        leerCfg()
        leerCfg_tpColores()
        '
        inicializar()
        '
        ' Tener en cuenta el �ltimo texto usado                     (31/Mar/07)
        Me.btnTextoNormal.Enabled = False
        If String.IsNullOrEmpty(My.Settings.ultimoTexto) = False Then
            textoSin = My.Settings.ultimoTexto
            ' Pero solo mostrarlo si se indica
            If Me.chkRecordarUltimoTexto.Checked Then
                If textoSin.TrimStart().StartsWith("{\rtf") Then
                    Me.rtEditor.Rtf = textoSin
                Else
                    Me.rtEditor.Text = textoSin
                End If
            Else
                Me.rtEditor.Text = ""
            End If
            Me.btnTextoNormal.Enabled = True
        Else
            Me.rtEditor.Text = ""
        End If
        '
        ' Seleccionar todo el texto                                 (17/Abr/07)
        Me.rtEditor.SelectAll()
        '
        sincronizando = False
        '
        'Me.btnTextoNormal.Enabled = False
    End Sub

    '
    '----------------------------------------------------------------------
    ' Para los colores y el tag <pre>                           (27/Nov/05)
    ' C�digo de la ficha de configuraci�n de gsHTMColorCode
    '----------------------------------------------------------------------
    '
    ' Seleccionar el color a usar                               (20/Oct/05)
    Private Sub seleccionarColor(ByVal txt As TextBox, ByVal lbl As Label, ByVal predet As String)
        ' Usar el color del texto de la caja
        ' se asignar� usando &H<color>
        ' Si da error, usar el color predeterminado
        Dim colDlg As New ColorDialog
        Try
            colDlg.Color = Color.FromArgb(CInt("&H" & txt.Text))
        Catch ex As Exception
            colDlg.Color = Color.FromArgb(CInt(predet))
        End Try
        If colDlg.ShowDialog = Windows.Forms.DialogResult.OK Then
            Dim s As String = colDlg.Color.ToArgb.ToString("x")
            If s.Length > 6 Then
                txt.Text = s.Substring(2)
            End If
            Try
                lbl.ForeColor = Color.FromArgb(CInt("&H" & txt.Text))
            Catch ex As Exception
                lbl.ForeColor = Color.FromArgb(CInt(txt.Text))
            End Try
        End If
    End Sub

    Private Sub cboPre_Click(ByVal sender As Object, ByVal e As System.EventArgs) _
                    Handles cboPre.Click, cboFuentes.Click, cboTamFuente.Click
        datosCambiados()
    End Sub

    Private Sub cboFuentes_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
                    Handles cboFuentes.TextChanged, cboTamFuente.TextChanged
        If inicializando Then Return

        datosCambiados()
        Try
            LabelColorTexto.Font = New Font(cboFuentes.Text, CSng(cboTamFuente.Text), FontStyle.Regular)
            Me.LabelColorKeywords.Font = New Font(cboFuentes.Text, CSng(cboTamFuente.Text), FontStyle.Regular)
            Me.LabelColorRem.Font = New Font(cboFuentes.Text, CSng(cboTamFuente.Text), FontStyle.Regular)
            Me.LabelColorXML.Font = New Font(cboFuentes.Text, CSng(cboTamFuente.Text), FontStyle.Regular)
            Me.LabelColorClases.Font = New Font(cboFuentes.Text, CSng(cboTamFuente.Text), FontStyle.Regular)
        Catch 'ex As Exception
        End Try
    End Sub


    Private Sub chkUsarStyle_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkUsarSpanStyle.CheckedChanged
        datosCambiados()
    End Sub

    Private Sub mnuFicSalir_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuFicSalir.Click, tsbSalir.Click
        Me.Close()
    End Sub

    Private Sub mnuFicAcerca_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tsbAcerca.Click, mnuFicAcerca.Click
        My.Forms.fAcercaDe.ShowDialog()
    End Sub

    Private Sub btnColorear_Click(
                    ByVal sender As Object, ByVal e As EventArgs) _
                    Handles tsbColorear.Click, tbSintaxColorearHTML.Click,
                            btnColorear.Click, mnuFicColorear.Click
        '
        ' Comprobar todo el texto para generar el HTML (tambi�n al pulsar F8)
        If rtEditor.TextLength = 0 Then Exit Sub
        '
        Me.statusInfo.Text = "Coloreando el c�digo..."
        Me.statusStrip1.Refresh()
        '
        ' El texto anterior
        ' Guardarlo como RTF o texto, seg�n proceda                 (08/Feb/07)
        If String.IsNullOrEmpty(rtEditor.Rtf) = False _
                AndAlso rtEditor.Rtf.TrimStart().StartsWith("{\rtf") Then
            textoSin = Me.rtEditor.Rtf
        Else
            textoSin = Me.rtEditor.Text
        End If
        'textoSin = Me.rtEditor.Text
        Me.btnTextoNormal.Enabled = True
        '
        If Me.chkIndentar.Checked Then
            Me.indentar = CInt(Me.txtIndentar.Value)
        Else
            Me.indentar = 0
        End If
        '
        Dim formatoColoreado As Colorear.FormatosColoreado
        If Me.tbSintaxColorearRTF.Checked Then
            formatoColoreado = Colorear.FormatosColoreado.RTF
        Else
            formatoColoreado = Colorear.FormatosColoreado.HTML
        End If
        ' No incluir los style
        Colorear.IncluirStyle = False
        '
        Dim s As String
        s = Colorear.ColorearCodigo(rtEditor.Text, lenguaje,
                        formatoColoreado,
                        Me.chkSyntaxMayusc.Checked,
                        Me.indentar,
                        Me.chkQuitarEspacios.Checked,
                        Colorear.ComprobacionesRem.Todos)
        If formatoColoreado = Colorear.FormatosColoreado.RTF Then
            Me.rtEditor.Text = s
            Me.statusInfo.Text = "Coloreado en formato RTF. Copia el c�digo y crea un fichero con la extensi�n .rtf"
        Else
            guardarTEMP(s)
            '' En Windows Vista, devuelve la barra del path          (23/Nov/06)
            'Dim ficTmp As String = System.IO.Path.GetTempPath() '& "\gsColorearCodigo.htm"
            'If ficTmp.EndsWith("\") Then
            '    ficTmp &= "gsColorearCodigo.htm"
            'Else
            '    ficTmp &= "\gsColorearCodigo.htm"
            'End If
            'Using sw As New System.IO.StreamWriter(ficTmp, False, System.Text.Encoding.UTF8)
            '    ' Aqu� si incluir el style
            '    sw.WriteLine("<style>pre{{font-family:{0}; font-size:{1}.0pt;}}</style>", _
            '                Me.cboFuentes.Text, Me.cboTamFuente.Text)
            '    sw.WriteLine(s)
            '    sw.Close()
            'End Using
            '' En C# y en algunos otros los espacios                 (20/Ago/06)
            '' los convierte en &nbsp; al pegar
            'Me.rtEditor.Text = s.Replace("&nbsp;", " ")
            'Me.WebBrowser1.Navigate(New Uri(ficTmp))
            ''
            'Me.statusInfo.Text = "C�digo coloreado. Pulsa en el Navegador para verlo o p�galo en una p�gina HTML."
            '' Seleccionar todo el c�digo despu�s de colorear        (17/Mar/06)
            'Me.rtEditor.SelectAll()
        End If
        Me.statusStrip1.Refresh()
    End Sub

    Private Sub guardarTEMP(ByVal s As String)
        ' En Windows Vista, devuelve la barra del path          (23/Nov/06)
        Dim ficTmp As String = System.IO.Path.GetTempPath() '& "\gsColorearCodigo.htm"
        If ficTmp.EndsWith("\") Then
            ficTmp &= "gsColorearCodigo.htm"
        Else
            ficTmp &= "\gsColorearCodigo.htm"
        End If
        Using sw As New System.IO.StreamWriter(ficTmp, False, System.Text.Encoding.UTF8)
            ' Aqu� si incluir el style
            sw.WriteLine("<style>pre{{font-family:{0}; font-size:{1}.0pt;}}</style>",
                        Me.cboFuentes.Text, Me.cboTamFuente.Text)
            sw.WriteLine(s)
            sw.Close()
        End Using
        ' En C# y en algunos otros los espacios                 (20/Ago/06)
        ' los convierte en &nbsp; al pegar
        Me.rtEditor.Text = s.Replace("&nbsp;", " ")
        Me.WebBrowser1.Navigate(New Uri(ficTmp))
        '
        Me.statusInfo.Text = "C�digo coloreado. Pulsa en el Navegador para verlo o p�galo en una p�gina HTML."
        ' Seleccionar todo el c�digo despu�s de colorear        (17/Mar/06)
        Me.rtEditor.SelectAll()
    End Sub

    Private Sub mnuEdi_Popup(ByVal sender As Object, ByVal e As System.EventArgs) Handles mnuEdi.DropDownOpening
        ' Este m�todo se llama tambi�n al cambiar de idioma y si cambia la selecci�n del texto
        ' Habilitar adecuadamente las opciones
        Dim bEsRTF As Boolean
        If String.IsNullOrEmpty(rtEditor.Rtf) = False _
        AndAlso rtEditor.Rtf.TrimStart().StartsWith("{\rtf") Then
            bEsRTF = True
        Else
            bEsRTF = False
        End If
        '
        mnuEdiDeshacer.Enabled = rtEditor.CanUndo
        mnuEdiCortar.Enabled = (rtEditor.SelectedText.Length > 0)
        mnuEdiCopiar.Enabled = mnuEdiCortar.Enabled
        '
        mnuEdiSeleccionarTodo.Enabled = (rtEditor.TextLength > 0)
        Me.mnuFicColorear.Enabled = Me.mnuEdiSeleccionarTodo.Enabled
        Me.mnuFicColorearDeRTF.Enabled = (Me.mnuEdiSeleccionarTodo.Enabled And bEsRTF)
        Me.tbSintaxColorearHTML.Enabled = Me.mnuEdiSeleccionarTodo.Enabled
        Me.tsbColorear.Enabled = Me.mnuEdiSeleccionarTodo.Enabled
        Me.btnColorear.Enabled = Me.mnuEdiSeleccionarTodo.Enabled
        Me.mnuFicNavegar.Enabled = Me.mnuEdiSeleccionarTodo.Enabled
        Me.tsbNavegar.Enabled = Me.mnuEdiSeleccionarTodo.Enabled
        '
        mnuEdiPegar.Enabled = False
        If Clipboard.GetDataObject.GetDataPresent(DataFormats.Text) Then
            mnuEdiPegar.Enabled = rtEditor.CanPaste(DataFormats.GetFormat(DataFormats.Text))
        ElseIf Clipboard.GetDataObject.GetDataPresent(DataFormats.StringFormat) Then
            ' StringFormat                                          (30/Oct/04)
            mnuEdiPegar.Enabled = rtEditor.CanPaste(DataFormats.GetFormat(DataFormats.StringFormat))
        ElseIf Clipboard.GetDataObject.GetDataPresent(DataFormats.Html) Then
            mnuEdiPegar.Enabled = rtEditor.CanPaste(DataFormats.GetFormat(DataFormats.Html))
        ElseIf Clipboard.GetDataObject.GetDataPresent(DataFormats.OemText) Then
            mnuEdiPegar.Enabled = rtEditor.CanPaste(DataFormats.GetFormat(DataFormats.OemText))
        ElseIf Clipboard.GetDataObject.GetDataPresent(DataFormats.UnicodeText) Then
            mnuEdiPegar.Enabled = rtEditor.CanPaste(DataFormats.GetFormat(DataFormats.UnicodeText))
        ElseIf Clipboard.GetDataObject.GetDataPresent(DataFormats.Rtf) Then
            mnuEdiPegar.Enabled = rtEditor.CanPaste(DataFormats.GetFormat(DataFormats.Rtf))
        End If
        '
        Me.tsbCortar.Enabled = rtEditor.SelectedText.Length > 0
        Me.tsbCopiar.Enabled = Me.tsbCortar.Enabled
        Me.tsbPegar.Enabled = mnuEdiPegar.Enabled
        '
        ' Actualizar tambi�n los del men� contextual
        If rtEditor.ContextMenuStrip IsNot Nothing Then
            For i As Integer = 0 To mnuEdi.DropDownItems.Count - 1
                rtEditor.ContextMenuStrip.Items(i).Enabled = mnuEdi.DropDownItems(i).Enabled
            Next
            ' Actualizar las opciones de Ver  y colorear el RTF     (08/Feb/07)
            rtEditor.ContextMenuStrip.Items("mnuColorearDeRTF").Enabled = (Me.mnuEdiSeleccionarTodo.Enabled And bEsRTF)
            rtEditor.ContextMenuStrip.Items("mnuVerRTF").Enabled = mnuEdiSeleccionarTodo.Enabled
        End If
        '
        Me.statusInfo.Text = Me.statusStrip1.Text
    End Sub
    Private Sub mnuEdi_Opening(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs)
        mnuEdi_Popup(Nothing, Nothing)
    End Sub

    Private Sub mnuEdiCopiar_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles mnuEdiCopiar.Click, tsbCopiar.Click
        ' Copiar el texto seleccionado en el portapapeles (en formato texto)
        Dim s As String = rtEditor.SelectedText
        Clipboard.SetDataObject(s, True)
    End Sub

    Private Sub mnuEdiCortar_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles mnuEdiCortar.Click, tsbCortar.Click
        rtEditor.Cut()
    End Sub

    Private Sub mnuEdiDeshacer_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles mnuEdiDeshacer.Click, tsbDeshacer.Click
        rtEditor.Undo()
    End Sub

    Private Sub mnuEdiPegar_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles mnuEdiPegar.Click, tsbPegar.Click
        ' Esto har� que el c�digo se vea como texto
        ' (manteniendo la indentaci�n, etc.)
        If Clipboard.ContainsText(TextDataFormat.Html) Then
            rtEditor.SelectedText = Clipboard.GetText() ' Clipboard.GetText(TextDataFormat.Html)
        ElseIf Clipboard.ContainsText(TextDataFormat.Rtf) Then
            rtEditor.SelectedRtf = Clipboard.GetText(TextDataFormat.Rtf)
        ElseIf Clipboard.ContainsText(TextDataFormat.UnicodeText) Then
            rtEditor.SelectedText = Clipboard.GetText(TextDataFormat.UnicodeText)
        ElseIf Clipboard.ContainsText() Then
            rtEditor.Paste()
        End If
    End Sub

    Private Sub mnuEdiSeleccionarTodo_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles mnuEdiSeleccionarTodo.Click
        rtEditor.SelectAll()
    End Sub

    Private Sub mnuSintax_Click(
                    ByVal sender As Object, ByVal e As EventArgs) _
                    Handles mnuSintax_CPP.Click, mnuSintax_VB6.Click,
                            mnuSintax_VB.Click, mnuSintax_SQL.Click,
                            mnuSintax_Pascal.Click, mnuSintax_Ninguno.Click,
                            mnuSintax_Java.Click, mnuSintax_IL.Click,
                            mnuSintax_FSharp.Click, mnuSintax_dotNet.Click,
                            mnuSintax_CS.Click, mnuSintax_XML.Click
        Static yaEstoy As Boolean
        If yaEstoy Then Exit Sub
        yaEstoy = True
        Dim mnu As ToolStripMenuItem
        For Each mnu In mnuSintax.DropDownItems
            mnu.Checked = False
        Next
        ' Hay dos opciones antes de la lista de lenguajes
        ' pero no importa recorrerlos todos, ya que es para quitar
        ' la selecci�n.
        For i As Integer = 0 To tsbSintax.DropDownItems.Count - 1
            mnu = TryCast(tsbSintax.DropDownItems(i), ToolStripMenuItem)
            If mnu IsNot Nothing Then
                mnu.Checked = False
            End If
        Next
        '
        mnu = DirectCast(sender, ToolStripMenuItem)
        Dim s As String = mnu.Name
        DirectCast(mnuSintax.DropDownItems(s), ToolStripMenuItem).Checked = True
        DirectCast(tsbSintax.DropDownItems(s), ToolStripMenuItem).Checked = True
        Dim le As Lenguajes = Lenguajes.Ninguno
        Select Case s
            Case "mnuSintax_CS"
                lenguaje = Lenguajes.CS
                le = lenguaje
            Case "mnuSintax_dotNet"
                lenguaje = Lenguajes.dotNet
                le = lenguaje
            Case "mnuSintax_VB"
                lenguaje = Lenguajes.VB
                le = lenguaje
            Case "mnuSintax_VB6"
                lenguaje = Lenguajes.VB6
                le = lenguaje
            Case "mnuSintax_Java"
                lenguaje = Lenguajes.Java
                le = lenguaje
            Case "mnuSintax_FSharp"
                lenguaje = Lenguajes.FSharp
                le = lenguaje
            Case "mnuSintax_SQL"
                lenguaje = Lenguajes.SQL
                le = lenguaje
            Case "mnuSintax_CPP"
                lenguaje = Lenguajes.CPP
                le = lenguaje
            Case "mnuSintax_Pascal"
                lenguaje = Lenguajes.Pascal
                le = lenguaje
            Case "mnuSintax_IL"
                lenguaje = Lenguajes.IL
                le = lenguaje
            Case "mnuSintax_XML"
                lenguaje = Lenguajes.XML
                le = lenguaje
        End Select
        ' Mostrar el lenguaje que se est� usando
        Me.statusSintax.Text = le.ToString()
        'If le.ToString().Length > 3 Then
        '    Me.statusSintax.Text = le.ToString().Substring(0, 3)
        'Else
        '    Me.statusSintax.Text = le.ToString()
        'End If
        '
        yaEstoy = False
    End Sub

    ''' <summary>
    ''' Carga las palabras clave en la colecci�n
    ''' </summary>
    ''' <remarks>
    ''' Si el fichero de palabras no existe,
    ''' se usar�n las palabras definidas en el programa,
    ''' que pueden ser gen�ricas (dotnet), de C# o de VB
    ''' </remarks>
    Private Sub asignarPalabrasClave()
        Colorear.AsignarPalabrasClave()
        '
        For Each de As System.Collections.Generic.KeyValuePair(Of Lenguajes, String) In PalabrasClave.Filenames
            If de.Value <> "" AndAlso de.Value <> Colorear.FicRecursos AndAlso System.IO.File.Exists(de.Value) Then
                Colorear.KeyWords.CargarPalabras(de.Key, de.Value)
            End If
        Next
        '
    End Sub

    Private Function clonarToolStripMenuItem(
            ByVal mnu As ToolStripMenuItem,
            ByVal eClick As EventHandler) As ToolStripMenuItem
        Return Me.clonarToolStripMenuItem(mnu, eClick, Nothing)
    End Function
    Private Function clonarToolStripMenuItem(
                    ByVal mnu As ToolStripMenuItem,
                    ByVal eClick As EventHandler,
                    ByVal eSelect As EventHandler) As ToolStripMenuItem
        Dim mnuC As New ToolStripMenuItem
        AddHandler mnuC.Click, eClick
        If eSelect IsNot Nothing Then
            AddHandler mnuC.DropDownOpening, eSelect
        End If
        'mnuC.Events.AddHandler(mnu.Events)
        mnuC.Checked = mnu.Checked
        mnuC.Enabled = mnu.Enabled
        mnuC.Font = mnu.Font
        mnuC.Image = mnu.Image
        mnuC.Name = mnu.Name
        mnuC.ShortcutKeys = mnu.ShortcutKeys
        mnuC.ShowShortcutKeys = mnu.ShowShortcutKeys
        mnuC.Tag = mnu.Tag
        mnuC.Text = mnu.Text
        mnuC.ToolTipText = mnu.ToolTipText

        Return mnuC
    End Function
    '
    '
    Private Sub StatusStrip1_MouseMove(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles statusStrip1.MouseMove
        statusInfo.Text = statusStrip1.Text
    End Sub

    Private Sub rtEditor_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles rtEditor.GotFocus
        ' Seleccionar todo al recibir el foco                       (17/Nov/06)
        rtEditor.SelectAll()
    End Sub

    Private Sub rtEditor_SelectionChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles rtEditor.SelectionChanged
        Me.mnuEdi_Popup(Nothing, Nothing)
    End Sub
    Private Sub rtEditor_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles rtEditor.TextChanged
        If inicializando Then Return

        'Me.mnuFicColorear.Enabled = (rtEditor.TextLength > 0)
        'Me.tbSintaxColorearHTML.Enabled = Me.mnuFicColorear.Enabled
        Me.mnuEdi_Popup(Nothing, Nothing)
    End Sub

    Private Sub btnTextoNormal_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnTextoNormal.Click
        ' Si es RTF pegarlo en el RTF                               (08/Feb/07)
        If textoSin.TrimStart().StartsWith("{\rtf") Then
            Me.rtEditor.Rtf = textoSin
        Else
            Me.rtEditor.Text = textoSin
        End If
        Me.btnTextoNormal.Enabled = False
    End Sub

    Private Sub chkIndentar_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkIndentar.CheckedChanged
        If inicializando Then Return

        If chkIndentar.Checked Then
            Me.chkQuitarEspacios.Checked = True
        End If
    End Sub

    Private Sub chkNotify_CheckedChanged(
                ByVal sender As Object, ByVal e As EventArgs) _
                Handles chkNotify.CheckedChanged, chkSyntaxMayusc.CheckedChanged,
                        chkRecordarUltimoTexto.CheckedChanged
        If inicializando Then Return

        datosCambiados()
    End Sub
    Private Sub cboColores_SelectedIndexChanged(
                    ByVal sender As Object, ByVal e As EventArgs) _
                    Handles cboColores.SelectedIndexChanged, cboColores.SelectedValueChanged
        If inicializando Then Return

        picColor.BackColor = Color.FromKnownColor(knownColorFromName(cboColores.Text))
        datosCambiados()
    End Sub

    Private Sub tbSintaxColorearRTF_Click(sender As Object,
                                          e As EventArgs) Handles tbSintaxColorearRTF.Click, mnuFicColorearEnRTF.Click

        ' En el men� ficheros he a�adido la opci�n de colorear RTF  (12/Sep/20)
        If sender Is tbSintaxColorearRTF Then
            Me.tbSintaxColorearRTF.Checked = Not tbSintaxColorearRTF.Checked
            Me.mnuFicColorearEnRTF.Checked = Me.tbSintaxColorearRTF.Checked
        Else
            Me.mnuFicColorearEnRTF.Checked = Not Me.mnuFicColorearEnRTF.Checked
            Me.tbSintaxColorearRTF.Checked = Me.mnuFicColorearEnRTF.Checked
        End If

        If Me.tbSintaxColorearRTF.Checked Then
            Me.btnColorear.Text = "Colorear en RTF"
        Else
            Me.btnColorear.Text = "Colorear en HTML"
        End If
        Me.tbSintaxColorearHTML.Text = Me.btnColorear.Text
        Me.mnuFicColorear.Text = Me.btnColorear.Text

    End Sub

    Private Sub mnuFicGuardar_Click(
                    ByVal sender As Object, ByVal e As EventArgs) _
                    Handles mnuFicGuardar.Click, tsbGuardar.Click
        ' Guardar como
        Dim saveFD As New SaveFileDialog
        Me.statusInfo.Text = "Guardar el texto en un fichero"
        With saveFD
            .Title = "Guardar el texto coloreado"
            .Filter = "HTML (*.htm; *.asp)|*.htm;*.asp|RTF (*.rtf)|*.rtf|Texto (*.txt)|*.txt|Todos los tipos (*.*)|*.*"
            If Me.tbSintaxColorearRTF.Checked Then
                .FilterIndex = 1
            Else
                .FilterIndex = 0
            End If
            If .ShowDialog = Windows.Forms.DialogResult.OK Then
                ' Si la extensi�n es .RTF                           (31/Mar/07)
                ' usar el Save del RichTextBox
                If System.IO.Path.GetExtension(.FileName).ToLower = ".rtf" Then
                    Try
                        Me.rtEditor.SaveFile(.FileName, RichTextBoxStreamType.RichText)
                    Catch ex As Exception
                        guardar(.FileName)
                    End Try
                Else
                    guardar(.FileName)
                End If
                'guardar(.FileName)
            End If
        End With
        Me.statusInfo.Text = Me.statusStrip1.Text
    End Sub

    Private Sub mnuFicAbrir_Click(
                    ByVal sender As Object, ByVal e As EventArgs) _
                    Handles mnuFicAbrir.Click, tsbAbrir.Click
        ' Abrir un fichero de c�digo
        ' Usar las extensiones definidas
        Dim openFD As New OpenFileDialog
        Me.statusInfo.Text = "Abrir un fichero de c�digo para colorearlo"
        ' Asignar las extensiones definidas
        Dim sb As New System.Text.StringBuilder("Fichero de c�digo|")
        For Each kv As KeyValuePair(Of Lenguajes, String) In PalabrasClave.Extensions
            sb.AppendFormat("{0};", kv.Value)
        Next
        sb.Remove(sb.Length - 1, 1)
        sb.Append("|Todos los ficheros (*.*)|*.*")
        With openFD
            .CheckFileExists = True
            .CheckPathExists = True
            .Multiselect = False
            .ShowReadOnly = True
            .Title = "Abrir fichero de c�digo"
            .Filter = sb.ToString
            If .ShowDialog = Windows.Forms.DialogResult.OK Then
                Me.abrir(.FileName)
            End If
        End With
        Me.statusInfo.Text = Me.statusStrip1.Text
    End Sub

    Private Sub mnuFicNavegar_Click(
                    ByVal sender As Object, ByVal e As EventArgs) _
                    Handles tsbNavegar.Click, mnuFicNavegar.Click
        'Dim ficTmp As String = System.IO.Path.GetTempPath() & "\gsColorearCodigo.htm"
        ' En Windows Vista, devuelve la barra del path
        ' Pero aqu� no lo hab�a cambiado                            (30/Ago/07)
        Dim ficTmp As String = System.IO.Path.GetTempPath()
        If ficTmp.EndsWith("\") Then
            ficTmp &= "gsColorearCodigo.htm"
        Else
            ficTmp &= "\gsColorearCodigo.htm"
        End If

        Using sw As New System.IO.StreamWriter(ficTmp, False, System.Text.Encoding.UTF8)
            ' Aqu� si incluir el style
            sw.WriteLine("<style>pre{{font-family:{0}; font-size:{1}.0pt;}}</style>",
                        Me.cboFuentes.Text, Me.cboTamFuente.Text)
            sw.WriteLine(Me.rtEditor.Text)
            sw.Close()
        End Using
        Me.WebBrowser1.Navigate(New Uri(ficTmp))
        Me.TabControl1.SelectedIndex = 1
        Me.statusInfo.Text = Me.statusStrip1.Text
    End Sub

    Private Sub mnuFic_DropDownOpening(ByVal sender As Object, ByVal e As System.EventArgs) Handles mnuFic.DropDownOpening
        Dim bEsRTF As Boolean
        If String.IsNullOrEmpty(rtEditor.Rtf) = False _
        AndAlso rtEditor.Rtf.TrimStart().StartsWith("{\rtf") Then
            bEsRTF = True
        Else
            bEsRTF = False
        End If
        '
        mnuEdiSeleccionarTodo.Enabled = (rtEditor.TextLength > 0)
        Me.mnuFicColorear.Enabled = Me.mnuEdiSeleccionarTodo.Enabled
        Me.mnuFicColorearDeRTF.Enabled = (Me.mnuEdiSeleccionarTodo.Enabled And bEsRTF)
    End Sub

    '
    ' Nuevas opciones a�adidas a la versi�n 1.0.8.0                 (10/Sep/20)
    '   cboPre_Validating
    '   mnuCboPreEliminar_Click
    '   btnColorTipos_Click
    '   chkUsarTemaOscuro_CheckedChanged
    '   txtColorClases_TextChanged
    '

    Private Sub cboPre_Validating(sender As Object,
                                  e As System.ComponentModel.CancelEventArgs) Handles cboPre.Validating
        If inicializando Then Return

        ' Comprobar si hay que a�adir el texto a la lista
        If inicializando Then Return
        If cboPre.Items.Contains(cboPre.Text) = False Then
            cboPre.Items.Add(cboPre.Text)
        End If
        If (chkUsarTemaOscuro.CheckState = CheckState.Unchecked AndAlso cboPre.Text <> Colorear.PreTagPre) OrElse
                (chkUsarTemaOscuro.CheckState = CheckState.Checked AndAlso cboPre.Text <> Colorear.PreTagOscuroPre) Then
            chkUsarTemaOscuro.CheckState = CheckState.Indeterminate
        End If
        'If cboPre.Text = gsColorearNET.Colorear.PreTagPre Then
        '    chkUsarTemaOscuro.CheckState = CheckState.Unchecked
        'ElseIf cboPre.Text = gsColorearNET.Colorear.PreTagoscuroPre Then
        '    chkUsarTemaOscuro.CheckState = CheckState.Checked
        'Else
        '    chkUsarTemaOscuro.CheckState = CheckState.Indeterminate
        'End If
        datosCambiados()
    End Sub

    Private Sub mnuCboPreEliminar_Click(sender As Object,
                                        e As EventArgs) Handles mnuCboPreEliminar.Click
        If inicializando Then Return
        Try
            Dim i = cboPre.SelectedIndex
            cboPre.Items.RemoveAt(i)
        Catch ex As Exception
        End Try
    End Sub

    Private Sub btnColor_Click(sender As Object,
                               e As EventArgs) Handles btnColorClases.Click, btnColorTexto.Click, btnColorRem.Click, btnColorXML.Click, btnColorKeywords.Click
        If sender Is btnColorTexto Then
            seleccionarColor(Me.txtColorTexto, Me.LabelColorTexto, Colorear.ColorTexto)
        ElseIf sender Is btnColorRem Then
            seleccionarColor(Me.txtColorRem, Me.LabelColorRem, Colorear.ColorComentarios)
        ElseIf sender Is btnColorXML Then
            seleccionarColor(Me.txtColorXML, Me.LabelColorXML, Colorear.ColorDocXML)
        ElseIf sender Is btnColorKeywords Then
            seleccionarColor(Me.txtColorKeywords, Me.LabelColorKeywords, Colorear.ColorInstrucciones)
        ElseIf sender Is btnColorClases Then
            seleccionarColor(Me.txtColorKeywords, Me.LabelColorKeywords, Colorear.ColorClases)
        End If

    End Sub

    Private Sub chkUsarTemaOscuro_CheckedChanged(sender As Object,
                                                 e As EventArgs) Handles chkUsarTemaOscuro.CheckedChanged
        If inicializando Then Return

        If chkUsarTemaOscuro.CheckState = CheckState.Checked Then
            ' Asignar los colores predeterminados del tema oscuro
            txtColorKeywords.Text = Colorear.ColorInstruccionesOscuroPre.Substring(2)
            txtColorRem.Text = Colorear.ColorComentariosOscuroPre.Substring(2)
            txtColorTexto.Text = Colorear.ColorTextoOscuroPre.Substring(2)
            txtColorXML.Text = Colorear.ColorDocXMLOscuroPre.Substring(2)
            txtColorClases.Text = Colorear.ColorClasesOscuroPre.Substring(2)
            cboPre.Text = Colorear.PreTagOscuroPre
        ElseIf chkUsarTemaOscuro.CheckState = CheckState.Unchecked Then
            ' Asignar los colores predeterminados del tema claro
            txtColorKeywords.Text = Colorear.ColorInstruccionesPre.Substring(2)
            txtColorRem.Text = Colorear.ColorComentariosPre.Substring(2)
            txtColorTexto.Text = Colorear.ColorTextoPre.Substring(2)
            txtColorXML.Text = Colorear.ColorDocXMLPre.Substring(2)
            txtColorClases.Text = Colorear.ColorClasesPre.Substring(2)
            cboPre.Text = Colorear.PreTagPre
        Else
            ' Asignar los colores que se est�n usando actualmente en la clase ???
            Colorear.FormatoColoreado = Colorear.FormatosColoreado.HTML
            txtColorKeywords.Text = Colorear.ColorInstrucciones
            txtColorRem.Text = Colorear.ColorComentarios
            txtColorTexto.Text = Colorear.ColorTexto
            txtColorXML.Text = Colorear.ColorDocXML
            txtColorClases.Text = Colorear.ColorClases
            cboPre.Text = Colorear.PreTag
        End If
    End Sub

    Private Sub txtColor_TextChanged(sender As Object,
                                     e As EventArgs) Handles txtColorClases.TextChanged, txtColorXML.TextChanged, txtColorTexto.TextChanged, txtColorRem.TextChanged, txtColorKeywords.TextChanged
        If inicializando Then Return

        Dim txt As TextBox = TryCast(sender, TextBox)
        Dim lbl As Label
        If txt Is txtColorClases Then
            lbl = LabelColorClases
        ElseIf txt Is txtColorKeywords Then
            lbl = LabelColorKeywords
        ElseIf txt Is txtColorRem Then
            lbl = LabelColorRem
        ElseIf txt Is txtColorTexto Then
            lbl = LabelColorTexto
        ElseIf txt Is txtColorXML Then
            lbl = LabelColorXML
        End If

        Try
            lbl.ForeColor = Color.FromArgb(CInt("&H" & txt.Text))
        Catch ex As Exception
            Try
                lbl.ForeColor = Color.FromArgb(CInt(txt.Text))
            Catch 'ex2 As Exception
            End Try
        End Try
        datosCambiados()

    End Sub
End Class
