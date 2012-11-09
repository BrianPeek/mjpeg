<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Form1
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
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

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
Me.btnStart = New System.Windows.Forms.Button()
Me.image = New System.Windows.Forms.PictureBox()
CType(Me.image, System.ComponentModel.ISupportInitialize).BeginInit()
Me.SuspendLayout()
'
'btnStart
'
Me.btnStart.Location = New System.Drawing.Point(283, 488)
Me.btnStart.Name = "btnStart"
Me.btnStart.Size = New System.Drawing.Size(75, 23)
Me.btnStart.TabIndex = 0
Me.btnStart.Text = "&Start"
Me.btnStart.UseVisualStyleBackColor = True
'
'image
'
Me.image.Location = New System.Drawing.Point(0, 0)
Me.image.Name = "image"
Me.image.Size = New System.Drawing.Size(640, 480)
Me.image.TabIndex = 1
Me.image.TabStop = False
'
'Form1
'
Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
Me.BackColor = System.Drawing.SystemColors.Control
Me.ClientSize = New System.Drawing.Size(641, 515)
Me.Controls.Add(Me.image)
Me.Controls.Add(Me.btnStart)
Me.Name = "Form1"
Me.Text = "Form1"
CType(Me.image, System.ComponentModel.ISupportInitialize).EndInit()
Me.ResumeLayout(False)

End Sub
    Private WithEvents btnStart As System.Windows.Forms.Button
    Private WithEvents image As System.Windows.Forms.PictureBox

End Class
