Public Class Form2
    Private currentX As Integer, currentY As Integer
    Private isDragging As Boolean = False
    Dim cox As Integer = 0
    Dim coy As Integer = 0
    Dim MO As Boolean = True
    ' SELEZIONA MODELLO ARDUINO
    Private Sub ComboBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox1.SelectedIndexChanged
        Button1.Visible = True
        If ComboBox1.SelectedItem = "Mini" Then
            PictureBox1.Image = My.Resources.MINI
        End If
        If ComboBox1.SelectedItem = "Nano" Then
            PictureBox1.Image = My.Resources.NANO
        End If
        If ComboBox1.SelectedItem = "Uno" Then
            PictureBox1.Image = My.Resources.UNO
        End If
        If ComboBox1.SelectedItem = "Mega" Then
            PictureBox1.Size = New System.Drawing.Size(PictureBox1.Width, PictureBox1.Height * 2)
            PictureBox1.Image = My.Resources.MEGA
        End If
        PictureBox1.Cursor = Cursors.SizeAll
        ComboBox1.Enabled = False
        ComboBox1.Visible = False
        Label1.Visible = False
        Button1.Visible = True
        Button2.Visible = True
    End Sub

    ' AVVIO PROGRAMMA
    Private Sub Form2_Load(sender As Object, e As EventArgs) Handles MyBase.Load, MyBase.MaximumSizeChanged
        PictureBox1.Location = New Point(ClientSize.Width / 2 - PictureBox1.Size.Width / 2, ClientSize.Height / 2 - (PictureBox1.Height / 2))
        PictureBox1.Anchor = AnchorStyles.None
    End Sub
    ' PAN IMMAGINE 1/3
    Private Sub PictureBox1_MouseDown(sender As Object, e As System.Windows.Forms.MouseEventArgs) Handles PictureBox1.MouseDown
        isDragging = True
        currentX = e.X
        currentY = e.Y
    End Sub
    ' PAN IMMAGINE 2/3
    Private Sub PictureBox1_MouseMove(sender As Object, e As System.Windows.Forms.MouseEventArgs) Handles PictureBox1.MouseMove
        If isDragging Then
            PictureBox1.Top = PictureBox1.Top + (e.Y - currentY)
            PictureBox1.Left = PictureBox1.Left + (e.X - currentX)
        End If
    End Sub
    ' PAN IMMAGINE 3/3
    Private Sub PictureBox1_MouseUp(sender As Object, e As System.Windows.Forms.MouseEventArgs) Handles PictureBox1.MouseUp
        isDragging = False
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        'PictureBox1.Cursor = Cursors.SizeAll
        'PictureBox1.Location = New Point(ClientSize.Width / 2 - PictureBox1.Size.Width / 2, ClientSize.Height / 2 - (PictureBox1.Height / 2))
        '        PictureBox1.Anchor = AnchorStyles.Top, Bottom, Left, Right
        'PictureBox1.Dock = DockStyle.Fill
        '        ListBox1.SelectedItem = 0

        If MO = False Then
            Button1.PerformClick()
        End If
        If ComboBox1.SelectedItem = "Mega" Then
            PictureBox1.Size = New System.Drawing.Size(PictureBox1.Width, PictureBox1.Height / 2)
        End If
        PictureBox1.Location = New Point(ClientSize.Width / 2 - PictureBox1.Size.Width / 2, ClientSize.Height / 2 - (PictureBox1.Height / 2))
        PictureBox1.Anchor = AnchorStyles.None
        ComboBox1.SelectedItem = Nothing
        PictureBox1.Image = Nothing
        ComboBox1.Enabled = True
        ComboBox1.Visible = True
        Label1.Visible = True
        Button1.Visible = False
        Button2.Visible = False
    End Sub
    ' ZOOM + / -
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If MO = True Then
            PictureBox1.Size = New System.Drawing.Size(PictureBox1.Width * 2, PictureBox1.Height * 2)
            PictureBox1.Location = New Point((PictureBox1.Parent.ClientSize.Width / 2) - (PictureBox1.Width / 2), (PictureBox1.Parent.ClientSize.Height / 2) - (PictureBox1.Height / 2))
            MO = False
        Else
            PictureBox1.Size = New System.Drawing.Size(PictureBox1.Width / 2, PictureBox1.Height / 2)
            PictureBox1.Location = New Point((PictureBox1.Parent.ClientSize.Width / 2) - (PictureBox1.Width / 2), (PictureBox1.Parent.ClientSize.Height / 2) - (PictureBox1.Height / 2))
            MO = True
        End If
    End Sub
End Class