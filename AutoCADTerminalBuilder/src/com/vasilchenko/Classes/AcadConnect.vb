Imports AutoCADTerminalBuilder.com.vasilchenko.Enums
Imports AutoCADTerminalBuilder.com.vasilchenko.Modules
Imports Autodesk.AutoCAD.DatabaseServices
Imports Autodesk.AutoCAD.Geometry

Namespace com.vasilchenko.Classes
    Public Class AcadConnector

        Public Function FillTerminalData(strTagstrip As String, strTerminalNumber As String, strLocation As String, Optional sideEnum As SideEnum = SideEnum.Right) As MultilevelTerminalClass
            Dim acTerminal = Database.FillTerminalData(strTagstrip, strTerminalNumber, strLocation)
            Database.FillObjectBlockPath(acTerminal)
            Return acTerminal
        End Function

        Public Sub DrawTerminalBlock(acDatabase As Autodesk.AutoCAD.DatabaseServices.Database, acTransaction As Transaction, objInputTerminal As MultilevelTerminalClass,
            acInsertPt As Point3d, acBlckRef As BlockReference, Optional dblScale As Double = 1.0)

            Dim blnDetailsTerminal As Boolean
            Dim blnNoCable = False

            IIf(acBlckRef.Name.StartsWith("SIM"), blnDetailsTerminal = False, blnDetailsTerminal = True)

            For Each objSingleTerminal As KeyValuePair(Of String, SingleLevelTerminalClass) In objInputTerminal.Terminal
                Dim acCurInsertPt = New Point3d(acInsertPt.X + (CInt(objSingleTerminal.Key) - 1) * 300, acInsertPt.Y, acInsertPt.Z)
                DrawBlocks.DrawTerminalBlock(acDatabase, acTransaction, objInputTerminal, objSingleTerminal, acCurInsertPt, dblScale, blnDetailsTerminal, False)
            Next
        End Sub

        Public Function GetWireMarkingList() As List(Of String)
            Return Database.GetWireMarkingList()
        End Function

        Public Function GetAddressMarkingList() As List(Of MarkingClass)
            Return Database.GetAddressMarkingList()
        End Function

    End Class
End Namespace
