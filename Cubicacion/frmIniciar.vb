Imports System.Data.SqlClient
Imports System.Configuration
Imports System.ServiceProcess
Imports System
Public Class frmIniciar
    Dim variable As SqlDataReader
    Dim consulta As New SqlCommand
    Dim bloqueo As String
    Protected configuracion As ConnectionStringSettings = ConfigurationManager.ConnectionStrings("cnn")
    Dim cnn As New SqlConnection(configuracion.ConnectionString)
    Public Sub cargardata()
        Try
            If cnn.State = ConnectionState.Open Then
                cnn.Close()
            End If
            Dim consulta As String = "select * from tbl_Empleado"

            Dim cmd As New SqlCommand(consulta, cnn)

            cmd.CommandType = CommandType.Text

            Dim da As New SqlDataAdapter(cmd)

            Dim dt As New DataTable

            da.Fill(dt)

            DataGridView1.DataSource = dt
            'DataGridView1.Columns(0).Width = 60
            'DataGridView1.Columns(1).Width = 120
            'DataGridView1.Columns(2).Width = 220
            'DataGridView1.Columns(3).Width = 60
            'Me.DataGridView1.Columns(0).Visible = False

        Catch ex As Exception
            MsgBox(ex.Message)
            MsgBox("Posible error de comunicación con la base de datos 'Cubicación'", MsgBoxStyle.Exclamation, "Atención Requerida")
            Button1.Enabled = False
            TextBox1.Enabled = False
            TextBox2.Enabled = False
        End Try
    End Sub
    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        Dim id_Empleado, count_Estado As String
        If cnn.State = ConnectionState.Open Then
            cnn.Close()
        End If
        cnn.Open()
        Dim query As String = "select * from tbl_Empleado where tbl_Empleado.Usuario = @usuario and tbl_Empleado.Contraseña= @contraseña"
        Dim cmd1 As New SqlClient.SqlCommand(query, cnn)
        cmd1.Parameters.AddWithValue("@usuario", TextBox1.Text)
        cmd1.Parameters.AddWithValue("@contraseña", TextBox2.Text)
        Dim id As String = cmd1.ExecuteScalar()

        Dim reader As SqlDataReader = cmd1.ExecuteReader()

        If reader.Read() Then
            If (reader("estado_Empleado")) = "1" Then
                Dim nombre As String = Convert.ToString(reader("primer_Nombre")).Trim
                Dim apellido As String = Convert.ToString(reader("apellido_Paterno")).Trim
                lblInfo.Text = (reader("id_Empleado"))

                clsLogin.IdUsuario = (reader("id_Empleado"))
                clsLogin.NombreUsuario = Convert.ToString(reader("primer_Nombre")).Trim & " " & Convert.ToString(reader("apellido_Paterno")).Trim
                reader.Close()
                Dim cmd4 As New SqlCommand("sp_CrearBitacora", cnn)

                cmd4.CommandType = CommandType.StoredProcedure

                cmd4.Parameters.AddWithValue("@fecha_Ingreso", Date.Now)
                cmd4.Parameters.AddWithValue("@id_EmpleadoIngreso", clsLogin.IdUsuario)
                cmd4.Parameters.AddWithValue("@Detalle", "LOGIN CORRECTO, USUARIO: " & TextBox1.Text)
                cmd4.Parameters.AddWithValue("@Tipo", 7)
                cmd4.ExecuteNonQuery()

                Dim query2 As String = "select id_empleado, count_Estado from tbl_Empleado where tbl_Empleado.Usuario = '" + TextBox1.Text + "'"
                Dim cmd5 As New SqlClient.SqlCommand(query2, cnn)

                Dim reader2 As SqlDataReader = cmd5.ExecuteReader()

                If reader2.Read() Then
                    id_Empleado = (reader2("id_Empleado"))
                    count_Estado = (reader2("count_Estado"))
                    reader2.Close()
                    Dim cmd3 As New SqlCommand("sp_ModificarCountEstado", cnn)

                    cmd3.CommandType = CommandType.StoredProcedure
                    cmd3.Parameters.AddWithValue("@id_Empleado", id_Empleado)
                    cmd3.Parameters.AddWithValue("@count_Estado", 0)
                    cmd3.ExecuteNonQuery()
                End If


                Form1.Show()
                Me.Close()
            ElseIf (reader("estado_Empleado")) = "2" Then
                MsgBox("Usuario bloqueado, contacte al administrador", MsgBoxStyle.Exclamation, "Atención")
                Return
            End If

        Else
            If TextBox1.Text = "" Then
                MsgBox("Ingrese Nombre de Usuario", MsgBoxStyle.Information, "Faltan Datos")
                Return
            ElseIf TextBox2.Text = "" Then
                MsgBox("Ingrese Contraseña", MsgBoxStyle.Information, "Faltan Datos")
                Return
            ElseIf TextBox1.Text = "" And TextBox2.Text = "" Then
                MsgBox("Ingrese Nombre de Usuario y Contraseña", MsgBoxStyle.Information, "Faltan Datos")
                Return
            Else

                reader.Close()
                Dim cmd2 As New SqlCommand("sp_CrearBitacora", cnn)

                cmd2.CommandType = CommandType.StoredProcedure

                cmd2.Parameters.AddWithValue("@fecha_Ingreso", Date.Now)
                cmd2.Parameters.AddWithValue("@id_EmpleadoIngreso", clsLogin.IdUsuario)
                cmd2.Parameters.AddWithValue("@Detalle", "LOGIN INCORRECTO, USUARIO: " & TextBox1.Text)
                cmd2.Parameters.AddWithValue("@Tipo", 7)
                cmd2.ExecuteNonQuery()

                Dim query2 As String = "select id_empleado, count_Estado, estado_Empleado from tbl_Empleado where tbl_Empleado.Usuario = '" + TextBox1.Text + "'"
                Dim cmd4 As New SqlClient.SqlCommand(query2, cnn)

                Dim reader2 As SqlDataReader = cmd4.ExecuteReader()



                If reader2.Read() Then
                    If (reader2("estado_Empleado")) = "1" Then
                        id_Empleado = (reader2("id_Empleado"))
                        count_Estado = (reader2("count_Estado"))
                        reader2.Close()
                        Dim cmd3 As New SqlCommand("sp_ModificarCountEstado", cnn)

                        cmd3.CommandType = CommandType.StoredProcedure
                        cmd3.Parameters.AddWithValue("@id_Empleado", id_Empleado)
                        cmd3.Parameters.AddWithValue("@count_Estado", CInt(count_Estado) + 1)
                        cmd3.ExecuteNonQuery()

                        MsgBox("Nombre de Usuario o Contraseña Incorrectos" + Chr(13) + "Intentos Restantes: " & (4 - CInt(count_Estado)), MsgBoxStyle.Information, "Faltan Datos")

                        Dim query3 As String = "select id_empleado, count_Estado from tbl_Empleado where tbl_Empleado.Usuario = '" + TextBox1.Text + "'"
                        Dim cmd6 As New SqlClient.SqlCommand(query3, cnn)

                        Dim reader3 As SqlDataReader = cmd6.ExecuteReader()

                        If reader3.Read() Then
                            count_Estado = (reader3("count_Estado"))
                        End If
                        reader3.Close()
                        If CInt(count_Estado) = 5 Then

                            Dim cmd5 As New SqlCommand("sp_BloquearEmpleado", cnn)

                            cmd5.CommandType = CommandType.StoredProcedure
                            cmd5.Parameters.AddWithValue("@id_Empleado", id_Empleado)
                            cmd5.Parameters.AddWithValue("@estado_Empleado", "2")
                            cmd5.ExecuteNonQuery()

                        End If

                        Return
                    ElseIf (reader2("estado_Empleado")) = "2" Then
                        MsgBox("Usuario bloqueado, contacte al administrador", MsgBoxStyle.Exclamation, "Atención")
                        Return

                    End If

                End If
            End If

        End If

        'Dim fila As Integer
        'fila = Me.DataGridView1.CurrentRow.Index
        ''TextBox1.Text = DataGridView1.Item(6, fila).Value
        'For x As Integer = 0 To DataGridView1.Rows.Count - 1
        '    MessageBox.Show(DataGridView1.Columns(x).Name.ToString)
        'Next
        cnn.Close()
    End Sub

 

    Private Sub frmIniciar_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Try
            Label4.Text = "Configurando..."
            ' Toggle the Telnet service - 
            ' If it is started (running, paused, etc), stop the service.
            ' If it is stopped, start the service.
            Dim sc As New ServiceController("MSSQLSERVER")

            If sc.Status.Equals(ServiceControllerStatus.Stopped) Or sc.Status.Equals(ServiceControllerStatus.StopPending) Then
                ' Start the service if the current status is stopped.
                Label4.Text = ("Iniciando Servidor MSSQLSERVER")
                sc.Start()
            Else
                ' Stop the service if its status is not set to "Stopped".
                'Console.WriteLine("Stopping the Telnet service...")
                'sc.Stop()
                Label4.Text = ("Base de Datos iniciada")
            End If

            ' Refresh and display the current service status.
            sc.Refresh()
            Label4.Text = "Conexión Exitosa"

        Catch ex As Exception

        End Try

        Try

            ' Toggle the Telnet service - 
            ' If it is started (running, paused, etc), stop the service.
            ' If it is stopped, start the service.
            Dim sc As New ServiceController("MSSQL$SQLEXPRESS")

            If sc.Status.Equals(ServiceControllerStatus.Stopped) Or sc.Status.Equals(ServiceControllerStatus.StopPending) Then
                ' Start the service if the current status is stopped.
                Label4.Text = ("Iniciando SERVIDOR MSSQL$SQLEXPRESS")
                sc.Start()
            Else
                ' Stop the service if its status is not set to "Stopped".
                'Console.WriteLine("Stopping the Telnet service...")
                'sc.Stop()
                Label4.Text = ("Base de Datos iniciada")
            End If

            ' Refresh and display the current service status.
            sc.Refresh()
            Label4.Text = "Conexión Exitosa"

        Catch ex As Exception

        End Try
        cargardata()
        cargarcombo()
        Label1.Parent = PictureBox2
        Label1.BackColor = Color.Transparent
        Label2.Parent = PictureBox2
        Label2.BackColor = Color.Transparent
        PictureBox1.Parent = PictureBox2
        PictureBox1.BackColor = Color.Transparent
        Button1.Parent = PictureBox2
        Button1.BackColor = Color.Transparent

    End Sub
    Public Sub cargarcombo()
        Try
            If cnn.State = ConnectionState.Open Then
                cnn.Close()
            End If
            consulta.CommandType = CommandType.Text
            consulta.CommandText = ("select Usuario from tbl_Empleado")
            consulta.Connection = (cnn)
            cnn.Open()
            variable = consulta.ExecuteReader()

            While variable.Read = True
                TextBox1.Items.Add(variable.Item(0))

            End While
            cnn.Close()
        Catch
        End Try
    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click

        'Using cn As New SqlConnection("Data Source=bastian-note;Initial Catalog=Valsandwich;Integrated Security=True")
        '    cn.Open()
        '    Dim query As String = "select * from Usuarios where Usuarios.Usuario = @usuario and Usuarios.Contraseña= @contraseña;"
        '    Dim cmd1 As New SqlClient.SqlCommand(query, cn)
        '    cmd1.Parameters.AddWithValue("@usuario", TextBox1.Text)
        '    cmd1.Parameters.AddWithValue("@contraseña", TextBox2.Text)
        '    Dim id As String = cmd1.ExecuteScalar()

        '    Dim reader As SqlDataReader = cmd1.ExecuteReader()

        '    If reader.Read() Then
        '        MsgBox("Si")
        '        Label3.Text = Convert.ToString(reader("Tipo"))
        '    Else
        '        MsgBox("No")

        '    End If


        '    'Dim fila As Integer
        '    'fila = Me.DataGridView1.CurrentRow.Index
        '    ''TextBox1.Text = DataGridView1.Item(6, fila).Value
        '    'For x As Integer = 0 To DataGridView1.Rows.Count - 1
        '    '    MessageBox.Show(DataGridView1.Columns(x).Name.ToString)
        '    'Next
        'End Using
    End Sub

    Private Sub TextBox2_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles TextBox2.KeyDown
        Try
            If e.KeyCode = Keys.Enter Then
                Button1_Click("", e)
            End If
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical, "Atención")
        End Try
    End Sub
End Class