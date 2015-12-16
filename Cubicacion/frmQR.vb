Imports System.Data.SqlClient
Imports System.Text.RegularExpressions
Imports System.Configuration
Imports System.Data.SqlTypes
Imports System.Collections.Generic
Public Class frmQR
    Dim idCapacidad, idProveedor, idObjeto As String
    Dim auxancho As String
    Protected configuracion As ConnectionStringSettings = ConfigurationManager.ConnectionStrings("cnn")
    Dim cnn As New SqlConnection(configuracion.ConnectionString)

    Private Function Exists(ByVal Id As String) As Boolean
        Dim sql As String = "SELECT COUNT(*) FROM tbl_objeto WHERE n_orden = @n_orden"


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

    Public Sub cargardataCubiculo()
        Try
            If txtAlto.Text = "" Or txtAncho.Text = "" Or txtLargo.Text = "" Or txtLargo.Text = "" Or txtPeso.Text = "" Or txtTemperatura.Text = "" Or txtProveedor.Text = "" Or txtOrden.Text = "" Or txtFechaDespacho.Text = "" Or txtCant.Text = "" Then
                MsgBox("Ingrese todos los campos solicitados", MsgBoxStyle.Information, "Atención")
            Else
                Dim command As SqlCommand
                Dim adapter As SqlDataAdapter
                Dim dtTable As DataTable
                cnn.Open()
                'Indico el SP que voy a utilizar
                command = New SqlCommand("sp_CubiculoOptimo", cnn)
                command.CommandType = CommandType.StoredProcedure
                command.Parameters.AddWithValue("@alto", txtAlto.Text)
                command.Parameters.AddWithValue("@ancho", txtAncho.Text)
                command.Parameters.AddWithValue("@largo", txtLargo.Text)
                command.Parameters.AddWithValue("@peso", txtPeso.Text)
                command.Parameters.AddWithValue("@temperatura", txtTemperatura.Text)
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

                Dim Columna, Fila, Ancho, Largo, Alto, Peso, Temperatura As String

                If DataGridView1.Rows.Count = 0 Then
                    Panel4.Visible = True
                    Panel3.Visible = False
                    lblEstado.Visible = False
                    PictureBox1.Visible = False
                    ' Button2.Enabled = False
                    lblCol.Text = "PISO"
                    lblFil.Text = 0
                Else
                    idCapacidad = DataGridView1.Rows(DataGridView1.CurrentRow.Index).Cells(0).Value
                    Columna = DataGridView1.Rows(DataGridView1.CurrentRow.Index).Cells(1).Value
                    Fila = DataGridView1.Rows(DataGridView1.CurrentRow.Index).Cells(2).Value
                    Ancho = DataGridView1.Rows(DataGridView1.CurrentRow.Index).Cells(3).Value
                    Largo = DataGridView1.Rows(DataGridView1.CurrentRow.Index).Cells(4).Value
                    Alto = DataGridView1.Rows(DataGridView1.CurrentRow.Index).Cells(5).Value
                    Peso = DataGridView1.Rows(DataGridView1.CurrentRow.Index).Cells(6).Value
                    Temperatura = DataGridView1.Rows(DataGridView1.CurrentRow.Index).Cells(7).Value
                    lblCol.Text = Columna
                    lblFil.Text = Fila
                    Panel3.Visible = True
                    Panel4.Visible = False
                    lblColumna.Text = Columna
                    lblFila.Text = Fila
                    lblEstado.Visible = True
                    PictureBox1.Visible = False
                    lblEstado.Text = Chr(13) + "Ancho: " + Ancho _
                         + Chr(13) + "Largo: " + Largo _
                        + Chr(13) + "Alto: " + Alto _
                        + Chr(13) + "Peso: " + Peso _
                         + Chr(13) + "Temperatura: " + Temperatura _
                         + Chr(13) + "Disponible despues de ingreso: " & CInt(Ancho - txtAncho.Text)

                End If

            End If

        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub

    Private Sub frmQR_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load
        lblUsuario.Text = clsLogin.NombreUsuario
        lblFecha.Text = Date.Today

    End Sub

    Private Sub Button3_Click(sender As System.Object, e As System.EventArgs) Handles Button3.Click
        If txtQR.Text = "Escanee el Codigo QR" Then
            MsgBox("Escanee codigo QR", MsgBoxStyle.Information, "Faltan Datos")
            Return
        Else
            Dim cadenacompleta As String
            Dim aux
            cadenacompleta = txtQR.Text
            aux = Split(cadenacompleta, ",")

            txtAlto.Text = aux(0)
            txtLargo.Text = aux(1)
            txtAncho.Text = aux(2)
            txtPeso.Text = aux(4)
            txtTemperatura.Text = aux(3)
            txtCant.Text = aux(5)
            lblFecha.Text = aux(6)
            lblNombre.Text = aux(7)
            txtProveedor.Text = aux(8)
            txtOrden.Text = aux(9)


            cargardataCubiculo()
            txtQR.Text = "Escanee el Codigo QR"
        End If
    End Sub

    Private Sub txtQR_Click(sender As Object, e As System.EventArgs) Handles txtQR.Click
        txtQR.SelectAll()
    End Sub

    Private Sub txtQR_KeyDown(sender As Object, e As System.Windows.Forms.KeyEventArgs) Handles txtQR.KeyDown
        If e.KeyCode = Keys.Enter Then
            Button3.PerformClick()

        End If
    End Sub

    Private Sub Button1_Click(sender As System.Object, e As System.EventArgs) Handles Button1.Click
        txtAlto.Text = ""
        txtLargo.Text = ""
        txtAncho.Text = ""
        txtPeso.Text = ""
        txtTemperatura.Text = ""
        txtCant.Text = ""
        lblNombre.Text = ""
        txtProveedor.Text = ""
        txtOrden.Text = ""
        txtObservacion.Text = ""
        txtQR.Text = "Escanee el Codigo QR"
        txtFechaDespacho.Text = ""
        Button2.Enabled = False
        PictureBox1.Visible = True
        Panel3.Visible = False
        Panel4.Visible = False
        lblEstado.Visible = False
    End Sub

  
    Private Sub Button2_Click(sender As System.Object, e As System.EventArgs) Handles Button2.Click
        Try
            If Exists(txtOrden.Text) Then
                MsgBox("Numero de orden, ya registrado, intente nuevamente", MsgBoxStyle.Information, "Atención")
                cnn.Close()
                Return
            End If
            If cnn.State = ConnectionState.Open Then
                cnn.Close()
            End If
            cnn.Open()



            Dim Sql As String = "Select id_cubiculo From tbl_Columna inner join tbl_Cubiculo on tbl_Columna.id_Columna = tbl_Cubiculo.id_Columna inner join tbl_Fila on tbl_Fila.id_Fila = tbl_Cubiculo.id_Fila where numero_Columna = @col and numero_Fila = @fil"

            Dim command As New SqlCommand(Sql, cnn)
            command.Parameters.AddWithValue("@col", lblCol.Text)
            command.Parameters.AddWithValue("@fil", lblFil.Text)

            Dim reader As SqlDataReader = command.ExecuteReader()

            If reader.Read() Then
                lblIdCub.Text = Convert.ToString(reader("id_cubiculo"))
            End If
            reader.Close()
            ' Leer id Proveedor

            Dim Sql1 As String = "Select id_Proveedor from tbl_Proveedor where Nombre like '%" & txtProveedor.Text & "%'"

            Dim command1 As New SqlCommand(Sql1, cnn)
            Dim reader1 As SqlDataReader = command1.ExecuteReader()
            If reader1.Read() Then
                idProveedor = (reader1.GetValue(0))
            Else
                idProveedor = "5"
            End If
            reader1.Close()
         

            'Creacion de capacidad
            If lblCol.Text <> "PISO" Then

                Dim cmd As New SqlCommand("sp_CrearObjeto", cnn)

                cmd.CommandType = CommandType.StoredProcedure

                cmd.Parameters.AddWithValue("@fecha_Ingreso", DateTime.Now)
                cmd.Parameters.AddWithValue("@id_EmpleadoIngreso", clsLogin.IdUsuario)
                cmd.Parameters.AddWithValue("@fecha_Modificacion", DateTime.Now)
                cmd.Parameters.AddWithValue("@n_Orden", txtOrden.Text)
                cmd.Parameters.AddWithValue("@id_Proveedor", idProveedor)
                cmd.Parameters.AddWithValue("@ancho", txtAncho.Text)
                cmd.Parameters.AddWithValue("@largo", txtLargo.Text)
                cmd.Parameters.AddWithValue("@alto", txtAlto.Text)
                cmd.Parameters.AddWithValue("@peso", txtPeso.Text)
                cmd.Parameters.AddWithValue("@temperatura", txtTemperatura.Text)
                'cmd.Parameters.AddWithValue("@id_EmpleadoModificacion", clsLogin.IdUsuario)
                cmd.Parameters.AddWithValue("@nombre_Objeto", "PRUEBAS DE INGRESO")
                cmd.Parameters.AddWithValue("@descripcion", txtObservacion.Text)
                cmd.Parameters.AddWithValue("@cantidad", txtCant.Text)
                cmd.Parameters.AddWithValue("@id_cubiculo", lblIdCub.Text)
                cmd.Parameters.AddWithValue("@id_qr", "1")

                cmd.ExecuteNonQuery()
                MsgBox("Ingresado", MsgBoxStyle.Information, "Exito al Guardar")

                Dim Sql2 As String = "Select tbl_Capacidad.cap_Disponible as nancho From tbl_Columna inner join tbl_Cubiculo on tbl_Columna.id_Columna = tbl_Cubiculo.id_Columna inner join tbl_Fila on tbl_Fila.id_Fila = tbl_Cubiculo.id_Fila inner join tbl_capacidad on tbl_Cubiculo.capacidad = tbl_Capacidad.id_Capacidad where numero_Columna = @col and numero_Fila = @fil"

                Dim command2 As New SqlCommand(Sql2, cnn)
                command2.Parameters.AddWithValue("@col", lblCol.Text)
                command2.Parameters.AddWithValue("@fil", lblFil.Text)

                Dim reader2 As SqlDataReader = command2.ExecuteReader()

                If reader2.Read() Then
                    auxancho = Convert.ToString(reader2("nancho"))
                End If
                reader2.Close()

                Dim cmd1 As New SqlCommand("sp_ModificarCapacidad", cnn)
                cmd1.CommandType = CommandType.StoredProcedure
                auxancho = CInt(auxancho - txtAncho.Text)
                cmd1.Parameters.AddWithValue("@ID_Capacidad", idCapacidad)
                MsgBox(auxancho)
                cmd1.Parameters.AddWithValue("@cap_Disponible", auxancho)

                cmd1.ExecuteNonQuery()


                Dim Sql3 As String = "Select top 1 isnull (id_Objeto,0 ) from tbl_Objeto order by id_Objeto desc"

                Dim command3 As New SqlCommand(Sql3, cnn)

                Dim reader3 As SqlDataReader = command3.ExecuteReader()
                reader3.Read()
                idObjeto = (reader3.GetValue(0))
                reader3.Close()
                MsgBox(idObjeto)

                Dim cmd3 As New SqlCommand("sp_CrearDespacho", cnn)

                cmd3.CommandType = CommandType.StoredProcedure

                cmd3.Parameters.AddWithValue("@id_EmpleadoIngreso", 1)
                cmd3.Parameters.AddWithValue("@descripcion", "Prueba Despacho")
                cmd3.Parameters.AddWithValue("@cliente", 1)
                cmd3.Parameters.AddWithValue("@id_Objeto", idObjeto)
                cmd3.Parameters.AddWithValue("@fecha_salida", Date.Now)
                cmd3.Parameters.AddWithValue("@estado", 1)

                cmd3.ExecuteNonQuery()


                Dim cmd4 As New SqlCommand("sp_CrearBitacora", cnn)

                cmd4.CommandType = CommandType.StoredProcedure

                cmd4.Parameters.AddWithValue("@fecha_Ingreso", Date.Now)
                cmd4.Parameters.AddWithValue("@id_EmpleadoIngreso", clsLogin.IdUsuario)
                cmd4.Parameters.AddWithValue("@Detalle", "PROVEEDOR: " & txtProveedor.Text & ", NUMERO DE ORDEN: " & txtOrden.Text & Chr(13) _
                                             & "COLUMNA: " & lblCol.Text & ", FILA: " & lblFil.Text)
                cmd4.Parameters.AddWithValue("@Tipo", 4)
                cmd4.ExecuteNonQuery()

                cnn.Close()

            Else
                Dim cmd As New SqlCommand("sp_CrearObjeto", cnn)

                cmd.CommandType = CommandType.StoredProcedure

                cmd.Parameters.AddWithValue("@fecha_Ingreso", DateTime.Now)
                cmd.Parameters.AddWithValue("@id_EmpleadoIngreso", clsLogin.IdUsuario)
                cmd.Parameters.AddWithValue("@fecha_Modificacion", DateTime.Now)
                cmd.Parameters.AddWithValue("@n_Orden", txtOrden.Text)
                cmd.Parameters.AddWithValue("@id_Proveedor", idProveedor)
                cmd.Parameters.AddWithValue("@ancho", txtAncho.Text)
                cmd.Parameters.AddWithValue("@largo", txtLargo.Text)
                cmd.Parameters.AddWithValue("@alto", txtAlto.Text)
                cmd.Parameters.AddWithValue("@peso", txtPeso.Text)
                cmd.Parameters.AddWithValue("@temperatura", txtTemperatura.Text)
                'cmd.Parameters.AddWithValue("@id_EmpleadoModificacion", clsLogin.IdUsuario)
                cmd.Parameters.AddWithValue("@nombre_Objeto", "PRUEBAS DE INGRESO")
                cmd.Parameters.AddWithValue("@descripcion", txtObservacion.Text)
                cmd.Parameters.AddWithValue("@cantidad", txtCant.Text)
                cmd.Parameters.AddWithValue("@id_cubiculo", "40")
                cmd.Parameters.AddWithValue("@id_qr", "1")

                cmd.ExecuteNonQuery()
                MsgBox("Ingresado", MsgBoxStyle.Information, "Exito al Guardar")

                Dim Sql3 As String = "Select top 1 isnull (id_Objeto,0 ) from tbl_Objeto order by id_Objeto desc"

                Dim command3 As New SqlCommand(Sql3, cnn)

                Dim reader3 As SqlDataReader = command3.ExecuteReader()
                reader3.Read()
                idObjeto = (reader3.GetValue(0))
                reader3.Close()
                MsgBox(idObjeto)

                Dim cmd3 As New SqlCommand("sp_CrearDespacho", cnn)

                cmd3.CommandType = CommandType.StoredProcedure

                cmd3.Parameters.AddWithValue("@id_EmpleadoIngreso", 1)
                cmd3.Parameters.AddWithValue("@descripcion", "Prueba Despacho")
                cmd3.Parameters.AddWithValue("@cliente", 1)
                cmd3.Parameters.AddWithValue("@id_Objeto", idObjeto)
                cmd3.Parameters.AddWithValue("@fecha_salida", Date.Now)
                cmd3.Parameters.AddWithValue("@estado", 1)

                cmd3.ExecuteNonQuery()

                Dim cmd4 As New SqlCommand("sp_CrearBitacora", cnn)

                cmd4.CommandType = CommandType.StoredProcedure

                cmd4.Parameters.AddWithValue("@fecha_Ingreso", Date.Now)
                cmd4.Parameters.AddWithValue("@id_EmpleadoIngreso", clsLogin.IdUsuario)
                cmd4.Parameters.AddWithValue("@Detalle", "PROVEEDOR: " & txtProveedor.Text & ", NUMERO DE ORDEN: " & txtOrden.Text & Chr(13) _
                                             & "OBJETO A PISO")
                cmd4.Parameters.AddWithValue("@Tipo", 4)
                cmd4.ExecuteNonQuery()

                cnn.Close()
               
            End If
            Form1.cargardataEstadoPiso()
            Form1.cargardataFechaSalida()
            Button1.PerformClick()
            lblEstado.Text = ""
        Catch ex As Exception
            MsgBox(ex.Message)
            cnn.Close()
        End Try
    End Sub

    Private Sub Button4_Click(sender As System.Object, e As System.EventArgs) Handles Button4.Click
        cargardataCubiculo()
    
    End Sub



    Private Sub txtProveedor_LostFocus(sender As Object, e As System.EventArgs) Handles txtProveedor.LostFocus
        Try
              cnn.Open()
            Dim Sql1 As String = "Select id_Proveedor from tbl_Proveedor where Nombre like '%" & txtProveedor.Text & "%'"

            Dim command1 As New SqlCommand(Sql1, cnn)
            Dim reader1 As SqlDataReader = command1.ExecuteReader()
            If reader1.Read() Then
                idProveedor = (reader1.GetValue(0))
            Else
                Dim pregunta As Integer
                pregunta = MsgBox("Proveedor no registrado, ¿Desea Registrarlo?", MsgBoxStyle.Information + MsgBoxStyle.YesNo, "Infromación")
                If pregunta = vbYes Then
                    frmProveedores.Show()
                    frmProveedores.txtNombre.Text = txtProveedor.Text
                    Me.Close()
                    Return
                End If
            End If
            reader1.Close()
            ' MsgBox(idProveedor)
            cnn.Close()
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub


    Private Sub txtQR_TextChanged(sender As System.Object, e As System.EventArgs) Handles txtQR.TextChanged

    End Sub

    Private Sub pbEditar1_Click(sender As System.Object, e As System.EventArgs) Handles pbEditar1.Click
        Panel1.Enabled = True
        pbOk1.Visible = True
        pbEditar1.Visible = False
        Button2.Enabled = False
    End Sub

    Private Sub pbEditar2_Click(sender As System.Object, e As System.EventArgs) Handles pbEditar2.Click
        Panel2.Enabled = True
        pbOk2.Visible = True
        pbEditar2.Visible = False
        Button2.Enabled = False
    End Sub

    Private Sub pbOk1_Click(sender As System.Object, e As System.EventArgs) Handles pbOk1.Click
        Panel1.Enabled = False
        pbOk1.Visible = False
        pbEditar1.Visible = True
        Button2.Enabled = True
        If txtAlto.Text = "" Or txtAncho.Text = "" Or txtLargo.Text = "" Or txtLargo.Text = "" Or txtPeso.Text = "" Or txtTemperatura.Text = "" Or txtProveedor.Text = "" Or txtOrden.Text = "" Or txtFechaDespacho.Text = "" Or txtCant.Text = "" Then
            Button2.Enabled = False
        Else
            Panel2.Enabled = False
            pbOk2.Visible = False
            pbEditar2.Visible = True
            Button2.Enabled = True
            Button4.PerformClick()

        End If

        If txtProveedor.Text = "" Or txtOrden.Text = "" Or txtFechaDespacho.Text = "" Or txtCant.Text = "" Then
            MsgBox("Ingrese todos los campos solicitados", MsgBoxStyle.Information, "Atención")
            Return
      
        End If

    End Sub

    Private Sub pbOk2_Click(sender As System.Object, e As System.EventArgs) Handles pbOk2.Click
        Panel2.Enabled = False
        pbOk2.Visible = False
        pbEditar2.Visible = True
        Button2.Enabled = True
        If txtAlto.Text = "" Or txtAncho.Text = "" Or txtLargo.Text = "" Or txtLargo.Text = "" Or txtPeso.Text = "" Or txtTemperatura.Text = "" Or txtProveedor.Text = "" Or txtOrden.Text = "" Or txtFechaDespacho.Text = "" Or txtCant.Text = "" Then
            Button2.Enabled = False
        Else
            Panel1.Enabled = False
            pbOk1.Visible = False
            pbEditar1.Visible = True
            Button2.Enabled = True
            Button4.PerformClick()

        End If

        If txtAlto.Text = "" Or txtAncho.Text = "" Or txtLargo.Text = "" Or txtLargo.Text = "" Or txtPeso.Text = "" Or txtTemperatura.Text = "" Then
            MsgBox("Ingrese todos los campos solicitados", MsgBoxStyle.Information, "Atención")
            Return
       
        End If
    End Sub

  
End Class