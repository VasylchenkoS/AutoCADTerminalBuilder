' (C) Copyright 2011 by v.vasilchenko'
Imports Autodesk.AutoCAD.Runtime
Imports Autodesk.AutoCAD.ApplicationServices
Imports System.Runtime.InteropServices

' This line is not mandatory, but improves loading performances
<Assembly: CommandClass(GetType(AutoCADTerminalBuilder.MyCommands))>
Namespace AutoCADTerminalBuilder

    ' This class is instantiated by AutoCAD for each document when
    ' a command is called by the user the first time in the context
    ' of a given document. In other words, non static data in this class
    ' is implicitly per-document!
    Public Class MyCommands

        <DllImport("accore.dll", CallingConvention:=CallingConvention.Cdecl, EntryPoint:="acedTrans")>
        Public Shared Function acedTrans(ByVal point As Double(), ByVal fromRb As IntPtr, ByVal toRb As IntPtr, ByVal disp As Integer, ByVal result As Double()) As Integer
        End Function

         <CommandMethod("ASU_New_Terminal_Builder", CommandFlags.Session)>
        Public Shared Sub Builder()

            Application.AcadApplication.ActiveDocument.SendCommand("(command ""_-Purge"")(command ""_ALL"")(command ""*"")(command ""_N"")" & vbCr)
            Application.AcadApplication.ActiveDocument.SendCommand("AEREBUILDDB" & vbCr)

            If Application.GetSystemVariable("MIRRTEXT").Equals("1") Then
                Application.SetSystemVariable("MIRRTEXT", 0)
                Application.DocumentManager.MdiActiveDocument.Editor.WriteMessage("MIRRTEXT variable set to 0")
            End If

            Using docLock As DocumentLock = Application.DocumentManager.MdiActiveDocument.LockDocument()
                Dim objForm = New ufTerminalSelector
                Try
                    objForm.ShowDialog()
                Catch ex As Exception
                    MsgBox("ERROR:[" & ex.Message & "]" & vbCr & "TargetSite: " & ex.TargetSite.ToString & vbCr & "StackTrace: " & ex.StackTrace, vbCritical, "ERROR!")
                End Try
            End Using

        End Sub

    End Class

End Namespace