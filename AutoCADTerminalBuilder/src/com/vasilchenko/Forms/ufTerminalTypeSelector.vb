Public Class ufTerminalTypeSelector

    Private Sub btnApply_Click(sender As Object, e As EventArgs) Handles btnApply.Click
        Me.Close()
    End Sub

    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        Me.rbtnSignalisation.Checked = False
        Me.rbtnMeasurement.Checked = False
        Me.rbtnControl.Checked = False
        Me.rbtnPower.Checked = False
        Me.rbtnMetering.Checked = False
        Me.rbtnRZA.Checked = False
        Me.rbtnTMRZA.Checked = False

        Me.Dispose()
    End Sub

    Private Sub rbtnControl_Click(sender As Object, e As EventArgs) Handles rbtnControl.Click
        Me.rbtnSignalisation.Checked = False
        Me.rbtnMeasurement.Checked = False
        Me.rbtnPower.Checked = False
        Me.rbtnMetering.Checked = False
        Me.rbtnRZA.Checked = False
        Me.rbtnTMRZA.Checked = False
    End Sub

    Private Sub rbtnMeasurement_Click(sender As Object, e As EventArgs) Handles rbtnMeasurement.Click
        Me.rbtnSignalisation.Checked = False
        Me.rbtnControl.Checked = False
        Me.rbtnPower.Checked = False
        Me.rbtnMetering.Checked = False
        Me.rbtnRZA.Checked = False
        Me.rbtnTMRZA.Checked = False
    End Sub

    Private Sub rbtnPower_Click(sender As Object, e As EventArgs) Handles rbtnPower.Click
        Me.rbtnSignalisation.Checked = False
        Me.rbtnMeasurement.Checked = False
        Me.rbtnControl.Checked = False
        Me.rbtnMetering.Checked = False
        Me.rbtnRZA.Checked = False
        Me.rbtnTMRZA.Checked = False
    End Sub

    Private Sub rbtnSignalisation_Click(sender As Object, e As EventArgs) Handles rbtnSignalisation.Click
        Me.rbtnMeasurement.Checked = False
        Me.rbtnControl.Checked = False
        Me.rbtnPower.Checked = False
        Me.rbtnMetering.Checked = False
        Me.rbtnRZA.Checked = False
        Me.rbtnTMRZA.Checked = False
    End Sub

    Private Sub rbtnMetering_Click(sender As Object, e As EventArgs) Handles rbtnMetering.Click, rbtnRZA.Click
        Me.rbtnSignalisation.Checked = False
        Me.rbtnMeasurement.Checked = False
        Me.rbtnControl.Checked = False
        Me.rbtnPower.Checked = False
        Me.rbtnRZA.Checked = False
        Me.rbtnTMRZA.Checked = False
    End Sub

    Private Sub ufTerminalTypeSelector_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub

    Private Sub rbtnRZA_Click(sender As Object, e As EventArgs) Handles rbtnRZA.CheckedChanged
        Me.rbtnSignalisation.Checked = False
        Me.rbtnMeasurement.Checked = False
        Me.rbtnControl.Checked = False
        Me.rbtnPower.Checked = False
        Me.rbtnMetering.Checked = False
        Me.rbtnTMRZA.Checked = False
        Me.rbtnRZA.Checked = True
    End Sub

    Private Sub rbtnTMRZA_Click(sender As Object, e As EventArgs) Handles rbtnTMRZA.CheckedChanged
        Me.rbtnSignalisation.Checked = False
        Me.rbtnMeasurement.Checked = False
        Me.rbtnControl.Checked = False
        Me.rbtnPower.Checked = False
        Me.rbtnMetering.Checked = False
        Me.rbtnRZA.Checked = False
        Me.rbtnTMRZA.Checked = True
    End Sub
End Class