<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class fAcercaDe
    Inherits System.Windows.Forms.Form

    'Form reemplaza a Dispose para limpiar la lista de componentes.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing Then
            If Not (components Is Nothing) Then
                components.Dispose()
            End If
        End If
        MyBase.Dispose(disposing)
    End Sub

    'Requerido por el Diseñador de Windows Forms
    Private components As System.ComponentModel.IContainer

    'NOTA: el Diseñador de Windows Forms necesita el siguiente procedimiento
    'Se puede modificar usando el Diseñador de Windows Forms.  
    'No lo modifique con el editor de código.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(fAcercaDe))
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
        Me.labelWeb = New System.Windows.Forms.Label()
        Me.labelDescripcion = New System.Windows.Forms.Label()
        Me.labelAutor = New System.Windows.Forms.Label()
        Me.labelTitulo = New System.Windows.Forms.Label()
        Me.btnAceptar = New System.Windows.Forms.Button()
        Me.linkBug = New System.Windows.Forms.LinkLabel()
        Me.linkPrograma = New System.Windows.Forms.LinkLabel()
        Me.ToolTip1 = New System.Windows.Forms.ToolTip(Me.components)
        Me.linkURL = New System.Windows.Forms.LinkLabel()
        Me.timerWeb = New System.Windows.Forms.Timer(Me.components)
        Me.TableLayoutPanel1.SuspendLayout()
        Me.SuspendLayout()
        '
        'TableLayoutPanel1
        '
        Me.TableLayoutPanel1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.TableLayoutPanel1.BackColor = System.Drawing.Color.Transparent
        Me.TableLayoutPanel1.ColumnCount = 2
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 285.0!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 84.0!))
        Me.TableLayoutPanel1.Controls.Add(Me.labelWeb, 0, 3)
        Me.TableLayoutPanel1.Controls.Add(Me.labelDescripcion, 0, 2)
        Me.TableLayoutPanel1.Controls.Add(Me.labelAutor, 0, 5)
        Me.TableLayoutPanel1.Controls.Add(Me.labelTitulo, 0, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.btnAceptar, 1, 8)
        Me.TableLayoutPanel1.Controls.Add(Me.linkBug, 0, 7)
        Me.TableLayoutPanel1.Controls.Add(Me.linkPrograma, 0, 8)
        Me.TableLayoutPanel1.Location = New System.Drawing.Point(231, 8)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        Me.TableLayoutPanel1.RowCount = 9
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 63.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 4.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 158.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 48.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 4.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 27.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 4.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 118.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
        Me.TableLayoutPanel1.Size = New System.Drawing.Size(369, 364)
        Me.TableLayoutPanel1.TabIndex = 0
        '
        'labelWeb
        '
        Me.TableLayoutPanel1.SetColumnSpan(Me.labelWeb, 2)
        Me.labelWeb.Dock = System.Windows.Forms.DockStyle.Fill
        Me.labelWeb.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.labelWeb.ForeColor = System.Drawing.Color.White
        Me.labelWeb.Location = New System.Drawing.Point(3, 225)
        Me.labelWeb.Name = "labelWeb"
        Me.labelWeb.Size = New System.Drawing.Size(363, 48)
        Me.labelWeb.TabIndex = 2
        Me.labelWeb.Text = "Info actualización" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Segunda línea" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Tercera"
        '
        'labelDescripcion
        '
        Me.TableLayoutPanel1.SetColumnSpan(Me.labelDescripcion, 2)
        Me.labelDescripcion.Dock = System.Windows.Forms.DockStyle.Fill
        Me.labelDescripcion.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.labelDescripcion.ForeColor = System.Drawing.Color.White
        Me.labelDescripcion.Location = New System.Drawing.Point(3, 67)
        Me.labelDescripcion.Name = "labelDescripcion"
        Me.labelDescripcion.Padding = New System.Windows.Forms.Padding(4, 4, 8, 4)
        Me.labelDescripcion.Size = New System.Drawing.Size(363, 158)
        Me.labelDescripcion.TabIndex = 1
        Me.labelDescripcion.Text = "gsColorearCodigo v1.0.0.0"
        '
        'labelAutor
        '
        Me.TableLayoutPanel1.SetColumnSpan(Me.labelAutor, 2)
        Me.labelAutor.Dock = System.Windows.Forms.DockStyle.Fill
        Me.labelAutor.Font = New System.Drawing.Font("Tahoma", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.labelAutor.ForeColor = System.Drawing.Color.White
        Me.labelAutor.Location = New System.Drawing.Point(3, 277)
        Me.labelAutor.Name = "labelAutor"
        Me.labelAutor.Size = New System.Drawing.Size(363, 27)
        Me.labelAutor.TabIndex = 3
        Me.labelAutor.Text = "©Guillermo 'guille' Som, 2005-2007"
        Me.labelAutor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'labelTitulo
        '
        Me.TableLayoutPanel1.SetColumnSpan(Me.labelTitulo, 2)
        Me.labelTitulo.Dock = System.Windows.Forms.DockStyle.Fill
        Me.labelTitulo.Font = New System.Drawing.Font("Tahoma", 27.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.labelTitulo.ForeColor = System.Drawing.Color.White
        Me.labelTitulo.Location = New System.Drawing.Point(3, 0)
        Me.labelTitulo.Name = "labelTitulo"
        Me.labelTitulo.Size = New System.Drawing.Size(363, 63)
        Me.labelTitulo.TabIndex = 0
        Me.labelTitulo.Text = "gsColorearCodigo"
        '
        'btnAceptar
        '
        Me.btnAceptar.BackColor = System.Drawing.Color.DarkBlue
        Me.btnAceptar.FlatAppearance.BorderColor = System.Drawing.Color.White
        Me.btnAceptar.FlatAppearance.MouseDownBackColor = System.Drawing.Color.RoyalBlue
        Me.btnAceptar.FlatAppearance.MouseOverBackColor = System.Drawing.Color.SlateBlue
        Me.btnAceptar.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnAceptar.ForeColor = System.Drawing.Color.White
        Me.btnAceptar.Location = New System.Drawing.Point(288, 337)
        Me.btnAceptar.Name = "btnAceptar"
        Me.btnAceptar.Size = New System.Drawing.Size(75, 23)
        Me.btnAceptar.TabIndex = 5
        Me.btnAceptar.Text = "Aceptar"
        Me.btnAceptar.UseVisualStyleBackColor = False
        '
        'linkBug
        '
        Me.linkBug.ActiveLinkColor = System.Drawing.Color.White
        Me.TableLayoutPanel1.SetColumnSpan(Me.linkBug, 2)
        Me.linkBug.Dock = System.Windows.Forms.DockStyle.Fill
        Me.linkBug.Font = New System.Drawing.Font("Tahoma", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.linkBug.LinkColor = System.Drawing.Color.White
        Me.linkBug.Location = New System.Drawing.Point(3, 308)
        Me.linkBug.Name = "linkBug"
        Me.linkBug.Size = New System.Drawing.Size(363, 26)
        Me.linkBug.TabIndex = 4
        Me.linkBug.TabStop = True
        Me.linkBug.Text = "Reportar un bug o una mejora"
        Me.ToolTip1.SetToolTip(Me.linkBug, " Pulsa en este link para reportar un bug o alguna mejora ")
        Me.linkBug.VisitedLinkColor = System.Drawing.Color.White
        '
        'linkPrograma
        '
        Me.linkPrograma.ActiveLinkColor = System.Drawing.Color.White
        Me.linkPrograma.AutoSize = True
        Me.linkPrograma.Font = New System.Drawing.Font("Tahoma", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.linkPrograma.LinkColor = System.Drawing.Color.White
        Me.linkPrograma.Location = New System.Drawing.Point(3, 334)
        Me.linkPrograma.Name = "linkPrograma"
        Me.linkPrograma.Size = New System.Drawing.Size(179, 16)
        Me.linkPrograma.TabIndex = 6
        Me.linkPrograma.TabStop = True
        Me.linkPrograma.Text = "Ir a la página de la utilidad"
        Me.ToolTip1.SetToolTip(Me.linkPrograma, " Ir a http://www.elguille.info/NET/vs2005/utilidades/gsColorearCodigo.htm ")
        Me.linkPrograma.VisitedLinkColor = System.Drawing.Color.White
        '
        'linkURL
        '
        Me.linkURL.ActiveLinkColor = System.Drawing.Color.White
        Me.linkURL.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.linkURL.BackColor = System.Drawing.Color.Transparent
        Me.linkURL.DisabledLinkColor = System.Drawing.Color.White
        Me.linkURL.Font = New System.Drawing.Font("Tahoma", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.linkURL.LinkColor = System.Drawing.Color.White
        Me.linkURL.Location = New System.Drawing.Point(8, 345)
        Me.linkURL.Name = "linkURL"
        Me.linkURL.Padding = New System.Windows.Forms.Padding(4, 0, 0, 0)
        Me.linkURL.Size = New System.Drawing.Size(217, 23)
        Me.linkURL.TabIndex = 1
        Me.linkURL.TabStop = True
        Me.linkURL.Text = "http://www.elguille.info/"
        Me.ToolTip1.SetToolTip(Me.linkURL, " Ir a mi sitio de Internet ")
        Me.linkURL.VisitedLinkColor = System.Drawing.Color.White
        '
        'timerWeb
        '
        '
        'fAcercaDe
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.SystemColors.Control
        Me.BackgroundImage = CType(resources.GetObject("$this.BackgroundImage"), System.Drawing.Image)
        Me.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
        Me.ClientSize = New System.Drawing.Size(608, 380)
        Me.Controls.Add(Me.linkURL)
        Me.Controls.Add(Me.TableLayoutPanel1)
        Me.ForeColor = System.Drawing.SystemColors.WindowText
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "fAcercaDe"
        Me.Opacity = 0.95R
        Me.Padding = New System.Windows.Forms.Padding(8)
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Acerca de..."
        Me.TableLayoutPanel1.ResumeLayout(False)
        Me.TableLayoutPanel1.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents TableLayoutPanel1 As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents btnAceptar As System.Windows.Forms.Button
    Friend WithEvents labelTitulo As System.Windows.Forms.Label
    Friend WithEvents labelAutor As System.Windows.Forms.Label
    Private WithEvents labelDescripcion As System.Windows.Forms.Label
    Friend WithEvents ToolTip1 As System.Windows.Forms.ToolTip
    Friend WithEvents linkBug As System.Windows.Forms.LinkLabel
    Friend WithEvents linkURL As System.Windows.Forms.LinkLabel
    Private WithEvents timerWeb As System.Windows.Forms.Timer
    Private WithEvents labelWeb As System.Windows.Forms.Label
    Friend WithEvents linkPrograma As System.Windows.Forms.LinkLabel

End Class
