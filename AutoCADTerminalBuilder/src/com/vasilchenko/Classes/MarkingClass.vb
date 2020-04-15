Namespace com.vasilchenko.Classes 
    Public Class MarkingClass
        Public Property PinOne As PinClass
        Public Property PinTwo As PinClass

        Sub New(pinOne As PinClass, pinTwo As PinClass)
            Me.PinOne = pinOne
            Me.PinTwo = pinTwo
        End Sub

    End Class
End Namespace
