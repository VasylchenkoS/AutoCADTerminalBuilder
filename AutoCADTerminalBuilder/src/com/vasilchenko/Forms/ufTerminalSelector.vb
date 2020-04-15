Imports AutoCADTerminalBuilder.com.vasilchenko
Imports AutoCADTerminalBuilder.com.vasilchenko.Enums

Public Class ufTerminalSelector
    Private Sub ufTerminalSelector_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim objLocationList As ArrayList = Database.DataAccessObject.GetAllLocations()

        cbxTerminal.Enabled = False
        cbxOrientation.Enabled = False

        If IsNothing(objLocationList) Then
            MsgBox("Проект пуст или не указаны местоположения", MsgBoxStyle.Critical, "Warning")
            Me.Close()
        Else
            cbxLocation.DataSource = objLocationList
        End If

        cbxOrientation.DataSource = New EnumDescriptorCollection(Of OrientationEnum)
        cbxDuctSide.DataSource = New EnumDescriptorCollection(Of SideEnum)

        'пока отключаю
        cbxOrientation.Enabled = False
        cbxDuctSide.Enabled = False
    End Sub

    Private Sub cbxLocation_SelectedValueChanged(sender As Object, e As EventArgs) Handles cbxLocation.SelectedValueChanged
        cbxTerminal.DataSource = Nothing

        If cbxLocation.Text <> "" Then
            cbxTerminal.DataSource = Database.DataAccessObject.GetAllTagstripInLocation(cbxLocation.Text)
            cbxTerminal.Enabled = True
        Else
            cbxTerminal.Enabled = False
        End If
    End Sub

    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        Me.Close()
    End Sub

    Private Sub btnApply_Click(sender As Object, e As EventArgs) Handles btnApply.Click
        If Not (cbxLocation.Text.Equals("") Or cbxTerminal.Text.Equals("") Or cbxOrientation.Equals("") Or cbxDuctSide.Text.Equals("")) Then
            Me.Hide()
            Modules.TerminalClassMapping.CreateTerminalBlock(cbxLocation.Text,
                                    cbxTerminal.Text,
                                    EnumFunctions.GetEnumFromDescriptionAttribute(Of OrientationEnum)(cbxOrientation.Text),
                                    EnumFunctions.GetEnumFromDescriptionAttribute(Of SideEnum)(cbxDuctSide.Text))
            Me.Close()
        Else
            MsgBox("Введите все данные", MsgBoxStyle.Critical, "DataError")
        End If
    End Sub

End Class