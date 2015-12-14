Imports System.Data.SqlClient
Imports System.Text.RegularExpressions
Imports System.Configuration
Public Class frmRealizarDespacho
    Dim norden, idDespacho, idCapacidad, idObjeto As String
    Protected configuracion As ConnectionStringSettings = ConfigurationManager.ConnectionStrings("cnn")
    Dim cnn As New SqlConnection(configuracion.ConnectionString)
    Public Sub cargardata()
        Dim command As SqlCommand
        Dim adapter As SqlDataAdapter
        Dim dtTable As DataTable
        cnn.Open()
        'Indico el SP que voy a utilizar
        command = New SqlCommand("sp_DespachoID", cnn)
        command.CommandType = CommandType.StoredProcedure
        command.Parameters.AddWithValue("@aux", norden)
        command.ExecuteNonQuery()

        command.CommandType = CommandType.StoredProcedure
        adapter = New SqlDataAdapter(command)
        dtTable = New DataTable

        'Aquí ejecuto el SP y lo lleno en el DataTable
        adapter.Fill(dtTable)
        'Enlazo mis datos obtenidos en el DataTable con el grid
        DataGridView1.DataSource = dtTable
        'DataGridView1.Columns(0).HeaderText = ""
        cnn.Close()
    End Sub

    Public Sub cargardatos()
        If InStr(txtQR.Text, ",") Then
            Dim cadenacompleta As String
            Dim aux
            cadenacompleta = txtQR.Text
            aux = Split(cadenacompleta, ",")
            norden = aux(9)

            txtQR.Text = "Escanee el Codigo QR o Ingrese N° Orden"

        Else
            norden = txtQR.Text
        End If

    End Sub


    Private Sub txtQR_KeyDown(sender As Object, e As System.Windows.Forms.KeyEventArgs) Handles txtQR.KeyDown
        If e.KeyCode = Keys.Enter Then
            cargardatos()
            realizarDespacho()
        End If
    End Sub

    Private Sub frmRealizarDespacho_FormClosing(sender As Object, e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        cnn.Open()
        Dim cmd1 As New SqlCommand("sp_SalidaDespacho", cnn)
        cmd1.CommandType = CommandType.StoredProcedure
        cmd1.Parameters.AddWithValue("@ID_Despacho", idDespacho)
        cmd1.Parameters.AddWithValue("@estado", "1")

        cmd1.ExecuteNonQuery()

        Form1.cargardataFechaSalida()
        cnn.Close()
    End Sub
    Private Sub frmRealizarDespacho_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load

    End Sub
    Public Sub realizarDespacho()
        cnn.Open()
        Dim Sql As String = "select tbl_Despacho.id_Despacho, tbl_Objeto.id_Objeto, tbl_capacidad.id_Capacidad from tbl_Despacho inner join tbl_Objeto on tbl_Despacho.id_Objeto=tbl_Objeto.id_Objeto inner join tbl_Cubiculo on tbl_Objeto.id_Cubiculo = tbl_Cubiculo.id_Cubiculo inner join tbl_Capacidad on tbl_Capacidad.id_Capacidad = tbl_Cubiculo.capacidad where tbl_Objeto.n_Orden = @aux"
        Dim command As New SqlCommand(Sql, cnn)
        command.Parameters.AddWithValue("@aux", norden)

        Dim reader As SqlDataReader = command.ExecuteReader()

        If reader.Read() Then
            idDespacho = Convert.ToString(reader("id_Despacho"))
            idCapacidad = Convert.ToString(reader("id_Capacidad"))
            idObjeto = Convert.ToString(reader("id_Objeto"))
        End If
        reader.Close()

        Dim cmd1 As New SqlCommand("sp_SalidaDespacho", cnn)
        cmd1.CommandType = CommandType.StoredProcedure
        cmd1.Parameters.AddWithValue("@ID_Despacho", idDespacho)
        cmd1.Parameters.AddWithValue("@estado", "3")

        cmd1.ExecuteNonQuery()

        Form1.cargardataFechaSalida()

        cnn.Close()
        cargardata()
    End Sub

    Private Sub Button1_Click(sender As System.Object, e As System.EventArgs) Handles Button1.Click
        cnn.Open()

        Dim cmd2 As New SqlCommand("sp_ModificarObjeto", cnn)
        cmd2.CommandType = CommandType.StoredProcedure
        cmd2.Parameters.AddWithValue("@ID_Objeto", idObjeto)
        cmd2.Parameters.AddWithValue("@id_Cubiculo", "35")

        cmd2.ExecuteNonQuery()

        Dim cmd3 As New SqlCommand("sp_ModificarCapacidad", cnn)
        cmd3.CommandType = CommandType.StoredProcedure
        cmd3.Parameters.AddWithValue("@ID_capacidad", idCapacidad)
        cmd3.Parameters.AddWithValue("@cap_disponible", DataGridView1.Rows(DataGridView1.CurrentRow.Index).Cells(4).Value)

        cmd3.ExecuteNonQuery()

        Dim cmd1 As New SqlCommand("sp_SalidaDespacho", cnn)
        cmd1.CommandType = CommandType.StoredProcedure
        cmd1.Parameters.AddWithValue("@ID_Despacho", idDespacho)
        cmd1.Parameters.AddWithValue("@estado", "4")

        cmd1.ExecuteNonQuery()

        Form1.cargardataFechaSalida()
        cnn.Close()
    End Sub
End Class