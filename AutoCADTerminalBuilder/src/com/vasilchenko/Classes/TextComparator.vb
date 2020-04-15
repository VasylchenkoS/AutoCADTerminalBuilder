Namespace com.vasilchenko.Classes
    Public Class TextComparator
        Implements Generic.IComparer(Of String)

        ''' <returns>
        ''' Zero if x is equal to y;
        ''' A value less than zero if x is greater than y;
        ''' A value greater than zero if x is less than y.
        ''' </returns>
        ''' <remarks></remarks>
        Public Function Compare(ByVal x As String, ByVal y As String) As Integer Implements System.Collections.Generic.IComparer(Of String).Compare
            Return CInt(x) - CInt(y)
        End Function
    End Class
End Namespace