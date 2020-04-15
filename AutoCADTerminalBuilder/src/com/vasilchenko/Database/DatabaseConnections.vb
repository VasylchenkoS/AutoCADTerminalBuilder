Imports System.Data.OleDb
Imports System.Data.SqlClient

Namespace com.vasilchenko.Database
    Module DatabaseConnections
        Public Function GetOleDataTable(sqlQuery As String, strSourcePath As String) As DataTable
            Dim sqlConnectionParameters As String
            Dim objOleDbCommand As OleDbCommand
            Dim objDataTable As New DataTable

            sqlConnectionParameters = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" & strSourcePath & ";"
            Using objOleDbConnection As New OleDbConnection(sqlConnectionParameters)
                Try
                    objOleDbConnection.Open()
                    objOleDbCommand = New OleDbCommand(sqlQuery, objOleDbConnection)
                    objDataTable.Load(objOleDbCommand.ExecuteReader)

                    Return objDataTable
                Catch ex As Exception
                    MsgBox(ex.Message)
                    Throw New ArgumentNullException
                End Try
            End Using
        End Function

        Public Function GetSqlDataTable(sqlQuery As String, sqlConnectionParameters As String) As DataTable
            Dim objDataTable As New DataTable

            Using objSqlDbConnection As New SqlConnection(sqlConnectionParameters)
                Try
                    objSqlDbConnection.Open()
                    Dim objSqlDbCommand = New SqlCommand(sqlQuery, objSqlDbConnection)
                    objDataTable.Load(objSqlDbCommand.ExecuteReader)

                    Return objDataTable
                Catch ex As Exception
                    MsgBox(ex.Message)
                    Throw New ArgumentNullException
                End Try
            End Using
        End Function

    End Module

End Namespace
