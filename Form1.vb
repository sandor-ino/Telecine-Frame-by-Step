
#Region "INIZIALIZAZIONE DATI"
Imports System.Drawing.Drawing2D
Imports System.IO
Imports System.IO.Ports
Imports System.Math
Imports System.Threading
Imports Accord.Video.FFMPEG
Imports AForge
Imports AForge.Imaging
Imports AForge.Imaging.Filters
Imports AForge.Video
Imports AForge.Video.DirectShow


Public Class Form1
    Private currentX As Integer, currentY As Integer
    Private isDragging As Boolean = False
    Dim originalx As Integer = 0
    Dim originaly As Integer = 0
    Dim led As Boolean = False
    Dim dcmotor As Boolean = False
    Dim menuclick As Boolean = False
    Dim setclick As Boolean = False
    Dim setmenuclick As Boolean = False

    'MOD CAM_MOD CLICK_MOD PIC
    Private videoDevices As FilterInfoCollection
    Private videoCapabilities As VideoCapabilities()
    Private videoDevice As VideoCaptureDevice
    Shared ReadOnly _serialPort As SerialPort
    Shared ReadOnly _continue As Boolean


    Dim ANNULLA As Boolean = False
    Dim CONNETTI As Boolean = False
    ReadOnly s8_n8 As String = "0"
    Dim Card As String
    Dim no As String

    Private MouseDownStage, MouseDownX, MouseDownY As Integer
    Dim imgB, imgH, PTBRCB, PTBRCH, PTBRCX, PTBRCY As Integer
    Dim IMGRAP As Long
    Dim IMGRCB, IMGRCH, IMGRCX, IMGRCY As Integer
    Dim CROPMX, CROPMY, CROPMB, CROPMH As Integer
    Dim CROPX, CROPY, CROPB, CROPH As Integer
    Dim esposizione, contrasto As Integer
    Dim BLCX, BLCY As Integer ', BLCH, BLCB
    Dim BLX, BLY, BLB, BLH, BLBR As Integer
    Dim BLXV, BLYV, BLYHV As Integer
    Dim X1, Y1, X2, Y2 As Integer
    Dim ZOOMX, ZOOMY As Decimal
    Dim ptbB, ptbH As Integer
    Dim ptbX, ptbY As Integer
    Dim angolo As Double = 0.0
    'Dim pause As Integer
    'Dim az1 As Integer = 0
    ReadOnly RB As Integer = 30
    Dim RH As Integer = 0
    Dim bl As Integer
    Dim n As Integer = 0
    Dim OCC1 As Integer = 1
    Dim mouseClicked As Boolean = False
    Dim mouseIsDown As Boolean
    Dim FLPY As Boolean = False
    Dim FLPX As Boolean = False
    Dim rotate As Boolean = False
    Dim PLAY1 As Boolean = False
    Dim picmod As Boolean = False
    Dim RENAME1 As Boolean = False
    Dim CONTROL As Boolean = True
    Dim CONTROL1 As Boolean = True
    Dim CONTROL2 As Boolean = True
    Dim STOPIC As Boolean = False
    ReadOnly SUNR As Boolean = True
    Dim AVA As Boolean = False
    Dim az As Boolean = False
    Dim real As Boolean = False
    Dim REES As Boolean = False
    Dim REES2 As Boolean = False
    Dim ZOOMON As Boolean = False
    ReadOnly ZOOMOV As Boolean = False
    Dim LIVECAM As Boolean = True
    Dim ELAB As Boolean = False

    Dim WIEW1 As String
    Dim A1 As String
    Dim bit, IMAGEN, IMAGENZOOM, original, LIVE As Bitmap
    Dim startPoint As New Point()
    Dim endPoint As New Point()
    Dim rectCropArea As Rectangle

    'VIEWR
    ReadOnly img As Image
    ReadOnly img1 As Image
    Dim flipy As Boolean = False
    Dim flipx As Boolean = False
    Dim PLAY_VIEW As Boolean
    Dim A As String
    Dim B As String
    Dim VIEW As String
    Dim vel As Integer

    'MOVIE
    Private ReadOnly reader As VideoFileReader = New VideoFileReader()

    Private ReadOnly writer As VideoFileWriter = New VideoFileWriter()
    Dim rename As Boolean = False
    Dim OCC As Boolean = False
    Dim PLAYMOVIE As Boolean = False
    Dim STP As Boolean = False
    Dim STPP As Boolean = False
    Dim RENA As Boolean = False

#Region "SIMULAZIONE CLICK DEL MOUSE"
    Enum MeFlags As Integer
        MOUSEEVENTF_MOVE = &H1
        MOUSEEVENTF_LEFTDOWN = &H2
        MOUSEEVENTF_LEFTUP = &H4
        MOUSEEVENTF_RIGHTDOWN = &H8
        MOUSEEVENTF_RIGHTUP = &H10
        MOUSEEVENTF_MIDDLEDOWN = &H20
        MOUSEEVENTF_MIDDLEUP = &H40
        MOUSEEVENTF_XDOWN = &H80
        MOUSEEVENTF_XUP = &H100
        MOUSEEVENTF_WHEEL = &H800
        MOUSEEVENTF_VIRTUALDESK = &H4000
        MOUSEEVENTF_ABSOLUTE = &H8000
    End Enum
    Declare Sub mouse_event Lib "user32" (ByVal dwFlags As MeFlags, ByVal Coords As System.Drawing.Point, ByVal dwData As Integer, ByVal dwExtraInfo As UIntPtr)
    Sub SimulateClick(ByVal Location As System.Drawing.Point)
        Dim trect As System.Drawing.Rectangle = Screen.GetBounds(Location)
        Dim tpnt As New System.Drawing.Point(65535.0 / trect.Width * Location.X, 65535.0 / trect.Height * Location.Y)
        mouse_event(MeFlags.MOUSEEVENTF_MOVE Or MeFlags.MOUSEEVENTF_ABSOLUTE, tpnt, 0, New UIntPtr(Convert.ToUInt32(0)))
        mouse_event(MeFlags.MOUSEEVENTF_LEFTDOWN Or MeFlags.MOUSEEVENTF_ABSOLUTE, tpnt, 0, New UIntPtr(Convert.ToUInt32(0)))
        mouse_event(MeFlags.MOUSEEVENTF_LEFTUP Or MeFlags.MOUSEEVENTF_ABSOLUTE, tpnt, 0, New UIntPtr(Convert.ToUInt32(0)))
    End Sub
#End Region


#End Region

