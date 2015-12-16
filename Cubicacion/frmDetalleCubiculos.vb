Imports System.Data.SqlClient
Imports System.Text.RegularExpressions
Imports System.Configuration

Public Class frmDetalleCubiculos

    Protected configuracion As ConnectionStringSettings = ConfigurationManager.ConnectionStrings("cnn")
    Dim cnn As New SqlConnection(configuracion.ConnectionString)

    Public Sub cargardataCubiculo()
        Try

            Dim command As SqlCommand
            Dim adapter As SqlDataAdapter
            Dim dtTable As DataTable
            cnn.Open()
            'Indico el SP que voy a utilizar
            command = New SqlCommand("sp_ResumenCubiculos", cnn)
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
            cnn.Close()
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub
    Private Sub frmResumenGeneral_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load
        cargardataCubiculo()
    End Sub


    Private Sub Button1_Click(sender As System.Object, e As System.EventArgs) Handles Button1.Click
        Try
            DefaultCursor.Current = Cursors.WaitCursor
            Dim dt As New DataTable
            dt = Me.DataGridView1.DataSource

            Dim cr As New crDetalleCubiculos
            cr.SetDataSource(dt)

            Dim r As New frmReporte
            r.CrystalReportViewer1.ReportSource = cr
            r.ShowDialog()
            cnn.Open()
            Dim cmd4 As New SqlCommand("sp_CrearBitacora", cnn)

            cmd4.CommandType = CommandType.StoredProcedure

            cmd4.Parameters.AddWithValue("@fecha_Ingreso", Date.Now)
            cmd4.Parameters.AddWithValue("@id_EmpleadoIngreso", clsLogin.IdUsuario)
            cmd4.Parameters.AddWithValue("@Detalle", "GENERA REPOTE COMPLETO")
            cmd4.Parameters.AddWithValue("@Tipo", 8)
            cmd4.ExecuteNonQuery()

            cnn.Close()

            Me.Close()
        Catch ex As Exception
            MsgBox(ex.Message)
            cnn.Open()
            Dim cmd4 As New SqlCommand("sp_CrearBitacora", cnn)

            cmd4.CommandType = CommandType.StoredProcedure

            cmd4.Parameters.AddWithValue("@fecha_Ingreso", Date.Now)
            cmd4.Parameters.AddWithValue("@id_EmpleadoIngreso", clsLogin.IdUsuario)
            cmd4.Parameters.AddWithValue("@Detalle", "ERROR AL GENERAR REPOTE CUBICULO " & ex.Message)
            cmd4.Parameters.AddWithValue("@Tipo", 9)
            cmd4.ExecuteNonQuery()

            cnn.Close()
        End Try
    End Sub
End Class