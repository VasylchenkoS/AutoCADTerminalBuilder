Namespace com.vasilchenko.Classes

    Public Class PinClass

        Public Property TagName As String
        Public Property Pin As String
        Public Property CableName As String

        Sub New(TagName As String, Pin As String)
            Me.TagName = TagName 
            Me.Pin = Pin 
        End Sub

        Public Sub New()
        End Sub
    End Class

End Namespace
