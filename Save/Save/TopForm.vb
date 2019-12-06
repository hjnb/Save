Imports System.Threading

Public Class TopForm
    '.iniファイルのパス
    Public iniFilePath As String = My.Application.Info.DirectoryPath & "\Save.ini"

    '画像
    Public imageFilePath As String = My.Application.Info.DirectoryPath & "\Save.ico"

    'チェックボックス列チェック制御用フラグ
    Private cellValueChangeFlg As Boolean = False

    'ini設定取得用
    Private saveDir As String
    Private saveMax As Integer
    Private saveItem As List(Of String)

    '経過時間表示用
    Private sw As New System.Diagnostics.Stopwatch()

    ''' <summary>
    ''' 行ヘッダーのカレントセルを表す三角マークを非表示に設定する為のクラス。
    ''' </summary>
    ''' <remarks></remarks>
    Public Class dgvRowHeaderCell

        'DataGridViewRowHeaderCell を継承
        Inherits DataGridViewRowHeaderCell

        'DataGridViewHeaderCell.Paint をオーバーライドして行ヘッダーを描画
        Protected Overrides Sub Paint(ByVal graphics As Graphics, ByVal clipBounds As Rectangle, _
           ByVal cellBounds As Rectangle, ByVal rowIndex As Integer, ByVal cellState As DataGridViewElementStates, _
           ByVal value As Object, ByVal formattedValue As Object, ByVal errorText As String, _
           ByVal cellStyle As DataGridViewCellStyle, ByVal advancedBorderStyle As DataGridViewAdvancedBorderStyle, _
           ByVal paintParts As DataGridViewPaintParts)
            '標準セルの描画からセル内容の背景だけ除いた物を描画(-5)
            MyBase.Paint(graphics, clipBounds, cellBounds, rowIndex, cellState, value, _
                     formattedValue, errorText, cellStyle, advancedBorderStyle, _
                     Not DataGridViewPaintParts.ContentBackground)
        End Sub

    End Class

    ''' <summary>
    ''' コンストラクタ
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()
        InitializeComponent()
        Me.StartPosition = FormStartPosition.CenterScreen
        Me.FormBorderStyle = FormBorderStyle.FixedSingle
        Me.MaximizeBox = False
    End Sub

    ''' <summary>
    ''' loadイベント
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub TopForm_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load
        'ファイル存在チェック
        If Not System.IO.File.Exists(iniFilePath) Then
            MsgBox("構成ファイルが存在しません。ファイルを配置して下さい。", MsgBoxStyle.Exclamation)
            Me.Close()
            Exit Sub
        End If
        If Not System.IO.File.Exists(imageFilePath) Then
            MsgBox("画像ファイルが存在しません。ファイルを配置して下さい。", MsgBoxStyle.Exclamation)
            Me.Close()
            Exit Sub
        End If

        topPicture.SizeMode = PictureBoxSizeMode.Zoom
        topPicture.ImageLocation = imageFilePath

        'iniファイル内容取得
        '保存先パス
        saveDir = Util.getIniString("System", "SaveDir", iniFilePath)
        If Not System.IO.Directory.Exists(saveDir) Then
            MsgBox("保存先のフォルダが存在しません。" & Environment.NewLine & "iniファイルのSaveDirに正しいパスを指定して下さい。", MsgBoxStyle.Exclamation)
            Me.Close()
            Exit Sub
        End If
        '件数
        Dim maxStr As String = Util.getIniString("System", "SaveMax", iniFilePath)
        If Not System.Text.RegularExpressions.Regex.IsMatch(maxStr, "^\d+$") Then
            MsgBox("iniファイルのSaveMaxに数値を設定して下さい。", MsgBoxStyle.Exclamation)
            Me.Close()
            Exit Sub
        Else
            saveMax = CInt(maxStr)
        End If
        '表示リスト
        saveItem = New List(Of String)
        For i As Integer = 1 To saveMax
            Dim path As String = Util.getIniString("System", "Save" & i, iniFilePath)
            saveItem.Add(path)
        Next

        'データグリッドビュー初期化
        initDgvSave()

        'データ表示
        displayDgvSave()

        'チェック全有
        btnCheckAll.PerformClick()
    End Sub

    ''' <summary>
    ''' データグリッドビュー初期化
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub initDgvSave()
        Util.EnableDoubleBuffering(dgvSave)

        With dgvSave
            .AllowUserToAddRows = False '行追加禁止
            .AllowUserToResizeColumns = False '列の幅をユーザーが変更できないようにする
            .AllowUserToResizeRows = False '行の高さをユーザーが変更できないようにする
            .AllowUserToDeleteRows = False '行削除禁止
            .BorderStyle = BorderStyle.FixedSingle
            .MultiSelect = False
            .SelectionMode = DataGridViewSelectionMode.CellSelect
            .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft
            .DefaultCellStyle.BackColor = Color.FromKnownColor(KnownColor.Control)
            .DefaultCellStyle.ForeColor = Color.Black
            .DefaultCellStyle.SelectionBackColor = Color.FromKnownColor(KnownColor.Control)
            .DefaultCellStyle.SelectionForeColor = Color.Black
            .RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing
            .ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing
            .ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
            .ColumnHeadersHeight = 20
            .RowHeadersWidth = 28
            .RowTemplate.Height = 18
            .RowTemplate.HeaderCell = New dgvRowHeaderCell() '行ヘッダの三角マークを非表示に
            .BackgroundColor = Color.FromKnownColor(KnownColor.Control)
            .ShowCellToolTips = True
            .EnableHeadersVisualStyles = False
            .Font = New Font("ＭＳ Ｐゴシック", 9)
            .ReadOnly = False
        End With

        '空行追加
        Dim dt As New DataTable()
        dt.Columns.Add("Start", GetType(String))
        dt.Columns.Add("Sel", GetType(Boolean))
        dt.Columns.Add("From", GetType(String))
        dt.Columns.Add("To", GetType(String))
        Dim saveMax As String = Util.getIniString("System", "SaveMax", iniFilePath)
        Dim maxRowCount As Integer = 0
        If System.Text.RegularExpressions.Regex.IsMatch(saveMax, "^\d+$") Then
            maxRowCount = CInt(saveMax)
        End If
        For i As Integer = 0 To maxRowCount - 1
            Dim row As DataRow = dt.NewRow()
            row("Start") = ""
            row("Sel") = False
            row("From") = ""
            row("To") = ""
            dt.Rows.Add(row)
        Next

        '表示
        dgvSave.DataSource = dt

        '幅設定等
        With dgvSave
            With .Columns("Start")
                .HeaderText = "Start"
                .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
                .DefaultCellStyle.ForeColor = Color.Blue
                .DefaultCellStyle.SelectionForeColor = Color.Blue
                .SortMode = DataGridViewColumnSortMode.NotSortable
                .ReadOnly = True
                .Width = 60
            End With
            With .Columns("Sel")
                .HeaderText = "Sel"
                .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
                .SortMode = DataGridViewColumnSortMode.NotSortable
                .Width = 25
            End With
            With .Columns("From")
                .HeaderText = "From"
                .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft
                .SortMode = DataGridViewColumnSortMode.NotSortable
                .ReadOnly = True
                .Width = 275
            End With
            With .Columns("To")
                .HeaderText = "To"
                .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft
                .SortMode = DataGridViewColumnSortMode.NotSortable
                .ReadOnly = True
                .Width = 200
            End With
        End With

        cellValueChangeFlg = True
    End Sub

    ''' <summary>
    ''' 内容クリア
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub clearDgv()
        For Each row As DataGridViewRow In dgvSave.Rows
            row.Cells("Start").Value = ""
            row.Cells("From").Value = ""
            row.Cells("To").Value = ""
        Next
    End Sub

    ''' <summary>
    ''' データ表示
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub displayDgvSave()
        '内容クリア
        clearDgv()

        'データ表示
        For i As Integer = 0 To saveMax - 1
            Dim path As String = saveItem(i)
            dgvSave("From", i).Value = path
            dgvSave("To", i).Value = saveDir & "\" & System.IO.Path.GetFileName(path)
        Next

    End Sub

    ''' <summary>
    ''' チェックボックス全有無
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub setCheckAll()
        Dim btnText As String = btnCheckAll.Text.Split(" ")(1)
        If btnText = "Clear" Then
            For Each row As DataGridViewRow In dgvSave.Rows
                row.Cells("Sel").Value = False
            Next
        Else
            For Each row As DataGridViewRow In dgvSave.Rows
                row.Cells("Sel").Value = True
            Next
        End If
    End Sub

    ''' <summary>
    ''' CellPaintingイベント
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub dgvSave_CellPainting(sender As Object, e As System.Windows.Forms.DataGridViewCellPaintingEventArgs) Handles dgvSave.CellPainting
        '行ヘッダーかどうか調べる
        If e.ColumnIndex < 0 AndAlso e.RowIndex >= 0 Then
            'セルを描画する
            e.Paint(e.ClipBounds, DataGridViewPaintParts.All)

            '行番号を描画する範囲を決定する
            'e.AdvancedBorderStyleやe.CellStyle.Paddingは無視しています
            Dim indexRect As Rectangle = e.CellBounds
            indexRect.Inflate(-2, -2)
            '行番号を描画する
            TextRenderer.DrawText(e.Graphics, _
                (e.RowIndex + 1).ToString(), _
                e.CellStyle.Font, _
                indexRect, _
                e.CellStyle.ForeColor, _
                TextFormatFlags.HorizontalCenter Or TextFormatFlags.VerticalCenter)
            '描画が完了したことを知らせる
            e.Handled = True
        End If
        '選択したセルに枠を付ける
        If e.ColumnIndex >= 0 AndAlso e.RowIndex >= 0 AndAlso (e.PaintParts And DataGridViewPaintParts.Background) = DataGridViewPaintParts.Background Then
            e.Graphics.FillRectangle(New SolidBrush(e.CellStyle.BackColor), e.CellBounds)

            If (e.PaintParts And DataGridViewPaintParts.SelectionBackground) = DataGridViewPaintParts.SelectionBackground AndAlso (e.State And DataGridViewElementStates.Selected) = DataGridViewElementStates.Selected Then
                e.Graphics.DrawRectangle(New Pen(Color.Black, 2I), e.CellBounds.X + 1I, e.CellBounds.Y + 1I, e.CellBounds.Width - 3I, e.CellBounds.Height - 3I)
            End If

            Dim pParts As DataGridViewPaintParts = e.PaintParts And Not DataGridViewPaintParts.Background
            e.Paint(e.ClipBounds, pParts)
            e.Handled = True
        End If
    End Sub

    Private Sub dgvSave_CellValueChanged(sender As Object, e As System.Windows.Forms.DataGridViewCellEventArgs) Handles dgvSave.CellValueChanged
        If cellValueChangeFlg AndAlso e.ColumnIndex = 1 Then
            Dim cellChecked As Boolean = dgvSave(e.ColumnIndex, e.RowIndex).Value
            If cellChecked Then
                dgvSave(e.ColumnIndex, e.RowIndex).Style.BackColor = Color.Red
                dgvSave(e.ColumnIndex, e.RowIndex).Style.SelectionBackColor = Color.Red
            Else
                dgvSave(e.ColumnIndex, e.RowIndex).Style.BackColor = Color.FromKnownColor(KnownColor.Control)
                dgvSave(e.ColumnIndex, e.RowIndex).Style.SelectionBackColor = Color.FromKnownColor(KnownColor.Control)
            End If
        End If
    End Sub

    Private Sub dgvSave_CurrentCellDirtyStateChanged(sender As Object, e As EventArgs) Handles dgvSave.CurrentCellDirtyStateChanged
        If dgvSave.CurrentCellAddress.X = 1 AndAlso dgvSave.IsCurrentCellDirty Then
            'コミットする
            dgvSave.CommitEdit(DataGridViewDataErrorContexts.Commit)
        End If
    End Sub

    ''' <summary>
    ''' SelClear or SelSetボタンクリックイベント
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub btnCheckAll_Click(sender As System.Object, e As System.EventArgs) Handles btnCheckAll.Click
        setCheckAll()
        If btnCheckAll.Text = "Sel Clear" Then
            btnCheckAll.Text = "Sel Set"
        Else
            btnCheckAll.Text = "Sel Clear"
        End If
    End Sub

    ''' <summary>
    ''' フォルダコピー
    ''' </summary>
    ''' <param name="sourceDirName">コピー対象のフォルダ</param>
    ''' <param name="destDirName">コピー先のフォルダ</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function copyDirectory(sourceDirName As String, destDirName As String) As Boolean
        Try
            'コピー先のディレクトリがないときは作る
            If Not System.IO.Directory.Exists(destDirName) Then
                System.IO.Directory.CreateDirectory(destDirName)
                '属性もコピー
                System.IO.File.SetAttributes(destDirName, System.IO.File.GetAttributes(sourceDirName))
            End If

            'コピー先のディレクトリ名の末尾に"\"をつける
            If destDirName.Chars((destDirName.Length - 1)) <> System.IO.Path.DirectorySeparatorChar Then
                destDirName = destDirName + System.IO.Path.DirectorySeparatorChar
            End If

            'コピー元のディレクトリにあるファイルをコピー
            Dim fs As String() = System.IO.Directory.GetFiles(sourceDirName)
            Dim f As String
            For Each f In fs
                System.IO.File.Copy(f, destDirName + System.IO.Path.GetFileName(f), True)
            Next

            'コピー元のディレクトリにあるディレクトリをコピー
            Dim dirs As String() = System.IO.Directory.GetDirectories(sourceDirName)
            Dim dir As String
            For Each dir In dirs
                copyDirectory(dir, destDirName + System.IO.Path.GetFileName(dir))
            Next

            Return True
        Catch ex As Exception
            '何かしらエラーの場合
            Return False
        End Try
    End Function

    ''' <summary>
    ''' ファイルコピー
    ''' </summary>
    ''' <param name="sourceFileName">コピー対象のファイル</param>
    ''' <param name="destDirName">コピー先のフォルダ</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function copyFile(sourceFileName As String, destDirName As String) As Boolean
        Try
            'コピー先のディレクトリ名の末尾に"\"をつける
            If destDirName.Chars((destDirName.Length - 1)) <> System.IO.Path.DirectorySeparatorChar Then
                destDirName = destDirName + System.IO.Path.DirectorySeparatorChar
            End If

            'コピー(YMDチェック有の場合は日付も加える)
            Dim ymd As String = If(chkYmd.Checked, Today.ToString("yyyyMMdd") & "_", "")
            System.IO.File.Copy(sourceFileName, destDirName & ymd & System.IO.Path.GetFileName(sourceFileName), True)

            Return True
        Catch ex As Exception
            '何かしらのエラーの場合
            Return False
        End Try
    End Function

    ''' <summary>
    ''' Runボタンクリックイベント
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub btnRun_Click(sender As System.Object, e As System.EventArgs) Handles btnRun.Click
        endLabel.Text = "処理中"
        endLabel.Refresh()

        dgvSave.FirstDisplayedScrollingRowIndex = 0

        Dim timerDelegate As TimerCallback = New TimerCallback(AddressOf worker)
        Dim timer As Timer = New Timer(timerDelegate, Nothing, 0, 1000)
        sw.Reset()
        sw.Start()
        timeLabel.Text = sw.Elapsed.Minutes.ToString("00") & ":" & sw.Elapsed.Seconds.ToString("00")

        'Start列の文字クリア
        For Each row As DataGridViewRow In dgvSave.Rows
            row.Cells("Start").Value = ""
        Next
        dgvSave.Refresh()

        'リスト上から順にコピー
        Dim ymd As String = If(chkYmd.Checked, Today.ToString("yyyyMMdd") & "_", "")
        For Each row As DataGridViewRow In dgvSave.Rows
            Dim isSaved As Boolean = False
            Dim copyTargetPath As String = Util.checkDBNullValue(row.Cells("From").Value)
            Dim checked As Boolean = row.Cells("Sel").Value
            If Not checked Then
                Continue For
            End If

            'コピー処理
            If System.IO.File.Exists(copyTargetPath) Then
                isSaved = copyFile(copyTargetPath, saveDir)
            ElseIf System.IO.Directory.Exists(copyTargetPath) Then
                isSaved = copyDirectory(copyTargetPath, saveDir & "\" & ymd & System.IO.Path.GetFileName(copyTargetPath))
            End If

            '結果表示
            If isSaved Then
                row.Cells("Start").Value = "Done"
            Else
                row.Cells("Start").Value = "Failed"
            End If
            dgvSave.FirstDisplayedScrollingRowIndex = row.Index
            dgvSave.Refresh()
        Next
        sw.Stop()
        timer.Dispose()

        endLabel.Text = "終了しました。"
    End Sub

    Delegate Sub DisplayTimeLabelDelegate()

    Private Sub displayTimeLabel()
        timeLabel.Text = sw.Elapsed.Minutes.ToString("00") & ":" & sw.Elapsed.Seconds.ToString("00")
        timeLabel.Refresh()
    End Sub

    Private Sub worker()
        Invoke(New DisplayTimeLabelDelegate(AddressOf displayTimeLabel))
    End Sub
End Class
