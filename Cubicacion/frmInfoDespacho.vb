Imports System.Data.SqlClient
Imports System.Text.RegularExpressions
Imports System.Configuration

Public Class frmInfoDespacho
    Protected configuracion As ConnectionStringSettings = ConfigurationManager.ConnectionStrings("cnn")
    Dim cnn As New SqlConnection(configuracion.ConnectionString)
    Public norden As String

    Protected Overrides Sub OnPaint(ByVal e As PaintEventArgs)
        MyBase.OnPaint(e)
        Dim AnchoBorde As Integer = 1
        Dim ColorBorde As Color = Color.Red
        ControlPaint.DrawBorder(e.Graphics, e.ClipRectangle, ColorBorde,
        AnchoBorde, ButtonBorderStyle.Solid, ColorBorde, AnchoBorde,
        ButtonBorderStyle.Solid, ColorBorde, AnchoBorde, ButtonBorderStyle.Solid,
        ColorBorde, AnchoBorde, ButtonBorderStyle.Solid)
    End Sub

    Private Sub frmInfoDespacho_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load
        Dim command As SqlCommand
     
        cnn.Open()
        'Indico el SP que voy a utilizar
        command = New SqlCommand("sp_DetalleDespacho", cnn)
        command.CommandType = CommandType.StoredProcedure
        command.Parameters.AddWithValue("@norden", norden)

        command.ExecuteNonQuery()

        command.CommandType = CommandType.StoredProcedure

        Dim reader As SqlDataReader = command.ExecuteReader()

        If reader.Read() Then
            lnlNOrden.Text = Convert.ToString(reader("n_orden"))
            lblEmpledoIngreso.Text = Convert.ToString(reader("primer_nombre"))
            lblProveedor.Text = Convert.ToString(reader("nombre"))
            lblColumna.Text = Convert.ToString(reader("columna"))
            lblFila.Text = Convert.ToString(reader("fila"))
            lblFechaIngreso.Text = Convert.ToString(reader("fecha_ingreso"))
            lblFechaSalida.Text = Convert.ToString(reader("fecha_salida"))
            lblAncho.Text = Convert.ToString(reader("ancho"))
            lblAlto.Text = Convert.ToString(reader("alto"))
            lblLargo.Text = Convert.ToString(reader("largo"))
            lblPeso.Text = Convert.ToString(reader("peso"))
            lblTemperatura.Text = Convert.ToString(reader("temperatura"))
            lblObservaciones.Text = Convert.ToString(reader("descripcion"))

        End If
        cnn.Close()
    End Sub

    Private Sub PictureBox1_Click(sender As System.Object, e As System.EventArgs) Handles PictureBox1.Click
        Me.Close()
    End Sub
End Class