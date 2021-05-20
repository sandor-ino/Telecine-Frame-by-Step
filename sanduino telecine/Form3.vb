Public Class Form3
    Dim PIC As Boolean
    Dim PIC1 As Boolean
    ' APERTURA PROGRAMMA
    Private Sub Form5_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        PictureBox1.Image = My.Resources.MINIDRIVER2
        PIC = True
        PIC1 = True
    End Sub
    ' CAMBIA IMMAGINE
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If (PIC = True) And (PIC1 = True) Then
            PictureBox1.Image = My.Resources.MINIDRIVER1
            PIC = True
            PIC1 = False
        ElseIf (PIC = True) And (PIC1 = False) Then
            PictureBox1.Image = My.Resources.MINIDRIVER3
            PIC = False
            PIC1 = False
        ElseIf (PIC = False) And (PIC1 = False) Then
            PictureBox1.Image = My.Resources.MINIDRIVER2
            PIC = True
            PIC1 = True
        End If
    End Sub

End Class