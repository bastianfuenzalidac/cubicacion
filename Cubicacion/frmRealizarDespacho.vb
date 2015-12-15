Imports System.Data.SqlClient
Imports System.Text.RegularExpressions
Imports System.Configuration
Public Class frmRealizarDespacho
    Dim norden, idDespacho, idCapacidad, idObjeto As String
    Protected configuracion As ConnectionStringSettings = ConfigurationManager.ConnectionStrings("cnn")
    Dim cnn As New SqlConnection(configuracion.ConnectionString)

    Private Function Exists(ByVal Id As String) As Boolean
        Dim sql As String = "SELECT COUNT(*)  FROM tbl_objeto WHERE n_orden = @n_orden and id_cubiculo <> '35'"


        Dim command As New SqlCommand(sql, cnn)
        command.Parameters.AddWithValue("@n_orden", Id)

        cnn.Open()

        Dim count As Integer = Convert.ToInt32(command.ExecuteScalar())

        If count = 0 Then
            Return False
        Else
            Return True

        End If
        cnn.Close()
    End Function

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
        DataGridView1.Columns(4).Visible = False
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
            txtQR.SelectAll()

        Else
            norden = txtQR.Text
        End If

    End Sub


    Private Sub txtQR_KeyDown(sender As Object, e As System.Windows.Forms.KeyEventArgs) Handles txtQR.KeyDown
        cargardatos()
        If e.KeyCode = Keys.Enter Then
            If Exists(norden) Then

                realizarDespacho()
            Else
                MsgBox("Orden no se encuentra registrada en la base de datos", MsgBoxStyle.Information, "Atención")
                txtQR.Text = "Escanee el Codigo QR o Ingrese N° Orden"
                txtQR.SelectAll()
                cnn.Close()
                Return
            End If
        End If
    End Sub

    Private Sub frmRealizarDespacho_FormClosing(sender As Object, e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        If cnn.State = ConnectionState.Open Then
            cnn.Close()
        End If
        cnn.Open()
        If norden <> "" Then

            Dim cmd1 As New SqlCommand("sp_SalidaDespacho", cnn)
            cmd1.CommandType = CommandType.StoredProcedure
            cmd1.Parameters.AddWithValue("@ID_Despacho", idDespacho)
            cmd1.Parameters.AddWithValue("@estado", "1")

            cmd1.ExecuteNonQuery()

            Form1.cargardataFechaSalida()
            cnn.Close()
        End If
    End Sub

    Public Sub realizarDespacho()
        If cnn.State = ConnectionState.Open Then
            cnn.Close()
        End If
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
        Try
            If txtQR.Text <> "Escanee el Codigo QR o Ingrese N° Orden" Then
                If Exists(txtQR.Text) Then
                    Dim pregunta As Integer
                    pregunta = MsgBox("Salida de despacho, Proveedor: " & DataGridView1.Rows(DataGridView1.CurrentRow.Index).Cells(0).Value & " Correcta?", MsgBoxStyle.YesNo, "Atención")
                    If pregunta = vbYes Then
                        If cnn.State = ConnectionState.Open Then
                            cnn.Close()
                        End If
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


                        Dim cmd As New SqlCommand("sp_CrearSalidaDespacho", cnn)
                        cmd.CommandType = CommandType.StoredProcedure
                        cmd.Parameters.AddWithValue("@ID_Empleado", clsLogin.IdUsuario)
                        cmd.Parameters.AddWithValue("@fecha", Date.Now)
                        cmd.Parameters.AddWithValue("@observacion", txtObservacion.Text)
                        cmd.Parameters.AddWithValue("@id_despacho", idDespacho)

                        cmd.ExecuteNonQuery()

                        Form1.cargardataFechaSalida()
                        Form1.cargardataEstadoPiso()
                        cnn.Close()
                        MsgBox("Exito al registrar despacho", MsgBoxStyle.Information, "Atención")
                        DataGridView1.DataSource = Nothing
                        txtQR.Text = "Escanee el Codigo QR o Ingrese N° Orden"
                        txtQR.SelectAll()
                    End If
                Else
                    MsgBox("Orden no se encuentra registrada en la base de datos", MsgBoxStyle.Information, "Atención")
                    txtQR.Text = "Escanee el Codigo QR o Ingrese N° Orden"
                    txtQR.SelectAll()
                    cnn.Close()
                    Return
                End If

            Else
                MsgBox("Escanee codigo QR o ingrese n° de orden", MsgBoxStyle.Information, "Faltan Datos")
                Return
            End If
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub

    Private Sub txtQR_Click(sender As Object, e As System.EventArgs) Handles txtQR.Click
        txtQR.SelectAll()
    End Sub

    Private Sub frmRealizarDespacho_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load
        lblEmpleado.Text = clsLogin.NombreUsuario
    End Sub
End Class