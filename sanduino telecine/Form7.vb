Imports System.IO
Imports System.Threading
Public Class Form7
    Dim img1 As Image
    Dim flipy As Boolean = True
    Dim flipx As Boolean = True
    Dim PLAY As Boolean
    Dim A As String
    Dim B As String
    Dim VIEW As String
    Dim vel As Integer
    Private currentX As Integer, currentY As Integer
    Private isDragging As Boolean = False
    ' APERTURA PROGRAMMA
    Private Sub Form7_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Location = New System.Drawing.Point(My.Settings.Form7_Location.X, My.Settings.Form7_Location.Y)

        If Not Directory.Exists(My.Settings.cart_lav) Then
            TextBox1.Text = ""
        Else
            TextBox1.Text = My.Settings.cart_lav
        End If

        PictureBox1.Image = My.Resources.ANTEPRIMA
        Label1.Text = 0 & " File "
        If TextBox1.Text <> Nothing Then
            Dim files() As String = Directory.GetFiles(TextBox1.Text)
            ListBox1.Items.Clear()
            For Each file As String In files
                ListBox1.Items.Add(Path.GetFileName(file))
            Next
            Dim pat As String
            pat = TextBox1.Text
            Dim filess() As String
            If Directory.Exists(pat) Then
                filess = Directory.GetFiles(pat)
                Label1.Text = filess.Length & " Files "
                If ListBox1.Items.Count.ToString() = 0 Then
                    Exit Sub
                Else
                    ListBox1.SelectedIndex = 0
                    ' ListBox1.SetSelected(ListBox1.Items.Count.ToString() - 1, True)
                End If
            End If
        End If
        PLAY = True
        Button10.Enabled = False
        VIEW = My.Settings.viewer_set
        My.Settings.view_on = True
        vel = 0
    End Sub
    ' CHIUSURA PROGRAMMA
    Private Sub Form7_FormClosing(sender As Object, e As EventArgs) Handles MyBase.FormClosing
        PictureBox1.Image = Nothing
        PLAY = False
        My.Settings.Form7_Location = New System.Drawing.Point(Me.Location.X, Me.Location.Y)
        My.Settings.Save()
        My.Settings.cart_lav = TextBox1.Text
        Timer1.Stop()
        My.Settings.view_on = False
        My.Settings.foto_in2 = False
    End Sub
    ' CAMBIA SELEZIONE LISTA FILES CARTELLA LAVORO
    Private Sub ListBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ListBox1.SelectedIndexChanged
        Try
            PictureBox1.Load(TextBox1.Text & "/" & ListBox1.SelectedItem)
            If flipy = False Then
                img1 = PictureBox1.Image
                img1.RotateFlip(RotateFlipType.RotateNoneFlipY)
                PictureBox1.Image = img1
            End If
            If flipx = False Then
                img1 = PictureBox1.Image
                img1.RotateFlip(RotateFlipType.RotateNoneFlipX)
                PictureBox1.Image = img1
            End If
        Catch ex As Exception
            PictureBox1.Image = My.Resources.ANTEPRIMA
            PLAY = False
            Button9.Enabled = True
            Button11.Enabled = True
            Button10.Enabled = False
        End Try
    End Sub
    ' SELEZIONA CARTELLA DI LAVORO
    Private Sub BrowseButton_Click(sender As Object, e As EventArgs) Handles BrowseButton.Click
        Dim folderDlg As New FolderBrowserDialog
        folderDlg.SelectedPath = TextBox1.Text
        folderDlg.ShowNewFolderButton = True
        PictureBox1.Image = My.Resources.ANTEPRIMA
        Label1.Text = 0 & " File "
        If (folderDlg.ShowDialog() = DialogResult.OK) Then
            TextBox1.Text = folderDlg.SelectedPath
            My.Settings.cart_lav = TextBox1.Text

            If TextBox1.Text <> "" Then
                Dim files() As String = Directory.GetFiles(TextBox1.Text)
                ListBox1.Items.Clear()
                For Each file As String In files
                    ListBox1.Items.Add(Path.GetFileName(file))
                Next
                Dim pat As String
                pat = TextBox1.Text
                Dim filess() As String
                If Directory.Exists(pat) Then
                    filess = Directory.GetFiles(pat)
                    Label1.Text = filess.Length & " Files "
                    If ListBox1.Items.Count.ToString() = 0 Then
                        Exit Sub
                    Else
                        ListBox1.SetSelected(ListBox1.Items.Count.ToString() - 1, True)
                    End If
                End If

            End If
        End If

    End Sub
    ' SPECCHIA IMMAGINI VERTICALE
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If ListBox1.Items.Count = Nothing Then
            Exit Sub
        End If
        If flipy = True Then
            img1 = PictureBox1.Image
            img1.RotateFlip(RotateFlipType.RotateNoneFlipY)
            PictureBox1.Image = img1
            flipy = False
        Else
            flipy = True
            img1 = PictureBox1.Image
            img1.RotateFlip(RotateFlipType.RotateNoneFlipY)
            PictureBox1.Image = img1
        End If
    End Sub
    ' SPECCHIA IMMAGINI ORIZZONTALE
    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        If ListBox1.Items.Count = Nothing Then
            Exit Sub
        End If
        If flipx = True Then
            img1 = PictureBox1.Image
            img1.RotateFlip(RotateFlipType.RotateNoneFlipX)
            PictureBox1.Image = img1
            flipx = False
        Else
            flipx = True
            img1 = PictureBox1.Image
            img1.RotateFlip(RotateFlipType.RotateNoneFlipX)
            PictureBox1.Image = img1
        End If
    End Sub
    ' ZOOM +
    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
        If ListBox1.Items.Count = Nothing Then
            Exit Sub
        End If
        PictureBox1.Size = New System.Drawing.Size(PictureBox1.Width + 400, PictureBox1.Height + 400)
        PictureBox1.Location = New Point((PictureBox1.Parent.ClientSize.Width / 2) - (PictureBox1.Width / 2), (PictureBox1.Parent.ClientSize.Height / 2) - (PictureBox1.Height / 2))
    End Sub
    ' ZOOM -
    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        If ListBox1.Items.Count = Nothing Then
            Exit Sub
        End If
        PictureBox1.Size = New System.Drawing.Size(PictureBox1.Width - 400, PictureBox1.Height - 400)
        PictureBox1.Location = New Point((PictureBox1.Parent.ClientSize.Width / 2) - (PictureBox1.Width / 2), (PictureBox1.Parent.ClientSize.Height / 2) - (PictureBox1.Height / 2))
    End Sub
    ' PAN IMMAGINE 1/3
    Private Sub PictureBox1_MouseDown(sender As Object, e As System.Windows.Forms.MouseEventArgs) Handles PictureBox1.MouseDown
        If ListBox1.Items.Count = Nothing Then
            Exit Sub
        End If
        isDragging = True
        currentX = e.X
        currentY = e.Y
    End Sub
    ' PAN IMMAGINE 2/3
    Private Sub PictureBox1_MouseMove(sender As Object, e As System.Windows.Forms.MouseEventArgs) Handles PictureBox1.MouseMove
        If ListBox1.Items.Count = Nothing Then
            Exit Sub
        End If
        If isDragging Then
            PictureBox1.Top = PictureBox1.Top + (e.Y - currentY)
            PictureBox1.Left = PictureBox1.Left + (e.X - currentX)
        End If
    End Sub
    ' PAN IMMAGINE 3/3
    Private Sub PictureBox1_MouseUp(sender As Object, e As System.Windows.Forms.MouseEventArgs) Handles PictureBox1.MouseUp
        If ListBox1.Items.Count = Nothing Then
            Exit Sub
        End If
        isDragging = False
    End Sub
    ' AVANTI UN FOTOGRAMMA
    Private Sub Button7_Click(sender As Object, e As EventArgs) Handles Button7.Click
        If ListBox1.Items.Count = Nothing Then
            Exit Sub
        End If
        A = ListBox1.FindString(ListBox1.SelectedItem.ToString())
        If A <= ListBox1.Items.Count.ToString() - 2 Then
            A = A + 1
            ListBox1.SetSelected(A, True)
        End If
    End Sub
    ' INDIETRO UN FOTOGRAMMA
    Private Sub Button8_Click(sender As Object, e As EventArgs) Handles Button8.Click
        If ListBox1.Items.Count = Nothing Then
            Exit Sub
        End If
        A = ListBox1.FindString(ListBox1.SelectedItem.ToString())
        If A >= 1 Then
            A = A - 1
            ListBox1.SetSelected(A, True)
        End If
    End Sub
    ' PLAY SLIDE SHOW AVANTI
    Private Sub Button9_Click(sender As Object, e As EventArgs) Handles Button9.Click
        If ListBox1.Items.Count = Nothing Then
            Exit Sub
        End If
        PLAY = True
        Button11.Enabled = False
        B = ListBox1.FindString(ListBox1.SelectedItem.ToString())
        While B <= ListBox1.Items.Count.ToString() - 2
            Application.DoEvents()
            Button9.Enabled = False
            If TrackBar1.Value = 0 Then
                vel = 10
            End If
            If TrackBar1.Value = 1 Then
                vel = 150
            End If
            If TrackBar1.Value = 2 Then
                vel = 300
            End If
            If PLAY = True Then
                Button10.Enabled = True
                B = B + 1
                Thread.Sleep(vel)
                ListBox1.SetSelected(B, True)
            ElseIf PLAY = False Then
                PLAY = True
                Button10.Enabled = False
                Exit While
            End If
        End While
        PLAY = False
        Button9.Enabled = True
        Button11.Enabled = True
        Button10.Enabled = False
    End Sub
    ' PLAY SLIDE SHOW INDIETRO
    Private Sub Button11_Click(sender As Object, e As EventArgs) Handles Button11.Click
        If ListBox1.Items.Count = Nothing Then
            Exit Sub
        End If
        PLAY = True
        Button9.Enabled = False
        B = ListBox1.FindString(ListBox1.SelectedItem.ToString())
        While B >= 1
            Application.DoEvents()
            Button11.Enabled = False
            If TrackBar1.Value = 0 Then
                vel = 10
            End If
            If TrackBar1.Value = 1 Then
                vel = 150
            End If
            If TrackBar1.Value = 2 Then
                vel = 300
            End If
            If PLAY = True Then
                Button10.Enabled = True
                B = B - 1
                System.Threading.Thread.Sleep(vel)
                ListBox1.SetSelected(B, True)
            ElseIf PLAY = False Then
                PLAY = True
                Button10.Enabled = False
                Exit While
            End If
        End While
        PLAY = False
        Button9.Enabled = True
        Button11.Enabled = True
        Button10.Enabled = False
    End Sub
    ' FERMA SLIDE SHOW
    Private Sub Button10_Click(sender As Object, e As EventArgs) Handles Button10.Click
        PLAY = False
        Button9.Enabled = True
        Button11.Enabled = True
        Button10.Enabled = False
    End Sub
    ' CENTRA E ADDATA IMMAGINE
    Private Sub Button6_Click_1(sender As Object, e As EventArgs) Handles Button6.Click
        If ListBox1.Items.Count = Nothing Then
            Exit Sub
        End If
        PictureBox1.Size = New System.Drawing.Size(Panel1.Width - 10, Panel1.Height - 10)
        PictureBox1.Location = New Point((PictureBox1.Parent.ClientSize.Width / 2) - (PictureBox1.Width / 2), (PictureBox1.Parent.ClientSize.Height / 2) - (PictureBox1.Height / 2))
    End Sub
    ' AGGIORNA CONTENUTO CARTELLA
    Private Sub Button12_Click(sender As Object, e As EventArgs) Handles Button12.Click
        TextBox1.Text = My.Settings.cart_lav
        PictureBox1.Image = My.Resources.ANTEPRIMA
        Label1.Text = 0 & " File "
        If TextBox1.Text <> "" Then
            Dim files() As String = Directory.GetFiles(TextBox1.Text)
            ListBox1.Items.Clear()
            For Each file As String In files
                ListBox1.Items.Add(Path.GetFileName(file))
            Next
            Dim pat As String
            pat = TextBox1.Text
            Dim filess() As String
            If Directory.Exists(pat) Then
                filess = Directory.GetFiles(pat)
                Label1.Text = filess.Length & " Files "
                If ListBox1.Items.Count.ToString() = 0 Then
                    Exit Sub
                Else
                    ListBox1.SetSelected(ListBox1.Items.Count.ToString() - 1, True)
                End If
            End If
        End If
    End Sub
    ' CANCELLA IMMAGINE DAL DISCO
    Private Sub Button13_Click(sender As Object, e As EventArgs) Handles Button13.Click
        If ListBox1.Items.Count = Nothing Then
            Exit Sub
        End If
        If MsgBox("Eliminare il File?" & vbCrLf & vbCrLf & "...\" & ListBox1.SelectedItem, MsgBoxStyle.YesNo, ) = MsgBoxResult.Yes Then
            Dim DA As String = (TextBox1.Text & "\" & ListBox1.SelectedItem)
            PictureBox1.Image = Nothing
            Dim li As Integer
            For li = ListBox1.SelectedIndices.Count - 1 To 0 Step -1

                ListBox1.Items.RemoveAt(ListBox1.SelectedIndices(li))
            Next
            My.Computer.FileSystem.DeleteFile(DA)
            DA = 0
            If TextBox1.Text <> Nothing Then
                Dim files() As String = Directory.GetFiles(TextBox1.Text)
                Dim pat As String
                pat = TextBox1.Text
                Dim filess() As String
                If Directory.Exists(pat) Then
                    filess = Directory.GetFiles(pat)
                    Label1.Text = filess.Length & " Files "
                    'ListBox1.SetSelected(ListBox1.Items.Count - 1, True)
                End If
            End If
            If ListBox1.Items.Count = Nothing Then
                Exit Sub
            Else
                ListBox1.SetSelected(ListBox1.Items.Count - 1, True)
            End If
        End If
    End Sub
    ' APRI O SELEZIONA VIEWER ESTERNO
    Private Sub Button14_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles Button14.MouseDown
        If ListBox1.Items.Count = Nothing Then
            Exit Sub
        End If
        If e.Button = Windows.Forms.MouseButtons.Right Then
            If MsgBox("Cambiare Viewer Esterno?", MsgBoxStyle.YesNo, ) = MsgBoxResult.Yes Then
                VIEW = ""
                Dim folderDlg As New OpenFileDialog
                If (folderDlg.ShowDialog() = DialogResult.OK) Then
                    VIEW = folderDlg.FileName
                    My.Settings.viewer_set = VIEW
                End If
            Else
                Exit Sub
            End If
        Else
            If VIEW = "" Then
                Dim folderDlg As New OpenFileDialog
                If (folderDlg.ShowDialog() = DialogResult.OK) Then
                    VIEW = folderDlg.FileName
                    My.Settings.viewer_set = VIEW
                End If
            Else
                Process.Start(VIEW, TextBox1.Text & "\" & ListBox1.SelectedItem)
            End If
        End If
    End Sub
    ' ESTRAE FILES DA ELENCO 2
    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        If My.Settings.foto_in2 = True Then
            Dim no As String = My.Settings.solo_file
            ListBox1.Items.Add(no)
            If ListBox1.Text <> Nothing Then
                ListBox1.SetSelected(ListBox1.Items.Count.ToString() - 1, True)
            End If
            Dim pat As String
            pat = TextBox1.Text
            Dim filess() As String
            If Directory.Exists(pat) Then
                filess = Directory.GetFiles(pat)
                Label1.Text = filess.Length & " Files "
            End If
            My.Settings.foto_in2 = False
        End If
    End Sub
    ' APRI CARTELLA IN ESPLORA RISORSE
    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        If TextBox1.Text <> Nothing Then
            Process.Start("explorer.exe", TextBox1.Text)
        End If
    End Sub
    ' INFORMAZIONI FILE
    Private Sub ListBox1_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles ListBox1.MouseDown
        If ListBox1.Items.Count = Nothing Then
            Exit Sub
        End If
        If e.Button = MouseButtons.Right Then
            ListBox1.SelectedIndex = ListBox1.IndexFromPoint(e.X, e.Y)
            Dim MyimageWxH
            Try
                Dim Myimg As System.Drawing.Image = System.Drawing.Image.FromFile(TextBox1.Text & "\" & ListBox1.SelectedItem)
                Dim MyImageWidth = PictureBox1.Image.Width
                Dim MyImageHeight = PictureBox1.Image.Height
                MyimageWxH = MyImageWidth & " X " & MyImageHeight
            Catch
                MyimageWxH = "Not Supported"
            End Try
            Dim Myimagepropety = FileLen(TextBox1.Text & "\" & ListBox1.SelectedItem)
            Dim Myimagedate As DateTime = File.GetCreationTime(TextBox1.Text & "\" & ListBox1.SelectedItem)
            MsgBox("Informazioni:" & vbCrLf & vbCrLf & TextBox1.Text & "\" & ListBox1.SelectedItem & vbCrLf & vbCrLf & " Data Creazione:  " & Myimagedate & vbCrLf & vbCrLf & " Formato:  " & MyimageWxH & vbCrLf & vbCrLf & " Dimensione:  " & Math.Round(Myimagepropety / 1000, 1) & " KB", MessageBoxIcon.Information)
        End If
    End Sub
End Class
