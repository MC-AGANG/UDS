Imports System.ComponentModel
Imports System.IO

Public Class Form1
    Dim flag As Boolean
    Dim n As Long
    Dim p1 As String, p2 As String
    Dim running As Boolean
    Dim INSERT As Boolean
    Dim writer As StreamWriter
    Dim adava As Boolean
    Dim dpath As Long
    Public Const WM_DEVICECHANGE = &H219
    Public Const DBT_DEVICEARRIVAL = &H8000
    Public Const DBT_CONFIGCHANGECANCELED = &H19
    Public Const DBT_CONFIGCHANGED = &H18
    Public Const DBT_CUSTOMEVENT = &H8006
    Public Const DBT_DEVICEQUERYREMOVE = &H8001
    Public Const DBT_DEVICEQUERYREMOVEFAILED = &H8002
    Public Const DBT_DEVICEREMOVECOMPLETE = &H8004
    Public Const DBT_DEVICEREMOVEPENDING = &H8003
    Public Const DBT_DEVICETYPESPECIFIC = &H8005
    Public Const DBT_DEVNODES_CHANGED = &H7
    Public Const DBT_QUERYCHANGECONFIG = &H17
    Public Const DBT_USERDEFINED = &HFFFF
    Dim xc1 As System.Threading.Thread

    Protected Overrides Sub WndProc(ByRef m As System.Windows.Forms.Message)
        If m.Msg = WM_DEVICECHANGE Then
            Select Case m.WParam
                Case WM_DEVICECHANGE
                Case DBT_DEVICEARRIVAL 'U盘插入
                    dpath = dpath + 1
                    p2 = TextBox1.Text
                    INSERT = True
                    Dim s() As DriveInfo = DriveInfo.GetDrives
                    For Each drive As DriveInfo In s
                        p1 = drive.Name.ToString
                        If drive.DriveType = DriveType.Removable Then
                            flag = False
                            n = 0
                            ListBox1.Items.Add(TimeString + "  U盘已插入，盘符为：" + drive.Name.ToString)
                            If running And INSERT Then
                                xc1 = New System.Threading.Thread(AddressOf Xcsub)
                                xc1.Start()
                                ListBox1.Items.Add(TimeString + "  开始复制")
                            End If
                        End If
                    Next
                Case DBT_CONFIGCHANGECANCELED
                Case DBT_CONFIGCHANGED
                Case DBT_CUSTOMEVENT
                Case DBT_DEVICEQUERYREMOVE
                Case DBT_DEVICEQUERYREMOVEFAILED
                Case DBT_DEVICEREMOVECOMPLETE 'U盘卸载 
                    INSERT = False
                    ListBox1.Items.Add(TimeString + "  U盘已拔出")
                Case DBT_DEVICEREMOVEPENDING
                Case DBT_DEVICETYPESPECIFIC
                Case DBT_DEVNODES_CHANGED
                Case DBT_QUERYCHANGECONFIG
                Case DBT_USERDEFINED
            End Select
        End If

        MyBase.WndProc(m)
    End Sub
    Private Function Xcsub() As Boolean
        Xcsub = False
        Try
            If CheckBox2.Checked = False Then
                CopyDerictory(New DirectoryInfo(p1), New DirectoryInfo(p2))
            Else
                CopyDerictory(New DirectoryInfo(p1), New DirectoryInfo(p2 + CStr(dpath) + "\"))
            End If
            flag = True
        Catch
        End Try
    End Function

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        ListBox1.Items.Clear()
        ListBox1.Items.Add(TimeString + "  程序已打开")
        adava = True
        dpath = 0
        Dim s(10) As String
        Dim i As Integer
        Dim reader As TextReader
        If My.Computer.FileSystem.FileExists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\AGANG\UDS\settings.vbson") Then
            reader = File.OpenText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\AGANG\UDS\settings.vbson")
            For i = 1 To 3
                s(i) = reader.ReadLine
            Next
            TextBox1.Text = Mid(s(1), 9, Len(s(1)) - 9)
            If Mid(s(2), 13, 4) = "True" Then
                CheckBox1.Checked = True
            ElseIf Mid(s(2), 13, 5) = "False" Then
                CheckBox1.Checked = False
            End If
            If Mid(s(3), 11, 4) = "True" Then
                CheckBox2.Checked = True
            ElseIf Mid(s(3), 11, 5) = "False" Then
                CheckBox2.Checked = False
            End If
            ListBox1.Items.Add(TimeString + "  成功读取上一次设置")
            reader.Close()
        Else
            Try
                Savestate()
                ListBox1.Items.Add(TimeString + "  无记忆文件，已成功创建")
            Catch
                ListBox1.Items.Add(TimeString + "  无法创建记忆文件，可能未获取%AppData%权限")
                adava = False
            End Try
        End If
        running = False
        INSERT = False
        NotifyIcon1.Icon = Me.Icon
    End Sub
    Public Sub CopyDerictory(ByVal DirectorySrc As DirectoryInfo, ByVal DirectoryDes As DirectoryInfo)
        Dim strDirectoryDesPath As String = DirectoryDes.FullName
        If Not Directory.Exists(strDirectoryDesPath) Then
            Directory.CreateDirectory(strDirectoryDesPath)
        End If
        Dim f, fs() As FileInfo
        fs = DirectorySrc.GetFiles()
        For Each f In fs
            If Not My.Computer.FileSystem.FileExists(strDirectoryDesPath & "" & f.Name) Or CheckBox1.Checked Then
                File.Copy(f.FullName, strDirectoryDesPath & "" & f.Name, True)
            End If
            n = n + 1
        Next
        Dim DirSrc, Dirs() As DirectoryInfo
        Dirs = DirectorySrc.GetDirectories()
        For Each DirSrc In Dirs
            Dim DirDes As New DirectoryInfo(strDirectoryDesPath + DirSrc.ToString + "\")
            CopyDerictory(DirSrc, DirDes)
        Next
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        FolderBrowserDialog1.SelectedPath = Mid(TextBox1.Text, 1, Len(TextBox1.Text) - 1)
        Dim se As DialogResult
        se = Me.FolderBrowserDialog1.ShowDialog
        If se = DialogResult.OK Then
            TextBox1.Text = FolderBrowserDialog1.SelectedPath.ToString + "\"
            ListBox1.Items.Add(TimeString + "  选择储存路径为" + FolderBrowserDialog1.SelectedPath)
        End If
    End Sub



    Private Sub NotifyIcon1_Click(sender As Object, e As EventArgs) Handles NotifyIcon1.Click
        Me.Visible = True
        NotifyIcon1.Visible = False
        ListBox1.Items.Add(TimeString + "  显示窗口")
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        ListBox1.Items.Add(TimeString + "  关闭程序")
        Try
            xc1.Abort()
        Catch
        End Try
        Me.Close()
    End Sub

    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
        ListBox1.Items.Clear()
    End Sub

    Private Sub Button6_Click(sender As Object, e As EventArgs) Handles Button6.Click
        If adava Then
            Savestate()
            ListBox1.Items.Add(TimeString + "  成功保存当前状态")
        End If
        Try
            If Not Directory.Exists(Mid(TextBox1.Text, 1, Len(TextBox1.Text) - 1)) Then
                Directory.CreateDirectory(Mid(TextBox1.Text, 1, Len(TextBox1.Text) - 1))
            End If
            Directory.CreateDirectory(TextBox1.Text + "agangtemp")
            running = True
            Button6.Enabled = False
            Button7.Enabled = True
            ListBox1.Items.Add(TimeString + "  程序启动")
        Catch
            MsgBox("选定的文件储存路径无法使用！")
        Finally
        End Try
        Try
            Directory.Delete(TextBox1.Text + "agangtemp")
        Catch
        End Try
    End Sub

    Private Sub Button7_Click(sender As Object, e As EventArgs) Handles Button7.Click
        running = False
        Button6.Enabled = True
        Button7.Enabled = False
        ListBox1.Items.Add(TimeString + "  程序停止")
        Try
            xc1.Abort()
        Catch
        End Try
    End Sub

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        MsgBox("作者:AGANG  版本：3.0")
        MsgBox("小心使用，切莫张扬")
    End Sub

    Private Sub Form1_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        Try
            xc1.Abort()
        Catch
        End Try
    End Sub

    Private Sub CheckBox1_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox1.CheckedChanged
        If CheckBox1.Checked Then
            CheckBox2.Checked = False
            CheckBox2.Enabled = False
        Else
            CheckBox2.Enabled = True
        End If
    End Sub

    Private Function Savestate() As Boolean
        If Not My.Computer.FileSystem.FileExists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\AGANG\UDS\settings.vbson") Then
            Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\AGANG\UDS")
            File.WriteAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\AGANG\UDS\settings.vbson", " ")
        End If
        Dim bol As String
        Savestate = False
        writer = New StreamWriter(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\AGANG\UDS\settings.vbson")
        writer.WriteLine("path = """ + TextBox1.Text + """")
        writer.Flush()
        writer.Close()
        writer = Nothing
        If CheckBox1.Checked Then
            bol = "True"
        Else
            bol = "False"
        End If
        writer = New StreamWriter(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\AGANG\UDS\settings.vbson", True, System.Text.Encoding.Default)
        writer.WriteLine("overwrite = " + bol)
        writer.Flush()
        writer.Close()
        writer = Nothing
        If CheckBox2.Checked Then
            bol = "True"
        Else
            bol = "False"
        End If
        writer = New StreamWriter(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\AGANG\UDS\settings.vbson", True, System.Text.Encoding.Default)
        writer.WriteLine("divided = " + bol)
        writer.Flush()
        writer.Close()
        writer = Nothing
    End Function

    Private Sub 彻底隐藏_Click(sender As Object, e As EventArgs) Handles 彻底隐藏.Click
        Me.Visible = False
        MsgBox("若要退出程序，请前往任务管理器")
    End Sub

    Private Sub Button2_MouseClick(sender As Object, e As MouseEventArgs) Handles Button2.MouseClick

        NotifyIcon1.Visible = True
        ListBox1.Items.Add(TimeString + "  隐藏窗口")
    End Sub

End Class
