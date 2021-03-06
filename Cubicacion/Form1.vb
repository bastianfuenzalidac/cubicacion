﻿Imports System.Data.SqlClient
Imports System.Text.RegularExpressions
Imports System.Configuration
Public Class Form1

    Protected configuracion As ConnectionStringSettings = ConfigurationManager.ConnectionStrings("cnn")
    Dim cnn As New SqlConnection(configuracion.ConnectionString)
    Public Sub cargardataFechaSalida()
        Try

            Dim command As SqlCommand
            Dim adapter As SqlDataAdapter
            Dim dtTable As DataTable
            cnn.Open()
            'Indico el SP que voy a utilizar
            command = New SqlCommand("sp_FechaSalida", cnn)
            command.CommandType = CommandType.StoredProcedure
            command.ExecuteNonQuery()

            command.CommandType = CommandType.StoredProcedure
            adapter = New SqlDataAdapter(command)
            dtTable = New DataTable

            'Aquí ejecuto el SP y lo lleno en el DataTable
            adapter.Fill(dtTable)
            'Enlazo mis datos obtenidos en el DataTable con el grid
            DataGridView1.DataSource = dtTable
            'DataGridView1.Columns(2).HeaderText = "% Disponible"
            DataGridView1.Columns(0).Width = 70
            DataGridView1.Columns(1).Width = 115
            DataGridView1.Columns(2).Width = 70
            DataGridView1.Columns(3).Visible = False
            DataGridView1.Columns(4).Visible = False

            With DataGridView1
                ' alternar colores  
                .RowsDefaultCellStyle.BackColor = Color.Silver

                .AlternatingRowsDefaultCellStyle.BackColor = Color.White
                .ForeColor = Color.Black
                .DefaultCellStyle.SelectionForeColor = Color.Black
                .DefaultCellStyle.SelectionBackColor = Color.LightSteelBlue
                .ClearSelection()
            End With

            cnn.Close()


        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub

    Public Sub cargardataEstadoPiso()
        Try

            Dim command As SqlCommand
            Dim adapter As SqlDataAdapter
            Dim dtTable As DataTable
            cnn.Open()
            'Indico el SP que voy a utilizar
            command = New SqlCommand("sp_EstadoPiso", cnn)
            command.CommandType = CommandType.StoredProcedure
            command.ExecuteNonQuery()

            command.CommandType = CommandType.StoredProcedure
            adapter = New SqlDataAdapter(command)
            dtTable = New DataTable

            'Aquí ejecuto el SP y lo lleno en el DataTable
            adapter.Fill(dtTable)
            'Enlazo mis datos obtenidos en el DataTable con el grid
            DataGridView2.DataSource = dtTable
            'DataGridView1.Columns(2).HeaderText = "% Disponible"
            DataGridView2.Columns(0).Width = 180
            DataGridView2.Columns(1).Width = 78


            With DataGridView2
                ' alternar colores  
                .RowsDefaultCellStyle.BackColor = Color.GreenYellow

                .AlternatingRowsDefaultCellStyle.BackColor = Color.White
                .ForeColor = Color.Black
                .DefaultCellStyle.SelectionForeColor = Color.Black
                .DefaultCellStyle.SelectionBackColor = Color.LightSteelBlue
                .ClearSelection()
            End With

            cnn.Close()
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub

    Private Sub EstadoCubiculosToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles EstadoCubiculosToolStripMenuItem.Click
        frmVisualizarCubiculos.MdiParent = Me
        frmVisualizarCubiculos.Show()
    End Sub

    Private Sub CubiculosToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles CubiculosToolStripMenuItem.Click
        frmAdmCubiculos.MdiParent = Me
        frmAdmCubiculos.Show()
    End Sub

    Private Sub CargaToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles CargaToolStripMenuItem.Click
        frmQR.MdiParent = Me
        frmQR.Show()
    End Sub

    Private Sub CompletoToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles CompletoToolStripMenuItem.Click
        frmResumenCompleto.MdiParent = Me
        frmResumenCompleto.Show()
    End Sub


    Private Sub ResumenToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles ResumenToolStripMenuItem.Click
        frmResumenResumido.MdiParent = Me
        frmResumenResumido.Show()
    End Sub

    Private Sub GenralToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles GenralToolStripMenuItem.Click
        frmResumenGeneral.MdiParent = Me
        frmResumenGeneral.Show()
    End Sub

    Private Sub Form1_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load
        cargardataFechaSalida()
        cargardataEstadoPiso()
        If clsLogin.Rol = "2" Then
            AdministraciónToolStripMenuItem.Visible = False
            ProveedoresToolStripMenuItem.Visible = False
            EstadoToolStripMenuItem.Visible = False
            DespachosToolStripMenuItem.Visible = False

        End If
    End Sub


    Private Sub ProveedoresToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles ProveedoresToolStripMenuItem.Click
        frmProveedores.MdiParent = Me
        frmProveedores.Show()
    End Sub

    Private Sub RealizarDespachoToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles RealizarDespachoToolStripMenuItem.Click
        frmRealizarDespacho.MdiParent = Me
        frmRealizarDespacho.Show()
    End Sub

    Private Sub DataGridView1_CellContentClick(sender As Object, e As System.Windows.Forms.DataGridViewCellEventArgs) Handles DataGridView1.CellContentClick
   
    End Sub

    Private Sub DataGridView1_CellFormatting(sender As Object, e As System.Windows.Forms.DataGridViewCellFormattingEventArgs) Handles DataGridView1.CellFormatting
        Dim row As DataGridViewRow = DataGridView1.Rows(e.RowIndex)
        Dim cell As DataGridViewCell = DataGridView1.Rows(e.RowIndex).Cells(3)

        If CStr(cell.Value) = "3" Then
            row.DefaultCellStyle.BackColor = Color.Blue
        ElseIf CStr(cell.Value) = "2" Then
            row.DefaultCellStyle.BackColor = Color.Red

        End If
    End Sub

    Private Sub CrearToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles CrearToolStripMenuItem.Click
        frmUsuarios.MdiParent = Me
        frmUsuarios.Show()
    End Sub

    Private Sub DataGridView1_DoubleClick(sender As Object, e As System.EventArgs) Handles DataGridView1.DoubleClick
        frmInfoDespacho.norden = DataGridView1.Rows(DataGridView1.CurrentRow.Index).Cells(4).Value
        frmInfoDespacho.Show()
    End Sub

    Private Sub ToolStripMenuItem1_Click(sender As System.Object, e As System.EventArgs) Handles ToolStripMenuItem1.Click
        frmdespachoscreados.MdiParent = Me
        frmdespachoscreados.Show()
    End Sub

    Private Sub PorFechaToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles PorFechaToolStripMenuItem.Click
        frmDespachosPendientes.MdiParent = Me
        frmDespachosPendientes.Show()
    End Sub


    Private Sub DetalleCubiculosToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles DetalleCubiculosToolStripMenuItem.Click
        frmDetalleCubiculos.MdiParent = Me
        frmDetalleCubiculos.Show()
    End Sub

End Class
