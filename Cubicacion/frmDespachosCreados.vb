Imports System.Data.SqlClient
Imports System.Text.RegularExpressions
Imports System.Configuration

Public Class frmdespachoscreados

    Protected configuracion As ConnectionStringSettings = ConfigurationManager.ConnectionStrings("cnn")
    Dim cnn As New SqlConnection(configuracion.ConnectionString)
    Dim variable As SqlDataReader
    Dim consulta As New SqlCommand


    Public Sub cargarcomboTipo()
        If cnn.State = ConnectionState.Open Then
            cnn.Close()
        End If
        consulta.CommandType = CommandType.Text
        consulta.CommandText = ("select nombre from tbl_proveedor")
        consulta.Connection = (cnn)
        cnn.Open()
        variable = consulta.ExecuteReader()

        While variable.Read = True
            cbProveedor.Items.Add(variable.Item(0))
        End While
        cnn.Close()
    End Sub

    Public Sub cargardataCubiculo()
        Try

            Dim command As SqlCommand
            Dim adapter As SqlDataAdapter
            Dim dtTable As DataTable
            cnn.Open()
            'Indico el SP que voy a utilizar
            command = New SqlCommand("sp_ResumenDespachosCreados", cnn)
            command.CommandType = CommandType.StoredProcedure
            command.Parameters.AddWithValue("@aux", cbProveedor.text)
            command.ExecuteNonQuery()

            command.CommandType = CommandType.StoredProcedure
            adapter = New SqlDataAdapter(command)
            dtTable = New DataTable

            'Aquí ejecuto el SP y lo lleno en el DataTable
            adapter.Fill(dtTable)
            'Enlazo mis datos obtenidos en el DataTable con el grid
            DataGridView1.DataSource = dtTable
            ' DataGridView1.Columns(0).HeaderText = ""
            cnn.Close()
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub

    Public Sub cargardataCubiculoTodos()
        Try

            Dim command As SqlCommand
            Dim adapter As SqlDataAdapter
            Dim dtTable As DataTable
            cnn.Open()
            'Indico el SP que voy a utilizar
            command = New SqlCommand("sp_ResumenDespachosCreadosTodos", cnn)
            command.CommandType = CommandType.StoredProcedure
            command.ExecuteNonQuery()

            command.CommandType = CommandType.StoredProcedure
            adapter = New SqlDataAdapter(command)
            dtTable = New DataTable

            'Aquí ejecuto el SP y lo lleno en el DataTable
            adapter.Fill(dtTable)
            'Enlazo mis datos obtenidos en el DataTable con el grid
            DataGridView1.DataSource = dtTable
            ' DataGridView1.Columns(0).HeaderText = ""
            cnn.Close()
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub

    Public Sub cargardataCubiculoNorden()
        Try

            Dim command As SqlCommand
            Dim adapter As SqlDataAdapter
            Dim dtTable As DataTable
            cnn.Open()
            'Indico el SP que voy a utilizar
            command = New SqlCommand("sp_ResumenDespachosCreadosNorden", cnn)
            command.CommandType = CommandType.StoredProcedure
            command.Parameters.AddWithValue("@aux", TextBox1.Text)
            command.ExecuteNonQuery()

            command.CommandType = CommandType.StoredProcedure
            adapter = New SqlDataAdapter(command)
            dtTable = New DataTable

            'Aquí ejecuto el SP y lo lleno en el DataTable
            adapter.Fill(dtTable)
            'Enlazo mis datos obtenidos en el DataTable con el grid
            DataGridView1.DataSource = dtTable
            ' DataGridView1.Columns(0).HeaderText = ""
            cnn.Close()
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub

    Private Sub frmResumenGeneral_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load
        cargardataCubiculo()
        cargarcomboTipo()
    End Sub


    Private Sub Button1_Click(sender As System.Object, e As System.EventArgs) Handles Button1.Click
        Try
            Cursor.Current = Cursors.WaitCursor
            Dim dt As New DataTable
            dt = Me.DataGridView1.DataSource

            Dim cr As New crDespachosCreados
            cr.SetDataSource(dt)

            Dim r As New frmReporte
            r.CrystalReportViewer1.ReportSource = cr
            r.ShowDialog()

            cnn.Open()
            Dim cmd4 As New SqlCommand("sp_CrearBitacora", cnn)

            cmd4.CommandType = CommandType.StoredProcedure

            cmd4.Parameters.AddWithValue("@fecha_Ingreso", Date.Now)
            cmd4.Parameters.AddWithValue("@id_EmpleadoIngreso", clsLogin.IdUsuario)
            cmd4.Parameters.AddWithValue("@Detalle", "GENERA REPOTE DESPACHOS CREADOS")
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
            cmd4.Parameters.AddWithValue("@Detalle", "ERROR AL GENERAR REPOTE GENERAL " & ex.Message)
            cmd4.Parameters.AddWithValue("@Tipo", 9)
            cmd4.ExecuteNonQuery()

            cnn.Close()
        End Try
    End Sub

    Private Sub cbProveedor_SelectedIndexChanged(sender As System.Object, e As System.EventArgs) Handles cbProveedor.SelectedIndexChanged
        cargardataCubiculo()
    End Sub

    Private Sub Button2_Click(sender As System.Object, e As System.EventArgs) Handles Button2.Click
        cargardataCubiculoTodos()
    End Sub

    Private Sub Button3_Click(sender As System.Object, e As System.EventArgs) Handles Button3.Click
        cargardataCubiculoNorden()
    End Sub
End Class