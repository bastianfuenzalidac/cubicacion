Imports System.Data.SqlClient
Imports System.Text.RegularExpressions
Imports System.Configuration
Imports System.Data.SqlTypes
Imports System.Collections.Generic
Public Class frmAdmCubiculos
    Dim estadoCubiculo As String
    Dim idColumna, idFila, idEstado As Integer
    Protected configuracion As ConnectionStringSettings = ConfigurationManager.ConnectionStrings("cnn")
    Dim cnn As New SqlConnection(configuracion.ConnectionString)

    Private Function Exists(ByVal idColumna As String, idFila As String) As Boolean
        Dim sql As String = "SELECT COUNT(*)  FROM tbl_Cubiculo WHERE id_Columna=@COL and id_Fila=@FIL"


        Dim command As New SqlCommand(sql, cnn)
        command.Parameters.AddWithValue("@COL", idColumna)
        command.Parameters.AddWithValue("@FIL", idFila)
        cnn.Open()

        Dim count As Integer = Convert.ToInt32(command.ExecuteScalar())

        If count = 0 Then
            Return False
        Else
            Return True

        End If
        cnn.Close()
    End Function

    Public Sub cargarComboColumna()

        If cbColumna.Text = "" Then

        End If
        cbColumna.Items.Clear()
        Dim variable As SqlDataReader
        Dim sql As New SqlCommand
        sql.CommandType = CommandType.Text
        sql.CommandText = ("select numero_Columna from tbl_Columna")
        sql.Connection = (cnn)
        cnn.Open()
        variable = sql.ExecuteReader()

        While variable.Read = True
            cbColumna.Items.Add(variable.Item(0).ToString.ToUpper)
        End While
        cnn.Close()

    End Sub

    Public Sub cargarComboFila()

        If cbFila.Text = "" Then

        End If
        cbFila.Items.Clear()
        Dim variable As SqlDataReader
        Dim sql As New SqlCommand
        sql.CommandType = CommandType.Text
        sql.CommandText = ("select numero_Fila from tbl_Fila")
        sql.Connection = (cnn)
        cnn.Open()
        variable = sql.ExecuteReader()

        While variable.Read = True
            cbFila.Items.Add(variable.Item(0).ToString.ToUpper)
        End While
        cnn.Close()
    End Sub

    Private Sub frmAdmCubiculos_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load
        cargarComboColumna()
        cargarComboFila()
        cbEstado.Items.Add("Disponible")
        cbEstado.Items.Add("Ocupado")
        cbEstado.Items.Add("Inactivo")
        cbEstado.Items.Add("En Espera")
        cbEstado.Items.Add("Otro")
    End Sub

    Private Sub btnGuardar_Click(sender As System.Object, e As System.EventArgs) Handles btnGuardar.Click
        Try
            If Exists(idColumna, idFila) Then
                MsgBox("Cubilo ya registrado", MsgBoxStyle.Information, "Cubiculo Existente")
                cnn.Close()
                Return
            Else
                If cnn.State = ConnectionState.Open Then
                    cnn.Close()
                End If
                cnn.Open()
                'Creacion de capacidad
                If txtAncho.Text <> "" Then

                    Dim cmd As New SqlCommand("sp_CrearCapacidad", cnn)

                    cmd.CommandType = CommandType.StoredProcedure

                    cmd.Parameters.AddWithValue("@ancho", txtAncho.Text)
                    cmd.Parameters.AddWithValue("@largo", txtLargo.Text)
                    cmd.Parameters.AddWithValue("@alto", txtAlto.Text)
                    cmd.Parameters.AddWithValue("@temperatura", txtTemperatura.Text)
                    cmd.Parameters.AddWithValue("@peso", txtPeso.Text)
                    cmd.Parameters.AddWithValue("@cap_Disponible", txtAncho.Text)
                    cmd.ExecuteNonQuery()


                    MsgBox("Capacidad Ingresada", MsgBoxStyle.Information, "Exito al Guardar")
                End If
                'Selecciona ultimo ingreso de capacidad
                Dim Sql As String = "Select top 1 isnull(id_capacidad,0) from tbl_Capacidad order by id_capacidad desc"

                Dim command As New SqlCommand(Sql, cnn)

                Dim reader As SqlDataReader = command.ExecuteReader()
                reader.Read()
                Dim id_capacidad = (reader.GetValue(0))
                reader.Close()

                'Creacion de cubiculo con id de capacidad
                If txtAncho.Text <> "" Then

                    Dim cmd As New SqlCommand("sp_CrearCubiculo", cnn)

                    cmd.CommandType = CommandType.StoredProcedure

                    cmd.Parameters.AddWithValue("@capacidad", id_capacidad)
                    cmd.Parameters.AddWithValue("@descripcion", txtDescripcion.Text)
                    cmd.Parameters.AddWithValue("@estado_Cubiculo", idEstado)
                    cmd.Parameters.AddWithValue("@id_Columna", idColumna)
                    cmd.Parameters.AddWithValue("@id_Fila", idFila)
                    cmd.Parameters.AddWithValue("@id_Qr", (SqlInt32.Null))
                    cmd.Parameters.AddWithValue("@fecha_Creacion", DateTime.Now)
                    cmd.Parameters.AddWithValue("@id_EmpCreacion", (SqlInt32.Null))
                    cmd.Parameters.AddWithValue("@fecha_Modificacion", DateTime.Now)
                    cmd.Parameters.AddWithValue("@id_EmpModificacion", (SqlInt32.Null))
                    cmd.ExecuteNonQuery()

                    Dim cmd4 As New SqlCommand("sp_CrearBitacora", cnn)

                    cmd4.CommandType = CommandType.StoredProcedure

                    cmd4.Parameters.AddWithValue("@fecha_Ingreso", Date.Now)
                    cmd4.Parameters.AddWithValue("@id_EmpleadoIngreso", clsLogin.IdUsuario)
                    cmd4.Parameters.AddWithValue("@Detalle", "COLUMNA: " & cbColumna.Text & " FILA: " & cbFila.Text & Chr(13) _
                                                 & "ANCHO: " & txtAncho.Text & Chr(13) _
                                                 & "LARGO: " & txtLargo.Text & Chr(13) _
                                                 & "ALTO: " & txtAlto.Text & Chr(13) _
                                                 & "PESO: " & txtPeso.Text & Chr(13) _
                                                 & "TEMPERATURA: " & txtTemperatura.Text & Chr(13) _
                                                 & "OBSERVACION: " & txtDescripcion.Text & Chr(13) _
                                                 & "ESTADO: " & cbEstado.Text)
                    cmd4.Parameters.AddWithValue("@Tipo", 1)
                    cmd4.ExecuteNonQuery()

                    cnn.Close()

                    MsgBox("Cubiculo Ingresado", MsgBoxStyle.Information, "Exito al Guardar")
                End If

            End If
        Catch ex As Exception
            MsgBox(ex.Message)
            cnn.Close()
        End Try
    End Sub

    Private Sub cbColumna_SelectedIndexChanged(sender As System.Object, e As System.EventArgs) Handles cbColumna.SelectedIndexChanged
        Try
            If cbColumna.Text = "" Then
            Else
                Dim Sql As String = "Select id_Columna From tbl_Columna where numero_Columna=@columna"

                Dim command As New SqlCommand(Sql, cnn)
                command.Parameters.AddWithValue("@columna", cbColumna.Text)
                cnn.Open()

                Dim reader As SqlDataReader = command.ExecuteReader()

                If reader.Read() Then
                    idColumna = Convert.ToString(reader("id_Columna"))
                Else
                End If
                cnn.Close()
            End If
        Catch ex As Exception

            MsgBox(ex.Message)
        End Try
    End Sub

    Private Sub cbFila_SelectedIndexChanged(sender As System.Object, e As System.EventArgs) Handles cbFila.SelectedIndexChanged
        Try
            If cbFila.Text = "" Then
            Else
                Dim Sql As String = "Select id_Fila From tbl_Fila where numero_Fila=@fila"

                Dim command As New SqlCommand(Sql, cnn)
                command.Parameters.AddWithValue("@fila", cbFila.Text)
                cnn.Open()

                Dim reader As SqlDataReader = command.ExecuteReader()

                If reader.Read() Then
                    idFila = Convert.ToString(reader("id_Fila"))
                Else
                End If
                cnn.Close()
            End If
        Catch ex As Exception

            MsgBox(ex.Message)
        End Try
    End Sub


    Private Sub cbEstado_SelectedIndexChanged(sender As System.Object, e As System.EventArgs) Handles cbEstado.SelectedIndexChanged
        If cbEstado.Text = "Disponible" Then
            idEstado = "1"
        ElseIf cbEstado.Text = "Ocupado" Then
            idEstado = "2"
        ElseIf cbEstado.Text = "Inactivo" Then
            idEstado = "3"
        ElseIf cbEstado.Text = "En Espera" Then
            idEstado = "4"
        ElseIf cbEstado.Text = "Otro" Then
            idEstado = "5"
        End If
    End Sub

    Private Sub Button2_Click(sender As System.Object, e As System.EventArgs) Handles Button2.Click

    End Sub
End Class