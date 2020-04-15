<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ufTerminalSelector
    Inherits System.Windows.Forms.Form

    'Форма переопределяет dispose для очистки списка компонентов.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Является обязательной для конструктора форм Windows Forms
    Private components As System.ComponentModel.IContainer

    'Примечание: следующая процедура является обязательной для конструктора форм Windows Forms
    'Для ее изменения используйте конструктор форм Windows Form.  
    'Не изменяйте ее в редакторе исходного кода.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.btnApply = New System.Windows.Forms.Button()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.cbxOrientation = New System.Windows.Forms.ComboBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.cbxTerminal = New System.Windows.Forms.ComboBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.cbxLocation = New System.Windows.Forms.ComboBox()
        Me.btnCancel = New System.Windows.Forms.Button()
        Me.cbxDuctSide = New System.Windows.Forms.ComboBox()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.SuspendLayout
        '
        'Label5
        '
        Me.Label5.AutoSize = true
        Me.Label5.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(204,Byte))
        Me.Label5.Location = New System.Drawing.Point(12, 9)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(351, 13)
        Me.Label5.TabIndex = 21
        Me.Label5.Text = "Укажите данные клеммника, который хотите отобразить"
        '
        'btnApply
        '
        Me.btnApply.Location = New System.Drawing.Point(170, 213)
        Me.btnApply.Name = "btnApply"
        Me.btnApply.Size = New System.Drawing.Size(90, 25)
        Me.btnApply.TabIndex = 20
        Me.btnApply.Text = "Apply"
        Me.btnApply.UseVisualStyleBackColor = true
        '
        'Label3
        '
        Me.Label3.AutoSize = true
        Me.Label3.Location = New System.Drawing.Point(12, 129)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(121, 13)
        Me.Label3.TabIndex = 17
        Me.Label3.Text = "Выберите ориентацию"
        '
        'cbxOrientation
        '
        Me.cbxOrientation.FormattingEnabled = true
        Me.cbxOrientation.Location = New System.Drawing.Point(186, 126)
        Me.cbxOrientation.Name = "cbxOrientation"
        Me.cbxOrientation.Size = New System.Drawing.Size(170, 21)
        Me.cbxOrientation.TabIndex = 16
        '
        'Label2
        '
        Me.Label2.AutoSize = true
        Me.Label2.Location = New System.Drawing.Point(12, 89)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(112, 13)
        Me.Label2.TabIndex = 15
        Me.Label2.Text = "Выберите клеммник"
        '
        'cbxTerminal
        '
        Me.cbxTerminal.FormattingEnabled = true
        Me.cbxTerminal.Location = New System.Drawing.Point(186, 86)
        Me.cbxTerminal.Name = "cbxTerminal"
        Me.cbxTerminal.Size = New System.Drawing.Size(170, 21)
        Me.cbxTerminal.TabIndex = 14
        '
        'Label1
        '
        Me.Label1.AutoSize = true
        Me.Label1.Location = New System.Drawing.Point(12, 49)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(152, 13)
        Me.Label1.TabIndex = 13
        Me.Label1.Text = "Выберите местонахождение"
        '
        'cbxLocation
        '
        Me.cbxLocation.FormattingEnabled = true
        Me.cbxLocation.Location = New System.Drawing.Point(186, 46)
        Me.cbxLocation.Name = "cbxLocation"
        Me.cbxLocation.Size = New System.Drawing.Size(170, 21)
        Me.cbxLocation.TabIndex = 12
        '
        'btnCancel
        '
        Me.btnCancel.Location = New System.Drawing.Point(266, 213)
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.Size = New System.Drawing.Size(90, 25)
        Me.btnCancel.TabIndex = 11
        Me.btnCancel.Text = "Cancel"
        Me.btnCancel.UseVisualStyleBackColor = true
        '
        'cbxDuctSide
        '
        Me.cbxDuctSide.FormattingEnabled = true
        Me.cbxDuctSide.Location = New System.Drawing.Point(186, 166)
        Me.cbxDuctSide.Name = "cbxDuctSide"
        Me.cbxDuctSide.Size = New System.Drawing.Size(170, 21)
        Me.cbxDuctSide.TabIndex = 18
        '
        'Label4
        '
        Me.Label4.AutoSize = true
        Me.Label4.Location = New System.Drawing.Point(12, 169)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(147, 13)
        Me.Label4.TabIndex = 19
        Me.Label4.Text = "Выберите сторону к коробу"
        '
        'ufTerminalSelector
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6!, 13!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(379, 253)
        Me.Controls.Add(Me.Label5)
        Me.Controls.Add(Me.btnApply)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.cbxDuctSide)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.cbxOrientation)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.cbxTerminal)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.cbxLocation)
        Me.Controls.Add(Me.btnCancel)
        Me.Name = "ufTerminalSelector"
        Me.Text = "Terminal Selector"
        Me.ResumeLayout(false)
        Me.PerformLayout

End Sub

    Friend WithEvents Label5 As Windows.Forms.Label
    Friend WithEvents btnApply As Windows.Forms.Button
    Friend WithEvents Label3 As Windows.Forms.Label
    Friend WithEvents cbxOrientation As Windows.Forms.ComboBox
    Friend WithEvents Label2 As Windows.Forms.Label
    Friend WithEvents cbxTerminal As Windows.Forms.ComboBox
    Friend WithEvents Label1 As Windows.Forms.Label
    Friend WithEvents cbxLocation As Windows.Forms.ComboBox
    Friend WithEvents btnCancel As Windows.Forms.Button
    Friend WithEvents cbxDuctSide As Windows.Forms.ComboBox
    Friend WithEvents Label4 As Windows.Forms.Label
End Class
