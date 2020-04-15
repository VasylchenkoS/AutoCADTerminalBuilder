Imports System.Linq
Imports AutoCADTerminalBuilder.com.vasilchenko.Classes
Imports AutoCADTerminalBuilder.com.vasilchenko.Enums
Imports AutoCADTerminalBuilder.com.vasilchenko.Modules
Imports Autodesk.AutoCAD.EditorInput
Imports Autodesk.AutoCAD.ApplicationServices
Imports Autodesk.Electrical.Project
Imports System.Text

Namespace com.vasilchenko.Database

    Module DataAccessObject

        Private Const DeatilPanelGraphicsPath As String = "D:\Autocad Additional Files\MyDatabase\User Graphics\Panel\2D_Graphics\"
        Private Const SimplePanelGraphicsPath As String = "D:\Autocad Additional Files\MyDatabase\User Graphics\Panel\Simple_Graphics\"
        Dim strConstProjectDatabasePath As String = ""

        Friend Function GetAllLocations() As ArrayList
            Dim dbOleDataTable As DataTable
            Dim strSqlQuery As String
            Dim locationList As New ArrayList
            strConstProjectDatabasePath = ProjectManager.GetInstance().GetActiveProject().GetDbFullPath()

            If Not IO.File.Exists(strConstProjectDatabasePath) Then
                MsgBox("Source file not found. File way: " & strConstProjectDatabasePath & " Please open some project file and repeat.", vbCritical, "File Not Found")
                Throw New ArgumentNullException
            End If

            strSqlQuery = "SELECT DISTINCT [LOC] FROM TERMS ORDER BY [LOC] DESC"

            dbOleDataTable = DatabaseConnections.GetOleDataTable(strSqlQuery, strConstProjectDatabasePath)

            Try
                For Each dbRow In dbOleDataTable.Rows
                    locationList.Add(dbRow.Item("LOC"))
                Next dbRow
            Catch ex As Exception
                MsgBox("Что-то не так: " & vbCrLf & ex.Message)
            End Try

            Return locationList
        End Function

        Friend Function GetAllTagstripInLocation(strLocation As String) As Object
            Dim dbOleDataTable As DataTable
            Dim strSqlQuery As String
            Dim tagstripList As New ArrayList

            strSqlQuery = "SELECT DISTINCT [TAGSTRIP] " &
                            "FROM TERMS " &
                            "WHERE LOC = '" & strLocation & "' " &
                            "ORDER BY [TAGSTRIP] ASC"

            dbOleDataTable = DatabaseConnections.GetOleDataTable(strSqlQuery, strConstProjectDatabasePath)

            Try
                For Each dbRow In dbOleDataTable.Rows
                    tagstripList.Add(dbRow.Item("TAGSTRIP"))
                Next dbRow
            Catch ex As Exception
                MsgBox("Что-то не так: " & vbCrLf & ex.Message)
            End Try

            Return tagstripList
        End Function

        Friend Function GetAllTermsInStrip(strLocation As String, strTagstrip As String) As List(Of String)
            Dim dbOleDataTable As DataTable
            Dim sqlQuery As String
            Dim objTerminalList As New List(Of String)

            sqlQuery = "SELECT DISTINCT [TERM] " &
                            "FROM TERMS " &
                            "WHERE [TAGSTRIP] = '" & strTagstrip & "' AND [LOC] = '" & strLocation & "' AND [TERM] IS NOT NULL AND [LNUMBER] IS NOT NULL"

            dbOleDataTable = DatabaseConnections.GetOleDataTable(sqlQuery, strConstProjectDatabasePath)

            Try
                If Not IsNothing(dbOleDataTable) Then
                    For Each dbRow In dbOleDataTable.Rows
                        objTerminalList.Add(dbRow.Item("TERM"))
                    Next dbRow
                End If
            Catch ex As Exception
                MsgBox("Что-то не так: " & vbCrLf & ex.Message)
            End Try

            objTerminalList = objTerminalList.OrderBy(Function(x) AdditionalFunctions.GetLastNumericFromString(x)).ToList()

            Return objTerminalList
        End Function

        Friend Function FillListTerminalData(objTermsNumList As List(Of String), strTagstrip As String, sideEnum As SideEnum, strLocation As String) As List(Of MultilevelTerminalClass)
            Dim objMultilevelTerminalList As New List(Of MultilevelTerminalClass)

            For Each strTerminalNumber In objTermsNumList
                objMultilevelTerminalList.Add(FillTerminalData(strTagstrip, strTerminalNumber, strLocation, sideEnum))
            Next
            objMultilevelTerminalList = UnionTerminals(objMultilevelTerminalList)

            Return objMultilevelTerminalList
        End Function

        Private Function UnionTerminals(objMultilevelTerminalList As List(Of MultilevelTerminalClass)) As List(Of MultilevelTerminalClass)
            Dim objResultArray As New List(Of MultilevelTerminalClass)

            objMultilevelTerminalList.Sort(Function(x, y) x.Terminal.Keys(0).CompareTo(y.Terminal.Keys(0)))

            For Each objTerminal In objMultilevelTerminalList
                If objTerminal.Terminal.Keys(0).Equals("01") OrElse Not objResultArray.Any(Function(x As MultilevelTerminalClass) x.TerminalLink = objTerminal.TerminalLink) Then
                    objResultArray.Add(objTerminal)
                ElseIf objResultArray.Any(Function(x As MultilevelTerminalClass) x.TerminalLink = objTerminal.TerminalLink) Then
                    objResultArray.First(Function(x As MultilevelTerminalClass) x.TerminalLink = objTerminal.TerminalLink).Terminal.Add(objTerminal.Terminal.Keys(0), objTerminal.Terminal.Values(0))
                Else
                    Throw New ArgumentNullException(String.Format("Клемма не связана: {0}:{1}", objTerminal.TagStrip, objTerminal.Terminal.Values(0).TerminalNumber))
                End If
            Next

            If Not IsNothing(objResultArray.Find(Function(x As MultilevelTerminalClass) x.Terminal.IndexOfKey("02") = 0 OrElse x.Terminal.IndexOfKey("03") = 0)) Then
                Dim strTemp As New StringBuilder
                objResultArray.FindAll(Function(x As MultilevelTerminalClass) x.Terminal.IndexOfKey("02") = 0 OrElse x.Terminal.IndexOfKey("03") = 0).ToList.ForEach(Function(x) strTemp.AppendLine(x.TerminalLink))
                Throw New ArgumentException("Существуют клеммы верхнего уровня без нижнего: " & strTemp.ToString)
            End If

            Return objResultArray
        End Function

        Friend Function FillTerminalData(strTagstrip As String, strTerminalNumber As String, strLocation As String, Optional sideEnum As SideEnum = SideEnum.Right) As MultilevelTerminalClass
            Dim dbOleDataTable As DataTable
            Dim sqlQuery As String
            Dim objTerminalList As New SortedList(Of String, SingleLevelTerminalClass)(New TextComparator)
            Dim objMultilevelTerminal As New MultilevelTerminalClass
            Dim objSingleTerminal As New SingleLevelTerminalClass
            Dim strLevelNumber = ""

            objMultilevelTerminal.TagStrip = strTagstrip
            objMultilevelTerminal.Location = strLocation
            objSingleTerminal.TerminalNumber = strTerminalNumber

            strConstProjectDatabasePath = ProjectManager.GetInstance().GetActiveProject().GetDbFullPath()

            If Not IO.File.Exists(strConstProjectDatabasePath) Then
                MsgBox("Source file not found. File way: " & strConstProjectDatabasePath & " Please open some project file and repeat.", vbCritical, "File Not Found")
                Throw New ArgumentNullException
            End If

            sqlQuery = "SELECT [INST], [MFG], [CAT], [HDL], [LNUMBER], [LINKTERM], [DESC3] " &
                        "FROM TERMS " &
                        "WHERE [TAGSTRIP] = '" & strTagstrip & "' " &
                        "AND [TERM] = '" & strTerminalNumber & "' " &
                        "AND [LOC] = '" & strLocation & "' " &
                        "AND [CAT] IS NOT NULL AND [LINKTERM] IS NOT NULL"

            dbOleDataTable = DatabaseConnections.GetOleDataTable(sqlQuery, strConstProjectDatabasePath)
            Try
                If Not IsNothing(dbOleDataTable) Then
                    For Each dbRow In dbOleDataTable.Rows
                        objMultilevelTerminal.NumericInstance = AdditionalFunctions.GetLastNumericFromString(dbRow.Item("INST"))
                        objMultilevelTerminal.TextInstance = dbRow.Item("INST")
                        objMultilevelTerminal.Manufacture = dbRow.Item("MFG")
                        objMultilevelTerminal.Catalog = dbRow.Item("CAT")
                        objMultilevelTerminal.TerminalLink = dbRow.Item("LINKTERM")
                        objMultilevelTerminal.Notes = IIf(IsDBNull(dbRow.Item("DESC3")), "", dbRow.Item("DESC3"))
                        objSingleTerminal.Handle = dbRow.Item("HDL")
                        strLevelNumber = dbRow.Item("LNUMBER")
                    Next dbRow

                    FillTerminalConnectionsData(objSingleTerminal, strLocation, sideEnum)
                    objTerminalList.Add(strLevelNumber, objSingleTerminal)
                    objMultilevelTerminal.Terminal = objTerminalList
                    FillObjectBlockPath(objMultilevelTerminal)

                End If
            Catch ex As Exception
                MsgBox("Чего-то не хватает в клемме: " & strTagstrip & ":" & strTerminalNumber & vbCrLf & ex.Message)
            End Try
            Return objMultilevelTerminal
        End Function

        Friend Sub FillObjectBlockPath(objMultilevelTerminal As Object)
            Dim dbSqlDataTable As DataTable
            Dim sqlQuery As String = ""

            sqlQuery = "SELECT [BLKNAM] " &
                    "FROM [" & objMultilevelTerminal.Manufacture & "] " &
                    "WHERE [CATALOG] = '" & objMultilevelTerminal.Catalog & "'"
            dbSqlDataTable = DatabaseConnections.GetSqlDataTable(sqlQuery, My.Settings.footprint_lookupSQLConnectionString)

            Try
                objMultilevelTerminal.DetailBlockPath = DeatilPanelGraphicsPath & dbSqlDataTable.Rows(0).Item("BLKNAM")
                objMultilevelTerminal.SimpleBlockPath = SimplePanelGraphicsPath & "SIM_" & dbSqlDataTable.Rows(0).Item("BLKNAM")
            Catch ex As Exception
                MsgBox("Нет графики для: " & objMultilevelTerminal.Manufacture & " - " & objMultilevelTerminal.Catalog & vbCrLf & ex.Message)
            End Try

        End Sub

        Friend Sub FillTerminalConnectionsData(objSingleTerminal As SingleLevelTerminalClass, strLocation As String, sideEnum As SideEnum)
            Dim dbOleDataTable As DataTable
            Dim sqlQuery As String
            Dim acEditor As Editor = Application.DocumentManager.MdiActiveDocument.Editor

            Dim objWiresDictionary As New TerminalDictionaryClass(Of String, List(Of WireClass))

            sqlQuery = "SELECT [WIRENO], [INST1], [LOC1], [NAM1], [PIN1], [INST2], [LOC2], [NAM2], [PIN2], [CBL], [TERMDESC1], [TERMDESC2], [NAMHDL1] " &
                            "FROM WFRM2ALL " &
                            "WHERE ([NAMHDL1] = '" & objSingleTerminal.Handle & "' OR [NAMHDL2] = '" & objSingleTerminal.Handle & "') " &
                            "AND (([NAM1] IS NOT NULL AND [PIN1] IS NOT NULL) OR ([NAM2] IS NOT NULL AND [PIN2] IS NOT NULL)) " &
                            "ORDER BY [WIRENO]"

            dbOleDataTable = DatabaseConnections.GetOleDataTable(sqlQuery, strConstProjectDatabasePath)

            If Not IsNothing(dbOleDataTable) Then
                Dim strWireno As String = ""
                For Each dbRow In dbOleDataTable.Rows

                    Dim objWire As New WireClass
                    Dim objCable As New CableClass
                    Dim objWiresList As New List(Of WireClass)

                    If dbRow.Item("NAMHDL1").Equals(objSingleTerminal.Handle) Then
                        objWire.Instance = AdditionalFunctions.GetLastNumericFromString(dbRow.Item("INST2").ToString)
                        objWire.ConnTag = dbRow.Item("NAM2").ToString
                        objWire.ConnPin = dbRow.Item("PIN2").ToString
                        objCable.Destination = IIf(dbRow.Item("LOC2").ToString.Equals("(??)"), "", dbRow.Item("LOC2").ToString)
                        objWire.Termdesc = IIf(dbRow.Item("TERMDESC2").ToString.Equals("E") OrElse dbRow.Item("TERMDESC2").ToString.Equals("I"), "", dbRow.Item("TERMDESC2").ToString)
                    Else
                        objWire.Instance = AdditionalFunctions.GetLastNumericFromString(dbRow.Item("INST1").ToString)
                        objWire.ConnTag = dbRow.Item("NAM1").ToString
                        objWire.ConnPin = dbRow.Item("PIN1").ToString
                        objCable.Destination = IIf(dbRow.Item("LOC1").ToString.Equals("(??)"), "", dbRow.Item("LOC1").ToString)
                        objWire.Termdesc = IIf(dbRow.Item("TERMDESC1").ToString.Equals("E") OrElse dbRow.Item("TERMDESC1").ToString.Equals("I"), "", dbRow.Item("TERMDESC1").ToString)
                    End If

                    If Not IsDBNull(dbRow.Item("WIRENO")) Then
                        strWireno = dbRow.Item("WIRENO")
                        objWire.WireNumber = strWireno
                    Else
                        acEditor.WriteMessage("Для вывода " & objWire.ConnTag & ":" & objWire.ConnPin & " не назначено номера провода" & vbCrLf)
                        Exit Sub
                    End If

                    If Not IsDBNull(dbRow.Item("CBL")) Then
                        objCable.Mark = dbRow.Item("CBL")
                        objWire.Cable = objCable
                    End If

                    If Not (objWire.ConnTag.Equals("") And objWire.ConnPin.Equals("") And Not objWire.HasCable) Then
                        If objWiresDictionary.ContainsKey(strWireno) Then
                            objWiresDictionary.Item(strWireno).Add(objWire)
                        Else
                            objWiresList.Add(objWire)
                            objWiresDictionary.Add(key:=strWireno, value:=objWiresList)
                        End If
                    End If
                Next

                If objWiresDictionary.Count <> 0 Then

                    AdditionalFunctions.SortCollectionByInstAndCables(objWiresDictionary)
                    AdditionalFunctions.SortDictionaryByInstAndCables(objWiresDictionary, strLocation)

                    If objWiresDictionary.Count > 2 Then
                        acEditor.WriteMessage("[WARNING]:Слишком много разной маркировки проводов для клеммы " & objSingleTerminal.TerminalNumber)
                        Throw New ArgumentOutOfRangeException
                    ElseIf objWiresDictionary.Count = 1 Then
                        Dim objTempList As List(Of WireClass) = objWiresDictionary.Item(0)
                        Dim intCableNum As Integer = AdditionalFunctions.CableInList(objTempList)

                        Dim objA = objTempList.FindAll(Function(x) x.WireNumber.StartsWith("D"))
                        Dim objB = Nothing
                        If objTempList.Find(Function(x) x.WireNumber.StartsWith("S")) IsNot Nothing Then
                            objB = objTempList.Find(Function(x) x.WireNumber.StartsWith("S"))
                        ElseIf objTempList.Find(Function(x) x.WireNumber.StartsWith("C")) IsNot Nothing Then
                            objB = objTempList.Find(Function(x) x.WireNumber.StartsWith("C"))
                        ElseIf objTempList.Find(Function(x) x.WireNumber.StartsWith("M")) IsNot Nothing Then
                            objB = objTempList.Find(Function(x) x.WireNumber.StartsWith("M"))
                        End If

                        If intCableNum <> -1 And Not objTempList.Count = 1 Then
                            objSingleTerminal.AddWireLeft = objTempList.Item(intCableNum)
                            objTempList.RemoveAt(intCableNum)
                            objSingleTerminal.WiresRigthList = (From x In objTempList
                                                                    Select x).ToList
                        ElseIf intCableNum <> -1 And objTempList.Count = 1 Then
                            objSingleTerminal.AddWireRigth = objTempList.Item(intCableNum)
                            objTempList.RemoveAt(intCableNum)
                        Else
                            If objA IsNot Nothing Then
                                objSingleTerminal.WiresLeftList = objA
                                objTempList.RemoveAll(Function(x) x.WireNumber.StartsWith("D"))
                            ElseIf objB IsNot Nothing Then
                                objSingleTerminal.AddWireRigth = objB
                                objTempList.Remove(objB)
                            End If

                            For i As Short = 0 To objTempList.Count - 1
                                If (i + 1) Mod 2 = 0 Then
                                    objSingleTerminal.AddWireLeft = objTempList.Item(i)
                                Else
                                    objSingleTerminal.AddWireRigth = objTempList.Item(i)
                                End If
                            Next
                        End If
                    ElseIf objWiresDictionary.Count = 2 Then
                        Dim objTempListA As List(Of WireClass) = objWiresDictionary.Item(0)
                        Dim objTempListB As List(Of WireClass) = objWiresDictionary.Item(1)

                        If AdditionalFunctions.CableInList(objTempListA) <> -1 Then
                            objSingleTerminal.WiresLeftList = objTempListA
                            objSingleTerminal.WiresRigthList = objTempListB
                        Else
                            Dim blnFlagA = objTempListA.Any(Function(x) x.WireNumber.StartsWith("D"))
                            Dim blnFlagB = objTempListB.Any(Function(x) x.WireNumber.StartsWith("S")) OrElse
                            objTempListB.Any(Function(x) x.WireNumber.StartsWith("C")) OrElse
                            objTempListB.Any(Function(x) x.WireNumber.StartsWith("M"))

                            If blnFlagB Or blnFlagA Then
                                objSingleTerminal.WiresLeftList = objTempListA
                                objSingleTerminal.WiresRigthList = objTempListB
                            Else
                                objSingleTerminal.WiresRigthList = objTempListA
                                objSingleTerminal.WiresLeftList = objTempListB
                            End If
                        End If

                    End If
                End If
            End If

        End Sub

        Friend Function FillJumperData(strLocation As String, strTagstrip As String) As List(Of JumperClass)
            Dim dbOleDataTable As DataTable
            Dim sqlQuery As String
            Dim objJumperList As New List(Of JumperClass)

            sqlQuery = String.Format("SELECT Min(CInt([TERM])) AS V1, FIRST([CAT]) AS V2, FIRST([MFG]) AS V3, FIRST([LOC]) AS V4 " &
                        "FROM TERMS " &
                        "WHERE [TAGSTRIP]= '{0}' " &
                            "AND [LOC] = '{1}' " &
                            "AND [JUMPER_ID] <> '' " &
                            "AND [MFG] <> '' " &
                        "GROUP BY [JUMPER_ID]", strTagstrip, strLocation)

            dbOleDataTable = DatabaseConnections.GetOleDataTable(sqlQuery, strConstProjectDatabasePath)

            For Each dbRow In dbOleDataTable.Rows
                Dim objJumper As New JumperClass With {
                    .TagStrip = strTagstrip,
                    .Location = strLocation,
                    .Manufacture = dbRow.item("V3"),
                    .Catalog = dbRow.item("V2"),
                    .StartTermNum = dbRow.item("V1")
                }

                DataAccessObject.FillObjectBlockPath(objJumper)

                If objJumper.Manufacture.ToLower.Equals("phoenix contact") Then
                    objJumper.TermCount = CInt(Replace(Mid(objJumper.Catalog, 5, 2), "-", ""))
                ElseIf objJumper.Manufacture.ToLower.Equals("klemsan") Then
                    objJumper.TermCount = CInt(Mid(objJumper.Catalog, InStr(objJumper.Catalog, "/") + 1, 2))
                End If

                objJumperList.Add(objJumper)
            Next dbRow

            Return objJumperList
        End Function

        Public Function GetWireMarkingList() As List(Of String)
            Dim markingList As New List(Of String)

            Const sqlQuery As String = "SELECT [WIRENO] " &
                                       "FROM [WNETLST] " &
                                       "WHERE [WIRENO] IS NOT NULL AND [WIRENO] <> 'PE' " &
                                       "ORDER BY [WIRENO]"

            Dim objDataTable As Data.DataTable = DatabaseConnections.GetOleDataTable(sqlQuery, ProjectManager.GetInstance().GetActiveProject().GetDbFullPath())

            If Not IsNothing(objDataTable) Then
                For Each objRow In objDataTable.Rows
                    markingList.Add(objRow.Item("WIRENO"))
                Next objRow
            End If

            Return markingList
        End Function

        Friend Function GetAddressMarkingList() As List(Of MarkingClass)
            Dim markingList As New List(Of MarkingClass)
            Dim strConstProjectDatabasePath = ProjectManager.GetInstance().GetActiveProject().GetDbFullPath()

            If Not IO.File.Exists(strConstProjectDatabasePath) Then
                MsgBox("Source file not found. File way: " & strConstProjectDatabasePath & " Please open some project file and repeat.", vbCritical, "File Not Found")
                Throw New ArgumentNullException
            End If


            Const sqlQuery As String = "SELECT [NAM1], [PIN1], [NAM2], [PIN2], [CBL], [TERMDESC1], [TERMDESC2] " &
                                       "FROM WFRM2ALL " &
                                       "WHERE [NAM1] IS NOT NULL AND [PIN1] IS NOT NULL AND [NAM2] IS NOT NULL AND [PIN2] IS NOT NULL " &
                                       "ORDER BY [NAM1], [NAM2]"

            Dim objDataTable As Data.DataTable = DatabaseConnections.GetOleDataTable(sqlQuery, strConstProjectDatabasePath)

            If Not IsNothing(objDataTable) Then
                For Each objRow In objDataTable.Rows
                    Dim pinOne = New PinClass(objRow.Item("NAM1"), objRow.Item("PIN1"))
                    Dim pinTwo = New PinClass(objRow.Item("NAM2"), objRow.Item("PIN2"))
                    If Not IsDBNull(objRow.Item("CBL")) Then
                        pinOne.CableName = objRow.Item("CBL")
                        pinTwo.CableName = objRow.Item("CBL")
                    End If
                    markingList.Add(New MarkingClass(pinOne, pinTwo))
                Next objRow
            End If

            Return markingList
        End Function

    End Module
End Namespace
