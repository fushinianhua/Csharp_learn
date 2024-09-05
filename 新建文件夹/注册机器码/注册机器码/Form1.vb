Imports System.Globalization
Imports System.IO
Imports System.Security.Cryptography
Imports System.Text

Imports System.Text.RegularExpressions
Imports Microsoft.Win32

Public Class Form1
    Private MF As New DSAPI.DS密法
    Private ReadOnly Rstry As New RegistryHelper
    Private 次数 As Integer
    Private 激活状态 As Boolean

    Public Sub New()

        ' 此调用是设计器所必需的。
        InitializeComponent()
        是否存在注册表()
        ' 在 InitializeComponent() 调用之后添加任何初始化。

    End Sub

    Private Sub 是否存在注册表()
        Try
            Dim result As Boolean
            If Rstry.RegistryKeyExists("Software\MyApp") Then
                次数 = Rstry.ReadRegistryValue("Software\MyApp"， result)
                激活状态 = result
            Else
                WriteRegistryValue()

            End If
        Catch ex As Exception

        End Try

    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Try
            Dim str As String = 机器码text.Text.Trim
            If str = "" Then
                MessageBox.Show("机器码不可为空")
                Return
            ElseIf str.Length < 20 Then
                MessageBox.Show("机器码不符合要求")
                Return
            End If
            Dim result As String = gonggong.字符串加密(str, "abc78xyz")
            激活码text.Text = gonggong.ExtractAlphanumeric(result).ToUpper
            My.Settings.使用 = True
            My.Settings.Save()
            次数 -= 1
            If 次数 = 0 Then
                Rstry.RegistryKeyExists("")
            End If
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Panel1.Visible = False
        '  Panel2.Visible = False
        If My.Settings.使用 = True Then
            次数 = ReadRegistryValue()
            If 次数 = 0 Then
                My.Settings.使用 = False
                My.Settings.Save()
                MessageBox.Show("次数已经用完,请联系管理员", "提示", MessageBoxButtons.OK)
                Button1.Enabled = False
                Exit Sub
            End If
        Else
            Dim number As Integer = My.Settings.使用次数
            次数 = number
            WriteRegistryValue(number)
            My.Settings.使用次数 = 0
            My.Settings.Save()

        End If
        Label4.Text = 次数
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Try
            Dim str As String = TextBox2.Text
            Dim st As String = MF.解密(str, "abc78xyz")
            MessageBox.Show(st.Substring(Len(st) - 2, 2))
        Catch ex As Exception
            MessageBox.Show("Error: " & ex.Message)
        End Try

    End Sub

    Private Sub Form1_MouseClick(sender As Object, e As MouseEventArgs) Handles MyBase.MouseClick
        If e.Location.X > 65 And e.X < 550 And e.Y > 355 And e.Y < 480 Then
            If Environment.UserName = "辛鹏" Then
                Panel1_MouseDown(sender， e)
            End If
        End If

    End Sub

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        Dim str As String = ComboBox1.SelectedItem
        Dim s As String = Now.ToString("yyyyMMddHHmmss")
        Dim zifuchuang As String = MF.加密($"{s}{str}", "abc78xyz")
        Text_序列号.Text = zifuchuang
        Label6.Text = Len（zifuchuang）
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        My.Settings.使用 = True
        My.Settings.Save()
        次数 -= 1
        If 次数 = 0 Then
            WriteRegistryValue(次数, False)
        End If
        WriteRegistryValue(次数)
    End Sub

    Private Sub Panel1_MouseDown(sender As Object, e As MouseEventArgs) Handles Panel1.MouseDown
        If e.Button = MouseButtons.Left Then
            Panel1.Visible = True
        ElseIf e.Button = MouseButtons.Right Then
            Panel1.Visible = False
        End If
    End Sub

    Public Class gonggong

        Public Shared Function 字符串加密(字符串 As String, 密码 As String) As String
            Try
                Dim str As String = Ensure8Characters(密码) ' 确保密码长度为 8 字节

                Dim key() As Byte = Encoding.ASCII.GetBytes(str)
                Dim databyteArray() As Byte = Encoding.UTF8.GetBytes(字符串)
                Dim encrypt As String = ""
                Dim ms As New MemoryStream
                Dim cs As CryptoStream
                Dim des As New DESCryptoServiceProvider()
                des.Key = key
                des.IV = key ' 使用相同的 key 和 IV

                cs = New CryptoStream(ms, des.CreateEncryptor, CryptoStreamMode.Write)
                cs.Write(databyteArray, 0, databyteArray.Length)
                cs.FlushFinalBlock()

                encrypt = Convert.ToBase64String(ms.ToArray)

                Return encrypt
            Catch ex As Exception
                MessageBox.Show("Error encrypting: " & ex.Message)
                Return ""
            End Try
        End Function

        ' 加密字符串
        ' 加密密钥，长度为16字节（128位）
        Private Const EncryptionKey As String = "abcdefghijklmnop"

        Private Shared ReadOnly key As Byte() = Encoding.UTF8.GetBytes("mysecretkey12345".PadRight(16)) ' 16 bytes key for AES-128
        Private Shared ReadOnly iv As Byte() = Encoding.UTF8.GetBytes("mysecretiv67890".PadRight(16))   ' 16 bytes IV for AES-128

        ' 加密字符串
        Public Shared Function EncryptString(input As String) As String
            Using aesAlg As New AesManaged()
                aesAlg.Key = key
                aesAlg.IV = iv

                Dim encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV)

                Using msEncrypt As New MemoryStream()
                    Using csEncrypt As New CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write)
                        Using swEncrypt As New StreamWriter(csEncrypt)
                            swEncrypt.Write(input)
                        End Using
                    End Using
                    Return Convert.ToBase64String(msEncrypt.ToArray())
                End Using
            End Using
        End Function

        ' 解密字符串
        ' 解密字符串
        Public Shared Function DecryptString(cipherText As String) As String
            Dim cipherBytes = Convert.FromBase64String(cipherText)

            Using aesAlg As New AesManaged()
                aesAlg.Key = key
                aesAlg.IV = iv

                Dim decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV)

                Using msDecrypt As New MemoryStream(cipherBytes)
                    Using csDecrypt As New CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read)
                        Using srDecrypt As New StreamReader(csDecrypt)
                            Return srDecrypt.ReadToEnd()
                        End Using
                    End Using
                End Using
            End Using
        End Function

        Public Shared Function 字符串解密(加密后字符串 As String, 密码 As String) As String
            Try
                Dim str As String = Ensure8Characters(密码) ' 确保密码长度为 8 字节

                Dim key() As Byte = Encoding.ASCII.GetBytes(str)
                Dim encryptedData As Byte() = Convert.FromBase64String(加密后字符串)

                Dim ms As New MemoryStream
                Dim cs As CryptoStream
                Dim des As New DESCryptoServiceProvider()
                des.Key = key
                des.IV = key ' 使用相同的 key 和 IV

                cs = New CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write)
                cs.Write(encryptedData, 0, encryptedData.Length)
                cs.FlushFinalBlock()

                Dim decryptedData As Byte() = ms.ToArray()
                Dim decryptedString As String = Encoding.UTF8.GetString(decryptedData)

                Return decryptedString
            Catch ex As Exception
                MessageBox.Show("Error decrypting: " & ex.Message)
                Return ""
            End Try
        End Function

        Private Shared Function Ensure8Characters(input As String) As String
            If input.Length < 8 Then
                Return input.PadRight(8, "1"c)
            Else
                Return input.Substring(0, 8)
            End If
        End Function

        Public Shared Function ExtractAlphanumeric(ByVal input As String) As String
            ' 定义一个正则表达式模式，只匹配字母和数字
            Dim pattern As String = "[A-Za-z0-9]+"

            ' 使用正则表达式匹配字符串中的所有字母和数字
            Dim matches As MatchCollection = Regex.Matches(input, pattern)

            ' 将匹配结果组合成一个字符串
            Dim result As New StringBuilder()
            For Each match As Match In matches
                result.Append(match.Value)
            Next

            ' 返回组合后的字符串
            Return result.ToString
        End Function

    End Class

End Class