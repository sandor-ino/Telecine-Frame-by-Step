Imports System.IO
Imports System.Drawing.Drawing2D
Imports AForge
Imports AForge.Video.DirectShow
Imports AForge.Imaging
Imports AForge.Video
Imports AForge.Imaging.Filters

Imports System.Threading

Public Class Form8
    Private videoDevices As FilterInfoCollection
    Private videoCapabilities As VideoCapabilities()
    Private videoDevice As VideoCaptureDevice
    Private MouseDownStage, MouseDownX, MouseDownY As Integer
    Dim imgB, imgH, PTBRCB, PTBRCH, PTBRCX, PTBRCY As Integer
    Dim IMGRCB, IMGRCH, IMGRCX, IMGRCY As Integer
    Dim CROPMX, CROPMY, CROPMB, CROPMH As Integer
    Dim CROPX, CROPY, CROPB, CROPH As Integer
    Dim esposizione, contrasto As Integer
    Dim BLCX, BLCY, BLCH, BLCB As Integer
    Dim BLX, BLY, BLB, BLH As Integer
    Dim BLXV, BLYV, BLYHV As Integer
    Dim X1, Y1, X2, Y2 As Integer
    Dim ZOOMX, ZOOMY As Decimal
    Dim ptbB, ptbH As Integer
    Dim angolo As Double = 0.0
    Dim pause As Integer
    Dim az1 As Integer = 0
    Dim RB As Integer = 30
    Dim RH As Integer = 0
    Dim bl As Integer
    Dim n As Integer = 0
    Dim OCC As Integer = 0
    Dim mouseClicked As Boolean
    Dim mouseIsDown As Boolean
    Dim flipy As Boolean = True
    Dim flipx As Boolean = True
    Dim rotate As Boolean = True
    Dim play As Boolean = False
    Dim picmod As Boolean = False
    Dim rename As Boolean = False
    Dim CONTROL As Boolean = True
    Dim CONTROL1 As Boolean = True
    Dim CONTROL2 As Boolean = True
    Dim STOPIC As Boolean = False
    Dim SUNR As Boolean = True
    Dim AVA As Boolean = False
    Dim az As Boolean = False
    Dim real As Boolean = False
    Dim REES As Boolean = False
    Dim REES2 As Boolean = False
    Dim ZOOMON As Boolean = False
    Dim ZOOMOV As Boolean = False
    Dim IMAGEN, IMAGENZOOM, IM As Bitmap
    Dim startPoint As New Point()
    Dim endPoint As New Point()
    Dim rectCropArea As Rectangle
    ' APERTURA PROGRAMMA
    Private Sub Form8_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        DoubleBuffered = True
        My.Settings.foto_in = False
        My.Settings.foto_in2 = False
        My.Settings.cam_on = True
        If Not Directory.Exists(My.Settings.ORIG_CART) Then
            TextBox4.Text = ""
        Else
            TextBox4.Text = My.Settings.ORIG_CART
        End If
        If Not Directory.Exists(My.Settings.DEST_CART) Then
            TextBox1.Text = ""
        Else
            TextBox1.Text = My.Settings.DEST_CART
        End If
        Me.Location = New System.Drawing.Point(My.Settings.Form8_Location.X, My.Settings.Form8_Location.Y)
        ButtonStop.Enabled = False
        Button1.Enabled = False
        Button2.Enabled = False
        Button36.Enabled = False
        EnumerateVideoDevices()
        If ComboBox2.Text <> "Not supported" Then
            CameraStart()
            pause = My.Settings.PAUSA
            Timer2.Interval = My.Settings.PAUSA '+ 500
            Timer2.Start()
        End If
    End Sub
    ' CHIUSURA PROGRAMMA
    Private Sub Form8_Closed(sender As Object, e As EventArgs) Handles MyBase.Closed
        Timer2.Stop()
        CameraStop()
        My.Settings.Form8_Location = New System.Drawing.Point(Me.Location.X, Me.Location.Y)
        My.Settings.Save()
        My.Settings.cam_on = False
        My.Settings.def_cam = ComboBox1.Text
        My.Settings.def_res = ComboBox2.Text
        My.Settings.ORIG_CART = TextBox4.Text
        My.Settings.DEST_CART = TextBox1.Text

    End Sub
    ' LISTA FOTOCAMERE DISPONIBILI
    Private Sub EnumerateVideoDevices()
        ' enumerate video devices
        videoDevices = New FilterInfoCollection(FilterCategory.VideoInputDevice)
        If videoDevices.Count <> 0 Then
            ' add all devices to combo
            For Each device As FilterInfo In videoDevices
                ComboBox1.Items.Add(device.Name)
            Next
        Else
            ComboBox1.Items.Add("No devices found")
            PictureBox1.Image = My.Resources.nocamera
        End If
        If ComboBox1.Items.Contains(My.Settings.def_cam) Then
            ComboBox1.Text = My.Settings.def_cam
            ComboBox2.Text = My.Settings.def_res
            PictureBox1.Image = Nothing
        Else
            ComboBox1.SelectedIndex = 0
        End If
    End Sub
    ' SELEZIONA RISOLUZIONE VIDEO CAMERA
    Private Sub EnumerateVideoModes(device As VideoCaptureDevice)
        ' get resolutions for selected video source
        Me.Cursor = Cursors.WaitCursor
        ComboBox2.Items.Clear()
        Try
            videoCapabilities = videoDevice.VideoCapabilities
            For Each capabilty As VideoCapabilities In videoCapabilities
                If Not ComboBox2.Items.Contains(capabilty.FrameSize) Then
                    ComboBox2.Items.Add(capabilty.FrameSize)
                End If
            Next
            If videoCapabilities.Length = 0 Then
                ComboBox2.Items.Add("Not supported")
                ComboBox2.SelectedIndex = 0
                PictureBox1.Image = My.Resources.nosupport
            End If
        Finally
            Me.Cursor = Cursors.[Default]
        End Try
    End Sub
    ' SELEZIONA CAMERA
    Private Sub ComboBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox1.SelectedIndexChanged
        If videoDevices.Count <> 0 Then
            videoDevice = New VideoCaptureDevice(videoDevices(ComboBox1.SelectedIndex).MonikerString)
            EnumerateVideoModes(videoDevice)
            ComboBox2.SelectedIndex = 0
        End If
    End Sub
    ' BOTTONE START/STOP
    Private Sub ButtonStart_Click(sender As Object, e As EventArgs) Handles ButtonStart.Click
        If ComboBox2.Text <> "Not supported" Then
            If play <> True Then
                CameraStart()
            Else
                CameraStop()
                If AVA = True Then
                    Button3.PerformClick()
                End If
            End If
        End If
    End Sub
    ' APRI PROPRIETA' CAMERA
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If (videoCapabilities IsNot Nothing) AndAlso (videoCapabilities.Length <> 0) Then
            videoDevice.DisplayPropertyPage(IntPtr.Zero)
        End If
    End Sub
    ' AVVIO CAMERA START
    Private Sub CameraStart()
        If videoDevice IsNot Nothing Then
            If (videoCapabilities IsNot Nothing) AndAlso (videoCapabilities.Length <> 0) Then
                videoDevice.VideoResolution = videoDevice.VideoCapabilities(ComboBox2.SelectedIndex)
                AddHandler videoDevice.NewFrame, New NewFrameEventHandler(AddressOf Video_NewFrame)
                'IMAGEN = Nothing
                videoDevice.Start()
                Dim ratio As String = ComboBox2.Text
                Dim split = ratio.Split("; ", 2, StringSplitOptions.RemoveEmptyEntries)
                PictureBox1.Height = PictureBox1.Width / (split(0) / split(1))
                ButtonStart.BackgroundImage = My.Resources.offcam
                play = True
                ComboBox1.Enabled = False
                ComboBox2.Enabled = False
                ButtonStop.Enabled = True
                Button1.Enabled = True
                Button2.Enabled = True
                Button36.Enabled = True
                ToolTip1.SetToolTip(ButtonStart, "Camera Stop")
            Else
                MessageBox.Show("non Compatibile")
                PictureBox1.Image = My.Resources.nosupport
            End If
        Else
            PictureBox1.BackgroundImage = Nothing
            PictureBox1.Image = My.Resources.nocamera
        End If
    End Sub
    ' FERMA CAMERA STOP
    Private Sub CameraStop()
        ' stop video device
        If videoDevice IsNot Nothing Then
            videoDevice.SignalToStop()
            videoDevice.WaitForStop()
            videoDevice.Stop()
            PictureBox1.BackgroundImage = Nothing
            ButtonStart.BackgroundImage = My.Resources.oncam
            play = False
            ComboBox1.Enabled = True
            ComboBox2.Enabled = True
            ButtonStop.Enabled = False
            Button1.Enabled = False
            Button2.Enabled = False
            Button36.Enabled = False
            ToolTip1.SetToolTip(ButtonStart, "Camera Start")
        End If
    End Sub
    ' SPECCHIA IN VERTICALE
    Private Sub ButtonStop_Click(sender As Object, e As EventArgs) Handles ButtonStop.Click
        If flipy <> False Then
            flipy = False
        Else
            flipy = True
        End If
    End Sub
    ' SPECCHIA IN ORIZZONTALE
    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        If flipx <> False Then
            flipx = False
        Else
            flipx = True
        End If
    End Sub
    ' ESTRAE FOTOGRAMMA DA VIDEO CAMERA
    Private Sub Video_NewFrame(sender As Object, eventArgs As NewFrameEventArgs)
        If IM IsNot Nothing Then IM.Dispose()
        IM = TryCast(eventArgs.Frame.Clone, Bitmap)
        If eventArgs.Frame IsNot Nothing Then eventArgs.Frame.Dispose()
        If IM IsNot Nothing Then
            If real = True Then
                If IMAGEN Is Nothing Then
                    'If IMAGEN IsNot Nothing Then IMAGEN.Dispose()
                    IMAGEN = TryCast(IM.Clone, Bitmap) ' New Bitmap(IM, IM.Width, IM.Height)
                    If PictureBox1.BackgroundImage IsNot Nothing Then PictureBox1.BackgroundImage.Dispose()
                    PictureBox1.BackgroundImage = IMAGEN
                    If IMAGEN IsNot Nothing Then IMAGEN = Nothing
                End If
            Else
                If OCC = 0 Then

                    If ZOOMON = False Then
                        IMAGEN = New Bitmap(IM, IM.Width, IM.Height)
                    Else
                        IMAGENZOOM = New Bitmap(IM, IM.Width, IM.Height)
                        Dim BAS As Integer = IMAGENZOOM.Width / NumericUpDown2.Value
                        Dim ALT As Integer = IMAGENZOOM.Height / NumericUpDown2.Value
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
                    If flipy <> True Then
                        IMAGEN.RotateFlip(RotateFlipType.RotateNoneFlipY)
                        Thread.Sleep(5)
                    End If
                    If flipx <> True Then
                        IMAGEN.RotateFlip(RotateFlipType.RotateNoneFlipX)
                        Thread.Sleep(5)
                    End If

                    If PictureBox1.BackgroundImage IsNot Nothing Then PictureBox1.BackgroundImage.Dispose()
                    PictureBox1.BackgroundImage = IMAGEN

                    OCC = 1
                End If
            End If
        End If
    End Sub
    ''''' FUNZIONI AVANZATE CONTROLLO VISIVO E RITAGLIO '''''
    Private Sub Timer2_Tick(sender As Object, e As EventArgs) Handles Timer2.Tick
        Application.DoEvents()
        ' verifica se si modifica valore pausa in setup
        If pause <> My.Settings.PAUSA Then
            Timer2.Interval = My.Settings.PAUSA '+ 500
            pause = My.Settings.PAUSA
        End If
        ' verifica se in modalita' immagini
        If picmod = True Then
            Try
                IMAGEN = Image.FromFile(TextBox4.Text & "/" & ListBox1.SelectedItem)
            Catch ex As Exception
            End Try
            If IMAGEN IsNot Nothing Then
                If flipy = False Then
                    Dim Filtmiry = New Mirror(True, False)
                    IMAGEN = Filtmiry.Apply(IMAGEN)
                End If
                If flipx = False Then
                    Dim Filtmirx = New Mirror(False, True)
                    IMAGEN = Filtmirx.Apply(IMAGEN)
                End If
                If rotate = False Then
                    Dim Filtrot = New RotateNearestNeighbor(angolo, True)
                    IMAGEN = Filtrot.Apply(IMAGEN)
                End If
                PictureBox1.BackgroundImage = IMAGEN
                OCC = 1
            End If
        End If
        If IMAGEN IsNot Nothing Then
            If OCC = 1 Then
                If real = False Then
                    ' funzione avanzate ritaglio controllo
                    If AVA = True Then
                        ' verifica selezione super 8 o normal 8
                        If My.Settings.SUP_NOR <> True Then
                            If SUNR = True Then
                                Button6.BackgroundImage = My.Resources.fotocenterN
                                PictureBox22.Image = My.Resources.CROPGIFN8
                                Me.Button6.PerformClick()
                                SUNR = False
                            End If
                        Else
                            If SUNR = False Then
                                Button6.BackgroundImage = My.Resources.fotocenter
                                PictureBox22.Image = My.Resources.CROPGIFS8
                                Me.Button6.PerformClick()
                                SUNR = True
                            End If
                        End If
                        ' ELABORA FOTOGRAMMA
                        If rectCropArea.X = Nothing Or rectCropArea.Y = Nothing Or rectCropArea.Width = Nothing Or rectCropArea.Height = Nothing Then
                            Panel16.Visible = False
                            PictureBox23.BackColor = Color.Black
                            bl = 0
                            PictureBox17.BackgroundImage = My.Resources.BLOB
                            Picturebox2.BackgroundImage = Nothing
                            PictureBox3.BackgroundImage = Nothing
                            If My.Settings.SUP_NOR = True Then
                                If REES = False Then
                                    PictureBox3.Image = My.Resources.S8PIC
                                    Picturebox2.Image = My.Resources.S8GIF
                                    REES = True
                                    REES2 = False
                                End If
                            Else
                                If REES2 = False Then
                                    PictureBox3.Image = My.Resources.N8PIC
                                    Picturebox2.Image = My.Resources.N8GIF
                                    REES2 = True
                                    REES = False
                                End If
                            End If
                        Else
                            REES = False
                            REES2 = False
                            Panel16.Visible = True
                            ptbB = PictureBox1.Width
                            ptbH = PictureBox1.Height
                            imgB = PictureBox1.BackgroundImage.Width
                            imgH = PictureBox1.BackgroundImage.Height
                            PTBRCX = rectCropArea.X
                            PTBRCY = rectCropArea.Y
                            PTBRCB = rectCropArea.Width
                            PTBRCH = rectCropArea.Height
                            IMGRCX = PTBRCX * imgB / ptbB
                            IMGRCY = PTBRCY * imgH / ptbH
                            IMGRCB = PTBRCB * imgB / ptbB
                            IMGRCH = PTBRCH * imgH / ptbH
                            Dim bit As New Bitmap(IMAGEN, IMAGEN.Width, IMAGEN.Height)
                            Dim cropBitmap As New Bitmap(IMGRCB, IMGRCH)
                            Dim H As Graphics = Graphics.FromImage(cropBitmap)
                            H.InterpolationMode = InterpolationMode.HighQualityBicubic
                            H.PixelOffsetMode = PixelOffsetMode.HighQuality
                            H.CompositingQuality = CompositingQuality.HighQuality
                            H.DrawImage(bit, New Rectangle(0, (0), CInt(IMGRCB), CInt(IMGRCH)),
                                                        IMGRCX, IMGRCY, IMGRCB, IMGRCH, GraphicsUnit.Pixel)
                            ' applica filtri estrazione blob
                            'Dim sample As Bitmap = CType(cropBitmap.Clone(), Bitmap)
                            Dim sample As Bitmap = DirectCast(cropBitmap.Clone, Bitmap)
                            Dim gray As Grayscale = New Grayscale(0.2125, 0.7154, 0.0721)
                            Dim filterInvert As Invert = New Invert()
                            Dim FILTRI = New FiltersSequence(New BrightnessCorrection(esposizione),
                                                                             New ContrastCorrection(contrasto),
                                                                             New SISThreshold()) ' soglia bianco e nero
                            Dim filter4 As Erosion = New Erosion()
                            Dim filter5 As Erosion3x3 = New Erosion3x3()
                            Dim gray1 As Bitmap = gray.Apply(sample)
                            'sample.Dispose()
                            Dim gray2 As Bitmap
                            Dim gray3 As Bitmap
                            Dim gray4 As Bitmap
                            Dim gray5 As Bitmap
                            If CheckBox3.Checked = True Then
                                'filtro inverti
                                gray2 = filterInvert.Apply(gray1)
                                'filtro erosion
                                If CheckBox1.Checked = True Then
                                    gray3 = filter4.Apply(gray2)
                                Else
                                    gray3 = gray2
                                End If
                                'filtro erosion3x3
                                If CheckBox2.Checked = True Then
                                    gray4 = filter5.Apply(gray3)
                                Else
                                    gray4 = gray3
                                End If
                                gray5 = FILTRI.Apply(gray4)
                            Else
                                'filtro erosion
                                If CheckBox1.Checked = True Then
                                    gray2 = filter4.Apply(gray1)
                                Else
                                    gray2 = gray1
                                End If
                                'filtro erosion3x3
                                If CheckBox2.Checked = True Then
                                    gray3 = filter5.Apply(gray2)
                                Else
                                    gray3 = gray2
                                End If
                                gray5 = FILTRI.Apply(gray3)
                            End If
                            Dim grayImage As Bitmap = gray5
                            ' elaborazione blob
                            Dim blobCounter As BlobCounter = New BlobCounter()
                            blobCounter.FilterBlobs = True
                            ' dimensione minima blob
                            blobCounter.MinWidth = grayImage.Width / 3
                            blobCounter.MinHeight = grayImage.Height / 3
                            blobCounter.ProcessImage(grayImage)
                            Dim blobs As Blob() = blobCounter.GetObjectsInformation()
                            Dim DRW As Bitmap = New Bitmap(cropBitmap.Width, cropBitmap.Height)
                            Dim g As Graphics = Graphics.FromImage(DRW)
                            Dim f As Graphics = Graphics.FromImage(DRW)
                            Dim v As Graphics = Graphics.FromImage(DRW)
                            Dim o As Graphics = Graphics.FromImage(DRW)
                            Dim pic2 As Bitmap = New Bitmap(grayImage.Width, grayImage.Height)
                            Dim rectangle2 = New Rectangle(0, IMGRCH / 2, IMGRCB, 1)
                            Dim rectangle3 As Rectangle
                            For Each blob As Blob In blobs
                                Dim edgePoints As List(Of IntPoint) = blobCounter.GetBlobsEdgePoints(blob)
                                For Each point As IntPoint In edgePoints
                                    ' disegna sagoma blobs 
                                    pic2.SetPixel(point.X, point.Y, Color.Red)
                                Next
                                ' disegna rettangolo blob
                                g.DrawRectangle(New Pen(Color.Yellow, 8), blob.Rectangle)
                                ' disegna rettangolo controllo visivo
                                If CheckBox7.Checked = True Then
                                    v.FillRectangle(Brushes.Red, rectangle3)
                                End If
                                BLX = blob.Rectangle.X
                                BLY = blob.Rectangle.Y
                                BLB = blob.Rectangle.Width
                                BLH = blob.Rectangle.Height
                                BLCX = BLX + (BLB / 2)
                                BLCY = BLY + (BLH / 2)
                                ' disegna angoli di riferimento rettangolo ritaglio 
                                If CheckBox4.Checked = True Then
                                    If CheckBox5.Checked = True And CheckBox6.Checked = False Then
                                        If CheckBox7.Checked <> True Then
                                            Dim r As New Rectangle(BLX + 5, BLY + 5, 50, 50)
                                            o.DrawImage(My.Resources.ALTR, r)
                                        End If
                                    End If
                                    If CheckBox5.Checked = False And CheckBox6.Checked = True Then
                                        If CheckBox7.Checked <> True Then
                                            Dim r As New Rectangle(BLX + 5, BLY + BLH - 55, 50, 50)
                                            o.DrawImage(My.Resources.BASSR, r)
                                        End If
                                    End If
                                End If
                            Next
                            If CheckBox5.Checked = True And CheckBox6.Checked = False Then
                                rectangle3 = New Rectangle(BLCX - RB / 2, BLY - (RH + BLH / 2) / 2, RB, RH + BLH / 2)
                            End If
                            If CheckBox5.Checked = True And CheckBox6.Checked = True Or CheckBox5.Checked = False And CheckBox6.Checked = False Then
                                rectangle3 = New Rectangle(BLCX - RB / 2, BLCY - (RH + BLH / 2) / 2, RB, RH + BLH / 2)
                            End If
                            If CheckBox5.Checked = False And CheckBox6.Checked = True Then
                                rectangle3 = New Rectangle(BLCX - RB / 2, BLY + BLH - (RH + BLH / 2) / 2, RB, RH + BLH / 2)
                            End If
                            'CONTROLLO VISIVO, CONTEGGIO, VERIFICA POSIZIONE E SPOSTAMENTO

                            If CheckBox7.Checked = True Then
                                Application.DoEvents()
                                ' disegna linea conteggio blob
                                f.DrawLine(New Pen(Color.Red, 8), 0, CInt(IMGRCH / 2), CInt(IMGRCB), CInt(IMGRCH / 2))
                                ' verifica se c'è un solo blob  
                                bl = Double.Parse(blobs.Length)
                                If bl <> 1 Then
                                    PictureBox17.BackgroundImage = My.Resources.BLOBRED
                                    ToolTip1.SetToolTip(PictureBox17, bl & " Blob")

                                    CONTROL1 = False
                                Else
                                    PictureBox17.BackgroundImage = My.Resources.BLOBVER
                                    ToolTip1.SetToolTip(PictureBox17, bl & " Blob")

                                    CONTROL1 = True
                                End If
                                ' verifica posizione
                                If rectangle2.IntersectsWith(rectangle3) Then
                                    v.FillRectangle(Brushes.Green, rectangle3)
                                    f.DrawRectangle(New Pen(Color.Green, 10), rectangle2)
                                    PictureBox23.BackColor = Color.Green
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
                                                    PictureBox16.BackgroundImage = My.Resources.CONTERVER
                                                    CONTROL2 = True
                                                    BLXV = BLX
                                                    BLYV = BLY
                                                    BLYHV = BLY + BLH
                                                    n = n + 1
                                                    ToolTip1.SetToolTip(PictureBox16, n & " Fotogrammi OK" & vbCrLf & "Doppio Clik Azzera")
                                                Else
                                                    PictureBox16.BackgroundImage = My.Resources.CONTERRED
                                                    CONTROL2 = False
                                                End If
                                            End If
                                        End If
                                    End If
                                Else
                                    CONTROL = False
                                    v.FillRectangle(Brushes.Red, rectangle3)
                                    PictureBox23.BackColor = Color.Red
                                    H.Dispose()
                                    g.Dispose()
                                    o.Dispose()
                                    f.Dispose()
                                End If
                            Else
                                CONTROL = True
                                CONTROL1 = True
                                CONTROL2 = True
                                PictureBox17.BackgroundImage = My.Resources.BLOB
                                PictureBox16.BackgroundImage = My.Resources.CONTER
                            End If

                            Dim pic3 As Bitmap = New Bitmap(bit.Width, bit.Height)

                            ' disegna rettangolo blob in picturebox1
                            If CheckBox4.Checked = True Or CheckBox7.Checked = True Then
                                Dim k As Graphics = Graphics.FromImage(pic3)
                                k.FillRectangle(Brushes.Yellow, IMGRCX + BLX, IMGRCY + BLY, BLB, BLH)
                                If CheckBox5.Checked = True And CheckBox6.Checked = False Then
                                    Dim r As New Rectangle(IMGRCX + BLX + 5, IMGRCY + BLY + 5, 50, 50)
                                    k.DrawImage(My.Resources.ALTR, r)
                                End If
                                If CheckBox5.Checked = False And CheckBox6.Checked = True Then
                                    Dim r As New Rectangle(IMGRCX + BLX + 5, IMGRCY + BLY + BLH - 55, 50, 50)
                                    k.DrawImage(My.Resources.BASSR, r)
                                End If
                                k.Dispose()
                            End If

                            If CheckBox4.Checked = True Then
                                ' disegna rettangolo ritaglio e maschera
                                Dim u As Graphics = Graphics.FromImage(pic3)
                                Dim z1 As Graphics = Graphics.FromImage(pic3)
                                Dim z2 As Graphics = Graphics.FromImage(pic3)
                                Dim z3 As Graphics = Graphics.FromImage(pic3)
                                Dim z4 As Graphics = Graphics.FromImage(pic3)
                                Dim z5 As Graphics = Graphics.FromImage(pic3)
                                If CheckBox5.Checked = True And CheckBox6.Checked = False Then
                                    u.DrawRectangle(New Pen(Color.Red, 8), IMGRCX + BLX + CROPMX, IMGRCY + BLY - CROPMY, CROPMB, CROPMH)
                                    If CheckBox8.Checked = True Then
                                        z1.FillRectangle(Brushes.Black, 0, 0, IMGRCX + BLX + CROPMX, imgH)
                                        z2.FillRectangle(Brushes.Black, IMGRCX + BLX + CROPMX, 0, CROPMB, IMGRCY - CROPMY + BLY)
                                        z3.FillRectangle(Brushes.Black, IMGRCX + BLX + CROPMX + CROPMB, 0, imgB - CROPMB - CROPMX, imgH)
                                        z4.FillRectangle(Brushes.Black, IMGRCX + BLX + CROPMX, IMGRCY + BLY - CROPMY + CROPMH, CROPMB, IMGRCY + BLY - CROPMY + CROPMH)
                                        z5.FillRectangle(Brushes.Yellow, IMGRCX + BLX, IMGRCY + BLY, BLB, BLH)
                                    End If
                                End If
                                If CheckBox5.Checked = True And CheckBox6.Checked = True Or CheckBox5.Checked = False And CheckBox6.Checked = False Then
                                    u.DrawRectangle(New Pen(Color.Red, 8), IMGRCX + BLX + CROPMX, IMGRCY + BLY + CInt(BLH / 2) - CROPMY, CROPMB, CROPMH)
                                    If CheckBox8.Checked = True Then
                                        z1.FillRectangle(Brushes.Black, 0, 0, IMGRCX + BLX + CROPMX, imgH)
                                        z2.FillRectangle(Brushes.Black, IMGRCX + BLX + CROPMX, 0, CROPMB, IMGRCY + CInt(BLH / 2) - CROPMY + BLY)
                                        z3.FillRectangle(Brushes.Black, IMGRCX + BLX + CROPMX + CROPMB, 0, imgB - IMGRCX - BLX - CROPMB - CROPMX, imgH)
                                        z4.FillRectangle(Brushes.Black, IMGRCX + BLX + CROPMX, IMGRCY + BLY + CInt(BLH / 2) - CROPMY + CROPMH, CROPMB, imgH - IMGRCY + BLY + CInt(BLH / 2) - CROPMY + CROPMH)
                                        z5.FillRectangle(Brushes.Yellow, IMGRCX + BLX, IMGRCY + BLY, BLB, BLH)
                                    End If
                                End If
                                If CheckBox5.Checked = False And CheckBox6.Checked = True Then
                                    u.DrawRectangle(New Pen(Color.Red, 8), IMGRCX + BLX + CROPMX, IMGRCY + BLY + BLH - CROPMY, CROPMB, CROPMH)
                                    If CheckBox8.Checked = True Then
                                        z1.FillRectangle(Brushes.Black, 0, 0, IMGRCX + BLX + CROPMX, imgH)
                                        z2.FillRectangle(Brushes.Black, IMGRCX + BLX + CROPMX, 0, CROPMB, IMGRCY + CInt(BLH) - CROPMY + BLY)
                                        z3.FillRectangle(Brushes.Black, IMGRCX + BLX + CROPMX + CROPMB, 0, imgB - IMGRCX - BLX - CROPMB - CROPMX, imgH)
                                        z4.FillRectangle(Brushes.Black, IMGRCX + BLX + CROPMX, IMGRCY + BLY + CInt(BLH) - CROPMY + CROPMH, CROPMB, imgH - IMGRCY + BLY + BLH - CROPMY + CROPMH)
                                        z5.FillRectangle(Brushes.Yellow, IMGRCX + BLX, IMGRCY + BLY, BLB, BLH)
                                    End If
                                End If

                                u.Dispose()
                                z1.Dispose()
                                z2.Dispose()
                                z3.Dispose()
                                z4.Dispose()
                                z5.Dispose()

                            End If
                            PictureBox1.Image = pic3
                            Picturebox2.BackgroundImage = cropBitmap
                            Picturebox2.Image = DRW
                            PictureBox3.Image = pic2
                            PictureBox3.BackgroundImage = grayImage
                        End If
                    End If
                    ' funzione salva immagine con o senza ritaglio
                    If IMAGEN IsNot Nothing Then
                        If picmod = False Then
                            If My.Settings.foto_in = True And My.Settings.foto_in2 = False Then
                                If CONTROL = True And CONTROL1 = True And CONTROL2 = True Then
                                    Dim no As String = My.Settings.perc_file
                                    If IMAGEN IsNot Nothing Then
                                        If CheckBox4.Checked = True Then
                                            Dim croBitmap As New Bitmap(IMAGEN, CROPMB, CROPMH)
                                            Dim p As Graphics = Graphics.FromImage(croBitmap)
                                            p.InterpolationMode = InterpolationMode.HighQualityBicubic
                                            p.PixelOffsetMode = PixelOffsetMode.HighQuality
                                            p.CompositingQuality = CompositingQuality.HighQuality
                                            If CheckBox5.Checked = True And CheckBox6.Checked = False Then
                                                p.DrawImage(IMAGEN, New Rectangle(0, 0, CROPMB, CROPMH),
                                                                IMGRCX + BLX + CROPMX, IMGRCY + BLY - CROPMY,
                                                                CROPMB, CROPMH, GraphicsUnit.Pixel)
                                            End If
                                            If CheckBox5.Checked = True And CheckBox6.Checked = True Or CheckBox5.Checked = False And CheckBox6.Checked = False Then
                                                p.DrawImage(IMAGEN, New Rectangle(0, 0, CROPMB, CROPMH),
                                                                IMGRCX + BLX + CROPMX,
                                                                IMGRCY + BLY + CInt(BLH / 2) - CROPMY,
                                                                CROPMB, CROPMH, GraphicsUnit.Pixel)
                                            End If
                                            If CheckBox5.Checked = False And CheckBox6.Checked = True Then
                                                p.DrawImage(IMAGEN, New Rectangle(0, 0, CROPMB, CROPMH),
                                                                IMGRCX + BLX + CROPMX, IMGRCY + BLY + BLH - CROPMY,
                                                                CROPMB, CROPMH, GraphicsUnit.Pixel)
                                            End If
                                            Select Case My.Settings.file_ext
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
                                            Select Case My.Settings.file_ext
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
                                        If My.Settings.view_on = True Then
                                            My.Settings.foto_in2 = True
                                        Else
                                            My.Settings.foto_in2 = False
                                        End If
                                        My.Settings.foto_in = False
                                        My.Settings.FERMA = False
                                    End If
                                Else
                                    My.Settings.foto_in = False
                                    My.Settings.foto_in2 = False
                                    My.Settings.FERMA = True
                                End If
                                Thread.Sleep(My.Settings.PAUSA / 2)
                                IMAGEN = Nothing
                                OCC = 0
                            Else
                                IMAGEN = Nothing
                                OCC = 0
                            End If
                        End If
                    End If
                End If
            Else
                Timer2.Stop()
                MsgBox("ATTENZIONE!" & vbCrLf & "Aumentare Tempo di Elaborazione in Setup", MessageBoxIcon.Warning)
                My.Settings.foto_in = False
                My.Settings.foto_in2 = False
                My.Settings.FERMA = True
                If AVA = True Then
                    Button3.PerformClick()
                End If
                If flipx = False Then
                    ButtonStop.PerformClick()
                End If
                If flipy = False Then
                    Button2.PerformClick()
                End If
                If ZOOMON = True Then
                    Button37.PerformClick()
                End If
                IMAGEN = Nothing
                OCC = 0
                Timer2.Start()
            End If
        End If
    End Sub
    ' APRI/CHIUDI AVANZATO 
    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        If Me.WindowState <> FormWindowState.Normal Then
            Me.WindowState = FormWindowState.Normal
        End If
        If AVA = False Then
            Me.Width = Me.MinimumSize.Width
            Me.Height = Me.MinimumSize.Height
            Panel12.Anchor = AnchorStyles.Top Or AnchorStyles.Left
            Me.MinimumSize = New Size(Me.Width * 2, Me.Height)
            Panel1.Visible = True
            Panel13.Visible = True
            PictureBox1.Image = Nothing
            Picturebox2.Image = Nothing
            Picturebox2.BackgroundImage = Nothing
            PictureBox3.Image = Nothing
            PictureBox3.BackgroundImage = Nothing
            Button3.BackgroundImage = My.Resources.back
            AVA = True
            Button45.Enabled = False
            ToolTip1.SetToolTip(Button3, "Chiudi Controllo Avanzato")
        Else
            REES = False
            REES2 = False
            If picmod = True Then
                Button17.PerformClick()
            End If
            Me.Width = Me.MinimumSize.Width
            Me.Height = Me.MinimumSize.Height
            Panel12.Anchor = AnchorStyles.Top Or AnchorStyles.Left
            Me.MinimumSize = New Size(Me.Width / 2, Me.Height)
            Me.Width = Me.Width / 2
            Panel12.Anchor = AnchorStyles.Top Or AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right
            Panel1.Visible = False
            Panel13.Visible = False
            PictureBox1.Image = Nothing
            Picturebox2.Image = Nothing
            Picturebox2.BackgroundImage = Nothing
            PictureBox3.Image = Nothing
            PictureBox3.BackgroundImage = Nothing
            Button3.BackgroundImage = My.Resources.forward
            CONTROL = True
            CONTROL1 = True
            CONTROL2 = True
            AVA = False
            'IMAGEN = Nothing
            Button45.Enabled = True
            ToolTip1.SetToolTip(Button3, "Controllo Avanzato")
        End If
    End Sub
    ' ATTIVA FUNZIONE CONTROLLO VISIVO
    Private Sub CheckBox7_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox7.CheckedChanged
        If CheckBox7.Checked = True Then
            Panel10.Visible = True
            PictureBox16.BackgroundImage = My.Resources.CONTER
            PictureBox17.BackgroundImage = My.Resources.BLOB
            PictureBox23.BackColor = Color.Black
        Else
            Panel10.Visible = False
            CONTROL = True
            CONTROL1 = True
            CONTROL2 = True
        End If
    End Sub
    ' CAMBIA LEGENDA QUANDO E' PREMUTO IL PULSANTE CAMBIA SELLEZIONE
    Private Sub CheckBox9_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox9.CheckedChanged
        If CheckBox9.Checked = True Then
            PictureBox15.BackgroundImage = My.Resources.linsel2
        Else
            PictureBox15.BackgroundImage = My.Resources.linsel
        End If

    End Sub
    ' ATTIVA FUNZIONE RITAGLIO CROP
    Private Sub CheckBox4_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox4.CheckedChanged
        If CheckBox4.Checked <> False Then
            CheckBox8.Visible = True
            Panel4.Visible = True
            Button6.PerformClick()
        Else
            CheckBox8.Visible = False
            Panel4.Visible = False
        End If
    End Sub
    ' SALVA IMPOSTAZIONI AVANZATO
    Private Sub Button13_Click(sender As Object, e As EventArgs) Handles Button13.Click
        If MsgBox("Salvare Impostazioni?", MsgBoxStyle.YesNo, ) = MsgBoxResult.Yes Then
            My.Settings.CMX = CROPMX
            My.Settings.CMY = CROPMY
            My.Settings.CMB = CROPMB
            My.Settings.CMH = CROPMH
            My.Settings.RCB = RB
            My.Settings.RCH = RH
            My.Settings.RIFA = CheckBox5.Checked
            My.Settings.RIFB = CheckBox6.Checked
            My.Settings.ER = CheckBox1.Checked
            My.Settings.ER3 = CheckBox2.Checked
            My.Settings.INV = CheckBox3.Checked
            My.Settings.ESPO = TrackBar2.Value
            My.Settings.CONT = TrackBar1.Value
            My.Settings.RSX = rectCropArea.X
            My.Settings.RSY = rectCropArea.Y
            My.Settings.RSB = rectCropArea.Width
            My.Settings.RSH = rectCropArea.Height
        End If
    End Sub
    ' CARICA IMPOSTAZIONI AVANZATO
    Private Sub Button11_Click(sender As Object, e As EventArgs) Handles Button11.Click
        If MsgBox("Caricare Impostazioni?", MsgBoxStyle.YesNo, ) = MsgBoxResult.Yes Then
            CROPMX = My.Settings.CMX
            CROPMY = My.Settings.CMY
            CROPMB = My.Settings.CMB
            CROPMH = My.Settings.CMH
            RB = My.Settings.RCB
            RH = My.Settings.RCH
            CheckBox5.Checked = My.Settings.RIFA
            CheckBox6.Checked = My.Settings.RIFB
            CheckBox1.Checked = My.Settings.ER
            CheckBox2.Checked = My.Settings.ER3
            CheckBox3.Checked = My.Settings.INV
            TrackBar2.Value = My.Settings.ESPO
            TrackBar1.Value = My.Settings.CONT
            rectCropArea.X = My.Settings.RSX
            rectCropArea.Y = My.Settings.RSY
            rectCropArea.Width = My.Settings.RSB
            rectCropArea.Height = My.Settings.RSH
        End If
    End Sub
    ' REGOLA ESPOSIZIONE SOLO IN AVANZATO
    Private Sub TrackBar2_Scroll(sender As Object, e As EventArgs) Handles TrackBar2.Scroll
        esposizione = TrackBar2.Value
    End Sub
    ' REGOLA CONTRASTO SOLO IN AVANZATO
    Private Sub TrackBar1_Scroll(sender As Object, e As EventArgs) Handles TrackBar1.Scroll
        contrasto = TrackBar1.Value
    End Sub
    ' DISEGNA RETTANGOLO RITAGLIO CENTRATO E PROPORZIONATO 
    Private Sub Button6_Click(sender As Object, e As EventArgs) Handles Button6.Click
        If PictureBox1.Image IsNot Nothing Then
            Dim pic5 As Bitmap = New Bitmap(PictureBox1.Image.Width, PictureBox1.Image.Height)
            Dim u As Graphics = Graphics.FromImage(pic5)
            If My.Settings.SUP_NOR <> True Then
                ' normal 8 
                Button6.BackgroundImage = My.Resources.fotocenterN
                ' riferimento blob alto
                If CheckBox5.Checked = True And CheckBox6.Checked = False Then
                    CROPMX = CInt(BLB * (2.28 / 1.8))
                    CROPMY = CInt(BLH * (2.94 / 1.23))
                    CROPX = IMGRCX + BLX + CROPMX
                    CROPY = IMGRCY + BLY - CROPMY
                    CROPB = CInt(BLB * (4.5 / 1.8))
                    If CROPB Mod (2) = 0 Then
                    Else
                        CROPB += 1
                    End If
                    CROPH = CInt(BLH * (3.3 / 1.23))
                    If CROPH Mod (2) = 0 Then
                    Else
                        CROPH += 1
                    End If
                    u.DrawRectangle(New Pen(Color.Red, 10), New Rectangle(CROPX, CROPY, CROPB, CROPH))
                    CROPMB = CROPB
                    CROPMH = CROPH
                    PictureBox1.Image = pic5
                    u.Dispose()
                    Label4.Text = CROPMB
                    Label6.Text = CROPMH
                End If
                ' riferimento blob centro
                If CheckBox5.Checked = True And CheckBox6.Checked = True Or CheckBox5.Checked = False And CheckBox6.Checked = False Then
                    CROPMX = CInt(BLB * (2.28 / 1.8))
                    CROPMY = CInt(BLH * (3.555 / 1.23))
                    CROPX = IMGRCX + BLX + CROPMX
                    CROPY = IMGRCY + BLY + BLH / 2 - CROPMY
                    CROPB = CInt(BLB * (4.5 / 1.8))
                    If CROPB Mod (2) = 0 Then
                    Else
                        CROPB += 1
                    End If
                    CROPH = CInt(BLH * (3.3 / 1.23))
                    If CROPH Mod (2) = 0 Then
                    Else
                        CROPH += 1
                    End If
                    u.DrawRectangle(New Pen(Color.Red, 10), New Rectangle(CROPX, CROPY, CROPB, CROPH))
                    CROPMB = CROPB
                    CROPMH = CROPH
                    PictureBox1.Image = pic5
                    u.Dispose()
                    Label4.Text = CROPMB
                    Label6.Text = CROPMH
                End If
                ' riferimento blob basso
                If CheckBox5.Checked = False And CheckBox6.Checked = True Then
                    CROPMX = CInt(BLB * (2.28 / 1.8))
                    CROPMY = CInt(BLH * (4.17 / 1.23))
                    CROPX = IMGRCX + BLX + CROPMX
                    CROPY = IMGRCY + BLY + BLH - CROPMY
                    CROPB = CInt(BLB * (4.5 / 1.8))
                    If CROPB Mod (2) = 0 Then
                    Else
                        CROPB += 1
                    End If
                    CROPH = CInt(BLH * (3.3 / 1.23))
                    If CROPH Mod (2) = 0 Then
                    Else
                        CROPH += 1
                    End If
                    u.DrawRectangle(New Pen(Color.Red, 10), New Rectangle(CROPX, CROPY, CROPB, CROPH))
                    CROPMB = CROPB
                    CROPMH = CROPH
                    PictureBox1.Image = pic5
                    u.Dispose()
                    Label4.Text = CROPMB
                    Label6.Text = CROPMH
                End If
            Else
                'super 8
                Button6.BackgroundImage = My.Resources.fotocenter
                ' riferimento blob alto
                If CheckBox5.Checked = True And CheckBox6.Checked = False Then
                    CROPMX = CInt(BLB * (1.08 / 0.91))
                    CROPMY = CInt(BLH * (1.435 / 1.14))
                    CROPX = IMGRCX + BLX + CROPMX
                    CROPY = IMGRCY + BLY - CROPMY
                    CROPB = CInt(BLB * (5.46 / 0.91))
                    If CROPB Mod (2) <> 0 Then
                        CROPB += 1
                    End If
                    CROPH = CInt(BLH * (4.01 / 1.14))
                    If CROPH Mod (2) <> 0 Then
                        CROPH += 1
                    End If
                    u.DrawRectangle(New Pen(Color.Red, 10), New Rectangle(CROPX, CROPY, CROPB, CROPH))
                    CROPMB = CROPB
                    CROPMH = CROPH
                    PictureBox1.Image = pic5
                    u.Dispose()
                    Label4.Text = CROPMB
                    Label6.Text = CROPMH
                End If
                ' riferimento blob centro
                If CheckBox5.Checked = True And CheckBox6.Checked = True Or CheckBox5.Checked = False And CheckBox6.Checked = False Then
                    CROPMX = CInt(BLB * (1.08 / 0.91))
                    CROPMY = CInt(BLH * (2.005 / 1.14))
                    CROPX = IMGRCX + BLX + CROPMX
                    CROPY = IMGRCY + BLY + BLH / 2 - CROPMY
                    CROPB = CInt(BLB * (5.46 / 0.91))
                    If CROPB Mod (2) <> 0 Then
                        CROPB += 1
                    End If
                    CROPH = CInt(BLH * (4.01 / 1.14))
                    If CROPH Mod (2) <> 0 Then
                        CROPH += 1
                    End If
                    u.DrawRectangle(New Pen(Color.Red, 10), New Rectangle(CROPX, CROPY, CROPB, CROPH))
                    CROPMB = CROPB
                    CROPMH = CROPH
                    PictureBox1.Image = pic5
                    u.Dispose()
                    Label4.Text = CROPMB
                    Label6.Text = CROPMH
                End If
                ' riferimento blob basso
                If CheckBox5.Checked = False And CheckBox6.Checked = True Then
                    CROPMX = CInt(BLB * (1.08 / 0.91))
                    CROPMY = CInt(BLH * (2.575 / 1.14))
                    CROPX = IMGRCX + BLX + CROPMX
                    CROPY = IMGRCY + BLY + BLH - CROPMY
                    CROPB = CInt(BLB * (5.46 / 0.91))
                    If CROPB Mod (2) <> 0 Then
                        CROPB += 1
                    End If
                    CROPH = CInt(BLH * (4.01 / 1.14))
                    If CROPH Mod (2) <> 0 Then
                        CROPH += 1
                    End If
                    u.DrawRectangle(New Pen(Color.Red, 10), New Rectangle(CROPX, CROPY, CROPB, CROPH))
                    CROPMB = CROPB
                    CROPMH = CROPH
                    PictureBox1.Image = pic5
                    u.Dispose()
                    Label4.Text = CROPMB
                    Label6.Text = CROPMH
                End If
            End If
        End If
    End Sub
    ' CAMBIA ICONA SELEZIONE RIFERIMENTO ALTO
    Private Sub CheckBox5_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox5.CheckedChanged
        If CheckBox5.Checked = True Then
            CheckBox5.BackgroundImage = My.Resources.ANGOLOA
            Button46.BackgroundImage = My.Resources.ALR
            If CheckBox6.Checked = True Then
                CheckBox6.Checked = False
            End If
        Else
            CheckBox5.BackgroundImage = My.Resources.ANGOLOA1
            Button46.BackgroundImage = My.Resources.ALA
        End If
    End Sub
    ' CAMBIA ICONA SELEZIONE RIFERIMENTO BASSO
    Private Sub CheckBox6_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox6.CheckedChanged
        If CheckBox6.Checked = True Then
            CheckBox6.BackgroundImage = My.Resources.ANGOLOB
            Button47.BackgroundImage = My.Resources.BAR
            If CheckBox5.Checked = True Then
                CheckBox5.Checked = False
            End If
        Else
            CheckBox6.BackgroundImage = My.Resources.ANGOLOB1
            Button47.BackgroundImage = My.Resources.BAA
        End If
    End Sub
    ' AUMENTA RIQUADRO CONTROLLO VISIVO
    Private Sub Button7_Click(sender As Object, e As EventArgs) Handles Button7.Click
        'RB += 5
        RH += 5
    End Sub
    ' RIDUCI RIQUADRO CONTROLLO VISIVO
    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        RH -= 5
    End Sub
    ''''' AVVIO RITAGLIA TUTTI IN MODALITA PICTURE '''''
    Private Sub Button16_Click(sender As Object, e As EventArgs) Handles Button16.Click
        If ListBox2.Items.Count.ToString() > 0 Then
            Select Case MsgBox("ATTENZIONE!" & vbCrLf & vbCrLf & "la cartella di Destinazione non è vuota." & vbCrLf & vbCrLf & "I Nomi Esistenti saranno Sovrascritti." & vbCrLf & vbCrLf & "Avvio Comunque il Processo?", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                Case Windows.Forms.DialogResult.Yes
                    Exit Select
                Case Windows.Forms.DialogResult.No
                    Exit Sub
            End Select
        End If
        Dim a As String = TimeOfDay()
        Dim numero = (ListBox1.Items.Count.ToString() - 1) - ListBox1.SelectedIndex
        Dim index As Integer = 0
        CheckBox9.Checked = False
        DISABLEALL()
        PictureBox22.Visible = True
        If Directory.Exists(TextBox1.Text) Then
            If ListBox1.SelectedItem IsNot Nothing Then
                Dim W As Integer = 0
                ProgressBar1.Maximum = numero
                ''''''''''''''''''''''''''''''''
                While index <= numero
                    Application.DoEvents()
                    If STOPIC = True Then
                        STOPIC = False
                        ENABLEALL()
                        PictureBox22.Visible = False
                        MsgBox("ANNULLATO" & vbCrLf & vbCrLf & "Files Creati n. " & index)
                        Label9.Text = Directory.GetFiles(TextBox1.Text).Length & " Files "
                        ProgressBar1.Value = 0
                        Exit Sub
                    Else
                        If rectCropArea.X = Nothing Or rectCropArea.Y = Nothing Or rectCropArea.Width = Nothing Or rectCropArea.Height = Nothing Then
                            ENABLEALL()
                            PictureBox22.Visible = False
                            MsgBox("Seleziona un Rettangolo")
                            Exit Sub
                        Else
                            If CheckBox4.Checked = True Then
                                Application.DoEvents()
                                If CONTROL = True And CONTROL1 = True Then
                                    Dim ratio As String = ListBox1.SelectedItem.ToString()
                                    Dim split2 = ratio.Split(".", 2, StringSplitOptions.RemoveEmptyEntries)
                                    Dim no As String = (TextBox1.Text & "\" & TextBox2.Text & "_" & NumericUpDown1.Text.PadLeft(5, "0")) & "." & split2(1)
                                    If IMAGEN Is Nothing Then
                                        MsgBox("ERRORE! Immagine" & vbCrLf & vbCrLf & "Files Creati n. " & index)
                                        ENABLEALL()
                                        PictureBox22.Visible = False
                                        ProgressBar1.Value = 0
                                        Label9.Text = Directory.GetFiles(TextBox1.Text).Length & " Files "
                                        ProgressBar1.Value = 0
                                        Exit Sub
                                    End If

                                    Dim croBitmap As New Bitmap(IMAGEN, CROPMB, CROPMH)
                                    Dim p As Graphics = Graphics.FromImage(croBitmap)
                                    p.InterpolationMode = Drawing2D.InterpolationMode.HighQualityBicubic
                                    p.PixelOffsetMode = Drawing2D.PixelOffsetMode.HighQuality
                                    p.CompositingQuality = Drawing2D.CompositingQuality.HighQuality
                                    If CheckBox5.Checked = True And CheckBox6.Checked = False Then
                                        p.DrawImage(IMAGEN, New Rectangle(0, 0, CROPMB, CROPMH), IMGRCX + BLX + CROPMX, IMGRCY + BLY - CROPMY, CROPMB, CROPMH, GraphicsUnit.Pixel)
                                    End If
                                    If CheckBox5.Checked = True And CheckBox6.Checked = True Or CheckBox5.Checked = False And CheckBox6.Checked = False Then
                                        p.DrawImage(IMAGEN, New Rectangle(0, 0, CROPMB, CROPMH), IMGRCX + BLX + CROPMX, IMGRCY + BLY + CInt(BLH / 2) - CROPMY, CROPMB, CROPMH, GraphicsUnit.Pixel)
                                    End If
                                    If CheckBox5.Checked = False And CheckBox6.Checked = True Then
                                        p.DrawImage(IMAGEN, New Rectangle(0, 0, CROPMB, CROPMH), IMGRCX + BLX + CROPMX, IMGRCY + BLY + BLH - CROPMY, CROPMB, CROPMH, GraphicsUnit.Pixel)
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
                                        ListBox2.Items.Add(TextBox2.Text & "_" & NumericUpDown1.Text.PadLeft(5, "0") & "." & split2(1))
                                        ListBox2.SelectedIndex = ListBox2.Items.Count.ToString() - 1
                                        index += 1
                                        NumericUpDown1.Value += 1
                                        ProgressBar1.Value = W
                                        If ListBox1.Items.Count.ToString() - 1 <> ListBox1.SelectedIndex Then
                                            ListBox1.SelectedIndex += 1
                                            W += 1
                                        End If
                                        Label9.Text = index & " Files Creati"
                                    End If
                                Else
                                    MsgBox("ERRORE! Verifica Controlli" & vbCrLf & vbCrLf & "Files Creati n. " & index)
                                    ENABLEALL()
                                    PictureBox22.Visible = False
                                    ProgressBar1.Value = 0
                                    Label9.Text = Directory.GetFiles(TextBox1.Text).Length & " Files "
                                    ProgressBar1.Value = 0
                                    Exit Sub
                                End If
                            Else
                                ENABLEALL()
                                PictureBox22.Visible = False
                                MsgBox("Funzione Ritaglio non selezionata!")
                                Exit Sub
                            End If
                        End If

                    End If
                End While
                ENABLEALL()
                PictureBox22.Visible = False
                Dim b As String = TimeOfDay()
                Dim t1 As TimeSpan = TimeSpan.Parse(a)
                Dim t2 As TimeSpan = TimeSpan.Parse(b)
                Dim tDiff As TimeSpan
                tDiff = t2.Subtract(t1)
                Dim days = (tDiff.ToString)
                MsgBox("OPERAZIONE COMPLETATA" & vbCrLf & vbCrLf & "Files Creati n. " & index & vbCrLf & vbCrLf & "Tempo Impiegato: " & days)
                Label9.Text = Directory.GetFiles(TextBox1.Text).Length & " Files "
                ProgressBar1.Value = 0
            Else
                ENABLEALL()
                PictureBox22.Visible = False
                MsgBox("La Cartella di Origine è Vuota")
            End If
        Else
            ENABLEALL()
            PictureBox22.Visible = False
            MsgBox("Selezionare cartella di Destinazione.")
        End If
    End Sub
    ' APRI CARTELLA ORIGINE IN ESPLORA RISORSE
    Private Sub Button19_Click(sender As Object, e As EventArgs) Handles Button19.Click
        If TextBox4.Text <> Nothing Then
            Process.Start("explorer.exe", TextBox4.Text)
        End If
    End Sub
    ' APRI CARTELLA DESTINAZIONE IN EXPLORER
    Private Sub Button23_Click(sender As Object, e As EventArgs) Handles Button23.Click
        If TextBox1.Text <> Nothing Then
            Process.Start("explorer.exe", TextBox1.Text)
        End If
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
    ' AGGIORNA LISTBOX CARTELLA SORGENTE
    Private Sub Button26_Click(sender As Object, e As EventArgs) Handles Button26.Click
        PictureBox1.BackgroundImage = My.Resources.ANTEPRIMA
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
    ' AGGIORNA LISTBOX CARTELLA DESTINAZIONE 
    Private Sub Button27_Click(sender As Object, e As EventArgs) Handles Button27.Click
        PictureBox21.Image = My.Resources.ANTEPRIMA
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
                If ListBox2.Items.Count.ToString() = 0 Then
                    Exit Sub
                Else
                    ListBox2.SelectedIndex = 0
                End If
            End If
        End If
    End Sub
    ' FERMA CREAZIONE RITAGLIO IMMAGINE DA PICTURE
    Private Sub Button28_Click(sender As Object, e As EventArgs) Handles Button28.Click
        If STOPIC <> True Then
            STOPIC = True
            Button28.Enabled = False
            ListBox1.Enabled = True
            ListBox2.Enabled = True
            Button16.Enabled = True
            PictureBox22.Visible = False
        End If
    End Sub
    ' CANCELLA TUTTO IL CONTENUTO DELLA CARTELLA DESTINAZIONE     
    Private Sub Button29_Click(sender As Object, e As EventArgs) Handles Button29.Click
        If ListBox2.Items.Count > 0 Then
            Select Case MsgBox("Cancellare Tutte le Immagini" & vbCrLf & vbCrLf & "nella Cartella di Destinazione?", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                Case Windows.Forms.DialogResult.Yes
                    ListBox2.ClearSelected()
                    If Directory.Exists(TextBox1.Text) Then
                        For Each _file As String In Directory.GetFiles(TextBox1.Text)
                            File.Delete(_file)
                        Next
                    End If
                    ListBox2.Items.Clear()
                    If Directory.Exists(TextBox1.Text) Then
                        Dim filess = Directory.GetFiles(TextBox1.Text)
                        Label9.Text = filess.Length & " Files "
                    End If
                Case Windows.Forms.DialogResult.No
                    Exit Sub
            End Select
        End If
    End Sub
    ' RUOTA IMMAGINE ANTIORARIO
    Private Sub Button31_Click(sender As Object, e As EventArgs) Handles Button31.Click
        angolo += 0.1
        rotate = False
    End Sub
    ' RUOTA IMMAGINE ORARIO
    Private Sub Button32_Click(sender As Object, e As EventArgs) Handles Button32.Click
        angolo -= 0.1
        rotate = False
    End Sub
    ' RIPRISTINA MODIFICHE IMMAGINE
    Private Sub Button30_Click(sender As Object, e As EventArgs) Handles Button30.Click
        flipy = True
        flipx = True
        rotate = True
        angolo = 0
    End Sub
    ' APRI VIEWER INTERNO
    Private Sub Button33_Click(sender As Object, e As EventArgs) Handles Button33.Click
        If ListBox2.Items.Count > 0 Then
            My.Settings.cart_lav = TextBox1.Text
            Form7.Show()
        End If

    End Sub
    ' APRI CREA VIDEO
    Private Sub Button34_Click(sender As Object, e As EventArgs) Handles Button34.Click
        If ListBox2.Items.Count > 0 Then
            If MsgBox("Le Impostazioni non Salvate Andranno Perse!" & vbCrLf & vbCrLf & "Passare a Crea Video?", MsgBoxStyle.YesNo, ) = MsgBoxResult.Yes Then
                Form7.Close()
                CameraStop()
                'Timer2.Stop()
                My.Settings.Form8_Location = New System.Drawing.Point(Me.Location.X, Me.Location.Y)
                My.Settings.Save()
                My.Settings.cam_on = False
                My.Settings.def_cam = ComboBox1.Text
                My.Settings.def_res = ComboBox2.Text
                My.Settings.CA_SOVID = TextBox1.Text
                Form9.Show()
                Me.Close()
            End If
        End If
    End Sub
    ' CAMBIO SELEZIONE LISTA CARTELLA ORIGINE 
    Private Sub ListBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ListBox1.SelectedIndexChanged
        If rename = False Then
            Try
                Dim ratio As String = ListBox1.SelectedItem.ToString()
                Dim split = ratio.Split("_", 2, StringSplitOptions.RemoveEmptyEntries)
                Dim split1 = ratio.Split("_"c, "."c)(1)
                TextBox2.Text = split(0)
                NumericUpDown1.Value = split1
            Catch ex As Exception
                MsgBox("Nome file non Valido" & vbCrLf & vbCrLf & "Formato corretto" & vbCrLf & vbCrLf & "...\Nome_00000.jpg")
            End Try
        End If
    End Sub
    ' RITAGLIA SINGOLO FOTOGRAMMA
    Private Sub Button35_Click(sender As Object, e As EventArgs) Handles Button35.Click
        If ListBox2.Items.Count.ToString() > 0 Then
            Select Case MsgBox("ATTENZIONE!" & vbCrLf & vbCrLf & "la cartella di Destinazione non è vuota." & vbCrLf & vbCrLf & "I Nomi Esistenti saranno Sovrascritti." & vbCrLf & vbCrLf & "Avvio Comunque il Processo?", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                Case Windows.Forms.DialogResult.Yes
                    Exit Select
                Case Windows.Forms.DialogResult.No
                    Exit Sub
            End Select
        End If

        If Directory.Exists(TextBox1.Text) Then
            If ListBox1.SelectedItem IsNot Nothing Then
                If rectCropArea.X = Nothing Or rectCropArea.Y = Nothing Or rectCropArea.Width = Nothing Or rectCropArea.Height = Nothing Then
                    MsgBox("Seleziona un Rettangolo")
                    Exit Sub
                Else
                    If CheckBox4.Checked = True Then
                        If IMAGEN IsNot Nothing Then
                            Dim ratio As String = ListBox1.SelectedItem.ToString()
                            Dim split2 = ratio.Split(".", 2, StringSplitOptions.RemoveEmptyEntries)
                            Dim no As String = (TextBox1.Text & "\" & TextBox2.Text & "_" & NumericUpDown1.Text.PadLeft(5, "0")) & "." & split2(1)
                            Dim croBitmap As New Bitmap(IMAGEN, CROPMB, CROPMH)
                            Dim p As Graphics = Graphics.FromImage(croBitmap)
                            p.InterpolationMode = Drawing2D.InterpolationMode.HighQualityBicubic
                            p.PixelOffsetMode = Drawing2D.PixelOffsetMode.HighQuality
                            p.CompositingQuality = Drawing2D.CompositingQuality.HighQuality
                            If CheckBox5.Checked = True And CheckBox6.Checked = False Then
                                p.DrawImage(IMAGEN, New Rectangle(0, 0, CROPMB, CROPMH), IMGRCX + BLX + CROPMX, IMGRCY + BLY - CROPMY, CROPMB, CROPMH, GraphicsUnit.Pixel)
                            End If
                            If CheckBox5.Checked = True And CheckBox6.Checked = True Or CheckBox5.Checked = False And CheckBox6.Checked = False Then
                                p.DrawImage(IMAGEN, New Rectangle(0, 0, CROPMB, CROPMH), IMGRCX + BLX + CROPMX, IMGRCY + BLY + CInt(BLH / 2) - CROPMY, CROPMB, CROPMH, GraphicsUnit.Pixel)
                            End If
                            If CheckBox5.Checked = False And CheckBox6.Checked = True Then
                                p.DrawImage(IMAGEN, New Rectangle(0, 0, CROPMB, CROPMH), IMGRCX + BLX + CROPMX, IMGRCY + BLY + BLH - CROPMY, CROPMB, CROPMH, GraphicsUnit.Pixel)
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
                            ListBox2.Items.Add(TextBox2.Text & "_" & NumericUpDown1.Text.PadLeft(5, "0") & "." & split2(1))
                            ListBox2.SelectedIndex = ListBox2.Items.Count.ToString() - 1
                            Label9.Text = Directory.GetFiles(TextBox1.Text).Length & " Files "
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
    ' CREA RETTANGOLO SELEZIONE BOTTONE PREMUTO MOUSE 
    Private Sub PictureBox1_MouseDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles PictureBox1.MouseDown
        If AVA = True Then
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
                PictureBox1.Image = Nothing
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
    Private Sub PictureBox1_MouseMove(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles PictureBox1.MouseMove
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
                    Timer2.Stop()
                    OCC = 1
                    PictureBox1.Refresh()
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
                    Timer2.Stop()
                    OCC = 1
                    PictureBox1.Refresh()
                End If
            End If
        End If
    End Sub
    ' CREA RETTANGOLO SELEZIONE BOTTONE RILASCIATO MOUSE
    Private Sub PictureBox1_MouseUp(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles PictureBox1.MouseUp
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
            PictureBox1.Refresh()
            Timer2.Start()
        End If
    End Sub
    ' BOTTONE APRE ZOOM IMMAGINE IN CAMERA PREVIEW
    Private Sub Button36_Click(sender As Object, e As EventArgs) Handles Button36.Click
        ZOOMON = True
        Button36.Visible = False
        Button37.Visible = True
        NumericUpDown2.Visible = True
        Button42.Visible = True
    End Sub
    ' BOTTONI MUOVI IMMAGINE ZOOM
    Private Sub Button43_Click(sender As Object, e As EventArgs) Handles Button43.Click
        ZOOMX = 0
        ZOOMY = 0
        TextBox3.Text = ZOOMX
        TextBox5.Text = ZOOMY
    End Sub
    Private Sub Button41_Click(sender As Object, e As EventArgs) Handles Button41.Click
        ZOOMY -= 10
        TextBox5.Text = ZOOMY
    End Sub
    Private Sub Button38_Click(sender As Object, e As EventArgs) Handles Button38.Click
        ZOOMY += 10
        TextBox5.Text = ZOOMY
    End Sub
    Private Sub Button39_Click(sender As Object, e As EventArgs) Handles Button39.Click
        ZOOMX += 10
        TextBox3.Text = ZOOMX
    End Sub
    Private Sub Button40_Click(sender As Object, e As EventArgs) Handles Button40.Click
        ZOOMX -= 10
        TextBox3.Text = ZOOMX
    End Sub
    'MUOVI IMMAGINE ZOOM INSERIMENTO TESTO
    Private Sub TextBox3_KeyPress(ByVal sender As Object, ByVal e As KeyPressEventArgs) Handles TextBox3.KeyPress
        If Asc(e.KeyChar) <> 8 And Asc(e.KeyChar) <> 45 Then
            If Asc(e.KeyChar) < 48 Or Asc(e.KeyChar) > 57 Then
                e.Handled = True
            End If
        End If
    End Sub
    Private Sub TextBox3_LostFocus(sender As Object, e As EventArgs) Handles TextBox3.LostFocus
        ZOOMX = TextBox3.Text
    End Sub
    Private Sub TextBox5_LostFocus(sender As Object, e As EventArgs) Handles TextBox5.LostFocus
        ZOOMY = TextBox5.Text
    End Sub
    ' AZZERA CONTATORE PASSAGGI SOLO IN AVANZATO
    Private Sub PictureBox16_DoubleClick(sender As Object, e As EventArgs) Handles PictureBox16.DoubleClick
        n = 0
        ToolTip1.SetToolTip(PictureBox16, n & " Fotogrammi OK" & vbCrLf & "Doppio Clik Azzera")
        PictureBox16.BackgroundImage = My.Resources.CONTER

    End Sub
    ' ALLINEA IN BASSO CONTROLLO VISIVO
    Private Sub Button47_Click(sender As Object, e As EventArgs) Handles Button47.Click
        If CheckBox6.Checked = True Then
            CheckBox6.Checked = False
        Else
            CheckBox6.Checked = True
        End If
    End Sub
    ' ALLINEA IN ALTO CONTROLLO VISIVO
    Private Sub Button46_Click(sender As Object, e As EventArgs) Handles Button46.Click
        If CheckBox5.Checked = True Then
            CheckBox5.Checked = False
        Else
            CheckBox5.Checked = True
        End If
    End Sub
    'CHIUDE FINESTRA POSIZIONA ZOOM
    Private Sub Button44_Click(sender As Object, e As EventArgs) Handles Button44.Click
        Button42.PerformClick()
    End Sub
    ' ATTIVA COMANDI MUOVI IMMAGINE ZOOM
    Private Sub Button42_Click(sender As Object, e As EventArgs) Handles Button42.Click
        If ZOOMOV = False Then
            Button42.BackColor = Color.DarkOrange
            Panel18.Visible = True
            Button44.Visible = True
            ZOOMOV = True
        Else
            Button42.BackColor = Color.Transparent
            Panel18.Visible = False
            Button44.Visible = False
            ZOOMOV = False
        End If
    End Sub
    ' ATTIVA DISATTIVA IMMAGINE CAMERA DIRETTA
    Private Sub Button45_Click(sender As Object, e As EventArgs) Handles Button45.Click
        If real = True Then
            Button45.BackColor = Color.Transparent
            ButtonStop.Enabled = True
            ButtonStart.Enabled = True
            Button2.Enabled = True
            Button3.Enabled = True
            Button17.Enabled = True
            Button36.Enabled = True
            Button37.Enabled = True
            Button42.Enabled = True
            NumericUpDown2.Enabled = True
            My.Settings.SaveTitle21 = False
            ToolTip1.SetToolTip(Button45, "Modalità Diretta")
            real = False
            Thread.Sleep(500)
            OCC = 0
        Else
            Button45.BackColor = Color.Red
            ButtonStop.Enabled = False
            ButtonStart.Enabled = False
            Button2.Enabled = False
            Button3.Enabled = False
            Button17.Enabled = False
            Button36.Enabled = False
            Button37.Enabled = False
            Button42.Enabled = False
            NumericUpDown2.Enabled = False
            My.Settings.SaveTitle21 = True
            ToolTip1.SetToolTip(Button45, "Esci Modalità Diretta")
            real = True
            Thread.Sleep(500)
            IMAGEN = Nothing
        End If
    End Sub
    ' BOTTONE CHIUDI ZOOM IMMAGINE IN CAMERA PREVIEW
    Private Sub Button37_Click(sender As Object, e As EventArgs) Handles Button37.Click
        If ZOOMOV = True Then
            Button42.BackColor = Color.Transparent
            Panel18.Visible = False
            Button44.Visible = False
            ZOOMOV = False
        End If

        ZOOMON = False
        Button36.Visible = True
        Button37.Visible = False
        NumericUpDown2.Visible = False
        Button42.Visible = False
    End Sub
    ' DISEGNA RETTANGOLO SELEZIONE
    Private Sub PicBox_Paint(ByVal sender As System.Object, ByVal e As System.Windows.Forms.PaintEventArgs) Handles PictureBox1.Paint
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
                Dim drawLine As New Pen(Color.Red, 3)
                drawLine.DashStyle = DashStyle.Dash
                e.Graphics.DrawRectangle(drawLine, rectCropArea)
            End If
        End If
    End Sub
    ' ATTIVA / DISATTIVA MODALITA ELABORA IMMAGINI ESISTENTI
    Private Sub Button17_Click(sender As Object, e As EventArgs) Handles Button17.Click
        If picmod <> True Then
            If AVA = False Then
                Button3.PerformClick()
            End If
            If ZOOMON = True Then
                Button37.PerformClick()
            End If
            Panel17.Visible = True
            Button36.Visible = False
            PictureBox16.Visible = False
            Me.Width = Me.MinimumSize.Width
            Me.Height = Me.MinimumSize.Height
            Panel12.Anchor = AnchorStyles.Top Or AnchorStyles.Left
            Me.MinimumSize = New Size(Me.Width, Me.Height * 1.75)
            CameraStop()
            IMAGEN = Nothing
            'Timer1.Start()
            ButtonStart.Enabled = False
            ComboBox1.Enabled = False
            ComboBox2.Enabled = False
            ButtonStop.Enabled = True
            Button2.Enabled = True
            Button36.Enabled = True
            Panel14.Visible = True
            picmod = True
            Button17.BackColor = Color.Red
            My.Settings.SaveTitle21 = True
            ToolTip1.SetToolTip(Button17, "Torna a Modalità Camera")
            TextBox4.Text = My.Settings.ORIG_CART
            TextBox1.Text = My.Settings.DEST_CART
            Label8.Text = 0 & " Files "
            If TextBox4.Text <> "" Then
                If Directory.Exists(TextBox4.Text) Then
                    Dim files() As String = Directory.GetFiles(TextBox4.Text)
                    ListBox1.Items.Clear()
                    For Each file As String In files
                        ListBox1.Items.Add(Path.GetFileName(file))
                    Next
                    Label8.Text = files.Length & " Files "
                    If ListBox1.Items.Count.ToString() = 0 Then
                        PictureBox1.Image = My.Resources.ANTEPRIMA
                    Else
                        ListBox1.SelectedIndex = 0
                    End If
                End If
            End If
            Label9.Text = 0 & " Files "
            If TextBox1.Text <> "" Then
                If Directory.Exists(TextBox1.Text) Then
                    Dim files2() As String = Directory.GetFiles(TextBox1.Text)
                    ListBox2.Items.Clear()
                    For Each file2 As String In files2
                        ListBox2.Items.Add(Path.GetFileName(file2))
                    Next
                    Label9.Text = files2.Length & " Files "
                    If ListBox2.Items.Count.ToString() = 0 Then
                        PictureBox21.Image = My.Resources.ANTEPRIMA
                    Else
                        ListBox2.SelectedIndex = 0
                    End If
                Else
                    TextBox1.Text = ""
                End If
            Else
                PictureBox21.Image = My.Resources.ANTEPRIMA
            End If
        Else
            Panel17.Visible = False
            Button36.Visible = True
            PictureBox16.Visible = True
            Me.Width = Me.MinimumSize.Width
            Me.Height = Me.MinimumSize.Height
            Panel12.Anchor = AnchorStyles.Top Or AnchorStyles.Left
            Me.MinimumSize = New Size(Me.Width, Me.Height / 1.75)
            Me.Height = Me.Height / 1.75
            ButtonStart.Enabled = True
            ComboBox1.Enabled = True
            ComboBox2.Enabled = True
            Panel14.Visible = False
            ButtonStop.Enabled = False
            Button2.Enabled = False
            Button36.Enabled = False
            picmod = False
            Button17.BackColor = Color.Transparent
            My.Settings.SaveTitle21 = False
            ToolTip1.SetToolTip(Button17, "Modalità Immagini")
            Timer2.Stop()
            CameraStart()
            OCC = 0
            Thread.Sleep(My.Settings.PAUSA)
            Timer2.Start()
        End If
    End Sub
    ' SELEZIONA CARTELLA DI DESTINAZIONE IN AVANZATO
    Private Sub Button14_Click(sender As Object, e As EventArgs) Handles Button14.Click
        Dim folderDlg As New FolderBrowserDialog
        folderDlg.SelectedPath = TextBox1.Text
        folderDlg.ShowNewFolderButton = True
        PictureBox21.Image = My.Resources.ANTEPRIMA
        Label9.Text = 0 & " File "
        If (folderDlg.ShowDialog() = DialogResult.OK) Then
            TextBox1.Text = folderDlg.SelectedPath
            If TextBox4.Text = TextBox1.Text Then
                MessageBox.Show("la cartella di Origine non puo essere" & vbCrLf & vbCrLf & "la cartella di Destinazione")
                TextBox1.Text = ""
                ListBox2.Items.Clear()
                PictureBox21.Image = My.Resources.ANTEPRIMA
                Button14.PerformClick()
                Exit Sub
            End If

            My.Settings.DEST_CART = TextBox1.Text

            If TextBox1.Text <> "" Then
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
                    If ListBox2.Items.Count.ToString() = 0 Then
                        PictureBox21.Image = My.Resources.ANTEPRIMA
                    Else
                        ListBox2.SelectedIndex = 0
                    End If
                End If
            Else
                PictureBox21.Image = My.Resources.ANTEPRIMA
            End If
        End If
    End Sub
    ' ATTIVA/DISATTIVA RINOMINA IMMAGINI DESTINAZIONE IN MODALITA' AVANZATA 
    Private Sub Button15_Click(sender As Object, e As EventArgs) Handles Button15.Click
        If ListBox1.SelectedIndex > -1 Then
            If rename = True Then
                TextBox2.ReadOnly = True
                NumericUpDown1.ReadOnly = True
                NumericUpDown1.Enabled = False
                Try
                    Dim ratio As String = ListBox1.SelectedItem
                    Dim split = ratio.Split("_", 2, StringSplitOptions.RemoveEmptyEntries)
                    Dim split1 = ratio.Split("_"c, "."c)(1)
                    TextBox2.Text = split(0)
                    NumericUpDown1.Value = split1
                Catch ex As Exception
                    MsgBox("Nome file non Valido" & vbCrLf & vbCrLf & "Formato corretto" & vbCrLf & vbCrLf & "File_00000.jpg")
                End Try
                rename = False
            Else
                TextBox2.ReadOnly = False
                NumericUpDown1.ReadOnly = False
                NumericUpDown1.Enabled = True
                NumericUpDown1.Value = 1
                rename = True
            End If
        End If

    End Sub
    ' SELEZIONA CARTELLA DI ORIGINE IN AVANZATO
    Private Sub Button18_Click(sender As Object, e As EventArgs) Handles Button18.Click
        Dim folderDlg As New FolderBrowserDialog
        folderDlg.SelectedPath = TextBox4.Text
        folderDlg.ShowNewFolderButton = True
        PictureBox1.BackgroundImage = My.Resources.ANTEPRIMA
        Label8.Text = 0 & " File "
        If (folderDlg.ShowDialog() = DialogResult.OK) Then
            TextBox4.Text = folderDlg.SelectedPath
            If TextBox4.Text = TextBox1.Text Then
                MessageBox.Show("la cartella di Origine non puo essere" & vbCrLf & vbCrLf & "la cartella di Destinazione")
                TextBox4.Text = ""
                ListBox1.Items.Clear()
                Button18.PerformClick()
                Exit Sub
            Else
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
                            If rename = False Then
                                Try
                                    Dim ratio As String = ListBox1.SelectedItem.ToString()
                                    Dim split = ratio.Split("_", 2, StringSplitOptions.RemoveEmptyEntries)
                                    Dim split1 = ratio.Split("_"c, "."c)(1) ',(StringSplitOptions.RemoveEmptyEntries)
                                    'Dim split2 = ratio.Split(".", 2, StringSplitOptions.RemoveEmptyEntries)
                                    TextBox2.Text = split(0)
                                    NumericUpDown1.Value = split1
                                Catch ex As Exception
                                    MsgBox("Nome file non Valido" & vbCrLf & vbCrLf & "Formato corretto" & vbCrLf & vbCrLf & "File_00000.jpg")
                                End Try
                            End If
                        End If
                    End If
                End If
            End If
            My.Settings.ORIG_CART = TextBox4.Text
        End If
    End Sub
    ' SELEZIONA E VISUALIZZA IMMAGINI CARTELLA DESTINAZIONE
    Private Sub ListBox2_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ListBox2.SelectedIndexChanged
        Try
            PictureBox21.Load(TextBox1.Text & "/" & ListBox2.SelectedItem)
        Catch ex As Exception
            PictureBox21.Image = My.Resources.ANTEPRIMA
        End Try
    End Sub
    ' REGOLA POSIZIONE E DIMENSIONE RETTANGOLO RITAGLIO 
    Private Sub Button8_MouseDown(sender As Object, e As MouseEventArgs) Handles Button8.MouseDown
        mouseIsDown = True
        Do
            CROPMX += 2
            Thread.Sleep(25)
            Application.DoEvents()
        Loop While mouseIsDown

    End Sub
    Private Sub Button8_MouseUp(sender As Object, e As MouseEventArgs) Handles Button8.MouseUp
        mouseIsDown = False
    End Sub
    Private Sub Button10_MouseDown(sender As Object, e As MouseEventArgs) Handles Button10.MouseDown
        mouseIsDown = True
        Do
            CROPMX -= 2
            Thread.Sleep(25)
            Application.DoEvents()
        Loop While mouseIsDown

    End Sub
    Private Sub Button10_MouseUp(sender As Object, e As MouseEventArgs) Handles Button10.MouseUp
        mouseIsDown = False
    End Sub
    Private Sub Button5_MouseDown(sender As Object, e As MouseEventArgs) Handles Button5.MouseDown
        mouseIsDown = True
        Do
            CROPMY += 2
            Thread.Sleep(25)
            Application.DoEvents()
        Loop While mouseIsDown

    End Sub
    Private Sub Button5_MouseUp(sender As Object, e As MouseEventArgs) Handles Button5.MouseUp
        mouseIsDown = False
    End Sub
    Private Sub Button12_MouseDown(sender As Object, e As MouseEventArgs) Handles Button12.MouseDown
        mouseIsDown = True
        Do
            CROPMY -= 2
            Thread.Sleep(25)
            Application.DoEvents()
        Loop While mouseIsDown

    End Sub
    Private Sub Button12_MouseUp(sender As Object, e As MouseEventArgs) Handles Button12.MouseUp
        mouseIsDown = False
    End Sub
    Private Sub Button20_MouseDown(sender As Object, e As MouseEventArgs) Handles Button20.MouseDown
        mouseIsDown = True
        Do
            CROPMB += 2
            Label4.Text = CROPMB
            Label6.Text = CROPMH
            Thread.Sleep(25)
            Application.DoEvents()
        Loop While mouseIsDown

    End Sub
    Private Sub Button20_MouseUp(sender As Object, e As MouseEventArgs) Handles Button20.MouseUp
        mouseIsDown = False
    End Sub
    Private Sub Button22_MouseDown(sender As Object, e As MouseEventArgs) Handles Button22.MouseDown
        mouseIsDown = True
        Do
            CROPMH += 2
            Label4.Text = CROPMB
            Label6.Text = CROPMH
            Thread.Sleep(25)
            Application.DoEvents()
        Loop While mouseIsDown
    End Sub
    Private Sub Button22_MouseUp(sender As Object, e As MouseEventArgs) Handles Button22.MouseUp
        mouseIsDown = False
    End Sub
    Private Sub Button9_MouseDown(sender As Object, e As MouseEventArgs) Handles Button9.MouseDown
        mouseIsDown = True
        Do
            CROPMH -= 2
            Label4.Text = CROPMB
            Label6.Text = CROPMH
            Thread.Sleep(25)
            Application.DoEvents()
        Loop While mouseIsDown

    End Sub
    Private Sub Button9_MouseUp(sender As Object, e As MouseEventArgs) Handles Button9.MouseUp
        mouseIsDown = False
    End Sub
    Private Sub Button21_MouseDown(sender As Object, e As MouseEventArgs) Handles Button21.MouseDown
        mouseIsDown = True
        Do
            CROPMB -= 2
            Label4.Text = CROPMB
            Label6.Text = CROPMH
            Thread.Sleep(25)
            Application.DoEvents()
        Loop While mouseIsDown

    End Sub
    Private Sub Button21_MouseUp(sender As Object, e As MouseEventArgs) Handles Button21.MouseUp
        mouseIsDown = False
    End Sub
    ' APRI/SELEZIONA VIEWER ESTERNO CARTELLA ORIGINE 
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
    ' APRI/SELEZIONA VIEWER ESTERNO CARTELLA DESTINAZIONE 
    Private Sub Button25_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles Button25.MouseDown
        If ListBox2.Items.Count = Nothing Then
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
                Process.Start(VIEW, TextBox1.Text & "\" & ListBox2.SelectedItem)
            End If
        End If
    End Sub
    ' INFORMAZIONI FILE CARTELLA ORIGINE
    Private Sub ListBox1_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles ListBox1.MouseDown
        If ListBox1.Items.Count = Nothing Then
            Exit Sub
        End If
        If e.Button = MouseButtons.Right Then
            ListBox1.SelectedIndex = ListBox1.IndexFromPoint(e.X, e.Y)
            Dim MyimageWxH
            Try
                Dim Myimg As System.Drawing.Image = System.Drawing.Image.FromFile(TextBox4.Text & "\" & ListBox1.SelectedItem)
                Dim MyImageWidth = PictureBox1.BackgroundImage.Width
                Dim MyImageHeight = PictureBox1.BackgroundImage.Height
                MyimageWxH = MyImageWidth & " X " & MyImageHeight
            Catch
                MyimageWxH = "Not Supported"
            End Try
            Dim Myimagepropety = FileLen(TextBox4.Text & "\" & ListBox1.SelectedItem)
            Dim Myimagedate As DateTime = File.GetCreationTime(TextBox4.Text & "\" & ListBox1.SelectedItem)
            MsgBox("Informazioni:" & vbCrLf & vbCrLf & TextBox4.Text & "\" & ListBox1.SelectedItem & vbCrLf & vbCrLf & " Data Creazione:  " & Myimagedate & vbCrLf & vbCrLf & " Formato:  " & MyimageWxH & vbCrLf & vbCrLf & " Dimensione:  " & System.Math.Round(Myimagepropety / 1000, 1) & " KB", MessageBoxIcon.Information)
        End If
    End Sub
    ' INFORMAZIONI FILE CARTELLA DESTINAZIONE
    Private Sub ListBox2_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles ListBox2.MouseDown
        If ListBox2.Items.Count = Nothing Then
            Exit Sub
        End If
        If e.Button = MouseButtons.Right Then
            ListBox2.SelectedIndex = ListBox2.IndexFromPoint(e.X, e.Y)
            Dim MyimageWxH
            Try
                Dim Myimg As System.Drawing.Image = System.Drawing.Image.FromFile(TextBox1.Text & "\" & ListBox2.SelectedItem)
                Dim MyImageWidth = PictureBox21.Image.Width
                Dim MyImageHeight = PictureBox21.Image.Height
                MyimageWxH = MyImageWidth & " X " & MyImageHeight
            Catch
                MyimageWxH = "Not Supported"
            End Try
            Dim Myimagepropety = FileLen(TextBox1.Text & "\" & ListBox2.SelectedItem)
            Dim Myimagedate As DateTime = File.GetCreationTime(TextBox1.Text & "\" & ListBox2.SelectedItem)
            MsgBox("Informazioni:" & vbCrLf & vbCrLf & TextBox1.Text & "\" & ListBox2.SelectedItem & vbCrLf & vbCrLf & " Data Creazione:  " & Myimagedate & vbCrLf & vbCrLf & " Formato:  " & MyimageWxH & vbCrLf & vbCrLf & " Dimensione:  " & System.Math.Round(Myimagepropety / 1000, 1) & " KB", MessageBoxIcon.Information)
        End If
    End Sub
    ' DISABILITA TUTTI I COMANDI E ABILITA BOTTONE FERMA
    Private Sub DISABLEALL()
        For Each ctrl As Control In Panel1.Controls
            ctrl.Enabled = False
        Next
        For Each ctrl As Control In Panel16.Controls
            ctrl.Enabled = False
        Next
        For Each ctrl As Control In Panel17.Controls
            ctrl.Enabled = False
        Next

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
        Button33.Enabled = False
        Button34.Enabled = False
        Button35.Enabled = False
        Button16.Enabled = False
        ButtonStop.Enabled = False
        Button2.Enabled = False
        Button36.Enabled = False
        Button3.Enabled = False
        Button17.Enabled = False
        ListBox1.Enabled = False
        ListBox2.Enabled = False
        Button28.Enabled = True
        If rename = True Then
            TextBox2.Enabled = False
            NumericUpDown1.Enabled = False
        End If


    End Sub
    ' ABILITA TUTTI I COMANDI E DISABILITA BOTTONE FERMA
    Private Sub ENABLEALL()
        For Each ctrl As Control In Panel1.Controls
            ctrl.Enabled = True
        Next
        For Each ctrl As Control In Panel16.Controls
            ctrl.Enabled = True
        Next
        For Each ctrl As Control In Panel17.Controls
            ctrl.Enabled = True
        Next
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
        Button33.Enabled = True
        Button34.Enabled = True
        Button35.Enabled = True
        Button16.Enabled = True
        ButtonStop.Enabled = True
        Button2.Enabled = True
        Button3.Enabled = True
        Button36.Enabled = True
        Button17.Enabled = True
        If rename = True Then
            TextBox2.Enabled = True
            NumericUpDown1.Enabled = True
        End If
        ListBox1.Enabled = True
        ListBox2.Enabled = True
        Button28.Enabled = False
    End Sub
End Class

