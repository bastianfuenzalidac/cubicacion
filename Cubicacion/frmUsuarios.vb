Imports System.Data.SqlClient
Imports System.Configuration

Public Class frmUsuarios
    Dim fecha = DateAndTime.Now.ToShortDateString.ToString
    Dim hora As String = TimeOfDay
    Dim tipo As String
    Dim variable As SqlDataReader
    Dim consulta As New SqlCommand
    Protected configuracion As ConnectionStringSettings = ConfigurationManager.ConnectionStrings("cnn")
    Dim cnn As New SqlConnection(configuracion.ConnectionString)

    Public Sub cargarcombo()
        If cnn.State = ConnectionState.Open Then
            cnn.Close()
        End If

        consulta.CommandType = CommandType.Text
        consulta.CommandText = ("select usuario from tbl_Empleado")
        consulta.Connection = (cnn)
        cnn.Open()
        variable = consulta.ExecuteReader()

        While variable.Read = True
            TextBox3.Items.Add(variable.Item(0))

        End While
        cnn.Close()
    End Sub


    Public Sub cargarcomboTipo()
        If cnn.State = ConnectionState.Open Then
            cnn.Close()
        End If
        consulta.CommandType = CommandType.Text
        consulta.CommandText = ("select rol from tbl_Rol")
        consulta.Connection = (cnn)
        cnn.Open()
        variable = consulta.ExecuteReader()

        While variable.Read = True
            cbTipo.Items.Add(variable.Item(0))

        End While
        cnn.Close()
    End Sub

    Private Function Exists(ByVal Id As String) As Boolean
        Dim sql As String = "SELECT COUNT(*)  FROM tbl_Empleado WHERE Usuario = @Usuario"


        Dim command As New SqlCommand(sql, cnn)
            command.Parameters.AddWithValue("Usuario", Id)

        cnn.Open()

            Dim count As Integer = Convert.ToInt32(command.ExecuteScalar())

            If count = 0 Then
                Return False
            Else
                Return True

        End If
        cnn.Close()
    End Function

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        Try
            If TextBox4.Text <> TextBox5.Text Then
                MsgBox("Contraseñas no coinciden", MsgBoxStyle.Information, "Atención")
            Else

                If Exists(TextBox3.Text) Then
                    MsgBox("Usuario ya registrado, escoja otro Nombre", MsgBoxStyle.Exclamation, "Usuario Existente")
                Else

                    If TextBox3.Text <> "" And lblTipoUsuario.Text <> "" Then
                        If cnn.State = ConnectionState.Open Then
                            cnn.Close()
                        End If
                        cnn.Open()
                        Dim cmd As New SqlCommand("sp_CrearEmpleado", cnn)

                        cmd.CommandType = CommandType.StoredProcedure
                        cmd.Parameters.AddWithValue("@rut", txtRut.Text)
                        cmd.Parameters.AddWithValue("@digito", txtDigito.Text)
                        cmd.Parameters.AddWithValue("@primer_nombre", TextBox1.Text)
                        cmd.Parameters.AddWithValue("@segundo_nombre", "")
                        cmd.Parameters.AddWithValue("@apellido_paterno", TextBox2.Text)
                        cmd.Parameters.AddWithValue("@apellido_materno", "")
                        cmd.Parameters.AddWithValue("@cargo", txtCargo.Text)
                        cmd.Parameters.AddWithValue("@area", "1")
                        cmd.Parameters.AddWithValue("@sub_Area", "1")
                        cmd.Parameters.AddWithValue("@estado_empleado", "1")
                        cmd.Parameters.AddWithValue("@id_rol", lblTipoUsuario.Text)
                        cmd.Parameters.AddWithValue("@usuario", TextBox3.Text)
                        cmd.Parameters.AddWithValue("@contraseña", TextBox4.Text)
                        cmd.Parameters.AddWithValue("@count_estado", "0")

                        cmd.ExecuteNonQuery()

                        MsgBox("Usuario Creado con Exito", MsgBoxStyle.Information, "Usuario Creado")
                        cnn.Close()
                        TextBox3.Items.Clear()
                        cargarcombo()
                        TextBox1.Text = ""
                        TextBox2.Text = ""
                        TextBox3.Text = ""
                        TextBox4.Text = ""
                        TextBox5.Text = ""
                        TextBox6.Text = ""
                        txtCargo.Text = ""
                        txtDigito.Text = ""
                        txtRut.Text = ""
                        tipo = ""
                        CheckBox1.Checked = False
                    Else
                        MsgBox("Ingrese los Datos Solicitados", MsgBoxStyle.Information, "Faltan Datos")
                        cnn.Close()
                    End If
               


            End If
        End If
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub

    'Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button3.Click
    '    Using cnn As New SqlConnection("Data Source=bastian-note;Initial Catalog=Valsandwich;Integrated Security=True")
    '        Try

    '            Dim Pregunta As Integer
    '            If TextBox6.Text <> "" Then

    '                If TextBox3.Text <> "" Then
    '                    Pregunta = MsgBox("Desea Eliminar Usuario de: " + TextBox1.Text + " " + TextBox2.Text, vbYesNo + MsgBoxStyle.Information, "Confirmación")

    '                    If Pregunta = vbYes Then
    '                        cnn.Open()

    '                        Dim cmd As New SqlCommand("sp_ElminarUsuario", cnn)
    '                        cmd.CommandType = CommandType.StoredProcedure

    '                        cmd.Parameters.AddWithValue("@Usuario", TextBox3.Text)

    '                        cmd.ExecuteNonQuery()
    '                        MsgBox("Usuario Eliminado Correctamente", MsgBoxStyle.Information, "Atención")
    '                        TextBox1.Text = ""
    '                        TextBox2.Text = ""
    '                        TextBox3.Text = ""
    '                        TextBox2.Text = ""
    '                        TextBox3.Text = ""
    '                        TextBox4.Text = ""
    '                        TextBox5.Text = ""
    '                        TextBox6.Text = ""
    '                        RadioButton2.Checked = True
    '                        tipo = ""
    '                        TextBox3.Items.Clear()
    '                        cargarcombo()

    '                    End If

    '                    If Pregunta = vbNo Then
    '                        ' what ever you want to do with a no reponse
    '                    End If
    '                Else
    '                    MsgBox("Seleccione Usuario a Eliminar", MsgBoxStyle.Information, "Atención")

    '                End If
    '            Else

    '                MsgBox("Ocurrio un error al eliminar el usuario", MsgBoxStyle.Information + MsgBoxStyle.OkOnly, "Imposible Eliminar")

    '            End If

    '        Catch ex As Exception
    '            MsgBox(ex.Message)
    '        End Try
    '    End Using
    'End Sub

    Private Sub frmUsuarios_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        'frmAdministrador.Show()
    End Sub

    Private Sub frmUsuarios_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        If TextBox6.Text <> "" Then
            Button1.Visible = False
            Button4.Visible = True
            Button2.Visible = True
            Button3.Visible = True
        Else
            Button1.Visible = True
            Button4.Visible = False
            Button2.Visible = False
            Button3.Visible = False
        End If


        cargarcombo()
        cargarcomboTipo()

    End Sub

    Private Sub TextBox3_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles TextBox3.KeyPress
        e.KeyChar = e.KeyChar.ToString.ToUpper
    End Sub


    Private Sub TextBox3_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TextBox3.SelectedIndexChanged
        If cnn.State = ConnectionState.Open Then
            cnn.Close()
        End If
        Dim Sql As String = "Select * From tbl_Empleado where Usuario = @usu"


        Dim command As New SqlCommand(Sql, cnn)
        command.Parameters.AddWithValue("@usu", TextBox3.Text)

        cnn.Open()

        Dim reader As SqlDataReader = command.ExecuteReader()

        If reader.Read() Then
            Label8.Visible = True
            Label9.Visible = True
            Label10.Visible = True
            Label11.Visible = True

            TextBox6.Text = Convert.ToString(reader("ID_Empleado"))
            TextBox1.Text = Convert.ToString(reader("primer_nombre"))
            TextBox2.Text = Convert.ToString(reader("apellido_paterno"))
            lblTipoUsuario.Text = (reader("id_rol"))
            TextBox4.Text = Convert.ToString(reader("Contraseña"))
            TextBox5.Text = Convert.ToString(reader("Contraseña"))
            txtRut.Text = Convert.ToString(reader("rut"))
            txtDigito.Text = Convert.ToString(reader("digito"))
            txtCargo.Text = Convert.ToString(reader("cargo"))
            Dim estado = Convert.ToString(reader("estado_empleado"))
            If estado = "2" Then
                CheckBox1.Checked = True
            Else
                CheckBox1.Checked = False
            End If
        Else
            Label8.Visible = False
            Label9.Visible = False
            Label10.Visible = False
            Label11.Visible = False
        End If
        reader.Close()
        If lblTipoUsuario.Text <> "" Then
            Dim Sql1 As String = "Select * From tbl_rol where id_rol = @usu"


            Dim command1 As New SqlCommand(Sql1, cnn)
            command1.Parameters.AddWithValue("@usu", lblTipoUsuario.Text)


            Dim reader1 As SqlDataReader = command1.ExecuteReader()

            If reader1.Read() Then
                '  MsgBox(reader1("Tipo"))
                cbTipo.SelectedItem = (reader1("rol"))

            Else

            End If
        End If
        cnn.Close()
    End Sub

    Private Sub TextBox6_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TextBox6.TextChanged
        If TextBox6.Text <> "" Then
            Button1.Visible = False
            Button4.Visible = True
            Button2.Visible = True
            Button3.Visible = True
            Label8.Visible = True
            Label9.Visible = True
            Label10.Visible = True
            Label11.Visible = True
        Else
            Button1.Visible = True
            Button4.Visible = False
            Button2.Visible = False
            Button3.Visible = False
            Label8.Visible = False
            Label9.Visible = False
            Label10.Visible = False
            Label11.Visible = False
        End If
    End Sub

    Private Sub Button4_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button4.Click
        TextBox1.Text = ""
        TextBox2.Text = ""
        TextBox3.Text = ""
        TextBox2.Text = ""
        TextBox3.Text = ""
        TextBox4.Text = ""
        TextBox5.Text = ""
        TextBox6.Text = ""
        txtCargo.Text = ""
        txtDigito.Text = ""
        txtRut.Text = ""
        tipo = ""
        CheckBox1.Checked = False
    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
      Try
            cnn.Open()

            Dim cmd As New SqlCommand("sp_ModificarUsuarios", cnn)
            cmd.CommandType = CommandType.StoredProcedure

            cmd.Parameters.AddWithValue("@ID_empleado", TextBox6.Text)
            cmd.CommandType = CommandType.StoredProcedure
            cmd.Parameters.AddWithValue("@rut", txtRut.Text)
            cmd.Parameters.AddWithValue("@digito", txtDigito.Text)
            cmd.Parameters.AddWithValue("@primer_nombre", TextBox1.Text)
            cmd.Parameters.AddWithValue("@segundo_nombre", "")
            cmd.Parameters.AddWithValue("@apellido_paterno", TextBox2.Text)
            cmd.Parameters.AddWithValue("@apellido_materno", "")
            cmd.Parameters.AddWithValue("@cargo", txtCargo.Text)
            cmd.Parameters.AddWithValue("@area", "1")
            cmd.Parameters.AddWithValue("@sub_Area", "1")
            If CheckBox1.Checked = True Then
                cmd.Parameters.AddWithValue("@estado_empleado", "2")
            Else
                cmd.Parameters.AddWithValue("@estado_empleado", "1")
            End If

            cmd.Parameters.AddWithValue("@id_rol", lblTipoUsuario.Text)
            cmd.Parameters.AddWithValue("@usuario", TextBox3.Text)
            cmd.Parameters.AddWithValue("@contraseña", TextBox4.Text)
            cmd.Parameters.AddWithValue("@count_estado", "0")

            cmd.ExecuteNonQuery()

            MsgBox("Usuario: " + TextBox3.Text + " Modificado con Exito", MsgBoxStyle.Information, "Actualizar Usuario")
            TextBox3.Items.Clear()
            cargarcombo()
            TextBox1.Text = ""
            TextBox2.Text = ""
            TextBox3.Text = ""
            TextBox4.Text = ""
            TextBox5.Text = ""
            TextBox6.Text = ""
            txtDigito.Text = ""
            txtRut.Text = ""
            txtCargo.Text = ""


        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub

    Private Sub cbTipo_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles cbTipo.KeyPress
        e.KeyChar = e.KeyChar.ToString.ToUpper
    End Sub

    Private Sub cbTipo_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cbTipo.SelectedIndexChanged
        If cnn.State = ConnectionState.Open Then
            cnn.Close()
        End If
        Dim Sql As String = "Select * From tbl_Rol where Rol = @usu"


        Dim command As New SqlCommand(Sql, cnn)
        command.Parameters.AddWithValue("@usu", cbTipo.Text)

        cnn.Open()

        Dim reader As SqlDataReader = command.ExecuteReader()

        If reader.Read() Then
            lblTipoUsuario.Text = (reader("ID_Rol"))
           
        Else
            
        End If
        cnn.Close()
    End Sub


    Private Sub lblTipoUsuario_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles lblTipoUsuario.TextChanged
        'If lblTipoUsuario.Text = Not Nothing Then
        '    Dim Sql As String = "Select * From TipoUsuario where ID_TipoUsuario = @usu"


        '    Dim command As New SqlCommand(Sql, cnn)
        '    command.Parameters.AddWithValue("@usu", lblTipoUsuario.Text)

        '    cnn.Open()

        '    Dim reader As SqlDataReader = command.ExecuteReader()

        '    If reader.Read() Then
        '        cbTipo.SelectedItem = (reader("Tipo"))

        '    Else

        '    End If
        'End If
        'cnn.Close()
    End Sub

    Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button3.Click
        Try

            Dim Pregunta As Integer
            If TextBox6.Text <> "" Then

                If TextBox3.Text <> "" Then
                    Pregunta = MsgBox("Desea Eliminar Usuario de: " + TextBox1.Text, vbYesNo + MsgBoxStyle.Information, "Confirmación")

                    If Pregunta = vbYes Then
                        cnn.Open()

                        Dim cmd As New SqlCommand("sp_ElminarUsuario", cnn)
                        cmd.CommandType = CommandType.StoredProcedure

                        cmd.Parameters.AddWithValue("@ID_Usuario", TextBox6.Text)

                        cmd.ExecuteNonQuery()
                        MsgBox("Usuario Eliminado Correctamente", MsgBoxStyle.Information, "Atención")
                        TextBox1.Text = ""
                        TextBox2.Text = ""
                        TextBox3.Text = ""
                        TextBox4.Text = ""
                        TextBox5.Text = ""
                        TextBox6.Text = ""
                        cbTipo.Text = ""
                        TextBox3.Items.Clear()
                        cargarcombo()

                    End If

                    If Pregunta = vbNo Then
                        ' what ever you want to do with a no reponse
                    End If
                Else
                    MsgBox("Seleccione Usuario a Eliminar", MsgBoxStyle.Information, "Atención")

                End If
            Else

                MsgBox("Ocurrio un error al eliminar el usuario", MsgBoxStyle.Information + MsgBoxStyle.OkOnly, "Imposible Eliminar")

            End If

        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub

End Class