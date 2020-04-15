Imports System.ComponentModel
Imports System.Reflection
Imports AutoCADTerminalBuilder.com.vasilchenko.Classes
Imports AutoCADTerminalBuilder.com.vasilchenko.Database

Namespace com.vasilchenko.Enums
    Module EnumFunctions
        Public Function GetEnumFromDescriptionAttribute(Of T)(description As String) As T
            Dim type = GetType(T)
            If Not type.IsEnum Then
                Throw New InvalidOperationException()
            End If
            For Each fieldInfo As FieldInfo In type.GetFields()
                Dim descriptionAttribute = TryCast(Attribute.GetCustomAttribute(fieldInfo, GetType(DescriptionAttribute)), DescriptionAttribute)
                If descriptionAttribute IsNot Nothing Then
                    If descriptionAttribute.Description <> description Then
                        Continue For
                    End If
                    Return DirectCast(fieldInfo.GetValue(Nothing), T)
                End If
                If fieldInfo.Name <> description Then
                    Continue For
                End If
                Return DirectCast(fieldInfo.GetValue(Nothing), T)
            Next
            Return Nothing
        End Function

        Public Function Convert(eAccEnum As TerminalAccessoriesEnum, objCurLevelTerminal As MultilevelTerminalClass) As MultilevelTerminalClass
            Dim objTerminalAcc As New MultilevelTerminalClass With {
                .TagStrip = objCurLevelTerminal.TagStrip,
                .TextInstance = IIf(objCurLevelTerminal.TextInstance.StartsWith("6"), "62.КЛЕММЫ-ПРОЧЕЕ", "762.КЛЕММЫ-ПРОЧЕЕ"),
                .Location = objCurLevelTerminal.Location,
                .Manufacture = objCurLevelTerminal.Manufacture,
                .Catalog = New EnumDescriptor(Of TerminalAccessoriesEnum)(eAccEnum).ToString
            }
            DataAccessObject.FillObjectBlockPath(objTerminalAcc)
            Return objTerminalAcc
        End Function

    End Module
End Namespace

