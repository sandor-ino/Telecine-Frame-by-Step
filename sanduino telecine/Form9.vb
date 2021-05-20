Imports System.IO
Imports Accord.Video.FFMPEG
Public Class Form9
    Private reader As VideoFileReader = New VideoFileReader()
    Private writer As VideoFileWriter = New VideoFileWriter()
    Dim rename As Boolean = False
    Dim OCC As Boolean = False
    Dim STP As Boolean = False
    Dim STPP As Boolean = False
    Dim RENA As Boolean = False
    ' APERTURA PROGRAMMA
    Private Sub Form9_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Location = New Point(My.Settings.Form9_Location.X, My.Settings.Form9_Location.Y)

        If Not Directory.Exists(My.Settings.CA_SOVID) Then
            TextBox4.Text = ""
        Else
            TextBox4.Text = My.Settings.CA_SOVID
        End If
        If Not Directory.Exists(My.Settings.CA_DEVID) Then
            TextBox1.Text = ""
        Else
            TextBox1.Text = My.Settings.CA_DEVID
        End If

        Button26.PerformClick()
        Button27.PerformClick()
        NumericUpDown1.Value = My.Settings.FTP_1
        ComboBox1.Text = My.Settings.QUA_1
        NumericUpDown3.Value = My.Settings.BTR_1
        ComboBox3.Text = My.Settings.RES_1
    End Sub
    ' CHIUSURA PROGRAMMA
    Private Sub Form9_Closed(sender As Object, e As EventArgs) Handles MyBase.Closed
        My.Settings.Form9_Location = New Point(Me.Location.X, Me.Location.Y)
        My.Settings.Save()

    End Sub
    ' SELEZIONA IMPOSTAZIONE 1
    Private Sub RadioButton1_CheckedChanged(sender As Object, e As EventArgs) Handles RadioButton1.CheckedChanged
        If RadioButton1.Checked = True Then
            RadioButton1.BackgroundImage = My.Resources.i1red
            NumericUpDown1.Value = My.Settings.FTP_1
            ComboBox1.Text = My.Settings.QUA_1
            NumericUpDown3.Value = My.Settings.BTR_1
            ComboBox3.Text = My.Settings.RES_1
        Else
            RadioButton1.BackgroundImage = My.Resources.i1
        End If

    End Sub
    ' SELEZIONA IMPOSTAZIONE 2
    Private Sub RadioButton2_CheckedChanged(sender As Object, e As EventArgs) Handles RadioButton2.CheckedChanged
        If RadioButton2.Checked = True Then
            RadioButton2.BackgroundImage = My.Resources.i2red
            NumericUpDown1.Value = My.Settings.FTP_2
            ComboBox1.Text = My.Settings.QUA_2
            NumericUpDown3.Value = My.Settings.BTR_2
            ComboBox3.Text = My.Settings.RES_2
        Else
            RadioButton2.BackgroundImage = My.Resources.i2
        End If
    End Sub
    ' SELEZIONA IMPOSTAZIONE 3
    Private Sub RadioButton3_CheckedChanged(sender As Object, e As EventArgs) Handles RadioButton3.CheckedChanged
        If RadioButton3.Checked = True Then
            RadioButton3.BackgroundImage = My.Resources.i3red
            NumericUpDown1.Value = My.Settings.FTP_3
            ComboBox1.Text = My.Settings.QUA_3
            NumericUpDown3.Value = My.Settings.BTR_3
            ComboBox3.Text = My.Settings.RES_3
        Else
            RadioButton3.BackgroundImage = My.Resources.i3
        End If
    End Sub
    ' SELEZIONA CARTELLA DI ORIGINE 
    Private Sub Button18_Click(sender As Object, e As EventArgs) Handles Button18.Click
        Dim folderDlg As New FolderBrowserDialog
        folderDlg.SelectedPath = TextBox4.Text
        folderDlg.ShowNewFolderButton = True
        PictureBox1.Image = My.Resources.ANTEPRIMA
        Label8.Text = 0 & " File "
        If (folderDlg.ShowDialog() = DialogResult.OK) Then
            TextBox4.Text = folderDlg.SelectedPath
            If TextBox4.Text = TextBox1.Text Then
                MessageBox.Show("la cartella di Origine non puo essere" & vbCrLf & vbCrLf & "la cartella di Destinazione")
                TextBox4.Text = ""
                ListBox1.Items.Clear()
                Button18.PerformClick()
                Exit Sub
            End If
            If TextBox4.Text <> "" Then
                My.Settings.CA_SOVID = TextBox4.Text
                Dim files() As String = Directory.GetFiles(TextBox4.Text)
                ListBox1.Items.Clear()
                For Each file As String In files
                    ListBox1.Items.Add(Path.GetFileName(file))
                Next
                Dim pat As String
                pat = TextBox4.Text
                Dim filess() As String
                If Directory.Exists(pat) Then
                    filess = Directory.GetFiles(pat)
                    Label8.Text = filess.Length & " Files "
                    If ListBox1.Items.Count.ToString() = 0 Then
                        Exit Sub
                    Else
                        ListBox1.SelectedIndex = 0
                    End If
                End If
            End If
        End If

    End Sub
    ' SELEZIONA CARTELLA DI DESTINAZIONE 
    Private Sub Button14_Click(sender As Object, e As EventArgs) Handles Button14.Click
        Dim folderDlg As New FolderBrowserDialog
        folderDlg.SelectedPath = TextBox1.Text
        folderDlg.ShowNewFolderButton = True
        PictureBox1.Image = My.Resources.ANTEPRIMA
        Label9.Text = 0 & " File "
        If (folderDlg.ShowDialog() = DialogResult.OK) Then
            TextBox1.Text = folderDlg.SelectedPath
            If TextBox4.Text = TextBox1.Text Then
                MessageBox.Show("la cartella di Origine non puo essere" & vbCrLf & vbCrLf & "la cartella di Destinazione")
                TextBox1.Text = ""
                ListBox2.Items.Clear()
                Button14.PerformClick()
                Exit Sub
            End If
            If TextBox1.Text <> "" Then
                My.Settings.CA_DEVID = TextBox1.Text
                Dim files() As String = Directory.GetFiles(TextBox1.Text)
                ListBox2.Items.Clear()
                For Each file As String In files
                    ListBox2.Items.Add(Path.GetFileName(file))
                Next
                Dim pat As String
                pat = TextBox1.Text
                Dim filess() As String
                If Directory.Exists(pat) Then
                    filess = Directory.GetFiles(pat)
                    Label9.Text = filess.Length & " Files "
                End If
            End If
        End If

    End Sub
    ' CAMBIO SELEZIONE LISTA CATRELLA SORGENTE  
    Private Sub ListBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ListBox1.SelectedIndexChanged
        If RENA = True Then
            If PictureBox1.Image IsNot Nothing Then
                PictureBox1.Image.Dispose()
                Image.FromFile(TextBox4.Text & "/" & ListBox1.SelectedItem).Dispose()
            End If
            Exit Sub

        Else
            If OCC <> True Then
                If PictureBox1.Image IsNot Nothing Then
                    PictureBox1.Image.Dispose()
                    PictureBox1.Image = Nothing

                End If
                Try
                    PictureBox1.Image = Image.FromFile(TextBox4.Text & "/" & ListBox1.SelectedItem)

                Catch ex As Exception
                    PictureBox1.Image = My.Resources.ANTEPRIMA

                End Try

            End If
            If rename = False Then
                Dim ratio As String = ListBox1.SelectedItem
                Dim split = ratio.Split("_", 2, StringSplitOptions.RemoveEmptyEntries)
                TextBox2.Text = split(0)
            End If
        End If

    End Sub
    ' RINOMINA FILE DI DESTINAZIONE
    Private Sub Button15_Click(sender As Object, e As EventArgs) Handles Button15.Click
        If rename = True Then
            TextBox2.ReadOnly = True
            If TextBox4.Text <> Nothing Then
                Dim ratio As String = ListBox1.SelectedItem
                Dim split = ratio.Split("_", 2, StringSplitOptions.RemoveEmptyEntries)
                TextBox2.Text = split(0)
            End If
            rename = False
        Else
            TextBox2.ReadOnly = False
            rename = True
        End If
    End Sub
    ' AVVIO CREAZIONE VIDEO
    Private Sub Button16_Click(sender As Object, e As EventArgs) Handles Button16.Click

        If TextBox4.Text <> Nothing Then
            If TextBox1.Text <> Nothing Then
                Dim newfile As String = TextBox1.Text & "\" & TextBox2.Text & "." & ComboBox6.Text
                If My.Computer.FileSystem.FileExists(newfile) Then
                    My.Computer.Audio.PlaySystemSound(System.Media.SystemSounds.Beep)
                    Select Case MsgBox("ATTENZIONE!" & vbCrLf & vbCrLf & "Il File  ...\" & TextBox2.Text & "." & ComboBox6.Text & vbCrLf & vbCrLf & "E' Esistente, Sovrascrivere?", MessageBoxButtons.YesNo)
                        Case Windows.Forms.DialogResult.Yes
                            Exit Select
                        Case Windows.Forms.DialogResult.No
                            Exit Sub
                    End Select
                End If

                If ListBox1.SelectedIndex <> -1 Then
                    ListBox1.SelectedIndex = 0
                    DisableAll()

                    Dim MIS As Image = Drawing.Image.FromFile(TextBox4.Text & "/" & ListBox1.SelectedItem)
                    Dim BAS As Integer = MIS.Width
                    Dim ALT As Integer = MIS.Height
                    Select Case ComboBox3.Text
                        Case "Original"
                            Exit Select
                        Case "p480"
                            BAS = ((480 * BAS) / ALT)
                            If BAS Mod (2) <> 0 Then
                                BAS += 1
                            End If
                            ALT = 480
                        Case "p720"
                            BAS = ((720 * BAS) / ALT)
                            If BAS Mod (2) <> 0 Then
                                BAS += 1
                            End If
                            ALT = 720
                        Case "p1080"
                            BAS = ((1080 * BAS) / ALT)
                            If BAS Mod (2) <> 0 Then
                                BAS += 1
                            End If
                            ALT = 1080
                    End Select
                    Dim FRM As Integer = NumericUpDown1.Value
                    Dim BTR As Integer = NumericUpDown3.Value * 1000
                    OCC = True
                    PictureBox1.Image = Nothing
                    PictureBox1.Image = My.Resources.WITE
                    Select Case ComboBox1.Text
                        Case "Default"
                            writer.Open(TextBox1.Text & "\" & TextBox2.Text & "." & ComboBox6.Text, BAS, ALT, FRM, VideoCodec.Default, BTR)
                        Case "MPEG2"
                            writer.Open(TextBox1.Text & "\" & TextBox2.Text & "." & ComboBox6.Text, BAS, ALT, FRM, VideoCodec.MPEG2, BTR)
                        Case "H264"
                            writer.Open(TextBox1.Text & "\" & TextBox2.Text & "." & ComboBox6.Text, BAS, ALT, FRM, VideoCodec.H264, BTR)
                        Case "FLV1"
                            writer.Open(TextBox1.Text & "\" & TextBox2.Text & "." & ComboBox6.Text, BAS, ALT, FRM, VideoCodec.FLV1, BTR)
                    End Select
                    Dim image As Bitmap
                    Dim A As Integer = 0
                    ProgressBar1.Maximum = ListBox1.Items.Count.ToString() - 1
                    For i As Integer = 0 To ListBox1.Items.Count.ToString() - 1
                        Application.DoEvents()
                        ProgressBar1.PerformStep()
                        ProgressBar1.Value = A
                        A += 1
                        If STP = True Then
                            writer.Close()
                            PictureBox1.Image = Nothing
                            My.Computer.Audio.PlaySystemSound(Media.SystemSounds.Beep)
                            If MsgBox("CREAZIONE VIDEO ANNULLATA!" & vbCrLf & vbCrLf & "Cancellare Il File  ...\" & TextBox2.Text & "." & ComboBox6.Text, MessageBoxButtons.YesNo) = DialogResult.Yes Then
                                My.Computer.FileSystem.DeleteFile(newfile)
                                Button27.PerformClick()
                            End If
                            EnableAll()
                            OCC = False
                            STP = False
                            ProgressBar1.Value = 0
                            Button27.PerformClick()
                            Exit Sub
                        Else
                            image = Drawing.Image.FromFile(TextBox4.Text & "/" & ListBox1.SelectedItem)
                            If ComboBox1.Text = "Default" And ComboBox3.Text = "Original" Then
                                writer.WriteVideoFrame(image)
                                image.Dispose()
                            Else
                                Dim newImage As Image = New Bitmap(BAS, ALT)
                                Dim H As Graphics = Graphics.FromImage(newImage)
                                H.InterpolationMode = Drawing2D.InterpolationMode.HighQualityBicubic
                                H.PixelOffsetMode = Drawing2D.PixelOffsetMode.HighQuality
                                H.CompositingQuality = Drawing2D.CompositingQuality.HighQuality
                                H.DrawImage(image, 0, 0, BAS, ALT)
                                writer.WriteVideoFrame(newImage)
                                image.Dispose()
                                H.Dispose()
                                newImage.Dispose()
                            End If
                            'System.Threading.Thread.Sleep(250)
                            If ListBox1.Items.Count.ToString() - 1 <> A Then
                                ListBox1.SelectedIndex += 1
                            End If
                        End If
                    Next
                    writer.Close()
                    ListBox1.SelectedIndex = 0
                    PictureBox1.Image = Nothing
                    MsgBox("OPERAZIONE ESEGUITA, CON SUCCESSO." & vbCrLf & vbCrLf & "File Video Creato: " & vbCrLf & vbCrLf & newfile, MessageBoxIcon.Information)
                    EnableAll()
                    OCC = False
                    ProgressBar1.Value = 0
                    ListBox1.SelectedIndex = 0
                    Button27.PerformClick()
                End If

            Else
                MsgBox("CARTELLA DESTINAZIONE VUOTA!")
            End If

        Else
            MsgBox("CARTELLA DI ORIGINE VUOTA!")
        End If
    End Sub
    ' AGGIORNA CONTENUTO LISTA CARTELLA SORGENTE
    Private Sub Button26_Click(sender As Object, e As EventArgs) Handles Button26.Click
        PictureBox1.Image = My.Resources.ANTEPRIMA
        Label8.Text = 0 & " File "
        If TextBox4.Text <> "" Then
            Dim files() As String = Directory.GetFiles(TextBox4.Text)
            ListBox1.Items.Clear()
            For Each file As String In files
                ListBox1.Items.Add(Path.GetFileName(file))
            Next
            Dim pat As String
            pat = TextBox4.Text
            Dim filess() As String
            If Directory.Exists(pat) Then
                filess = Directory.GetFiles(pat)
                Label8.Text = filess.Length & " Files "
                If ListBox1.Items.Count.ToString() = 0 Then
                    Exit Sub
                Else
                    ListBox1.SelectedIndex = 0
                End If
            End If
        End If
    End Sub
    ' AGGIORNA CONTENUTO LISTA CARTELLA DESTINAZIONE
    Private Sub Button27_Click(sender As Object, e As EventArgs) Handles Button27.Click

        Label9.Text = 0 & " File "
        If TextBox1.Text <> "" Then
            Dim files() As String = Directory.GetFiles(TextBox1.Text)
            ListBox2.Items.Clear()
            For Each file As String In files
                ListBox2.Items.Add(Path.GetFileName(file))
            Next

            Dim pat As String = TextBox1.Text
            Dim filess() As String
            If Directory.Exists(pat) Then
                filess = Directory.GetFiles(TextBox1.Text)
                Label9.Text = filess.Length & " Files "
            End If
        End If
    End Sub
    ' CANCELLA FILE SELEZIONATO IN CARTELLA DESTINAZIONE
    Private Sub Button29_Click(sender As Object, e As EventArgs) Handles Button29.Click
        If ListBox2.Items.Count = Nothing Then
            Exit Sub
        End If
        If MsgBox("Eliminare il File?" & vbCrLf & vbCrLf & "...\" & ListBox2.SelectedItem, MsgBoxStyle.YesNo, ) = MsgBoxResult.Yes Then
            Dim DA As String = (TextBox1.Text & "\" & ListBox2.SelectedItem)
            PictureBox1.Image = Nothing
            Dim li As Integer
            For li = ListBox2.SelectedIndices.Count - 1 To 0 Step -1
                ListBox2.Items.RemoveAt(ListBox2.SelectedIndices(li))
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
                    Label9.Text = filess.Length & " Files "
                End If
            End If
        End If
    End Sub
    ' APRE CARTELLA DESTINAZIONE IN ESPLORA RISORSE
    Private Sub Button23_Click(sender As Object, e As EventArgs) Handles Button23.Click
        If TextBox1.Text <> Nothing Then
            Process.Start("explorer.exe", TextBox1.Text)
        End If
    End Sub
    ' APRE CARTELLA SORGENTE IN ESPLORA RISORSE
    Private Sub Button19_Click(sender As Object, e As EventArgs) Handles Button19.Click
        If TextBox4.Text <> Nothing Then
            Process.Start("explorer.exe", TextBox4.Text)
        End If
    End Sub
    'APRI/SELEZIONA VIEWER ESTERNO DA LISTA CARTELLA SORGENTE
    Private Sub Button24_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles Button24.MouseDown
        If ListBox1.Items.Count = Nothing Then
            Exit Sub
        End If
        Dim VIEW As String = My.Settings.viewer_set
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
                Process.Start(VIEW, TextBox4.Text & "\" & ListBox1.SelectedItem)
            End If
        End If
    End Sub
    ' APRI/SELEZIONA LETTORE VIDEO ESTERNO
    Private Sub Button25_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles Button25.MouseDown
        If ListBox2.Items.Count = Nothing Then
            Exit Sub
        End If
        Dim PREVID As String = My.Settings.PRE_VID
        If e.Button = Windows.Forms.MouseButtons.Right Then
            If MsgBox("Cambiare Lettore VIDEO Esterno?", MsgBoxStyle.YesNo, ) = MsgBoxResult.Yes Then
                PREVID = ""
                Dim folderDlg As New OpenFileDialog
                If (folderDlg.ShowDialog() = DialogResult.OK) Then
                    PREVID = folderDlg.FileName
                    My.Settings.PRE_VID = PREVID
                End If
            Else
                Exit Sub
            End If
        Else

            If PREVID = "" Then
                Dim folderDlg As New OpenFileDialog
                If (folderDlg.ShowDialog() = DialogResult.OK) Then
                    PREVID = folderDlg.FileName
                    My.Settings.PRE_VID = PREVID
                End If
            Else
                If ListBox2.SelectedIndex > -1 Then
                    Process.Start(PREVID, TextBox1.Text & "\" & ListBox2.SelectedItem)
                End If
            End If
        End If

    End Sub
    ' INFORMAZIONI FILE IMMAGINI CARTELLA SORGENTE
    Private Sub ListBox1_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles ListBox1.MouseDown
        If ListBox1.Items.Count = Nothing Then
            Exit Sub
        End If
        If e.Button = MouseButtons.Right Then
            If ListBox1.SelectedIndex > -1 Then
                ListBox1.SelectedIndex = ListBox1.IndexFromPoint(e.X, e.Y)
                Dim MyimageWxH As String = ""
                Dim Myimg As Image = Image.FromFile(TextBox4.Text & "\" & ListBox1.SelectedItem)
                Dim MyImageWidth = Myimg.Width
                Dim MyImageHeight = Myimg.Height
                MyimageWxH = MyImageWidth & " X " & MyImageHeight
                Dim Myimagepropety = FileLen(TextBox4.Text & "\" & ListBox1.SelectedItem)
                Dim Myimagedate As DateTime = File.GetCreationTime(TextBox4.Text & "\" & ListBox1.SelectedItem)
                MsgBox("Informazioni:" & vbCrLf & vbCrLf & TextBox4.Text & "\" & ListBox1.SelectedItem & vbCrLf & vbCrLf & " Data: " & Myimagedate & vbCrLf & vbCrLf & " Formato: " & MyimageWxH & vbCrLf & vbCrLf & " Dimensione: " & Math.Round(Myimagepropety / 1000, 1) & " KB", MessageBoxIcon.Information)
            End If
        End If

    End Sub
    ' INFORMAZIONI FILE VIDEO CARTELLA DESTINAZIONE
    Private Sub ListBox2_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles ListBox2.MouseDown
        If ListBox2.Items.Count = Nothing Then
            Exit Sub
        End If
        If e.Button = MouseButtons.Right Then
            ListBox2.SelectedIndex = ListBox2.IndexFromPoint(e.X, e.Y)
            Dim MyimageWxH As String = ""
            Dim MyImageWidth As Integer, MyImageHeight As Integer
            Dim FRMR As Integer, FRAM As Integer, BITR As Integer
            Dim CODEC As String = ""
            Dim DIME As Long
            Dim Myimagedate As DateTime
            Dim s As Integer, m As Integer, h As Integer
            Dim DUR As Long, DURA As String = ""
            If ListBox2.SelectedIndex > -1 Then
                DIME = FileLen(TextBox1.Text & "\" & ListBox2.SelectedItem)
                Myimagedate = File.GetCreationTime(TextBox1.Text & "\" & ListBox2.SelectedItem)
                reader.Open(TextBox1.Text & "\" & ListBox2.SelectedItem)
                FRMR = (reader.FrameRate)
                CODEC = (reader.CodecName)
                FRAM = (reader.FrameCount)
                BITR = (reader.BitRate) / 1000
                MyImageWidth = reader.Width
                MyImageHeight = reader.Height
                MyimageWxH = reader.Width & " X " & reader.Height
                If reader.IsOpen = True Then
                    reader.Close()
                End If
                DUR = DIME / ((DIME / FRAM) * FRMR)
                s = DUR Mod 60
                DUR = Int(DUR / 60)
                m = DUR Mod 60
                DUR = Int(DUR / 60)
                h = DUR
                DURA = Format(h, "00") & ":" & Format(m, "00") & ":" & Format(s, "00")
            End If
            MsgBox("Informazioni:" & vbCrLf & vbCrLf & TextBox1.Text & "\" & ListBox2.SelectedItem & vbCrLf & vbCrLf & "Data: " & Myimagedate & vbCrLf & vbCrLf & "Formato: " & MyimageWxH & vbCrLf & "Framerate: " & FRMR & vbCrLf & "Bitrate: " & BITR & " Kbps" & vbCrLf & "Codec: " & CODEC & vbCrLf & "N. Frame: " & FRAM & vbCrLf & "Dimensione: " & Math.Round(DIME / 1000000, 1) & " MB" & vbCrLf & "Durata: " & DURA, MessageBoxIcon.Information)
        End If

    End Sub
    ' SALVA IMPOSTAZIONI 1 2 3
    Private Sub Button13_Click(sender As Object, e As EventArgs) Handles Button13.Click
        If RadioButton1.Checked = True Then
            My.Settings.FTP_1 = NumericUpDown1.Value
            My.Settings.QUA_1 = ComboBox1.Text
            My.Settings.BTR_1 = NumericUpDown3.Value
            My.Settings.RES_1 = ComboBox3.Text
        End If
        If RadioButton2.Checked = True Then
            My.Settings.FTP_2 = NumericUpDown1.Value
            My.Settings.QUA_2 = ComboBox1.Text
            My.Settings.BTR_2 = NumericUpDown3.Value
            My.Settings.RES_2 = ComboBox3.Text
        End If
        If RadioButton3.Checked = True Then
            My.Settings.FTP_3 = NumericUpDown1.Value
            My.Settings.QUA_3 = ComboBox1.Text
            My.Settings.BTR_3 = NumericUpDown3.Value
            My.Settings.RES_3 = ComboBox3.Text
        End If
    End Sub
    ' FERMA CRAZIONE VIDEO
    Private Sub Button28_Click(sender As Object, e As EventArgs) Handles Button28.Click
        STP = True
        EnableAll()
    End Sub
    ' CAMBIO SELEZIONE CARTELLA VIDEO
    Private Sub ListBox2_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ListBox2.SelectedIndexChanged

        If reader.IsOpen = True Then
            reader.Close()
            PictureBox1.Image.Dispose()
            Button3.PerformClick()
        End If

    End Sub
    ' PLAY ANTEPRIMA VIDEO IN PITUREBOX
    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        If ListBox2.Items.Count = Nothing Then
            Exit Sub
        End If
        ListBox1.Enabled = False
        ListBox2.Enabled = False
        Button2.Enabled = False
        Button3.Enabled = True
        If ListBox2.SelectedIndex > -1 Then
            reader.Open(TextBox1.Text & "\" & ListBox2.SelectedItem)
            Dim P As Integer = 1000 / reader.FrameRate
            For i As Integer = 0 To reader.FrameCount() - 1
                Application.DoEvents()
                Threading.Thread.Sleep(CInt(P * 0.5))
                If STPP = True Then
                    If reader.IsOpen = True Then
                        reader.Close()
                    End If
                    STPP = False
                    Exit For
                End If
                If PictureBox1.Image IsNot Nothing Then
                    PictureBox1.Image.Dispose()
                End If

                Try
                    PictureBox1.Image = DirectCast(reader.ReadVideoFrame, Bitmap)
                Catch ex As Exception

                End Try

            Next
            If reader.IsOpen = True Then
                reader.Close()
            End If

        End If
        ListBox1.Enabled = True
        ListBox2.Enabled = True
        Button2.Enabled = True
        Button3.Enabled = False
    End Sub
    ' STOP ANTEPRIMA VIDEO
    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        STPP = True
        ListBox1.Enabled = True
        ListBox2.Enabled = True
        Button2.Enabled = True
        Button3.Enabled = False
    End Sub
    ' DISBILITA TUTTI I CONTROLLI TRANNE STOP
    Private Sub DisableAll()
        Button1.Enabled = False
        Button18.Enabled = False
        Button19.Enabled = False
        Button24.Enabled = False
        Button26.Enabled = False
        Button14.Enabled = False
        Button15.Enabled = False
        Button23.Enabled = False
        Button25.Enabled = False
        Button29.Enabled = False
        Button27.Enabled = False
        Button13.Enabled = False
        Button16.Enabled = False ' button start
        Button28.Enabled = True ' button stop
        Button2.Enabled = False
        Button3.Enabled = False
        ListBox1.Enabled = False
        ListBox2.Enabled = False
        TextBox2.Enabled = False
        ComboBox6.Enabled = False
        ComboBox1.Enabled = False
        ComboBox3.Enabled = False
        RadioButton1.Enabled = False
        RadioButton2.Enabled = False
        RadioButton3.Enabled = False
        NumericUpDown1.Enabled = False
        NumericUpDown3.Enabled = False
        TextBox3.Enabled = False
        TextBox5.Enabled = False
        NumericUpDown2.Enabled = False
        Cancel_Button.Enabled = False
        OK_Button.Enabled = False

    End Sub
    ' ABILITA TUTTI I CONTROLLI TRANNE STOP
    Private Sub EnableAll()
        Button1.Enabled = True

        Button18.Enabled = True
        Button19.Enabled = True
        Button24.Enabled = True
        Button26.Enabled = True
        Button14.Enabled = True
        Button15.Enabled = True
        Button23.Enabled = True
        Button25.Enabled = True
        Button29.Enabled = True
        Button27.Enabled = True
        Button13.Enabled = True
        Button16.Enabled = True ' button start
        Button28.Enabled = False ' button stop
        Button2.Enabled = True
        Button3.Enabled = True
        ListBox1.Enabled = True
        ListBox2.Enabled = True
        TextBox2.Enabled = True
        ComboBox6.Enabled = True
        ComboBox1.Enabled = True
        ComboBox3.Enabled = True
        RadioButton1.Enabled = True
        RadioButton2.Enabled = True
        RadioButton3.Enabled = True
        NumericUpDown1.Enabled = True
        NumericUpDown3.Enabled = True
        TextBox3.Enabled = True
        TextBox5.Enabled = True
        NumericUpDown2.Enabled = True
        Cancel_Button.Enabled = True
        OK_Button.Enabled = True
    End Sub
    ' APRI FINESTRA RINOMINA TUTTI I FILES
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click

        If ListBox1.Items.Count > 0 Then
            RENA = True
            Panel1.Visible = True
            ListBox1.Enabled = False
            ListBox1.SelectedIndex = 0
            If PictureBox1.Image IsNot Nothing Then
                PictureBox1.Image.Dispose()
                PictureBox1.Image = Nothing
                Image.FromFile(TextBox4.Text & "/" & ListBox1.SelectedItem).Dispose()

            End If
            TextBox5.Text = ""
            NumericUpDown2.Value = 1
            TextBox3.Text = ""
            Dim ratio As String = ListBox1.SelectedItem.ToString
            Dim split2 = ratio.Split(".", 2, StringSplitOptions.RemoveEmptyEntries)
            TextBox3.Text = split2(1)
        End If
    End Sub
    ' CHIUDI FINESTRA RINOMINA TUTTI I FILES
    Private Sub Cancel_Button_Click(sender As Object, e As EventArgs) Handles Cancel_Button.Click
        RENA = False
        ListBox1.SelectedIndex = 1
        ListBox1.SelectedIndex = 0
        Panel1.Visible = False
        ListBox1.Enabled = True
    End Sub
    ' CAMBIA NOME IN RINOMINA
    Private Sub TextBox5_TextChanged(sender As Object, e As EventArgs) Handles TextBox5.TextChanged
        If NumericUpDown2.Text = "" Or TextBox3.Text = "" Then
            Label11.Text = "...\..."
            Label10.Text = "...\..."
            Label2.Text = "...\..."
        Else
            Label11.Text = "...\" & TextBox5.Text & "_" & NumericUpDown2.Text.PadLeft(5, "0") & "." & TextBox3.Text
            Dim n As String = NumericUpDown2.Value + 1
            Label10.Text = "...\" & TextBox5.Text & "_" & n.PadLeft(5, "0") & "." & TextBox3.Text
            n += 1
            Label2.Text = "...\" & TextBox5.Text & "_" & n.PadLeft(5, "0") & "." & TextBox3.Text

        End If
    End Sub
    ' CAMBIA NUMERO IN RINOMINA
    Private Sub NumericUpDown2_ValueChanged(sender As Object, e As EventArgs) Handles NumericUpDown2.ValueChanged
        If TextBox5.Text = "" Or TextBox3.Text = "" Then
            Label11.Text = "...\..."
            Label10.Text = "...\..."
            Label2.Text = "...\..."
        Else
            Label11.Text = "...\" & TextBox5.Text & "_" & NumericUpDown2.Text.PadLeft(5, "0") & "." & TextBox3.Text
            Dim n As String = NumericUpDown2.Value + 1
            Label10.Text = "...\" & TextBox5.Text & "_" & n.PadLeft(5, "0") & "." & TextBox3.Text
            n += 1
            Label2.Text = "...\" & TextBox5.Text & "_" & n.PadLeft(5, "0") & "." & TextBox3.Text

        End If
    End Sub
    ' CAMBIA ESTENZIONE IN RINOMINA
    Private Sub TextBox3_TextChanged(sender As Object, e As EventArgs) Handles TextBox3.TextChanged
        If NumericUpDown2.Text = "" Or TextBox5.Text = "" Then
            Label11.Text = "...\..."
            Label10.Text = "...\..."
            Label2.Text = "...\..."
        Else
            Label11.Text = "...\" & TextBox5.Text & "_" & NumericUpDown2.Text.PadLeft(5, "0") & "." & TextBox3.Text
            Dim n As String = NumericUpDown2.Value + 1
            Label10.Text = "...\" & TextBox5.Text & "_" & n.PadLeft(5, "0") & "." & TextBox3.Text
            n += 1
            Label2.Text = "...\" & TextBox5.Text & "_" & n.PadLeft(5, "0") & "." & TextBox3.Text

        End If

    End Sub
    ' CONFERMA RINOMINA TUTTI I FILES
    Private Sub OK_Button_Click(sender As Object, e As EventArgs) Handles OK_Button.Click
        If TextBox5.Text = "" Or NumericUpDown2.Text = "" Or TextBox3.Text = "" Then
            MsgBox("Inserire Nome, Numero, Estensione")
            Exit Sub
        Else
            If MsgBox("Tutti i Files nella Cartella Saranno Rinominati" & vbCrLf & vbCrLf & "Continuare?", MsgBoxStyle.YesNo, ) = MsgBoxResult.No Then
                Exit Sub
            End If

            RENA = True
            DisableAll()
            Dim BAR As Integer = 0
            ProgressBar2.Maximum = ListBox1.Items.Count.ToString() - 1
            For N As Integer = 0 To ListBox1.Items.Count() - 1
                If ListBox1.SelectedItem = TextBox5.Text & "_" & NumericUpDown2.Text.PadLeft(5, "0") & "." & TextBox3.Text Then
                    MsgBox("I Nomi Corrispondono, Impossibile Rinominare")
                    Exit Sub
                End If

                My.Computer.FileSystem.RenameFile(TextBox4.Text & "\" & ListBox1.SelectedItem, TextBox5.Text & "_" & NumericUpDown2.Text.PadLeft(5, "0") & "." & TextBox3.Text)
                'Threading.Thread.Sleep(100)
                Application.DoEvents()
                ProgressBar2.PerformStep()
                ProgressBar2.Value = BAR
                BAR += 1
                If ListBox1.SelectedIndex <> ListBox1.Items.Count() - 1 Then
                    NumericUpDown2.Value += 1
                    ListBox1.SelectedIndex += 1
                End If
            Next
            RENA = False
            EnableAll()
            Button26.PerformClick()
            Threading.Thread.Sleep(100)
            Panel1.Visible = False
            ProgressBar2.Value = 0
            ListBox1.Enabled = True
        End If
    End Sub
End Class