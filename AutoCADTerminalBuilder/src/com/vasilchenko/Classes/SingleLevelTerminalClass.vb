Imports System.Linq

Namespace com.vasilchenko.Classes
    Public Class SingleLevelTerminalClass
        Implements IEquatable(Of SingleLevelTerminalClass)
        Implements IComparable(Of SingleLevelTerminalClass)

        Private _objWiresLeftList As List(Of WireClass)
        Private _objWiresRigthList As List(Of WireClass)

        Public Property Handle As String
        Public Property TerminalNumber As String
        Public Property TerminalLevel As String

        Public Sub New()
            Me._objWiresLeftList = New List(Of WireClass)
            Me._objWiresRigthList = New List(Of WireClass)
        End Sub

        Public WriteOnly Property AddWireLeft As WireClass
            Set(value As WireClass)
                _objWiresLeftList.Add(value)
            End Set
        End Property

        Public WriteOnly Property AddWireRigth As WireClass
            Set(value As WireClass)
                _objWiresRigthList.Add(value)
            End Set
        End Property

        Public Property WiresLeftList As List(Of WireClass)
            Get
                Return _objWiresLeftList
            End Get
            Set(value As List(Of WireClass))
                _objWiresLeftList = value
            End Set
        End Property

        Public Property WiresRigthList As List(Of WireClass)
            Get
                Return _objWiresRigthList
            End Get
            Set(value As List(Of WireClass))
                _objWiresRigthList = value
            End Set
        End Property

        Friend Function WireNameStartsWith(strWireName As String) As Boolean
            Return Me._objWiresLeftList.Any(Function(x) x.WireNumber.StartsWith(strWireName)) Or Me._objWiresRigthList.Any(Function(x) x.WireNumber.StartsWith(strWireName))
        End Function

        Friend Overloads Function WireContains(strWireName As String) As Boolean
            Return Me._objWiresLeftList.Any(Function(x) x.WireNumber.Contains(strWireName)) Or Me._objWiresRigthList.Any(Function(x) x.WireNumber.Contains(strWireName))
        End Function

        Friend Overloads Function WireContains(wireList As List(Of String)) As Boolean
            Dim blnExist As Boolean
            For Each strWireName In wireList
                blnExist = Me._objWiresLeftList.Any(Function(x) x.WireNumber.Contains(strWireName)) Or Me._objWiresRigthList.Any(Function(x) x.WireNumber.Contains(strWireName))
                If blnExist Then Return True
            Next
            Return False
        End Function
        Friend Overloads Function WireRightStartWithCompareTo(objInputTerminal As SingleLevelTerminalClass) As Boolean
            Dim blnExist As Boolean
            'Next
            For Each wire In Me._objWiresRigthList
                blnExist = objInputTerminal.WiresRigthList.Exists(Function(x) x.WireNumber.Contains(Strings.Left(wire.WireNumber, 2)))
                If blnExist Then Return blnExist
            Next
            Return False
        End Function
        Friend Function WireRightContains(wireList As List(Of String)) As Boolean
            Dim blnExist As Boolean
            For Each strWireName In wireList
                blnExist = Me._objWiresRigthList.Any(Function(x) x.WireNumber.Contains(strWireName))
                If blnExist Then Return True
            Next
            Return False
        End Function
        Friend Function WireRightStartWith(wireList As List(Of String)) As Boolean
            Dim blnExist As Boolean
            For Each strWireName In wireList
                blnExist = Me._objWiresRigthList.Any(Function(x) x.WireNumber.StartsWith(strWireName))
                If blnExist Then Return True
            Next
            Return False
        End Function

        Friend Function WireLeftContains(wireList As List(Of String)) As Boolean
            Dim blnExist As Boolean
            For Each strWireName In wireList
                blnExist = Me._objWiresLeftList.Any(Function(x) x.WireNumber.Contains(strWireName))
                If blnExist Then Return True
            Next
            Return False
        End Function

        Public Overloads Function Equals(other As SingleLevelTerminalClass) As Boolean Implements IEquatable(Of SingleLevelTerminalClass).Equals
            If other Is Nothing Then
                Return 1
            Else : Return Me.TerminalNumber.Equals(other.TerminalNumber)
            End If
        End Function

        Public Function CompareTo(other As SingleLevelTerminalClass) As Integer Implements IComparable(Of SingleLevelTerminalClass).CompareTo
            If other Is Nothing Then
                Return 1
            Else : Return Me.TerminalNumber.CompareTo(other.TerminalNumber)
            End If
        End Function

        Protected Overrides Sub Finalize()
            Me._objWiresLeftList = Nothing
            Me._objWiresRigthList = Nothing
            MyBase.Finalize()
        End Sub
    End Class
End Namespace
