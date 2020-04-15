Namespace com.vasilchenko.Classes 
    Public Class TerminalDictionaryClass(Of TKey, TValue)
        Private _count As Integer
        Private ReadOnly _keys As New List(Of TKey)
        Private ReadOnly _values As New List(Of TValue)
        Public ReadOnly Property Count As Integer
            Get
                Return _keys.Count
            End Get
        End Property
        Public ReadOnly Property Keys As List(Of TKey)
            Get
                Return _keys
            End Get
        End Property
        Public ReadOnly Property Values As List(Of TValue)
            Get
                Return _values
            End Get
        End Property

        Friend Function Items() As IEnumerable(Of TValue)
            Return _values
        End Function

        Public Sub Add(key As TKey, value As TValue)
            _keys.Add(key)
            _values.Add(value)
            _count += 1
        End Sub
        Public Sub AddAt(key As TKey, value As TValue, index As Integer)
            _keys.Insert(index, key)
            _values.Insert(index, value)
            _count += 1
        End Sub
        Public Sub RemoveAt(index As Integer)
            _keys.RemoveAt(index)
            _values.RemoveAt(index)
            _count -= 1
        End Sub
        Public Sub Remove(key As TKey)
            Dim index As Integer = _keys.IndexOf(key)
            _keys.RemoveAt(index)
            _values.RemoveAt(index)
            _count -= 1
        End Sub
        Public Function ContainsKey(key As TKey) As Boolean
            Return _keys.Contains(key)
        End Function
        Public Function Item(key As TKey) As TValue
            Return _values.Item(_keys.IndexOf(key))
        End Function
        Public Function Item(index As Int64) As TValue
            Return _values.Item(index)
        End Function
    End Class
End Namespace