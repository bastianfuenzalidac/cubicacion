Imports System.Data.SqlClient
Imports System.Text.RegularExpressions
Imports System.Configuration
Imports System.Data.SqlTypes
Imports System.Collections.Generic
Public Class frmReporte
    Protected configuracion As ConnectionStringSettings = ConfigurationManager.ConnectionStrings("cnn")
    Dim cnn As New SqlConnection(configuracion.ConnectionString)
    Private Sub frmReporte_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load
        Try

        Catch ex As Exception
            cnn.Open()
            Dim cmd4 As New SqlCommand("sp_CrearBitacora", cnn)

            cmd4.CommandType = CommandType.StoredProcedure

            cmd4.Parameters.AddWithValue("@fecha_Ingreso", Date.Now)
            cmd4.Parameters.AddWithValue("@id_EmpleadoIngreso", clsLogin.IdUsuario)
            cmd4.Parameters.AddWithValue("@Detalle", "ERROR AL CARGAR REPORTE, " & ex.Message)
            cmd4.Parameters.AddWithValue("@Tipo", 9)
            cmd4.ExecuteNonQuery()

            cnn.Close()
        End Try
    End Sub
End Class