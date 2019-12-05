<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class TopForm
    Inherits System.Windows.Forms.Form

    'フォームがコンポーネントの一覧をクリーンアップするために dispose をオーバーライドします。
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

    'Windows フォーム デザイナーで必要です。
    Private components As System.ComponentModel.IContainer

    'メモ: 以下のプロシージャは Windows フォーム デザイナーで必要です。
    'Windows フォーム デザイナーを使用して変更できます。  
    'コード エディターを使って変更しないでください。
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Me.btnCheckAll = New System.Windows.Forms.Button()
        Me.chkYmd = New System.Windows.Forms.CheckBox()
        Me.btnRun = New System.Windows.Forms.Button()
        Me.timeLabel = New System.Windows.Forms.Label()
        Me.endLabel = New System.Windows.Forms.Label()
        Me.dgvSave = New Save.ExDataGridView(Me.components)
        CType(Me.dgvSave, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'btnCheckAll
        '
        Me.btnCheckAll.Location = New System.Drawing.Point(101, 408)
        Me.btnCheckAll.Name = "btnCheckAll"
        Me.btnCheckAll.Size = New System.Drawing.Size(67, 23)
        Me.btnCheckAll.TabIndex = 1
        Me.btnCheckAll.Text = "Sel Set"
        Me.btnCheckAll.UseVisualStyleBackColor = True
        '
        'chkYmd
        '
        Me.chkYmd.AutoSize = True
        Me.chkYmd.Location = New System.Drawing.Point(182, 411)
        Me.chkYmd.Name = "chkYmd"
        Me.chkYmd.Size = New System.Drawing.Size(83, 16)
        Me.chkYmd.TabIndex = 2
        Me.chkYmd.Text = "YMD option"
        Me.chkYmd.UseVisualStyleBackColor = True
        '
        'btnRun
        '
        Me.btnRun.Location = New System.Drawing.Point(287, 403)
        Me.btnRun.Name = "btnRun"
        Me.btnRun.Size = New System.Drawing.Size(72, 33)
        Me.btnRun.TabIndex = 3
        Me.btnRun.Text = "Run"
        Me.btnRun.UseVisualStyleBackColor = True
        '
        'timeLabel
        '
        Me.timeLabel.AutoSize = True
        Me.timeLabel.Font = New System.Drawing.Font("MS UI Gothic", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.timeLabel.Location = New System.Drawing.Point(373, 411)
        Me.timeLabel.Name = "timeLabel"
        Me.timeLabel.Size = New System.Drawing.Size(0, 14)
        Me.timeLabel.TabIndex = 4
        '
        'endLabel
        '
        Me.endLabel.AutoSize = True
        Me.endLabel.ForeColor = System.Drawing.Color.Blue
        Me.endLabel.Location = New System.Drawing.Point(441, 413)
        Me.endLabel.Name = "endLabel"
        Me.endLabel.Size = New System.Drawing.Size(0, 12)
        Me.endLabel.TabIndex = 5
        '
        'dgvSave
        '
        Me.dgvSave.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvSave.Location = New System.Drawing.Point(9, 9)
        Me.dgvSave.Name = "dgvSave"
        Me.dgvSave.RowTemplate.Height = 21
        Me.dgvSave.Size = New System.Drawing.Size(607, 382)
        Me.dgvSave.TabIndex = 0
        '
        'TopForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(626, 449)
        Me.Controls.Add(Me.endLabel)
        Me.Controls.Add(Me.timeLabel)
        Me.Controls.Add(Me.btnRun)
        Me.Controls.Add(Me.chkYmd)
        Me.Controls.Add(Me.btnCheckAll)
        Me.Controls.Add(Me.dgvSave)
        Me.Name = "TopForm"
        Me.Text = "Save"
        CType(Me.dgvSave, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents dgvSave As Save.ExDataGridView
    Friend WithEvents btnCheckAll As System.Windows.Forms.Button
    Friend WithEvents chkYmd As System.Windows.Forms.CheckBox
    Friend WithEvents btnRun As System.Windows.Forms.Button
    Friend WithEvents timeLabel As System.Windows.Forms.Label
    Friend WithEvents endLabel As System.Windows.Forms.Label

End Class
