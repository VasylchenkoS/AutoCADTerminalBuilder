Imports System.Linq
Imports AutoCADTerminalBuilder.com.vasilchenko.Classes
Imports AutoCADTerminalBuilder.com.vasilchenko.Database
Imports AutoCADTerminalBuilder.com.vasilchenko.Enums
Imports Autodesk.AutoCAD.EditorInput
Imports Autodesk.AutoCAD.Geometry
Imports Autodesk.AutoCAD.ApplicationServices
Imports Autodesk.AutoCAD.DatabaseServices

Namespace com.vasilchenko.Modules
    Module TerminalClassMapping

        Friend Sub CreateTerminalBlock(strLocation As String, strTagstrip As String, orientationEnum As OrientationEnum, sideEnum As SideEnum)

            Dim dblScale As Double = 1

            Dim blnNoCable = False

            Dim objTerminalStripList As New TerminalStripClass
            Dim acDatabase As Autodesk.AutoCAD.DatabaseServices.Database = HostApplicationServices.WorkingDatabase()

            Dim objTermsNumList As List(Of String) = DataAccessObject.GetAllTermsInStrip(strLocation, strTagstrip)
            objTerminalStripList.TerminalList = DataAccessObject.FillListTerminalData(objTermsNumList, strTagstrip, sideEnum, strLocation)

            objTerminalStripList.TerminalList.Sort(Function(x As MultilevelTerminalClass, y As MultilevelTerminalClass)
                                                       Return AdditionalFunctions.GetLastNumericFromString(x.MainTermNumber).CompareTo(AdditionalFunctions.GetLastNumericFromString(y.MainTermNumber))
                                                   End Function)

            objTerminalStripList.JumperList = FillStripByJumpers(objTerminalStripList)

            Dim ufTerminalTypeSelector As New ufTerminalTypeSelector
            With ufTerminalTypeSelector
                ufTerminalTypeSelector.ShowDialog()
                If .rbtnSignalisation.Checked Then
                    AdditionalFunctions.AddAccessoriesForSignalisation(objTerminalStripList.TerminalList)
                ElseIf .rbtnMeasurement.Checked Then
                    AdditionalFunctions.AddAccessoriesForMeasurement(objTerminalStripList.TerminalList)
                ElseIf .rbtnControl.Checked Then
                    AdditionalFunctions.AddAccessoriesForControl(objTerminalStripList.TerminalList)
                ElseIf .rbtnPower.Checked Then
                    AdditionalFunctions.AddAccessoriesForPower(objTerminalStripList.TerminalList)
                ElseIf .rbtnMetering.Checked Then
                    AdditionalFunctions.AddAccessoriesForMetering(objTerminalStripList.TerminalList)
                ElseIf .rbtnRZA.Checked Then
                    AdditionalFunctions.AddAccessoriesForRZA(objTerminalStripList.TerminalList)
                    dblScale = 0.6
                ElseIf .rbtnTMRZA.Checked Then
                    AdditionalFunctions.AddAccessoriesForTMRZA(objTerminalStripList.TerminalList)
                    blnNoCable = True
                    dblScale = 0.6
                Else
                    Exit Sub
                End If
            End With

            DrawLayerChecker.CheckLayers()

            Dim objJumper As JumperClass
            Dim blnDetailsTerminal As Boolean
            Select Case MsgBox("Клеммник делаем детализированный?", MsgBoxStyle.YesNoCancel, "Определяем вид клеммника")
                Case MsgBoxResult.Yes
                    blnDetailsTerminal = True
                Case MsgBoxResult.No
                    blnDetailsTerminal = False
                Case MsgBoxResult.Cancel
                    Throw New Exception
            End Select

            Using acTransaction As Transaction = acDatabase.TransactionManager.StartTransaction
                Dim acInsertPt As Point3d = SetInsertPoint()
                Dim acPrevTerminal As New MultilevelTerminalClass

                For Each objCurItem As MultilevelTerminalClass In objTerminalStripList.TerminalList
                    If IsNothing(objCurItem) Then
                        Throw New ArgumentNullException(String.Format("Объект после {0}:{1} не существует", acPrevTerminal.TagStrip, acPrevTerminal.MainTermNumber))
                    ElseIf IsNothing(objCurItem.Terminal) Then
                        DrawBlocks.DrawAccessorisesBlock(acDatabase, acTransaction, blnDetailsTerminal, objCurItem, acInsertPt, dblScale)
                    Else
                        If blnDetailsTerminal Then
                            DrawBlocks.DrawTerminalBlock(acDatabase, acTransaction, objCurItem, New KeyValuePair(Of String, SingleLevelTerminalClass)(objCurItem.Terminal.Keys(0), objCurItem.Terminal.Values(0)), acInsertPt, dblScale, blnDetailsTerminal, blnNoCable)
                            objJumper = objTerminalStripList.JumperList.Find(Function(x) x.StartTermNum = objCurItem.Terminal.Values(0).TerminalNumber)
                            If objJumper IsNot Nothing Then
                                Dim acJumperInsertPt As Point3d = CalculateJumperInsertPoint(dblScale, acInsertPt, objJumper, objCurItem, blnDetailsTerminal)
                                DrawBlocks.DrawJumpers(acDatabase, acTransaction, objJumper, acJumperInsertPt, dblScale, blnDetailsTerminal)
                            End If
                        Else
                            For Each objSingleTerminal As KeyValuePair(Of String, SingleLevelTerminalClass) In objCurItem.Terminal
                                Dim acCurInsertPt = New Point3d(acInsertPt.X + (CInt(objSingleTerminal.Key) - 1) * 300, acInsertPt.Y, acInsertPt.Z)
                                DrawBlocks.DrawTerminalBlock(acDatabase, acTransaction, objCurItem, objSingleTerminal, acCurInsertPt, dblScale, blnDetailsTerminal, blnNoCable)
                                objJumper = objTerminalStripList.JumperList.Find(Function(x) x.StartTermNum = objSingleTerminal.Value.TerminalNumber)
                                If objJumper IsNot Nothing Then
                                    Dim acJumperInsertPt As Point3d = CalculateJumperInsertPoint(dblScale, acCurInsertPt, objJumper, objCurItem, blnDetailsTerminal)
                                    DrawBlocks.DrawJumpers(acDatabase, acTransaction, objJumper, acJumperInsertPt, dblScale, blnDetailsTerminal)
                                End If
                            Next
                        End If
                    End If
                    acInsertPt = New Point3d(acInsertPt.X, acInsertPt.Y - (objCurItem.Height * dblScale), acInsertPt.Z)
                    acPrevTerminal = objCurItem
                Next
                acTransaction.Commit()
            End Using
        End Sub

        Private Function SetInsertPoint() As Point3d
            Dim acPromptPntOpt As New PromptPointOptions("Выберите точку вставки для уровня")
            Dim acPromptPntResult As PromptPointResult = Application.DocumentManager.MdiActiveDocument.Editor.GetPoint(acPromptPntOpt)

            If acPromptPntResult.Status <> PromptStatus.OK Then
                MsgBox("fail to get the insert point")
                Throw New ArgumentNullException
            End If
            Return acPromptPntResult.Value
        End Function

        Private Function FillStripByJumpers(objTerminalStripList As TerminalStripClass) As List(Of JumperClass)
            Dim objPrevItem As JumperClass = Nothing
            Dim objJumperList As List(Of JumperClass) = DataAccessObject.FillJumperData(objTerminalStripList.TerminalList(0).Location, objTerminalStripList.TerminalList(0).TagStrip)

            objJumperList.Sort(Function(x, y) x.StartTermNum < y.StartTermNum)

            For Each objCurItem As JumperClass In objJumperList
                If objPrevItem Is Nothing Then
                    objCurItem.Side = SideEnum.Right
                ElseIf ((objPrevItem.StartTermNum + objPrevItem.TermCount - 1) = objCurItem.StartTermNum) Then

                    Dim temp = objTerminalStripList.TerminalList.Find(Function(x) x.Terminal.Values.Any(Function(y) y.TerminalNumber = objCurItem.StartTermNum))
                    If temp.Manufacture.ToLower.Equals("klemsan") AndAlso Not temp.Catalog.Equals("AVK 2,5 R Gray") Then
                        Throw New Exception("На клемме " & objCurItem.StartTermNum & " невозможно присоединить 2 перемычки")
                    End If

                    If objPrevItem.Side = SideEnum.Right Then
                        objCurItem.Side = SideEnum.Left
                    Else
                        objCurItem.Side = SideEnum.Right
                    End If

                Else
                    objCurItem.Side = SideEnum.Right
                End If
                If objTerminalStripList.TerminalList(0).TextInstance.StartsWith("7") Then objCurItem.TextInstance = "763.КЛЕММЫ-ПЕРЕМЫЧКИ"
                objPrevItem = objCurItem
            Next
            Return objJumperList
        End Function

        Private Function CalculateJumperInsertPoint(dblScale As Double, acInsertPt As Point3d, objJumper As JumperClass, objCurItem As MultilevelTerminalClass, blnDetailsTerminal As Boolean) As Point3d

            Dim acJumperInsertPt As Point3d

            If blnDetailsTerminal Then
                If objCurItem.Catalog.StartsWith("UT ") OrElse objCurItem.Catalog.StartsWith("ST ") And Not (objCurItem.Catalog.Contains("-MT")) Then
                    If objJumper.Side = SideEnum.Left Then
                        acJumperInsertPt = New Point3d(acInsertPt.X - (2.5 * dblScale), acInsertPt.Y - ((objCurItem.Height / 2) * dblScale), acInsertPt.Z)
                    Else
                        acJumperInsertPt = New Point3d(acInsertPt.X + (2.5 * dblScale), acInsertPt.Y - ((objCurItem.Height / 2) * dblScale), acInsertPt.Z)
                    End If
                ElseIf objCurItem.Catalog.Equals("UT 2,5-MT") Or objCurItem.Catalog.Equals("UT 4-MT") Or objCurItem.Catalog.Equals("UT 4-MTD-DIO/R-L") Then
                    If objJumper.Side = SideEnum.Left Then
                        acJumperInsertPt = New Point3d(acInsertPt.X + (2.5 * dblScale), acInsertPt.Y - ((objCurItem.Height / 2) * dblScale), acInsertPt.Z)
                    Else
                        acJumperInsertPt = New Point3d(acInsertPt.X + (7.5 * dblScale), acInsertPt.Y - ((objCurItem.Height / 2) * dblScale), acInsertPt.Z)
                    End If
                ElseIf objCurItem.Catalog.StartsWith("UTTB 2,5") OrElse objCurItem.Catalog.StartsWith("STTB 2,5") Then
                    If objJumper.Side = SideEnum.Left Then
                        acJumperInsertPt = New Point3d(acInsertPt.X + 15, acInsertPt.Y - (6.425 * dblScale), acInsertPt.Z)
                    Else
                        acJumperInsertPt = New Point3d(acInsertPt.X + 20, acInsertPt.Y - (6.425 * dblScale), acInsertPt.Z)
                    End If
                ElseIf objCurItem.Catalog = "URTK 6" Then
                    If objJumper.Catalog.StartsWith("SB") Then
                        acJumperInsertPt = New Point3d(acInsertPt.X - 25, acInsertPt.Y - (4.075 * dblScale), acInsertPt.Z)
                    ElseIf objJumper.Catalog.StartsWith("FBRI") Then
                        acJumperInsertPt = New Point3d(acInsertPt.X + 15, acInsertPt.Y - (4.075 * dblScale), acInsertPt.Z)
                    End If

                ElseIf objCurItem.Catalog.StartsWith("AVK 2,5 R") Then
                    If objJumper.Side = SideEnum.Left Then
                        acJumperInsertPt = New Point3d(acInsertPt.X - 10, acInsertPt.Y - ((objCurItem.Height / 2) * dblScale), acInsertPt.Z)
                    Else
                        acJumperInsertPt = New Point3d(acInsertPt.X - 5, acInsertPt.Y - ((objCurItem.Height / 2) * dblScale), acInsertPt.Z)
                    End If
                ElseIf objCurItem.Catalog.StartsWith("AVK 4 A") Or objCurItem.Catalog.StartsWith("AVK 2,5 A") Then
                    acJumperInsertPt = New Point3d(acInsertPt.X + 10, acInsertPt.Y - ((objCurItem.Height / 2) * dblScale), acInsertPt.Z)
                ElseIf objCurItem.Catalog.StartsWith("AVK 6") Then
                    acJumperInsertPt = New Point3d(acInsertPt.X, acInsertPt.Y - ((objCurItem.Height / 2) * dblScale), acInsertPt.Z)
                ElseIf objCurItem.Catalog.StartsWith("WGO 4") Then
                    If objJumper.Catalog.StartsWith("UK") Then
                        acJumperInsertPt = New Point3d(acInsertPt.X + 15, acInsertPt.Y - ((objCurItem.Height / 2) * dblScale), acInsertPt.Z)
                    ElseIf objJumper.Catalog.StartsWith("IZUK") Then
                        acJumperInsertPt = New Point3d(acInsertPt.X - 25, acInsertPt.Y - ((objCurItem.Height / 2) * dblScale), acInsertPt.Z)
                    End If

                ElseIf objCurItem.Catalog.StartsWith("WDU 2.5") Then
                    If objJumper.Side = SideEnum.Left Then
                        acJumperInsertPt = New Point3d(acInsertPt.X - (8.5 * dblScale), acInsertPt.Y - ((objCurItem.Height / 2) * dblScale), acInsertPt.Z)
                    Else
                        acJumperInsertPt = New Point3d(acInsertPt.X + (5 * dblScale), acInsertPt.Y - ((objCurItem.Height / 2) * dblScale), acInsertPt.Z)
                    End If
                End If
            Else
                If objJumper.Side = SideEnum.Left Then
                    acJumperInsertPt = New Point3d(acInsertPt.X - (10 * dblScale), acInsertPt.Y - ((objCurItem.Height / 2) * dblScale), acInsertPt.Z)
                Else
                    acJumperInsertPt = New Point3d(acInsertPt.X + (10 * dblScale), acInsertPt.Y - ((objCurItem.Height / 2) * dblScale), acInsertPt.Z)
                End If
            End If
            Return acJumperInsertPt
        End Function

    End Module
End Namespace
