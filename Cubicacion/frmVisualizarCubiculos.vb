Imports System.Data.SqlClient
Imports System.Text.RegularExpressions
Imports System.Configuration
Public Class frmVisualizarCubiculos
    Dim estadoCubiculo As String
    Dim idColumna, idFila, estadoPiso As Integer
    Protected configuracion As ConnectionStringSettings = ConfigurationManager.ConnectionStrings("cnn")
    Dim cnn As New SqlConnection(configuracion.ConnectionString)

    Public Sub cargardataCubiculo()
        Try

            Dim command As SqlCommand
            Dim adapter As SqlDataAdapter
            Dim dtTable As DataTable
            cnn.Open()
            'Indico el SP que voy a utilizar
            command = New SqlCommand("EstadoCubiculo", cnn)
            command.CommandType = CommandType.StoredProcedure
            command.Parameters.AddWithValue("@aux", cbNColumna.Text)
            command.ExecuteNonQuery()

            command.CommandType = CommandType.StoredProcedure
            adapter = New SqlDataAdapter(command)
            dtTable = New DataTable

            'Aquí ejecuto el SP y lo lleno en el DataTable
            adapter.Fill(dtTable)
            'Enlazo mis datos obtenidos en el DataTable con el grid
            DataGridView1.DataSource = dtTable
            DataGridView1.Columns(0).HeaderText = ""

            Dim cmd4 As New SqlCommand("sp_CrearBitacora", cnn)

            cmd4.CommandType = CommandType.StoredProcedure

            cmd4.Parameters.AddWithValue("@fecha_Ingreso", Date.Now)
            cmd4.Parameters.AddWithValue("@id_EmpleadoIngreso", clsLogin.IdUsuario)
            cmd4.Parameters.AddWithValue("@Detalle", "VISUALIZA ESTADO GRAFICO DE BODEGA, COLUMNA: " & cbNColumna.Text)
            cmd4.Parameters.AddWithValue("@Tipo", 10)
            cmd4.ExecuteNonQuery()


            cnn.Close()
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub

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


    Public Sub cargarComboNColumna()

        If cbFila.Text = "" Then

        End If
        cbNColumna.Items.Clear()
        Dim variable As SqlDataReader
        Dim sql As New SqlCommand
        sql.CommandType = CommandType.Text
        sql.CommandText = ("select left(numero_Columna,1) from tbl_Columna where numero_columna <> 'P1' group by left(numero_Columna,1)")
        sql.Connection = (cnn)
        cnn.Open()
        variable = sql.ExecuteReader()

        While variable.Read = True
            cbNColumna.Items.Add(variable.Item(0).ToString.ToUpper)
        End While
        cnn.Close()

    End Sub

    Private Sub frmVisualizarCubiculos_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load
        Try
            cargarComboNColumna()
            cargarComboColumna()
            cargarComboFila()
            cargardataCubiculo()
            cbNColumna.SelectedIndex = 0
            If cnn.State = ConnectionState.Open Then
                cnn.Close()
            End If
            cnn.Open()
            Dim cmd4 As New SqlCommand("sp_CrearBitacora", cnn)

            cmd4.CommandType = CommandType.StoredProcedure

            cmd4.Parameters.AddWithValue("@fecha_Ingreso", Date.Now)
            cmd4.Parameters.AddWithValue("@id_EmpleadoIngreso", clsLogin.IdUsuario)
            cmd4.Parameters.AddWithValue("@Detalle", "VISUALIZA ESTO GRAFICO DE BODEGA, COLUMNA: " & cbColumna.Text)
            cmd4.Parameters.AddWithValue("@Tipo", 10)
            cmd4.ExecuteNonQuery()

            cnn.Close()

        Catch ex As Exception

        End Try
    End Sub

    Private Sub btnBuscar_Click(sender As System.Object, e As System.EventArgs) Handles btnBuscar.Click
        Try
            If cbFila.Text = "" And cbColumna.Text = "" Then
                MsgBox("Ingrese Columna y Fila", MsgBoxStyle.Information, "Atención")
            ElseIf cbColumna.Text = "" Then
                MsgBox("Ingrese Columna", MsgBoxStyle.Information, "Atención")
            ElseIf cbFila.Text = "" Then
                MsgBox("Ingrese Fila", MsgBoxStyle.Information, "Atención")

            Else
                Dim Sql As String = "Select estado_Cubiculo From tbl_Cubiculo where id_Columna=@columna and id_Fila=@fila"

                Dim command As New SqlCommand(Sql, cnn)
                command.Parameters.AddWithValue("@columna", idColumna)
                command.Parameters.AddWithValue("@fila", idFila)
                cnn.Open()

                Dim reader As SqlDataReader = command.ExecuteReader()

                If reader.Read() Then
                    estadoCubiculo = Convert.ToString(reader("estado_Cubiculo"))
                    If estadoCubiculo = 1 Then
                        lblEstado.Text = "Cubiculo Disponible"
                    ElseIf estadoCubiculo = 2 Then
                        lblEstado.Text = "Cubiculo Ocupado"
                    ElseIf estadoCubiculo = 3 Then
                        lblEstado.Text = "Cubiculo Inactivo"
                    ElseIf estadoCubiculo = 4 Then
                        lblEstado.Text = "Cubiculo En Espera"
                    ElseIf estadoCubiculo = 5 Then
                        lblEstado.Text = "Cubiculo con Otro Estado"
                    End If
                Else
                    lblEstado.Text = "Cubiculo no Registrado"
                End If
                cnn.Close()

            End If
        Catch ex As Exception

            MsgBox(ex.Message)
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

    Private Sub DataGridView1_CellFormatting(sender As Object, e As DataGridViewCellFormattingEventArgs) Handles DataGridView1.CellFormatting

        If estadoPiso <> "1" Then

            For i As Integer = 0 To DataGridView1.RowCount - 1
                For j As Integer = 1 To DataGridView1.ColumnCount - 1
                    'MsgBox(DataGridView1.Rows(i).Cells(j).Value.ToString)
                    If DataGridView1.Rows(i).Cells(j).Value.ToString >= "30" And e.RowIndex = i And e.ColumnIndex = j Then
                        e.CellStyle.BackColor = Color.Yellow
                        e.CellStyle.ForeColor = Color.Black

                    ElseIf DataGridView1.Rows(i).Cells(j).Value.ToString() = "100" AndAlso e.RowIndex = i AndAlso e.ColumnIndex = j Then
                        e.CellStyle.BackColor = Color.Lime
                        e.CellStyle.ForeColor = Color.Black
                    ElseIf DataGridView1.Rows(i).Cells(j).Value.ToString() < "30" And DataGridView1.Rows(i).Cells(j).Value.ToString() > "0" AndAlso e.RowIndex = i AndAlso e.ColumnIndex = j Then
                        e.CellStyle.BackColor = Color.Tomato
                        e.CellStyle.ForeColor = Color.Black
                    ElseIf DataGridView1.Rows(i).Cells(j).Value.ToString() = "0" AndAlso e.RowIndex = i AndAlso e.ColumnIndex = j Then
                        e.CellStyle.BackColor = Color.Red
                        e.CellStyle.ForeColor = Color.Black
                        'ElseIf DataGridView1.Rows(i).Cells(j).Value.ToString() = "3" AndAlso e.RowIndex = i AndAlso e.ColumnIndex = j Then
                        '    e.CellStyle.BackColor = Color.Blue
                        '    e.CellStyle.ForeColor = Color.Blue
                        'ElseIf DataGridView1.Rows(i).Cells(j).Value.ToString() = "4" AndAlso e.RowIndex = i AndAlso e.ColumnIndex = j Then
                        '    e.CellStyle.BackColor = Color.Yellow
                        '    e.CellStyle.ForeColor = Color.Yellow
                        'ElseIf DataGridView1.Rows(i).Cells(j).Value.ToString() = "5" AndAlso e.RowIndex = i AndAlso e.ColumnIndex = j Then
                        '    e.CellStyle.BackColor = Color.Aqua
                        '    e.CellStyle.ForeColor = Color.Aqua
                    End If

                Next

            Next

            DataGridView1.Columns(0).Width = 25
            DataGridView1.RowsDefaultCellStyle.BackColor = Color.White
            DataGridView1.DefaultCellStyle.SelectionBackColor = Color.DodgerBlue
        Else

            With DataGridView1
                ' alternar colores  
                .RowsDefaultCellStyle.BackColor = Color.GreenYellow

                .AlternatingRowsDefaultCellStyle.BackColor = Color.White
                .ForeColor = Color.Black
                .DefaultCellStyle.SelectionForeColor = Color.Black
                .DefaultCellStyle.SelectionBackColor = Color.LightSteelBlue
                .ClearSelection()
            End With
        End If
        'DataGridView1.ClearSelection()
    End Sub

    Private Sub cbNColumna_SelectedIndexChanged(sender As System.Object, e As System.EventArgs) Handles cbNColumna.SelectedIndexChanged
        estadoPiso = "0"
        cargardataCubiculo()
    End Sub

    Private Sub DataGridView1_CellContentClick(sender As System.Object, e As System.Windows.Forms.DataGridViewCellEventArgs) Handles DataGridView1.CellContentClick
        Try
            If estadoPiso <> "1" Then
                Dim fila = DataGridView1.CurrentRow.Index
                ' MessageBox.Show(DataGridView1.Columns(e.ColumnIndex).Name & ", " & DataGridView1.Item(0, fila).Value)



                Dim command As SqlCommand
                Dim adapter As SqlDataAdapter
                Dim dtTable As DataTable
                cnn.Open()
                'Indico el SP que voy a utilizar
                command = New SqlCommand("sp_DetalleCubiculo", cnn)
                command.CommandType = CommandType.StoredProcedure
                command.Parameters.AddWithValue("@col", DataGridView1.Columns(e.ColumnIndex).Name)
                command.Parameters.AddWithValue("@fil", DataGridView1.Item(0, fila).Value)
                command.ExecuteNonQuery()

                command.CommandType = CommandType.StoredProcedure
                adapter = New SqlDataAdapter(command)
                dtTable = New DataTable

                'Aquí ejecuto el SP y lo lleno en el DataTable
                adapter.Fill(dtTable)
                'Enlazo mis datos obtenidos en el DataTable con el grid
                DataGridView2.DataSource = dtTable
                DataGridView2.Columns(0).HeaderText = ""
                cnn.Close()
            Else
            End If

        Catch ex As Exception
            MsgBox(ex.Message)
        End Try

    End Sub

    Private Sub DataGridView1_CellMouseEnter(sender As Object, e As System.Windows.Forms.DataGridViewCellEventArgs) Handles DataGridView1.CellMouseEnter
        Try
            If estadoPiso <> "1" Then
                If e.RowIndex > -1 Then
                    DataGridView1.Rows(e.RowIndex).Cells(e.ColumnIndex).Selected = True

                    With Me.DataGridView1.Rows(e.RowIndex).Cells(e.ColumnIndex)
                        Dim command As SqlCommand

                        cnn.Open()
                        'Indico el SP que voy a utilizar
                        command = New SqlCommand("sp_DetalleCubiculoM", cnn)
                        command.CommandType = CommandType.StoredProcedure
                        Dim fila = DataGridView1.Rows(e.RowIndex).Index
                        Label12.Text = ("Columna: " & DataGridView1.Columns(e.ColumnIndex).Name & Chr(13) & "Fila: " & DataGridView1.Item(0, fila).Value)
                        command.Parameters.AddWithValue("@col", DataGridView1.Columns(e.ColumnIndex).Name)
                        command.Parameters.AddWithValue("@fil", DataGridView1.Item(0, fila).Value)
                        command.ExecuteNonQuery()

                        command.CommandType = CommandType.StoredProcedure
                        Dim reader As SqlDataReader = command.ExecuteReader()


                        If reader.Read() Then
                            .ToolTipText = "N° Objetos: " + Convert.ToString(reader("numero_Columna")) _
                            & Chr(13) & "Ancho: " + Convert.ToString(reader("ancho")) _
                            & Chr(13) & "Largo: " + Convert.ToString(reader("largo")) _
                            & Chr(13) & "Alto: " + Convert.ToString(reader("alto")) _
                            & Chr(13) & "Peso Maximo: " + Convert.ToString(reader("peso")) _
                            & Chr(13) & "Temperatura: " + Convert.ToString(reader("temperatura"))


                        End If
                        cnn.Close()
                    End With
                End If
            Else
            End If

        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub

    Private Sub DataGridView1_CellMouseLeave(sender As Object, e As System.Windows.Forms.DataGridViewCellEventArgs) Handles DataGridView1.CellMouseLeave
        If e.RowIndex > -1 Then
            DataGridView1.Rows(e.RowIndex).Cells(e.ColumnIndex).Selected = False
        End If
    End Sub

    Private Sub Button1_Click(sender As System.Object, e As System.EventArgs) Handles Button1.Click
        Try
            estadoPiso = 1
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
            DataGridView1.DataSource = dtTable
            'DataGridView1.Columns(0).HeaderText = ""


            Dim cmd4 As New SqlCommand("sp_CrearBitacora", cnn)

            cmd4.CommandType = CommandType.StoredProcedure

            cmd4.Parameters.AddWithValue("@fecha_Ingreso", Date.Now)
            cmd4.Parameters.AddWithValue("@id_EmpleadoIngreso", clsLogin.IdUsuario)
            cmd4.Parameters.AddWithValue("@Detalle", "VISUALIZA ESTADO PISO BODEGA")
            cmd4.Parameters.AddWithValue("@Tipo", 10)
            cmd4.ExecuteNonQuery()


            cnn.Close()
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub
End Class