#Region "COMANDI FINESTRA"
    'AVVIO PROGRAMMA
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        originalx = Panello_chiudi.Width
        originaly = Panello_comandi.Height + Panello_chiudi.Height

        ' ricorda posizione form
        'Me.Location = New System.Drawing.Point(My.Settings.Form1_Location.X, My.Settings.Form1_Location.Y)
        ' aggiorna elenco porte com
        For Each sp As String In My.Computer.Ports.SerialPortNames
            ComboBox1.Items.Add(sp)
        Next
        ' CARICA IMPOSTAZIONI SALVATE setup
        'WIEW1 = My.Settings.viewer_set
        'Card = My.Settings.cart_ardu
        TextBox15.Text = My.Settings.nome
        NumericUpDown8.Text = My.Settings.inizio
        ComboBox3.Text = My.Settings.file_ext

        If ComboBox1.Items.Contains(My.Settings.set_com) Then
            ComboBox1.Text = My.Settings.set_com
        Else
            'ComboBox1.SelectedIndex = 0
        End If



        'TextBox3.Text = My.Settings.SaveTitle3
        'TextBox2.Text = My.Settings.SaveTitle4
        'TextBox5.Text = My.Settings.SaveTitle5
        'TextBox8.Text = My.Settings.SaveTitle6
        'TextBox9.Text = My.Settings.SaveTitle22
        'ComboBox1.Text = My.Settings.SaveTitle24

        ''TextBox4.Text = My.Settings.PAUSA
        'If Not Directory.Exists(My.Settings.cart_lav) Then
        '    TextBox12.Text = ""
        'Else
        '    TextBox12.Text = My.Settings.cart_lav
        'End If
        'If Not Directory.Exists(My.Settings.DEST_CART) Then
        '    TextBox14.Text = ""
        'Else
        '    TextBox14.Text = My.Settings.DEST_CART
        'End If
        'My.Settings.cam_on = False
        'My.Settings.foto_in = False
        'My.Settings.foto_in2 = False
        ' aggiorna lista files cartella lavoro
        'Label32.Text = 0 & " File "
        'If TextBox12.Text <> Nothing Then
        '    Dim files() As String = Directory.GetFiles(TextBox12.Text)
        '    ListBox1.Items.Clear()
        '    For Each file As String In files
        '        ListBox1.Items.Add(Path.GetFileName(file))
        '    Next
        '    Dim pat As String
        '    pat = TextBox12.Text
        '    Dim filess() As String
        '    If Directory.Exists(pat) Then
        '        filess = Directory.GetFiles(pat)
        '        'Label32.Text = filess.Length & " Files "
        '        If ListBox1.Items.Count.ToString() = 0 Then
        '            Exit Sub
        '        Else
        '            ListBox1.SetSelected(ListBox1.Items.Count.ToString() - 1, True)
        '        End If
        '    End If
        'End If

        ' DISABILITA BOTTONI
        'Button14.Enabled = False
        'Button24.Enabled = False
        'Button25.Enabled = False
        'Button103.Enabled = False
        'Button104.Enabled = False
        'Button9.Visible = False
        ''Button15.Enabled = False
        'Button52.Enabled = False
        'Button78.Enabled = False
        ''Button20.Enabled = False
        'TrackBar2.Enabled = False
        'TrackBar4.Enabled = False
        'NumericUpDown1.Enabled = False
        'NumericUpDown2.Enabled = False
        'NumericUpDown3.Enabled = False
        'Button22.PerformClick() ' connetti arduino
        'DoubleBuffered = True
        '''''EnumerateVideoDevices()
        'If ComboBox4.Text <> "Not supported" Then
        '    CameraStart()
        '    'pause = My.Settings.PAUSA
        '    'Timer2.Interval = My.Settings.PAUSA '+ 500
        '    'Timer2.Start()
        'End If





    End Sub
    'CHIUSURA PROGRAMMA
    Private Sub Form1_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        'My.Settings.Form1_Location = New System.Drawing.Point(Me.Location.X, Me.Location.Y)

        If PLAY_VIEW = True Then
            PLAY_VIEW = False
            Button74.Enabled = True
            Button72.Enabled = True
            Button73.Enabled = False
        End If

        Select Case MessageBox.Show("Sicuro di voler uscire?", "Conferma Chiusura", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
            Case DialogResult.Yes


                'My.Settings.inizio = NumericUpDown8.Text
                My.Settings.cart_lav = TextBox12.Text
                Try
                    If Not SerialPort1.IsOpen Then SerialPort1.Open()
                    Thread.Sleep(250)
                    SerialPort1.Write("8")
                    SerialPort1.Write("0")
                    Thread.Sleep(250)
                    SerialPort1.Write("7")
                    SerialPort1.Write("0")
                    Thread.Sleep(250)
                    SerialPort1.Write("6")
                    SerialPort1.Close()
                Catch ex As Exception
                    Exit Select
                End Try
            Case DialogResult.No
                e.Cancel = True
        End Select
        CameraStop()
        'My.Settings.cam_on = False
        'My.Settings.def_cam = ComboBox5.Text
        'My.Settings.def_res = ComboBox4.Text
        'My.Settings.DEST_CART = TextBox14.Text
        My.Settings.Save()


    End Sub
    'BOTTONE chiudi programma
    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        Me.Close()
    End Sub
    'minimizza
    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click

        Me.WindowState = FormWindowState.Minimized

    End Sub
    'amplia a schermo intero
    Private Sub Button7_Click(sender As Object, e As EventArgs) Handles Button7.Click
        Me.WindowState = FormWindowState.Maximized
        Button7.Visible = False
        Button8.Visible = True
    End Sub
    'riduci in finestra 
    Private Sub Button8_Click(sender As Object, e As EventArgs) Handles Button8.Click

        If Pannello_setup.Visible = True Then
            Button38.PerformClick()
            Me.WindowState = FormWindowState.Normal
            Button8.Visible = False
            Button7.Visible = True
            Button38.PerformClick()
        Else
            Me.WindowState = FormWindowState.Normal
            Button8.Visible = False
            Button7.Visible = True
        End If

    End Sub
    'sposta finestra con il mouse 
    Private Sub Panel2_MouseDown(sender As Object, e As System.Windows.Forms.MouseEventArgs) Handles Panello_chiudi.MouseDown
        isDragging = True
        currentX = e.X
        currentY = e.Y
    End Sub
    Private Sub Panel2_MouseMove(sender As Object, e As System.Windows.Forms.MouseEventArgs) Handles Panello_chiudi.MouseMove
        If isDragging Then
            Me.Top += (e.Y - currentY)
            Me.Left += (e.X - currentX)
        End If
    End Sub
    Private Sub Panel2_MouseUp(sender As Object, e As System.Windows.Forms.MouseEventArgs) Handles Panello_chiudi.MouseUp
        isDragging = False
    End Sub
    'seleziona s8 o n8
    Private Sub RadioButton9_CheckedChanged(sender As Object, e As EventArgs) Handles RadioButton9.CheckedChanged
        If AVA = True Then
            ELABORA()
        End If

    End Sub
#End Region

#Region "MOD VIEWER"
    'apre view mod
    Private Sub RadioButton5_CheckedChanged(sender As Object, e As EventArgs) Handles RadioButton5.CheckedChanged
        Dim p11 As Integer = Panello_sorgente.Height
        If RadioButton5.Checked = True Then
            Panello_viewer.Size = New Size(Panello_chiudi.Width - Panello_comandi.Width, Panello_comandi.Height - Panello_sorgente.Height)
            Panello_viewer.Visible = True
            Panello_sorgente.Size = New Size(Panello_chiudi.Width - Panello_comandi.Width, p11)
            Panello_sorgente.Visible = True
            PictureBox6.Image = Nothing

            Modview()
        Else
            If PLAY_VIEW = True Then
                PLAY_VIEW = False
                Button74.Enabled = True
                Button72.Enabled = True
                Button73.Enabled = False
            End If
            Panello_viewer.Visible = False
            Panello_sorgente.Visible = False
            Panello_sorgente.Size = New Size(Panello_arduino.Width, p11)
            My.Settings.Save()
            My.Settings.cart_lav = TextBox12.Text
            PictureBox6.Image = Nothing
        End If
    End Sub
    'apertura mod viewer
    Private Sub Modview()
        If Not Directory.Exists(My.Settings.cart_lav) Then
            TextBox12.Text = ""
        Else
            TextBox12.Text = My.Settings.cart_lav
        End If

        'PictureBox32.Image = My.Resources.ANTEPRIMA
        Label27.Text = 0 & " File "
        If TextBox12.Text <> Nothing Then
            Dim files() As String = Directory.GetFiles(TextBox12.Text)
            ListBox4.Items.Clear()
            For Each file As String In files
                ListBox4.Items.Add(Path.GetFileName(file))
            Next
            Dim pat As String
            pat = TextBox12.Text
            Dim filess() As String
            If Directory.Exists(pat) Then
                filess = Directory.GetFiles(pat)
                Label27.Text = filess.Length & " Files "
                If ListBox4.Items.Count.ToString() = 0 Then
                    Exit Sub
                Else
                    ListBox4.SelectedIndex = 0
                End If
            End If
        End If
        vel = 0
    End Sub
    'CAMBIA SELEZIONE LISTA FILES
    Private Sub ListBox4_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ListBox4.SelectedIndexChanged
        Try

            PictureBox6.Image = Image.FromFile(TextBox12.Text & "/" & ListBox4.SelectedItem)

            If flipy = True Then
                PictureBox6.Image.RotateFlip(RotateFlipType.RotateNoneFlipY)
            End If
            If flipx = True Then
                PictureBox6.Image.RotateFlip(RotateFlipType.RotateNoneFlipX)
            End If


        Catch ex As Exception
            'PictureBox32.Image = My.Resources.ANTEPRIMA
            'PLAY = False
            'Button74.Enabled = True
            'Button72.Enabled = True
            'Button73.Enabled = False
        End Try
    End Sub
    'specchia verticale
    Private Sub Button68_Click(sender As Object, e As EventArgs) Handles Button68.Click
        If ListBox4.SelectedItem = Nothing Then
            Exit Sub
        End If
        'PictureBox6.Image = Image.FromFile(TextBox12.Text & "/" & ListBox4.SelectedItem)

        If flipx = True Then
            Button68.BackColor = Color.FromArgb(27, 27, 27)

            flipx = False

        Else
            Button68.BackColor = Color.DodgerBlue

            flipx = True
        End If
        Dim sel As Integer = ListBox4.SelectedIndex
        ListBox4.ClearSelected()
        ListBox4.SelectedIndex = sel
    End Sub
    'specchia orizzontale
    Private Sub Button64_Click(sender As Object, e As EventArgs) Handles Button64.Click
        If ListBox4.SelectedItem = Nothing Then
            Exit Sub
        End If
        'PictureBox6.Image = Image.FromFile(TextBox12.Text & "/" & ListBox4.SelectedItem)

        If flipy = True Then
            Button64.BackColor = Color.FromArgb(27, 27, 27)
            flipy = False
        Else
            Button64.BackColor = Color.DodgerBlue
            flipy = True
            'PictureBox6.Image.RotateFlip(RotateFlipType.RotateNoneFlipY)
        End If
        Dim sel As Integer = ListBox4.SelectedIndex
        ListBox4.ClearSelected()
        ListBox4.SelectedIndex = sel
    End Sub
    'zoom +
    Private Sub Button65_Click(sender As Object, e As EventArgs) Handles Button65.Click
        If ListBox4.SelectedItem = Nothing Then
            Exit Sub
        End If
        PictureBox6.Size = New System.Drawing.Size(PictureBox6.Width + 400, PictureBox6.Height + 400)
        PictureBox6.Location = New System.Drawing.Point((PictureBox6.Parent.ClientSize.Width / 2) - (PictureBox6.Width / 2), (PictureBox6.Parent.ClientSize.Height / 2) - (PictureBox6.Height / 2))

    End Sub
    'zoom -
    Private Sub Button61_Click(sender As Object, e As EventArgs) Handles Button61.Click
        If ListBox4.Items.Count = Nothing Then
            Exit Sub
        End If
        PictureBox6.Size = New System.Drawing.Size(PictureBox6.Width - 400, PictureBox6.Height - 400)
        PictureBox6.Location = New System.Drawing.Point((PictureBox6.Parent.ClientSize.Width / 2) - (PictureBox6.Width / 2), (PictureBox6.Parent.ClientSize.Height / 2) - (PictureBox6.Height / 2))

    End Sub
    'avanti
    Private Sub Button66_Click(sender As Object, e As EventArgs) Handles Button66.Click
        If ListBox4.Items.Count = Nothing Then
            Exit Sub
        End If
        A = ListBox4.FindString(ListBox4.SelectedItem.ToString())
        If A >= 1 Then
            A -= 1
            ListBox4.SetSelected(A, True)
        End If
    End Sub
    'indietro
    Private Sub Button62_Click(sender As Object, e As EventArgs) Handles Button62.Click
        If ListBox4.Items.Count = Nothing Then
            Exit Sub
        End If
        A = ListBox4.FindString(ListBox4.SelectedItem.ToString())
        If A <= ListBox4.Items.Count.ToString() - 2 Then
            A += 1
            ListBox4.SetSelected(A, True)
        End If
    End Sub
    'play slide avanti
    Private Sub Button74_Click(sender As Object, e As EventArgs) Handles Button74.Click
        If ListBox4.Items.Count = Nothing Then
            Exit Sub
        End If
        PLAY_VIEW = True
        Button72.Enabled = False
        Button74.Enabled = False
        Button74.BackColor = Color.DodgerBlue
        B = ListBox4.FindString(ListBox4.SelectedItem.ToString())
        While B <= ListBox4.Items.Count.ToString() - 2
            Application.DoEvents()
            If TrackBar5.Value = 0 Then
                vel = 10
            End If
            If TrackBar5.Value = 1 Then
                vel = 50
            End If
            If TrackBar5.Value = 2 Then
                vel = 100
            End If
            If TrackBar5.Value = 3 Then
                vel = 150
            End If
            If TrackBar5.Value = 4 Then
                vel = 200
            End If
            If TrackBar5.Value = 5 Then
                vel = 250
            End If
            If PLAY_VIEW = True Then
                Button73.Enabled = True
                B += 1
                Thread.Sleep(vel)
                ListBox4.SetSelected(B, True)
            Else
                Exit While
            End If
        End While
        PLAY_VIEW = False
        Button74.BackColor = Color.FromArgb(27, 27, 27)
        Button74.Enabled = True
        Button72.Enabled = True
        Button73.Enabled = False

    End Sub
    'play slide indoetro
    Private Sub Button72_Click(sender As Object, e As EventArgs) Handles Button72.Click
        If ListBox4.Items.Count = Nothing Then
            Exit Sub
        End If
        PLAY_VIEW = True
        Button74.Enabled = False
        Button72.Enabled = False
        Button72.BackColor = Color.DodgerBlue
        B = ListBox4.FindString(ListBox4.SelectedItem.ToString())
        While B >= 1
            Application.DoEvents()
            If TrackBar5.Value = 0 Then
                vel = 10
            End If
            If TrackBar5.Value = 1 Then
                vel = 50
            End If
            If TrackBar5.Value = 2 Then
                vel = 100
            End If
            If TrackBar5.Value = 3 Then
                vel = 150
            End If
            If TrackBar5.Value = 4 Then
                vel = 200
            End If
            If TrackBar5.Value = 5 Then
                vel = 250
            End If
            If PLAY_VIEW = True Then
                Button73.Enabled = True
                B -= 1
                Thread.Sleep(vel)
                ListBox4.SetSelected(B, True)
            Else
                Exit While
            End If
        End While
        PLAY_VIEW = False
        Button72.BackColor = Color.FromArgb(27, 27, 27)
        Button72.Enabled = True
        Button74.Enabled = True
        Button73.Enabled = False
    End Sub
    'stop slide
    Private Sub Button73_Click(sender As Object, e As EventArgs) Handles Button73.Click
        PLAY_VIEW = False
    End Sub
    'centra e adatta immagine
    Private Sub Button67_Click(sender As Object, e As EventArgs) Handles Button67.Click
        If ListBox4.Items.Count = Nothing Then
            Exit Sub
        End If
        PictureBox6.Size = New System.Drawing.Size(PictureBox6.Parent.ClientSize.Width - 10, PictureBox6.Parent.ClientSize.Height - 10)
        PictureBox6.Location = New System.Drawing.Point((PictureBox6.Parent.ClientSize.Width / 2) - (PictureBox6.Width / 2), (PictureBox6.Parent.ClientSize.Height / 2) - (PictureBox6.Height / 2))

    End Sub
    'aggiorna contenuto cartella
    Private Sub Button71_Click(sender As Object, e As EventArgs) Handles Button71.Click
        TextBox12.Text = My.Settings.cart_lav
        'PictureBox32.Image = My.Resources.ANTEPRIMA
        Label27.Text = 0 & " File "
        If TextBox12.Text <> "" Then
            Dim files() As String = Directory.GetFiles(TextBox12.Text)
            ListBox4.Items.Clear()
            For Each file As String In files
                ListBox4.Items.Add(Path.GetFileName(file))
            Next
            Dim pat As String
            pat = TextBox12.Text
            Dim filess() As String
            If Directory.Exists(pat) Then
                filess = Directory.GetFiles(pat)
                Label27.Text = filess.Length & " Files "
                If ListBox4.Items.Count.ToString() = 0 Then
                    Exit Sub
                Else
                    ListBox4.SelectedIndex = 0
                    ' ListBox4.SetSelected(ListBox4.Items.Count.ToString() - 1, True)
                End If
            End If
        End If

    End Sub
    'cancella immagine selezionata
    Private Sub Button70_Click(sender As Object, e As EventArgs) Handles Button70.Click
        If ListBox4.Items.Count = Nothing Then
            Exit Sub
        End If
        If MsgBox("Eliminare il File?" & vbCrLf & vbCrLf & "...\" & ListBox4.SelectedItem, MsgBoxStyle.YesNo Or MsgBoxStyle.Exclamation) = MsgBoxResult.Yes Then
            Try
                Dim DA As String = (TextBox12.Text & "\" & ListBox4.SelectedItem)
                My.Computer.FileSystem.DeleteFile(DA)
                PictureBox6.Image = Nothing
                Dim li As Integer
                For li = ListBox4.SelectedIndices.Count - 1 To 0 Step -1
                    ListBox4.Items.RemoveAt(ListBox4.SelectedIndices(li))
                Next
                DA = 0
            Catch ex As Exception
                MsgBox("Impossibile Eliminare il File?" & vbCrLf & vbCrLf & "...\" & ListBox4.SelectedItem,)
            End Try


            If TextBox12.Text <> Nothing Then
                Dim files() As String = Directory.GetFiles(TextBox12.Text)
                Dim pat As String
                pat = TextBox12.Text
                Dim filess() As String
                If Directory.Exists(pat) Then
                    filess = Directory.GetFiles(pat)
                    Label27.Text = filess.Length & " Files "
                    'ListBox1.SetSelected(ListBox1.Items.Count - 1, True)
                End If
            End If
            If ListBox4.Items.Count = Nothing Then
                Exit Sub
            Else
                ListBox4.SetSelected(ListBox4.Items.Count - 1, True)
            End If
        End If

    End Sub
    'apri seleziona programma esterno
    Private Sub Button63_MouseDown(ByVal sender As Object, ByVal e As MouseEventArgs) Handles Button63.MouseDown
        If ListBox4.Items.Count = Nothing Then
            Exit Sub
        End If
        If e.Button = Windows.Forms.MouseButtons.Right Then
            If MsgBox("Cambiare Viewer Esterno?", MsgBoxStyle.OkCancel Or MsgBoxStyle.Question, ) = MsgBoxResult.Ok Then
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
                Process.Start(VIEW, TextBox12.Text & "\" & ListBox4.SelectedItem)
            End If
        End If

    End Sub
    'apri cartella in explorer
    Private Sub Button69_Click(sender As Object, e As EventArgs) Handles Button69.Click
        If TextBox12.Text <> Nothing Then
            Process.Start("explorer.exe", TextBox12.Text)
        End If
    End Sub
    'INFORMAZIONI FILE
    Private Sub ListBox4_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles ListBox4.MouseDown
        If ListBox4.Items.Count = Nothing Then
            Exit Sub
        End If
        If e.Button = MouseButtons.Right Then
            ListBox4.SelectedIndex = ListBox4.IndexFromPoint(e.X, e.Y)
            Dim MyimageWxH
            Try
                Dim Myimg As System.Drawing.Image = System.Drawing.Image.FromFile(TextBox12.Text & "\" & ListBox4.SelectedItem)
                Dim MyImageWidth = PictureBox6.Image.Width
                Dim MyImageHeight = PictureBox6.Image.Height
                MyimageWxH = MyImageWidth & " X " & MyImageHeight
            Catch
                MyimageWxH = "Not Supported"
            End Try
            Dim Myimagepropety = FileLen(TextBox12.Text & "\" & ListBox4.SelectedItem)
            Dim Myimagedate As DateTime = File.GetCreationTime(TextBox12.Text & "\" & ListBox4.SelectedItem)
            MsgBox("Informazioni:" & vbCrLf & vbCrLf & TextBox12.Text & "\" & ListBox4.SelectedItem & vbCrLf & vbCrLf & " Data Creazione:  " & Myimagedate & vbCrLf & vbCrLf & " Formato:  " & MyimageWxH & vbCrLf & vbCrLf & " Dimensione:  " & System.Math.Round(Myimagepropety / 1000, 1) & " KB", MessageBoxIcon.Information)
        End If
    End Sub
    'PAN IMMAGINE 1/3
    Private Sub PictureBox6_MouseDown(sender As Object, e As System.Windows.Forms.MouseEventArgs) Handles PictureBox6.MouseDown
        If ListBox4.Items.Count = Nothing Then
            Exit Sub
        End If
        isDragging = True
        currentX = e.X
        currentY = e.Y
    End Sub
    'PAN IMMAGINE 2/3
    Private Sub PictureBox6_MouseMove(sender As Object, e As System.Windows.Forms.MouseEventArgs) Handles PictureBox6.MouseMove
        If ListBox4.Items.Count = Nothing Then
            Exit Sub
        End If
        If isDragging Then
            PictureBox6.Top = PictureBox6.Top + (e.Y - currentY)
            PictureBox6.Left = PictureBox6.Left + (e.X - currentX)
        End If
    End Sub
    'PAN IMMAGINE 3/3
    Private Sub PictureBox6_MouseUp(sender As Object, e As System.Windows.Forms.MouseEventArgs) Handles PictureBox6.MouseUp
        If ListBox4.Items.Count = Nothing Then
            Exit Sub
        End If
        isDragging = False
    End Sub
#End Region

#Region "MOD MOVIE"
    'apre movie mod
    Private Sub RadioButton1_CheckedChanged(sender As Object, e As EventArgs) Handles RadioButton1.CheckedChanged
        Dim p11 As Integer = Panello_sorgente.Height
        If RadioButton1.Checked = True Then
            Panello_movie.Size = New Size(Panello_chiudi.Width - Panello_comandi.Width, Panello_comandi.Height - Panello_sorgente.Height)
            Panello_movie.Visible = True
            Panello_sorgente.Size = New Size(Panello_chiudi.Width - Panello_comandi.Width, p11)
            Panello_sorgente.Visible = True
            PictureBox30.Image = Nothing
            TextBox7.Text = My.Settings.cart_dest_vid
            Modmovie()

        Else
            Panello_movie.Visible = False
            Panello_sorgente.Visible = False
            Panello_sorgente.Size = New Size(Panello_arduino.Width, p11)
            PictureBox30.Image = Nothing
        End If
    End Sub
    'apertura mod movie
    Private Sub Modmovie()
        If Not Directory.Exists(My.Settings.cart_lav) Then
            TextBox12.Text = ""
        Else
            TextBox12.Text = My.Settings.cart_lav
        End If
        If Not Directory.Exists(My.Settings.cart_dest_vid) Then
            TextBox7.Text = ""
        Else
            TextBox7.Text = My.Settings.cart_dest_vid
        End If

        Button51.PerformClick()
        Button54.PerformClick()
        NumericUpDown4.Value = My.Settings.FTP_1
        ComboBox4.Text = My.Settings.QUA_1
        NumericUpDown5.Value = My.Settings.BTR_1
        ComboBox5.Text = My.Settings.RES_1
    End Sub

    ' AVVIO CREAZIONE VIDEO
    Private Sub Button56_Click(sender As Object, e As EventArgs) Handles Button56.Click

        If TextBox12.Text <> Nothing Then
            If TextBox7.Text <> Nothing Then
                If MsgBox("Creare il file Video?" & vbCrLf & vbCrLf & "...\" & TextBox6.Text & "." & ComboBox6.Text, MsgBoxStyle.OkCancel Or MsgBoxStyle.Question, ) = MsgBoxResult.Ok Then
                    PictureBox30.SizeMode = PictureBoxSizeMode.CenterImage
                    Dim newfile As String = TextBox7.Text & "\" & TextBox6.Text & "." & ComboBox6.Text
                    If My.Computer.FileSystem.FileExists(newfile) Then
                        My.Computer.Audio.PlaySystemSound(System.Media.SystemSounds.Beep)
                        If MsgBox("ATTENZIONE!" & vbCrLf & vbCrLf & "Il File  ...\" & TextBox6.Text & "." & ComboBox6.Text & vbCrLf & vbCrLf & "E' Esistente, Sovrascrivere?", MsgBoxStyle.OkCancel Or MsgBoxStyle.Exclamation, ) = MsgBoxResult.Cancel Then
                            PictureBox30.SizeMode = PictureBoxSizeMode.Zoom
                            Exit Sub
                        End If
                    End If

                    If ListBox2.SelectedIndex <> -1 Then
                        ListBox2.SelectedIndex = 0
                        PictureBox30.Image = My.Resources.wait_gif
                        DisableMovie()
                        Dim MIS As System.Drawing.Image = System.Drawing.Image.FromFile(TextBox12.Text & "/" & ListBox2.SelectedItem)
                        Dim BAS As Integer = MIS.Width
                        Dim ALT As Integer = MIS.Height
                        Select Case ComboBox5.Text
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
                        Dim FRM As Integer = NumericUpDown4.Value
                        Dim BTR As Integer = NumericUpDown5.Value * 1000
                        Select Case ComboBox4.Text
                            Case "Default"
                                writer.Open(TextBox7.Text & "\" & TextBox6.Text & "." & ComboBox6.Text, BAS, ALT, FRM, VideoCodec.Default, BTR)
                            Case "MPEG2"
                                writer.Open(TextBox7.Text & "\" & TextBox6.Text & "." & ComboBox6.Text, BAS, ALT, FRM, VideoCodec.MPEG2, BTR)
                            Case "H264"
                                writer.Open(TextBox7.Text & "\" & TextBox6.Text & "." & ComboBox6.Text, BAS, ALT, FRM, VideoCodec.H264, BTR)
                            Case "FLV1"
                                writer.Open(TextBox7.Text & "\" & TextBox6.Text & "." & ComboBox6.Text, BAS, ALT, FRM, VideoCodec.FLV1, BTR)
                        End Select
                        Dim image As Bitmap
                        Dim A As Integer = 0
                        ProgressBar1.Maximum = ListBox2.Items.Count.ToString() - 1
                        For i As Integer = 0 To ListBox2.Items.Count.ToString() - 1
                            Application.DoEvents()
                            ProgressBar1.PerformStep()
                            ProgressBar1.Value = A
                            A += 1
                            If STP = True Then
                                writer.Close()
                                PictureBox30.Image = Nothing
                                My.Computer.Audio.PlaySystemSound(Media.SystemSounds.Beep)
                                If MsgBox("CREAZIONE VIDEO ANNULLATA!" & vbCrLf & vbCrLf & "Cancellare Il File  ...\" & TextBox6.Text & "." & ComboBox6.Text, MsgBoxStyle.OkCancel Or MsgBoxStyle.Critical, ) = MsgBoxResult.Ok Then
                                    My.Computer.FileSystem.DeleteFile(newfile)
                                    Button54.PerformClick()
                                End If
                                PictureBox30.Image = Nothing
                                PictureBox30.SizeMode = PictureBoxSizeMode.Zoom
                                EnableMovie()
                                STP = False
                                ProgressBar1.Value = 0
                                Button54.PerformClick()

                                Exit Sub

                            Else
                                image = System.Drawing.Image.FromFile(TextBox12.Text & "/" & ListBox2.SelectedItem)
                                If ComboBox4.Text = "Default" And ComboBox5.Text = "Original" Then
                                    writer.WriteVideoFrame(image)
                                    image.Dispose()
                                Else
                                    Dim newImage As System.Drawing.Image = New System.Drawing.Bitmap(BAS, ALT)
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
                                If ListBox2.Items.Count.ToString() - 1 <> A Then
                                    ListBox2.SelectedIndex += 1
                                End If
                            End If
                        Next
                        writer.Close()
                        ListBox2.SelectedIndex = 0
                        PictureBox30.Image = Nothing
                        MsgBox("OPERAZIONE ESEGUITA, CON SUCCESSO." & vbCrLf & vbCrLf & "File Video Creato: " & vbCrLf & vbCrLf & newfile, MessageBoxIcon.Information)
                        EnableMovie()
                        ProgressBar1.Value = 0
                        ListBox2.SelectedIndex = 0
                        Button54.PerformClick()
                    End If
                    PictureBox30.SizeMode = PictureBoxSizeMode.Zoom
                End If
            Else
                MsgBox("CARTELLA DESTINAZIONE VUOTA!")
            End If
        Else
            MsgBox("CARTELLA DI ORIGINE VUOTA!")
        End If

    End Sub
    ' FERMA CRAZIONE VIDEO
    Private Sub Button57_Click(sender As Object, e As EventArgs) Handles Button57.Click
        STP = True
    End Sub

    ' APRE CARTELLA SORGENTE IN ESPLORA RISORSE
    Private Sub Button49_Click(sender As Object, e As EventArgs) Handles Button49.Click
        If TextBox12.Text <> Nothing Then
            Process.Start("explorer.exe", TextBox12.Text)
        End If
    End Sub
    'APRI/SELEZIONA VIEWER ESTERNO DA LISTA CARTELLA SORGENTE
    Private Sub Button48_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles Button48.MouseDown
        If ListBox2.Items.Count = Nothing Then
            Exit Sub
        End If
        Dim VIEW As String = My.Settings.viewer_set
        If e.Button = Windows.Forms.MouseButtons.Right Then
            If MsgBox("Cambiare Viewer Esterno?", MsgBoxStyle.OkCancel Or MsgBoxStyle.Question, ) = MsgBoxResult.Ok Then
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
                Process.Start(VIEW, TextBox12.Text & "\" & ListBox2.SelectedItem)
            End If
        End If
    End Sub
    ' CAMBIO SELEZIONE LISTA CATRELLA SORGENTE  
    Private Sub ListBox2_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ListBox2.SelectedIndexChanged
        If ListBox2.SelectedIndex > -1 Then
            If RENA = True Then
                If PictureBox30.BackgroundImage IsNot Nothing Then
                    PictureBox30.BackgroundImage.Dispose()
                    Image.FromFile(TextBox12.Text & "/" & ListBox2.SelectedItem).Dispose()
                End If
                Exit Sub
            Else
                ListBox3.SelectedItems.Clear()
                Try
                    PictureBox30.BackgroundImage = Image.FromFile(TextBox12.Text & "/" & ListBox2.SelectedItem)
                Catch ex As Exception
                End Try

            End If
            If rename = False Then
                Dim ratio As String = ListBox2.SelectedItem
                Dim split = ratio.Split("_", 2, StringSplitOptions.RemoveEmptyEntries)
                TextBox6.Text = split(0)

            End If
        End If
    End Sub
    ' AGGIORNA CONTENUTO LISTA CARTELLA SORGENTE
    Private Sub Button51_Click(sender As Object, e As EventArgs) Handles Button51.Click
        Label13.Text = 0 & " File "
        If TextBox12.Text <> "" Then
            Dim files() As String = Directory.GetFiles(TextBox12.Text)
            ListBox2.Items.Clear()
            For Each file As String In files
                ListBox2.Items.Add(Path.GetFileName(file))
            Next
            Dim pat As String
            pat = TextBox12.Text
            Dim filess() As String
            If Directory.Exists(pat) Then
                filess = Directory.GetFiles(pat)
                Label13.Text = filess.Length & " Files "
                If ListBox2.Items.Count.ToString() = 0 Then
                    Exit Sub
                Else
                    ListBox2.SelectedIndex = 0
                End If
            End If
        End If
    End Sub
    ' INFORMAZIONI FILE IMMAGINI CARTELLA SORGENTE
    Private Sub ListBox2_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles ListBox2.MouseDown
        If ListBox2.Items.Count = Nothing Then
            Exit Sub
        End If
        If e.Button = MouseButtons.Right Then
            If ListBox2.SelectedIndex > -1 Then
                ListBox2.SelectedIndex = ListBox2.IndexFromPoint(e.X, e.Y)
                Dim MyimageWxH As String
                Dim Myimg As System.Drawing.Image = System.Drawing.Image.FromFile(TextBox12.Text & "\" & ListBox2.SelectedItem)
                Dim MyImageWidth = Myimg.Width
                Dim MyImageHeight = Myimg.Height
                MyimageWxH = MyImageWidth & " X " & MyImageHeight
                Dim Myimagepropety = FileLen(TextBox12.Text & "\" & ListBox2.SelectedItem)
                Dim Myimagedate As DateTime = File.GetCreationTime(TextBox12.Text & "\" & ListBox2.SelectedItem)
                MsgBox("Informazioni:" & vbCrLf & vbCrLf & TextBox12.Text & "\" & ListBox2.SelectedItem & vbCrLf & vbCrLf & " Data: " & Myimagedate & vbCrLf & vbCrLf & " Formato: " & MyimageWxH & vbCrLf & vbCrLf & " Dimensione: " & System.Math.Round(Myimagepropety / 1000, 1) & " KB", MessageBoxIcon.Information)
            End If
        End If

    End Sub

    ' SELEZIONA CARTELLA DI DESTINAZIONE 
    Private Sub Button59_Click(sender As Object, e As EventArgs) Handles Button59.Click
        Using folderDlg As New FolderBrowserDialog
            folderDlg.SelectedPath = TextBox7.Text
            folderDlg.ShowNewFolderButton = True
            'PictureBox30.Image = My.Resources.ANTEPRIMA
            Label12.Text = 0 & " File "
            If (folderDlg.ShowDialog() = DialogResult.OK) Then
                TextBox7.Text = folderDlg.SelectedPath
                If TextBox12.Text = TextBox7.Text Then
                    MessageBox.Show("la cartella di Origine non puo essere" & vbCrLf & vbCrLf & "la cartella di Destinazione")
                    TextBox7.Text = ""
                    ListBox3.Items.Clear()
                    Button59.PerformClick()
                    Exit Sub
                End If
                If TextBox7.Text <> "" Then
                    My.Settings.cart_dest_vid = TextBox7.Text
                    Dim files() As String = Directory.GetFiles(TextBox7.Text)
                    ListBox3.Items.Clear()
                    For Each file As String In files
                        ListBox3.Items.Add(Path.GetFileName(file))
                    Next
                    Dim pat As String
                    pat = TextBox7.Text
                    Dim filess() As String
                    If Directory.Exists(pat) Then
                        filess = Directory.GetFiles(pat)
                        Label12.Text = filess.Length & " Files "
                    End If
                End If
            End If
        End Using

    End Sub
    ' APRE CARTELLA DESTINAZIONE IN ESPLORA RISORSE
    Private Sub Button53_Click(sender As Object, e As EventArgs) Handles Button53.Click
        If TextBox7.Text <> Nothing Then
            Process.Start("explorer.exe", TextBox7.Text)
        End If
    End Sub
    ' APRI FILE IN LETTORE VIDEO ESTERNO
    Private Sub Button50_Click(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles Button50.Click
        If ListBox3.Items.Count = Nothing Then
            Exit Sub
        End If
        If ListBox3.SelectedIndex > -1 Then
            Process.Start(TextBox7.Text & "\" & ListBox3.SelectedItem)
        End If
    End Sub
    ' CANCELLA FILE SELEZIONATO IN CARTELLA DESTINAZIONE
    Private Sub Button55_Click(sender As Object, e As EventArgs) Handles Button55.Click
        If ListBox3.Items.Count = Nothing Then
            Exit Sub
        End If
        If MsgBox("Eliminare il File?" & vbCrLf & vbCrLf & "...\" & ListBox3.SelectedItem, MsgBoxStyle.YesNo, ) = MsgBoxResult.Yes Then
            Dim DA As String = (TextBox7.Text & "\" & ListBox3.SelectedItem)
            PictureBox30.Image = Nothing
            Dim li As Integer
            For li = ListBox3.SelectedIndices.Count - 1 To 0 Step -1
                ListBox3.Items.RemoveAt(ListBox3.SelectedIndices(li))
            Next
            My.Computer.FileSystem.DeleteFile(DA)
            DA = 0
            If TextBox7.Text <> Nothing Then
                Dim files() As String = Directory.GetFiles(TextBox7.Text)
                Dim pat As String
                pat = TextBox7.Text
                Dim filess() As String
                If Directory.Exists(pat) Then
                    filess = Directory.GetFiles(pat)
                    Label12.Text = filess.Length & " Files "
                End If
            End If
        End If
    End Sub
    ' AGGIORNA CONTENUTO LISTA CARTELLA DESTINAZIONE
    Private Sub Button54_Click(sender As Object, e As EventArgs) Handles Button54.Click

        Label12.Text = 0 & " File "
        If TextBox7.Text <> "" Then
            Dim files() As String = Directory.GetFiles(TextBox7.Text)
            ListBox3.Items.Clear()
            For Each file As String In files
                ListBox3.Items.Add(Path.GetFileName(file))
            Next

            Dim pat As String = TextBox7.Text
            Dim filess() As String
            If Directory.Exists(pat) Then
                filess = Directory.GetFiles(TextBox7.Text)
                Label12.Text = filess.Length & " Files "
            End If
        End If
    End Sub
    ' INFORMAZIONI FILE VIDEO CARTELLA DESTINAZIONE
    Private Sub ListBox3_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles ListBox3.MouseDown
        If ListBox3.Items.Count = Nothing Then
            Exit Sub
        End If
        If e.Button = MouseButtons.Right Then
            ListBox3.SelectedIndex = ListBox3.IndexFromPoint(e.X, e.Y)
            Dim MyimageWxH As String = ""
            Dim MyImageWidth As Integer, MyImageHeight As Integer
            Dim FRMR As Integer, FRAM As Integer, BITR As Integer
            Dim CODEC As String = ""
            Dim DIME As Long
            Dim Myimagedate As DateTime
            Dim s As Integer, m As Integer, h As Integer
            Dim DUR As Long, DURA As String = ""
            If ListBox3.SelectedIndex > -1 Then
                DIME = FileLen(TextBox7.Text & "\" & ListBox3.SelectedItem)
                Myimagedate = File.GetCreationTime(TextBox7.Text & "\" & ListBox3.SelectedItem)
                reader.Open(TextBox7.Text & "\" & ListBox3.SelectedItem)
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
            MsgBox("Informazioni:" & vbCrLf & vbCrLf & TextBox7.Text & "\" & ListBox3.SelectedItem & vbCrLf & vbCrLf & "Data: " & Myimagedate & vbCrLf & vbCrLf & "Formato: " & MyimageWxH & vbCrLf & "Framerate: " & FRMR & vbCrLf & "Bitrate: " & BITR & " Kbps" & vbCrLf & "Codec: " & CODEC & vbCrLf & "N. Frame: " & FRAM & vbCrLf & "Dimensione: " & System.Math.Round(DIME / 1000000, 1) & " MB" & vbCrLf & "Durata: " & DURA, MessageBoxIcon.Information)
        End If

    End Sub

    ' SALVA IMPOSTAZIONI 1 2 3
    Private Sub Button45_Click(sender As Object, e As EventArgs) Handles Button45.Click
        If RadioButton4.Checked = True Then
            My.Settings.FTP_1 = NumericUpDown4.Value
            My.Settings.QUA_1 = ComboBox4.Text
            My.Settings.BTR_1 = NumericUpDown5.Value
            My.Settings.RES_1 = ComboBox5.Text
        End If
        If RadioButton7.Checked = True Then
            My.Settings.FTP_2 = NumericUpDown4.Value
            My.Settings.QUA_2 = ComboBox4.Text
            My.Settings.BTR_2 = NumericUpDown5.Value
            My.Settings.RES_2 = ComboBox5.Text
        End If
        If RadioButton10.Checked = True Then
            My.Settings.FTP_3 = NumericUpDown4.Value
            My.Settings.QUA_3 = ComboBox4.Text
            My.Settings.BTR_3 = NumericUpDown5.Value
            My.Settings.RES_3 = ComboBox5.Text
        End If
    End Sub
    ' SELEZIONA IMPOSTAZIONE 1
    Private Sub RadioButton4_CheckedChanged(sender As Object, e As EventArgs) Handles RadioButton4.CheckedChanged
        If RadioButton4.Checked = True Then
            RadioButton4.BackgroundImage = My.Resources._1red
            NumericUpDown4.Value = My.Settings.FTP_1
            ComboBox4.Text = My.Settings.QUA_1
            NumericUpDown5.Value = My.Settings.BTR_1
            ComboBox5.Text = My.Settings.RES_1
        Else
            RadioButton4.BackgroundImage = My.Resources._1
        End If

    End Sub
    ' SELEZIONA IMPOSTAZIONE 2
    Private Sub RadioButton7_CheckedChanged(sender As Object, e As EventArgs) Handles RadioButton7.CheckedChanged
        If RadioButton7.Checked = True Then
            RadioButton7.BackgroundImage = My.Resources._2red
            NumericUpDown4.Value = My.Settings.FTP_2
            ComboBox4.Text = My.Settings.QUA_2
            NumericUpDown5.Value = My.Settings.BTR_2
            ComboBox5.Text = My.Settings.RES_2
        Else
            RadioButton7.BackgroundImage = My.Resources._2
        End If
    End Sub
    ' SELEZIONA IMPOSTAZIONE 3
    Private Sub RadioButton10_CheckedChanged(sender As Object, e As EventArgs) Handles RadioButton10.CheckedChanged
        If RadioButton10.Checked = True Then
            RadioButton10.BackgroundImage = My.Resources._3red
            NumericUpDown4.Value = My.Settings.FTP_3
            ComboBox4.Text = My.Settings.QUA_3
            NumericUpDown5.Value = My.Settings.BTR_3
            ComboBox5.Text = My.Settings.RES_3
        Else
            RadioButton10.BackgroundImage = My.Resources._3
        End If
    End Sub

    ' CAMBIO SELEZIONE CARTELLA VIDEO
    Private Sub ListBox3_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ListBox3.SelectedIndexChanged
        If ListBox3.SelectedIndex > -1 Then
            ListBox2.SelectedItems.Clear()
            Try
                Dim FRMR As Integer, FRAM As Integer
                Dim DIME As Long
                Dim s As Integer, m As Integer, h As Integer
                Dim DUR As Long, DURA As String = ""
                Dim readerv As VideoFileReader = New VideoFileReader()
                TableLayoutPanel77.Visible = True
                If PictureBox30.Image IsNot Nothing Then
                    PictureBox30.Image.Dispose()
                End If


                DIME = FileLen(TextBox7.Text & "\" & ListBox3.SelectedItem)
                readerv.Open(TextBox7.Text & "\" & ListBox3.SelectedItem)
                FRMR = readerv.FrameRate
                FRAM = readerv.FrameCount
                DUR = DIME / ((DIME / FRAM) * FRMR)
                s = DUR Mod 60
                DUR = Int(DUR / 60)
                m = DUR Mod 60
                DUR = Int(DUR / 60)
                h = DUR
                DURA = Format(h, "00") & ":" & Format(m, "00") & ":" & Format(s, "00")
                Label26.Text = DURA
                Label25.Text = "00:00:00"
                PictureBox30.SizeMode = PictureBoxSizeMode.Zoom
                PictureBox30.Image = readerv.ReadVideoFrame(0)
                readerv.Close()
                TrackBar6.Value = 0
                'PictureBox30.Image = videoFrame
                'videoFrame.Dispose()
            Catch ex As Exception
                MsgBox("Errore Video!", MsgBoxStyle.Critical,)
                TableLayoutPanel77.Visible = False
            End Try
        Else
            TableLayoutPanel77.Visible = False

            If PictureBox30.Image IsNot Nothing Then
                PictureBox30.Image = Nothing
                'PictureBox30.Image.Dispose()
            End If

        End If
    End Sub
    ' CAMBIO tempo barra video
    Private Sub TrackBar6_ValueChanged(sender As Object, e As EventArgs) Handles TrackBar6.ValueChanged
        Dim FRMR As Integer, FRAM As Integer
        Dim DIME As Long
        Dim s As Integer, m As Integer, h As Integer
        Dim DUR As Long, DURA As String = ""
        Dim readerv As VideoFileReader = New VideoFileReader()
        If PictureBox30.Image IsNot Nothing Then
            PictureBox30.Image.Dispose()
        End If
        If readerv.IsOpen = False Then
            DIME = FileLen(TextBox7.Text & "\" & ListBox3.SelectedItem)
            readerv.Open(TextBox7.Text & "\" & ListBox3.SelectedItem)
            FRMR = readerv.FrameRate
            FRAM = readerv.FrameCount
        End If

        TrackBar6.Maximum = FRAM
        DUR = DIME / ((DIME / TrackBar6.Value) * FRMR)
        s = DUR Mod 60
        DUR = Int(DUR / 60)
        m = DUR Mod 60
        DUR = Int(DUR / 60)
        h = DUR
        DURA = Format(h, "00") & ":" & Format(m, "00") & ":" & Format(s, "00")
        Label25.Text = DURA
        Try
            PictureBox30.Image = readerv.ReadVideoFrame(TrackBar6.Value)
            readerv.Close()
        Catch ex As Exception
            MsgBox("Errore Video2!", MsgBoxStyle.Critical,)
        End Try

    End Sub

    Private Sub TrackBar6_MouseDown(sender As Object, e As MouseEventArgs) Handles TrackBar6.MouseDown
        Dim dblValue As Double
        dblValue = (CDbl(e.X) / CDbl(TrackBar6.Width)) * (TrackBar6.Maximum - TrackBar6.Minimum)
        TrackBar6.Value = Convert.ToInt32(dblValue)
    End Sub

    Private Sub TrackBar6_MouseLeave(sender As Object, e As EventArgs) Handles TrackBar6.MouseLeave

    End Sub


    ' PLAY ANTEPRIMA VIDEO IN PITUREBOX
    Private Sub Button47_Click(sender As Object, e As EventArgs) Handles Button47.Click
        If ListBox3.Items.Count = Nothing Then
            Exit Sub
        End If
        PLAYMOVIE = True
        DisableMovie()
        If ListBox3.SelectedIndex > -1 Then
            reader.Open(TextBox7.Text & "\" & ListBox3.SelectedItem)
            Dim P As Integer = 1000 / reader.FrameRate
            For i As Integer = TrackBar6.Value To reader.FrameCount() - 1
                Application.DoEvents()
                Threading.Thread.Sleep(CInt(P * 0.5))
                If STPP = True Then
                    If reader.IsOpen = True Then
                        PictureBox30.Image = reader.ReadVideoFrame(TrackBar6.Value)
                        reader.Close()
                    End If
                    STPP = False
                    Exit For
                End If
                If PictureBox30.Image IsNot Nothing Then
                    PictureBox30.Image.Dispose()
                End If
                Try
                    PictureBox30.Image = DirectCast(reader.ReadVideoFrame, Bitmap)
                    TrackBar6.Value += 1

                Catch ex As Exception
                    If MsgBox("Errore Video!", MsgBoxStyle.Critical,) Then
                        Exit For
                    End If

                End Try
            Next
            If reader.IsOpen = True Then
                PictureBox30.Image = reader.ReadVideoFrame(0)
                reader.Close()
            End If
        End If
        EnableMovie()
        TrackBar6.Value = 0
        PLAYMOVIE = False
    End Sub
    ' STOP ANTEPRIMA VIDEO
    Private Sub Button46_Click(sender As Object, e As EventArgs) Handles Button46.Click
        STPP = True
    End Sub

    ' RINOMINA FILE DI DESTINAZIONE
    Private Sub Button58_Click(sender As Object, e As EventArgs) Handles Button58.Click
        If rename = True Then
            TextBox6.ReadOnly = True
            Button58.BackColor = Color.FromArgb(27, 27, 27)

            If TextBox12.Text <> Nothing Then
                Dim ratio As String = ListBox2.SelectedItem
                Dim split = ratio.Split("_", 2, StringSplitOptions.RemoveEmptyEntries)
                TextBox6.Text = split(0)
            End If
            rename = False
        Else
            TextBox6.ReadOnly = False
            Button58.BackColor = Color.DodgerBlue
            rename = True
        End If
    End Sub
    'CAMBIA NOME IN RINOMINA
    Private Sub TextBox8_TextChanged(sender As Object, e As EventArgs) Handles TextBox8.TextChanged

        If ListBox2.Items.Count > 0 Then
            If TextBox8.Text = "" Then
                TableLayoutPanel69.Visible = False
                NumericUpDown6.Value = 1
                TextBox9.Text = ""
                Label21.Text = "...\..."
                Label22.Text = "...\..."
                Label23.Text = "...\..."
            Else
                TableLayoutPanel69.Visible = True

                Dim ratio As String = ListBox2.SelectedItem.ToString
                Dim split2 = ratio.Split(".", 2, StringSplitOptions.RemoveEmptyEntries)
                TextBox9.Text = split2(1)
                Dim n As String = NumericUpDown6.Value
                Label21.Text = "...\" & TextBox8.Text & "_" & n.PadLeft(5, "0") & "." & TextBox9.Text
                n += 1
                Label22.Text = "...\" & TextBox8.Text & "_" & n.PadLeft(5, "0") & "." & TextBox9.Text
                n += 1
                Label23.Text = "...\" & TextBox8.Text & "_" & n.PadLeft(5, "0") & "." & TextBox9.Text
            End If
        End If
    End Sub
    'CAMBIA NUMERO IN RINOMINA
    Private Sub NumericUpDown6_ValueChanged(sender As Object, e As EventArgs) Handles NumericUpDown6.ValueChanged
        If TextBox8.Text = "" Then
            TableLayoutPanel69.Visible = False
            Label21.Text = "...\..."
            Label22.Text = "...\..."
            Label23.Text = "...\..."
        Else
            TableLayoutPanel69.Visible = True
            Dim n As String = NumericUpDown6.Value
            Label21.Text = "...\" & TextBox8.Text & "_" & n.PadLeft(5, "0") & "." & TextBox9.Text
            n += 1
            Label22.Text = "...\" & TextBox8.Text & "_" & n.PadLeft(5, "0") & "." & TextBox9.Text
            n += 1
            Label23.Text = "...\" & TextBox8.Text & "_" & n.PadLeft(5, "0") & "." & TextBox9.Text
        End If
    End Sub
    'CONFERMA RINOMINA TUTTI I FILES
    Private Sub Button60_Click(sender As Object, e As EventArgs) Handles Button60.Click
        If MsgBox("Tutti i Files nella Cartella Saranno Rinominati" & vbCrLf & vbCrLf & "impossibile annullare l'operazione!", MsgBoxStyle.OkCancel Or MsgBoxStyle.Exclamation, ) = MsgBoxResult.Cancel Then
            Exit Sub
        End If
        RENA = True
        DisableMovie()
        Button57.Enabled = False
        Dim BAR As Integer = 0
        ProgressBar2.Maximum = ListBox2.Items.Count.ToString() - 1
        For N As Integer = 0 To ListBox2.Items.Count() - 1
            If ListBox2.SelectedItem = TextBox8.Text & "_" & NumericUpDown6.Text.PadLeft(5, "0") & "." & TextBox9.Text Then
                MsgBox("I Nomi Corrispondono, Impossibile Rinominare", MsgBoxStyle.Critical)
                RENA = False
                EnableMovie()
                Exit Sub
            End If
            Try
                My.Computer.FileSystem.RenameFile(TextBox12.Text & "\" & ListBox2.SelectedItem, TextBox8.Text & "_" & NumericUpDown6.Text.PadLeft(5, "0") & "." & TextBox9.Text)
            Catch ex As Exception
                MsgBox("Errore, File Esistente!", MsgBoxStyle.Critical)
                RENA = False
                EnableMovie()
                Exit Sub
            End Try
            'Threading.Thread.Sleep(100)
            Application.DoEvents()
            ProgressBar2.PerformStep()
            ProgressBar2.Value = BAR
            BAR += 1
            If ListBox2.SelectedIndex <> ListBox2.Items.Count() - 1 Then
                NumericUpDown6.Value += 1
                ListBox2.SelectedIndex += 1
            End If
        Next
        RENA = False
        EnableMovie()
        Button51.PerformClick()
        Threading.Thread.Sleep(100)
        ProgressBar2.Value = 0
        NumericUpDown6.Value = 1
        TextBox8.Text = ""
    End Sub
    ' DISBILITA TUTTI I CONTROLLI TRANNE STOP
    Private Sub DisableMovie()


        Button56.Enabled = False
        Button57.Enabled = True
        Panello_comandi.Enabled = False
        Panello_chiudi.Enabled = False
        TableLayoutPanel90.Enabled = False
        TableLayoutPanel50.Enabled = False
        TableLayoutPanel56.Enabled = False
        TableLayoutPanel62.Enabled = False
        TableLayoutPanel63.Enabled = False
        TableLayoutPanel64.Enabled = False
        TableLayoutPanel65.Enabled = False
        TableLayoutPanel66.Enabled = False
        TableLayoutPanel67.Enabled = False
        If PLAYMOVIE = True Then
            TableLayoutPanel50.Enabled = True
            Button57.Enabled = False
            TableLayoutPanel54.Enabled = False
            TableLayoutPanel57.Enabled = False
            TableLayoutPanel58.Enabled = False
            ListBox3.Enabled = False
            Button47.Enabled = False
            Button47.BackColor = Color.DodgerBlue
            Button46.Enabled = True
        End If

    End Sub
    ' ABILITA TUTTI I CONTROLLI TRANNE STOP
    Private Sub EnableMovie()
        Button56.Enabled = True
        Button57.Enabled = False
        Panello_comandi.Enabled = True
        Panello_chiudi.Enabled = True
        TableLayoutPanel90.Enabled = True
        TableLayoutPanel50.Enabled = True
        TableLayoutPanel56.Enabled = True
        TableLayoutPanel62.Enabled = True
        TableLayoutPanel63.Enabled = True
        TableLayoutPanel64.Enabled = True
        TableLayoutPanel65.Enabled = True
        TableLayoutPanel66.Enabled = True
        TableLayoutPanel67.Enabled = True
        If PLAYMOVIE = True Then
            TableLayoutPanel50.Enabled = True
            TableLayoutPanel54.Enabled = True
            TableLayoutPanel57.Enabled = True
            TableLayoutPanel58.Enabled = True
            ListBox3.Enabled = True
            Button47.Enabled = True
            Button47.BackColor = Color.FromArgb(27, 27, 27)
            Button46.Enabled = False
        End If
    End Sub
#End Region

#Region "MOD CLICK"

    'apre click mod
    Private Sub RadioButton3_CheckedChanged(sender As Object, e As EventArgs) Handles RadioButton3.CheckedChanged
        If RadioButton3.Checked = True Then

            setclick = True

            If Button8.Visible = True Then
                Button8.PerformClick()
            End If
            Panello_arduino.Visible = True
            TableLayoutPanel1.Visible = True
            Panel35.Visible = True
            Panel36.Visible = True
            Button7.Enabled = False 'blocca tasto schermo intero
            Button100.Visible = True
            Button101.Visible = True
            Me.Size = New Size(Panello_arduino.Width + Panello_comandi.Width, Panello_arduino.Height + Panello_chiudi.Height + Panello_sorgente.Height)
        Else
            If menuclick = True Then
                setmenuclick = True
                Button101.PerformClick()
            End If
            setclick = False
            Button101.Visible = False
            Button100.Visible = False
            Panello_arduino.Visible = False
            TableLayoutPanel1.Visible = False
            Panel35.Visible = False
            Panel36.Visible = False
            Button7.Enabled = True

            Me.Size = New Size(originalx, originaly)
        End If
    End Sub

    ' rileva coordinate mouse Click
    Private Sub Button6_Click(sender As Object, e As EventArgs) Handles Button6.Click
        Dialog1.ShowDialog()
        Thread.Sleep(100)
        TextBox2.Text = My.Settings.xclick
        TextBox3.Text = My.Settings.yclick
    End Sub
#End Region

#Region "MOD PICTURE"
    'apre pic mod
    Private Sub RadioButton6_CheckedChanged(sender As Object, e As EventArgs) Handles RadioButton6.CheckedChanged
        Dim p11 As Integer = Panello_sorgente.Height
        If RadioButton6.Checked = True Then
            PictureBox22.Image = Nothing
            PictureBox22.BackgroundImage = Nothing
            PictureBox24.Image = Nothing
            picmod = True
            Button75.Visible = True
            Button76.Visible = True
            Button97.Visible = True
            Panello_elabora.Size = New Size(Panello_chiudi.Width - Panello_comandi.Width - Panello_arduino.Width, Panello_comandi.Height - Panello_sorgente.Height)
            Panello_elabora.Visible = True
            Panello_sorgente.Size = New Size(Panello_chiudi.Width - Panello_comandi.Width, p11)
            Panello_sorgente.Visible = True
            Panel35.Visible = True
            Panello_elenco.Visible = True
            Panel47.Visible = True
            CheckBox4.Visible = True 'specchia x
            CheckBox19.Visible = True ' specchia y
            Button21.Visible = True ' zoom
            CheckBox8.Visible = True ' apri avanzato

            AVA = True
            ' CheckBox8.Checked = CheckState.Checked
            CheckBox8.Enabled = False
            TableLayoutPanel28.Visible = True
            CheckBox7.Visible = True
            CheckBox6.Visible = True
            CheckBox7.Checked = True
            CheckBox6.Checked = True
            TextBox14.Text = My.Settings.cart_dest_pic

            Button95.PerformClick()

        Else

            Button75.Visible = False
            Button76.Visible = False
            Button97.Visible = False
            Panello_elabora.Visible = False
            Panello_sorgente.Visible = False
            Panello_sorgente.Size = New Size(Panello_arduino.Width, p11)
            Panel35.Visible = False
            Panello_elenco.Visible = False
            Panel47.Visible = False
            PictureBox6.Image = Nothing
            PictureBox22.Image = Nothing
            CheckBox4.Visible = False 'specchia x
            CheckBox19.Visible = False ' specchia y
            Button21.Visible = False ' zoom
            CheckBox8.Visible = False ' apri avanzato
            picmod = False

            AVA = False
            ' CheckBox8.Checked = CheckState.Unchecked
            CheckBox8.Enabled = True
            REES = False
            REES2 = False
            TableLayoutPanel28.Visible = False
            CheckBox7.Checked = False
            CheckBox6.Checked = False
            CheckBox6.Visible = False
            CheckBox7.Visible = False
            Button27.Visible = False ' salva avanzato
            Button28.Visible = False ' ricarica avanzato
            CheckBox9.Visible = False ' ridimensiona sposa selezione
            PictureBox22.Image = Nothing
            PictureBox22.BackgroundImage = Nothing
            PictureBox24.Image = Nothing


            CONTROL = True
            CONTROL1 = True
            CONTROL2 = True
            ToolTip1.SetToolTip(CheckBox8, "Controllo Avanzato")

        End If






    End Sub

    ' APRI/SELEZIONA VIEWER ESTERNO CARTELLA ORIGINE 
    Private Sub Button102_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles Button102.MouseDown
        If ListBox8.Items.Count = Nothing Then
            Exit Sub
        End If
        Dim VIEW As String = My.Settings.viewer_set
        If e.Button = Windows.Forms.MouseButtons.Right Then
            If MsgBox("Cambiare Viewer Esterno?", MsgBoxStyle.OkCancel Or MsgBoxStyle.Question, ) = MsgBoxResult.Ok Then
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
                Process.Start(VIEW, TextBox12.Text & "\" & ListBox8.SelectedItem)
            End If
        End If
    End Sub
    ' APRI/SELEZIONA VIEWER ESTERNO CARTELLA DESTINAZIONE 
    Private Sub Button79_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles Button79.MouseDown
        If ListBox6.Items.Count = Nothing Then
            Exit Sub
        End If
        Dim VIEW As String = My.Settings.viewer_set
        If e.Button = Windows.Forms.MouseButtons.Right Then
            If MsgBox("Cambiare Viewer Esterno?", MsgBoxStyle.OkCancel Or MsgBoxStyle.Question, ) = MsgBoxResult.Ok Then
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
                Process.Start(VIEW, TextBox14.Text & "\" & ListBox6.SelectedItem)
            End If
        End If
    End Sub
    ' INFORMAZIONI FILE CARTELLA ORIGINE
    Private Sub ListBox8_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles ListBox8.MouseDown
        If ListBox8.Items.Count = Nothing Then
            Exit Sub
        End If
        If e.Button = MouseButtons.Right Then
            ListBox8.SelectedIndex = ListBox8.IndexFromPoint(e.X, e.Y)
            Dim MyimageWxH
            Try
                Dim Myimg As System.Drawing.Image = System.Drawing.Image.FromFile(TextBox12.Text & "\" & ListBox8.SelectedItem)
                Dim MyImageWidth = PictureBox22.BackgroundImage.Width
                Dim MyImageHeight = PictureBox22.BackgroundImage.Height
                MyimageWxH = MyImageWidth & " X " & MyImageHeight
            Catch
                MyimageWxH = "Not Supported"
            End Try
            Dim Myimagepropety = FileLen(TextBox12.Text & "\" & ListBox8.SelectedItem)
            Dim Myimagedate As DateTime = File.GetCreationTime(TextBox12.Text & "\" & ListBox8.SelectedItem)
            MsgBox("Informazioni:" & vbCrLf & vbCrLf & TextBox12.Text & "\" & ListBox8.SelectedItem & vbCrLf & vbCrLf & " Data Creazione:  " & Myimagedate & vbCrLf & vbCrLf & " Formato:  " & MyimageWxH & vbCrLf & vbCrLf & " Dimensione:  " & System.Math.Round(Myimagepropety / 1000, 1) & " KB", MessageBoxIcon.Information)
        End If
    End Sub
    ' INFORMAZIONI FILE CARTELLA DESTINAZIONE
    Private Sub ListBox6_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles ListBox6.MouseDown
        If ListBox6.Items.Count = Nothing Then
            Exit Sub
        End If
        If e.Button = MouseButtons.Right Then
            ListBox6.SelectedIndex = ListBox6.IndexFromPoint(e.X, e.Y)
            Dim MyimageWxH
            Try
                Dim Myimg As System.Drawing.Image = System.Drawing.Image.FromFile(TextBox14.Text & "\" & ListBox6.SelectedItem)
                Dim MyImageWidth = PictureBox24.Image.Width
                Dim MyImageHeight = PictureBox24.Image.Height
                MyimageWxH = MyImageWidth & " X " & MyImageHeight
            Catch
                MyimageWxH = "Not Supported"
            End Try
            Dim Myimagepropety = FileLen(TextBox14.Text & "\" & ListBox6.SelectedItem)
            Dim Myimagedate As DateTime = File.GetCreationTime(TextBox14.Text & "\" & ListBox6.SelectedItem)
            MsgBox("Informazioni:" & vbCrLf & vbCrLf & TextBox14.Text & "\" & ListBox6.SelectedItem & vbCrLf & vbCrLf & " Data Creazione:  " & Myimagedate & vbCrLf & vbCrLf & " Formato:  " & MyimageWxH & vbCrLf & vbCrLf & " Dimensione:  " & System.Math.Round(Myimagepropety / 1000, 1) & " KB", MessageBoxIcon.Information)
        End If
    End Sub
    ' CAMBIO SELEZIONE LISTA CARTELLA ORIGINE 
    Private Sub ListBox8_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ListBox8.SelectedIndexChanged
        If RENAME1 = False Then
            Try
                Dim ratio As String = ListBox8.SelectedItem.ToString()
                Dim split = ratio.Split("_", 2, StringSplitOptions.RemoveEmptyEntries)
                Dim split1 = ratio.Split("_"c, "."c)(1)
                TextBox13.Text = split(0)
                NumericUpDown7.Value = split1
                ELABORA()
            Catch ex As Exception
                MsgBox("Nome file non Valido" & vbCrLf & vbCrLf & "Formato corretto" & vbCrLf & vbCrLf & "...\Nome_00000.jpg")
            End Try
        End If
    End Sub
    ' RITAGLIA SINGOLO FOTOGRAMMA
    Private Sub Button99_Click(sender As Object, e As EventArgs) Handles Button99.Click
        If ListBox6.Items.Count.ToString() > 0 Then
            Select Case MsgBox("ATTENZIONE!" & vbCrLf & vbCrLf & "la cartella di Destinazione non è vuota." & vbCrLf & vbCrLf & "I Nomi Esistenti saranno Sovrascritti." & vbCrLf & vbCrLf & "Avvio Comunque il Processo?", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                Case Windows.Forms.DialogResult.Yes
                    Exit Select
                Case Windows.Forms.DialogResult.No
                    Exit Sub
            End Select
        End If

        If Directory.Exists(TextBox14.Text) Then
            If ListBox8.SelectedItem IsNot Nothing Then
                If rectCropArea.X = Nothing Or rectCropArea.Y = Nothing Or rectCropArea.Width = Nothing Or rectCropArea.Height = Nothing Then
                    MsgBox("Seleziona un Rettangolo")
                    Exit Sub
                Else
                    If CheckBox7.Checked = True Then
                        If IMAGEN IsNot Nothing Then
                            Dim ratio As String = ListBox8.SelectedItem.ToString()
                            Dim split2 = ratio.Split(".", 2, StringSplitOptions.RemoveEmptyEntries)
                            Dim no As String = (TextBox14.Text & "\" & TextBox13.Text & "_" & NumericUpDown7.Text.PadLeft(5, "0")) & "." & split2(1)
                            Dim croBitmap As New Bitmap(CROPMB, CROPMH)
                            Dim p As Graphics = Graphics.FromImage(croBitmap)
                            p.InterpolationMode = Drawing2D.InterpolationMode.HighQualityBicubic
                            p.PixelOffsetMode = Drawing2D.PixelOffsetMode.HighQuality
                            p.CompositingQuality = Drawing2D.CompositingQuality.HighQuality
                            If CheckBox13.Checked = True And CheckBox14.Checked = False Then
                                p.DrawImage(IMAGEN, New Rectangle(0, 0, CROPMB, CROPMH), IMGRCX + BLX + CROPMX, IMGRCY + BLY - CROPMY - CInt(IMGRCH / 2), CROPMB, CROPMH, GraphicsUnit.Pixel)
                            End If
                            If CheckBox13.Checked = True And CheckBox14.Checked = True Or CheckBox13.Checked = False And CheckBox14.Checked = False Then
                                p.DrawImage(IMAGEN, New Rectangle(0, 0, CROPMB, CROPMH), IMGRCX + BLX + CROPMX, IMGRCY + BLY + CInt(BLH / 2) - CROPMY - CInt(IMGRCH / 2), CROPMB, CROPMH, GraphicsUnit.Pixel)
                            End If
                            If CheckBox13.Checked = False And CheckBox14.Checked = True Then
                                p.DrawImage(IMAGEN, New Rectangle(0, 0, CROPMB, CROPMH), IMGRCX + BLX + CROPMX, IMGRCY + BLY + BLH - CROPMY - CInt(IMGRCH / 2), CROPMB, CROPMH, GraphicsUnit.Pixel)
                            End If
                            Select Case split2(1)
                                Case "jpg", "JPG"
                                    croBitmap.Save(no, System.Drawing.Imaging.ImageFormat.Jpeg)
                                Case "bmp", "BMP"
                                    croBitmap.Save(no, System.Drawing.Imaging.ImageFormat.Bmp)
                                Case "png", "PNG"
                                    croBitmap.Save(no, System.Drawing.Imaging.ImageFormat.Png)
                                Case "gif", "GIF"
                                    croBitmap.Save(no, System.Drawing.Imaging.ImageFormat.Gif)
                                Case "tiff", "TIFF"
                                    croBitmap.Save(no, System.Drawing.Imaging.ImageFormat.Tiff)
                            End Select
                            'Thread.Sleep(250)
                            ListBox6.Items.Add(TextBox13.Text & "_" & NumericUpDown7.Text.PadLeft(5, "0") & "." & split2(1))
                            ListBox6.SelectedIndex = ListBox6.Items.Count.ToString() - 1
                            Label31.Text = Directory.GetFiles(TextBox14.Text).Length & " Files "
                        End If

                        'If IMAGEN IsNot Nothing Then IMAGEN.Dispose()
                        'If IMAGEN IsNot Nothing Then IMAGEN = Nothing
                    Else
                        MsgBox("Funzione Ritaglio non selezionata!")
                        Exit Sub
                    End If
                End If
            Else
                MsgBox("La Cartella di Origine è Vuota")
            End If
        Else
            MsgBox("Selezionare cartella di Destinazione.")
        End If
    End Sub

    ''''' AVVIO RITAGLIA TUTTI IN MODALITA PICTURE '''''
    Private Sub Button85_Click(sender As Object, e As EventArgs) Handles Button85.Click
        If ListBox6.Items.Count.ToString() > 0 Then
            Select Case MsgBox("ATTENZIONE!" & vbCrLf & vbCrLf & "la cartella di Destinazione non è vuota." & vbCrLf & vbCrLf & "I Nomi Esistenti saranno Sovrascritti." & vbCrLf & vbCrLf & "Avvio Comunque il Processo?", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                Case Windows.Forms.DialogResult.Yes
                    Exit Select
                Case Windows.Forms.DialogResult.No
                    Exit Sub
            End Select
        End If
        Dim a As String = TimeOfDay()
        Dim numero = (ListBox8.Items.Count.ToString() - 1) - ListBox8.SelectedIndex
        Dim index As Integer = 0
        CheckBox9.Checked = False
        DISABLEALL()
        PictureBox30.Visible = True
        If Directory.Exists(TextBox14.Text) Then
            If ListBox8.SelectedItem IsNot Nothing Then
                Dim W As Integer = 0
                ProgressBar3.Maximum = numero
                ''''''''''''''''''''''''''''''''
                While index <= numero
                    Application.DoEvents()
                    If STOPIC = True Then
                        STOPIC = False
                        ENABLEALL()
                        PictureBox30.Visible = False
                        MsgBox("ANNULLATO" & vbCrLf & vbCrLf & "Files Creati n. " & index)
                        Label31.Text = Directory.GetFiles(TextBox14.Text).Length & " Files "
                        ProgressBar3.Value = 0
                        Exit Sub
                    Else
                        If rectCropArea.X = Nothing Or rectCropArea.Y = Nothing Or rectCropArea.Width = Nothing Or rectCropArea.Height = Nothing Then
                            ENABLEALL()
                            PictureBox30.Visible = False
                            MsgBox("Seleziona un Rettangolo")
                            Exit Sub
                        Else
                            If CheckBox7.Checked = True Then
                                Application.DoEvents()
                                If CONTROL = True And CONTROL1 = True Then
                                    Dim ratio As String = ListBox8.SelectedItem.ToString()
                                    Dim split2 = ratio.Split(".", 2, StringSplitOptions.RemoveEmptyEntries)
                                    Dim no As String = (TextBox14.Text & "\" & TextBox13.Text & "_" & NumericUpDown7.Text.PadLeft(5, "0")) & "." & split2(1)
                                    If IMAGEN Is Nothing Then
                                        MsgBox("ERRORE! Immagine" & vbCrLf & vbCrLf & "Files Creati n. " & index)
                                        ENABLEALL()
                                        PictureBox30.Visible = False
                                        ProgressBar3.Value = 0
                                        Label31.Text = Directory.GetFiles(TextBox14.Text).Length & " Files "
                                        ProgressBar3.Value = 0
                                        Exit Sub
                                    End If

                                    Dim croBitmap As New Bitmap(IMAGEN, CROPMB, CROPMH)
                                    Dim p As Graphics = Graphics.FromImage(croBitmap)
                                    p.InterpolationMode = Drawing2D.InterpolationMode.HighQualityBicubic
                                    p.PixelOffsetMode = Drawing2D.PixelOffsetMode.HighQuality
                                    p.CompositingQuality = Drawing2D.CompositingQuality.HighQuality
                                    If CheckBox13.Checked = True And CheckBox14.Checked = False Then
                                        p.DrawImage(IMAGEN, New Rectangle(0, 0, CROPMB, CROPMH), IMGRCX + BLX + CROPMX, IMGRCY + BLY - CROPMY - CInt(IMGRCH / 2), CROPMB, CROPMH, GraphicsUnit.Pixel)
                                    End If
                                    If CheckBox13.Checked = True And CheckBox14.Checked = True Or CheckBox13.Checked = False And CheckBox14.Checked = False Then
                                        p.DrawImage(IMAGEN, New Rectangle(0, 0, CROPMB, CROPMH), IMGRCX + BLX + CROPMX, IMGRCY + BLY + CInt(BLH / 2) - CROPMY - CInt(IMGRCH / 2), CROPMB, CROPMH, GraphicsUnit.Pixel)
                                    End If
                                    If CheckBox13.Checked = False And CheckBox14.Checked = True Then
                                        p.DrawImage(IMAGEN, New Rectangle(0, 0, CROPMB, CROPMH), IMGRCX + BLX + CROPMX, IMGRCY + BLY + BLH - CROPMY - CInt(IMGRCH / 2), CROPMB, CROPMH, GraphicsUnit.Pixel)
                                    End If
                                    'Thread.Sleep(100)
                                    Application.DoEvents()
                                    If CONTROL = True And CONTROL1 = True Then
                                        Select Case split2(1)
                                            Case "jpg", "JPG"
                                                croBitmap.Save(no, System.Drawing.Imaging.ImageFormat.Jpeg)
                                            Case "bmp", "BMP"
                                                croBitmap.Save(no, System.Drawing.Imaging.ImageFormat.Bmp)
                                            Case "png", "PNG"
                                                croBitmap.Save(no, System.Drawing.Imaging.ImageFormat.Png)
                                            Case "gif", "GIF"
                                                croBitmap.Save(no, System.Drawing.Imaging.ImageFormat.Gif)
                                            Case "tiff", "TIFF"
                                                croBitmap.Save(no, System.Drawing.Imaging.ImageFormat.Tiff)
                                        End Select
                                        ListBox6.Items.Add(TextBox13.Text & "_" & NumericUpDown7.Text.PadLeft(5, "0") & "." & split2(1))
                                        ListBox6.SelectedIndex = ListBox6.Items.Count.ToString() - 1
                                        index += 1
                                        NumericUpDown7.Value += 1
                                        ProgressBar3.Value = W
                                        If ListBox8.Items.Count.ToString() - 1 <> ListBox8.SelectedIndex Then
                                            ListBox8.SelectedIndex += 1
                                            W += 1
                                        End If
                                        Label31.Text = index & " Files Creati"
                                    End If
                                Else
                                    MsgBox("ERRORE! Verifica Controlli" & vbCrLf & vbCrLf & "Files Creati n. " & index)
                                    ENABLEALL()
                                    PictureBox30.Visible = False
                                    ProgressBar3.Value = 0
                                    Label31.Text = Directory.GetFiles(TextBox14.Text).Length & " Files "
                                    ProgressBar3.Value = 0
                                    Exit Sub
                                End If
                            Else
                                ENABLEALL()
                                PictureBox30.Visible = False
                                MsgBox("Funzione Ritaglio non selezionata!")
                                Exit Sub
                            End If
                        End If

                    End If
                End While
                ENABLEALL()
                PictureBox30.Visible = False
                Dim b As String = TimeOfDay()
                Dim t1 As TimeSpan = TimeSpan.Parse(a)
                Dim t2 As TimeSpan = TimeSpan.Parse(b)
                Dim tDiff As TimeSpan
                tDiff = t2.Subtract(t1)
                Dim days = (tDiff.ToString)
                MsgBox("OPERAZIONE COMPLETATA" & vbCrLf & vbCrLf & "Files Creati n. " & index & vbCrLf & vbCrLf & "Tempo Impiegato: " & days)
                Label31.Text = Directory.GetFiles(TextBox14.Text).Length & " Files "
                ProgressBar3.Value = 0
            Else
                ENABLEALL()
                PictureBox30.Visible = False
                MsgBox("La Cartella di Origine è Vuota")
            End If
        Else
            ENABLEALL()
            PictureBox30.Visible = False
            MsgBox("Selezionare cartella di Destinazione.")
        End If
    End Sub
    ' APRI CARTELLA ORIGINE IN EXPLORER
    Private Sub Button96_Click(sender As Object, e As EventArgs) Handles Button96.Click
        If TextBox12.Text <> Nothing Then
            Process.Start("explorer.exe", TextBox12.Text)
        End If
    End Sub
    ' APRI CARTELLA DESTINAZIONE IN EXPLORER
    Private Sub Button80_Click(sender As Object, e As EventArgs) Handles Button80.Click
        If TextBox14.Text <> Nothing Then
            Process.Start("explorer.exe", TextBox14.Text)
        End If
    End Sub
    ' AGGIORNA LISTBOX CARTELLA SORGENTE
    Private Sub Button95_Click(sender As Object, e As EventArgs) Handles Button95.Click
        'PictureBox22.BackgroundImage = My.Resources.ANTEPRIMA
        Label33.Text = 0 & " File "
        If TextBox12.Text <> "" Then
            Dim files() As String = Directory.GetFiles(TextBox12.Text)
            ListBox8.Items.Clear()
            For Each file As String In files
                ListBox8.Items.Add(Path.GetFileName(file))
            Next
            Dim pat As String
            pat = TextBox12.Text
            Dim filess() As String
            If Directory.Exists(pat) Then
                filess = Directory.GetFiles(pat)
                Label33.Text = filess.Length & " Files "
                If ListBox8.Items.Count.ToString() = 0 Then
                    Exit Sub
                Else
                    ListBox8.SelectedIndex = 0
                End If
            End If
        End If
    End Sub
    ' AGGIORNA LISTBOX CARTELLA DESTINAZIONE 
    Private Sub Button81_Click(sender As Object, e As EventArgs) Handles Button81.Click
        'PictureBox24.Image = My.Resources.ANTEPRIMA
        Label31.Text = 0 & " File "
        If TextBox14.Text <> "" Then
            Dim files() As String = Directory.GetFiles(TextBox14.Text)
            ListBox6.Items.Clear()
            For Each file As String In files
                ListBox6.Items.Add(Path.GetFileName(file))
            Next
            Dim pat As String = TextBox14.Text
            Dim filess() As String
            If Directory.Exists(pat) Then
                filess = Directory.GetFiles(TextBox14.Text)
                Label31.Text = filess.Length & " Files "
                If ListBox6.Items.Count.ToString() = 0 Then
                    Exit Sub
                Else
                    ListBox6.SelectedIndex = 0
                End If
            End If
        End If
    End Sub
    ' FERMA CREAZIONE RITAGLIO IMMAGINE DA PICTURE
    Private Sub Button82_Click(sender As Object, e As EventArgs) Handles Button82.Click
        If STOPIC <> True Then
            STOPIC = True
            Button82.Enabled = False
            ListBox8.Enabled = True
            ListBox6.Enabled = True
            Button85.Enabled = True
            PictureBox30.Visible = False
        End If
    End Sub
    ' CANCELLA TUTTO IL CONTENUTO DELLA CARTELLA DESTINAZIONE     
    Private Sub Button78_Click(sender As Object, e As EventArgs) Handles Button78.Click
        If ListBox6.Items.Count > 0 Then
            Select Case MsgBox("Cancellare Tutte le Immagini" & vbCrLf & vbCrLf & "nella Cartella di Destinazione?", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                Case Windows.Forms.DialogResult.Yes
                    ListBox6.ClearSelected()
                    If Directory.Exists(TextBox14.Text) Then
                        For Each _file As String In Directory.GetFiles(TextBox14.Text)
                            File.Delete(_file)
                        Next
                    End If
                    ListBox6.Items.Clear()
                    If Directory.Exists(TextBox14.Text) Then
                        Dim filess = Directory.GetFiles(TextBox14.Text)
                        Label31.Text = filess.Length & " Files "
                    End If
                    If PictureBox24.Image IsNot Nothing Then PictureBox24.Image = Nothing
                Case Windows.Forms.DialogResult.No
                    Exit Sub
            End Select
        End If
    End Sub
    ' RUOTA IMMAGINE ANTIORARIO
    Private Sub Button97_Click(sender As Object, e As EventArgs) Handles Button97.Click
        angolo += 0.1
        rotate = True
        If angolo <= -0.1 Or angolo >= 0.1 Then
            If angolo <= -0.1 Then
                Button75.Text = -angolo * 10
            End If
            If angolo >= 0.1 Then
                Button97.Text = angolo * 10
            End If

        Else
            Button75.Text = Nothing
            Button97.Text = Nothing
            angolo = 0



        End If


        ELABORA()
    End Sub
    ' RUOTA IMMAGINE ORARIO
    Private Sub Button75_Click(sender As Object, e As EventArgs) Handles Button75.Click
        angolo -= 0.1
        rotate = True
        If angolo <= -0.1 Or angolo >= 0.1 Then
            If angolo <= -0.1 Then
                Button75.Text = -angolo * 10
            End If
            If angolo >= 0.1 Then
                Button97.Text = angolo * 10
            End If
        Else
            angolo = 0
            Button75.Text = Nothing
            Button97.Text = Nothing

        End If
        ELABORA()
    End Sub
    ' RIPRISTINA MODIFICHE IMMAGINE
    Private Sub Button76_Click(sender As Object, e As EventArgs) Handles Button76.Click
        If FLPY = True Then
            CheckBox4.Checked = CheckState.Unchecked
            FLPY = False
        End If
        If FLPX = True Then
            CheckBox19.CheckState = CheckState.Unchecked
            FLPX = False
        End If
        rotate = False
        angolo = 0
        Button97.Text = Nothing
        Button75.Text = Nothing
        ELABORA()
    End Sub
    ' SELEZIONA E VISUALIZZA IMMAGINI CARTELLA DESTINAZIONE
    Private Sub ListBox6_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ListBox6.SelectedIndexChanged
        Try
            PictureBox24.Load(TextBox14.Text & "/" & ListBox6.SelectedItem)
        Catch ex As Exception
            '  PictureBox24.Image = My.Resources.ANTEPRIMA
        End Try
    End Sub
    ' ATTIVA/DISATTIVA RINOMINA IMMAGINI DESTINAZIONE IN MODALITA' AVANZATA 
    Private Sub Button84_Click(sender As Object, e As EventArgs) Handles Button84.Click
        If ListBox8.SelectedIndex > -1 Then
            If RENAME1 = True Then
                TextBox13.ReadOnly = True
                NumericUpDown7.ReadOnly = True
                NumericUpDown7.Enabled = False
                Button84.BackColor = Color.FromArgb(27, 27, 27)
                Try
                    Dim ratio As String = ListBox8.SelectedItem
                    Dim split = ratio.Split("_", 2, StringSplitOptions.RemoveEmptyEntries)
                    Dim split1 = ratio.Split("_"c, "."c)(1)
                    TextBox13.Text = split(0)
                    NumericUpDown7.Value = split1
                Catch ex As Exception
                    MsgBox("Nome file non Valido" & vbCrLf & vbCrLf & "Formato corretto" & vbCrLf & vbCrLf & "File_00000.jpg")
                End Try
                RENAME1 = False
            Else
                TextBox13.ReadOnly = False
                NumericUpDown7.ReadOnly = False
                NumericUpDown7.Enabled = True
                NumericUpDown7.Value = 1
                Button84.BackColor = Color.DodgerBlue
                RENAME1 = True
            End If
        End If

    End Sub
    ' SELEZIONA CARTELLA DI DESTINAZIONE IN AVANZATO
    Private Sub Button83_Click(sender As Object, e As EventArgs) Handles Button83.Click
        Using folderDlg As New FolderBrowserDialog
            folderDlg.SelectedPath = TextBox14.Text
            folderDlg.ShowNewFolderButton = True
            'PictureBox24.Image = My.Resources.ANTEPRIMA
            Label31.Text = 0 & " File "
            If (folderDlg.ShowDialog() = DialogResult.OK) Then
                TextBox14.Text = folderDlg.SelectedPath
                If TextBox12.Text = TextBox14.Text Then
                    MessageBox.Show("la cartella di Origine non puo essere" & vbCrLf & vbCrLf & "la cartella di Destinazione")
                    TextBox14.Text = ""
                    ListBox6.Items.Clear()
                    ' PictureBox24.Image = My.Resources.ANTEPRIMA
                    Button83.PerformClick()
                    Exit Sub
                End If

                My.Settings.cart_dest_pic = TextBox14.Text

                If TextBox14.Text <> "" Then
                    Dim files() As String = Directory.GetFiles(TextBox14.Text)
                    ListBox6.Items.Clear()
                    For Each file As String In files
                        ListBox6.Items.Add(Path.GetFileName(file))
                    Next
                    Dim pat As String
                    pat = TextBox14.Text
                    Dim filess() As String
                    If Directory.Exists(pat) Then
                        filess = Directory.GetFiles(pat)
                        Label31.Text = filess.Length & " Files "
                        If ListBox6.Items.Count.ToString() = 0 Then
                            ' PictureBox24.Image = My.Resources.ANTEPRIMA
                        Else
                            ListBox6.SelectedIndex = 0
                        End If
                    End If
                Else
                    ' PictureBox24.Image = My.Resources.ANTEPRIMA
                End If
            End If
        End Using
    End Sub
#End Region

#Region "SETUP"
    'apre/chiude setup
    Private Sub Button38_Click(sender As Object, e As EventArgs) Handles Button38.Click
        If setclick = True Then
            If setmenuclick = True Then
                setmenuclick = False
                Button38.BackColor = Color.FromArgb(27, 27, 27)
                Button38.Enabled = False
                Button100.Enabled = True
                Button100.BackColor = Color.FromArgb(64, 64, 64)
                Panello_target_click.Visible = False
                Panel20.Visible = True
                Pannello_setup.Size = New Size(Panello_arduino.Width, Panello_comandi.Height - Panello_sorgente.Height - Panello_arduino.Height)
                Pannello_setup.Visible = True
                Button1.Visible = True
                Button2.Visible = True
            End If
        Else
            If setmenuclick = False Then
                setmenuclick = True
                Button38.BackColor = Color.FromArgb(27, 27, 27)
                Panel20.Visible = True
                Pannello_setup.Size = New Size(Panello_arduino.Width, Panello_comandi.Height - Panello_sorgente.Height - Panello_arduino.Height)
                Pannello_setup.Visible = True
                Button1.Visible = True
                Button2.Visible = True
            Else
                setmenuclick = False
                Button38.BackColor = Color.FromArgb(64, 64, 64)
                Panel20.Visible = False
                Pannello_setup.Visible = False
                Button1.Visible = False
                Button2.Visible = False
            End If
        End If
    End Sub
    'apre setup coordinate click
    Private Sub Button100_Click(sender As Object, e As EventArgs) Handles Button100.Click

        If setmenuclick = False Then
            setmenuclick = True
            Button100.BackColor = Color.FromArgb(27, 27, 27)
            Button100.Enabled = False
            Button38.BackColor = Color.FromArgb(64, 64, 64)
            Button38.Enabled = True
            Panello_target_click.Visible = True
            Panel20.Visible = False

        End If


    End Sub
    'apre/chiude setup in mod click
    Private Sub Button101_Click(sender As Object, e As EventArgs) Handles Button101.Click
        If menuclick = False Then
            menuclick = True
            Button101.BackgroundImage = My.Resources.up
            Me.Size = New Size(Panello_arduino.Width + Panello_comandi.Width, originaly)
            Button38.BackColor = Color.FromArgb(27, 27, 27)
            Button38.Enabled = False
            Panel20.Visible = True
            Pannello_setup.Size = New Size(Panello_arduino.Width, Panello_comandi.Height - Panello_sorgente.Height - Panello_arduino.Height)
            Pannello_setup.Visible = True
            Button1.Visible = True
            Button2.Visible = True
        Else
            menuclick = False
            setmenuclick = False
            Button38.Enabled = True
            Button100.Enabled = True
            Button38.BackColor = Color.FromArgb(64, 64, 64)
            Button100.BackColor = Color.FromArgb(64, 64, 64)
            Panello_target_click.Visible = False
            Panel20.Visible = True
            Pannello_setup.Visible = False
            Button1.Visible = False
            Button2.Visible = False
            Button101.BackgroundImage = My.Resources.down
            Me.Size = New Size(Panello_arduino.Width + Panello_comandi.Width, Panello_chiudi.Height + Panello_sorgente.Height + Panello_arduino.Height)
        End If
    End Sub
    ' SALVA IMPOSTAZIONI setup
    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        If MsgBox("Il precedente Salvataggio andrà perso!" & vbCrLf & vbCrLf & "Sicuro di Salvare i Dati?", MsgBoxStyle.YesNo, ) = MsgBoxResult.Yes Then
            My.Settings.set_com = ComboBox1.Text
            My.Settings.click_x = TextBox2.Text
            My.Settings.click_y = TextBox3.Text
            My.Settings.wait_pic = TextBox4.Text
            My.Settings.wait_motor = TextBox5.Text
            My.Settings.wait_step = TextBox34.Text
            My.Settings.wait_microstep = TextBox35.Text
            My.Settings.val_led = TrackBar2.Value
            My.Settings.val_motor = TrackBar4.Value
        End If
    End Sub
    ' CARICA IMPOSTAZIONI SALVATE setup
    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        If MsgBox("Le attuali impostazioni andranno perse!" & vbCrLf & vbCrLf & "Sicuro di velere Caricare i Dati?", MsgBoxStyle.YesNo, ) = MsgBoxResult.Yes Then
            ComboBox1.Text = My.Settings.set_com
            TextBox2.Text = My.Settings.click_x
            TextBox3.Text = My.Settings.click_y
            TextBox4.Text = My.Settings.wait_pic
            TextBox5.Text = My.Settings.wait_motor
            TextBox34.Text = My.Settings.wait_step
            TextBox35.Text = My.Settings.wait_microstep
            TrackBar2.Value = My.Settings.val_led
            TrackBar4.Value = My.Settings.val_motor
        End If
    End Sub

#Region "LUCE E MOTORE DC"
    'accene/spegne led
    Private Sub Button106_Click(sender As Object, e As EventArgs) Handles Button106.Click
        If CONNETTI = True Then
            If led = True Then
                PictureBox23.BackgroundImage = My.Resources.light_off
                PictureBox23.BackColor = Color.FromArgb(27, 27, 27)
                Button106.BackgroundImage = My.Resources.off
                If Not SerialPort1.IsOpen Then SerialPort1.Open()
                SerialPort1.Write("7")
                If TrackBar2.Value = 0 And TrackBar2.Value < 1 Then
                    SerialPort1.Write("1")
                ElseIf TrackBar2.Value > 0 And TrackBar2.Value <= 1 Then
                    SerialPort1.Write("2")
                ElseIf TrackBar2.Value > 1 And TrackBar2.Value <= 2 Then
                    SerialPort1.Write("3")
                ElseIf TrackBar1.Value > 2 And TrackBar2.Value <= 3 Then
                    SerialPort1.Write("4")
                ElseIf TrackBar2.Value > 3 And TrackBar2.Value <= 4 Then
                    SerialPort1.Write("5")
                ElseIf TrackBar2.Value > 4 And TrackBar2.Value <= 5 Then
                    SerialPort1.Write("6")
                ElseIf TrackBar2.Value > 5 And TrackBar2.Value <= 6 Then
                    SerialPort1.Write("7")
                ElseIf TrackBar2.Value > 6 And TrackBar2.Value <= 7 Then
                    SerialPort1.Write("8")
                ElseIf TrackBar2.Value > 7 Then
                    SerialPort1.Write("9")
                End If

                TrackBar2.Enabled = False
                led = False
            Else
                PictureBox23.BackgroundImage = My.Resources.light_on
                PictureBox23.BackColor = Color.DodgerBlue
                Button106.BackgroundImage = My.Resources._on

                'impostazione luce bottone spegni
                If Not SerialPort1.IsOpen Then SerialPort1.Open()
                SerialPort1.Write("7")
                SerialPort1.Write("0")
                TrackBar2.Enabled = True
                led = True
            End If
            SerialPort1.Close()
        End If
    End Sub
    'accene/spegne dc motor
    Private Sub Button107_Click(sender As Object, e As EventArgs) Handles Button107.Click
        If CONNETTI = True Then
            If dcmotor = True Then
                PictureBox25.Image = My.Resources.rewind
                PictureBox25.BackColor = Color.FromArgb(27, 27, 27)
                Button107.BackgroundImage = My.Resources.off
                If Not SerialPort1.IsOpen Then SerialPort1.Open()
                SerialPort1.Write("8")
                If TrackBar4.Value = 0 And TrackBar4.Value < 1 Then
                    SerialPort1.Write("1")
                ElseIf TrackBar4.Value > 0 And TrackBar4.Value <= 1 Then
                    SerialPort1.Write("2")
                ElseIf TrackBar4.Value > 1 And TrackBar4.Value <= 2 Then
                    SerialPort1.Write("3")
                ElseIf TrackBar4.Value > 2 And TrackBar4.Value <= 3 Then
                    SerialPort1.Write("4")
                ElseIf TrackBar4.Value > 3 Then
                    SerialPort1.Write("5")
                End If
                TrackBar4.Enabled = False
                dcmotor = False
            Else
                PictureBox25.Image = My.Resources.rewindgif
                PictureBox25.BackColor = Color.DodgerBlue
                Button107.BackgroundImage = My.Resources._on

                'impostazione DC motor bottone ferma
                If Not SerialPort1.IsOpen Then SerialPort1.Open()
                SerialPort1.Write("8")
                SerialPort1.Write("0")
                TrackBar4.Enabled = True
                dcmotor = True
            End If
            SerialPort1.Close()
        End If
    End Sub
    ' BARRA REGOLAZIONE LUCE
    Private Sub TrackBar2_Scroll(sender As Object, e As EventArgs) Handles TrackBar2.Scroll
        If led = True Then
            If Not SerialPort1.IsOpen Then SerialPort1.Open()
            SerialPort1.Write("7")
            If TrackBar2.Value = 0 And TrackBar2.Value < 1 Then
                SerialPort1.Write("1")
            ElseIf TrackBar2.Value > 0 And TrackBar2.Value <= 1 Then
                SerialPort1.Write("2")
            ElseIf TrackBar2.Value > 1 And TrackBar2.Value <= 2 Then
                SerialPort1.Write("3")
            ElseIf TrackBar2.Value > 2 And TrackBar2.Value <= 3 Then
                SerialPort1.Write("4")
            ElseIf TrackBar2.Value > 3 And TrackBar2.Value <= 4 Then
                SerialPort1.Write("5")
            ElseIf TrackBar2.Value > 4 And TrackBar2.Value <= 5 Then
                SerialPort1.Write("6")
            ElseIf TrackBar2.Value > 5 And TrackBar2.Value <= 6 Then
                SerialPort1.Write("7")
            ElseIf TrackBar2.Value > 6 And TrackBar2.Value <= 7 Then
                SerialPort1.Write("8")
            ElseIf TrackBar2.Value > 7 Then
                SerialPort1.Write("9")
            End If
        End If
    End Sub
    ' BARRA REGOLAZIONE MOTORE DC
    Private Sub TrackBar4_Scroll(sender As Object, e As EventArgs) Handles TrackBar4.Scroll
        If dcmotor = True Then
            If Not SerialPort1.IsOpen Then SerialPort1.Open()
            SerialPort1.Write("8")
            If TrackBar4.Value = 0 And TrackBar4.Value < 1 Then
                SerialPort1.Write("1")
            ElseIf TrackBar4.Value > 0 And TrackBar4.Value <= 1 Then
                SerialPort1.Write("2")
            ElseIf TrackBar4.Value > 1 And TrackBar4.Value <= 2 Then
                SerialPort1.Write("3")
            ElseIf TrackBar4.Value > 2 And TrackBar4.Value <= 3 Then
                SerialPort1.Write("4")
            ElseIf TrackBar4.Value > 3 Then
                SerialPort1.Write("5")
            End If
        End If
    End Sub
#End Region

#Region "SETUP ARDUINO"
    ' AGGIORNA LISTA PORTE COM
    Private Sub Button105_Click(sender As Object, e As EventArgs) Handles Button105.Click
        ComboBox1.Items.Clear()
        For Each sp As String In My.Computer.Ports.SerialPortNames
            ComboBox1.Items.Add(sp)
            ComboBox1.DroppedDown = True
        Next
    End Sub
    'apre/chiude setup arduino
    Private Sub CheckBox3_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox3.CheckedChanged
        If CheckBox3.Checked = True Then
            If RadioButton3.Checked = True Then
                Me.Size = New Size(originalx, originaly)
            End If
            If RadioButton2.Checked = True Then
                Panello_elabora.Visible = False
                Panello_file.Visible = False
                Panello_sorgente.Visible = False
            End If
            If RadioButton3.Checked = True Then
                Panello_sketch.Size = New Size(originalx - Panello_comandi.Width - Panello_arduino.Width, originaly - Panello_sorgente.Height - Panello_chiudi.Height)
            Else
                Panello_sketch.Size = New Size(Panello_chiudi.Width - Panello_comandi.Width - Panello_arduino.Width, Panello_comandi.Height - Panello_sorgente.Height)

            End If
            Panello_sketch.Visible = True
            Loadarset()
        Else
            Panello_sketch.Visible = False
            If RadioButton3.Checked = True Then
                Me.Size = New Size(Panello_arduino.Width + Panello_comandi.Width, originaly)
            End If
            If RadioButton2.Checked = True Then
                Panello_elabora.Visible = True
                Panello_file.Visible = True
                Panello_sorgente.Visible = True
            End If
        End If
    End Sub
    'RICARICA SALVATAGGIO
    Private Sub Button94_Click(sender As Object, e As EventArgs) Handles Button94.Click
        Loadarset()
    End Sub
    'APERTURA PROGRAMMA
    Private Sub Loadarset()
        TextBox30.Text = My.Settings.M2_SN8
        TextBox21.Text = My.Settings.AR_LED
        TextBox31.Text = My.Settings.M2_PAUSE
        TextBox32.Text = My.Settings.M2_SGM
        TextBox33.Text = My.Settings.M2_SS8
        TextBox19.Text = My.Settings.AR_M2_1
        TextBox24.Text = My.Settings.AR_M2_3
        TextBox22.Text = My.Settings.AR_M2_2
        TextBox29.Text = My.Settings.M1_SGB
        TextBox27.Text = My.Settings.M1_SPEED
        TextBox28.Text = My.Settings.M1_SGM
        TextBox26.Text = My.Settings.M1_DINFU
        TextBox23.Text = My.Settings.AR_M1_3
        TextBox25.Text = My.Settings.AR_M1_4
        TextBox18.Text = My.Settings.AR_M1_1
        TextBox20.Text = My.Settings.AR_M1_2
        TextBox17.Text = My.Settings.AR_DC
        If My.Settings.M1_DIR <> 0 Then
            CheckBox5.Checked = True
        Else
            CheckBox5.Checked = False
        End If
        If My.Settings.M2_DIR <> 0 Then
            CheckBox20.Checked = True
        Else
            CheckBox20.Checked = False
        End If
    End Sub
    'BOTTONE SALVA E CHIUDI
    Private Sub Button43_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button43.Click
        My.Settings.M2_SN8 = TextBox30.Text
        My.Settings.AR_LED = TextBox21.Text
        My.Settings.M2_PAUSE = TextBox31.Text
        My.Settings.M2_SGM = TextBox32.Text
        My.Settings.M2_SS8 = TextBox33.Text
        My.Settings.AR_M2_1 = TextBox19.Text
        My.Settings.AR_M2_3 = TextBox24.Text
        My.Settings.AR_M2_2 = TextBox22.Text
        My.Settings.M1_SGB = TextBox29.Text
        My.Settings.M1_SPEED = TextBox27.Text
        My.Settings.M1_SGM = TextBox28.Text
        My.Settings.M1_DINFU = TextBox26.Text
        My.Settings.AR_M1_3 = TextBox23.Text
        My.Settings.AR_M1_4 = TextBox25.Text
        My.Settings.AR_M1_1 = TextBox18.Text
        My.Settings.AR_M1_2 = TextBox20.Text
        My.Settings.AR_DC = TextBox17.Text
        If CheckBox5.Checked = True Then
            My.Settings.M1_DIR = 1
        Else
            My.Settings.M1_DIR = 0
        End If
        If CheckBox20.Checked = True Then
            My.Settings.M2_DIR = 1
        Else
            My.Settings.M2_DIR = 0
        End If
        ' CheckBox3.Checked = False
    End Sub
    'ESCI SENZA SALVARE MODIFICHE
    Private Sub Button44_Click(sender As Object, e As EventArgs) Handles Button44.Click
        CheckBox3.Checked = False
    End Sub
    'BOTTONE CREA SKETCH ARDUINO
    Private Sub Button40_Click(sender As Object, e As EventArgs) Handles Button40.Click
        Dim index As Integer = 1
        Dim num As Integer = 100
        ' Button75.Enabled = True
        Button15.Enabled = True
        ' DATI PIN MOTORI CORONA
        Dim PINM1_1 = My.Settings.AR_M2_3
        Dim PINM1_2 = My.Settings.AR_M2_2
        Dim PINM1_3 = My.Settings.AR_M2_1
        ' Dim PINM1_4 = My.Settings.SaveTitle10
        ' DATI PIN MOTORI BOBINA
        Dim PINM2_1 = My.Settings.AR_M1_1
        Dim PINM2_2 = My.Settings.AR_M1_2
        Dim PINM2_3 = My.Settings.AR_M1_3
        Dim PINM2_4 = My.Settings.AR_M1_4
        ' DATI PIN PWM LED ARDUINO
        Dim PINLED = My.Settings.AR_LED
        ' DATI PIN PWM MOTORE DC ARDUINO
        Dim PINMOT = My.Settings.AR_DC
        ' DATI MOTORE CORONA
        Dim SGM1 = My.Settings.M2_SGM   ' N. STEP PER GIRO
        Dim SFS8 = My.Settings.M2_SS8  ' STEP / FOTOGRAMMA SUPER 8
        Dim SFN8 = My.Settings.M2_SN8    ' STEP / FOTOGRAMMA NORMAL 8
        Dim MSM1 = 1                ' STEP / MICROSTEP AVANTI - INDIETRO
        Dim SPM1 = My.Settings.M2_PAUSE    ' VELOCITA' MOTORE CORONA GIRI/MINUTO
        ' DATI MOTORE BOBINA
        Dim SGM2 = My.Settings.M1_SGM  ' N. STEP PER GIRO MOTORE DEFAULT
        Dim DIMF = My.Settings.M1_DINFU   ' DIAMTRO FULCRO
        Dim MSM2 = 2                ' STEP / MICROSTEP AVANTI - INDIETRO
        Dim SEM2 = My.Settings.M1_SGB   ' STEP / GIRO BOBINA
        Dim SPM2 = My.Settings.M1_SPEED   ' VELOCITA' MOTORE BOBINA GIRI/MINUTO
        ' DATI VARI E CALCOLI MOTORE BOBINA
        Dim C = 0.16                ' SPESSORE PELLICOLA MM 
        Dim S8 = 4.01               ' ALTEZZA FOTOGRAMMA SUPER 8 MM (4.01) 
        Dim N8 = 3.3                ' ALTEZZA FOTOGRAMMA NORMAL 8 MM (3.3)
        Dim M = 0                   ' NUMERO DI GIRO INIZIO
        Dim F = DIMF                ' DIAMETRO AL NUMERO DI GIRO INIZIALE
        Dim GS8 = (F * PI) / S8     ' FOTO AL NUMERO DI GIRO SUPER 8
        Dim GN8 = (F * PI) / N8     ' FOTO AL NUMERO DI GIRO NORMAL 8
        Dim HS8 = SEM2 / GS8           ' STEP X FOTO AL NUMERO DI GIRO SUPER 8
        Dim IS8 = GS8               ' FOTO TOTALI AL NUMERO DI GIRO SUPER 8
        Dim HN8 = SEM2 / GN8           ' STEP X FOTO AL NUMERO DI GIRO NORMAL 8
        Dim IN8 = GN8               ' FOTO TOTALI AL NUMERO DI GIRO NORMAL 8
        Dim LS8 = HS8 * IS8         ' STEP TOTALI AL NUMERO DI GIRO SUPER 8    
        Dim JS8 = HS8
        Dim LN8 = HN8 * IN8         ' STEP TOTALI AL NUMERO DI GIRO NORMAL 8    
        Dim JN8 = HN8
        Dim HHS8 = HS8
        Dim HHN8 = HN8
        Dim Stp = 1
        Dim DIR1D
        Dim DIR1S
        Dim DIR2D
        Dim DIR2S

        If My.Settings.M1_DIR <> 0 Then
            DIR1D = "-"
            DIR1S = ""
        Else
            DIR1D = ""
            DIR1S = "-"
        End If
        If My.Settings.M2_DIR <> 0 Then
            DIR2D = "HIGH"
            DIR2S = "LOW"
        Else
            DIR2D = "LOW"
            DIR2S = "HIGH"
        End If

        RichTextBox2.Text = ("// SKETCH ARDUINO For TELECINE FRAME BY Step")
        RichTextBox2.Text += vbCrLf & ("// CONTROL 2 Step MOTOR, 1 LED, 1 DC MOTOR ")
        RichTextBox2.Text += vbCrLf & ("// SANDUINO SOFTWARE '2018'") & vbCrLf
        RichTextBox2.Text += vbCrLf & (("#include <Stepper.h>"))
        RichTextBox2.Text += vbCrLf & ("Stepper myStepper1 (" & CInt(SGM2) & "," & CInt(PINM2_1) & "," & CInt(PINM2_3) & "," & CInt(PINM2_2) & "," & CInt(PINM2_4) & ");//MOTORE BOBINA")
        'RichTextBox2.Text += vbCrLf & ("Stepper myStepper2 (" & CInt(SGM1) & "," & CInt(PINM1_1) & "," & CInt(PINM1_3) & "," & CInt(PINM1_2) & "," & CInt(PINM1_4) & ");//MOTORE CORONA")
        RichTextBox2.Text += vbCrLf & ("int motor1Pin1 = " & CInt(PINM1_1) & ";")
        RichTextBox2.Text += vbCrLf & ("int motor1Pin2 = " & CInt(PINM1_2) & ";")
        RichTextBox2.Text += vbCrLf & ("int motor1Pin3 = " & CInt(PINM1_3) & ";")
        'RichTextBox2.Text += vbCrLf & ("int motor1Pin4 = " & CInt(PINM1_4) & ";")
        RichTextBox2.Text += vbCrLf & ("int motor2Pin1 = " & CInt(PINM2_1) & ";")
        RichTextBox2.Text += vbCrLf & ("int motor2Pin2 = " & CInt(PINM2_2) & ";")
        RichTextBox2.Text += vbCrLf & ("int motor2Pin3 = " & CInt(PINM2_3) & ";")
        RichTextBox2.Text += vbCrLf & ("int motor2Pin4 = " & CInt(PINM2_4) & ";")
        RichTextBox2.Text += vbCrLf & (("int led = " & CInt(PINLED) & "; // PIN PWM LED ILLUMINAZIONE") & vbCrLf & ("int brillo = 0;") & vbCrLf & ("int recibido = 0;"))
        RichTextBox2.Text += vbCrLf & (("int mot = " & CInt(PINMOT) & "; // PIN PWM MOTORE DC RIAVVOLGI") & vbCrLf & ("int vel = 0;") & vbCrLf & ("int rec = 0;")) & vbCrLf
        RichTextBox2.Text += vbCrLf & ("void setup() {")
        RichTextBox2.Text += vbCrLf & ("myStepper1.setSpeed(" & CInt(SPM2) & ");")
        'RichTextBox2.Text += vbCrLf & ("myStepper2.setSpeed(" & CInt(SPM1) & ");")
        RichTextBox2.Text += vbCrLf & ("pinMode(motor1Pin1, OUTPUT);")
        RichTextBox2.Text += vbCrLf & ("pinMode(motor1Pin2, OUTPUT);")
        RichTextBox2.Text += vbCrLf & ("pinMode(motor1Pin3, OUTPUT);")
        'RichTextBox2.Text += vbCrLf & ("digitalWrite(motor1Pin3,HIGH);")
        'RichTextBox2.Text += vbCrLf & ("pinMode(motor1Pin4, OUTPUT);")
        RichTextBox2.Text += vbCrLf & ("pinMode(motor2Pin1, OUTPUT);")
        RichTextBox2.Text += vbCrLf & ("pinMode(motor2Pin2, OUTPUT);")
        RichTextBox2.Text += vbCrLf & ("pinMode(motor2Pin3, OUTPUT);")
        RichTextBox2.Text += vbCrLf & ("pinMode(motor2Pin4, OUTPUT);")
        RichTextBox2.Text += vbCrLf & ("pinMode(led, OUTPUT);")
        RichTextBox2.Text += vbCrLf & ("pinMode(mot, OUTPUT);")
        RichTextBox2.Text += vbCrLf & ("Serial.begin(9600);") & vbCrLf & ("}")
        RichTextBox2.Text += vbCrLf & ("void loop() {")
        RichTextBox2.Text += vbCrLf & ("int pos;") & vbCrLf & ("int val;") & vbCrLf & ("int stp;") & vbCrLf & ("int x;") & vbCrLf & ("int dir;")
        RichTextBox2.Text += vbCrLf & ("String n_foto = """";")
        RichTextBox2.Text += vbCrLf & ("String n_dir = """";")
        RichTextBox2.Text += vbCrLf & ("int ok;")
        RichTextBox2.Text += vbCrLf & ("int d;")
        RichTextBox2.Text += vbCrLf & ("if (Serial.available())  {") & vbCrLf & ("delay(200);")
        RichTextBox2.Text += vbCrLf & ("while (Serial.available() > 0) {") & vbCrLf & ("pos = Serial.read();")
        RichTextBox2.Text += vbCrLf & ("if (pos == '0') {// TASTO AVVIO Super 8 mm")
        RichTextBox2.Text += vbCrLf & ("while (Serial.available() > 0) {") & vbCrLf & ("val = Serial.read();")
        RichTextBox2.Text += vbCrLf & ("n_foto += val -  '0';") & vbCrLf & ("x = n_foto.toInt();")
        RichTextBox2.Text += vbCrLf & (("stp=") & (CInt(HS8)) & (";") & vbCrLf & ("if (x >= 0 && x < ") & (CInt(IS8)) & (") {stp = stp;}") & vbCrLf & ("if (x >= ") & CInt(IS8) & (" && x < "))
        M += 1
        F = DIMF + (C * 2 * M)
        GS8 = (F * PI) / S8
        HS8 = SEM2 / GS8
        IS8 += GS8
        LS8 = HS8 * IS8

        Do While index <= 200
            index += 1
            If CInt(HS8) = Decimal.Truncate(JS8) Then
                JS8 -= 1
                RichTextBox2.Text += CInt(IS8) & (") {stp -=") & (Stp) & (";}") & vbCrLf & ("if (x >= ") & CInt(IS8) & (" && x < ")
                Stp += 1
            End If
            M += 1
            F = DIMF + (C * 2 * M)
            GS8 = (F * PI) / S8
            HS8 = SEM2 / GS8
            IS8 += GS8
            LS8 = HS8 * IS8
        Loop
        RichTextBox2.Lines = RichTextBox2.Lines.Take(RichTextBox2.Lines.Count - 1).ToArray
        RichTextBox2.Text += vbCrLf & ("}")

        RichTextBox2.Text += vbCrLf & ("digitalWrite(motor1Pin1," & DIR2D & ");")
        RichTextBox2.Text += vbCrLf & ("digitalWrite(motor1Pin3,LOW);")
        RichTextBox2.Text += vbCrLf & ("for(int x = 0; x < " & CInt(SFS8) & "; x++) {")
        RichTextBox2.Text += vbCrLf & ("digitalWrite(motor1Pin2,HIGH);")
        RichTextBox2.Text += vbCrLf & ("delayMicroseconds(" & SPM1 & ");")
        RichTextBox2.Text += vbCrLf & ("digitalWrite(motor1Pin2,LOW);")
        RichTextBox2.Text += vbCrLf & ("delayMicroseconds(" & SPM1 & ");")
        RichTextBox2.Text += vbCrLf & ("}")
        RichTextBox2.Text += vbCrLf & ("digitalWrite(motor1Pin3,HIGH);")



        'RichTextBox2.Text += vbCrLf & ("myStepper2.step(" & DIR2D & CInt(SFS8) & ");")
        RichTextBox2.Text += vbCrLf & ("myStepper1.step(" & DIR1D & "stp);")
        RichTextBox2.Text += vbCrLf & ("Serial.println(x);") & vbCrLf & ("}")
        RichTextBox2.Text += vbCrLf & ("if (pos == '1') {// TASTO AVVIO 8 mm")
        RichTextBox2.Text += vbCrLf & ("while (Serial.available() > 0) {") & vbCrLf & ("val = Serial.read();")
        RichTextBox2.Text += vbCrLf & ("n_foto += val -  '0';") & vbCrLf & ("x = n_foto.toInt();")
        DIMF = My.Settings.M1_DINFU
        M = 0                   ' NUMERO DI GIRO INIZIALE
        F = DIMF                ' DIAMETRO AL NUMERO DI GIRO INIZIALE
        index = 1
        Stp = 1
        RichTextBox2.Text += vbCrLf & (("stp=") & (CInt(HN8)) & (";") & vbCrLf & ("if (x >= 0 && x < ") & (CInt(IN8)) & (") {stp = stp;}") & vbCrLf & ("if (x >= ") & CInt(IN8) & (" && x < "))
        M += 1
        F = DIMF + (C * 2 * M)
        GN8 = (F * PI) / N8
        HN8 = SEM2 / GN8
        IN8 += GN8
        LN8 = HN8 * IN8
        Do While index <= 200
            index += 1
            If CInt(HN8) = Decimal.Truncate(JN8) Then
                JN8 -= 1
                RichTextBox2.Text += CInt(IN8) & (") {stp -=") & (Stp) & (";}") & vbCrLf & ("if (x >= ") & CInt(IN8) & (" && x < ")
                Stp += 1
            End If
            M += 1
            F = DIMF + (C * 2 * M)
            GN8 = (F * PI) / N8
            HN8 = SEM2 / GN8
            IN8 += GN8
            LN8 = HN8 * IN8
        Loop
        RichTextBox2.Lines = RichTextBox2.Lines.Take(RichTextBox2.Lines.Count - 1).ToArray
        RichTextBox2.Text += vbCrLf & ("}")


        RichTextBox2.Text += vbCrLf & ("digitalWrite(motor1Pin1," & DIR2D & ");")
        RichTextBox2.Text += vbCrLf & ("digitalWrite(motor1Pin3,LOW);")
        RichTextBox2.Text += vbCrLf & ("for(int x = 0; x < " & CInt(SFN8) & "; x++) {")
        RichTextBox2.Text += vbCrLf & ("digitalWrite(motor1Pin2,HIGH);")
        RichTextBox2.Text += vbCrLf & ("delayMicroseconds(" & SPM1 & ");")
        RichTextBox2.Text += vbCrLf & ("digitalWrite(motor1Pin2,LOW);")
        RichTextBox2.Text += vbCrLf & ("delayMicroseconds(" & SPM1 & ");")
        RichTextBox2.Text += vbCrLf & ("}")
        RichTextBox2.Text += vbCrLf & ("digitalWrite(motor1Pin3,HIGH);")


        RichTextBox2.Text += vbCrLf & ("myStepper1.step(" & DIR1D & "stp);")

        'RichTextBox2.Text += vbCrLf & ("myStepper2.step(" & DIR2D & CInt(SFN8) & ");") & vbCrLf & ("myStepper1.step(" & DIR1D & "stp);")
        RichTextBox2.Text += vbCrLf & ("Serial.println(x);") & vbCrLf & ("}")
        RichTextBox2.Text += vbCrLf & ("if (pos == '2') {// TASTO IDIETRO 1 FOTOGRAMMA")
        RichTextBox2.Text += vbCrLf & ("while (Serial.available() > 0) {") & vbCrLf & ("dir = Serial.read();")
        RichTextBox2.Text += vbCrLf & ("n_dir += dir -  '0';") & vbCrLf & ("d = n_dir.toInt();") & vbCrLf & ("}")
        RichTextBox2.Text += vbCrLf & ("if (d == 0) {// IDIETRO 1 FOTOGRAMMA SUPER 8")
        RichTextBox2.Text += vbCrLf & ("myStepper1.step(" & DIR1S & CInt(HHS8) & ");") ' & vbCrLf & ("myStepper2.step(" & DIR2S & CInt(SFS8) & ");") & vbCrLf & ("}")

        RichTextBox2.Text += vbCrLf & ("digitalWrite(motor1Pin1," & DIR2S & ");")
        RichTextBox2.Text += vbCrLf & ("digitalWrite(motor1Pin3,LOW);")
        RichTextBox2.Text += vbCrLf & ("for(int x = 0; x < " & CInt(SFS8) & "; x++) {")
        RichTextBox2.Text += vbCrLf & ("digitalWrite(motor1Pin2,HIGH);")
        RichTextBox2.Text += vbCrLf & ("delayMicroseconds(" & SPM1 & ");")
        RichTextBox2.Text += vbCrLf & ("digitalWrite(motor1Pin2,LOW);")
        RichTextBox2.Text += vbCrLf & ("delayMicroseconds(" & SPM1 & ");")
        RichTextBox2.Text += vbCrLf & ("}")
        RichTextBox2.Text += vbCrLf & ("digitalWrite(motor1Pin3,HIGH);") & vbCrLf & ("}")


        RichTextBox2.Text += vbCrLf & ("else if (d == 1) {// IDIETRO 1 FOTOGRAMMA NORMAL 8")
        RichTextBox2.Text += vbCrLf & ("myStepper1.step(" & DIR1S & CInt(HHN8) & ");") ' & vbCrLf & ("myStepper2.step(" & DIR2S & CInt(SFN8) & ");") & vbCrLf & ("}")

        RichTextBox2.Text += vbCrLf & ("digitalWrite(motor1Pin1," & DIR2S & ");")
        RichTextBox2.Text += vbCrLf & ("digitalWrite(motor1Pin3,LOW);")
        RichTextBox2.Text += vbCrLf & ("for(int x = 0; x < " & CInt(SFN8) & "; x++) {")
        RichTextBox2.Text += vbCrLf & ("digitalWrite(motor1Pin2,HIGH);")
        RichTextBox2.Text += vbCrLf & ("delayMicroseconds(" & SPM1 & ");")
        RichTextBox2.Text += vbCrLf & ("digitalWrite(motor1Pin2,LOW);")
        RichTextBox2.Text += vbCrLf & ("delayMicroseconds(" & SPM1 & ");")
        RichTextBox2.Text += vbCrLf & ("}")
        RichTextBox2.Text += vbCrLf & ("digitalWrite(motor1Pin3,HIGH);") & vbCrLf & ("}")


        RichTextBox2.Text += vbCrLf & ("}")
        RichTextBox2.Text += vbCrLf & ("if (pos == '3') {// TASTO AVANTI 1 FOTOGRAMMA")
        RichTextBox2.Text += vbCrLf & ("while (Serial.available() > 0) {") & vbCrLf & ("dir = Serial.read();")
        RichTextBox2.Text += vbCrLf & ("n_dir += dir -  '0';") & vbCrLf & ("d = n_dir.toInt();") & vbCrLf & ("}")
        RichTextBox2.Text += vbCrLf & ("if (d == 0) {// AVANTI 1 FOTOGRAMMA SUPER 8")


        RichTextBox2.Text += vbCrLf & ("digitalWrite(motor1Pin1," & DIR2D & ");")
        RichTextBox2.Text += vbCrLf & ("digitalWrite(motor1Pin3,LOW);")
        RichTextBox2.Text += vbCrLf & ("for(int x = 0; x < " & CInt(SFS8) & "; x++) {")
        RichTextBox2.Text += vbCrLf & ("digitalWrite(motor1Pin2,HIGH);")
        RichTextBox2.Text += vbCrLf & ("delayMicroseconds(" & SPM1 & ");")
        RichTextBox2.Text += vbCrLf & ("digitalWrite(motor1Pin2,LOW);")
        RichTextBox2.Text += vbCrLf & ("delayMicroseconds(" & SPM1 & ");")
        RichTextBox2.Text += vbCrLf & ("}")
        RichTextBox2.Text += vbCrLf & ("digitalWrite(motor1Pin3,HIGH);")




        RichTextBox2.Text += vbCrLf & ("myStepper1.step(" & DIR1D & CInt(HHS8) & ");") & vbCrLf & ("}")
        'RichTextBox2.Text += vbCrLf & ("myStepper2.step(" & DIR2D & CInt(SFS8) & ");") & vbCrLf & ("myStepper1.step(" & DIR1D & CInt(HHS8) & ");") & vbCrLf & ("}")


        RichTextBox2.Text += vbCrLf & ("else if (d == 1) {// AVANTI 1 FOTOGRAMMA NORMAL 8")


        RichTextBox2.Text += vbCrLf & ("digitalWrite(motor1Pin1," & DIR2D & ");")
        RichTextBox2.Text += vbCrLf & ("digitalWrite(motor1Pin3,LOW);")
        RichTextBox2.Text += vbCrLf & ("for(int x = 0; x < " & CInt(SFN8) & "; x++) {")
        RichTextBox2.Text += vbCrLf & ("digitalWrite(motor1Pin2,HIGH);")
        RichTextBox2.Text += vbCrLf & ("delayMicroseconds(" & SPM1 & ");")
        RichTextBox2.Text += vbCrLf & ("digitalWrite(motor1Pin2,LOW);")
        RichTextBox2.Text += vbCrLf & ("delayMicroseconds(" & SPM1 & ");")
        RichTextBox2.Text += vbCrLf & ("}")
        RichTextBox2.Text += vbCrLf & ("digitalWrite(motor1Pin3,HIGH);")



        RichTextBox2.Text += vbCrLf & ("myStepper1.step(" & DIR1D & CInt(HHN8) & ");") & vbCrLf & ("}")

        'RichTextBox2.Text += vbCrLf & ("myStepper2.step(" & DIR2D & CInt(SFN8) & ");") & vbCrLf & ("myStepper1.step(" & DIR1D & CInt(HHN8) & ");") & vbCrLf & ("}")
        RichTextBox2.Text += vbCrLf & ("}")
        RichTextBox2.Text += vbCrLf & ("if (pos == '4') {// TASTO AVANTI 1 MICROSTEP")

        RichTextBox2.Text += vbCrLf & ("digitalWrite(motor1Pin1," & DIR2D & ");")
        RichTextBox2.Text += vbCrLf & ("digitalWrite(motor1Pin3,LOW);")
        RichTextBox2.Text += vbCrLf & ("for(int x = 0; x < " & CInt(MSM1) & "; x++) {")
        RichTextBox2.Text += vbCrLf & ("digitalWrite(motor1Pin2,HIGH);")
        RichTextBox2.Text += vbCrLf & ("delayMicroseconds(" & SPM1 & ");")
        RichTextBox2.Text += vbCrLf & ("digitalWrite(motor1Pin2,LOW);")
        RichTextBox2.Text += vbCrLf & ("delayMicroseconds(" & SPM1 & ");")
        RichTextBox2.Text += vbCrLf & ("}")
        RichTextBox2.Text += vbCrLf & ("delay(100);")
        RichTextBox2.Text += vbCrLf & ("digitalWrite(motor1Pin3,HIGH);")

        RichTextBox2.Text += vbCrLf & ("myStepper1.step(" & DIR1D & CInt(MSM2) & ");") & vbCrLf & ("}")

        ' RichTextBox2.Text += vbCrLf & ("myStepper2.step(" & DIR2D & CInt(MSM1) & ");") & vbCrLf & ("myStepper1.step(" & DIR1D & CInt(MSM2) & ");") & vbCrLf & ("}")

        RichTextBox2.Text += vbCrLf & ("if (pos == '5') {// TASTO IDIETRO 1 MICROSTEP")

        RichTextBox2.Text += vbCrLf & ("myStepper1.step(" & DIR1S & CInt(MSM2) & ");") ' & vbCrLf & ("myStepper2.step(" & DIR2S & CInt(MSM1) & ");") & vbCrLf & ("}")

        RichTextBox2.Text += vbCrLf & ("digitalWrite(motor1Pin1," & DIR2S & ");")
        RichTextBox2.Text += vbCrLf & ("digitalWrite(motor1Pin3,LOW);")
        RichTextBox2.Text += vbCrLf & ("for(int x = 0; x < " & CInt(MSM1) & "; x++) {")
        RichTextBox2.Text += vbCrLf & ("digitalWrite(motor1Pin2,HIGH);")
        RichTextBox2.Text += vbCrLf & ("delayMicroseconds(" & SPM1 & ");")
        RichTextBox2.Text += vbCrLf & ("digitalWrite(motor1Pin2,LOW);")
        RichTextBox2.Text += vbCrLf & ("delayMicroseconds(" & SPM1 & ");")
        RichTextBox2.Text += vbCrLf & ("}")
        RichTextBox2.Text += vbCrLf & ("delay(100);")
        RichTextBox2.Text += vbCrLf & ("digitalWrite(motor1Pin3,HIGH);") & vbCrLf & ("}")



        RichTextBox2.Text += vbCrLf & ("if (pos == '6') {// SPEGNE I MOTORI")
        'RichTextBox2.Text += vbCrLf & ("digitalWrite(motor1Pin1, LOW);")
        'RichTextBox2.Text += vbCrLf & ("digitalWrite(motor1Pin2, LOW);")
        RichTextBox2.Text += vbCrLf & ("digitalWrite(motor1Pin3, HIGH);")
        'RichTextBox2.Text += vbCrLf & ("digitalWrite(motor1Pin4, LOW);")
        RichTextBox2.Text += vbCrLf & ("digitalWrite(motor2Pin1, LOW);")
        RichTextBox2.Text += vbCrLf & ("digitalWrite(motor2Pin2, LOW);")
        RichTextBox2.Text += vbCrLf & ("digitalWrite(motor2Pin3, LOW);")
        RichTextBox2.Text += vbCrLf & ("digitalWrite(motor2Pin4, LOW);")
        RichTextBox2.Text += vbCrLf & ("delay(100);") & vbCrLf & ("}")
        RichTextBox2.Text += vbCrLf & ("if (pos == '7') {// IMPOSTA LUCE LED")
        RichTextBox2.Text += vbCrLf & ("while (Serial.available() > 0) {")
        RichTextBox2.Text += vbCrLf & ("recibido = Serial.read();")
        RichTextBox2.Text += vbCrLf & ("switch (recibido)") & vbCrLf & ("{")
        RichTextBox2.Text += vbCrLf & ("case '0': brillo = 0; break;")
        RichTextBox2.Text += vbCrLf & ("case '1': brillo = 2; break;")
        RichTextBox2.Text += vbCrLf & ("case '2': brillo = 4; break;")
        RichTextBox2.Text += vbCrLf & ("case '3': brillo = 8; break;")
        RichTextBox2.Text += vbCrLf & ("case '4': brillo = 16; break;")
        RichTextBox2.Text += vbCrLf & ("case '5': brillo = 24; break;")
        RichTextBox2.Text += vbCrLf & ("case '6': brillo = 48; break;")
        RichTextBox2.Text += vbCrLf & ("case '7': brillo = 96; break;")
        RichTextBox2.Text += vbCrLf & ("case '8': brillo = 192; break;")
        RichTextBox2.Text += vbCrLf & ("case '9': brillo = 250; break;") & vbCrLf & ("}")
        RichTextBox2.Text += vbCrLf & ("analogWrite(led, brillo);") & vbCrLf & ("}") & vbCrLf & ("}")
        RichTextBox2.Text += vbCrLf & ("if (pos == '8') {// IMPOSTA MOTORE DC RIAVVOLGI")
        RichTextBox2.Text += vbCrLf & ("while (Serial.available() > 0) {")
        RichTextBox2.Text += vbCrLf & ("rec = Serial.read();")
        RichTextBox2.Text += vbCrLf & ("switch (rec)") & vbCrLf & ("{")
        RichTextBox2.Text += vbCrLf & ("case '0': vel = 0; break;")
        RichTextBox2.Text += vbCrLf & ("case '1': vel = 150; break;")
        RichTextBox2.Text += vbCrLf & ("case '2': vel = 200; break;")
        RichTextBox2.Text += vbCrLf & ("case '3': vel = 250; break;") & vbCrLf & ("}")
        RichTextBox2.Text += vbCrLf & ("analogWrite(mot, vel);") & vbCrLf & ("}") & vbCrLf & ("}")
        RichTextBox2.Text += vbCrLf & ("if (pos == '9') {// TEST CONNESSIONE")
        RichTextBox2.Text += vbCrLf & ("ok = 5;")
        RichTextBox2.Text += vbCrLf & ("Serial.println(ok);") & vbCrLf & ("}")
        RichTextBox2.Text += vbCrLf & ("else {") & vbCrLf & ("delay(100);")
        RichTextBox2.Text += vbCrLf & ("}") & vbCrLf & ("}") & vbCrLf & ("}") & vbCrLf & ("}")

    End Sub
    'SELEZIONA TUTTO E COPIA SKETCH GENERATO 
    Private Sub Button41_Click(sender As Object, e As EventArgs) Handles Button41.Click
        RichTextBox2.SelectAll()
        RichTextBox2.Copy()
    End Sub
    'RESET PULISCI SKETCH GENERATO 
    Private Sub Button42_Click(sender As Object, e As EventArgs) Handles Button42.Click
        RichTextBox2.Clear()
        My.Computer.Clipboard.Clear()
        ' Button75.Enabled = False
        Button45.Enabled = False
    End Sub
    'SELEZIONA E AVVIO SOFTWARE DI ARDUINO 
    Private Sub Button39_MouseDown(sender As Object, e As MouseEventArgs) Handles Button39.MouseDown
        If e.Button = Windows.Forms.MouseButtons.Right Then
            If MsgBox("Cambiare Software Esterno di Arduino?", MsgBoxStyle.YesNo, ) = MsgBoxResult.Yes Then
                Card = ""
                Dim folderDlg As New OpenFileDialog
                If (folderDlg.ShowDialog() = DialogResult.OK) Then
                    Card = folderDlg.FileName
                    My.Settings.cart_ardu = Card
                End If
            Else
                Exit Sub
            End If
        Else
            If Card = "" Then
                Dim folderDlg As New OpenFileDialog
                If (folderDlg.ShowDialog() = DialogResult.OK) Then
                    Card = folderDlg.FileName
                    My.Settings.cart_ardu = Card
                End If
            Else
                Process.Start(Card)
            End If
        End If


    End Sub
    'INVERTE DIREZIONE MOTORE BOBINA NELLO SKETCH
    Private Sub CheckBox5_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox5.CheckedChanged
        If CheckBox5.Checked = True Then
            CheckBox5.BackgroundImage = My.Resources.rotate_left
        Else
            CheckBox5.BackgroundImage = My.Resources.rotate_right

        End If
    End Sub
    'INVERTE DIREZIONE MOTORE CORONA NELLO SKETCH
    Private Sub CheckBox20_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox20.CheckedChanged
        If CheckBox20.Checked = True Then
            CheckBox20.BackgroundImage = My.Resources.rotate_left
        Else
            CheckBox20.BackgroundImage = My.Resources.rotate_right

        End If
    End Sub
#End Region
#End Region

#Region "CONTROLLO ARDUINO"
    'BOTTONE CONNETTI / DISCONNETTI ARDUINO
    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
        Dim online = 0
        If CONNETTI = True Then
            Try
                If Not SerialPort1.IsOpen Then SerialPort1.Open()
                Thread.Sleep(250)
                SerialPort1.Write("8")
                SerialPort1.Write("0")
                Thread.Sleep(250)
                SerialPort1.Write("7")
                SerialPort1.Write("0")
                Thread.Sleep(250)
                SerialPort1.Write("6")
                SerialPort1.Close()
                Button5.BackgroundImage = My.Resources.usb_off
                CONNETTI = False
                MsgBox("ARDUINO DISCONNESSO", MessageBoxIcon.Error)
                TableLayoutPanel2.Enabled = False
                ComboBox1.Enabled = True
                Exit Sub
            Catch ex As Exception
                CONNETTI = False
                MsgBox("ARDUINO NON CONNESSO", MessageBoxIcon.Error)
                TableLayoutPanel2.Enabled = False
                ComboBox1.Enabled = True
            End Try
        End If
        If ComboBox1.Text <> "" Then
            SerialPort1.Close()
            SerialPort1.PortName = ComboBox1.Text
            SerialPort1.BaudRate = 9600
            SerialPort1.DataBits = 8
            SerialPort1.Parity = Parity.None
            SerialPort1.StopBits = StopBits.One
            SerialPort1.Handshake = Handshake.None
            SerialPort1.Encoding = System.Text.Encoding.Default
        Else
            MsgBox("ERRORE COM !" & vbCrLf & "Seleziona Porta" & vbCrLf & "COM di Arduino" & vbCrLf & "in Setup", MessageBoxIcon.Error)
            ' PictureBox52.BackgroundImage = My.Resources.errore
            Button5.BackgroundImage = My.Resources.usb_off
            CONNETTI = False
            Exit Sub
        End If
        Try
            SerialPort1.Open()
        Catch ex As Exception
            CONNETTI = False
            MsgBox("ERRORE COM !" & vbCrLf & "Impossibile Aprire" & vbCrLf & "la Porta " & ComboBox1.Text & vbCrLf & "Reimposta e Riprova", MessageBoxIcon.Error)
            Button5.BackgroundImage = My.Resources.usb_off
            Exit Sub
        End Try
        SerialPort1.Write("9")
        Try
            online = SerialPort1.ReadLine()
        Catch ex As Exception
            CONNETTI = False
            MsgBox("ERRORE ARDUINO !" & vbCrLf & "Arduino Non Risponde" & vbCrLf & "Verifica " & ComboBox1.Text & vbCrLf & "Ricollega Arduino", MessageBoxIcon.Error)
            Button5.BackgroundImage = My.Resources.usb_off
            Exit Sub
        End Try
        If online <> 5 Then
            CONNETTI = False
            MsgBox("ERRORE SKETCH !" & vbCrLf & "Copiare in Arduino" & vbCrLf & "Lo Sketch Creato" & vbCrLf & "Dal Programma", MessageBoxIcon.Error)
            Button5.BackgroundImage = My.Resources.usb_off
            Exit Sub
        End If
        SerialPort1.Close()
        CONNETTI = True
        MsgBox("ARDUINO CONNESSO", MessageBoxIcon.Information)
        TableLayoutPanel2.Enabled = True
        ComboBox1.Enabled = False
        Button5.BackgroundImage = My.Resources.usb_on
    End Sub
    'blocca e sblocca bottoni durante cattura e movimento
    Private Sub Blocca()
        Button5.Enabled = False
        Button14.Enabled = False
        Button24.Enabled = True
        Button25.Enabled = False
        Button52.Enabled = False
        Button103.Enabled = False
        Button104.Enabled = False
        NumericUpDown1.Enabled = False
        NumericUpDown2.Enabled = False
        NumericUpDown3.Enabled = False
    End Sub
    Private Sub Sblocca()
        Button5.Enabled = True
        Button14.Enabled = True
        Button24.Enabled = False
        Button25.Enabled = True
        Button52.Enabled = True
        Button103.Enabled = True
        Button104.Enabled = True
        NumericUpDown1.Enabled = True
        NumericUpDown2.Enabled = True
        NumericUpDown3.Enabled = True
    End Sub
    'TASTO AVVIO CATTURA 
    Private Sub Button14_Click(sender As Object, e As EventArgs) Handles Button14.Click
        Dim Numero = NumericUpDown1.Value
        Dim index As Integer = 1
        Dim cont As Integer
        Dim mess1 As Integer
        Dim Secondi As Integer, Minuti As Integer, Ore As Integer
        Dim tempo As Long, mio As String


        If setclick = True Then
            'TASTO AVVIO MODO CLICK
            If TextBox3.Text = "" Or TextBox2.Text = "" Then
                MsgBox("Configurare Coordinate CLICK in Setup", MessageBoxIcon.Stop)
                Exit Sub
            End If

            Blocca()
            Label1.Text = TimeOfDay()
            My.Computer.Audio.PlaySystemSound(System.Media.SystemSounds.Asterisk)
            While index <= Numero
                Application.DoEvents()
                If ANNULLA = True Then
                    If MsgBox("Sicuro di voler Annullare?", MsgBoxStyle.YesNo Or MsgBoxStyle.Question,) = MsgBoxResult.Yes Then
                        If Not SerialPort1.IsOpen Then SerialPort1.Open()
                        SerialPort1.Write("6")
                        SerialPort1.Close()
                        Label28.Text = -Numero - 1 + index
                        MsgBox("ANNULLATO" & vbCrLf & vbCrLf & "Ora Inizio " & Label1.Text() & vbCrLf & vbCrLf & "Ora Fine " & TimeOfDay() & vbCrLf & vbCrLf & "Click OK  " & (NumericUpDown1.Value + Label28.Text), MessageBoxIcon.Stop)
                        Label28.Text = ("Click n.")
                        Label28.ForeColor = Color.White
                        Label2.Text = ""
                        If Not SerialPort1.IsOpen Then SerialPort1.Open()
                        SerialPort1.DiscardInBuffer()
                        SerialPort1.Close()
                        Label1.Text = ""
                        ANNULLA = False
                        Sblocca()
                        Exit Sub
                    Else
                        ANNULLA = False
                        Button24.Enabled = True
                    End If
                End If

                Label28.Text = ("")
                Label28.ForeColor = Color.Red
                Label28.Text = -Numero + index
                tempo = (CInt(-Label28.Text) * (CInt(TextBox4.Text) + CInt(TextBox5.Text)) / 1000)
                Secondi = tempo Mod 60
                tempo = Int(tempo / 60)
                Minuti = tempo Mod 60
                tempo = Int(tempo / 60)
                Ore = tempo
                mio = Format(Ore, "00") & ":" & Format(Minuti, "00") & ":" & Format(Secondi, "00")
                Label2.Text = mio
                index += 1
                Dim pnt As System.Drawing.Point
                pnt = New System.Drawing.Point(TextBox2.Text, TextBox3.Text)
                SimulateClick(pnt)
                Thread.Sleep(TextBox4.Text) 'attesa click
                TextBox1.Text += 1
                cont = CInt(TextBox1.Text)
                Application.DoEvents()
                If Not SerialPort1.IsOpen Then SerialPort1.Open()
                SerialPort1.Write(s8_n8)
                SerialPort1.Write(CInt(TextBox1.Text))
                Try
                    mess1 = SerialPort1.ReadLine()
                Catch ex As Exception
                    If Not SerialPort1.IsOpen Then SerialPort1.Open()
                    SerialPort1.Write("6")
                    SerialPort1.Close()
                    Label28.Text = -Numero - 1 + index
                    MsgBox("ERRORE: Nessuna Risposta" & vbCrLf & vbCrLf & "Disconnessione di Arduino" & vbCrLf & vbCrLf & "Resettare o Ricaricare lo Sketch", MessageBoxIcon.Stop)
                    MsgBox("ANNULLAMENTO AUTOMATICO" & vbCrLf & vbCrLf & "Ora Inizio " & Label1.Text() & vbCrLf & vbCrLf & "Ora Fine " & TimeOfDay() & vbCrLf & vbCrLf & "Click OK  " & (NumericUpDown1.Value + Label28.Text), MessageBoxIcon.Stop)
                    Label28.ForeColor = Color.White
                    Label28.Text = ("n. photo")
                    Label2.Text = ""
                    Label1.Text = ""
                    Blocca()
                    Exit Sub
                End Try
                SerialPort1.DiscardInBuffer()
                SerialPort1.Close()
                Thread.Sleep(TextBox5.Text) 'attesa movimento motori 
                Label3.Text = mess1
                Application.DoEvents()
            End While
            If CheckBox2.Checked = True Then
                If Not SerialPort1.IsOpen Then SerialPort1.Open()
                Thread.Sleep(250)
                SerialPort1.Write("8")
                SerialPort1.Write("0")
                Thread.Sleep(250)
                SerialPort1.Write("7")
                SerialPort1.Write("0")
                Thread.Sleep(250)
                SerialPort1.Write("6")
                SerialPort1.Close()
                System.Diagnostics.Process.Start("shutdown", "-s -f -t 60")
                MsgBox("Spegnimento in corso" & vbCrLf & vbCrLf & "Premi Ok per Annullare.", MsgBoxStyle.OkOnly, )
                If MsgBoxResult.Ok Then
                    System.Diagnostics.Process.Start("shutdown", "-a")
                    MsgBox("Spegnimento Annullato", MessageBoxIcon.Stop)
                End If

            End If

            If Not SerialPort1.IsOpen Then SerialPort1.Open()
            SerialPort1.Write("6")
            SerialPort1.Close()
            Label28.Text = 0
            Dim a As String = Label1.Text()
            Dim b As String = TimeOfDay()
            Dim t1 As TimeSpan = TimeSpan.Parse(a)
            Dim t2 As TimeSpan = TimeSpan.Parse(b)
            Dim tDiff As TimeSpan
            tDiff = t2.Subtract(t1)
            Dim days = (tDiff.ToString)
            Label2.Text = days
            MsgBox("FINE" & vbCrLf & vbCrLf & "Ora Inizio " & Label1.Text() & vbCrLf & vbCrLf & "Ora Fine " & TimeOfDay() & vbCrLf & vbCrLf & "Durata " & days & vbCrLf & vbCrLf & "Click OK  " & (NumericUpDown1.Value), MessageBoxIcon.Information)
            Label28.Text = ("n. photo")
            Label28.ForeColor = Color.White
            If Not SerialPort1.IsOpen Then SerialPort1.Open()
            SerialPort1.DiscardInBuffer()
            SerialPort1.Close()
            Label1.Text = ""
            Label2.Text = ""
            Sblocca()
        Else
            'TASTO AVVIO MODO CAMERA -----------------------------------------------------
            If PLAY1 = False Then
                MsgBox("Prima Apri Preview Camera", MessageBoxIcon.Exclamation)
                Exit Sub
            End If
            Label1.Text = TimeOfDay()
            My.Computer.Audio.PlaySystemSound(Media.SystemSounds.Asterisk)
            Blocca()
            While index <= Numero
                If ANNULLA = True Then
                    If MsgBox("Sicuro di voler Annullare?", MsgBoxStyle.YesNo Or MsgBoxStyle.Question,) = MsgBoxResult.Yes Then
                        If Not SerialPort1.IsOpen Then SerialPort1.Open()
                        SerialPort1.Write("6")
                        SerialPort1.Close()
                        Label28.Text = -Numero - 1 + index
                        MsgBox("ANNULLATO" & vbCrLf & vbCrLf & "Ora Inizio " & Label1.Text() & vbCrLf & vbCrLf & "Ora Fine " & TimeOfDay() & vbCrLf & vbCrLf & "Click OK  " & (NumericUpDown1.Value + Label28.Text), MessageBoxIcon.Stop)
                        Label28.Text = ("Click n.")
                        Label28.ForeColor = Color.White
                        Label2.Text = ""
                        If Not SerialPort1.IsOpen Then SerialPort1.Open()
                        SerialPort1.DiscardInBuffer()
                        SerialPort1.Close()
                        Label1.Text = ""
                        ANNULLA = False
                        Sblocca()
                        Exit Sub
                    Else
                        ANNULLA = False
                        Button24.Enabled = True
                    End If
                End If
                PictureBox1.BackColor = Color.Green
                Label28.Text = ("")
                Label28.ForeColor = Color.Red
                Label28.Text = -Numero + index
                tempo = (CInt(-Label28.Text) * (CInt(TextBox4.Text) + CInt(TextBox5.Text)) / 1000)
                Secondi = tempo Mod 60
                tempo = Int(tempo / 60)
                Minuti = tempo Mod 60
                tempo = Int(tempo / 60)
                Ore = tempo
                mio = Format(Ore, "00") & ":" & Format(Minuti, "00") & ":" & Format(Secondi, "00")
                Label2.Text = mio
                index += 1
                If TextBox12.Text <> Nothing And NumericUpDown8.Text <> Nothing And TextBox15.Text <> Nothing Then
                    no = TextBox12.Text & "\" & TextBox15.Text & "_" & NumericUpDown8.Text.PadLeft(5, "0") & "." & ComboBox3.Text

                    'verifica se il file di destinazione esiste
                    If My.Computer.FileSystem.FileExists(no) Then
                        My.Computer.Audio.PlaySystemSound(Media.SystemSounds.Beep)
                        MsgBox(no & vbCrLf & vbCrLf & "Il File è Esitente!", MessageBoxIcon.Stop, MsgBoxStyle.OkOnly)
                        Label28.Text = ("n. photo")
                        Label28.ForeColor = Color.White
                        If Not SerialPort1.IsOpen Then SerialPort1.Open()
                        SerialPort1.DiscardInBuffer()
                        SerialPort1.Close()
                        Label1.Text = ""
                        Label2.Text = ""
                        Sblocca()
                        Exit Sub
                    End If

                    TextBox16.Text = no
                    Dim G As Integer = 0
                    Do While G <= 10
                        Application.DoEvents()
                        If OCC1 = 1 Then
                            ' Application.DoEvents()
                            Exit Do
                        Else
                            G = 0
                        End If
                    Loop
                    ELABORA()

                    Dim F As Integer = 0
                    Do While F <= 10
                        Application.DoEvents()
                        If ELAB = False Then
                            'Application.DoEvents()
                            Exit Do
                        Else
                            F = 0
                        End If
                    Loop
                    If My.Settings.FERMA = True Then '------------------------------------------------
                        If Not SerialPort1.IsOpen Then SerialPort1.Open()
                        SerialPort1.Write("6")
                        SerialPort1.Close()
                        Label28.Text = -Numero - 1 + index
                        MsgBox("ERRORE! CONTROLLO VISIVO" & vbCrLf & vbCrLf & "Ora Inizio " & Label1.Text() & vbCrLf & vbCrLf & "Ora Fine " & TimeOfDay() & vbCrLf & vbCrLf & "Click OK  " & (NumericUpDown1.Value + Label28.Text), MessageBoxIcon.Stop)
                        Label28.Text = ("n. photo")
                        Label28.ForeColor = Color.White
                        If Not SerialPort1.IsOpen Then SerialPort1.Open()
                        SerialPort1.DiscardInBuffer()
                        SerialPort1.Close()
                        Label1.Text = ""
                        Label2.Text = ""
                        ANNULLA = False
                        My.Settings.FERMA = False
                        ToolTip1.SetToolTip(Button12, "Reset Contatori")
                        Sblocca()
                        Exit Sub
                    End If

                    My.Settings.foto_in = True
                    SALVA()
                    Dim D As Integer = 0
                    Do While D <= 10
                        Application.DoEvents()
                        If My.Settings.foto_in = False Then
                            'Application.DoEvents()
                            Exit Do
                        Else
                            D = 0
                        End If
                    Loop
                    Thread.Sleep(TextBox4.Text)
                    PictureBox1.BackColor = Color.FromArgb(27, 27, 27)
                Else
                    TextBox16.Text = ""
                    PictureBox1.BackColor = Color.FromArgb(27, 27, 27)
                    MsgBox("Inserisci percorso destinazione" & vbCrLf & vbCrLf & "Nome e Numero del Prossimo File", MessageBoxIcon.Stop)
                    Exit Sub
                End If
                NumericUpDown8.Text = NumericUpDown8.Text + 1
                TextBox1.Text = TextBox1.Text + 1
                cont = CInt(TextBox1.Text)
                PictureBox3.BackColor = Color.Green
                Application.DoEvents()
                If Not SerialPort1.IsOpen Then SerialPort1.Open()
                SerialPort1.Write(s8_n8)
                SerialPort1.Write(CInt(TextBox1.Text))
                Try
                    mess1 = SerialPort1.ReadLine()
                Catch ex As Exception
                    If Not SerialPort1.IsOpen Then SerialPort1.Open()
                    SerialPort1.Write("6")
                    SerialPort1.Close()
                    Label28.Text = -Numero - 1 + index
                    MsgBox("ERRORE: Nessuna Risposta" & vbCrLf & vbCrLf & "Arduino non trovato" & vbCrLf & vbCrLf & "Resettare o Ricaricare lo Sketch", MessageBoxIcon.Error)
                    MsgBox("ANNULLAMENTO AUTOMATICO" & vbCrLf & vbCrLf & "Ora Inizio " & Label1.Text() & vbCrLf & vbCrLf & "Ora Fine " & TimeOfDay() & vbCrLf & vbCrLf & "Click OK  " & (NumericUpDown1.Value + Label28.Text), MessageBoxIcon.Error)
                    Label28.ForeColor = Color.White
                    Label28.Text = ("n. photo")
                    PictureBox3.BackColor = Color.FromArgb(27, 27, 27)
                    Label2.Text = ""
                    Label1.Text = ""
                    Blocca()
                    Exit Sub
                End Try
                SerialPort1.DiscardInBuffer()
                SerialPort1.Close()
                Thread.Sleep(TextBox5.Text) 'attesa movimento motori 
                Label3.Text = mess1
                PictureBox3.BackColor = Color.FromArgb(27, 27, 27)
                OCC1 = 0
            End While
            'spegni al termine del ciclo
            If CheckBox2.Checked = True Then
                If Not SerialPort1.IsOpen Then SerialPort1.Open()
                Thread.Sleep(250)
                SerialPort1.Write("8")
                SerialPort1.Write("0")
                Thread.Sleep(250)
                SerialPort1.Write("7")
                SerialPort1.Write("0")
                Thread.Sleep(250)
                SerialPort1.Write("6")
                SerialPort1.Close()
                System.Diagnostics.Process.Start("shutdown", "-s -f -t 60")
                MsgBox("Spegnimento in corso" & vbCrLf & vbCrLf & "Premi Ok per Annullare.", MsgBoxStyle.OkOnly, )
                If MsgBoxResult.Ok Then
                    System.Diagnostics.Process.Start("shutdown", "-a")
                    MsgBox("Spegnimento Annullato", MessageBoxIcon.Stop)
                End If
            End If
            If Not SerialPort1.IsOpen Then SerialPort1.Open()
            SerialPort1.Write("6")
            SerialPort1.Close()
            Label28.Text = 0
            Dim a As String = Label1.Text()
            Dim b As String = TimeOfDay()
            Dim t1 As TimeSpan = TimeSpan.Parse(a)
            Dim t2 As TimeSpan = TimeSpan.Parse(b)
            Dim tDiff As TimeSpan
            tDiff = t2.Subtract(t1)
            Dim days = (tDiff.ToString)
            Label2.Text = days
            MsgBox("FINE" & vbCrLf & vbCrLf & "Ora Inizio " & Label1.Text() & vbCrLf & vbCrLf & "Ora Fine " & TimeOfDay() & vbCrLf & vbCrLf & "Durata " & days & vbCrLf & vbCrLf & "Click OK  " & (NumericUpDown1.Value), MessageBoxIcon.Information)
            Label28.Text = ("n. photo")
            Label28.ForeColor = Color.White
            If Not SerialPort1.IsOpen Then SerialPort1.Open()
            SerialPort1.DiscardInBuffer()
            SerialPort1.Close()
            Label1.Text = ""
            Label2.Text = ""
            Sblocca()
        End If
    End Sub
    'TASTO INDIETRO 1 FOTOGRAMMA
    Private Sub Button52_Click(sender As Object, e As EventArgs) Handles Button52.Click
        Dim Numero = NumericUpDown2.Value
        Dim index As Integer = 1
        Blocca()
        While index <= Numero
            Application.DoEvents()
            If ANNULLA = True Then
                ANNULLA = False
                Exit While
            End If
            Button52.BackColor = Color.DodgerBlue
            Label29.ForeColor = Color.Red
            Label29.Text = 1 + Numero - index
            index += 1
            If Not SerialPort1.IsOpen Then SerialPort1.Open()
            SerialPort1.Write("2")
            SerialPort1.Write(s8_n8)
            SerialPort1.Close()
            Application.DoEvents()
            Thread.Sleep(TextBox34.Text)
        End While
        If Not SerialPort1.IsOpen Then SerialPort1.Open()
        SerialPort1.Write("6")
        SerialPort1.Close()
        Label29.ForeColor = Color.White
        Label29.Text = "step"
        Button52.BackColor = Color.FromArgb(27, 27, 27)
        Sblocca()

    End Sub
    'TASTO AVANTI 1 FOTOGRAMMA
    Private Sub Button25_Click(sender As Object, e As EventArgs) Handles Button25.Click
        Dim Numero = NumericUpDown2.Value
        Dim index As Integer = 1
        Blocca()
        While index <= Numero
            Application.DoEvents()
            If ANNULLA = True Then
                ANNULLA = False
                Exit While
            End If
            Button25.BackColor = Color.DodgerBlue
            Label29.ForeColor = Color.Red
            Label29.Text = 1 + Numero - index
            index += 1
            If Not SerialPort1.IsOpen Then SerialPort1.Open()
            SerialPort1.Write("3")
            SerialPort1.Write(s8_n8)
            SerialPort1.Close()
            Application.DoEvents()
            Thread.Sleep(TextBox34.Text)
        End While
        If Not SerialPort1.IsOpen Then SerialPort1.Open()
        SerialPort1.Write("6")
        SerialPort1.Close()
        Label29.ForeColor = Color.White
        Label29.Text = "step"
        Button25.BackColor = Color.FromArgb(27, 27, 27)
        Sblocca()
    End Sub
    'TASTO AVANTI 1/20 DI FOTOGRAMMA MICRO STEP
    Private Sub Button103_Click(sender As Object, e As EventArgs) Handles Button103.Click
        Dim Numero = NumericUpDown3.Value
        Dim index As Integer = 1
        Blocca()
        While index <= Numero
            Application.DoEvents()
            If ANNULLA = True Then
                ANNULLA = False
                Exit While
            End If
            Button103.BackColor = Color.DodgerBlue
            Label30.ForeColor = Color.Red
            Label30.Text = 1 + Numero - index
            index += 1
            If Not SerialPort1.IsOpen Then SerialPort1.Open()
            SerialPort1.Write("4")
            SerialPort1.Close()
            Application.DoEvents()
            Thread.Sleep(TextBox35.Text)
        End While
        If Not SerialPort1.IsOpen Then SerialPort1.Open()
        SerialPort1.Write("6")
        SerialPort1.Close()
        Label30.ForeColor = Color.White
        Label30.Text = "microstep"
        Button103.BackColor = Color.FromArgb(27, 27, 27)
        Sblocca()
    End Sub
    'TASTO INDIETRO 1/20 DI FOTOGRAMMA MICRO STEP
    Private Sub Button104_Click(sender As Object, e As EventArgs) Handles Button104.Click
        Dim Numero = NumericUpDown3.Value
        Dim index As Integer = 1
        Blocca()
        While index <= Numero
            Application.DoEvents()
            If ANNULLA = True Then
                ANNULLA = False
                Exit While
            End If
            Button104.BackColor = Color.DodgerBlue
            Label30.ForeColor = Color.Red
            Label30.Text = 1 + Numero - index
            index += 1
            If Not SerialPort1.IsOpen Then SerialPort1.Open()
            SerialPort1.Write("5")
            SerialPort1.Close()
            Application.DoEvents()
            Thread.Sleep(TextBox35.Text)
        End While
        If Not SerialPort1.IsOpen Then SerialPort1.Open()
        SerialPort1.Write("6")
        SerialPort1.Close()
        Label30.ForeColor = Color.White
        Label30.Text = "microstep"
        Button104.BackColor = Color.FromArgb(27, 27, 27)
        Sblocca()
    End Sub
    'TASTO ANNULLA
    Private Sub Button24_Click(sender As Object, e As EventArgs) Handles Button24.Click
        ANNULLA = True
        My.Computer.Audio.PlaySystemSound(System.Media.SystemSounds.Beep)
        Button24.Enabled = False

    End Sub
    'RESET CONTATORE
    Private Sub Button12_Click(sender As Object, e As EventArgs) Handles Button12.Click
        TextBox1.Text = ("0")
        Label3.Text = ("0")
        Label1.Text = "00:00:00"
        Label2.Text = "00:00:00"
        'NumericUpDown1.ReadOnly = False
        NumericUpDown1.Text = 1
        NumericUpDown2.Text = 1
        NumericUpDown3.Text = 1
        CheckBox2.CheckState = CheckState.Unchecked
    End Sub
#End Region

#Region "DESTINAZIONE FILE"
    ' SELEZIONA CARTELLA DI LAVORO
    Private Sub Button26_Click(sender As Object, e As EventArgs) Handles Button26.Click
        'in mod camera
        If RadioButton2.Checked = True Then
            Using folderDlg As New FolderBrowserDialog
                folderDlg.SelectedPath = TextBox12.Text
                folderDlg.ShowNewFolderButton = True
                Label32.Text = 0 & " File "
                If (folderDlg.ShowDialog() = DialogResult.OK) Then
                    TextBox12.Text = folderDlg.SelectedPath
                    My.Settings.cart_lav = TextBox12.Text
                    If TextBox12.Text <> "" Then
                        Dim files() As String = Directory.GetFiles(TextBox12.Text)
                        ListBox1.Items.Clear()
                        For Each file As String In files
                            ListBox1.Items.Add(Path.GetFileName(file))
                        Next
                        Dim pat As String
                        pat = TextBox12.Text
                        Dim filess() As String
                        If Directory.Exists(pat) Then
                            filess = Directory.GetFiles(pat)
                            Label32.Text = filess.Length & " Files "
                            If ListBox1.Items.Count.ToString() = 0 Then
                                Exit Sub
                            Else
                                ListBox1.SetSelected(ListBox1.Items.Count.ToString() - 1, True)
                            End If
                        End If
                    End If
                End If
            End Using
        End If
        'in mod picture
        If RadioButton6.Checked = True Then
            Using folderDlg As New FolderBrowserDialog
                folderDlg.SelectedPath = TextBox12.Text
                folderDlg.ShowNewFolderButton = True
                ' PictureBox22.BackgroundImage = My.Resources.ANTEPRIMA
                Label33.Text = 0 & " File "
                If (folderDlg.ShowDialog() = DialogResult.OK) Then
                    TextBox12.Text = folderDlg.SelectedPath
                    If TextBox12.Text = TextBox14.Text Then
                        MessageBox.Show("la cartella di Origine non puo essere" & vbCrLf & vbCrLf & "la cartella di Destinazione")
                        TextBox12.Text = ""
                        ListBox8.Items.Clear()
                        Button26.PerformClick()
                        Exit Sub
                    Else
                        If TextBox12.Text <> "" Then
                            Dim files() As String = Directory.GetFiles(TextBox12.Text)
                            ListBox8.Items.Clear()
                            For Each file As String In files
                                ListBox8.Items.Add(Path.GetFileName(file))
                            Next
                            Dim pat As String
                            pat = TextBox12.Text
                            Dim filess() As String
                            If Directory.Exists(pat) Then
                                filess = Directory.GetFiles(pat)
                                Label33.Text = filess.Length & " Files "
                                If ListBox8.Items.Count.ToString() = 0 Then
                                    Exit Sub
                                Else
                                    ListBox8.SelectedIndex = 0
                                    If RENAME1 = False Then
                                        Try
                                            Dim ratio As String = ListBox8.SelectedItem.ToString()
                                            Dim split = ratio.Split("_", 2, StringSplitOptions.RemoveEmptyEntries)
                                            Dim split1 = ratio.Split("_"c, "."c)(1) ',(StringSplitOptions.RemoveEmptyEntries)
                                            'Dim split2 = ratio.Split(".", 2, StringSplitOptions.RemoveEmptyEntries)
                                            TextBox13.Text = split(0)
                                            NumericUpDown7.Value = split1
                                        Catch ex As Exception
                                            MsgBox("Nome file non Valido" & vbCrLf & vbCrLf & "Formato corretto" & vbCrLf & vbCrLf & "File_00000.jpg")
                                        End Try
                                    End If
                                End If
                            End If
                        End If
                    End If
                End If
            End Using
        End If
        'in mod movie
        If RadioButton1.Checked = True Then
            Using folderDlg As New FolderBrowserDialog
                folderDlg.SelectedPath = TextBox12.Text
                folderDlg.ShowNewFolderButton = True
                Label13.Text = 0 & " File "
                If (folderDlg.ShowDialog() = DialogResult.OK) Then
                    TextBox12.Text = folderDlg.SelectedPath
                    My.Settings.cart_lav = TextBox12.Text
                    If TextBox12.Text <> "" Then
                        Dim files() As String = Directory.GetFiles(TextBox12.Text)
                        ListBox2.Items.Clear()
                        For Each file As String In files
                            ListBox2.Items.Add(Path.GetFileName(file))
                        Next
                        Dim pat As String
                        pat = TextBox12.Text
                        Dim filess() As String
                        If Directory.Exists(pat) Then
                            filess = Directory.GetFiles(pat)
                            Label13.Text = filess.Length & " Files "
                            If ListBox2.Items.Count.ToString() = 0 Then
                                Exit Sub
                            Else
                                ListBox2.SetSelected(ListBox2.Items.Count.ToString() - 1, True)
                            End If
                        End If
                    End If
                End If
            End Using
        End If
        'in mod viewer
        If RadioButton5.Checked = True Then
            Using folderDlg As New FolderBrowserDialog
                folderDlg.SelectedPath = TextBox12.Text
                folderDlg.ShowNewFolderButton = True
                ' PictureBox22.BackgroundImage = My.Resources.ANTEPRIMA
                Label27.Text = 0 & " File "
                If (folderDlg.ShowDialog() = DialogResult.OK) Then
                    TextBox12.Text = folderDlg.SelectedPath
                    If TextBox12.Text = TextBox14.Text Then
                        MessageBox.Show("la cartella di Origine non puo essere" & vbCrLf & vbCrLf & "la cartella di Destinazione")
                        TextBox12.Text = ""
                        ListBox4.Items.Clear()
                        Button26.PerformClick()
                        Exit Sub
                    Else
                        If TextBox12.Text <> "" Then
                            Dim files() As String = Directory.GetFiles(TextBox12.Text)
                            ListBox4.Items.Clear()
                            For Each file As String In files
                                ListBox4.Items.Add(Path.GetFileName(file))
                            Next
                            Dim pat As String
                            pat = TextBox12.Text
                            Dim filess() As String
                            If Directory.Exists(pat) Then
                                filess = Directory.GetFiles(pat)
                                Label27.Text = filess.Length & " Files "
                                If ListBox4.Items.Count.ToString() = 0 Then
                                    Exit Sub
                                Else
                                    ListBox4.SelectedIndex = 0
                                End If
                            End If
                        End If
                    End If
                End If
            End Using
        End If
    End Sub
    ' MODIFICA TESTO FILE DA CREARE
    Private Sub TextBox15_TextChanged(sender As Object, e As EventArgs) Handles TextBox15.TextChanged

        If TextBox12.Text <> Nothing And TextBox15.Text <> Nothing And NumericUpDown8.Text <> Nothing And ComboBox3.Text <> Nothing Then

            ' If My.Settings.foto_in = False And My.Settings.foto_in2 = False Then
            no = (TextBox12.Text & "\" & TextBox15.Text & "_" & NumericUpDown8.Text.PadLeft(5, "0")) & "." & ComboBox3.Text
            My.Settings.solo_file = (TextBox15.Text & "_" & NumericUpDown8.Text.PadLeft(5, "0") & "." & ComboBox3.Text)
            TextBox16.Text = no
            My.Settings.nome = TextBox15.Text
            ' End If
        Else
            TextBox16.Text = ""


        End If
    End Sub
    ' MODIFICA NUMERO FILE DA CREARE
    Private Sub NumericUpDown8_TextChanged(sender As Object, e As EventArgs) Handles NumericUpDown8.TextChanged
        If TextBox12.Text <> Nothing And TextBox15.Text <> Nothing And NumericUpDown8.Text <> Nothing And ComboBox3.Text <> Nothing Then

            ' If My.Settings.foto_in = False And My.Settings.foto_in2 = False Then
            no = (TextBox12.Text & "\" & TextBox15.Text & "_" & NumericUpDown8.Text.PadLeft(5, "0")) & "." & ComboBox3.Text
            My.Settings.solo_file = (TextBox15.Text & "_" & NumericUpDown8.Text.PadLeft(5, "0") & "." & ComboBox3.Text)
            TextBox16.Text = no
            My.Settings.inizio = NumericUpDown8.Text
            ' End If
        Else
            TextBox16.Text = ""

        End If
    End Sub
    ' MODIFICA CARTELLA DI LAVORO
    Private Sub TextBox12_TextChanged(sender As Object, e As EventArgs) Handles TextBox12.TextChanged
        If TextBox12.Text <> Nothing And TextBox15.Text <> Nothing And NumericUpDown8.Text <> Nothing And ComboBox3.Text <> Nothing Then

            '   If My.Settings.foto_in = False And My.Settings.foto_in2 = False Then
            no = (TextBox12.Text & "\" & TextBox15.Text & "_" & NumericUpDown8.Text.PadLeft(5, "0")) & "." & ComboBox3.Text
            My.Settings.solo_file = (TextBox15.Text & "_" & NumericUpDown8.Text.PadLeft(5, "0") & "." & ComboBox3.Text)
            TextBox16.Text = no
            My.Settings.cart_lav = TextBox12.Text
            '   End If
        Else
            TextBox16.Text = ""

        End If
    End Sub
    ' SELEZIONE TIPO DI FILE DA CREARE
    Private Sub ComboBox3_textchanged(sender As Object, e As EventArgs) Handles ComboBox3.TextChanged

        If TextBox12.Text <> Nothing And TextBox15.Text <> Nothing And NumericUpDown8.Text <> Nothing And ComboBox3.Text <> Nothing Then

            ' If My.Settings.foto_in = False And My.Settings.foto_in2 = False Then
            no = (TextBox12.Text & "\" & TextBox15.Text & "_" & NumericUpDown8.Text.PadLeft(5, "0")) & "." & ComboBox3.Text
            My.Settings.solo_file = (TextBox15.Text & "_" & NumericUpDown8.Text.PadLeft(5, "0") & "." & ComboBox3.Text)
            My.Settings.cart_lav = TextBox12.Text
            TextBox16.Text = no
            '  End If
        Else
            TextBox16.Text = ""

        End If
    End Sub
    ' SE IL FILE DI DESINAZIONE E' ESISTENTE CAMBIA COLORE TESTO IN ROSSO
    Private Sub Textbox16_TextChanged(sender As Object, e As EventArgs) Handles TextBox16.TextChanged
        If TextBox16.Text IsNot Nothing Then
            If My.Computer.FileSystem.FileExists(no) Then
                TextBox16.ForeColor = Color.Red
                ToolTip1.SetToolTip(TextBox16, "File Esistente!")
            Else
                TextBox16.ForeColor = Color.Green
                ToolTip1.SetToolTip(TextBox16, "File Destinazione")
            End If
        End If

    End Sub
#End Region

#Region "CATTURA, ELABORA, AVANZATO"

#Region "FUNZIONI VIDEOCAMERA"
    'apre cam mod
    Private Sub RadioButton2_CheckedChanged(sender As Object, e As EventArgs) Handles RadioButton2.CheckedChanged
        If RadioButton2.Checked = True Then
            'Button86.Visible = True
            Button98.Visible = True
            Panello_elabora.Size = New Size(Panello_chiudi.Width - Panello_comandi.Width - Panello_arduino.Width, Panello_comandi.Height - Panello_sorgente.Height)
            Panello_elabora.Visible = True
            Panello_arduino.Visible = True
            Panello_file.Size = New Size(Panello_chiudi.Width - Panello_comandi.Width - Panello_arduino.Width, Panello_sorgente.Height)
            PictureBox22.Size = New Size(Panel3.Width, Panel3.Height)

            Panello_file.Visible = True
            Panello_sorgente.Visible = True
            Panel35.Visible = True
            Panel36.Visible = True
            Panello_estrai_video.Visible = True
            TableLayoutPanel97.Visible = True
            Button15.PerformClick()
            PictureBox20.Image = Nothing
            PictureBox22.Image = Nothing
            PictureBox22.BackgroundImage = Nothing
            PictureBox27.Image = Nothing
            EnumerateVideoDevices()
        Else
            If Pannello_setup.Visible = True Then
                Button38.PerformClick()
            End If

            'Button86.Visible = False
            Button98.Visible = False
            Panello_elabora.Visible = False
            Panello_arduino.Visible = False
            Panello_file.Visible = False
            Panello_sorgente.Visible = False
            Panel35.Visible = False
            Panel36.Visible = False
            Panello_estrai_video.Visible = False
            TableLayoutPanel97.Visible = False
            CameraStop()
            PictureBox20.Image = Nothing
            PictureBox22.Image = Nothing
            PictureBox22.BackgroundImage = Nothing
            PictureBox27.Image = Nothing
        End If
    End Sub
    ' INIZIALIZZAZIONE VIDEO CAMERA
    Private Sub EnumerateVideoDevices()
        ComboBox2.Items.Clear()
        ' enumerate video devices
        videoDevices = New FilterInfoCollection(FilterCategory.VideoInputDevice)
        If videoDevices.Count <> 0 Then
            ' add all devices to combo
            For Each device As FilterInfo In videoDevices
                ComboBox2.Items.Add(device.Name)
            Next
            If ComboBox2.Items.Contains(My.Settings.def_cam) Then
                ComboBox2.Text = My.Settings.def_cam
                ComboBox8.Text = My.Settings.def_res
                PictureBox22.Image = Nothing
            Else
                ComboBox2.SelectedIndex = 0
            End If

        Else
            ComboBox2.Items.Add("No devices found")
            ' PictureBox22.Image = My.Resources.nocamera
        End If

    End Sub
    ' INIZIALIZZAZIONE RISOLUZIONE VIDEO CAMERA
    Private Sub EnumerateVideoModes() 'device As VideoCaptureDevice)
        ' get resolutions for selected video source
        Me.Cursor = Cursors.WaitCursor
        ComboBox8.Items.Clear()
        Try
            videoCapabilities = videoDevice.VideoCapabilities
            For Each capabilty As VideoCapabilities In videoCapabilities
                If Not ComboBox8.Items.Contains(capabilty.FrameSize) Then
                    ComboBox8.Items.Add(capabilty.FrameSize)
                End If
            Next
            If videoCapabilities.Length = 0 Then
                ComboBox8.Items.Add("Not supported")
                ComboBox8.SelectedIndex = 0
                'PictureBox22.Image = My.Resources.nosupport
            End If
        Finally
            Me.Cursor = Cursors.[Default]
        End Try
    End Sub
    ' SELEZIONA CAMERA
    Private Sub ComboBox2_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox2.SelectedIndexChanged
        If videoDevices.Count <> 0 Then
            videoDevice = New VideoCaptureDevice(videoDevices(ComboBox2.SelectedIndex).MonikerString)
            EnumerateVideoModes()
            ComboBox8.SelectedIndex = 0

        End If
    End Sub
    ' BOTTONE camera START/STOP
    Private Sub Button98_Click(sender As Object, e As EventArgs) Handles Button98.Click
        If ComboBox8.Text <> "Not supported" Then


            If PLAY1 = False Then
                TableLayoutPanel18.Enabled = True
                CameraStart()
            Else
                If AVA = True Then
                    CheckBox8.Checked = False
                End If
                TableLayoutPanel18.Enabled = False
                CameraStop()
            End If
        End If
    End Sub
    ' AVVIO CAMERA START
    Private Sub CameraStart()

        videoDevice = New VideoCaptureDevice(videoDevices(ComboBox2.SelectedIndex).MonikerString)


        If videoDevice IsNot Nothing Then
            If (videoCapabilities IsNot Nothing) AndAlso (videoCapabilities.Length <> 0) Then
                videoDevice.VideoResolution = videoDevice.VideoCapabilities(ComboBox8.SelectedIndex)
                AddHandler videoDevice.NewFrame, New NewFrameEventHandler(AddressOf Video_NewFrame)
                videoDevice.Start()
                Dim ratio As String = ComboBox8.Text
                Dim split = ratio.Split("; ", 2, StringSplitOptions.RemoveEmptyEntries)
                PictureBox22.Height = PictureBox22.Width / (split(0) / split(1))
                Button98.BackgroundImage = My.Resources.cam_off
                Button9.BackgroundImage = My.Resources.preoff
                PLAY1 = True
                My.Settings.cam_on = True
                'ComboBox2.Enabled = False
                CheckBox4.Visible = True 'specchia x
                CheckBox19.Visible = True ' specchia y
                Button21.Visible = True ' zoom
                Button86.Visible = True ' carica immagine
                CheckBox8.Visible = True ' apri avanzato
                TableLayoutPanel97.Enabled = False

                'Button37.Visible = True ' zoom
                'Button42.Visible = True ' zoom
                '        NumericUpDown9.Visible = True ' zoom
                'Button9.Visible = True ' attiva/disattiva anteprima
                'Button10.Visible = True ' apri in finestra 
                'Button11.Visible = True ' impostazioni cam
                'Button20.Visible = True ' salva fotogramma

                ToolTip1.SetToolTip(Button98, "Camera Stop")
                OCC1 = 0



            Else
                MessageBox.Show("non Compatibile")
                'PictureBox22.Image = My.Resources.nosupport
            End If
        Else
            PictureBox22.BackgroundImage = Nothing
            PictureBox22.Image = My.Resources.cam_off
            Button9.BackgroundImage = My.Resources.preoff

        End If
    End Sub
    ' FERMA CAMERA STOP
    Private Sub CameraStop()
        ' stop video device
        If videoDevice IsNot Nothing Then
            videoDevice.SignalToStop()
            videoDevice.WaitForStop()
            videoDevice.Stop()

            LIVECAM = True
            Button98.BackgroundImage = My.Resources.cam_on
            Button9.BackgroundImage = My.Resources.preon

            My.Settings.def_cam = ComboBox2.Text
            My.Settings.def_res = ComboBox8.Text


            PLAY1 = False
            My.Settings.cam_on = False
            If Button21.Visible = False Then
                Button77.PerformClick()
            End If
            Button21.Visible = False ' zoom
            Button27.Visible = False ' salva avanzato
            Button28.Visible = False ' ricarica avanzato
            Button86.Visible = False ' carica immagine
            CheckBox4.Visible = False 'specchia x
            CheckBox8.Visible = False ' apri avanzato
            CheckBox9.Visible = False ' ridimensiona sposa selezione
            CheckBox19.Visible = False ' specchia y
            TableLayoutPanel97.Enabled = True
            ToolTip1.SetToolTip(Button98, "Camera Start")
            PictureBox22.BackgroundImage = Nothing
            PictureBox27.Image = Nothing
        End If
    End Sub
    ' APRI PROPRIETA' CAMERA
    Private Sub Button11_Click(sender As Object, e As EventArgs) Handles Button11.Click, Button23.Click
        If (videoCapabilities IsNot Nothing) AndAlso (videoCapabilities.Length <> 0) Then
            videoDevice.DisplayPropertyPage(IntPtr.Zero)
        End If
    End Sub
    ' ESTRAZIONE FOTOGRAMMI DA VIDEO CAMERA
    Private Sub Video_NewFrame(sender As Object, eventArgs As NewFrameEventArgs)
        If real = True Then

            If PictureBox5.BackgroundImage IsNot Nothing Then PictureBox5.BackgroundImage.Dispose()
            PictureBox5.BackgroundImage = TryCast(eventArgs.Frame.Clone, Bitmap)
        Else
            If LIVECAM = True Then
                If PictureBox27.Image IsNot Nothing Then PictureBox27.Image.Dispose()
                If LIVE IsNot Nothing Then LIVE.Dispose()
                LIVE = TryCast(eventArgs.Frame.Clone, Bitmap)
                If eventArgs.Frame IsNot Nothing Then eventArgs.Frame.Dispose()
                PictureBox27.Image = LIVE

            End If

            If OCC1 = 0 Then

                If PictureBox22.BackgroundImage IsNot Nothing Then PictureBox22.BackgroundImage.Dispose()
                If IMAGEN IsNot Nothing Then IMAGEN.Dispose()
                If original IsNot Nothing Then original.Dispose()
                If LIVECAM = True Then
                    IMAGEN = LIVE.Clone
                Else
                    If LIVE IsNot Nothing Then LIVE.Dispose()
                    IMAGEN = TryCast(eventArgs.Frame.Clone, Bitmap)
                    If eventArgs.Frame IsNot Nothing Then eventArgs.Frame.Dispose()
                End If
                If IMAGEN Is Nothing Then
                    OCC1 = 0
                    Exit Sub
                Else
                    original = IMAGEN.Clone
                End If
                'zoom
                If ZOOMON = True Then
                    IMAGENZOOM = IMAGEN.Clone
                    'IMAGEN.Dispose()
                    Dim BAS As Integer = IMAGENZOOM.Width / NumericUpDown9.Value
                    Dim ALT As Integer = IMAGENZOOM.Height / NumericUpDown9.Value
                    Dim BAS2 As Integer = IMAGENZOOM.Width * 0.5
                    Dim ALT2 As Integer = IMAGENZOOM.Height * 0.5
                    Dim BAS3 As Integer = BAS * 0.5
                    Dim ALT3 As Integer = ALT * 0.5
                    Dim ZOOMBitmap As New Bitmap(BAS, ALT)
                    Dim Z As Graphics = Graphics.FromImage(ZOOMBitmap)
                    Z.InterpolationMode = InterpolationMode.HighQualityBicubic
                    Z.PixelOffsetMode = PixelOffsetMode.HighQuality
                    Z.CompositingQuality = CompositingQuality.HighQuality
                    Z.DrawImage(IMAGENZOOM, New Rectangle(0, 0, BAS, ALT),
                                                 -(BAS3 - BAS2) + ZOOMX, -(ALT3 - ALT2) + ZOOMY,
                                                   BAS, ALT, GraphicsUnit.Pixel)
                    Z.Dispose()
                    IMAGEN = New Bitmap(ZOOMBitmap, ZOOMBitmap.Width, ZOOMBitmap.Height)
                    ZOOMBitmap.Dispose()
                    IMAGENZOOM.Dispose()
                End If
                'specchia verticale
                If FLPY = True Then
                    IMAGEN.RotateFlip(RotateFlipType.RotateNoneFlipY)
                    original.RotateFlip(RotateFlipType.RotateNoneFlipY)
                    'Thread.Sleep(25)
                End If
                'specchia orizzontale
                If FLPX = True Then
                    IMAGEN.RotateFlip(RotateFlipType.RotateNoneFlipX)
                    original.RotateFlip(RotateFlipType.RotateNoneFlipX)
                    'Thread.Sleep(25)
                End If
                PictureBox22.BackgroundImage = IMAGEN
                OCC1 = 1
            End If
        End If
    End Sub
    ' SALVA FOTOGRAMMA IN MODALITA' CAMERA
    Private Sub Button20_Click(sender As Object, e As EventArgs) Handles Button20.Click
        If My.Settings.cam_on = True Then
            If My.Settings.SaveTitle21 = True Then
                MsgBox("Funzione disabilitata in" & vbCrLf & "Modalità Immagini e Diretta", MessageBoxIcon.Exclamation)
                Exit Sub
            Else
                If TextBox16.Text <> Nothing Then
                    no = TextBox12.Text & "\" & TextBox15.Text & "_" & NumericUpDown8.Text.PadLeft(5, "0") & "." & ComboBox3.Text
                    If My.Computer.FileSystem.FileExists(no) Then
                        My.Computer.Audio.PlaySystemSound(System.Media.SystemSounds.Beep)
                        MsgBox(no & vbCrLf & vbCrLf & "Il File è Esitente!", MessageBoxIcon.Exclamation, MsgBoxStyle.OkOnly)
                        Exit Sub
                    End If
                    My.Settings.foto_in = True
                    no = TextBox12.Text & "\" & TextBox15.Text & "_" & NumericUpDown8.Text.PadLeft(5, "0") & "." & ComboBox3.Text
                    My.Settings.solo_file = (TextBox15.Text & "_" & NumericUpDown8.Text.PadLeft(5, "0") & "." & ComboBox3.Text)
                    My.Settings.nome = TextBox15.Text
                    My.Settings.inizio = NumericUpDown8.Text
                    My.Settings.cart_lav = TextBox12.Text
                    SALVA()
                    NumericUpDown8.Text = NumericUpDown8.Text + 1

                    Dim D As Integer = 0
                    Do While D <= 10
                        Application.DoEvents()
                        If My.Settings.foto_in = False And My.Settings.foto_in2 = False Then
                            Button20.Enabled = True
                            Exit Do
                        Else
                            Button20.Enabled = False
                            D = 0
                        End If
                    Loop
                    Me.Button15.PerformClick()
                Else
                    '      
                    MsgBox("Inserisci percorso destinazione" & vbCrLf & vbCrLf & "Nome e Numero del Prossimo File", MessageBoxIcon.Exclamation)
                End If
            End If
        Else
            MsgBox("Apri Preview Camera", MessageBoxIcon.Exclamation)
        End If


    End Sub
    'BOTTONE NUOVO FOTOGRAAMMAA
    Private Sub Button86_Click(sender As Object, e As EventArgs) Handles Button86.Click
        OCC1 = 0
        If AVA = True Then
            Dim G As Integer = 0
            Do While G <= 10
                Application.DoEvents()
                If OCC1 = 1 Then
                    ELABORA()
                    Exit Do
                Else
                    G = 0
                End If
            Loop
        End If
    End Sub
    'ATTIVA / DISATTIVA LIVE CAM DIRETTA 
    Private Sub Button9_Click_1(sender As Object, e As EventArgs) Handles Button9.Click
        If LIVECAM = True Then
            LIVECAM = False
            Button9.BackgroundImage = My.Resources.preon
            PictureBox27.Image = Nothing
            PictureBox27.Image = My.Resources.preon
        Else
            LIVECAM = True
            Button9.BackgroundImage = My.Resources.preoff
        End If
    End Sub
    ' APRI ANTEPRIMA video A SCHERMO INTERO
    Private Sub Button10_Click(sender As Object, e As EventArgs) Handles Button10.Click
        TableLayoutPanel95.Visible = False
        TableLayoutPanel34.Visible = True
        real = True
    End Sub
    'chiudi anteprima video a schermo initero
    Private Sub Button22_Click(sender As Object, e As EventArgs) Handles Button22.Click
        TableLayoutPanel34.Visible = False
        TableLayoutPanel95.Visible = True
        real = False
    End Sub
    ' AZZERA CONTATORE PASSAGGI SOLO IN AVANZATO
    Private Sub PictureBox35_DoubleClick(sender As Object, e As EventArgs) Handles PictureBox35.DoubleClick
        n = 0
        ToolTip1.SetToolTip(PictureBox17, n & " Fotogrammi OK" & vbCrLf & "Doppio Clik Azzera")
        PictureBox17.BackgroundImage = My.Resources.counter

    End Sub

#End Region

#Region "RETTANGOLO SELEZIONE"
    'RIDIMENSIONA/SPOSTA RIQUADRO SELEZIONE 
    Private Sub CheckBox9_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox9.CheckedChanged
        ELABORA()
    End Sub
    ' DETERMINA SE LA POSIZIONE MOUSE E' SOPRA L'ANGOLO O AL CENTRO DEL RETTANGOLO 
    Private Function MouseOverRectangle(x As Integer, y As Integer) As Integer
        If CheckBox9.Checked = True Then
            Dim RIDIMENSIONA As New Rectangle(X2 - 5, Y2 - 5, 10, 10)
            Dim SPOSTA As New Rectangle((X1 + ((X2 - X1) / 2) - 10), (Y1 + ((Y2 - Y1) / 2) - 10), 20, 20) '(MX2 - MX1), (MY2 - MY1))
            If RIDIMENSIONA.Contains(x, y) Then
                MouseOverRectangle = 2
            ElseIf SPOSTA.Contains(x, y) Then
                MouseOverRectangle = 1
            Else
                MouseOverRectangle = 0
            End If
        Else
            Return False
        End If
    End Function
    ' CREA RETTANGOLO SELEZIONE BOTTONE PREMUTO MOUSE 
    Private Sub PictureBox22_MouseDown(ByVal sender As Object, ByVal e As MouseEventArgs) Handles PictureBox22.MouseDown
        If AVA = True Then
            OCC1 = 1
            If CheckBox9.Checked = True Then
                Select Case MouseOverRectangle(e.X, e.Y)
                    Case 1  'SPOSTA
                        Cursor.Current = Cursors.SizeAll
                    Case 2 'RIDIMENSIONA
                        Cursor.Current = Cursors.SizeNWSE
                    Case Else
                        Cursor.Current = Cursors.Default
                End Select
                MouseDownStage = MouseOverRectangle(e.X, e.Y)
                MouseDownX = X1
                MouseDownY = Y1
            Else
                PictureBox22.Image = Nothing
                mouseClicked = True
                startPoint.X = e.X
                startPoint.Y = e.Y
                X1 = startPoint.X
                Y1 = startPoint.Y
                endPoint.X = -1
                endPoint.Y = -1
                Dim mous = New System.Drawing.Point(e.X, e.Y)
                rectCropArea = New Rectangle(mous, New Size())
            End If

        End If
    End Sub
    ' CREA RETTANGOLO SELEZIONE MOVIMENTO MOUSE
    Private Sub PictureBox22_MouseMove(ByVal sender As Object, ByVal e As MouseEventArgs) Handles PictureBox22.MouseMove
        If AVA = True Then
            If CheckBox9.Checked = True Then
                If MouseDownStage > 0 Then
                    If MouseDownStage = 1 Then
                        'sposta
                        Dim dx, dy, w, h As Integer
                        dx = e.X - MouseDownX
                        dy = e.Y - MouseDownY
                        w = (X2 - X1)
                        h = (Y2 - Y1)
                        X1 = MouseDownX + dx - w / 2
                        Y1 = MouseDownY + dy - h / 2
                        X2 = X1 + w
                        Y2 = Y1 + h
                    Else
                        'ridimensiona
                        X2 = e.X
                        Y2 = e.Y
                    End If
                    rectCropArea.X = X1
                    rectCropArea.Y = Y1
                    rectCropArea.Width = X2 - X1
                    rectCropArea.Height = Y2 - Y1
                    PictureBox22.Image = Nothing
                Else
                    Select Case MouseOverRectangle(e.X, e.Y)
                        Case 1  'SPOSTA
                            Cursor.Current = Cursors.SizeAll
                        Case 2 'RIDIMENSIONA
                            Cursor.Current = Cursors.SizeNWSE
                        Case Else
                            Cursor.Current = Cursors.Default
                    End Select
                End If
            Else
                Dim ptCurrent As New Point(e.X, e.Y)
                If (mouseClicked) Then
                    endPoint = ptCurrent
                    If (e.X > startPoint.X And e.Y > startPoint.Y) Then
                        rectCropArea.Width = e.X - startPoint.X
                        rectCropArea.Height = e.Y - startPoint.Y
                    ElseIf (e.X < startPoint.X And e.Y > startPoint.Y) Then
                        rectCropArea.Width = startPoint.X - e.X
                        rectCropArea.Height = e.Y - startPoint.Y
                        rectCropArea.X = e.X
                        rectCropArea.Y = startPoint.Y
                    ElseIf (e.X > startPoint.X And e.Y < startPoint.Y) Then
                        rectCropArea.Width = e.X - startPoint.X
                        rectCropArea.Height = startPoint.Y - e.Y
                        rectCropArea.X = startPoint.X
                        rectCropArea.Y = e.Y
                    Else
                        rectCropArea.Width = startPoint.X - e.X
                        rectCropArea.Height = startPoint.Y - e.Y
                        rectCropArea.X = e.X
                        rectCropArea.Y = e.Y
                    End If
                    PictureBox22.Image = Nothing
                End If
            End If
        End If
    End Sub
    ' CREA RETTANGOLO SELEZIONE BOTTONE RILASCIATO MOUSE
    Private Sub PictureBox22_MouseUp(ByVal sender As Object, ByVal e As MouseEventArgs) Handles PictureBox22.MouseUp
        If AVA = True Then
            If CheckBox9.Checked = True Then
                MouseDownStage = 0
            Else
                mouseClicked = False
                If (endPoint.X <> -1) Then
                    Dim currentPoint As New Point(e.X, e.Y)
                    X2 = e.X
                    Y2 = e.Y
                End If
                endPoint.X = -1
                endPoint.Y = -1
                startPoint.X = -1
                startPoint.Y = -1
            End If
            ELABORA()
        End If
    End Sub
    ' DISEGNA RETTANGOLO SELEZIONE
    Private Sub PictureBox22_Paint(ByVal sender As Object, ByVal e As PaintEventArgs) Handles PictureBox22.Paint
        If AVA = True Then
            If CheckBox9.Checked = True Then
                With e.Graphics
                    Using p As New Pen(Color.White, 3)
                        p.DashStyle = Drawing2D.DashStyle.Dash
                        .DrawRectangle(p, X1, Y1, (X2 - X1), (Y2 - Y1))
                        p.Width = 3
                        p.Color = Color.LimeGreen
                        p.DashStyle = Drawing2D.DashStyle.Solid
                        Dim rect As New Rectangle(X2 - 5, Y2 - 5, 10, 10)
                        .FillEllipse(Brushes.Red, rect)
                        Dim CENTER As New Rectangle(X1 + CInt((X2 - X1) / 2) - 5, Y1 + CInt((Y2 - Y1) / 2) - 5, 10, 10)
                        .FillEllipse(Brushes.Red, CENTER)
                    End Using
                End With
            Else
                Using drawLine As New Pen(Color.Red, 3)
                    drawLine.DashStyle = DashStyle.Dash
                    e.Graphics.DrawRectangle(drawLine, rectCropArea)
                End Using
            End If
        End If
    End Sub

#End Region

#Region "RETTANGOLO RITAGLIO"
    ' DISEGNA RETTANGOLO RITAGLIO CENTRATO E PROPORZIONATO 
    Private Sub Button33_Click(sender As Object, e As EventArgs) Handles Button33.Click
        If rectCropArea.Width <> rectCropArea.Height And PictureBox22.Image IsNot Nothing Then
            If RadioButton9.Checked = False Then
                ' normal 8 
                Button33.BackgroundImage = My.Resources.fotocenterN
                ' riferimento blob alto
                If CheckBox13.Checked = True And CheckBox14.Checked = False Then
                    CROPMX = CInt(BLB * (2.28 / 1.8))
                    CROPMY = CInt(BLH * (2.94 / 1.23))
                    CROPX = IMGRCX + BLX + CROPMX
                    CROPY = IMGRCY + BLY - CROPMY
                End If
                ' riferimento blob centro
                If CheckBox13.Checked = True And CheckBox14.Checked = True Or CheckBox13.Checked = False And CheckBox14.Checked = False Then
                    CROPMX = CInt(BLB * (2.28 / 1.8))
                    CROPMY = CInt(BLH * (3.555 / 1.23))
                    CROPX = IMGRCX + BLX + CROPMX
                    CROPY = IMGRCY + BLY + BLH / 2 - CROPMY
                End If
                ' riferimento blob basso
                If CheckBox13.Checked = False And CheckBox14.Checked = True Then
                    CROPMX = CInt(BLB * (2.28 / 1.8))
                    CROPMY = CInt(BLH * (4.17 / 1.23))
                    CROPX = IMGRCX + BLX + CROPMX
                    CROPY = IMGRCY + BLY + BLH - CROPMY
                End If
                CROPB = CInt(BLB * (4.5 / 1.8))
                CROPH = CInt(BLH * (3.3 / 1.23))
            Else
                'super 8
                Button33.BackgroundImage = My.Resources.fotocenter
                ' riferimento blob alto
                If CheckBox13.Checked = True And CheckBox14.Checked = False Then
                    CROPMX = CInt(BLB * (1.08 / 0.91))
                    CROPMY = CInt(BLH * (1.435 / 1.14))
                    CROPX = IMGRCX + BLX + CROPMX
                    CROPY = IMGRCY + BLY - CROPMY
                    CROPB = CInt(BLB * (5.46 / 0.91))
                End If
                ' riferimento blob centro
                If CheckBox13.Checked = True And CheckBox14.Checked = True Or CheckBox13.Checked = False And CheckBox14.Checked = False Then
                    CROPMX = CInt(BLB * (1.08 / 0.91))
                    CROPMY = CInt(BLH * (2.005 / 1.14))
                    CROPX = IMGRCX + BLX + CROPMX
                    CROPY = IMGRCY + BLY + BLH / 2 - CROPMY
                    CROPB = CInt(BLB * (5.46 / 0.91))
                End If
                ' riferimento blob basso
                If CheckBox13.Checked = False And CheckBox14.Checked = True Then
                    CROPMX = CInt(BLB * (1.08 / 0.91))
                    CROPMY = CInt(BLH * (2.575 / 1.14))
                    CROPX = IMGRCX + BLX + CROPMX
                    CROPY = IMGRCY + BLY + BLH - CROPMY
                End If
                CROPB = CInt(BLB * (5.46 / 0.91))
                CROPH = CInt(BLH * (4.01 / 1.14))
            End If
            'ottieni sempre numeri pari
            If CROPB Mod (2) = 0 Then
            Else
                CROPB += 1
            End If
            If CROPH Mod (2) = 0 Then
            Else
                CROPH += 1
            End If
            CROPMB = CROPB
            CROPMH = CROPH
            Label6.Text = CROPMB
            Label7.Text = CROPMH
            ELABORA()
        End If
    End Sub
    ' CAMBIA ICONA SELEZIONE RIFERIMENTO ALTO
    Private Sub CheckBox13_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox13.CheckedChanged
        If CheckBox13.Checked = True Then
            'CheckBox13.BackgroundImage = My.Resources.ANGOLOA
            'Button29.BackgroundImage = My.Resources.ALR
            If CheckBox14.Checked = True Then
                CheckBox14.Checked = False
            End If
        Else
            'CheckBox13.BackgroundImage = My.Resources.ANGOLOA1
            'Button29.BackgroundImage = My.Resources.ALA
        End If
        Me.Button33.PerformClick()
    End Sub
    ' CAMBIA ICONA SELEZIONE RIFERIMENTO BASSO
    Private Sub CheckBox14_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox14.CheckedChanged
        If CheckBox14.Checked = True Then
            'CheckBox14.BackgroundImage = My.Resources.ANGOLOB
            'Button32.BackgroundImage = My.Resources.BAR
            If CheckBox13.Checked = True Then
                CheckBox13.Checked = False
            End If
        Else
            'CheckBox14.BackgroundImage = My.Resources.ANGOLOB1
            'Button32.BackgroundImage = My.Resources.BAA
        End If
        Me.Button33.PerformClick()
    End Sub
    ' ALLINEA IN ALTO CONTROLLO VISIVO
    Private Sub Butto29_Click(sender As Object, e As EventArgs) Handles Button29.Click

        If CheckBox13.Checked = True Then
            CheckBox13.Checked = False
        Else
            CheckBox13.Checked = True
        End If
        If CheckBox7.Checked = True Then
            ELABORA()
        End If

    End Sub
    ' ALLINEA IN BASSO CONTROLLO VISIVO
    Private Sub Button32_Click(sender As Object, e As EventArgs) Handles Button32.Click
        If CheckBox14.Checked = True Then
            CheckBox14.Checked = False
        Else
            CheckBox14.Checked = True
        End If
        If CheckBox7.Checked = True Then
            ELABORA()
        End If
    End Sub
    ' SELEZIONA METODO RIDIMENSIONA XY O BH RITAGLIO
    Private Sub CheckBox15_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox15.CheckedChanged
        If CheckBox15.Checked = True Then
            CheckBox15.BackgroundImage = My.Resources.resize2
        Else
            CheckBox15.BackgroundImage = My.Resources.move
        End If
    End Sub
    'REGOLA POSIZIONE E DIMENSIONE RETTANGOLO RITAGLIO
    Private Sub Button36_MouseDown(sender As Object, e As MouseEventArgs) Handles Button36.MouseDown
        If CheckBox15.Checked = False Then
            mouseIsDown = True
            Do
                CROPMX += 2
                Thread.Sleep(25)
                Application.DoEvents()
            Loop While mouseIsDown
        Else
            mouseIsDown = True
            Do
                CROPMB += 2
                Label6.Text = CROPMB
                Label7.Text = CROPMH
                Thread.Sleep(25)
                Application.DoEvents()
            Loop While mouseIsDown
        End If
    End Sub
    Private Sub Button36_MouseUp(sender As Object, e As MouseEventArgs) Handles Button36.MouseUp
        mouseIsDown = False
        ELABORA()
    End Sub
    Private Sub Button37_MouseDown(sender As Object, e As MouseEventArgs) Handles Button37.MouseDown
        If CheckBox15.Checked = False Then
            mouseIsDown = True
            Do
                CROPMX -= 2
                Thread.Sleep(25)
                Application.DoEvents()
            Loop While mouseIsDown
        Else
            mouseIsDown = True
            Do
                CROPMB -= 2
                Label6.Text = CROPMB
                Label7.Text = CROPMH
                Thread.Sleep(25)
                Application.DoEvents()
            Loop While mouseIsDown
        End If
    End Sub
    Private Sub Button37_MouseUp(sender As Object, e As MouseEventArgs) Handles Button37.MouseUp
        mouseIsDown = False
        ELABORA()
    End Sub
    Private Sub Button34_MouseDown(sender As Object, e As MouseEventArgs) Handles Button34.MouseDown
        If CheckBox15.Checked = False Then
            mouseIsDown = True
            Do
                CROPMY += 2
                Thread.Sleep(25)
                Application.DoEvents()
            Loop While mouseIsDown
        Else
            mouseIsDown = True
            Do
                CROPMH -= 2
                Label6.Text = CROPMB
                Label7.Text = CROPMH
                Thread.Sleep(25)
                Application.DoEvents()
            Loop While mouseIsDown
        End If
    End Sub
    Private Sub Button34_MouseUp(sender As Object, e As MouseEventArgs) Handles Button34.MouseUp
        mouseIsDown = False
        ELABORA()
    End Sub
    Private Sub Button35_MouseDown(sender As Object, e As MouseEventArgs) Handles Button35.MouseDown
        If CheckBox15.Checked = False Then
            mouseIsDown = True
            Do
                CROPMY -= 2
                Thread.Sleep(25)
                Application.DoEvents()
            Loop While mouseIsDown
        Else
            mouseIsDown = True
            Do
                CROPMH += 2
                Label6.Text = CROPMB
                Label7.Text = CROPMH
                Thread.Sleep(25)
                Application.DoEvents()
            Loop While mouseIsDown
        End If
    End Sub
    Private Sub Button35_MouseUp(sender As Object, e As MouseEventArgs) Handles Button35.MouseUp
        mouseIsDown = False
        ELABORA()
    End Sub
    'ATTIVA DISATTIVA MASCHERA
    Private Sub CheckBox16_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox16.CheckedChanged
        ELABORA()
    End Sub
#End Region

#Region "ELABORA"
    ' FUNZIONI AVANZATE CONTROLLO VISIVO E RITAGLIO '
    Private Sub ELABORA()
        ELAB = True

#Region "AGGIORNA PAUSA"

        'Application.DoEvents()
        ' verifica se si modifica valore pausa in setup
        'If pause <> My.Settings.PAUSA Then
        '    Timer2.Interval = My.Settings.PAUSA '+ 500
        '    pause = My.Settings.PAUSA
        'End If
#End Region
#Region "ELABORA PICMOD"
        ' verifica se in modalita' immagini
        If picmod = True Then

            Try
                If IMAGEN IsNot Nothing Then IMAGEN.Dispose()
                IMAGEN = Image.FromFile(TextBox12.Text & "/" & ListBox8.SelectedItem)
                If original IsNot Nothing Then original.Dispose()
                original = IMAGEN.Clone
            Catch ex As Exception
            End Try


            If IMAGEN IsNot Nothing Then
                If FLPY = True Then
                    Dim Filtmiry = New Mirror(True, False)
                    IMAGEN = Filtmiry.Apply(IMAGEN)
                End If
                If FLPX = True Then
                    Dim Filtmirx = New Mirror(False, True)
                    IMAGEN = Filtmirx.Apply(IMAGEN)
                End If
                If rotate = True Then
                    Dim Filtrot = New RotateNearestNeighbor(angolo, True)
                    IMAGEN = Filtrot.Apply(IMAGEN)
                End If

                If IMAGEN.Width > IMAGEN.Height Then
                    PictureBox22.Height = Panel3.Height
                    PictureBox22.Width = PictureBox22.Height * IMAGEN.Width / IMAGEN.Height
                Else
                    PictureBox22.Width = Panel3.Width
                    PictureBox22.Height = Panel3.Width * IMAGEN.Height / IMAGEN.Width
                End If

                If ZOOMON = True Then
                    If original IsNot Nothing Then original.Dispose()
                    original = IMAGEN.Clone
                    ZOOMVER()
                    'PictureBox22.BackgroundImage = IMAGEN
                Else
                    PictureBox22.BackgroundImage = IMAGEN
                End If

            End If



        End If

#End Region
#Region "RIFERIMENTI"

        If AVA = True Then
            'Dim D As Integer = 0
            'Do While D <= 10
            '    Application.DoEvents()
            '    If OCC = 1 Then
            '        Exit Do
            '    Else
            '        D = 0
            '    End If
            'Loop

            If IMAGEN IsNot Nothing And OCC1 = 1 Then

                If rectCropArea.Width = rectCropArea.Height Then

                    TableLayoutPanel32.Enabled = False
                    TableLayoutPanel35.Enabled = False
                    CheckBox9.Visible = False
                    Button27.Visible = False
                    Button28.Visible = False

                    bl = 0
                    PictureBox19.BackgroundImage = My.Resources.blob
                    PictureBox17.BackgroundImage = My.Resources.counter
                    PictureBox18.BackgroundImage = My.Resources.aligns

                    PictureBox29.BackgroundImage = Nothing
                    PictureBox21.BackgroundImage = Nothing
                    'cambia immagini gif selezione s8 o n8
                    If RadioButton9.Checked = True Then
                        If REES = False Then
                            PictureBox21.Image = My.Resources.S8PIC
                            PictureBox29.Image = My.Resources.S8GIF
                            REES = True
                            REES2 = False
                        End If
                    Else
                        If REES2 = False Then
                            PictureBox21.Image = My.Resources.N8PIC
                            PictureBox29.Image = My.Resources.N8GIF
                            REES2 = True
                            REES = False
                        End If
                    End If

                Else

                    REES = False
                    REES2 = False
                    TableLayoutPanel32.Enabled = True
                    TableLayoutPanel35.Enabled = True
                    CheckBox9.Visible = True
                    Button27.Visible = True
                    Button28.Visible = True

                    ptbH = PictureBox22.Height
                    ptbB = PictureBox22.Width
                    imgB = PictureBox22.BackgroundImage.Width
                    imgH = PictureBox22.BackgroundImage.Height
                    PTBRCX = rectCropArea.X
                    PTBRCY = rectCropArea.Y
                    PTBRCB = rectCropArea.Width
                    PTBRCH = rectCropArea.Height
                    IMGRCX = PTBRCX * (imgB / ptbB)
                    IMGRCY = PTBRCY * (imgH / ptbH)
                    IMGRCB = PTBRCB * (imgB / ptbB)
                    IMGRCH = PTBRCH * (imgH / ptbH)

                    Dim IMGRCB_2 As Integer = IMGRCB / 3
                    Dim IMGRCY_2 As Integer = IMGRCY / 2
                    Dim IMGRCH_2 As Integer = IMGRCH * 2
                    Dim IMGRCH_3 As Integer = IMGRCH / 2

                    bit = New Bitmap(IMAGEN, IMAGEN.Width, IMAGEN.Height)
                    Dim cropBitmap As New Bitmap(IMGRCB, IMGRCH_2)
                    Dim H As Graphics = Graphics.FromImage(cropBitmap)
                    H.InterpolationMode = InterpolationMode.HighQualityBicubic
                    H.PixelOffsetMode = PixelOffsetMode.HighQuality
                    H.CompositingQuality = CompositingQuality.HighQuality
                    H.DrawImage(bit, New Rectangle(0, 0, IMGRCB, IMGRCH_2), (IMGRCX), (IMGRCY - IMGRCH_3), IMGRCB, IMGRCH_2, GraphicsUnit.Pixel)

#End Region
#Region "FILTRI"
                    ' applica filtri estrazione blob
                    Dim sample As New Bitmap(IMGRCB_2, IMGRCH_2)
                    Dim J As Graphics = Graphics.FromImage(sample)
                    J.InterpolationMode = InterpolationMode.HighQualityBicubic
                    J.PixelOffsetMode = PixelOffsetMode.HighQuality
                    J.CompositingQuality = CompositingQuality.HighQuality
                    J.DrawImage(bit, New Rectangle(0, 0, IMGRCB_2, IMGRCH_2), (IMGRCX), (IMGRCY - IMGRCH_3), IMGRCB_2, IMGRCH_2, GraphicsUnit.Pixel)

                    Dim gray As Grayscale = New Grayscale(0.2125, 0.7154, 0.0721)
                    Dim filterInvert As Invert = New Invert()
                    Dim FILTRI = New FiltersSequence(New BrightnessCorrection(esposizione),
                             New ContrastCorrection(contrasto),
                             New SISThreshold()) ' soglia bianco e nero
                    Dim filter4 As Erosion = New Erosion()
                    Dim filter5 As Erosion3x3 = New Erosion3x3()
                    Dim gray1 As Bitmap = gray.Apply(sample.Clone)
                    ' sample.Dispose()
                    Dim gray2 As Bitmap
                    Dim gray3 As Bitmap
                    Dim gray4 As Bitmap
                    Dim gray5 As Bitmap
                    If CheckBox10.Checked = True Then
                        'filtro inverti
                        gray2 = filterInvert.Apply(gray1)
                        'filtro erosion
                        If CheckBox12.Checked = True Then
                            gray3 = filter4.Apply(gray2)
                        Else
                            gray3 = gray2
                        End If
                        'filtro erosion3x3
                        If CheckBox11.Checked = True Then
                            gray4 = filter5.Apply(gray3)
                        Else
                            gray4 = gray3
                        End If
                        gray5 = FILTRI.Apply(gray4)
                    Else
                        'filtro erosion
                        If CheckBox12.Checked = True Then
                            gray2 = filter4.Apply(gray1)
                        Else
                            gray2 = gray1
                        End If
                        'filtro erosion3x3
                        If CheckBox11.Checked = True Then
                            gray3 = filter5.Apply(gray2)
                        Else
                            gray3 = gray2
                        End If
                        gray5 = FILTRI.Apply(gray3)
                    End If
                    Dim grayImage As Bitmap = gray5

#End Region
#Region "ESTRAI BLOB"
                    ' elaborazione blob
                    ' dimensione minima blob
                    Dim blobCounter As BlobCounter = New BlobCounter With {
                        .FilterBlobs = True,
                        .MinWidth = grayImage.Width / 3,
                        .MinHeight = grayImage.Height / 3
                    }
                    blobCounter.ProcessImage(grayImage)
                    Dim blobs As Blob() = blobCounter.GetObjectsInformation()

                    Dim DRW As Bitmap = New Bitmap(cropBitmap.Width, cropBitmap.Height)
                    Dim g As Graphics = Graphics.FromImage(DRW)
                    Dim f As Graphics = Graphics.FromImage(DRW)
                    Dim v As Graphics = Graphics.FromImage(DRW)
                    Dim o As Graphics = Graphics.FromImage(DRW)
                    Dim pic2 As Bitmap = New Bitmap(grayImage.Width, grayImage.Height)

                    'riferimento rettangolo controllo visivo
                    Dim rectangle2 As Rectangle
                    If CheckBox13.Checked = True And CheckBox14.Checked = False Then
                        rectangle2 = New Rectangle(0, IMGRCH / 2, IMGRCB, 1)
                    End If
                    If CheckBox13.Checked = True And CheckBox14.Checked = True Or CheckBox13.Checked = False And CheckBox14.Checked = False Then
                        rectangle2 = New Rectangle(0, IMGRCH, IMGRCB, 1)
                    End If
                    If CheckBox13.Checked = False And CheckBox14.Checked = True Then
                        rectangle2 = New Rectangle(0, IMGRCH + (IMGRCH / 2), IMGRCB, 1)
                    End If

                    Dim rectangle3 As Rectangle
                    For Each blob As Blob In blobs
                        Dim edgePoints As List(Of IntPoint) = blobCounter.GetBlobsEdgePoints(blob)
                        For Each point As IntPoint In edgePoints
                            ' disegna sagoma blobs 
                            pic2.SetPixel(point.X, point.Y, Color.Red)
                        Next

                        BLX = blob.Rectangle.X
                        BLY = blob.Rectangle.Y
                        BLB = blob.Rectangle.Height * 0.91 / 1.14 'blob.Rectangle.Width
                        BLH = blob.Rectangle.Height
                        BLBR = blob.Rectangle.Width
                        BLCX = BLX + (BLB / 2)
                        BLCY = BLY + (BLH / 2)
                        'disegna rettangolo blob
                        Using drawLine As New Pen(Color.Yellow, 6)
                            drawLine.DashStyle = DashStyle.Dash
                            g.DrawRectangle(drawLine, BLX, BLY, BLB, BLH)
                        End Using




                    Next

#End Region
#Region "CONTROLLO VISIVO, CONTEGGIO, VERIFICA POSIZIONE E SPOSTAMENTO"
                    ' disegna angoli di riferimento rettangolo ritaglio 
                    If CheckBox7.Checked = True Then
                        If CheckBox13.Checked = True And CheckBox14.Checked = False Then
                            If CheckBox6.Checked <> True Then
                                Dim r As New Rectangle(BLX + 5, BLY + 5, 50, 50)
                                o.DrawImage(My.Resources.ALTR, r)
                            End If
                        End If
                        If CheckBox13.Checked = False And CheckBox14.Checked = True Then
                            If CheckBox6.Checked <> True Then
                                Dim r As New Rectangle(BLX + 5, BLY - 55 + BLH, 50, 50)
                                o.DrawImage(My.Resources.BASSR, r)
                            End If
                        End If
                    End If

                    Dim pic3 As Bitmap = New Bitmap(bit.Width, bit.Height)

                    ' disegna rettangolo blob in picturebox
                    If CheckBox6.Checked = True Then
                        If CheckBox7.Checked = True Or CheckBox6.Checked = True Then
                            Dim k As Graphics = Graphics.FromImage(pic3)


                            'k.FillRectangle(Brushes.Yellow, IMGRCX + BLX, IMGRCY + BLY - IMGRCH_3, BLB, BLH)

                            ' disegna rettangolo blob in picturebox
                            Dim l As New Rectangle(IMGRCX + BLX, IMGRCY + BLY - IMGRCH_3, BLB, BLH)
                            k.DrawImage(My.Resources.S8NOC, l)

                            ' disegna angoli di riferimento in picturebox
                            If CheckBox13.Checked = True And CheckBox14.Checked = False Then
                                Dim r As New Rectangle(IMGRCX + BLX + 5, IMGRCY + BLY - IMGRCH_3 + 5, 50, 50)
                                k.DrawImage(My.Resources.ALTR, r)
                            End If
                            If CheckBox13.Checked = False And CheckBox14.Checked = True Then
                                Dim r As New Rectangle(IMGRCX + BLX + 5, IMGRCY + BLY + BLH - IMGRCH_3 - 55, 50, 50)
                                k.DrawImage(My.Resources.BASSR, r)
                            End If
                            k.Dispose()
                        End If
                    End If
                    ' disegna rettangolo ritaglio e maschera
                    If CheckBox7.Checked = True Then
                        Dim u As Graphics = Graphics.FromImage(pic3)
                        Dim z1 As Graphics = Graphics.FromImage(pic3)
                        Dim z2 As Graphics = Graphics.FromImage(pic3)
                        Dim z3 As Graphics = Graphics.FromImage(pic3)
                        Dim z4 As Graphics = Graphics.FromImage(pic3)
                        Dim z5 As Graphics = Graphics.FromImage(pic3)
                        If CheckBox13.Checked = True And CheckBox14.Checked = False Then
                            u.DrawRectangle(New Pen(Color.Red, 8), IMGRCX + BLX + CROPMX, IMGRCY + BLY - CROPMY - IMGRCH_3, CROPMB, CROPMH)
                            If CheckBox16.Checked = True Then
                                z1.FillRectangle(Brushes.Black, 0, 0, IMGRCX + BLX + CROPMX, imgH)
                                z2.FillRectangle(Brushes.Black, IMGRCX + BLX + CROPMX, -IMGRCH_3, CROPMB, IMGRCY - CROPMY + BLY)
                                z3.FillRectangle(Brushes.Black, IMGRCX + BLX + CROPMX + CROPMB, 0, imgB - CROPMB - CROPMX, imgH)
                                z4.FillRectangle(Brushes.Black, IMGRCX + BLX + CROPMX, IMGRCY + BLY - CROPMY + CROPMH - IMGRCH_3, CROPMB, IMGRCY + BLY - CROPMY + CROPMH)

                                'z5.FillRectangle(Brushes.Yellow, IMGRCX + BLX, IMGRCY + BLY - IMGRCH_3, BLB, BLH)
                                Dim l As New Rectangle(IMGRCX + BLX, IMGRCY + BLY - IMGRCH_3, BLB, BLH)
                                z5.DrawImage(My.Resources.S8NOC, l)
                            End If
                        End If
                        If CheckBox13.Checked = True And CheckBox14.Checked = True Or CheckBox13.Checked = False And CheckBox14.Checked = False Then
                            u.DrawRectangle(New Pen(Color.Red, 8), IMGRCX + BLX + CROPMX, IMGRCY + BLY + CInt(BLH / 2) - CROPMY - IMGRCH_3, CROPMB, CROPMH)
                            If CheckBox16.Checked = True Then
                                z1.FillRectangle(Brushes.Black, 0, 0, IMGRCX + BLX + CROPMX, imgH)
                                z2.FillRectangle(Brushes.Black, IMGRCX + BLX + CROPMX, -IMGRCH_3, CROPMB, IMGRCY + CInt(BLH / 2) - CROPMY + BLY)
                                z3.FillRectangle(Brushes.Black, IMGRCX + BLX + CROPMX + CROPMB, 0, imgB - IMGRCX - BLX - CROPMB - CROPMX, imgH)
                                z4.FillRectangle(Brushes.Black, IMGRCX + BLX + CROPMX, IMGRCY + BLY + CInt(BLH / 2) - CROPMY + CROPMH - IMGRCH_3, CROPMB, imgH - IMGRCY + BLY + CInt(BLH / 2) - CROPMY + CROPMH)

                                'z5.FillRectangle(Brushes.Yellow, IMGRCX + BLX, IMGRCY + BLY - IMGRCH_3, BLB, BLH)
                                Dim l As New Rectangle(IMGRCX + BLX, IMGRCY + BLY - IMGRCH_3, BLB, BLH)
                                z5.DrawImage(My.Resources.S8NOC, l)
                            End If
                        End If
                        If CheckBox13.Checked = False And CheckBox14.Checked = True Then
                            u.DrawRectangle(New Pen(Color.Red, 8), IMGRCX + BLX + CROPMX, IMGRCY + BLY + BLH - CROPMY - IMGRCH_3, CROPMB, CROPMH)
                            If CheckBox16.Checked = True Then
                                z1.FillRectangle(Brushes.Black, 0, 0, IMGRCX + BLX + CROPMX, imgH)
                                z2.FillRectangle(Brushes.Black, IMGRCX + BLX + CROPMX, -IMGRCH_3, CROPMB, IMGRCY + CInt(BLH) - CROPMY + BLY)
                                z3.FillRectangle(Brushes.Black, IMGRCX + BLX + CROPMX + CROPMB, 0, imgB - IMGRCX - BLX - CROPMB - CROPMX, imgH)
                                z4.FillRectangle(Brushes.Black, IMGRCX + BLX + CROPMX, IMGRCY + BLY + CInt(BLH) - CROPMY + CROPMH - IMGRCH_3, CROPMB, imgH - IMGRCY + BLY + BLH - CROPMY + CROPMH)

                                'z5.FillRectangle(Brushes.Yellow, IMGRCX + BLX, IMGRCY + BLY - IMGRCH_3, BLB, BLH)
                                Dim l As New Rectangle(IMGRCX + BLX, IMGRCY + BLY - IMGRCH_3, BLB, BLH)
                                z5.DrawImage(My.Resources.S8NOC, l)
                            End If
                        End If
                        u.Dispose()
                        z1.Dispose()
                        z2.Dispose()
                        z3.Dispose()
                        z4.Dispose()
                        z5.Dispose()
                    End If
                    ' disegna rettangolo controllo visivo
                    If CheckBox6.Checked = True Then
                        v.FillRectangle(Brushes.Red, rectangle3)
                    End If
                    'riferimento rettangolo controllo visivo
                    If CheckBox13.Checked = True And CheckBox14.Checked = False Then
                        rectangle3 = New Rectangle(BLCX - RB / 2, BLY - (RH + BLH) / 2, RB, RH + BLH)
                    End If
                    If CheckBox13.Checked = True And CheckBox14.Checked = True Or CheckBox13.Checked = False And CheckBox14.Checked = False Then
                        rectangle3 = New Rectangle(BLCX - RB / 2, BLCY - (RH + BLH) / 2, RB, RH + BLH)
                    End If
                    If CheckBox13.Checked = False And CheckBox14.Checked = True Then
                        rectangle3 = New Rectangle(BLCX - RB / 2, BLY + BLH - (RH + BLH) / 2, RB, RH + BLH)
                    End If
                    ' verifica blob
                    If CheckBox6.Checked = True Then
                        Application.DoEvents()
                        ' disegna linea conteggio blob
                        'f.DrawLine(New Pen(Color.Red, 8), 0, CInt(IMGRCH), CInt(IMGRCB), CInt(IMGRCH))



                        If CheckBox13.Checked = True And CheckBox14.Checked = False Then
                            f.DrawLine(New Pen(Color.Red, 8), 0, CInt(IMGRCH / 2), CInt(IMGRCB), CInt(IMGRCH / 2))
                        End If
                        If CheckBox13.Checked = True And CheckBox14.Checked = True Or CheckBox13.Checked = False And CheckBox14.Checked = False Then
                            f.DrawLine(New Pen(Color.Red, 8), 0, CInt(IMGRCH), CInt(IMGRCB), CInt(IMGRCH))
                        End If
                        If CheckBox13.Checked = False And CheckBox14.Checked = True Then
                            f.DrawLine(New Pen(Color.Red, 8), 0, CInt(IMGRCH + IMGRCH / 2), CInt(IMGRCB), CInt(IMGRCH + IMGRCH / 2))
                        End If

                        ' verifica se c'è un solo blob  
                        bl = Double.Parse(blobs.Length)
                        If bl <> 1 Then
                            PictureBox19.BackgroundImage = My.Resources.blobred
                            ToolTip1.SetToolTip(PictureBox19, bl & " Blob")
                            CONTROL1 = False
                        Else
                            PictureBox19.BackgroundImage = My.Resources.blobgreen
                            ToolTip1.SetToolTip(PictureBox19, bl & " Blob")
                            CONTROL1 = True
                        End If
                        ' verifica posizione
                        If rectangle2.IntersectsWith(rectangle3) Then
                            v.FillRectangle(Brushes.Green, rectangle3)
                            f.DrawRectangle(New Pen(Color.Green, 10), rectangle2)
                            PictureBox18.BackgroundImage = My.Resources.alignsgreen
                            CONTROL = True
                            If picmod = False Then
                                'conteggio blob
                                If az = False Then
                                    BLXV = BLX
                                    BLYV = BLY
                                    BLYHV = BLY + BLH
                                    az = True
                                Else
                                    'verifica spostamento blob
                                    If My.Settings.foto_in = True Then
                                        If BLXV <> BLX Or BLXV <> BLY Or BLYHV <> BLY + BLH Then
                                            PictureBox17.BackgroundImage = My.Resources.countergreen
                                            CONTROL2 = True
                                            BLXV = BLX
                                            BLYV = BLY
                                            BLYHV = BLY + BLH
                                            n += 1
                                            ToolTip1.SetToolTip(PictureBox17, n & " Fotogrammi OK" & vbCrLf & "Doppio Clik Azzera")
                                        Else
                                            PictureBox17.BackgroundImage = My.Resources.counterred
                                            CONTROL2 = False
                                        End If
                                    End If
                                End If
                            End If
                        Else
                            CONTROL = False
                            v.FillRectangle(Brushes.Red, rectangle3)
                            PictureBox18.BackgroundImage = My.Resources.alignsred
                            H.Dispose()
                            g.Dispose()
                            o.Dispose()
                            f.Dispose()
                        End If
                    Else
                        CONTROL = True
                        CONTROL1 = True
                        CONTROL2 = True
                        PictureBox19.BackgroundImage = My.Resources.blob
                        PictureBox17.BackgroundImage = My.Resources.counter
                        PictureBox18.BackgroundImage = My.Resources.aligns
                    End If

                    If PictureBox29.Image IsNot Nothing Then PictureBox29.Image.Dispose()
                    If PictureBox29.BackgroundImage IsNot Nothing Then PictureBox29.Image.Dispose()
                    If PictureBox21.Image IsNot Nothing Then PictureBox21.Image.Dispose()
                    If PictureBox21.BackgroundImage IsNot Nothing Then PictureBox21.Image.Dispose()
                    If PictureBox22.Image IsNot Nothing Then PictureBox22.Image.Dispose()

                    PictureBox29.BackgroundImage = cropBitmap
                    PictureBox29.Image = DRW
                    PictureBox22.Image = pic3

                    Dim grayImage_2 As New Bitmap(BLBR, BLH)
                    Dim GI As Graphics = Graphics.FromImage(grayImage_2)
                    GI.InterpolationMode = InterpolationMode.HighQualityBicubic
                    GI.PixelOffsetMode = PixelOffsetMode.HighQuality
                    GI.CompositingQuality = CompositingQuality.HighQuality
                    GI.DrawImage(grayImage, New Rectangle(0, 0, BLBR, BLH), (BLX), BLY, BLBR, BLH, GraphicsUnit.Pixel)

                    PictureBox21.BackgroundImage = grayImage_2

                    Dim pic2_2 As New Bitmap(BLBR, BLH)
                    Dim p2 As Graphics = Graphics.FromImage(pic2_2)
                    p2.InterpolationMode = Drawing2D.InterpolationMode.HighQualityBicubic
                    p2.PixelOffsetMode = Drawing2D.PixelOffsetMode.HighQuality
                    p2.CompositingQuality = Drawing2D.CompositingQuality.HighQuality
                    p2.DrawImage(pic2, New Rectangle(0, 0, BLBR, BLH), BLX, BLY, BLBR, BLH, GraphicsUnit.Pixel)

                    PictureBox21.Image = pic2_2
                End If
            End If
        End If
        ELAB = False
    End Sub
#End Region
#End Region

#Region "SALVA"
    Private Sub SALVA()
        ' funzione salva immagine con o senza ritaglio
        If IMAGEN IsNot Nothing Then
            If My.Settings.foto_in = True And My.Settings.foto_in2 = False Then
                If CONTROL = True And CONTROL1 = True And CONTROL2 = True Then
                    If CheckBox7.Checked = True Then
                        Dim croBitmap As New Bitmap(CROPMB, CROPMH)
                        Dim p As Graphics = Graphics.FromImage(croBitmap)
                        p.InterpolationMode = InterpolationMode.HighQualityBicubic
                        p.PixelOffsetMode = PixelOffsetMode.HighQuality
                        p.CompositingQuality = CompositingQuality.HighQuality
                        If CheckBox13.Checked = True And CheckBox14.Checked = False Then
                            p.DrawImage(IMAGEN, New Rectangle(0, 0, CROPMB, CROPMH),
                                                                        IMGRCX + BLX + CROPMX, IMGRCY + BLY - CROPMY,
                                                                        CROPMB, CROPMH, GraphicsUnit.Pixel)
                        End If
                        If CheckBox13.Checked = True And CheckBox14.Checked = True Or CheckBox13.Checked = False And CheckBox14.Checked = False Then
                            p.DrawImage(IMAGEN, New Rectangle(0, 0, CROPMB, CROPMH),
                                                                        IMGRCX + BLX + CROPMX,
                                                                        IMGRCY + BLY + CInt(BLH / 2) - CROPMY - CInt(IMGRCY / 2),
                                                                        CROPMB, CROPMH, GraphicsUnit.Pixel)
                        End If
                        If CheckBox13.Checked = False And CheckBox14.Checked = True Then
                            p.DrawImage(IMAGEN, New Rectangle(0, 0, CROPMB, CROPMH),
                                                                        IMGRCX + BLX + CROPMX, IMGRCY + BLY + BLH - CROPMY,
                                                                        CROPMB, CROPMH, GraphicsUnit.Pixel)
                        End If
                        Select Case ComboBox3.Text
                            Case "jpg"
                                croBitmap.Save(no, System.Drawing.Imaging.ImageFormat.Jpeg)
                            Case "bmp"
                                croBitmap.Save(no, System.Drawing.Imaging.ImageFormat.Bmp)
                            Case "png"
                                croBitmap.Save(no, System.Drawing.Imaging.ImageFormat.Png)
                            Case "gif"
                                croBitmap.Save(no, System.Drawing.Imaging.ImageFormat.Gif)
                            Case "tiff"
                                croBitmap.Save(no, System.Drawing.Imaging.ImageFormat.Tiff)
                        End Select
                    Else
                        Select Case ComboBox3.Text
                            Case "jpg"
                                IMAGEN.Save(no, System.Drawing.Imaging.ImageFormat.Jpeg)
                            Case "bmp"
                                IMAGEN.Save(no, System.Drawing.Imaging.ImageFormat.Bmp)
                            Case "png"
                                IMAGEN.Save(no, System.Drawing.Imaging.ImageFormat.Png)
                            Case "gif"
                                IMAGEN.Save(no, System.Drawing.Imaging.ImageFormat.Gif)
                            Case "tiff"
                                IMAGEN.Save(no, System.Drawing.Imaging.ImageFormat.Tiff)
                        End Select
                    End If

                    'If My.Settings.view_on = True Then
                    '    My.Settings.foto_in2 = True
                    'Else
                    '    My.Settings.foto_in2 = False
                    'End If





                    Me.Button15.PerformClick()

                    My.Settings.foto_in = False
                    My.Settings.FERMA = False



                    'Dim K As Integer = 0
                    'Do While K <= 10
                    '    If My.Settings.foto_in = False And My.Settings.foto_in2 = False Then
                    '        OCC = 0
                    '        Exit Do
                    '    Else
                    '        K = 0
                    '    End If
                    'Loop

                Else
                    My.Settings.foto_in = False
                    My.Settings.foto_in2 = False
                    My.Settings.FERMA = True
                End If
            End If
        End If
    End Sub
#End Region

#Region "COMANDI ZOOM"
    'apre mod zoom
    Private Sub Button21_Click(sender As Object, e As EventArgs) Handles Button21.Click
        ZOOMON = True
        Button21.Visible = False
        Panel49.Visible = True
        ZOOMVER()
    End Sub
    'chiude mod zoom
    Private Sub Button77_Click(sender As Object, e As EventArgs) Handles Button77.Click
        If Panel50.Visible = True Then
            Panel50.Visible = False
        End If
        Panel49.Visible = False
        Button21.Visible = True
        Button93.PerformClick()
        PictureBox22.BackgroundImage = Nothing
        IMAGEN = original.Clone
        PictureBox22.BackgroundImage = IMAGEN
        ZOOMON = False

    End Sub
    'ZOOMVER 
    Private Sub ZOOMVER()
        If IMAGEN IsNot Nothing And ZOOMON = True Then
            If original IsNot Nothing And NumericUpDown9.Value <> 1 Then
                'IMAGENZOOM = original.Clone
                'IMAGEN.Dispose()
                Dim BAS As Integer = original.Width / NumericUpDown9.Value
                Dim ALT As Integer = original.Height / NumericUpDown9.Value
                Dim BAS2 As Integer = original.Width * 0.5
                Dim ALT2 As Integer = original.Height * 0.5
                Dim BAS3 As Integer = BAS * 0.5
                Dim ALT3 As Integer = ALT * 0.5
                Dim ZOOMBitmap As New Bitmap(BAS, ALT)
                Dim Z As Graphics = Graphics.FromImage(ZOOMBitmap)
                Z.InterpolationMode = InterpolationMode.HighQualityBicubic
                Z.PixelOffsetMode = PixelOffsetMode.HighQuality
                Z.CompositingQuality = CompositingQuality.HighQuality
                Z.DrawImage(original, New Rectangle(0, 0, BAS, ALT),
                                         -(BAS3 - BAS2) + ZOOMX, -(ALT3 - ALT2) + ZOOMY,
                                           BAS, ALT, GraphicsUnit.Pixel)
                Z.Dispose()
                IMAGEN = New Bitmap(ZOOMBitmap, ZOOMBitmap.Width, ZOOMBitmap.Height)
                ZOOMBitmap.Dispose()
                'IMAGENZOOM.Dispose()

                PictureBox22.BackgroundImage = IMAGEN
            Else
                PictureBox22.BackgroundImage = IMAGEN
            End If
        End If
    End Sub
    'apre finestra move zoom
    Private Sub Button87_Click(sender As Object, e As EventArgs) Handles Button87.Click
        Panel50.Location = New System.Drawing.Point(PictureBox22.Width / 2 - Panel50.Width / 2, PictureBox22.Height / 2 - Panel50.Height / 2)
        Panel50.Visible = True
    End Sub
    'chiude finestra move zoom
    Private Sub Button88_Click(sender As Object, e As EventArgs) Handles Button88.Click
        Panel50.Visible = False
    End Sub
    ' SELEZIONE VALORE ZOOM 
    Private Sub NumericUpDown9_ValueChanged(sender As Object, e As EventArgs) Handles NumericUpDown9.ValueChanged
        If IMAGEN IsNot Nothing And ZOOMON = True Then
            ZOOMVER()
            ELABORA()
        End If
    End Sub
    ' BOTTONI MUOVI IMMAGINE ZOOM
    Private Sub Button93_Click(sender As Object, e As EventArgs) Handles Button93.Click
        ZOOMX = 0
        ZOOMY = 0
        TextBox10.Text = ZOOMX
        TextBox11.Text = ZOOMY
        ZOOMVER()
    End Sub
    Private Sub Button89_Click(sender As Object, e As EventArgs) Handles Button89.Click
        ZOOMY -= 10
        TextBox11.Text = ZOOMY
        ZOOMVER()
    End Sub
    Private Sub Button90_Click(sender As Object, e As EventArgs) Handles Button90.Click
        ZOOMY += 10
        TextBox11.Text = ZOOMY
        ZOOMVER()
    End Sub
    Private Sub Button91_Click(sender As Object, e As EventArgs) Handles Button91.Click
        ZOOMX += 10
        TextBox10.Text = ZOOMX
        ZOOMVER()
    End Sub
    Private Sub Button92_Click(sender As Object, e As EventArgs) Handles Button92.Click
        ZOOMX -= 10
        TextBox10.Text = ZOOMX
        ZOOMVER()
    End Sub




    'MUOVI IMMAGINE ZOOM INSERIMENTO TESTO
    Private Sub TextBox10_KeyPress(ByVal sender As Object, ByVal e As KeyPressEventArgs) Handles TextBox10.KeyPress
        If Asc(e.KeyChar) <> 8 And Asc(e.KeyChar) <> 45 Then
            If Asc(e.KeyChar) < 48 Or Asc(e.KeyChar) > 57 Then
                e.Handled = True
            End If
        End If
    End Sub

    Private Sub TextBox10_LostFocus(sender As Object, e As EventArgs) Handles TextBox10.LostFocus
        ZOOMX = TextBox10.Text
        ZOOMVER()
    End Sub

    Private Sub TextBox11_KeyPress(ByVal sender As Object, ByVal e As KeyPressEventArgs) Handles TextBox11.KeyPress
        If Asc(e.KeyChar) <> 8 And Asc(e.KeyChar) <> 45 Then
            If Asc(e.KeyChar) < 48 Or Asc(e.KeyChar) > 57 Then
                e.Handled = True
            End If
        End If
    End Sub
    Private Sub TextBox11_LostFocus(sender As Object, e As EventArgs) Handles TextBox11.LostFocus
        ZOOMY = TextBox11.Text
        ZOOMVER()
    End Sub
    'sposta pannello move zoom con il mouse 
    Private Sub TableLayoutPanel83_MouseDown(sender As Object, e As System.Windows.Forms.MouseEventArgs) Handles TableLayoutPanel83.MouseDown
        isDragging = True
        currentX = e.X
        currentY = e.Y
    End Sub
    Private Sub TableLayoutPanel83_MouseMove(sender As Object, e As System.Windows.Forms.MouseEventArgs) Handles TableLayoutPanel83.MouseMove
        If isDragging Then
            Panel50.Top += (e.Y - currentY)
            Panel50.Left += (e.X - currentX)
        End If
    End Sub
    Private Sub TableLayoutPanel83_MouseUp(sender As Object, e As System.Windows.Forms.MouseEventArgs) Handles TableLayoutPanel83.MouseUp
        isDragging = False
    End Sub
#End Region

#Region "FUNZIONALITA' VIEWER"

    Private Sub ListBox1_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles ListBox1.MouseDown
        If ListBox1.Items.Count = Nothing Then
            Exit Sub
        End If
        If e.Button = MouseButtons.Right Then
            ListBox1.SelectedIndex = ListBox1.IndexFromPoint(e.X, e.Y)
            Dim MyimageWxH
            Try
                Dim Myimg As System.Drawing.Image = System.Drawing.Image.FromFile(TextBox12.Text & "\" & ListBox1.SelectedItem)
                Dim MyImageWidth = PictureBox20.Image.Width
                Dim MyImageHeight = PictureBox20.Image.Height
                MyimageWxH = MyImageWidth & " X " & MyImageHeight
            Catch
                MyimageWxH = "Not Supported"
            End Try
            Dim Myimagepropety = FileLen(TextBox12.Text & "\" & ListBox1.SelectedItem)
            Dim Myimagedate As DateTime = File.GetCreationTime(TextBox12.Text & "\" & ListBox1.SelectedItem)
            MsgBox("Informazioni:" & vbCrLf & vbCrLf & TextBox12.Text & "\" & ListBox1.SelectedItem & vbCrLf & vbCrLf & " Data Creazione:  " & Myimagedate & vbCrLf & vbCrLf & " Formato:  " & MyimageWxH & vbCrLf & vbCrLf & " Dimensione:  " & System.Math.Round(Myimagepropety / 1000, 1) & " KB", MessageBoxIcon.Information)
        End If
    End Sub
    ' APRI O SELEZIONA VIEWER ESTERNO
    Private Sub Button13_Click(sender As Object, e As EventArgs) Handles Button13.Click
        If TextBox12.Text <> Nothing Then
            Process.Start("explorer.exe", TextBox12.Text)
        End If
    End Sub

    ' CANCELLA IMMAGINE DAL DISCO
    Private Sub Button16_Click(sender As Object, e As EventArgs) Handles Button16.Click
        If ListBox1.Items.Count = Nothing Then
            Exit Sub
        End If
        If MsgBox("Eliminare il File?" & vbCrLf & vbCrLf & "...\" & ListBox1.SelectedItem, MsgBoxStyle.YesNo, ) = MsgBoxResult.Yes Then
            Dim DA As String = (TextBox12.Text & "\" & ListBox1.SelectedItem)
            PictureBox20.Image = Nothing
            Dim li As Integer
            For li = ListBox1.SelectedIndices.Count - 1 To 0 Step -1

                ListBox1.Items.RemoveAt(ListBox1.SelectedIndices(li))
            Next
            My.Computer.FileSystem.DeleteFile(DA)
            DA = 0
            If TextBox12.Text <> Nothing Then
                Dim files() As String = Directory.GetFiles(TextBox12.Text)
                Dim pat As String
                pat = TextBox12.Text
                Dim filess() As String
                If Directory.Exists(pat) Then
                    filess = Directory.GetFiles(pat)
                    Label32.Text = filess.Length & " Files "
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

    ' AGGIORNA CONTENUTO CARTELLA
    Private Sub Button15_Click(sender As Object, e As EventArgs) Handles Button15.Click
        TextBox12.Text = My.Settings.cart_lav
        ' PictureBox20.Image = My.Resources.ANTEPRIMA
        Label32.Text = 0 & " File "
        If TextBox12.Text <> "" Then
            Dim files() As String = Directory.GetFiles(TextBox12.Text)
            ListBox1.Items.Clear()
            For Each file As String In files
                ListBox1.Items.Add(Path.GetFileName(file))
            Next
            Dim pat As String
            pat = TextBox12.Text
            Dim filess() As String
            If Directory.Exists(pat) Then
                filess = Directory.GetFiles(pat)
                Label32.Text = filess.Length & " Files "
                If ListBox1.Items.Count.ToString() = 0 Then
                    Exit Sub
                Else
                    ListBox1.SetSelected(ListBox1.Items.Count.ToString() - 1, True)
                End If
            End If
        End If
    End Sub

    ' INDIETRO UN FOTOGRAMMA
    Private Sub Button17_Click(sender As Object, e As EventArgs) Handles Button17.Click

        If ListBox1.Items.Count = Nothing Then
            Exit Sub
        End If
        A1 = ListBox1.FindString(ListBox1.SelectedItem.ToString())
        If A1 >= 1 Then
            A1 -= 1
            ListBox1.SetSelected(A1, True)
        End If
    End Sub

    ' AVANTI UN FOTOGRAMMA
    Private Sub Button18_Click(sender As Object, e As EventArgs) Handles Button18.Click

        If ListBox1.Items.Count = Nothing Then
            Exit Sub
        End If
        A1 = ListBox1.FindString(ListBox1.SelectedItem.ToString())
        If A1 <= ListBox1.Items.Count.ToString() - 2 Then
            A1 += 1
            ListBox1.SetSelected(A1, True)
        End If
    End Sub

    ' CAMBIA SELEZIONE LISTA FILES CARTELLA LAVORO
    Private Sub ListBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ListBox1.SelectedIndexChanged
        Try
            PictureBox20.Load(TextBox12.Text & "/" & ListBox1.SelectedItem)

        Catch ex As Exception
            'PictureBox20.Image = My.Resources.ANTEPRIMA
        End Try
    End Sub
#End Region

#Region "COMANDI VARI AVANZATO"
    'SALVA IMPOSTAZIONI AVANZATO
    Private Sub Button27_Click(sender As Object, e As EventArgs) Handles Button27.Click
        If MsgBox("Salvare Impostazioni?", MsgBoxStyle.YesNo, ) = MsgBoxResult.Yes Then
            My.Settings.CMX = CROPMX
            My.Settings.CMY = CROPMY
            My.Settings.CMB = CROPMB
            My.Settings.CMH = CROPMH
            'My.Settings.RCB = RB
            My.Settings.RCH = RH
            My.Settings.RIFA = CheckBox13.Checked
            My.Settings.RIFB = CheckBox14.Checked
            My.Settings.ER = CheckBox12.Checked
            My.Settings.ER3 = CheckBox11.Checked
            My.Settings.INV = CheckBox10.Checked
            My.Settings.ESPO = TrackBar3.Value
            My.Settings.CONT = TrackBar1.Value
            My.Settings.RSX = rectCropArea.X
            My.Settings.RSY = rectCropArea.Y
            My.Settings.RSB = rectCropArea.Width
            My.Settings.RSH = rectCropArea.Height
        End If
    End Sub
    'CARICA IMPOSTAZIONI AVANZATO
    Private Sub Button28_Click(sender As Object, e As EventArgs) Handles Button28.Click, Button57.Click
        If MsgBox("Caricare Impostazioni?", MsgBoxStyle.YesNo, ) = MsgBoxResult.Yes Then
            CROPMX = My.Settings.CMX
            CROPMY = My.Settings.CMY
            CROPMB = My.Settings.CMB
            CROPMH = My.Settings.CMH
            'RB = My.Settings.RCB
            RH = My.Settings.RCH
            CheckBox13.Checked = My.Settings.RIFA
            CheckBox14.Checked = My.Settings.RIFB
            CheckBox12.Checked = My.Settings.ER
            CheckBox11.Checked = My.Settings.ER3
            CheckBox10.Checked = My.Settings.INV
            TrackBar3.Value = My.Settings.ESPO
            TrackBar1.Value = My.Settings.CONT
            rectCropArea.X = My.Settings.RSX
            rectCropArea.Y = My.Settings.RSY
            rectCropArea.Width = My.Settings.RSB
            rectCropArea.Height = My.Settings.RSH
            ELABORA()
        End If
    End Sub

    'SPECCHIA IN VERTICALE
    Private Sub CheckBox4_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox4.CheckedChanged

        If FLPY = True Then
            IMAGEN.RotateFlip(RotateFlipType.RotateNoneFlipY)
            original.RotateFlip(RotateFlipType.RotateNoneFlipY)
            'If PictureBox21.BackgroundImage IsNot Nothing Then PictureBox21.BackgroundImage.Dispose()
            PictureBox22.BackgroundImage = Nothing
            PictureBox22.BackgroundImage = IMAGEN
            FLPY = False
        Else

            IMAGEN.RotateFlip(RotateFlipType.RotateNoneFlipY)
            original.RotateFlip(RotateFlipType.RotateNoneFlipY)
            PictureBox22.BackgroundImage = Nothing
            PictureBox22.BackgroundImage = IMAGEN
            FLPY = True
        End If
        If AVA = True And rectCropArea <> Nothing Then
            ELABORA()
        End If
    End Sub
    'SPECCHIA IN ORIZZONTALE
    Private Sub CheckBox19_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox19.CheckedChanged

        If FLPX = True Then
            IMAGEN.RotateFlip(RotateFlipType.RotateNoneFlipX)
            original.RotateFlip(RotateFlipType.RotateNoneFlipX)
            'If PictureBox21.BackgroundImage IsNot Nothing Then PictureBox21.BackgroundImage.Dispose()
            PictureBox22.Image = Nothing
            PictureBox22.BackgroundImage = IMAGEN
            FLPX = False
        Else
            IMAGEN.RotateFlip(RotateFlipType.RotateNoneFlipX)
            original.RotateFlip(RotateFlipType.RotateNoneFlipX)
            PictureBox22.Image = Nothing
            PictureBox22.BackgroundImage = IMAGEN
            FLPX = True
        End If
        If AVA = True And rectCropArea <> Nothing Then
            ELABORA()
        End If
    End Sub
    'APRI/CHIUDI AVANZATO 
    Private Sub CheckBox8_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox8.CheckedChanged

        'If Me.WindowState <> FormWindowState.Normal Then
        '    Me.WindowState = FormWindowState.Normal
        'End If
        If PictureBox22.BackgroundImage IsNot Nothing Then
            If AVA = False Then
                AVA = True
                TableLayoutPanel28.Visible = True
                CheckBox7.Visible = True
                CheckBox6.Visible = True
                CheckBox7.Checked = True
                CheckBox6.Checked = True
                'PictureBox22.Image = Nothing
                'PictureBox29.Image = Nothing
                'PictureBox29.BackgroundImage = Nothing
                'PictureBox21.Image = Nothing
                'PictureBox21.BackgroundImage = Nothing
                ToolTip1.SetToolTip(CheckBox8, "Chiudi Controllo Avanzato")
                'ELABORA()
            Else
                'If picmod = True Then
                '    'RadioButton3.PerformClick()
                '    Exit Sub
                'End If
                AVA = False
                REES = False
                REES2 = False
                TableLayoutPanel28.Visible = False
                CheckBox7.Checked = False
                CheckBox6.Checked = False
                CheckBox6.Visible = False
                CheckBox7.Visible = False
                Button27.Visible = False ' salva avanzato
                Button28.Visible = False ' ricarica avanzato
                CheckBox9.Visible = False ' ridimensiona sposa selezione
                PictureBox22.Image = Nothing

                'PictureBox22.Image = Nothing
                'PictureBox29.Image = Nothing
                'PictureBox29.BackgroundImage = Nothing
                'PictureBox21.Image = Nothing
                'PictureBox21.BackgroundImage = Nothing
                CONTROL = True
                CONTROL1 = True
                CONTROL2 = True
                ToolTip1.SetToolTip(CheckBox8, "Controllo Avanzato")
            End If
        End If
    End Sub
    'ATTIVA FUNZIONE CONTROLLO VISIVO
    Private Sub CheckBox6_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox6.CheckedChanged
        If CheckBox6.Checked = True Then
            TableLayoutPanel35.Visible = True

            'PictureBox17.BackgroundImage = My.Resources.counter
            'PictureBox19.BackgroundImage = My.Resources.blob
            'PictureBox18.BackColor = Color.Black
        Else
            TableLayoutPanel35.Visible = False

            'CONTROL = True
            'CONTROL1 = True
            'CONTROL2 = True
        End If
        ELABORA()
    End Sub
    'ATTIVA FUNZIONE RITAGLIO CROP
    Private Sub CheckBox7_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox7.CheckedChanged
        If CheckBox7.Checked = True Then
            TableLayoutPanel32.Visible = True
        Else
            TableLayoutPanel32.Visible = False
        End If
        ELABORA()
    End Sub


    'INVERTI B/N FINESTRA BLOB
    Private Sub CheckBox10_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox10.CheckedChanged
        ELABORA()
    End Sub
    'EROSION 3X3 FINESTRA BLOB
    Private Sub CheckBox11_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox11.CheckedChanged
        ELABORA()
    End Sub
    'EROSION FINESTRA BLOB
    Private Sub CheckBox12_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox12.CheckedChanged
        ELABORA()
    End Sub
    'REGOLA ESPOSIZIONE FINESTRA BLOB
    Private Sub TrackBar3_Scroll(sender As Object, e As EventArgs) Handles TrackBar3.Scroll
        esposizione = TrackBar3.Value
    End Sub
    Private Sub TrackBar3_MOUSEUP(sender As Object, e As EventArgs) Handles TrackBar3.MouseUp
        ELABORA()
    End Sub
    Private Sub PictureBox16_DoubleClick(sender As Object, e As EventArgs) Handles PictureBox16.DoubleClick
        TrackBar3.Value = 0
        esposizione = TrackBar3.Value
        ELABORA()
    End Sub
    'REGOLA CONTRASTO FINESTRA BLOB
    Private Sub TrackBar1_Scroll(sender As Object, e As EventArgs) Handles TrackBar1.Scroll
        contrasto = TrackBar1.Value
    End Sub
    Private Sub TrackBar1_MOUSEUP(sender As Object, e As EventArgs) Handles TrackBar1.MouseUp
        ELABORA()
    End Sub
    Private Sub PictureBox15_DoubleClick(sender As Object, e As EventArgs) Handles PictureBox15.DoubleClick
        TrackBar1.Value = 15
        contrasto = TrackBar1.Value
        ELABORA()
    End Sub
    'AUMENTA RIQUADRO CONTROLLO VISIVO
    Private Sub Button30_Click(sender As Object, e As EventArgs) Handles Button30.Click
        'RB += 5
        RH += 5
        ELABORA()
    End Sub
    'RIDUCI RIQUADRO CONTROLLO VISIVO
    Private Sub Button31_Click(sender As Object, e As EventArgs) Handles Button31.Click
        RH -= 5
        ELABORA()
    End Sub

    ' APRI O SELEZIONA VIEWER ESTERNO
    Private Sub Button19_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles Button19.MouseDown
        If ListBox1.Items.Count = Nothing Then
            Exit Sub
        End If
        If e.Button = Windows.Forms.MouseButtons.Right Then

            If MsgBox("Cambiare Viewer Esterno?", MsgBoxStyle.OkCancel Or MsgBoxStyle.Question, ) = MsgBoxResult.Ok Then
                WIEW1 = ""
                Dim folderDlg As New OpenFileDialog
                If (folderDlg.ShowDialog() = DialogResult.OK) Then
                    WIEW1 = folderDlg.FileName
                    My.Settings.viewer_set = WIEW1
                End If
            Else
                Exit Sub
            End If
        Else
            If WIEW1 = "" Then
                Dim folderDlg As New OpenFileDialog
                If (folderDlg.ShowDialog() = DialogResult.OK) Then
                    WIEW1 = folderDlg.FileName
                    My.Settings.viewer_set = WIEW1
                End If
            Else
                Process.Start(WIEW1, TextBox12.Text & "\" & ListBox1.SelectedItem)
            End If
        End If
    End Sub

    'DISABILITA TUTTI I COMANDI E ABILITA BOTTONE FERMA
    Private Sub DISABLEALL()
        For Each ctrl As Control In Panello_arduino.Controls
            ctrl.Enabled = False
        Next
        For Each ctrl As Control In Panello_target_click.Controls
            ctrl.Enabled = False
        Next
        For Each ctrl As Control In Panello_comandi_arduino.Controls
            ctrl.Enabled = False
        Next


        Button67.Enabled = False
        Button102.Enabled = False
        Button95.Enabled = False
        Button83.Enabled = False
        Button84.Enabled = False
        Button80.Enabled = False
        Button79.Enabled = False
        Button78.Enabled = False
        Button81.Enabled = False
        Button54.Enabled = False
        Button53.Enabled = False
        Button99.Enabled = False
        Button85.Enabled = False
        CheckBox4.Enabled = False
        CheckBox19.Enabled = False
        Button21.Enabled = False
        CheckBox8.Enabled = False
        'Button75.Enabled = False
        ListBox8.Enabled = False
        ListBox6.Enabled = False
        Button82.Enabled = True
        If RENAME1 = True Then
            TextBox13.Enabled = False
            NumericUpDown7.Enabled = False
        End If


    End Sub
    'ABILITA TUTTI I COMANDI E DISABILITA BOTTONE FERMA
    Private Sub ENABLEALL()
        For Each ctrl As Control In Panello_arduino.Controls
            ctrl.Enabled = True
        Next
        For Each ctrl As Control In Panello_target_click.Controls
            ctrl.Enabled = True
        Next
        For Each ctrl As Control In Panello_comandi_arduino.Controls
            ctrl.Enabled = True
        Next

        Button67.Enabled = True
        Button102.Enabled = True
        Button95.Enabled = True
        Button83.Enabled = True
        Button84.Enabled = True
        Button80.Enabled = True
        Button79.Enabled = True
        Button78.Enabled = True
        Button81.Enabled = True
        Button54.Enabled = True
        Button53.Enabled = True
        Button99.Enabled = True
        Button85.Enabled = True
        CheckBox4.Enabled = True
        CheckBox19.Enabled = True
        CheckBox8.Enabled = True
        Button21.Enabled = True
        'Button75.Enabled = True
        If RENAME1 = True Then
            TextBox13.Enabled = True
            NumericUpDown7.Enabled = True
        End If
        ListBox8.Enabled = True
        ListBox6.Enabled = True
        Button82.Enabled = False
    End Sub
#End Region
#End Region
End Class
