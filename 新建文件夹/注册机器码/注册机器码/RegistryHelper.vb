Imports System.Globalization
Imports Microsoft.Win32

Public Class RegistryHelper

    ''' <summary>
    ''' 判断注册表键是否存在
    ''' </summary>
    ''' <param name="subKeyPath">注册表键的路径</param>
    ''' <returns></returns>
    Public Function RegistryKeyExists(subKeyPath As String) As Boolean
        Try
            Using key As RegistryKey = Registry.CurrentUser.OpenSubKey(subKeyPath, False)
                Return key IsNot Nothing
            End Using
        Catch ex As Exception
            Return False
        End Try
    End Function

    Public Sub WriteRegistryValue(int As Integer) '写入注册表
        Try
            ' 创建或打开注册表键
            Using key As RegistryKey = Registry.CurrentUser.CreateSubKey("Software\MyApp")
                If key IsNot Nothing Then
                    ' 写入值
                    key.SetValue("UsageCount", int, RegistryValueKind.DWord)
                    'MessageBox.Show("使用次数已成功写入注册表。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Else
                    ' MessageBox.Show("未能打开或创建注册表路径：Software\MyApp", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End If

            End Using
        Catch ex As Exception
            'MessageBox.Show($"写入注册表错误: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Public Function ReadRegistryValue(str As String， ByRef state As Boolean) As Integer '读取注册表
        Try
            Dim result As Integer = 0
            Dim sta As Boolean = False
            ' 打开注册表键
            Using key As RegistryKey = Registry.CurrentUser.OpenSubKey(str, False)
                If key IsNot Nothing Then
                    ' 读取指定名称的值
                    Dim value As Object = key.GetValue("UsageCount")
                    result = Convert.ToInt32(value)

                    If value IsNot Nothing AndAlso Integer.TryParse(value.ToString(), Nothing) Then
                        Dim usageCount As Integer = Convert.ToInt32(value)
                        ' MessageBox.Show($"当前使用次数为: {usageCount}", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information)
                    Else
                        ' MessageBox.Show("未找到有效的使用次数信息。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information)
                    End If
                Else
                    MessageBox.Show("未找到注册表路径：Software\MyApp", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End If
            End Using
            state = False

            Return result
        Catch ex As Exception
            Return -1
            '  MessageBox.Show($"读取注册表错误: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Function

    Public Sub WriteRegistryValue(str As String, state As Boolean, ParamArray pint() As Integer) '写入注册表
        Try
            If pint.Length < 2 Then
                Throw
            End If

            ' 创建或打开注册表键
            Using key As RegistryKey = Registry.CurrentUser.CreateSubKey(str)
                If key IsNot Nothing Then
                    ' 写入值
                    key.SetValue("UsageCount", pint(0), RegistryValueKind.DWord)
                    key.SetValue("Userstate"， pint(1), RegistryValueKind.Binary)
                    'MessageBox.Show("使用次数已成功写入注册表。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Else
                    ' MessageBox.Show("未能打开或创建注册表路径：Software\MyApp", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End If

            End Using
        Catch ex As Exception
            'MessageBox.Show($"写入注册表错误: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

End Class