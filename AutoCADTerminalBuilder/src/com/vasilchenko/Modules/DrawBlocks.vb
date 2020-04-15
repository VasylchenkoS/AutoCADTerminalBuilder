Imports AutoCADTerminalBuilder.com.vasilchenko.Classes
Imports Autodesk.AutoCAD.DatabaseServices
Imports Autodesk.AutoCAD.Geometry
Imports System.Linq

Namespace com.vasilchenko.Modules
    Module DrawBlocks

        Public Sub DrawTerminalBlock(acDatabase As Autodesk.AutoCAD.DatabaseServices.Database, acTransaction As Transaction, objInputTerminal As MultilevelTerminalClass, objSingleTerminal As KeyValuePair(Of String, SingleLevelTerminalClass),
            acInsertPt As Point3d, dblScale As Double, blnDetailsTerminal As Boolean, blnNoCable As Boolean)
            Dim acBlockTable As BlockTable = Nothing
            Dim acInsObjectId As ObjectId = Nothing

            If blnDetailsTerminal Then
                DetailBlockExists(acDatabase, acTransaction, objInputTerminal, acBlockTable, acInsObjectId)
            Else
                SimpleBlockExists(acDatabase, acTransaction, objInputTerminal, acBlockTable, acInsObjectId, objSingleTerminal.Key)
            End If

            Using acBlkRef As New BlockReference(acInsertPt, acInsObjectId)
                'Dim acBlkRef As New BlockReference(acInsertPt, acInsObjectId)
                acBlkRef.Layer = "PSYMS"
                acBlkRef.ScaleFactors = New Scale3d(dblScale)

                Dim acBlockTableRecord As BlockTableRecord = acTransaction.GetObject(acBlockTable.Item(BlockTableRecord.ModelSpace), OpenMode.ForWrite)

                acBlockTableRecord.AppendEntity(acBlkRef)
                acTransaction.AddNewlyCreatedDBObject(acBlkRef, True)

                Dim acBlockTableAttrRec As BlockTableRecord = acTransaction.GetObject(acInsObjectId, OpenMode.ForRead)
                Dim acAttrObjectId As ObjectId

                For Each acAttrObjectId In acBlockTableAttrRec
                    Dim acAttrEntity As Entity = acTransaction.GetObject(acAttrObjectId, OpenMode.ForRead)
                    Dim acAttrDefinition As AttributeDefinition = TryCast(acAttrEntity, AttributeDefinition)
                    If (acAttrDefinition IsNot Nothing) Then
                        Dim acAttrReference As New AttributeReference()
                        Dim strTermdesc As String = ""
                        acAttrReference.SetAttributeFromBlock(acAttrDefinition, acBlkRef.BlockTransform)
                        Select Case acAttrReference.Tag
                            Case "WIDTH"
                                objInputTerminal.Width = CDbl(acAttrReference.TextString)
                            Case "HEIGHT"
                                objInputTerminal.Height = CDbl(acAttrReference.TextString)
                        End Select
                    End If
                Next

                For Each acAttrObjectId In acBlockTableAttrRec
                    Dim acAttrEntity As Entity = acTransaction.GetObject(acAttrObjectId, OpenMode.ForRead)
                    Dim acAttrDefinition = TryCast(acAttrEntity, AttributeDefinition)
                    If (acAttrDefinition IsNot Nothing) Then
                        Using acAttrReference As New AttributeReference()
                            Dim strTermdesc As String = ""
                            acAttrReference.SetAttributeFromBlock(acAttrDefinition, acBlkRef.BlockTransform)
                            Select Case acAttrReference.Tag
                                Case "P_TAGSTRIP"
                                    acAttrReference.TextString = objInputTerminal.TagStrip
                                    acAttrReference.TextStyleId = CType(acTransaction.GetObject(acDatabase.TextStyleTableId, OpenMode.ForRead), TextStyleTable)("WD_IEC")
                                    acAttrReference.Layer = "PTAG"
                                Case "INST"
                                    If objSingleTerminal.Key = "01" Then _
                                    acAttrReference.TextString = objInputTerminal.TextInstance
                                    acAttrReference.TextStyleId = CType(acTransaction.GetObject(acDatabase.TextStyleTableId, OpenMode.ForRead), TextStyleTable)("WD_IEC")
                                    acAttrReference.Layer = "PLOC"
                                Case "LOC"
                                    If objSingleTerminal.Key = "01" Then _
                                    acAttrReference.TextString = objInputTerminal.Location
                                    acAttrReference.TextStyleId = CType(acTransaction.GetObject(acDatabase.TextStyleTableId, OpenMode.ForRead), TextStyleTable)("WD_IEC")
                                    acAttrReference.Layer = "PLOC"
                                Case "MFG"
                                    If objSingleTerminal.Key = "01" Then _
                                    acAttrReference.TextString = objInputTerminal.Manufacture
                                    acAttrReference.TextStyleId = CType(acTransaction.GetObject(acDatabase.TextStyleTableId, OpenMode.ForRead), TextStyleTable)("WD_IEC")
                                    acAttrReference.Layer = "PMFG"
                                Case "CAT"
                                    If objSingleTerminal.Key = "01" Then _
                                    acAttrReference.TextString = objInputTerminal.Catalog
                                    acAttrReference.TextStyleId = CType(acTransaction.GetObject(acDatabase.TextStyleTableId, OpenMode.ForRead), TextStyleTable)("WD_IEC")
                                    acAttrReference.Layer = "PCAT"
                                Case "TERM"
                                    acAttrReference.TextString = objSingleTerminal.Value.TerminalNumber
                                    acAttrReference.TextStyleId = CType(acTransaction.GetObject(acDatabase.TextStyleTableId, OpenMode.ForRead), TextStyleTable)("WD")
                                    acAttrReference.Layer = "PTERM"
                                Case "DESC3"
                                    If objSingleTerminal.Key = "01" Then _
                                    acAttrReference.TextString = objInputTerminal.Notes
                                    acAttrReference.TextStyleId = CType(acTransaction.GetObject(acDatabase.TextStyleTableId, OpenMode.ForRead), TextStyleTable)("WD_IEC")
                                    acAttrReference.Layer = "PTAG"
                                Case "WIRENOL"
                                    If objSingleTerminal.Value.WiresLeftList.Count <> 0 AndAlso
                                        objSingleTerminal.Value.WiresLeftList.Item(0).WireNumber.ToLower <> "pe" AndAlso
                                        Not (objSingleTerminal.Value.WiresLeftList.Item(0).HasCable And blnNoCable) Then
                                        acAttrReference.TextString = objSingleTerminal.Value.WiresLeftList.Item(0).WireNumber
                                    End If
                                    acAttrReference.TextStyleId = CType(acTransaction.GetObject(acDatabase.TextStyleTableId, OpenMode.ForRead), TextStyleTable)("WD_IEC")
                                    acAttrReference.Layer = "PWIRE"
                                Case "WIRENOR"
                                    If objSingleTerminal.Value.WiresRigthList.Count <> 0 AndAlso
                                        objSingleTerminal.Value.WiresRigthList.Item(0).WireNumber.ToLower <> "pe" AndAlso
                                        Not (objSingleTerminal.Value.WiresRigthList.Item(0).HasCable And blnNoCable) Then
                                        acAttrReference.TextString = objSingleTerminal.Value.WiresRigthList.Item(0).WireNumber
                                    End If
                                    acAttrReference.TextStyleId = CType(acTransaction.GetObject(acDatabase.TextStyleTableId, OpenMode.ForRead), TextStyleTable)("WD_IEC")
                                    acAttrReference.Layer = "PWIRE"
                                Case "TERMDESCL"
                                    Dim isCable = False
                                    If objSingleTerminal.Value.WiresLeftList.Count <> 0 AndAlso
                                        Not (objSingleTerminal.Value.WiresLeftList.Item(0).HasCable And blnNoCable) Then
                                        acAttrReference.TextString = SetTermdesc(objSingleTerminal.Value.WiresLeftList, isCable)

                                        If blnDetailsTerminal Then
                                            If objInputTerminal.Catalog.StartsWith("UT ") OrElse
                                               objInputTerminal.Catalog.StartsWith("ST ") OrElse
                                               objInputTerminal.Catalog.StartsWith("UT 6-HESI") OrElse
                                               objInputTerminal.Catalog.StartsWith("AVK") OrElse
                                               objInputTerminal.Catalog.StartsWith("WDU 2.5") OrElse
                                               objInputTerminal.Catalog.StartsWith("WPE 2.5") OrElse
                                               objInputTerminal.Catalog.StartsWith("WGO") OrElse
                                               objInputTerminal.Catalog.StartsWith("280") OrElse
                                               objInputTerminal.Catalog.StartsWith("IGNORED") Then
                                                Dim x = acInsertPt.X - (objInputTerminal.Width / 2) * dblScale
                                                Dim y = acInsertPt.Y - (objInputTerminal.Height / 2) * dblScale
                                                DrawLine(acDatabase, acTransaction, New Point3d(x, y, 0), -20, isCable, dblScale)
                                            ElseIf objInputTerminal.Catalog.StartsWith("UTTB 2,5") OrElse
                                                objInputTerminal.Catalog.StartsWith("STTB 2,5") Then
                                                Dim x = acInsertPt.X - (35) * dblScale
                                                Dim y = acInsertPt.Y - (6) * dblScale
                                                DrawLine(acDatabase, acTransaction, New Point3d(x, y, 0), -10, isCable, dblScale)
                                            ElseIf objInputTerminal.Catalog.Equals("URTK 6") Then
                                                Dim x = acInsertPt.X - (objInputTerminal.Width / 2) * dblScale
                                                Dim y = acInsertPt.Y - (objInputTerminal.Height / 2) * dblScale
                                                DrawLine(acDatabase, acTransaction, New Point3d(x, y, 0), -26, isCable, dblScale)
                                            End If
                                        End If
                                    End If

                                    acAttrReference.TextStyleId = CType(acTransaction.GetObject(acDatabase.TextStyleTableId, OpenMode.ForRead), TextStyleTable)("WD")
                                    acAttrReference.Layer = "PDESC"
                                Case "TERMDESCR"
                                    Dim isCable = False
                                    If objSingleTerminal.Value.WiresRigthList.Count <> 0 AndAlso
                                        Not (objSingleTerminal.Value.WiresRigthList.Item(0).HasCable And blnNoCable) Then
                                        acAttrReference.TextString = SetTermdesc(objSingleTerminal.Value.WiresRigthList, isCable)

                                        If blnDetailsTerminal Then
                                            If objInputTerminal.Catalog.StartsWith("UT ") OrElse
                                               objInputTerminal.Catalog.StartsWith("ST ") OrElse
                                               objInputTerminal.Catalog.StartsWith("UT 6-HESI") OrElse
                                               objInputTerminal.Catalog.StartsWith("AVK") OrElse
                                               objInputTerminal.Catalog.StartsWith("WDU 2.5") OrElse
                                               objInputTerminal.Catalog.StartsWith("WPE 2.5") OrElse
                                               objInputTerminal.Catalog.StartsWith("WGO") OrElse
                                               objInputTerminal.Catalog.StartsWith("280") OrElse
                                               objInputTerminal.Catalog.StartsWith("IGNORED") Then
                                                Dim x = acInsertPt.X + (objInputTerminal.Width / 2) * dblScale
                                                Dim y = acInsertPt.Y - (objInputTerminal.Height / 2) * dblScale
                                                DrawLine(acDatabase, acTransaction, New Point3d(x, y, 0), 20, isCable, dblScale)
                                            ElseIf objInputTerminal.Catalog.StartsWith("UTTB 2,5") OrElse
                                                   objInputTerminal.Catalog.StartsWith("STTB 2,5") Then
                                                Dim x = acInsertPt.X + (35) * dblScale
                                                Dim y = acInsertPt.Y - (6) * dblScale
                                                DrawLine(acDatabase, acTransaction, New Point3d(x, y, 0), 10, isCable, dblScale)
                                            ElseIf objInputTerminal.Catalog.Equals("URTK 6") Then
                                                Dim x = acInsertPt.X + (objInputTerminal.Width / 2) * dblScale
                                                Dim y = acInsertPt.Y - (objInputTerminal.Height / 2) * dblScale
                                                DrawLine(acDatabase, acTransaction, New Point3d(x, y, 0), 26, isCable, dblScale)
                                            End If
                                        End If
                                    End If

                                    acAttrReference.TextStyleId = CType(acTransaction.GetObject(acDatabase.TextStyleTableId, OpenMode.ForRead), TextStyleTable)("WD")
                                    acAttrReference.Layer = "PDESC"
                                Case "WIRENOTL"
                                    If objInputTerminal.Terminal.Values(1).WiresLeftList.Count <> 0 AndAlso
                                        objInputTerminal.Terminal.Values(1).WiresLeftList.Item(0).WireNumber.ToLower <> "pe" Then
                                        acAttrReference.TextString = objInputTerminal.Terminal.Values(1).WiresLeftList.Item(0).WireNumber
                                    End If
                                    acAttrReference.TextStyleId = CType(acTransaction.GetObject(acDatabase.TextStyleTableId, OpenMode.ForRead), TextStyleTable)("WD_IEC")
                                    acAttrReference.Layer = "PWIRE"
                                Case "WIRENOTR"
                                    If objInputTerminal.Terminal.Values(1).WiresRigthList.Count <> 0 AndAlso
                                        objInputTerminal.Terminal.Values(1).WiresRigthList.Item(0).WireNumber.ToLower <> "pe" Then
                                        acAttrReference.TextString = objInputTerminal.Terminal.Values(1).WiresRigthList.Item(0).WireNumber
                                    End If
                                    acAttrReference.TextStyleId = CType(acTransaction.GetObject(acDatabase.TextStyleTableId, OpenMode.ForRead), TextStyleTable)("WD_IEC")
                                    acAttrReference.Layer = "PWIRE"
                                Case "TERMDESCTL"
                                    Dim isCable = False
                                    If objInputTerminal.Terminal.Values(1).WiresLeftList.Count <> 0 Then
                                        If objInputTerminal.Terminal.Values(1).WiresLeftList.Any(Function(x As WireClass) x.HasCable) Then
                                            strTermdesc = "в " & objInputTerminal.Terminal.Values(1).WiresLeftList.Find(Function(x As WireClass) x.HasCable).Cable.Mark
                                            isCable = True
                                        Else
                                            For lngA = 0 To objInputTerminal.Terminal.Values(1).WiresLeftList.Count - 1
                                                If objInputTerminal.Terminal.Values(1).WiresLeftList.Item(lngA).Termdesc <> "" Then
                                                    If strTermdesc = "" Then
                                                        strTermdesc = "к " & objInputTerminal.Terminal.Values(1).WiresLeftList.Item(lngA).Termdesc & ", "
                                                    Else
                                                        strTermdesc = strTermdesc & objInputTerminal.Terminal.Values(1).WiresLeftList.Item(lngA).Termdesc & ", "
                                                    End If
                                                End If
                                            Next
                                            If strTermdesc <> "" Then strTermdesc = strTermdesc.Remove(strTermdesc.Length - 2)
                                        End If
                                        acAttrReference.TextString = strTermdesc
                                        If objInputTerminal.Catalog.StartsWith("UTTB 2,5") OrElse
                                           objInputTerminal.Catalog.StartsWith("STTB 2,5") Then
                                            Dim x = acInsertPt.X - (20 * dblScale)
                                            Dim y = acInsertPt.Y - (3.5 * dblScale)
                                            DrawLine(acDatabase, acTransaction, New Point3d(x, y, 0), -25, isCable, dblScale)
                                        End If
                                    End If
                                    strTermdesc = ""
                                    acAttrReference.TextStyleId = CType(acTransaction.GetObject(acDatabase.TextStyleTableId, OpenMode.ForRead), TextStyleTable)("WD")
                                    acAttrReference.Layer = "PDESC"
                                Case "TERMDESCTR"
                                    Dim isCable = False
                                    If objInputTerminal.Terminal.Values(1).WiresRigthList.Count <> 0 Then
                                        If objInputTerminal.Terminal.Values(1).WiresRigthList.Any(Function(x As WireClass) x.HasCable) Then
                                            strTermdesc = "в " & objInputTerminal.Terminal.Values(1).WiresRigthList.Find(Function(x As WireClass) x.HasCable).Cable.Mark
                                            isCable = True
                                        Else
                                            For lngA = 0 To objInputTerminal.Terminal.Values(1).WiresRigthList.Count - 1
                                                If objInputTerminal.Terminal.Values(1).WiresRigthList.Item(lngA).Termdesc <> "" Then
                                                    If strTermdesc = "" Then
                                                        strTermdesc = "к " & objInputTerminal.Terminal.Values(1).WiresRigthList.Item(lngA).Termdesc & ", "
                                                    Else
                                                        strTermdesc = strTermdesc & objInputTerminal.Terminal.Values(1).WiresRigthList.Item(lngA).Termdesc & ", "
                                                    End If
                                                End If
                                            Next
                                            If strTermdesc <> "" Then strTermdesc = strTermdesc.Remove(strTermdesc.Length - 2)
                                        End If
                                        acAttrReference.TextString = strTermdesc
                                        If objInputTerminal.Catalog.StartsWith("UTTB 2,5") OrElse
                                           objInputTerminal.Catalog.StartsWith("STTB 2,5") Then
                                            Dim x = acInsertPt.X + (10 * dblScale)
                                            Dim y = acInsertPt.Y - (3.5 * dblScale)
                                            DrawLine(acDatabase, acTransaction, New Point3d(x, y, 0), 35, isCable, dblScale)
                                        End If
                                    End If
                                    strTermdesc = ""
                                    acAttrReference.TextStyleId = CType(acTransaction.GetObject(acDatabase.TextStyleTableId, OpenMode.ForRead), TextStyleTable)("WD")
                                    acAttrReference.Layer = "PDESC"

                            End Select

                            acBlkRef.AttributeCollection.AppendAttribute(acAttrReference)
                            acTransaction.AddNewlyCreatedDBObject(acAttrReference, True)
                        End Using
                    End If
                Next
            End Using
        End Sub

        Private Function SetTermdesc(wireList As List(Of WireClass), ByRef isCable As Boolean) As String
            Dim strTermdesc = ""
            If wireList.Any(Function(x As WireClass) x.HasCable) Then
                strTermdesc = "в " & wireList.Find(Function(x As WireClass) x.HasCable).Cable.Mark
                isCable = True
            Else
                For lngA = 0 To wireList.Count - 1
                    If wireList.Item(lngA).Termdesc <> "" Then
                        If strTermdesc = "" Then
                            strTermdesc = "к " & wireList.Item(lngA).Termdesc & ", "
                        Else
                            strTermdesc = strTermdesc & wireList.Item(lngA).Termdesc & ", "
                        End If
                    End If
                Next
                If strTermdesc <> "" Then strTermdesc = strTermdesc.Remove(strTermdesc.Length - 2)
            End If
            Return strTermdesc
        End Function

        Friend Sub DrawAccessorisesBlock(acDatabase As Autodesk.AutoCAD.DatabaseServices.Database, acTransaction As Transaction, blnDetailsTerminal As Boolean,
            ByRef objInputTerminal As MultilevelTerminalClass, acInsertPt As Point3d, dblScale As Double)
            Dim acBlockTable As BlockTable = Nothing
            Dim acInsObjectId As ObjectId = Nothing

            If blnDetailsTerminal Then
                DetailBlockExists(acDatabase, acTransaction, objInputTerminal, acBlockTable, acInsObjectId)
            Else
                SimpleBlockExists(acDatabase, acTransaction, objInputTerminal, acBlockTable, acInsObjectId)
            End If

            Using acBlkRef As New BlockReference(acInsertPt, acInsObjectId)
                acBlkRef.Layer = "PSYMS"
                acBlkRef.ScaleFactors = New Scale3d(dblScale)

                Dim acBlockTableRecord As BlockTableRecord = acTransaction.GetObject(acBlockTable.Item(BlockTableRecord.ModelSpace), OpenMode.ForWrite)
                acBlockTableRecord.AppendEntity(acBlkRef)
                acTransaction.AddNewlyCreatedDBObject(acBlkRef, True)

                Dim acBlockTableAttrRec As BlockTableRecord = acTransaction.GetObject(acInsObjectId, OpenMode.ForRead)
                Dim acAttrObjectId As ObjectId
                For Each acAttrObjectId In acBlockTableAttrRec
                    Dim acAttrEntity As Entity = acTransaction.GetObject(acAttrObjectId, OpenMode.ForRead)
                    Dim acAttrDefinition = TryCast(acAttrEntity, AttributeDefinition)
                    If (acAttrDefinition IsNot Nothing) Then
                        Using acAttrReference As New AttributeReference()
                            acAttrReference.SetAttributeFromBlock(acAttrDefinition, acBlkRef.BlockTransform)
                            Select Case acAttrReference.Tag
                                Case "P_TAGSTRIP"
                                    acAttrReference.TextString = objInputTerminal.TagStrip
                                    acAttrReference.TextStyleId = CType(acTransaction.GetObject(acDatabase.TextStyleTableId, OpenMode.ForRead), TextStyleTable)("WD_IEC")
                                    acAttrReference.Layer = "PTAG"
                                Case "INST"
                                    acAttrReference.TextString = objInputTerminal.TextInstance
                                    acAttrReference.TextStyleId = CType(acTransaction.GetObject(acDatabase.TextStyleTableId, OpenMode.ForRead), TextStyleTable)("WD_IEC")
                                    acAttrReference.Layer = "PLOC"
                                Case "LOC"
                                    acAttrReference.TextString = objInputTerminal.Location
                                    acAttrReference.TextStyleId = CType(acTransaction.GetObject(acDatabase.TextStyleTableId, OpenMode.ForRead), TextStyleTable)("WD_IEC")
                                    acAttrReference.Layer = "PLOC"
                                Case "MFG"
                                    acAttrReference.TextString = objInputTerminal.Manufacture
                                    acAttrReference.TextStyleId = CType(acTransaction.GetObject(acDatabase.TextStyleTableId, OpenMode.ForRead), TextStyleTable)("WD_IEC")
                                    acAttrReference.Layer = "PMFG"
                                Case "CAT"
                                    acAttrReference.TextString = objInputTerminal.Catalog
                                    acAttrReference.TextStyleId = CType(acTransaction.GetObject(acDatabase.TextStyleTableId, OpenMode.ForRead), TextStyleTable)("WD_IEC")
                                    acAttrReference.Layer = "PCAT"
                                Case "HEIGHT"
                                    objInputTerminal.Height = CDbl(acAttrReference.TextString)
                            End Select

                            acBlkRef.AttributeCollection.AppendAttribute(acAttrReference)
                            acTransaction.AddNewlyCreatedDBObject(acAttrReference, True)
                        End Using
                    End If
                Next
            End Using
        End Sub

        Friend Sub DrawJumpers(acDatabase As Autodesk.AutoCAD.DatabaseServices.Database, acTransaction As Transaction, objJumper As JumperClass, acJumperInsertPt As Point3d, dblScale As Double, blnDetailsTerminal As Boolean)
            Dim acBlockTable As BlockTable = Nothing
            Dim acInsObjectId As ObjectId = Nothing

            If blnDetailsTerminal Then
                DetailBlockExists(acDatabase, acTransaction, objJumper, acBlockTable, acInsObjectId)
            Else
                SimpleBlockExists(acDatabase, acTransaction, objJumper, acBlockTable, acInsObjectId)
            End If

            Using acBlkRef As New BlockReference(acJumperInsertPt, acInsObjectId)
                acBlkRef.Layer = "PSYMS"
                acBlkRef.ScaleFactors = New Scale3d(dblScale)

                Dim acBlockTableRecord As BlockTableRecord = acTransaction.GetObject(acBlockTable.Item(BlockTableRecord.ModelSpace), OpenMode.ForWrite)
                acBlockTableRecord.AppendEntity(acBlkRef)
                acTransaction.AddNewlyCreatedDBObject(acBlkRef, True)

                Dim acBlockTableAttrRec As BlockTableRecord = acTransaction.GetObject(acInsObjectId, OpenMode.ForRead)
                Dim acAttrObjectId As ObjectId
                For Each acAttrObjectId In acBlockTableAttrRec
                    Dim acAttrEntity As Entity = acTransaction.GetObject(acAttrObjectId, OpenMode.ForRead)
                    Dim acAttrDefinition = TryCast(acAttrEntity, AttributeDefinition)
                    If (acAttrDefinition IsNot Nothing) Then
                        Using acAttrReference As New AttributeReference()
                            acAttrReference.SetAttributeFromBlock(acAttrDefinition, acBlkRef.BlockTransform)
                            Select Case acAttrReference.Tag
                                Case "P_TAGSTRIP"
                                    acAttrReference.TextString = objJumper.TagStrip
                                    acAttrReference.TextStyleId = CType(acTransaction.GetObject(acDatabase.TextStyleTableId, OpenMode.ForRead), TextStyleTable)("WD_IEC")
                                    acAttrReference.Layer = "PTAG"
                                Case "INST"
                                    acAttrReference.TextString = objJumper.TextInstance
                                    acAttrReference.TextStyleId = CType(acTransaction.GetObject(acDatabase.TextStyleTableId, OpenMode.ForRead), TextStyleTable)("WD_IEC")
                                    acAttrReference.Layer = "PLOC"
                                Case "LOC"
                                    acAttrReference.TextString = objJumper.Location
                                    acAttrReference.TextStyleId = CType(acTransaction.GetObject(acDatabase.TextStyleTableId, OpenMode.ForRead), TextStyleTable)("WD_IEC")
                                    acAttrReference.Layer = "PLOC"
                                Case "MFG"
                                    acAttrReference.TextString = objJumper.Manufacture
                                    acAttrReference.TextStyleId = CType(acTransaction.GetObject(acDatabase.TextStyleTableId, OpenMode.ForRead), TextStyleTable)("WD_IEC")
                                    acAttrReference.Layer = "PMFG"
                                Case "CAT"
                                    acAttrReference.TextString = objJumper.Catalog
                                    acAttrReference.TextStyleId = CType(acTransaction.GetObject(acDatabase.TextStyleTableId, OpenMode.ForRead), TextStyleTable)("WD_IEC")
                                    acAttrReference.Layer = "PCAT"
                            End Select

                            acBlkRef.AttributeCollection.AppendAttribute(acAttrReference)
                            acTransaction.AddNewlyCreatedDBObject(acAttrReference, True)
                        End Using
                    End If
                Next
            End Using
        End Sub

        Private Sub DetailBlockExists(acDatabase As Autodesk.AutoCAD.DatabaseServices.Database, acTransaction As Transaction, objInputObject As Object, ByRef acBlockTable As BlockTable, ByRef acInsObjectId As ObjectId)
            Dim strBlkName As String = SymbolUtilityServices.GetBlockNameFromInsertPathName(objInputObject.DetailBlockPath)
            acBlockTable = acTransaction.GetObject(acDatabase.BlockTableId, OpenMode.ForRead)

            If acBlockTable.Has(strBlkName) Then
                Dim acCurrBlkTblRcd As BlockTableRecord = acTransaction.GetObject(acBlockTable.Item(strBlkName), OpenMode.ForRead)
                acInsObjectId = acCurrBlkTblRcd.Id
            Else
                Try
                    Dim acNewDbDwg As New Autodesk.AutoCAD.DatabaseServices.Database(False, True)
                    acNewDbDwg.ReadDwgFile(objInputObject.DetailBlockPath, FileOpenMode.OpenTryForReadShare, True, "")
                    acInsObjectId = acDatabase.Insert(strBlkName, acNewDbDwg, True)
                    acNewDbDwg.Dispose()
                Catch ex As Exception
                    MsgBox(String.Format("Не найден графический образ с именем {0}", strBlkName))
                End Try
            End If
        End Sub

        Private Sub SimpleBlockExists(acDatabase As Autodesk.AutoCAD.DatabaseServices.Database, acTransaction As Transaction, objInputObject As Object, ByRef acBlockTable As BlockTable, ByRef acInsObjectId As ObjectId, Optional strLevel As String = "")
            Dim strPath = ""

            If Not strLevel.Equals("") Then
                strPath = objInputObject.SimpleBlockPath.Insert(objInputObject.SimpleBlockPath.IndexOf("."), "_" & strLevel)
            Else
                strPath = objInputObject.SimpleBlockPath
            End If

            Dim strBlkName As String = SymbolUtilityServices.GetBlockNameFromInsertPathName(strPath)
            acBlockTable = acTransaction.GetObject(acDatabase.BlockTableId, OpenMode.ForRead)

            If acBlockTable.Has(strBlkName) Then
                Dim acCurrBlkTblRcd As BlockTableRecord = acTransaction.GetObject(acBlockTable.Item(strBlkName), OpenMode.ForRead)
                acInsObjectId = acCurrBlkTblRcd.Id
            Else
                Try
                    Dim acNewDbDwg As New Autodesk.AutoCAD.DatabaseServices.Database(False, True)
                    acNewDbDwg.ReadDwgFile(strPath, FileOpenMode.OpenTryForReadShare, True, "")
                    acInsObjectId = acDatabase.Insert(strBlkName, acNewDbDwg, True)
                    acNewDbDwg.Dispose()
                Catch ex As Exception
                    MsgBox(String.Format("Не найден графический образ с именем {0}", strBlkName))
                End Try
            End If
        End Sub

        Private Sub DrawLine(acDatabase As Autodesk.AutoCAD.DatabaseServices.Database, acTransaction As Transaction, acInsertionPoint As Point3d, lenght As Integer, isCable As Boolean, dblScale As Double)
            Dim acBlockTable As BlockTable = acTransaction.GetObject(acDatabase.BlockTableId, OpenMode.ForRead)
            Dim acBlockTableRecord As BlockTableRecord = acTransaction.GetObject(acBlockTable(BlockTableRecord.ModelSpace), OpenMode.ForWrite)

            Using acLine As Line = New Line(acInsertionPoint,
                                            New Point3d(acInsertionPoint.X + (lenght * dblScale), acInsertionPoint.Y, 0))
                If isCable Then acLine.Layer = "Cable"
                acBlockTableRecord.AppendEntity(acLine)
                acTransaction.AddNewlyCreatedDBObject(acLine, True)
            End Using
        End Sub

    End Module
End Namespace
