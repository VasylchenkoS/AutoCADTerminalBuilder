Namespace com.vasilchenko.Enums
    ''' <summary>
    ''' A collection of EnumDescriptors for an enumerated type.
    ''' </summary>
    ''' <typeparam name="T">
    ''' The type of the enumeration for which the EnumDescriptors are created.
    ''' </typeparam>
    Public Class EnumDescriptorCollection(Of T)
        Inherits ObjectModel.Collection(Of EnumDescriptor(Of T))

        ''' <summary>
        ''' Creates a new instance of the <b>EnumDescriptorCollection</b> class.
        ''' </summary>
        Public Sub New()
            'Populate the collection with an EnumDescriptor for each enumerated value.
            For Each value As T In [Enum].GetValues(GetType(T))
                Me.Items.Add(New EnumDescriptor(Of T)(value))
            Next
        End Sub

    End Class

End Namespace
