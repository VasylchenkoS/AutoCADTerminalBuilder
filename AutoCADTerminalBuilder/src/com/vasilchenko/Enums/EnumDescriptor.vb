Imports System.ComponentModel
Imports System.Reflection

Namespace com.vasilchenko.Enums
    ''' <summary>
    ''' Contains an enumerated constant value and a friendly description of that value, if one exists.
    ''' </summary>
    ''' <typeparam name="T">
    ''' The enumerated type of the value.
    ''' </typeparam>
    Public Class EnumDescriptor(Of T)

        ''' <summary>
        ''' The friendly description of the value.
        ''' </summary>
        Private _description As String
        ''' <summary>
        ''' The enumerated constant value.
        ''' </summary>
        Private _value As T

        ''' <summary>
        ''' Gets the friendly description of the value.
        ''' </summary>
        ''' <value>
        ''' A <b>String</b> containing the value's Description attribute value if one exists; otherwise, the value name.
        ''' </value>
        Public ReadOnly Property Description() As String
            Get
                Return Me._description
            End Get
        End Property

        ''' <summary>
        ''' Gets the enumerated constant value.
        ''' </summary>
        ''' <value>
        ''' An enumerated constant of the <b>EnumDescriptor's</b> generic parameter type.
        ''' </value>
        Public ReadOnly Property Value() As T
            Get
                Return Me._value
            End Get
        End Property

        ''' <summary>
        ''' Creates a new instance of the <b>EnumDescriptor</b> class.
        ''' </summary>
        ''' <param name="value">
        ''' The value to provide a description for.
        ''' </param>
        Public Sub New(ByVal value As T)
            Me._value = value

            'Get the Description attribute.
            Dim field As FieldInfo = value.GetType().GetField(value.ToString())
            Dim attributes As DescriptionAttribute() = DirectCast(field.GetCustomAttributes(GetType(DescriptionAttribute), False), DescriptionAttribute())

            'Use the Description attribte if one exists, otherwise use the value itself as the description.
            Me._description = If(attributes.Length = 0,
                             value.ToString(),
                             attributes(0).Description)
        End Sub

        ''' <summary>
        ''' Overridden.  Creates a string representation of the object.
        ''' </summary>
        ''' <returns>
        ''' The friendly description of the value.
        ''' </returns>
        Public Overrides Function ToString() As String
            Return Me.Description
        End Function

    End Class

End Namespace