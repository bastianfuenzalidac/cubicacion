Imports System.Data.SqlClient
Imports System.Text.RegularExpressions
Imports System.Configuration
Public Class frmProveedores
    Protected configuracion As ConnectionStringSettings = ConfigurationManager.ConnectionStrings("cnn")
    Dim cnn As New SqlConnection(configuracion.ConnectionString)
    Dim fecha As String = DateAndTime.Today.Date
    Dim hora As String = TimeOfDay

    Private Function Exists(ByVal Id As String) As Boolean
        Dim sql As String = "SELECT COUNT(*)  FROM tbl_Proveedor WHERE Nombre = @Nombre"


        Dim command As New SqlCommand(sql, cnn)
        command.Parameters.AddWithValue("@Nombre", Id)

        cnn.Open()

        Dim count As Integer = Convert.ToInt32(command.ExecuteScalar())

        If count = 0 Then
            Return False
        Else
            Return True

        End If
        cnn.Close()
    End Function

    Public Sub cargarData()
        cnn.Open()
        Dim consulta As String = "Select * From tbl_Proveedor"

        Dim cmd As New SqlCommand(consulta, cnn)

        cmd.CommandType = CommandType.Text

        Dim da As New SqlDataAdapter(cmd)

        Dim dt As New DataTable

        da.Fill(dt)

        'DataGridView1.DataSource = dt
        'DataGridView1.Columns(0).Width = 30
        'DataGridView1.Columns(1).Width = 70
        'DataGridView1.Columns(2).Width = 70
        ''DataGridView1.Columns(5).Width = 88
        'DataGridView1.Columns(0).Visible = False
        'DataGridView1.Columns(1).Visible = False
        'DataGridView1.Columns(2).Visible = False
        'DataGridView1.Columns(6).Visible = False
        'DataGridView1.Columns(7).Visible = False
        'DataGridView1.Columns(8).Visible = False
        With DataGridView1
            ' alternar colores  
            .RowsDefaultCellStyle.BackColor = Color.Orange

            .AlternatingRowsDefaultCellStyle.BackColor = Color.White
            .ForeColor = Color.Black
            .DefaultCellStyle.SelectionForeColor = Color.Black
            .DefaultCellStyle.SelectionBackColor = Color.LightSteelBlue

        End With
        DataGridView1.ClearSelection()
        cnn.Close()
    End Sub

    Public Function validar_Mail(ByVal sMail As String) As Boolean
        ' retorna true o false   
        Return Regex.IsMatch(sMail, _
                  "^[_a-z0-9-]+(\.[_a-z0-9-]+)*@[a-z0-9-]+(\.[a-z0-9-]+)*(\.[a-z]{2,4})$")
    End Function

    Public Sub borrardatos()
        txtNombre.Text = ""
        txtEmail.Text = ""
        txtDireccion.Text = ""
        txtTelefono.Text = ""
        txtWeb.Text = ""
    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        Try
            If cnn.State = ConnectionState.Open Then
                cnn.Close()
            End If
            cnn.Close()
            If txtNombre.Text = "" Or txtTelefono.Text = "" Then
                MsgBox("Ingrese Nombre y Telefono del Proveedor", MsgBoxStyle.Information, "Faltan Datos")
                Return
            End If

            If txtEmail.Text <> "" Then
                If validar_Mail(LCase(txtEmail.Text)) = False Then
                    MessageBox.Show("Dirección de correo electronico no valida, el correo debe tener el formato: nombre@dominio.com", "Validación de correo electronico", MessageBoxButtons.OK, MessageBoxIcon.Information)
                    txtEmail.Focus()
                    cnn.Close()
                    Return
                End If
            End If

            If txtTelefono.Text <> "" Then
                If Exists(txtTelefono.Text) Then
                    MsgBox("Nombre ya registrado", MsgBoxStyle.Exclamation, "Nombre Existente")
                    cnn.Close()
                    Return
                Else
                    Dim cmd As New SqlCommand("sp_CrearProveedor", cnn)

                    cmd.CommandType = CommandType.StoredProcedure

                    cmd.Parameters.AddWithValue("@Fecha", Date.Now)
                    cmd.Parameters.AddWithValue("@Nombre", txtNombre.Text)
                    cmd.Parameters.AddWithValue("@Direccion", txtDireccion.Text)
                    cmd.Parameters.AddWithValue("@Telefono", txtTelefono.Text)
                    cmd.Parameters.AddWithValue("@Correo", txtEmail.Text)
                    cmd.Parameters.AddWithValue("@WEB", txtWeb.Text)

                    cmd.ExecuteNonQuery()
                    cnn.Close()
                    borrardatos()
                    MsgBox("Proveedor: " + txtNombre.Text + " Guardado con exito", MsgBoxStyle.Information, "Exito al Guardar")
                    cargarData()
                End If
            End If
        Catch ex As Exception
            MsgBox(ex.Message)
            cnn.Close()
        End Try
    End Sub

    Private Sub frmProveedores_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        cargarData()
        Button2.PerformClick()
    End Sub

    Private Sub DataGridView1_SelectionChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles DataGridView1.SelectionChanged
        Try
            If DataGridView1.Rows(DataGridView1.CurrentRow.Index).Cells(2).Value Is System.DBNull.Value Then
            Else
                txtNombre.Text = DataGridView1.Rows(DataGridView1.CurrentRow.Index).Cells(2).Value
            End If

            If DataGridView1.Rows(DataGridView1.CurrentRow.Index).Cells(3).Value Is System.DBNull.Value Then
            Else
                txtDireccion.Text = DataGridView1.Rows(DataGridView1.CurrentRow.Index).Cells(3).Value
            End If

            If DataGridView1.Rows(DataGridView1.CurrentRow.Index).Cells(4).Value Is System.DBNull.Value Then
            Else
                txtTelefono.Text = DataGridView1.Rows(DataGridView1.CurrentRow.Index).Cells(4).Value
            End If

            If DataGridView1.Rows(DataGridView1.CurrentRow.Index).Cells(5).Value Is System.DBNull.Value Then
            Else
                txtEmail.Text = DataGridView1.Rows(DataGridView1.CurrentRow.Index).Cells(5).Value
            End If

            If DataGridView1.Rows(DataGridView1.CurrentRow.Index).Cells(6).Value Is System.DBNull.Value Then
            Else
                txtWeb.Text = DataGridView1.Rows(DataGridView1.CurrentRow.Index).Cells(6).Value
            End If

            If DataGridView1.Rows(DataGridView1.CurrentRow.Index).Cells(0).Value Is System.DBNull.Value Then
            Else
                Label2.Text = DataGridView1.Rows(DataGridView1.CurrentRow.Index).Cells(0).Value
            End If


            If Exists(txtNombre.Text) Then
                Button3.Visible = True
                Button4.Visible = True
                Button1.Visible = False
                cnn.Close()
                Return
            Else
                Button3.Visible = False
                Button4.Visible = False
                Button1.Visible = True
                cnn.Close()
                Return
            End If

        Catch ex As Exception

        End Try


    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        borrardatos()
        If Exists(txtNombre.Text) Then
            Button3.Visible = True
            Button4.Visible = True
            Button1.Visible = False
            cnn.Close()
            Return
        Else
            Button3.Visible = False
            Button4.Visible = False
            Button1.Visible = True
            cnn.Close()
            Return
        End If
    End Sub

    Private Sub TextBox1_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TextBox1.TextChanged
        Try
            If TextBox1.Text = "" Then

                cargarData()
                Button2.PerformClick()
            Else
                cnn.Open()
                ' Creamos la consulta de combinación
                Dim sql As String = "SELECT * FROM tbl_Proveedor WHERE Nombre like '%" + TextBox1.Text.Trim + "%'" ' NUM_REF='" & TextBox1.Text & "' )"
                ' Creamos un adaptador de datos
                Dim da As SqlClient.SqlDataAdapter = New SqlClient.SqlDataAdapter(sql, cnn)
                ' Creamos un objeto DataTable
                Dim dt As DataTable = New DataTable("Nombre")
                ' Rellenamos el objeto DataTable
                da.Fill(dt)
                ' Y por último, mostramos los datos en el control DataGridView

                Me.DataGridView1.DataSource = dt

                'DataGridView1.DataSource = dt
                'DataGridView1.Columns(0).Width = 30
                'DataGridView1.Columns(1).Width = 70
                'DataGridView1.Columns(2).Width = 70
                ''DataGridView1.Columns(5).Width = 88
                'DataGridView1.Columns(0).Visible = False
                'DataGridView1.Columns(1).Visible = False
                'DataGridView1.Columns(2).Visible = False
                With DataGridView1
                    ' alternar colores  
                    .RowsDefaultCellStyle.BackColor = Color.Orange

                    .AlternatingRowsDefaultCellStyle.BackColor = Color.White
                    .ForeColor = Color.Black
                    .DefaultCellStyle.SelectionForeColor = Color.Black
                    .DefaultCellStyle.SelectionBackColor = Color.LightSteelBlue

                End With
                DataGridView1.ClearSelection()
            End If
            cnn.Close()
            If Exists(txtNombre.Text) Then
                Button3.Visible = True
                Button4.Visible = True
                Button1.Visible = False
                cnn.Close()
                Return
            Else
                Button3.Visible = False
                Button4.Visible = False
                Button1.Visible = True
                cnn.Close()
                Return
            End If
        Catch ex As Exception
            MsgBox(ex.Message, 48)
            cnn.Close()
        End Try
    End Sub

    Private Sub TextBox2_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TextBox2.TextChanged
        Try
            If TextBox2.Text = "" Then

                cargarData()
                Button2.PerformClick()
            Else
                cnn.Open()
                ' Creamos la consulta de combinación
                Dim sql As String = "SELECT * FROM tbl_Proveedor WHERE Telefono like '%" + TextBox2.Text.Trim + "%'" ' NUM_REF='" & TextBox1.Text & "' )"
                ' Creamos un adaptador de datos
                Dim da As SqlClient.SqlDataAdapter = New SqlClient.SqlDataAdapter(sql, cnn)
                ' Creamos un objeto DataTable
                Dim dt As DataTable = New DataTable("Telefono")
                ' Rellenamos el objeto DataTable
                da.Fill(dt)
                ' Y por último, mostramos los datos en el control DataGridView

                Me.DataGridView1.DataSource = dt
                DataGridView1.DataSource = dt
                'DataGridView1.Columns(0).Width = 30
                'DataGridView1.Columns(1).Width = 70
                'DataGridView1.Columns(2).Width = 70
                'DataGridView1.Columns(5).Width = 88
                'DataGridView1.Columns(0).Visible = False
                'DataGridView1.Columns(1).Visible = False
                'DataGridView1.Columns(2).Visible = False
                'DataGridView1.Columns(6).Visible = False
                'DataGridView1.Columns(7).Visible = False
                'DataGridView1.Columns(8).Visible = False
                With DataGridView1
                    ' alternar colores  
                    .RowsDefaultCellStyle.BackColor = Color.Orange

                    .AlternatingRowsDefaultCellStyle.BackColor = Color.White
                    .ForeColor = Color.Black
                    .DefaultCellStyle.SelectionForeColor = Color.Black
                    .DefaultCellStyle.SelectionBackColor = Color.LightSteelBlue

                End With
                DataGridView1.ClearSelection()
            End If
            cnn.Close()
            If Exists(txtNombre.Text) Then
                Button3.Visible = True
                Button4.Visible = True
                Button1.Visible = False
                cnn.Close()
                Return
            Else
                Button3.Visible = False
                Button4.Visible = False
                Button1.Visible = True
                cnn.Close()
                Return
            End If
        Catch ex As Exception
            MsgBox(ex.Message, 48)
            cnn.Close()
        End Try
    End Sub


    Private Sub Button4_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button4.Click
        Try

            Dim Pregunta As Integer

            If txtNombre.Text <> "" Then
                Pregunta = MsgBox("Desea Eliminar el Proveedor: " + txtNombre.Text, vbYesNo + MsgBoxStyle.Information, "Confirmación")

                If Pregunta = vbYes Then
                    cnn.Open()

                    Dim cmd As New SqlCommand("sp_ElminarProveedor", cnn)
                    cmd.CommandType = CommandType.StoredProcedure

                    cmd.Parameters.AddWithValue("@Nombre", txtNombre.Text)

                    cmd.ExecuteNonQuery()
                    MsgBox("Proveedor Eliminado Correctamente", MsgBoxStyle.Information, "Atención")

                    Button2.PerformClick()
                    cargarData()

                    cnn.Close()
                    Return

                End If

            Else
                MsgBox("Seleccione Proveedor a Eliminar", MsgBoxStyle.Information, "Atención")

            End If

        Catch ex As Exception
            MsgBox("Proveedor no se puede eliminar ya que posee objetos asociados.", MsgBoxStyle.Information, "Imposible Eliminar")
            cnn.Close()
        End Try
    End Sub

    Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button3.Click
        Try
            cnn.Open()

            Dim cmd As New SqlCommand("sp_ModificarProveedores", cnn)
            cmd.CommandType = CommandType.StoredProcedure

            cmd.Parameters.AddWithValue("@ID_Proveedor", Label2.Text)
            cmd.Parameters.AddWithValue("@Nombre", txtNombre.Text)
            cmd.Parameters.AddWithValue("@Direccion", txtDireccion.Text)
            cmd.Parameters.AddWithValue("@Telefono", txtTelefono.Text)
            cmd.Parameters.AddWithValue("@Correo", txtEmail.Text)
            cmd.Parameters.AddWithValue("@WEB", txtWeb.Text)

            cmd.ExecuteNonQuery()

            MsgBox("Proveedor: " + txtNombre.Text + " Modificado con Exito", MsgBoxStyle.Information, "Actualizar Proveedor")
            cnn.Close()
            cargarData()
            Button2.PerformClick()

        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub

    Private Sub txtTelefono_KeyPress(sender As Object, e As System.Windows.Forms.KeyPressEventArgs) Handles txtTelefono.KeyPress
        Try
            Dim Sep As Char
            If Not (Char.IsNumber(e.KeyChar) Or e.KeyChar.Equals(Sep) Or Char.IsControl(e.KeyChar)) Then e.Handled = True
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical, "Atención")
        End Try
    End Sub

    Private Sub txtTelefono_TextChanged(sender As System.Object, e As System.EventArgs) Handles txtTelefono.TextChanged

    End Sub
End Class