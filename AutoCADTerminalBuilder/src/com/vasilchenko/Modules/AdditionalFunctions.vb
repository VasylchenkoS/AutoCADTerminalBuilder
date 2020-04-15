Imports System.Linq
Imports System.Text.RegularExpressions
Imports AutoCADTerminalBuilder.com.vasilchenko.Classes
Imports AutoCADTerminalBuilder.com.vasilchenko.Enums

Namespace com.vasilchenko.Modules
    Module AdditionalFunctions

        Private ReadOnly UncoverObjects As New List(Of String) From {"UT 4-MT", "AVK 2,5/4T Yellow-Green", "USLKG 5"}

        Friend Function GetLastNumericFromString(s As String) As Double

            If s.Equals("") Then Return -1

            Dim rgx As New Regex("-?\d*\.?\d+", RegexOptions.IgnoreCase)
            Dim matches As MatchCollection
            Try
                matches = rgx.Matches(s)
                If matches.Count > 0 Then
                    Return Math.Abs(Double.Parse(matches(matches.Count - 1).Value))
                Else
                    Throw New ArgumentNullException
                End If
            Catch ex As Exception
                MsgBox("Что-то не так: " & vbCrLf & ex.Message)
            End Try
        End Function

        Friend Sub SortCollectionByInstAndCables(objWiresDictionary As TerminalDictionaryClass(Of String, List(Of WireClass)))
            Dim objTempWire As WireClass
            Dim shtFalse As Short = 0

            For Each objConnectionList As List(Of WireClass) In objWiresDictionary.Items
                If objConnectionList.Count = 1 Then
                    For intY As Short = 0 To objConnectionList.Count - 2
                        If objConnectionList.Item(intY).Instance > objConnectionList.Item(intY + 1).Instance And Not (objConnectionList.Item(intY).HasCable OrElse objConnectionList.Item(intY + 1).HasCable) Then
                            objTempWire = objConnectionList(intY)
                            objConnectionList.RemoveAt(intY)
                            objConnectionList.Insert(intY + 1, objTempWire)
                            intY = -1
                        ElseIf objConnectionList.Item(intY).HasCable And Not objConnectionList.Item(intY + 1).HasCable Then
                            objTempWire = objConnectionList(intY)
                            objConnectionList.RemoveAt(intY)
                            objConnectionList.Insert(intY + 1, objTempWire)
                            intY = -1
                        ElseIf objConnectionList.Item(intY + 1).HasCable And (intY + 2) < objConnectionList.Count Then
                            objTempWire = objConnectionList(intY + 1)
                            objConnectionList.RemoveAt(intY + 1)
                            objConnectionList.Insert(intY + 2, objTempWire)
                            intY = -1
                        End If
                        shtFalse += 1
                        If shtFalse = 20 Then
                            MsgBox("В клемме ошибка. Измените подключение " & objConnectionList.Item(intY + 1).Termdesc & ":" & objConnectionList.Item(intY + 1).WireNumber)
                            Exit Sub
                        End If
                    Next
                End If
            Next
        End Sub

        Friend Sub SortDictionaryByInstAndCables(objWiresDictionary As TerminalDictionaryClass(Of String, List(Of WireClass)), strLocation As String)
            Dim objConnectionListF As List(Of WireClass)
            Dim objConnectionListS As List(Of WireClass)
            Dim objTempList As List(Of WireClass)
            Dim blnChange = False


            For lngA As Short = 0 To objWiresDictionary.Count - 2
                objConnectionListF = objWiresDictionary.Item(lngA)
                objConnectionListS = objWiresDictionary.Item(lngA + 1)
                If objConnectionListF.Min(Function(x) x.Instance) = -1 OrElse
                    objConnectionListF.Min(Function(x) x.Instance) > objConnectionListS.Min(Function(x) x.Instance) Then
                    blnChange = True
                End If
                If objConnectionListF.Any(Function(x)
                                              If x.HasCable AndAlso (x.Cable.Destination <> "" And x.Cable.Destination <> strLocation) Then
                                                  Return True
                                              Else : Return False
                                              End If
                                          End Function) Then
                    blnChange = True
                End If
                If blnChange Then
                    objTempList = objWiresDictionary.Item(lngA)
                    objWiresDictionary.RemoveAt(lngA)
                    objWiresDictionary.Add(key:=objTempList.Item(0).WireNumber, value:=objTempList)
                End If
            Next
        End Sub

        Friend Function CableInList(objTempList As List(Of WireClass)) As Integer
            For i As Short = 0 To objTempList.Count - 1
                If objTempList.Item(i).HasCable Then
                    Return i
                    Exit Function
                End If
            Next
            Return -1
        End Function

        Friend Sub AddAccessoriesForSignalisation(objMappingTermsList As List(Of MultilevelTerminalClass))
            If objMappingTermsList.Count = 0 Then Exit Sub
            Dim objCurTerminal As MultilevelTerminalClass = Nothing

            Dim intCount As Integer = objMappingTermsList.Count - 2
            Dim index As Integer = 0

            objMappingTermsList.Insert(index, AddTerminalMarker(objMappingTermsList.Item(index)))
            objMappingTermsList.Insert(index + 1, GetCoverObject(objMappingTermsList.Item(index + 1)))

            index += 2
            intCount += 2

            Do While index <> intCount

                objCurTerminal = objMappingTermsList.Item(index)

                If objCurTerminal.MainTermNumber = 8 Or objCurTerminal.MainTermNumber = 24 Then
                    objMappingTermsList.Insert(index + 1, GetCoverObject(objCurTerminal))
                    index += 1
                    intCount += 1
                Else
                    If objCurTerminal.MainTermNumber Mod 32 = 0 Then
                        objMappingTermsList.Insert(index + 1, GetPlateObject(objCurTerminal))
                        index += 1
                        intCount += 1
                    ElseIf objCurTerminal.MainTermNumber Mod 16 = 0 Then
                        objMappingTermsList.Insert(index + 1, GetCoverObject(objCurTerminal))
                        index += 1
                        intCount += 1
                    End If
                End If
                index += 1
            Loop
            objMappingTermsList.Add(GetCoverObject(objCurTerminal))
            objMappingTermsList.Add(AddEndClamp(objCurTerminal))
        End Sub

        Friend Sub AddAccessoriesForMeasurement(objMappingTermsList As List(Of MultilevelTerminalClass))
            If objMappingTermsList.Count = 0 Then Exit Sub
            Dim objCurTerminal As MultilevelTerminalClass = Nothing
            Dim intCount As Integer = objMappingTermsList.Count - 2
            Dim index As Integer = 0
            Dim shtCount As Short = 0

            objMappingTermsList.Insert(index, AddTerminalMarker(objMappingTermsList.Item(index)))
            objMappingTermsList.Insert(index + 1, GetCoverObject(objMappingTermsList.Item(index + 1)))
            index += 2
            intCount += 2

            Dim msgResult = MsgBox("Модуль 8-миканальный (560AIRxx) ?", MsgBoxStyle.YesNo)
            Select Case msgResult
                Case MsgBoxResult.Yes
                    shtCount = 8
                Case MsgBoxResult.No
                    shtCount = 6
            End Select

            Do While index <> intCount
                objCurTerminal = objMappingTermsList.Item(index)
                If objCurTerminal.MainTermNumber Mod shtCount * 2 = 0 Then
                    objMappingTermsList.Insert(index + 1, GetPlateObject(objCurTerminal))
                    index += 1
                    intCount += 1
                End If
                index += 1
            Loop
            objMappingTermsList.Add(GetCoverObject(objCurTerminal))
            objMappingTermsList.Add(AddEndClamp(objCurTerminal))
        End Sub

        Friend Sub AddAccessoriesForControl(objMappingTermsList As List(Of MultilevelTerminalClass))
            If objMappingTermsList.Count = 0 Then Exit Sub
            Dim objCurTerminal As MultilevelTerminalClass = Nothing
            Dim intCount As Integer = objMappingTermsList.Count - 2
            Dim index As Integer = 0
            Dim shtCount As Short = 0

            Try
                shtCount = CInt(InputBox("Сколько проводов в схеме ТУ?", "ControlCount", "5", 100, 100))
            Catch
                MsgBox("Нужно было вводить целое число) Установлено по-умолчанию (5)")
                shtCount = 5
            End Try

            objMappingTermsList.Insert(index, AddTerminalMarker(objMappingTermsList.Item(index)))
            objMappingTermsList.Insert(index + 1, GetCoverObject(objMappingTermsList.Item(index + 1)))
            index += 2
            intCount += 2

            Do While index <> intCount
                objCurTerminal = objMappingTermsList.Item(index)
                If objCurTerminal.MainTermNumber Mod shtCount = 0 Then
                    objMappingTermsList.Insert(index + 1, GetPlateObject(objCurTerminal))
                    index += 1
                    intCount += 1
                End If
                index += 1
            Loop
            objMappingTermsList.Add(GetCoverObject(objCurTerminal))
            objMappingTermsList.Add(AddEndClamp(objCurTerminal))
        End Sub

        Friend Sub AddAccessoriesForPower(objMappingTermsList As List(Of MultilevelTerminalClass))
            If objMappingTermsList.Count = 0 Then Exit Sub
            Dim objCurTerminal As MultilevelTerminalClass = Nothing
            Dim intCount As Integer = objMappingTermsList.Count
            Dim index = 0

            Dim intCurWire As Integer
            Dim intPrevWire As Integer

            Do While index <> intCount
                objCurTerminal = objMappingTermsList.Item(index)

                If objCurTerminal.Terminal.Values.Any(Function(x As SingleLevelTerminalClass) x.WireContains(New List(Of String)() From {"L", "PP", "PV", "+"})) Then
                    intCurWire = 1
                ElseIf objCurTerminal.Terminal.Values.Any(Function(x As SingleLevelTerminalClass) x.WireContains(New List(Of String)() From {"N", "PM", "PT", "-"})) Then
                    intCurWire = 2
                ElseIf objCurTerminal.Terminal.Values.Any(Function(x As SingleLevelTerminalClass) x.WireContains("PE")) Then
                    intCurWire = 3
                Else
                    intCurWire = 4
                End If

                If index = 0 Then
                    objMappingTermsList.Insert(index, AddTerminalMarker(objMappingTermsList.Item(index)))
                    objMappingTermsList.Insert(index + 1, GetCoverObject(objMappingTermsList.Item(index + 1)))
                    index += 2
                    intCount += 2
                ElseIf _
                    (objCurTerminal.MainTermNumber = 2 Or objCurTerminal.MainTermNumber = 4) And
                    objCurTerminal.Catalog = "UT 6-HESI (6,3X32)" Then
                    objMappingTermsList.Insert(index + 1, GetPlateObject(objCurTerminal))
                    index += 1
                    intCount += 1
                ElseIf objCurTerminal.Catalog <> "UT 6-HESI (6,3X32)" Then
                    'If objPrevTerminal.Terminal.Values.Any(Function(x As SingleLevelTerminalClass) x.WireContains(New List(Of String)() From {"L", "PP", "PV", "+"})) And
                    '    objCurTerminal.Terminal.Values.Any(Function(x As SingleLevelTerminalClass) x.WireContains(New List(Of String)() From {"N", "PM", "PT", "-"})) Then
                    If intPrevWire = 1 And intCurWire = 2 Then
                        objMappingTermsList.Insert(index, GetCoverObject(objCurTerminal))
                        index += 1
                        intCount += 1
                        'ElseIf objCurTerminal.Terminal.Values.Any(Function(x As SingleLevelTerminalClass) x.WireContains(New List(Of String)() From {"L", "PP", "PV", "+"})) And
                        '    objPrevTerminal.Terminal.Values.Any(Function(x As SingleLevelTerminalClass) x.WireContains(New List(Of String)() From {"N", "PM", "PT", "-"})) Then
                    ElseIf intCurWire = 1 And intPrevWire = 2 OrElse intCurWire = 3 And intPrevWire <> 3 Then
                        objMappingTermsList.Insert(index, GetPlateObject(objCurTerminal))
                        index += 1
                        intCount += 1
                    End If
                End If
                index += 1
                If Not intCurWire = 4 Then intPrevWire = intCurWire
            Loop
            If Not UncoverObjects.Contains(objCurTerminal.Catalog) Then _
                objMappingTermsList.Add(GetCoverObject(objCurTerminal))
            objMappingTermsList.Add(AddEndClamp(objCurTerminal))
        End Sub

        Public Sub AddAccessoriesForMetering(objMappingTermsList As List(Of MultilevelTerminalClass))
            If objMappingTermsList.Count = 0 Then Exit Sub
            Dim objCurTerminal As MultilevelTerminalClass = Nothing
            Dim blnPrevWirenum = False
            Dim blnCurWirenum As Boolean
            Dim index = 0
            Dim blnIsTt = False

            Dim msgResult = MsgBox("Клеммник цепей ТТ?", MsgBoxStyle.YesNo)
            Select Case msgResult
                Case MsgBoxResult.Yes
                    blnIsTt = True
                Case MsgBoxResult.No
                    blnIsTt = False
            End Select

            Do While index <> objMappingTermsList.Count
                objCurTerminal = objMappingTermsList.Item(index)
                blnCurWirenum = objCurTerminal.Terminal.Values.Any(Function(x As SingleLevelTerminalClass) x.WireNameStartsWith("A"))
                If objCurTerminal.MainTermNumber = 1 Then
                    objMappingTermsList.Insert(index, AddTerminalMarker(objCurTerminal))
                    If Not objCurTerminal.Catalog.ToLower.StartsWith("wgo 4") Then
                        objMappingTermsList.Insert(index + 1, GetCoverObject(objCurTerminal))
                        index += 1
                    End If
                    index += 1
                ElseIf blnCurWirenum And blnIsTt Then
                    If Not objCurTerminal.Catalog.ToLower.StartsWith("wgo 4") Then
                        objMappingTermsList.Insert(index, GetCoverObject(objCurTerminal))
                        objMappingTermsList.Insert(index + 1, AddTerminalMarker(objCurTerminal))
                        objMappingTermsList.Insert(index + 2, GetCoverObject(objCurTerminal))
                        index += 3
                    Else
                        objMappingTermsList.Insert(index, AddTerminalMarker(objCurTerminal))
                        index += 1
                    End If
                ElseIf blnPrevWirenum And blnCurWirenum And Not blnIsTt Then
                    If Not objCurTerminal.Catalog.ToLower.StartsWith("wgo 4") Then
                        objMappingTermsList.Insert(index, GetCoverObject(objCurTerminal))
                        index += 1
                    End If
                    blnPrevWirenum = False
                End If
                index += 1

                If _
                    objCurTerminal.Terminal.Values(0).WiresLeftList.Count <> 0 Or
                    objCurTerminal.Terminal.Values(0).WiresRigthList.Count <> 0 Then
                    blnPrevWirenum = objCurTerminal.Terminal.Values(0).WireNameStartsWith("N") Or
                                     objCurTerminal.Terminal.Values(0).WireNameStartsWith("O")
                End If
            Loop
            If Not objCurTerminal.Catalog.ToLower.StartsWith("wgo 4") Then _
                objMappingTermsList.Add(GetCoverObject(objCurTerminal))
            objMappingTermsList.Add(AddEndClamp(objCurTerminal))
        End Sub

        Friend Sub AddAccessoriesForTMRZA(objMappingTermsList As List(Of MultilevelTerminalClass))
            If objMappingTermsList.Count = 0 Then Exit Sub
            Dim objCurTerminal As MultilevelTerminalClass = Nothing
            Dim objPrevTerminal As MultilevelTerminalClass = Nothing
            Dim intCount As Integer = objMappingTermsList.Count
            Dim index = 0

            Dim intCurWire As Integer
            Dim intPrevWire As Integer

            Do While index <> intCount
                objCurTerminal = objMappingTermsList.Item(index)

                If objCurTerminal.Terminal.Values.Any(Function(x As SingleLevelTerminalClass) x.WireRightStartWith(New List(Of String)() From {"ТК", "TK", "ТА", "TA"})) Then
                    intCurWire = 1
                ElseIf objCurTerminal.Terminal.Values.Any(Function(x As SingleLevelTerminalClass) x.WireRightStartWith(New List(Of String)() From {"ТС", "TC", "РР", "PP"})) Then
                    intCurWire = 2
                ElseIf objCurTerminal.Terminal.Values.Any(Function(x As SingleLevelTerminalClass) x.WireRightStartWith(New List(Of String)() From {"ТВ", "TB", "GND", "PM"})) Then
                    intCurWire = 3
                Else
                    intCurWire = 4
                End If

                If index = 0 Then
                    objMappingTermsList.Insert(index, AddTerminalMarker(objMappingTermsList.Item(index)))
                    objMappingTermsList.Insert(index + 1, GetCoverObject(objMappingTermsList.Item(index + 1)))
                    index += 2
                    intCount += 2
                ElseIf objCurTerminal.Catalog.Equals("URTK 6") And Not objPrevTerminal.Catalog.Equals("URTK 6") Then
                    objMappingTermsList.Insert(index, GetPlateObject(objCurTerminal))
                    index += 1
                    intCount += 1
                ElseIf objCurTerminal.Catalog.Equals("URTK 6") And objPrevTerminal.Catalog.Equals("URTK 6") Then
                    objMappingTermsList.Insert(index + 1, GetCoverObject(objCurTerminal))
                    index += 1
                    intCount += 1
                ElseIf (intCurWire = 1 And intPrevWire = 2) OrElse (intCurWire = 1 And intPrevWire = 3) OrElse (intCurWire = 2 And intPrevWire = 3) Then
                    objMappingTermsList.Insert(index, GetCoverObject(objCurTerminal))
                    index += 1
                    intCount += 1
                End If
                index += 1
                If Not intCurWire = 4 Then intPrevWire = intCurWire
                objPrevTerminal = objCurTerminal
            Loop
            If Not UncoverObjects.Contains(objCurTerminal.Catalog) Then _
                objMappingTermsList.Add(GetCoverObject(objCurTerminal))
            objMappingTermsList.Add(AddEndClamp(objCurTerminal))
        End Sub

        Friend Sub AddAccessoriesForRZA(objMappingTermsList As List(Of MultilevelTerminalClass))
            If objMappingTermsList.Count = 0 Then Exit Sub
            Dim objCurTerminal As MultilevelTerminalClass = Nothing
            Dim intCount As Integer = objMappingTermsList.Count
            objMappingTermsList.Insert(0, AddTerminalMarker(objMappingTermsList.Item(0)))
        End Sub

        Private Function GetCoverObject(objCurTerminal As MultilevelTerminalClass) As MultilevelTerminalClass
            Select Case objCurTerminal.Catalog
                Case "UT 2,5", "UT 2,5-PE", "UT 4", "UT 4-PE", "UT 6", "UT 6-PE"
                    Return EnumFunctions.Convert(TerminalAccessoriesEnum.PC_EndCoverForUT2_5, objCurTerminal)
                Case "UT 16"
                    Return EnumFunctions.Convert(TerminalAccessoriesEnum.PC_EndCoverForUT16, objCurTerminal)
                Case "UT 2,5-MT", "UT 4-MTD-DIO/R-L"
                    Return EnumFunctions.Convert(TerminalAccessoriesEnum.PC_EndCoverForUT2_5MT, objCurTerminal)
                Case "UTTB 2,5", "UTTB 2,5-PV", "UTTB 2,5-MT-P/P"
                    Return EnumFunctions.Convert(TerminalAccessoriesEnum.PC_EndCoverForUTTB2_5, objCurTerminal)
                Case "UT 2,5-QUATTRO"
                    Return EnumFunctions.Convert(TerminalAccessoriesEnum.PC_EndCoverForUT2_5QUATTRO, objCurTerminal)
                Case "URTK 6"
                    Return EnumFunctions.Convert(TerminalAccessoriesEnum.PC_EndCoverForURTK_6, objCurTerminal)
                Case "UT 6-HESI (6,3X32)"
                    Return EnumFunctions.Convert(TerminalAccessoriesEnum.PC_PartitionPlateForUT6, objCurTerminal)
                Case "ST 2,5"
                    Return EnumFunctions.Convert(TerminalAccessoriesEnum.PC_EndCoverForST2_5, objCurTerminal)
                Case "STTB 2,5-L/N", "STTB 2,5-PE"
                    Return EnumFunctions.Convert(TerminalAccessoriesEnum.PC_EndCoverForSTTB2_5, objCurTerminal)
                Case "UKK 5"
                    Return EnumFunctions.Convert(TerminalAccessoriesEnum.PC_EndCoverForUKK_5, objCurTerminal)
                Case "AVK 4 A Gray", "AVK 2,5 A Gray", "AVK 2,5 R Gray"
                    Return EnumFunctions.Convert(TerminalAccessoriesEnum.K_EndCoverForAVK2_5_R_AVK_4_A, objCurTerminal)
                Case "AVK 2,5 Gray", "AVK 4 Gray", "AVK 6 Gray", "AVK 10 Gray"
                    Return EnumFunctions.Convert(TerminalAccessoriesEnum.K_EndCoverForForAVK2_5_10, objCurTerminal)
                Case "WDU 2.5", "WPE 2.5"
                    Return EnumFunctions.Convert(TerminalAccessoriesEnum.K_EndCoverForForAVK2_5_10, objCurTerminal)
                Case "280-601", "280-607"
                    Return EnumFunctions.Convert(TerminalAccessoriesEnum.WAGO_EndCoverFor280_601_7, objCurTerminal)
                Case "280-833"
                    Return EnumFunctions.Convert(TerminalAccessoriesEnum.WAGO_EndCoverFor280_833, objCurTerminal)
                Case "UT 2,5", "UT 2,5-PE", "UT 4", "UT 6", "UT 6-PE"
                    Return EnumFunctions.Convert(TerminalAccessoriesEnum.PC_EndCoverForUT2_5, objCurTerminal)
                Case "UK 5 N", "UK 5 N BU", "UK 5 N-TG"
                    Return EnumFunctions.Convert(TerminalAccessoriesEnum.PC_EndCoverForUK_5_N, objCurTerminal)
                Case "UK 2,5 N"
                    Return EnumFunctions.Convert(TerminalAccessoriesEnum.PC_EndCoverForUK_2_5_N, objCurTerminal)
                Case Else
                    Return Nothing
            End Select
        End Function

        Private Function GetPlateObject(objCurTerminal As MultilevelTerminalClass) As MultilevelTerminalClass

            If objCurTerminal.Manufacture = "Klemsan" Then Return GetCoverObject(objCurTerminal)

            Select Case objCurTerminal.Catalog
                Case "UT 2,5", "UT 2,5-PE", "UT 4", "UT 4-PE", "UT 6", "UT 6-PE"
                    Return EnumFunctions.Convert(TerminalAccessoriesEnum.PC_PartitionPlateForUT2_5, objCurTerminal)
                Case "UT 16"
                    Return EnumFunctions.Convert(TerminalAccessoriesEnum.PC_PartitionPlateForUT16, objCurTerminal)
                Case "UT 2,5-MT", "UT 4-MTD-DIO/R-L"
                    Return EnumFunctions.Convert(TerminalAccessoriesEnum.PC_PartitionPlateForUT2_5MT, objCurTerminal)
                Case "UTTB 2,5", "UTTB 2,5-PV", "UTTB 2,5-MT-P/P"
                    Return EnumFunctions.Convert(TerminalAccessoriesEnum.PC_PartitionPlateForUTTB2_5, objCurTerminal)
                Case "UT 2,5-QUATTRO"
                    Return EnumFunctions.Convert(TerminalAccessoriesEnum.PC_PartitionPlateForUT2_5QUATTRO, objCurTerminal)
                Case "UT 6-HESI (6,3X32)"
                    Return EnumFunctions.Convert(TerminalAccessoriesEnum.PC_PartitionPlateForUT6, objCurTerminal)
                Case "URTK 6"
                    Return EnumFunctions.Convert(TerminalAccessoriesEnum.PC_PartitionPlateForURTK6, objCurTerminal)
                Case "ST 2,5"
                    Return EnumFunctions.Convert(TerminalAccessoriesEnum.PC_PartitionPlateForST4, objCurTerminal)
                Case "STTB 2,5-L/N", "STTB 2,5-PE"
                    Return EnumFunctions.Convert(TerminalAccessoriesEnum.PC_EndCoverForSTTB2_5, objCurTerminal)
                Case "UKK 5"
                    Return EnumFunctions.Convert(TerminalAccessoriesEnum.PC_EndCoverForUKK_5, objCurTerminal)
                Case "WDU 2.5", "WPE 2.5"
                    Return EnumFunctions.Convert(TerminalAccessoriesEnum.WM_EndCoverForForWDU2_5, objCurTerminal)
                Case "280-601", "280-607"
                    Return EnumFunctions.Convert(TerminalAccessoriesEnum.WAGO_EndCoverFor280_601_7, objCurTerminal)
                Case "280-833"
                    Return EnumFunctions.Convert(TerminalAccessoriesEnum.WAGO_PartitionPlateFor280_833, objCurTerminal)
                Case "UK 5 N", "UK 5 N BU", "UK 5 N-TG"
                    Return EnumFunctions.Convert(TerminalAccessoriesEnum.PC_EndCoverForUK_5_N, objCurTerminal)
                Case "UK 2,5 N"
                    Return EnumFunctions.Convert(TerminalAccessoriesEnum.PC_EndCoverForUK_2_5_N, objCurTerminal)
                Case Else
                    Return Nothing
            End Select
        End Function

        Private Function AddEndClamp(objCurTerminal As MultilevelTerminalClass) As MultilevelTerminalClass
            If objCurTerminal.Manufacture.ToLower.Equals("phoenix contact") Then
                Return EnumFunctions.Convert(TerminalAccessoriesEnum.PC_EndClamp35, objCurTerminal)
            ElseIf objCurTerminal.Manufacture.ToLower.Equals("klemsan") Then
                Return EnumFunctions.Convert(TerminalAccessoriesEnum.K_EndClamp, objCurTerminal)
            ElseIf objCurTerminal.Manufacture.ToLower.Equals("weidmuller") Then
                Return EnumFunctions.Convert(TerminalAccessoriesEnum.WM_EndClamp, objCurTerminal)
            ElseIf objCurTerminal.Manufacture.ToLower.Equals("wago") Then
                Return EnumFunctions.Convert(TerminalAccessoriesEnum.WAGO_EndClamp, objCurTerminal)
            End If
            Return Nothing
        End Function

        Private Function AddTerminalMarker(objCurTerminal As MultilevelTerminalClass) As MultilevelTerminalClass
            If objCurTerminal.Manufacture.ToLower.Equals("phoenix contact") Then
                Select Case objCurTerminal.Catalog
                    Case "UK 5 N", "UK 5 N BU", "UK 5 N-TG", "UK 2,5 N", "UKK 5"
                        Return EnumFunctions.Convert(TerminalAccessoriesEnum.PC_TerminalMarker2, objCurTerminal)
                    Case Else
                        Return EnumFunctions.Convert(TerminalAccessoriesEnum.PC_TerminalMarker, objCurTerminal)
                End Select
            ElseIf objCurTerminal.Manufacture.ToLower.Equals("klemsan") Then
                Return EnumFunctions.Convert(TerminalAccessoriesEnum.K_TerminalMarker, objCurTerminal)
            ElseIf objCurTerminal.Manufacture.ToLower.Equals("weidmuller") Then
                Return EnumFunctions.Convert(TerminalAccessoriesEnum.WM_TerminalMarker, objCurTerminal)
            ElseIf objCurTerminal.Manufacture.ToLower.Equals("wago") Then
                Return EnumFunctions.Convert(TerminalAccessoriesEnum.WAGO_TerminalMarker, objCurTerminal)
            ElseIf objCurTerminal.Catalog.Contains("IGNORED") Then
                Return EnumFunctions.Convert(TerminalAccessoriesEnum.IGNORED_TerminalMarker, objCurTerminal)
            End If
            Return Nothing
        End Function

    End Module
End Namespace