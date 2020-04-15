Namespace com.vasilchenko.Classes
    Public Class JumperClass

        Public Property TagStrip As String
        Private Property NumericInstance As Short 
        Public Property TextInstance As String
        Public Property Location As String
        Public Property Manufacture As String
        Public Property Catalog As String
        Public Property Notes As String
        Private Property WdBlockName As String
        Public Property SimpleBlockPath As String
        Public Property DetailBlockPath As String
        Public Property StartTermNum As Integer
        Public Property Side As Enums.SideEnum
        Public Property TermCount As Integer

        Public Sub New()
            TextInstance = "63.КЛЕММЫ-ПЕРЕМЫЧКИ"
            Me.NumericInstance = 63
            Me.WdBlockName = "TRMS"
        End Sub

        Public ReadOnly Property GetInstance As String
            Get
                Return Me.NumericInstance
            End Get
        End Property

        Public ReadOnly Property GetWdBlockName As String
            Get
                Return Me.WdBlockName
            End Get
        End Property

        Protected Overrides Sub Finalize()
            MyBase.Finalize()
        End Sub

    End Class
End Namespace
