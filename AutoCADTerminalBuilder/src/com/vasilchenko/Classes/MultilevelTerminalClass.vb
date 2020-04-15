Imports System.Linq
Imports AutoCADTerminalBuilder.com.vasilchenko.Modules

Namespace com.vasilchenko.Classes

    Public Class MultilevelTerminalClass
        Public Property Terminal As SortedList(Of String, SingleLevelTerminalClass)
        Public Property TagStrip As String
        Public Property NumericInstance As Integer
        Public Property TextInstance As String
        Public Property Location As String
        Public Property Manufacture As String
        Public Property Catalog As String
        Public Property Notes As String
        Public Property WdBlockName As String
        Public Property Width As Double
        Public Property Height As Double
        Public Property SimpleBlockPath As String
        Public Property DetailBlockPath As String
        Public Property TerminalLink As String


        Public Sub New()
            Me.WdBlockName = "TRMS"
        End Sub

        Public ReadOnly Property MainTermNumber As String
            Get
                Return Terminal.Values.Min(Function(x) AdditionalFunctions.GetLastNumericFromString(x.TerminalNumber))
            End Get
        End Property

        Protected Overrides Sub Finalize()
            Me.Terminal = Nothing
            MyBase.Finalize()
        End Sub
    End Class
End Namespace