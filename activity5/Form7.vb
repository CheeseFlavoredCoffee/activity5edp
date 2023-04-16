﻿Imports System.IO
Imports MySql.Data.MySqlClient

Public Class Form7
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        With Me
            Call Connect_to_DB()
            Dim mycmd As New MySqlCommand
            Try
                strSQL = "Insert into renter values('" _
                & .TextBox1.Text & "', '" _
                & .TextBox2.Text & "','" _
                & .TextBox3.Text & "','" _
                & .TextBox4.Text & "')"
                mycmd.CommandText = strSQL
                mycmd.Connection = myconn
                mycmd.ExecuteNonQuery()
                MsgBox("Renter Info Successfully Added")
                Call Clear_Boxes()
            Catch ex As MySqlException
                MsgBox(ex.Number & " " & ex.Message)
            End Try
            Disconnect_to_DB()
        End With
    End Sub

    Private Sub Clear_Boxes()
        With Me
            .TextBox1.Text = vbNullString
            .TextBox2.Text = ""
            .TextBox3.Text = ""
            .TextBox4.Text = vbNullString
        End With
    End Sub

    Private Sub SearchButton_Click(sender As Object, e As EventArgs) Handles SearchButton.Click
        Call Connect_to_DB()
        With Me
            Dim mycmd As New MySqlCommand
            Dim myreader As MySqlDataReader


            strSQL = "Select renter_id, renter_name, renter_address, renter_phoneno from renter where renter_name = '" _
                    & .TextboxSearch.Text & "'"
            'MsgBox(strSQL)
            mycmd.CommandText = strSQL
            mycmd.Connection = myconn
            myreader = mycmd.ExecuteReader
            If myreader.HasRows Then
                While myreader.Read()
                    .TextBox1.Text = myreader.GetString(0)
                    .TextBox2.Text = myreader.GetString(1)
                    .TextBox3.Text = myreader.GetString(2)
                    .TextBox4.Text = myreader.GetString(3)
                End While
            Else
                MsgBox("The renter you are searching for does not exist!")
            End If
            Call Disconnect_to_DB()

        End With
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        With Me
            Call Connect_to_DB()
            Dim mycmd As New MySqlCommand
            Try
                strSQL = "UPDATE renter SET renter_id = '" _
                & .TextBox1.Text & "', renter_address = '" _
                & .TextBox3.Text & "', renter_phoneno = '" _
                & .TextBox4.Text & "', WHERE renter_name = '" _
                & .TextBox2.Text & "'"
                mycmd.CommandText = strSQL
                mycmd.Connection = myconn
                mycmd.ExecuteNonQuery()
                MsgBox("Renter Info Successfully Updated")
                Call Clear_Boxes()
            Catch ex As MySqlException
                MsgBox(ex.Number & " " & ex.Message)
            End Try
            Disconnect_to_DB()
        End With
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        With Me
            Call Connect_to_DB()
            Dim mycmd As New MySqlCommand
            Try
                Dim answer As MsgBoxResult
                answer = MsgBox("Are you sure you want to delete this renter", MsgBoxStyle.YesNo)
                If answer = MsgBoxResult.Yes Then
                    strSQL = "Delete from renter" _
                                    & " where renter_name = '" _
                                    & .TextBox2.Text & "'"
                    'MsgBox(strSQL)
                    mycmd.CommandText = strSQL
                    mycmd.Connection = myconn
                    mycmd.ExecuteNonQuery()
                    MsgBox("Renter Successfully Deleted")
                    Call Clear_Boxes()
                End If

            Catch ex As MySqlException
                MsgBox(ex.Number & " " & ex.Message)
            End Try
            Disconnect_to_DB()
        End With
    End Sub

    Private Sub Form2_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim connection As New MySqlConnection("datasource=localhost; port=3306; username=root; password=password")
        Dim table As New DataTable()
        Dim adapter As New MySqlDataAdapter("SELECT * FROM lib_db.renter", connection)

        adapter.Fill(table)

        DataGridView1.DataSource = table
    End Sub

    Private Sub Button9_Click(sender As Object, e As EventArgs) Handles Button9.Click
        Dim table As New DataTable()
        Dim adapter As New MySqlDataAdapter("SELECT * FROM lib_db.renter", myconn)

        adapter.Fill(table)
        DataGridView1.DataSource = table
    End Sub

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        Me.Hide()
        Form1.Show()
    End Sub

    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
        Me.Hide()
        Form2.Show()
    End Sub

    Private Sub Button6_Click(sender As Object, e As EventArgs) Handles Button6.Click
        Me.Hide()
        Form4.Show()
    End Sub

    Private Sub Button7_Click(sender As Object, e As EventArgs) Handles Button7.Click
        Me.Hide()
        Form5.Show()
    End Sub

    Private Sub Button8_Click(sender As Object, e As EventArgs) Handles Button8.Click
        Me.Close()
    End Sub

    Private Sub Button10_Click(sender As Object, e As EventArgs) Handles Button10.Click
        Dim dlg As New OpenFileDialog()
        dlg.Filter = "CSV Files (*.csv)|*.csv|All Files (*.*)|*.*"
        If dlg.ShowDialog() = DialogResult.OK Then
            Dim dt As New DataTable()
            Using reader As New StreamReader(dlg.FileName)
                Dim header As Boolean = True
                While Not reader.EndOfStream
                    Dim line As String = reader.ReadLine()
                    Dim values As String() = line.Split(","c)
                    If header Then
                        For Each value As String In values
                            dt.Columns.Add(value)
                        Next
                        header = False
                    Else
                        dt.Rows.Add(values)
                    End If
                End While
            End Using

            Dim connectionString As String = "Server=127.0.0.1;Database=lib_db;Uid=root;Pwd=password;"
            Using connection As New MySqlConnection(connectionString)
                connection.Open()

                For Each row As DataRow In dt.Rows
                    Dim insertSql As String = "INSERT INTO renter ("
                    Dim valuesSql As String = "VALUES ("
                    For Each column As DataColumn In dt.Columns
                        insertSql += "`" + column.ColumnName + "`, "
                        valuesSql += "@" + column.ColumnName + ", "
                    Next
                    insertSql = insertSql.TrimEnd(", ".ToCharArray()) + ")"
                    valuesSql = valuesSql.TrimEnd(", ".ToCharArray()) + ")"
                    Dim insertCommand As New MySqlCommand(insertSql + valuesSql, connection)
                    For Each column As DataColumn In dt.Columns
                        insertCommand.Parameters.AddWithValue("@" + column.ColumnName, row(column))
                    Next
                    insertCommand.ExecuteNonQuery()
                Next

                MessageBox.Show("CSV file imported successfully!")
            End Using
        End If
    End Sub

    Private Sub Button11_Click(sender As Object, e As EventArgs) Handles Button11.Click
        Dim connectionString As String = "Server=127.0.0.1;Database=lib_db;Uid=root;Pwd=password;"
        Using connection As New MySqlConnection(connectionString)
            connection.Open()

            Dim selectSql As String = "SELECT * FROM renter"
            Dim selectCommand As New MySqlCommand(selectSql, connection)
            Dim adapter As New MySqlDataAdapter(selectCommand)
            Dim dt As New DataTable()
            adapter.Fill(dt)

            Dim saveFileDialog As New SaveFileDialog()
            saveFileDialog.Filter = "CSV file (*.csv)|*.csv"
            saveFileDialog.Title = "Export CSV file"
            If saveFileDialog.ShowDialog() <> DialogResult.OK Then
                Exit Sub
            End If

            Using writer As New StreamWriter(saveFileDialog.FileName)
                For i As Integer = 0 To dt.Columns.Count - 1
                    writer.Write(dt.Columns(i).ColumnName)
                    If i < dt.Columns.Count - 1 Then
                        writer.Write(",")
                    End If
                Next
                writer.WriteLine()

                For Each row As DataRow In dt.Rows
                    For i As Integer = 0 To dt.Columns.Count - 1
                        writer.Write(row(i).ToString())
                        If i < dt.Columns.Count - 1 Then
                            writer.Write(",")
                        End If
                    Next
                    writer.WriteLine()
                Next
            End Using
        End Using

        MessageBox.Show("Export Completed!")
    End Sub
End Class