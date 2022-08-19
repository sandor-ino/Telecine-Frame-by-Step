Public Class Dialog1
    Private currentX As Integer, currentY As Integer
    Private isDragging As Boolean = False
    Dim cox As Integer = 0
    Dim coy As Integer = 0
    Private Sub Dialog1_Load(sender As Object, e As EventArgs) Handles MyBase.Load, MyBase.MaximumSizeChanged
        Panel2.Location = New Point(ClientSize.Width / 2 - Panel2.Size.Width / 2, ClientSize.Height / 2 - Panel2.Size.Height / 2)
        Panel2.Anchor = AnchorStyles.None
        PictureBox1.Location = New Point(ClientSize.Width / 2 - PictureBox1.Size.Width / 2, ClientSize.Height / 6) ' - Panel2.Size.Height / 2)
        PictureBox1.Anchor = AnchorStyles.None
        Button1.Enabled = False
        Label2.Text = ""
        Label3.Text = ""
    End Sub
    Private Sub PictureBox1_MouseDown(sender As Object, e As System.Windows.Forms.MouseEventArgs) Handles PictureBox1.MouseDown
        isDragging = True
        currentX = e.X
        currentY = e.Y
        Button1.Enabled = True
    End Sub
    Private Sub PictureBox1_MouseMove(sender As Object, e As System.Windows.Forms.MouseEventArgs) Handles PictureBox1.MouseMove
        If isDragging Then
            PictureBox1.Top = PictureBox1.Top + (e.Y - currentY)
            PictureBox1.Left = PictureBox1.Left + (e.X - currentX)
            Label2.Text = PictureBox1.Location.X - 50 + 82
            Label3.Text = PictureBox1.Location.Y - 50 + 82
        End If
    End Sub
    Private Sub PictureBox1_MouseUp(sender As Object, e As System.Windows.Forms.MouseEventArgs) Handles PictureBox1.MouseUp
        isDragging = False
    End Sub
    Private Sub Panel2_MouseDown(sender As Object, e As System.Windows.Forms.MouseEventArgs) Handles Panel2.MouseDown
        isDragging = True
        currentX = e.X
        currentY = e.Y
    End Sub
    Private Sub Panel2_MouseMove(sender As Object, e As System.Windows.Forms.MouseEventArgs) Handles Panel2.MouseMove
        If isDragging Then
            Panel2.Top = Panel2.Top + (e.Y - currentY)
            Panel2.Left = Panel2.Left + (e.X - currentX)
        End If
    End Sub
    Private Sub Panel2_MouseUp(sender As Object, e As System.Windows.Forms.MouseEventArgs) Handles Panel2.MouseUp
        isDragging = False
    End Sub
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        cox = Label2.Text
        coy = Label3.Text
        'MessageBox.Show("Coordinate Click Rilevate" & vbCrLf & vbCrLf & "X =  " & CInt(cox) & "     Y =  " & CInt(coy))
        My.Settings.xclick = Label2.Text
        My.Settings.yclick = Label3.Text
        Me.Close()
    End Sub
    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Me.Close()
    End Sub
End Class
