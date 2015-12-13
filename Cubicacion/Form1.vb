Imports System.Data.SqlClient
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
            DataGridView1.Columns(2).Width = 35
            DataGridView1.Columns(3).Width = 35

            With DataGridView1
                ' alternar colores  
                .RowsDefaultCellStyle.BackColor = Color.MediumAquamarine

                .AlternatingRowsDefaultCellStyle.BackColor = Color.Khaki
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
    End Sub

    Private Sub DespachoToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles DespachoToolStripMenuItem.Click
        frmInfoDespacho.MdiParent = Me
        frmInfoDespacho.Show()
    End Sub

    Private Sub UsuariosToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles UsuariosToolStripMenuItem.Click
        frmUsuarios.MdiParent = Me
        frmUsuarios.Show()
    End Sub

    Private Sub ProveedoresToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles ProveedoresToolStripMenuItem.Click

    End Sub
End Class
