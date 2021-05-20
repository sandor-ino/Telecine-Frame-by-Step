'TELECINE FRAME BY STEP 
'Control pc click and Arduino with Visual Basic 
'SANDUINO SOFTWARE '2018'

Imports System.IO.Ports
Imports System.Threading
Imports System.Math
Public Class Form1
    Shared _serialPort As SerialPort
    Shared _continue As Boolean
    Dim LED_ON As Boolean
    Dim DC_ON As Boolean
    Dim MODO As Boolean
    Dim ANNULLA As Boolean = False
    Dim CONNETTI As Boolean = False
    Dim s8_n8 As String = "0"
    Dim Card As String

    'SIMULAZIONE DI CLICK DEL MOUSE 
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
    Declare Sub mouse_event Lib "user32" (ByVal dwFlags As MeFlags, ByVal Coords As Drawing.Point, ByVal dwData As Integer, ByVal dwExtraInfo As UIntPtr)
    Sub SimulateClick(ByVal Location As Drawing.Point)
        Dim trect As Drawing.Rectangle = Screen.GetBounds(Location)
        Dim tpnt As New Drawing.Point(65535.0 / trect.Width * Location.X, 65535.0 / trect.Height * Location.Y)
        mouse_event(MeFlags.MOUSEEVENTF_MOVE Or MeFlags.MOUSEEVENTF_ABSOLUTE, tpnt, 0, New UIntPtr(Convert.ToUInt32(0)))
        mouse_event(MeFlags.MOUSEEVENTF_LEFTDOWN Or MeFlags.MOUSEEVENTF_ABSOLUTE, tpnt, 0, New UIntPtr(Convert.ToUInt32(0)))
        mouse_event(MeFlags.MOUSEEVENTF_LEFTUP Or MeFlags.MOUSEEVENTF_ABSOLUTE, tpnt, 0, New UIntPtr(Convert.ToUInt32(0)))
    End Sub
    'CHIUSURA PROGRAMMA
    Private Sub Form1_FormClosing(sender As Object, e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        My.Settings.Form1_Location = New System.Drawing.Point(Me.Location.X, Me.Location.Y)
        My.Settings.Save()
        Select Case MessageBox.Show("Sicuro di voler uscire?", "Conferma Chiusura", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
            Case Windows.Forms.DialogResult.Yes
                Form8.Close()
                Form7.Close()
                My.Settings.inizio = NumericUpDown4.Text
                My.Settings.cart_lav = TextBox2.Text
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
            Case Windows.Forms.DialogResult.No
                e.Cancel = True
        End Select
    End Sub
    'AVVIO PROGRAMMA
    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Me.Location = New System.Drawing.Point(My.Settings.Form1_Location.X, My.Settings.Form1_Location.Y)
        For Each sp As String In My.Computer.Ports.SerialPortNames
            ComboBox1.Items.Add(sp)
        Next
        LED_ON = True
        DC_ON = True
        TrackBar1.Enabled = False
        TrackBar2.Enabled = False
        Card = My.Settings.cart_ardu
        'CARICA IMPOSTAZIONI SALVATE setup
        TextBox5.Text = My.Settings.SaveTitle3
        TextBox6.Text = My.Settings.SaveTitle4
        TextBox7.Text = My.Settings.SaveTitle5
        TextBox8.Text = My.Settings.SaveTitle6
        TextBox9.Text = My.Settings.SaveTitle22
        ComboBox1.Text = My.Settings.SaveTitle24
        TextBox27.Text = My.Settings.PAUSA

        If Not IO.Directory.Exists(My.Settings.cart_lav) Then
            TextBox2.Text = ""
        Else
            TextBox2.Text = My.Settings.cart_lav
        End If

        TextBox4.Text = My.Settings.nome
        NumericUpDown4.Text = My.Settings.inizio
        ComboBox2.Text = My.Settings.file_ext
        'DISABILITA BOTTONI
        Button17.Enabled = False
        Button15.Enabled = False
        Button1.Enabled = False
        Button3.Enabled = False
        Button4.Enabled = False
        Button6.Enabled = False
        Button8.Enabled = False
        Button11.Enabled = False
        Button20.Enabled = False
        Button2.Enabled = False
        NumericUpDown1.Enabled = False
        NumericUpDown2.Enabled = False
        NumericUpDown3.Enabled = False
        'Button5.Enabled = False
        My.Settings.cam_on = False
        Button22.PerformClick()
        Button9.Visible = False
    End Sub
    'TASTO AVVIO
    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        Dim Numero = NumericUpDown1.Value
        Dim index As Integer = 1
        Dim cont As Integer
        Dim mess1 As Integer
        Dim Secondi As Integer, Minuti As Integer, Ore As Integer
        Dim tempo As Long, mio As String
        If MODO <> True Then
            'TASTO AVVIO MODO CLICK
            If TextBox5.Text = "" Then
                MsgBox("Configurare Coordinate CLICK in Setup", MessageBoxIcon.Exclamation)
                Exit Sub
            End If
            If TextBox6.Text = "" Then
                MsgBox("Configurare Coordinate CLICK in Setup", MessageBoxIcon.Exclamation)
                Exit Sub
            End If
            Button1.BackgroundImage = My.Resources.avvioverde
            Button1.Enabled = False
            Button3.Enabled = False
            Button4.Enabled = False
            Button6.Enabled = False
            Button8.Enabled = False
            Button11.Enabled = False
            Button20.Enabled = False
            Button2.Enabled = True
            Button2.FlatAppearance.BorderSize = 2
            NumericUpDown1.ReadOnly = True
            Label2.Text = TimeOfDay()
            My.Computer.Audio.PlaySystemSound(System.Media.SystemSounds.Asterisk)
            While index <= Numero
                Application.DoEvents()
                If ANNULLA = True Then
                    Button1.BackgroundImage = My.Resources.avviorosso
                    If Not SerialPort1.IsOpen Then SerialPort1.Open()
                    SerialPort1.Write("6")
                    SerialPort1.Close()
                    Label9.Text = -Numero - 1 + index
                    MsgBox("ANNULLATO" & vbCrLf & vbCrLf & "Ora Inizio " & Label2.Text() & vbCrLf & vbCrLf & "Ora Fine " & TimeOfDay() & vbCrLf & vbCrLf & "Click OK  " & (NumericUpDown1.Value + Label9.Text), MessageBoxIcon.Exclamation)
                    Label9.Text = ("Click n.")
                    Label9.ForeColor = Color.Black
                    Button1.BackgroundImage = My.Resources.avvio
                    Label7.Text = ""
                    If Not SerialPort1.IsOpen Then SerialPort1.Open()
                    SerialPort1.DiscardInBuffer()
                    SerialPort1.Close()
                    Label2.Text = ""
                    ANNULLA = False
                    Button2.BackgroundImage = My.Resources.stop_nero
                    Button7.BackgroundImage = My.Resources.reset_contatori
                    ToolTip1.SetToolTip(Button7, "Reset Contatori")
                    Button1.Enabled = True
                    Button3.Enabled = True
                    Button4.Enabled = True
                    Button6.Enabled = True
                    Button8.Enabled = True
                    Button11.Enabled = True
                    Button20.Enabled = True
                    Button2.Enabled = False
                    Button2.FlatAppearance.BorderSize = 0
                    Exit Sub
                End If


                Label9.Text = ("")
                Label9.ForeColor = Color.Red
                Label9.Text = -Numero + index
                tempo = (CInt(-Label9.Text) * (CInt(TextBox27.Text) + CInt(TextBox7.Text)) / 1000)
                Secondi = tempo Mod 60
                tempo = Int(tempo / 60)
                Minuti = tempo Mod 60
                tempo = Int(tempo / 60)
                Ore = tempo
                mio = Format(Ore, "00") & ":" & Format(Minuti, "00") & ":" & Format(Secondi, "00")
                Label7.Text = mio
                index += 1
                Dim pnt As Drawing.Point
                pnt = New Point(TextBox6.Text, TextBox5.Text)
                SimulateClick(pnt)
                Thread.Sleep(TextBox27.Text) 'attesa click
                TextBox1.Text += 1
                cont = CInt(TextBox1.Text)
                Button1.BackgroundImage = My.Resources.avvioarancio
                Application.DoEvents()
                If Not SerialPort1.IsOpen Then SerialPort1.Open()
                SerialPort1.Write(s8_n8)
                SerialPort1.Write(CInt(TextBox1.Text))
                Try
                    mess1 = SerialPort1.ReadLine()
                Catch ex As Exception
                    Button1.BackgroundImage = My.Resources.avviorosso
                    If Not SerialPort1.IsOpen Then SerialPort1.Open()
                    SerialPort1.Write("6")
                    SerialPort1.Close()
                    Label9.Text = -Numero - 1 + index
                    MsgBox("ERRORE: Nessuna Risposta" & vbCrLf & vbCrLf & "Disconnessione di Arduino" & vbCrLf & vbCrLf & "Resettare o Ricaricare lo Sketch", MessageBoxIcon.Exclamation)
                    MsgBox("ANNULLAMENTO AUTOMATICO" & vbCrLf & vbCrLf & "Ora Inizio " & Label2.Text() & vbCrLf & vbCrLf & "Ora Fine " & TimeOfDay() & vbCrLf & vbCrLf & "Click OK  " & (NumericUpDown1.Value + Label9.Text), MessageBoxIcon.Exclamation)
                    Label9.ForeColor = Color.Black
                    Label9.Text = ("Click n.")
                    Button22.BackgroundImage = My.Resources.usb_off2
                    Button1.BackgroundImage = My.Resources.avvio
                    Label7.Text = ""
                    Label2.Text = ""
                    Button1.Enabled = False
                    Button3.Enabled = False
                    Button4.Enabled = False
                    Button6.Enabled = False
                    Button8.Enabled = False
                    Button11.Enabled = False
                    Button20.Enabled = False
                    Button2.Enabled = True
                    Button2.FlatAppearance.BorderSize = 2
                    ComboBox1.Enabled = True
                    NumericUpDown1.ReadOnly = False
                    Exit Sub
                End Try
                SerialPort1.DiscardInBuffer()
                SerialPort1.Close()
                Thread.Sleep(TextBox7.Text) 'attesa movimento motori 
                Label1.Text = mess1
                Button1.BackgroundImage = My.Resources.avvioverde
                Application.DoEvents()
            End While
            If CheckBox1.Checked = True Then
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
                    MsgBox("Spegnimento Annullato", MessageBoxIcon.Information)
                End If

            End If

            If Not SerialPort1.IsOpen Then SerialPort1.Open()
            Button1.BackgroundImage = My.Resources.avviorosso
            SerialPort1.Write("6")
            SerialPort1.Close()
            Label9.Text = 0
            Dim a As String = Label2.Text()
            Dim b As String = TimeOfDay()
            Dim t1 As TimeSpan = TimeSpan.Parse(a)
            Dim t2 As TimeSpan = TimeSpan.Parse(b)
            Dim tDiff As TimeSpan
            tDiff = t2.Subtract(t1)
            Dim days = (tDiff.ToString)
            Label7.Text = days
            MsgBox("FINE" & vbCrLf & vbCrLf & "Ora Inizio " & Label2.Text() & vbCrLf & vbCrLf & "Ora Fine " & TimeOfDay() & vbCrLf & vbCrLf & "Durata " & days & vbCrLf & vbCrLf & "Click OK  " & (NumericUpDown1.Value), MessageBoxIcon.Information)
            Label9.Text = ("Click n.")
            Label9.ForeColor = Color.Black
            Button1.BackgroundImage = My.Resources.avvio
            Label7.Text = ""
            If Not SerialPort1.IsOpen Then SerialPort1.Open()
            SerialPort1.DiscardInBuffer()
            SerialPort1.Close()
            Label2.Text = ""
            NumericUpDown1.ReadOnly = False
            Button1.Enabled = True
            Button3.Enabled = True
            Button4.Enabled = True
            Button6.Enabled = True
            Button8.Enabled = True
            Button11.Enabled = True
            Button20.Enabled = True
            Button2.Enabled = False
            Button2.FlatAppearance.BorderSize = 0
        Else
            'TASTO AVVIO MODO CAMERA -----------------------------------------------------
            If My.Settings.cam_on <> True Then
                MsgBox("Prima Apri Preview Camera", MessageBoxIcon.Exclamation)
                Exit Sub
            End If
            Button1.BackgroundImage = My.Resources.avvioverde
            Button1.Enabled = False
            Button3.Enabled = False
            Button4.Enabled = False
            Button6.Enabled = False
            Button8.Enabled = False
            Button11.Enabled = False
            Button20.Enabled = False
            Button2.Enabled = True
            Button2.FlatAppearance.BorderSize = 2
            NumericUpDown1.ReadOnly = True
            Label2.Text = TimeOfDay()
            My.Computer.Audio.PlaySystemSound(System.Media.SystemSounds.Asterisk)

            While index <= Numero
                Application.DoEvents()
                If ANNULLA = True Then
                    Button1.BackgroundImage = My.Resources.avviorosso
                    If Not SerialPort1.IsOpen Then SerialPort1.Open()
                    SerialPort1.Write("6")
                    SerialPort1.Close()
                    Label9.Text = -Numero - 1 + index
                    MsgBox("ANNULLATO" & vbCrLf & vbCrLf & "Ora Inizio " & Label2.Text() & vbCrLf & vbCrLf & "Ora Fine " & TimeOfDay() & vbCrLf & vbCrLf & "Click OK  " & (NumericUpDown1.Value + Label9.Text), MessageBoxIcon.Exclamation)
                    Label9.Text = ("Click n.")
                    Label9.ForeColor = Color.Black
                    Button1.BackgroundImage = My.Resources.avvio
                    Label7.Text = ""
                    If Not SerialPort1.IsOpen Then SerialPort1.Open()
                    SerialPort1.DiscardInBuffer()
                    SerialPort1.Close()
                    Label2.Text = ""
                    ANNULLA = False
                    Button2.BackgroundImage = My.Resources.stop_nero
                    Button7.BackgroundImage = My.Resources.reset_contatori
                    ToolTip1.SetToolTip(Button7, "Reset Contatori")
                    Button1.Enabled = True
                    Button3.Enabled = True
                    Button4.Enabled = True
                    Button6.Enabled = True
                    Button8.Enabled = True
                    Button11.Enabled = True
                    Button20.Enabled = True
                    Button2.Enabled = False
                    Button2.FlatAppearance.BorderSize = 0
                    Exit Sub
                End If
                If My.Settings.FERMA = True Then '------------------------------------------------
                    Button1.BackgroundImage = My.Resources.avviorosso
                    If Not SerialPort1.IsOpen Then SerialPort1.Open()
                    SerialPort1.Write("6")
                    SerialPort1.Close()
                    Label9.Text = -Numero - 1 + index
                    MsgBox("ERRORE! CONTROLLO VISIVO" & vbCrLf & vbCrLf & "Ora Inizio " & Label2.Text() & vbCrLf & vbCrLf & "Ora Fine " & TimeOfDay() & vbCrLf & vbCrLf & "Click OK  " & (NumericUpDown1.Value + Label9.Text), MessageBoxIcon.Error)
                    Label9.Text = ("Click n.")
                    Label9.ForeColor = Color.Black
                    Button1.BackgroundImage = My.Resources.avvio
                    Label7.Text = ""
                    If Not SerialPort1.IsOpen Then SerialPort1.Open()
                    SerialPort1.DiscardInBuffer()
                    SerialPort1.Close()
                    Label2.Text = ""
                    ANNULLA = False
                    My.Settings.FERMA = False
                    Button2.BackgroundImage = My.Resources.stop_nero
                    Button7.BackgroundImage = My.Resources.reset_contatori
                    ToolTip1.SetToolTip(Button7, "Reset Contatori")
                    Button1.Enabled = True
                    Button3.Enabled = True
                    Button4.Enabled = True
                    Button6.Enabled = True
                    Button8.Enabled = True
                    Button11.Enabled = True
                    Button20.Enabled = True
                    Button2.Enabled = False
                    Button2.FlatAppearance.BorderSize = 0
                    Exit Sub
                End If
                Label9.Text = ("")
                Label9.ForeColor = Color.Red
                Label9.Text = -Numero + index
                tempo = (CInt(-Label9.Text) * (CInt(TextBox27.Text) + CInt(TextBox7.Text)) / 1000)
                Secondi = tempo Mod 60
                tempo = Int(tempo / 60)
                Minuti = tempo Mod 60
                tempo = Int(tempo / 60)
                Ore = tempo
                mio = Format(Ore, "00") & ":" & Format(Minuti, "00") & ":" & Format(Secondi, "00")
                Label7.Text = mio
                index += 1
                If TextBox2.Text <> Nothing And NumericUpDown4.Text <> Nothing And TextBox4.Text <> Nothing Then
                    My.Settings.perc_file = TextBox2.Text & "\" & TextBox4.Text & "_" & NumericUpDown4.Text.PadLeft(5, "0") & "." & My.Settings.file_ext
                    If My.Computer.FileSystem.FileExists(My.Settings.perc_file) Then
                        My.Computer.Audio.PlaySystemSound(System.Media.SystemSounds.Beep)
                        Button1.BackgroundImage = My.Resources.avviorosso
                        MsgBox(My.Settings.perc_file & vbCrLf & vbCrLf & "Il File è Esitente!", MessageBoxIcon.Exclamation, MsgBoxStyle.OkOnly)
                        Label9.Text = ("Click n.")
                        Label9.ForeColor = Color.Black
                        Button1.BackgroundImage = My.Resources.avvio
                        Label7.Text = ""
                        If Not SerialPort1.IsOpen Then SerialPort1.Open()
                        SerialPort1.DiscardInBuffer()
                        SerialPort1.Close()
                        Label2.Text = ""
                        NumericUpDown1.ReadOnly = False
                        Button1.Enabled = True
                        Button3.Enabled = True
                        Button4.Enabled = True
                        Button6.Enabled = True
                        Button8.Enabled = True
                        Button11.Enabled = True
                        Button20.Enabled = True
                        Button2.Enabled = False
                        Button2.FlatAppearance.BorderSize = 0
                        Exit Sub
                    End If
                    My.Settings.foto_in = True
                    My.Settings.solo_file = (TextBox4.Text & "_" & NumericUpDown4.Text.PadLeft(5, "0") & "." & My.Settings.file_ext)
                    My.Settings.nome = TextBox4.Text
                    My.Settings.inizio = NumericUpDown4.Text
                    My.Settings.cart_lav = TextBox2.Text
                    NumericUpDown4.Text = NumericUpDown4.Text + 1
                    TextBox10.Text = TextBox2.Text & "\" & TextBox4.Text & "_" & NumericUpDown4.Text.PadLeft(5, "0") & "." & My.Settings.file_ext
                    Dim D As Integer = 0
                    Do While D <= 10
                        Application.DoEvents()
                        If My.Settings.foto_in = False And My.Settings.foto_in2 = False Then
                            Exit Do
                        Else
                            D = 0
                        End If
                    Loop
                Else
                    TextBox10.Text = ""
                    MsgBox("Inserisci percorso destinazione" & vbCrLf & vbCrLf & "Nome e Numero del Prossimo File", MessageBoxIcon.Exclamation)
                    Exit Sub
                End If
                TextBox1.Text += 1
                cont = CInt(TextBox1.Text)
                Button1.BackgroundImage = My.Resources.avvioarancio
                Application.DoEvents()
                If Not SerialPort1.IsOpen Then SerialPort1.Open()
                SerialPort1.Write(s8_n8)
                SerialPort1.Write(CInt(TextBox1.Text))
                Try
                    mess1 = SerialPort1.ReadLine()
                Catch ex As Exception
                    Button1.BackgroundImage = My.Resources.avviorosso
                    If Not SerialPort1.IsOpen Then SerialPort1.Open()
                    SerialPort1.Write("6")
                    SerialPort1.Close()
                    Label9.Text = -Numero - 1 + index
                    MsgBox("ERRORE: Nessuna Risposta" & vbCrLf & vbCrLf & "Arduino non trovato" & vbCrLf & vbCrLf & "Resettare o Ricaricare lo Sketch", MessageBoxIcon.Error)
                    MsgBox("ANNULLAMENTO AUTOMATICO" & vbCrLf & vbCrLf & "Ora Inizio " & Label2.Text() & vbCrLf & vbCrLf & "Ora Fine " & TimeOfDay() & vbCrLf & vbCrLf & "Click OK  " & (NumericUpDown1.Value + Label9.Text), MessageBoxIcon.Exclamation)
                    Label9.ForeColor = Color.Black
                    Label9.Text = ("Click n.")
                    Button22.BackgroundImage = My.Resources.usb_off2
                    Button1.BackgroundImage = My.Resources.avvio
                    Label7.Text = ""
                    Label2.Text = ""
                    Button1.Enabled = False
                    Button3.Enabled = False
                    Button4.Enabled = False
                    Button6.Enabled = False
                    Button8.Enabled = False
                    Button11.Enabled = False
                    Button20.Enabled = False
                    Button2.Enabled = True
                    Button2.FlatAppearance.BorderSize = 2
                    ComboBox1.Enabled = True
                    NumericUpDown1.ReadOnly = False
                    Exit Sub
                End Try
                SerialPort1.DiscardInBuffer()
                SerialPort1.Close()
                Thread.Sleep(TextBox7.Text) 'attesa movimento motori 
                Label1.Text = mess1
                Button1.BackgroundImage = My.Resources.avvioverde
                Application.DoEvents()
            End While
            If CheckBox1.Checked = True Then
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
                    MsgBox("Spegnimento Annullato", MessageBoxIcon.Information)
                End If
            End If
            If Not SerialPort1.IsOpen Then SerialPort1.Open()
            Button1.BackgroundImage = My.Resources.avviorosso
            SerialPort1.Write("6")
            SerialPort1.Close()
            Label9.Text = 0
            Dim a As String = Label2.Text()
            Dim b As String = TimeOfDay()
            Dim t1 As TimeSpan = TimeSpan.Parse(a)
            Dim t2 As TimeSpan = TimeSpan.Parse(b)
            Dim tDiff As TimeSpan
            tDiff = t2.Subtract(t1)
            Dim days = (tDiff.ToString)
            Label7.Text = days
            MsgBox("FINE" & vbCrLf & vbCrLf & "Ora Inizio " & Label2.Text() & vbCrLf & vbCrLf & "Ora Fine " & TimeOfDay() & vbCrLf & vbCrLf & "Durata " & days & vbCrLf & vbCrLf & "Click OK  " & (NumericUpDown1.Value), MessageBoxIcon.Information)
            Label9.Text = ("Click n.")
            Label9.ForeColor = Color.Black
            Button1.BackgroundImage = My.Resources.avvio
            Label7.Text = ""
            If Not SerialPort1.IsOpen Then SerialPort1.Open()
            SerialPort1.DiscardInBuffer()
            SerialPort1.Close()
            Label2.Text = ""
            NumericUpDown1.ReadOnly = False
            Button1.Enabled = True
            Button3.Enabled = True
            Button4.Enabled = True
            Button6.Enabled = True
            Button8.Enabled = True
            Button11.Enabled = True
            Button20.Enabled = True
            Button2.Enabled = False
            Button2.FlatAppearance.BorderSize = 0
        End If
    End Sub
    'TASTO INDIETRO 1 FOTOGRAMMA
    Private Sub Button4_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button4.Click
        Dim Numero = NumericUpDown2.Value
        Dim stopClick As Boolean = False
        Dim index As Integer = 1
        Button1.Enabled = False
        Button3.Enabled = False
        Button4.Enabled = False
        Button6.Enabled = False
        Button8.Enabled = False
        Button11.Enabled = False
        Button20.Enabled = False
        Button2.Enabled = True
        Button2.FlatAppearance.BorderSize = 2
        While index <= Numero
            Application.DoEvents()
            If ANNULLA = True Then
                Button2.BackgroundImage = My.Resources.stop_nero
                Button7.BackgroundImage = My.Resources.reset_contatori
                ANNULLA = False
                ToolTip1.SetToolTip(Button7, "Reset Contatori")
                Exit While
            End If
            Button4.Visible = False
            Label8.Text = 1 + Numero - index
            index += 1
            If Not SerialPort1.IsOpen Then SerialPort1.Open()
            SerialPort1.Write("2")
            SerialPort1.Write(s8_n8)
            SerialPort1.Close()
            Application.DoEvents()
            Thread.Sleep(TextBox8.Text)
        End While
        Button4.Visible = False
        If Not SerialPort1.IsOpen Then SerialPort1.Open()
        SerialPort1.Write("6")
        SerialPort1.Close()
        Label8.Text = ""
        Button4.Visible = True
        Button1.Enabled = True
        Button3.Enabled = True
        Button4.Enabled = True
        Button6.Enabled = True
        Button8.Enabled = True
        Button11.Enabled = True
        Button20.Enabled = True
        Button2.Enabled = False
        Button2.FlatAppearance.BorderSize = 0
    End Sub
    'TASTO AVANTI 1 FOTOGRAMMA
    Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button3.Click
        Dim Numero = NumericUpDown2.Value
        Dim stopClick As Boolean = False
        Dim index As Integer = 1
        Button1.Enabled = False
        Button3.Enabled = False
        Button4.Enabled = False
        Button6.Enabled = False
        Button8.Enabled = False
        Button11.Enabled = False
        Button20.Enabled = False
        Button2.Enabled = True
        Button2.FlatAppearance.BorderSize = 2
        While index <= Numero
            Application.DoEvents()
            If ANNULLA = True Then
                Button2.BackgroundImage = My.Resources.stop_nero
                Button7.BackgroundImage = My.Resources.reset_contatori
                ANNULLA = False
                ToolTip1.SetToolTip(Button7, "Reset Contatori")
                Exit While
            End If
            Button3.Visible = False
            Label6.Text = 1 + Numero - index
            index += 1
            If Not SerialPort1.IsOpen Then SerialPort1.Open()
            SerialPort1.Write("3")
            SerialPort1.Write(s8_n8)
            SerialPort1.Close()
            Application.DoEvents()
            Thread.Sleep(TextBox8.Text)
        End While
        If Not SerialPort1.IsOpen Then SerialPort1.Open()
        SerialPort1.Write("6")
        SerialPort1.Close()
        Label6.Text = ""
        Button3.Visible = True
        Button1.Enabled = True
        Button3.Enabled = True
        Button4.Enabled = True
        Button6.Enabled = True
        Button8.Enabled = True
        Button11.Enabled = True
        Button20.Enabled = True
        Button2.Enabled = False
        Button2.FlatAppearance.BorderSize = 0
    End Sub
    'TASTO AVANTI 1/20 DI FOTOGRAMMA MICRO STEP
    Private Sub Button6_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button6.Click
        Dim Numero = NumericUpDown3.Value
        Dim stopClick As Boolean = False
        Dim index As Integer = 1
        Button1.Enabled = False
        Button3.Enabled = False
        Button4.Enabled = False
        Button6.Enabled = False
        Button8.Enabled = False
        Button11.Enabled = False
        Button20.Enabled = False
        Button2.Enabled = True
        Button2.FlatAppearance.BorderSize = 2
        While index <= Numero
            Application.DoEvents()
            If ANNULLA = True Then
                Button2.BackgroundImage = My.Resources.stop_nero
                Button7.BackgroundImage = My.Resources.reset_contatori
                ANNULLA = False
                ToolTip1.SetToolTip(Button7, "Reset Contatori")
                Exit While
            End If
            Button6.Visible = False
            Label12.Text = 1 + Numero - index
            index += 1
            If Not SerialPort1.IsOpen Then SerialPort1.Open()
            SerialPort1.Write("4")
            SerialPort1.Close()
            Application.DoEvents()
            Thread.Sleep(TextBox9.Text)
        End While
        If Not SerialPort1.IsOpen Then SerialPort1.Open()
        SerialPort1.Write("6")
        SerialPort1.Close()
        Label12.Text = ""
        Button6.Visible = True
        Button1.Enabled = True
        Button3.Enabled = True
        Button4.Enabled = True
        Button6.Enabled = True
        Button8.Enabled = True
        Button11.Enabled = True
        Button20.Enabled = True
        Button2.Enabled = False
        Button2.FlatAppearance.BorderSize = 0
    End Sub
    'TASTO INDIETRO 1/20 DI FOTOGRAMMA MICRO STEP
    Private Sub Button8_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button8.Click
        Dim Numero = NumericUpDown3.Value
        Dim stopClick As Boolean = False
        Dim index As Integer = 1
        Button1.Enabled = False
        Button3.Enabled = False
        Button4.Enabled = False
        Button6.Enabled = False
        Button8.Enabled = False
        Button11.Enabled = False
        Button20.Enabled = False
        Button2.Enabled = True
        Button2.FlatAppearance.BorderSize = 2
        While index <= Numero
            Application.DoEvents()
            If ANNULLA = True Then
                Button2.BackgroundImage = My.Resources.stop_nero
                Button7.BackgroundImage = My.Resources.reset_contatori
                ANNULLA = False
                ToolTip1.SetToolTip(Button7, "Reset Contatori")
                Exit While
            End If
            Button8.Visible = False
            Label11.Text = 1 + Numero - index
            index += 1
            If Not SerialPort1.IsOpen Then SerialPort1.Open()
            SerialPort1.Write("5")
            SerialPort1.Close()
            Application.DoEvents()
            Thread.Sleep(TextBox9.Text)
        End While
        If Not SerialPort1.IsOpen Then SerialPort1.Open()
        SerialPort1.Write("6")
        SerialPort1.Close()
        Label11.Text = ""
        Button8.Visible = True
        Button1.Enabled = True
        Button3.Enabled = True
        Button4.Enabled = True
        Button6.Enabled = True
        Button8.Enabled = True
        Button11.Enabled = True
        Button20.Enabled = True
        Button2.Enabled = False
        Button2.FlatAppearance.BorderSize = 0
    End Sub
    'RESET CONTATORE
    Private Sub Button7_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button7.Click
        TextBox1.Text = ("0")
        Label1.Text = ("0")
        Label2.Text = ""
        NumericUpDown1.ReadOnly = False
        NumericUpDown1.Text = 1
        NumericUpDown2.Text = 1
        NumericUpDown3.Text = 1
        CheckBox1.CheckState = CheckState.Unchecked
    End Sub
    ' Selezione pellicola Super 8
    Private Sub RadioButton1_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RadioButton1.CheckedChanged
        s8_n8 = "0"
        My.Settings.SUP_NOR = True
    End Sub
    ' selezione pellicola 8 mm Standard
    Private Sub RadioButton2_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RadioButton2.CheckedChanged
        s8_n8 = "1"
        My.Settings.SUP_NOR = False
    End Sub
    'TASTO ANNULLA
    Private Sub Button2_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click

        My.Computer.Audio.PlaySystemSound(System.Media.SystemSounds.Beep)
        ANNULLA = True
        Button2.BackgroundImage = My.Resources.stop_rosso
        Button1.BackgroundImage = My.Resources.avvio

        Button1.Enabled = False
        Button3.Enabled = False
        Button4.Enabled = False
        Button6.Enabled = False
        Button8.Enabled = False
        Button11.Enabled = False
        Button20.Enabled = False
        Button2.Enabled = False
        Button2.FlatAppearance.BorderSize = 0
        NumericUpDown1.ReadOnly = False
        If SerialPort1.IsOpen Then
            SerialPort1.Write("6")
            SerialPort1.Close()
        End If
    End Sub
    ' rileva coordinate mouse Click
    Private Sub Button10_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button10.Click
        Dialog1.ShowDialog()
        Thread.Sleep(100)
        TextBox5.Text = My.Settings.SaveTitle3
        TextBox6.Text = My.Settings.SaveTitle4
    End Sub
    'BOTTONE CREA SKETCH ARDUINO
    Private Sub Button14_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button14.Click
        Dim index As Integer = 1
        Dim num As Integer = 100
        Button17.Enabled = True
        Button15.Enabled = True
        ' DATI PIN MOTORI CORONA
        Dim PINM1_1 = My.Settings.SaveTitle11
        Dim PINM1_2 = My.Settings.SaveTitle12
        Dim PINM1_3 = My.Settings.SaveTitle9
        Dim PINM1_4 = My.Settings.SaveTitle10
        ' DATI PIN MOTORI BOBINA
        Dim PINM2_1 = My.Settings.SaveTitle19
        Dim PINM2_2 = My.Settings.SaveTitle20
        Dim PINM2_3 = My.Settings.SaveTitle17
        Dim PINM2_4 = My.Settings.SaveTitle18
        ' DATI PIN PWM LED ARDUINO
        Dim PINLED = My.Settings.SaveTitle1
        ' DATI PIN PWM MOTORE DC ARDUINO
        Dim PINMOT = My.Settings.SaveTitle23
        ' DATI MOTORE CORONA
        Dim SGM1 = My.Settings.SaveTitle7   ' N. STEP PER GIRO
        Dim SFS8 = My.Settings.SaveTitle8  ' STEP / FOTOGRAMMA SUPER 8
        Dim SFN8 = My.Settings.SaveTitle    ' STEP / FOTOGRAMMA NORMAL 8
        Dim MSM1 = 1                ' STEP / MICROSTEP AVANTI - INDIETRO
        Dim SPM1 = My.Settings.SaveTitle2    ' VELOCITA' MOTORE CORONA GIRI/MINUTO
        ' DATI MOTORE BOBINA
        Dim SGM2 = My.Settings.SaveTitle15  ' N. STEP PER GIRO MOTORE DEFAULT
        Dim DIMF = My.Settings.SaveTitle16   ' DIAMTRO FULCRO
        Dim MSM2 = 2                ' STEP / MICROSTEP AVANTI - INDIETRO
        Dim SEM2 = My.Settings.SaveTitle13   ' STEP / GIRO BOBINA
        Dim SPM2 = My.Settings.SaveTitle14   ' VELOCITA' MOTORE BOBINA GIRI/MINUTO
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

        If My.Settings.SaveTile25 <> 0 Then
            DIR1D = "-"
            DIR1S = ""
        Else
            DIR1D = ""
            DIR1S = "-"
        End If
        If My.Settings.SaveTile26 <> 0 Then
            DIR2D = "-"
            DIR2S = ""
        Else
            DIR2D = ""
            DIR2S = "-"
        End If

        RichTextBox2.Text = ("// SKETCH ARDUINO For TELECINE FRAME BY Step")
        RichTextBox2.Text += vbCrLf & ("// CONTROL 2 Step MOTOR, 1 LED, 1 DC MOTOR ")
        RichTextBox2.Text += vbCrLf & ("// SANDUINO SOFTWARE '2018'") & vbCrLf
        RichTextBox2.Text += vbCrLf & (("#include <Stepper.h>"))
        RichTextBox2.Text += vbCrLf & ("Stepper myStepper1 (" & CInt(SGM2) & "," & CInt(PINM2_1) & "," & CInt(PINM2_3) & "," & CInt(PINM2_2) & "," & CInt(PINM2_4) & ");//MOTORE BOBINA")
        RichTextBox2.Text += vbCrLf & ("Stepper myStepper2 (" & CInt(SGM1) & "," & CInt(PINM1_1) & "," & CInt(PINM1_3) & "," & CInt(PINM1_2) & "," & CInt(PINM1_4) & ");//MOTORE CORONA")
        RichTextBox2.Text += vbCrLf & ("int motor1Pin1 = " & CInt(PINM1_1) & ";")
        RichTextBox2.Text += vbCrLf & ("int motor1Pin2 = " & CInt(PINM1_2) & ";")
        RichTextBox2.Text += vbCrLf & ("int motor1Pin3 = " & CInt(PINM1_3) & ";")
        RichTextBox2.Text += vbCrLf & ("int motor1Pin4 = " & CInt(PINM1_4) & ";")
        RichTextBox2.Text += vbCrLf & ("int motor2Pin1 = " & CInt(PINM2_1) & ";")
        RichTextBox2.Text += vbCrLf & ("int motor2Pin2 = " & CInt(PINM2_2) & ";")
        RichTextBox2.Text += vbCrLf & ("int motor2Pin3 = " & CInt(PINM2_3) & ";")
        RichTextBox2.Text += vbCrLf & ("int motor2Pin4 = " & CInt(PINM2_4) & ";")
        RichTextBox2.Text += vbCrLf & (("int led = " & CInt(PINLED) & "; // PIN PWM LED ILLUMINAZIONE") & vbCrLf & ("int brillo = 0;") & vbCrLf & ("int recibido = 0;"))
        RichTextBox2.Text += vbCrLf & (("int mot = " & CInt(PINMOT) & "; // PIN PWM MOTORE DC RIAVVOLGI") & vbCrLf & ("int vel = 0;") & vbCrLf & ("int rec = 0;")) & vbCrLf
        RichTextBox2.Text += vbCrLf & ("void setup() {")
        RichTextBox2.Text += vbCrLf & ("myStepper1.setSpeed(" & CInt(SPM2) & ");")
        RichTextBox2.Text += vbCrLf & ("myStepper2.setSpeed(" & CInt(SPM1) & ");")
        RichTextBox2.Text += vbCrLf & ("pinMode(motor1Pin1, OUTPUT);")
        RichTextBox2.Text += vbCrLf & ("pinMode(motor1Pin2, OUTPUT);")
        RichTextBox2.Text += vbCrLf & ("pinMode(motor1Pin3, OUTPUT);")
        RichTextBox2.Text += vbCrLf & ("pinMode(motor1Pin4, OUTPUT);")
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
                JS8 = JS8 - 1
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
        RichTextBox2.Text += vbCrLf & ("myStepper2.step(" & DIR2D & CInt(SFS8) & ");")
        RichTextBox2.Text += vbCrLf & ("myStepper1.step(" & DIR1D & "stp);")
        RichTextBox2.Text += vbCrLf & ("Serial.println(x);") & vbCrLf & ("}")
        RichTextBox2.Text += vbCrLf & ("if (pos == '1') {// TASTO AVVIO 8 mm")
        RichTextBox2.Text += vbCrLf & ("while (Serial.available() > 0) {") & vbCrLf & ("val = Serial.read();")
        RichTextBox2.Text += vbCrLf & ("n_foto += val -  '0';") & vbCrLf & ("x = n_foto.toInt();")
        DIMF = My.Settings.SaveTitle16
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
                JN8 = JN8 - 1
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
        RichTextBox2.Text += vbCrLf & ("myStepper2.step(" & DIR2D & CInt(SFN8) & ");") & vbCrLf & ("myStepper1.step(" & DIR1D & "stp);")
        RichTextBox2.Text += vbCrLf & ("Serial.println(x);") & vbCrLf & ("}")
        RichTextBox2.Text += vbCrLf & ("if (pos == '2') {// TASTO IDIETRO 1 FOTOGRAMMA")
        RichTextBox2.Text += vbCrLf & ("while (Serial.available() > 0) {") & vbCrLf & ("dir = Serial.read();")
        RichTextBox2.Text += vbCrLf & ("n_dir += dir -  '0';") & vbCrLf & ("d = n_dir.toInt();") & vbCrLf & ("}")
        RichTextBox2.Text += vbCrLf & ("if (d == 0) {// IDIETRO 1 FOTOGRAMMA SUPER 8")
        RichTextBox2.Text += vbCrLf & ("myStepper1.step(" & DIR1S & CInt(HHS8) & ");") & vbCrLf & ("myStepper2.step(" & DIR2S & CInt(SFS8) & ");") & vbCrLf & ("}")
        RichTextBox2.Text += vbCrLf & ("else if (d == 1) {// IDIETRO 1 FOTOGRAMMA NORMAL 8")
        RichTextBox2.Text += vbCrLf & ("myStepper1.step(" & DIR1S & CInt(HHN8) & ");") & vbCrLf & ("myStepper2.step(" & DIR2S & CInt(SFN8) & ");") & vbCrLf & ("}")
        RichTextBox2.Text += vbCrLf & ("}")
        RichTextBox2.Text += vbCrLf & ("if (pos == '3') {// TASTO AVANTI 1 FOTOGRAMMA")
        RichTextBox2.Text += vbCrLf & ("while (Serial.available() > 0) {") & vbCrLf & ("dir = Serial.read();")
        RichTextBox2.Text += vbCrLf & ("n_dir += dir -  '0';") & vbCrLf & ("d = n_dir.toInt();") & vbCrLf & ("}")
        RichTextBox2.Text += vbCrLf & ("if (d == 0) {// AVANTI 1 FOTOGRAMMA SUPER 8")
        RichTextBox2.Text += vbCrLf & ("myStepper2.step(" & DIR2D & CInt(SFS8) & ");") & vbCrLf & ("myStepper1.step(" & DIR1D & CInt(HHS8) & ");") & vbCrLf & ("}")
        RichTextBox2.Text += vbCrLf & ("else if (d == 1) {// AVANTI 1 FOTOGRAMMA NORMAL 8")
        RichTextBox2.Text += vbCrLf & ("myStepper2.step(" & DIR2D & CInt(SFN8) & ");") & vbCrLf & ("myStepper1.step(" & DIR1D & CInt(HHN8) & ");") & vbCrLf & ("}")
        RichTextBox2.Text += vbCrLf & ("}")
        RichTextBox2.Text += vbCrLf & ("if (pos == '4') {// TASTO AVANTI 1 MICROSTEP")
        RichTextBox2.Text += vbCrLf & ("myStepper2.step(" & DIR2D & CInt(MSM1) & ");") & vbCrLf & ("myStepper1.step(" & DIR1D & CInt(MSM2) & ");") & vbCrLf & ("}")
        RichTextBox2.Text += vbCrLf & ("if (pos == '5') {// TASTO IDIETRO 1 MICROSTEP")
        RichTextBox2.Text += vbCrLf & ("myStepper1.step(" & DIR1S & CInt(MSM2) & ");") & vbCrLf & ("myStepper2.step(" & DIR2S & CInt(MSM1) & ");") & vbCrLf & ("}")
        RichTextBox2.Text += vbCrLf & ("if (pos == '6') {// SPEGNE I MOTORI")
        RichTextBox2.Text += vbCrLf & ("digitalWrite(motor1Pin1, LOW);")
        RichTextBox2.Text += vbCrLf & ("digitalWrite(motor1Pin2, LOW);")
        RichTextBox2.Text += vbCrLf & ("digitalWrite(motor1Pin3, LOW);")
        RichTextBox2.Text += vbCrLf & ("digitalWrite(motor1Pin4, LOW);")
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
    ' SELEZIONA TUTTO E COPIA SKETCH GENERATO 
    Private Sub Button15_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button15.Click
        RichTextBox2.SelectAll()
        RichTextBox2.Copy()
    End Sub
    ' RESET PULISCI SKETCH GENERATO 
    Private Sub Button17_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button17.Click
        RichTextBox2.Clear()
        My.Computer.Clipboard.Clear()
        Button17.Enabled = False
        Button15.Enabled = False
    End Sub
    ' SALVA IMPOSTAZIONI setup
    Private Sub Button12_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button12.Click
        If MsgBox("Il precedente Salvataggio andrà perso!" & vbCrLf & vbCrLf & "Sicuro di Salvare i Dati?", MsgBoxStyle.YesNo, ) = MsgBoxResult.Yes Then
            My.Settings.SaveTitle3 = TextBox5.Text
            My.Settings.SaveTitle4 = TextBox6.Text
            My.Settings.SaveTitle5 = TextBox7.Text
            My.Settings.SaveTitle6 = TextBox8.Text
            My.Settings.SaveTitle22 = TextBox9.Text
            My.Settings.SaveTitle24 = ComboBox1.Text
            My.Settings.PAUSA = TextBox27.Text
        End If
    End Sub
    ' CARICA IMPOSTAZIONI SALVATE setup
    Private Sub Button18_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button18.Click
        If MsgBox("Le attuali impostazioni andranno perse!" & vbCrLf & vbCrLf & "Sicuro di velere Caricare i Dati?", MsgBoxStyle.YesNo, ) = MsgBoxResult.Yes Then
            TextBox5.Text = My.Settings.SaveTitle3
            TextBox6.Text = My.Settings.SaveTitle4
            TextBox7.Text = My.Settings.SaveTitle5
            TextBox8.Text = My.Settings.SaveTitle6
            TextBox9.Text = My.Settings.SaveTitle22
            ComboBox1.Text = My.Settings.SaveTitle24
            TextBox27.Text = My.Settings.PAUSA
        End If
    End Sub
    ' impostazione luce
    Private Sub TrackBar1_Scroll(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TrackBar1.Scroll
        If LED_ON <> True Then
            TrackBar1.Enabled = True
            If Not SerialPort1.IsOpen Then SerialPort1.Open()
            SerialPort1.Write("7")
            If TrackBar1.Value = 0 And TrackBar1.Value < 1 Then
                SerialPort1.Write("1")
                PictureBox1.Image = My.Resources.ledon1
                Label18.Text = TrackBar1.Value + 1
            ElseIf TrackBar1.Value > 0 And TrackBar1.Value <= 1 Then
                SerialPort1.Write("2")
                PictureBox1.Image = My.Resources.ledon1
                Label18.Text = TrackBar1.Value + 1
            ElseIf TrackBar1.Value > 1 And TrackBar1.Value <= 2 Then
                SerialPort1.Write("3")
                PictureBox1.Image = My.Resources.ledon1
                Label18.Text = TrackBar1.Value + 1
            ElseIf TrackBar1.Value > 2 And TrackBar1.Value <= 3 Then
                SerialPort1.Write("4")
                PictureBox1.Image = My.Resources.ledon2
                Label18.Text = TrackBar1.Value + 1
            ElseIf TrackBar1.Value > 3 And TrackBar1.Value <= 4 Then
                SerialPort1.Write("5")
                PictureBox1.Image = My.Resources.ledon2
                Label18.Text = TrackBar1.Value + 1
            ElseIf TrackBar1.Value > 4 And TrackBar1.Value <= 5 Then
                SerialPort1.Write("6")
                PictureBox1.Image = My.Resources.ledon2
                Label18.Text = TrackBar1.Value + 1
            ElseIf TrackBar1.Value > 5 And TrackBar1.Value <= 6 Then
                SerialPort1.Write("7")
                PictureBox1.Image = My.Resources.ledon3
                Label18.Text = TrackBar1.Value + 1
            ElseIf TrackBar1.Value > 6 And TrackBar1.Value <= 7 Then
                SerialPort1.Write("8")
                PictureBox1.Image = My.Resources.ledon3
                Label18.Text = TrackBar1.Value + 1
            ElseIf TrackBar1.Value > 7 Then
                SerialPort1.Write("9")
                PictureBox1.Image = My.Resources.ledon3
                Label18.Text = TrackBar1.Value + 1
            End If
        End If
    End Sub
    ' impostazione luce bottone Acendi/spegni
    Private Sub Button11_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button11.Click
        If LED_ON <> True Then
            'impostazione luce bottone spegni
            If Not SerialPort1.IsOpen Then SerialPort1.Open()
            SerialPort1.Write("7")
            SerialPort1.Write("0")
            PictureBox1.Image = My.Resources.ledoff
            Button11.BackgroundImage = My.Resources.accendi
            Label18.Text = ""
            LED_ON = True
            TrackBar1.Enabled = False
        Else
            If Not SerialPort1.IsOpen Then SerialPort1.Open()
            SerialPort1.Write("7")
            If TrackBar1.Value = 0 And TrackBar1.Value < 1 Then
                SerialPort1.Write("1")
                PictureBox1.Image = My.Resources.ledon1
                Label18.Text = TrackBar1.Value + 1
            ElseIf TrackBar1.Value > 0 And TrackBar1.Value <= 1 Then
                SerialPort1.Write("2")
                PictureBox1.Image = My.Resources.ledon1
                Label18.Text = TrackBar1.Value + 1
            ElseIf TrackBar1.Value > 1 And TrackBar1.Value <= 2 Then
                SerialPort1.Write("3")
                PictureBox1.Image = My.Resources.ledon1
                Label18.Text = TrackBar1.Value + 1
            ElseIf TrackBar1.Value > 2 And TrackBar1.Value <= 3 Then
                SerialPort1.Write("4")
                PictureBox1.Image = My.Resources.ledon2
                Label18.Text = TrackBar1.Value + 1
            ElseIf TrackBar1.Value > 3 And TrackBar1.Value <= 4 Then
                SerialPort1.Write("5")
                PictureBox1.Image = My.Resources.ledon2
                Label18.Text = TrackBar1.Value + 1
            ElseIf TrackBar1.Value > 4 And TrackBar1.Value <= 5 Then
                SerialPort1.Write("6")
                PictureBox1.Image = My.Resources.ledon2
                Label18.Text = TrackBar1.Value + 1
            ElseIf TrackBar1.Value > 5 And TrackBar1.Value <= 6 Then
                SerialPort1.Write("7")
                PictureBox1.Image = My.Resources.ledon3
                Label18.Text = TrackBar1.Value + 1
            ElseIf TrackBar1.Value > 6 And TrackBar1.Value <= 7 Then
                SerialPort1.Write("8")
                PictureBox1.Image = My.Resources.ledon3
                Label18.Text = TrackBar1.Value + 1
            ElseIf TrackBar1.Value > 7 Then
                SerialPort1.Write("9")
                PictureBox1.Image = My.Resources.ledon3
                Label18.Text = TrackBar1.Value + 1
            End If
            LED_ON = False
            TrackBar1.Enabled = True
            Button11.BackgroundImage = My.Resources.spegni
        End If
        SerialPort1.Close()
    End Sub
    'BOTTONE AVVIO RIAVVOLGE MOTORE
    Private Sub Button20_Click_1(sender As Object, e As EventArgs) Handles Button20.Click
        If DC_ON <> True Then
            'impostazione DC motor bottone ferma
            If Not SerialPort1.IsOpen Then SerialPort1.Open()
            SerialPort1.Write("8")
            SerialPort1.Write("0")
            PictureBox3.Image = My.Resources.JPG2
            Button20.BackgroundImage = My.Resources.accendi
            Label29.Text = ""
            DC_ON = True
            TrackBar2.Enabled = False
        Else
            PictureBox3.Image = My.Resources.GIF
            If Not SerialPort1.IsOpen Then SerialPort1.Open()
            SerialPort1.Write("8")
            If TrackBar2.Value = 0 And TrackBar2.Value < 1 Then
                SerialPort1.Write("1")
                Label29.Text = TrackBar2.Value + 1
            ElseIf TrackBar2.Value > 0 And TrackBar2.Value <= 1 Then
                SerialPort1.Write("2")
                Label29.Text = TrackBar2.Value + 1
            ElseIf TrackBar2.Value > 1 And TrackBar2.Value <= 2 Then
                SerialPort1.Write("3")
                Label29.Text = TrackBar2.Value + 1
            ElseIf TrackBar2.Value > 2 And TrackBar2.Value <= 3 Then
                SerialPort1.Write("4")
                Label29.Text = TrackBar2.Value + 1
            ElseIf TrackBar2.Value > 3 Then
                SerialPort1.Write("5")
                Label29.Text = TrackBar2.Value + 1
            End If
            DC_ON = False
            TrackBar2.Enabled = True
            Button20.BackgroundImage = My.Resources.spegni
        End If
        SerialPort1.Close()
    End Sub
    'BARRA RIAVVOLGE MOTORE
    Private Sub TrackBar2_Scroll_1(sender As Object, e As EventArgs) Handles TrackBar2.Scroll
        If DC_ON <> True Then
            TrackBar2.Enabled = True
            If Not SerialPort1.IsOpen Then SerialPort1.Open()
            SerialPort1.Write("8")
            If TrackBar2.Value = 0 And TrackBar2.Value < 1 Then
                SerialPort1.Write("1")
                Label29.Text = TrackBar2.Value + 1
            ElseIf TrackBar2.Value > 0 And TrackBar2.Value <= 1 Then
                SerialPort1.Write("2")
                Label29.Text = TrackBar2.Value + 1
            ElseIf TrackBar2.Value > 1 And TrackBar2.Value <= 2 Then
                SerialPort1.Write("3")
                Label29.Text = TrackBar2.Value + 1
            ElseIf TrackBar2.Value > 2 And TrackBar2.Value <= 3 Then
                SerialPort1.Write("4")
                Label29.Text = TrackBar2.Value + 1
            ElseIf TrackBar2.Value > 3 Then
                SerialPort1.Write("5")
                Label29.Text = TrackBar2.Value + 1
            End If
        End If
    End Sub
    'APRE CONFIGURATORE GRAFICO SKETCH ARDUINO
    Private Sub Button21_Click(sender As Object, e As EventArgs) Handles Button21.Click
        Dialog2.ShowDialog()
        Thread.Sleep(100)
    End Sub
    'BOTTONE CONNETTI / DISCONNETTI ARDUINO
    Private Sub Button22_Click(sender As Object, e As EventArgs) Handles Button22.Click
        Dim online = 0
        PictureBox11.BackgroundImage = My.Resources.arduin

        If CONNETTI <> False Then
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
                Button22.BackgroundImage = My.Resources.usb_off2
                CONNETTI = False
                Button1.Enabled = False
                Button3.Enabled = False
                Button4.Enabled = False
                Button6.Enabled = False
                Button8.Enabled = False
                Button11.Enabled = False
                NumericUpDown1.Enabled = False
                NumericUpDown2.Enabled = False
                NumericUpDown3.Enabled = False
                Label15.ForeColor = Color.Red
                Label15.Text = "ARDUINO NON CONNESSO"
                PictureBox13.BackgroundImage = My.Resources.noarduino
                PictureBox14.BackgroundImage = My.Resources.noarduino
                Button20.Enabled = False
                ComboBox1.Enabled = True
                Button25.Enabled = True
                GroupBox10.Visible = True
                Label13.Text = ("CONNETTI" & vbCrLf & "ARDUINO")
                Exit Sub
            Catch ex As Exception
                PictureBox11.BackgroundImage = My.Resources.errore
                Button22.BackgroundImage = My.Resources.usb_off2
                CONNETTI = False
                Button1.Enabled = False
                Button3.Enabled = False
                Button4.Enabled = False
                Button6.Enabled = False
                Button8.Enabled = False
                Button11.Enabled = False
                NumericUpDown1.Enabled = False
                NumericUpDown2.Enabled = False
                NumericUpDown3.Enabled = False
                Label15.ForeColor = Color.Red
                Label15.Text = "ARDUINO NON CONNESSO"
                PictureBox13.BackgroundImage = My.Resources.noarduino
                PictureBox14.BackgroundImage = My.Resources.noarduino
                Button20.Enabled = False
                ComboBox1.Enabled = True
                Button25.Enabled = True
                GroupBox10.Visible = True
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
            Label13.Text = ""
            Label13.Text = ("ERRORE COM !" & vbCrLf & "Seleziona Porta" & vbCrLf & "COM di Arduino" & vbCrLf & "in Setup")
            PictureBox11.BackgroundImage = My.Resources.errore
            Button22.BackgroundImage = My.Resources.usb_off2
            CONNETTI = False
            Exit Sub
        End If
        Try
            SerialPort1.Open()
        Catch ex As Exception
            Label13.Text = ""
            Label13.Text = ("ERRORE USB !" & vbCrLf & "Impossibile Aprire" & vbCrLf & "la Porta " & ComboBox1.Text & vbCrLf & "Reimposta e Riprova")
            'MsgBox("Impossibile aprire la Porta COM" & vbCrLf & vbCrLf & "Impostare la Porta COM in Setup", MessageBoxIcon.Exclamation)
            PictureBox11.BackgroundImage = My.Resources.errore
            Button22.BackgroundImage = My.Resources.usb_off2
            CONNETTI = False
            Exit Sub
        End Try
        SerialPort1.Write("9")
        Try
            online = SerialPort1.ReadLine()
        Catch ex As Exception
            Label13.Text = ""
            Label13.Text = ("ERRORE ARDUINO !" & vbCrLf & "Arduino Non Risponde" & vbCrLf & "Verifica " & ComboBox1.Text & vbCrLf & "Ricollega Arduino")
            PictureBox11.BackgroundImage = My.Resources.errore
            Button22.BackgroundImage = My.Resources.usb_off2
            CONNETTI = False
            Exit Sub
        End Try
        If online <> 5 Then
            Label13.Text = ""
            Label13.Text = ("ERRORE SKETCH !" & vbCrLf & "Copiare in Arduino" & vbCrLf & "Lo Sketch Creato" & vbCrLf & "Nel Programma")
            PictureBox11.BackgroundImage = My.Resources.errore
            Button22.BackgroundImage = My.Resources.usb_off2
            CONNETTI = False
            Exit Sub
        End If
        SerialPort1.Close()
        Button1.Enabled = True
        Button3.Enabled = True
        Button4.Enabled = True
        Button6.Enabled = True
        Button8.Enabled = True
        Button11.Enabled = True
        NumericUpDown1.Enabled = True
        NumericUpDown2.Enabled = True
        NumericUpDown3.Enabled = True
        Label15.ForeColor = Color.Green
        Label15.Text = "ARDUINO CONNESSO"
        PictureBox13.BackgroundImage = My.Resources.checked
        PictureBox14.BackgroundImage = My.Resources.checked
        Button20.Enabled = True
        ComboBox1.Enabled = False
        Button25.Enabled = False
        GroupBox10.Visible = False
        PictureBox11.BackgroundImage = My.Resources.arduin
        Button22.BackgroundImage = My.Resources.usb_on2
        CONNETTI = True
    End Sub
    ' SELEZIONA CARTELLA DI LAVORO
    Private Sub BrowseButton_Click(sender As Object, e As EventArgs) Handles BrowseButton.Click
        Dim folderDlg As New FolderBrowserDialog
        folderDlg.SelectedPath = TextBox2.Text
        folderDlg.ShowNewFolderButton = True
        If (folderDlg.ShowDialog() = DialogResult.OK) Then
            TextBox2.Text = folderDlg.SelectedPath
            My.Settings.cart_lav = TextBox2.Text
        End If
    End Sub
    ' APRI CARTELLA DI LAVORO IN EXPLORER
    Private Sub Button23_Click(sender As Object, e As EventArgs) Handles Button23.Click
        If TextBox2.Text <> Nothing Then
            Process.Start("explorer.exe", TextBox2.Text)
        End If
    End Sub
    ' SINCRONIZZA CARTELLA DI LAVORO CON VIEWER
    Private Sub Button19_Click(sender As Object, e As EventArgs) Handles Button19.Click
        TextBox2.Text = My.Settings.cart_lav
    End Sub
    ' APRE PICTURE VIEWER
    Private Sub Button24_Click(sender As Object, e As EventArgs) Handles Button24.Click
        Form9.Close()
        Form7.Show()
    End Sub
    ' APRE CAMERA PREVIEW
    Private Sub Button16_Click(sender As Object, e As EventArgs) Handles Button16.Click
        Form9.Close()
        Form8.Show()
    End Sub
    'SCATTA FOTOGRAMMA IN MODALITA' CAMERA
    Private Sub Button13_Click(sender As Object, e As EventArgs) Handles Button13.Click
        If My.Settings.cam_on = True Then
            If My.Settings.SaveTitle21 = True Then
                MsgBox("Funzione disabilitata in" & vbCrLf & "Modalità Immagini e Diretta", MessageBoxIcon.Exclamation)
                Exit Sub
            Else
                If TextBox2.Text <> Nothing And NumericUpDown4.Text <> Nothing And TextBox4.Text <> Nothing Then
                    My.Settings.perc_file = TextBox2.Text & "\" & TextBox4.Text & "_" & NumericUpDown4.Text.PadLeft(5, "0") & "." & My.Settings.file_ext
                    If My.Computer.FileSystem.FileExists(My.Settings.perc_file) Then
                        My.Computer.Audio.PlaySystemSound(System.Media.SystemSounds.Beep)
                        MsgBox(My.Settings.perc_file & vbCrLf & vbCrLf & "Il File è Esitente!", MessageBoxIcon.Exclamation, MsgBoxStyle.OkOnly)
                        Exit Sub
                    End If
                    My.Settings.foto_in = True
                    My.Settings.perc_file = TextBox2.Text & "\" & TextBox4.Text & "_" & NumericUpDown4.Text.PadLeft(5, "0") & "." & My.Settings.file_ext
                    My.Settings.solo_file = (TextBox4.Text & "_" & NumericUpDown4.Text.PadLeft(5, "0") & "." & My.Settings.file_ext)
                    My.Settings.nome = TextBox4.Text
                    My.Settings.inizio = NumericUpDown4.Text
                    My.Settings.cart_lav = TextBox2.Text
                    NumericUpDown4.Text = NumericUpDown4.Text + 1
                    TextBox10.Text = TextBox2.Text & "\" & TextBox4.Text & "_" & NumericUpDown4.Text.PadLeft(5, "0") & "." & My.Settings.file_ext
                    Dim D As Integer = 0
                    Do While D <= 10
                        Application.DoEvents()
                        If My.Settings.foto_in = False And My.Settings.foto_in2 = False Then
                            Button13.Enabled = True
                            Exit Do
                        Else
                            Button13.Enabled = False
                            D = 0
                        End If
                    Loop



                Else
                    TextBox10.Text = ""
                    MsgBox("Inserisci percorso destinazione" & vbCrLf & vbCrLf & "Nome e Numero del Prossimo File", MessageBoxIcon.Exclamation)
                End If
            End If
        Else
            MsgBox("Apri Preview Camera", MessageBoxIcon.Exclamation)
        End If

    End Sub
    ' MODIFICA TESTO FILE DA CREARE
    Private Sub TextBox4_TextChanged(sender As Object, e As EventArgs) Handles TextBox4.TextChanged

        If TextBox2.Text <> Nothing And NumericUpDown4.Text <> Nothing And TextBox4.Text <> Nothing Then
            If My.Settings.foto_in = False And My.Settings.foto_in2 = False Then
                My.Settings.perc_file = (TextBox2.Text & "\" & TextBox4.Text & "_" & NumericUpDown4.Text.PadLeft(5, "0")) & "." & My.Settings.file_ext
                My.Settings.solo_file = (TextBox4.Text & "_" & NumericUpDown4.Text.PadLeft(5, "0") & "." & My.Settings.file_ext)
                TextBox10.Text = TextBox2.Text & "\" & TextBox4.Text & "_" & NumericUpDown4.Text.PadLeft(5, "0") & "." & My.Settings.file_ext
                My.Settings.nome = TextBox4.Text
            End If
        Else
            TextBox10.Text = ""
        End If
    End Sub
    ' MODIFICA NUMERO FILE DA CREARE
    Private Sub NumericUpDown4_TextChanged(sender As Object, e As EventArgs) Handles NumericUpDown4.TextChanged
        If TextBox2.Text <> Nothing And NumericUpDown4.Text <> Nothing And TextBox4.Text <> Nothing Then
            If My.Settings.foto_in = False And My.Settings.foto_in2 = False Then
                My.Settings.perc_file = (TextBox2.Text & "\" & TextBox4.Text & "_" & NumericUpDown4.Text.PadLeft(5, "0")) & "." & My.Settings.file_ext
                My.Settings.solo_file = (TextBox4.Text & "_" & NumericUpDown4.Text.PadLeft(5, "0") & "." & My.Settings.file_ext)
                TextBox10.Text = My.Settings.perc_file
                My.Settings.inizio = NumericUpDown4.Text
            End If
        Else
            TextBox10.Text = ""
        End If
    End Sub
    ' MODIFICA CARTELLA DI LAVORO
    Private Sub TextBox2_TextChanged(sender As Object, e As EventArgs) Handles TextBox2.TextChanged
        If TextBox2.Text <> Nothing And NumericUpDown4.Text <> Nothing And TextBox4.Text <> Nothing Then
            If My.Settings.foto_in = False And My.Settings.foto_in2 = False Then
                My.Settings.perc_file = (TextBox2.Text & "\" & TextBox4.Text & "_" & NumericUpDown4.Text.PadLeft(5, "0")) & "." & My.Settings.file_ext
                My.Settings.solo_file = (TextBox4.Text & "_" & NumericUpDown4.Text.PadLeft(5, "0") & "." & My.Settings.file_ext)
                TextBox10.Text = My.Settings.perc_file
                My.Settings.cart_lav = TextBox2.Text
            End If
        Else
            TextBox10.Text = ""
        End If
    End Sub
    ' SPEGNI AL TERMINE DEL CICLO
    Private Sub CheckBox1_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox1.CheckedChanged
        If CheckBox1.Checked = True Then
            CheckBox1.BackgroundImage = My.Resources.shutdownrosso
        Else
            CheckBox1.BackgroundImage = My.Resources.shutdown
        End If
    End Sub
    ' AGGIORNA LISTA PORTE COM
    Private Sub Button25_Click(sender As Object, e As EventArgs) Handles Button25.Click
        ComboBox1.Items.Clear()
        For Each sp As String In My.Computer.Ports.SerialPortNames
            ComboBox1.Items.Add(sp)
            ComboBox1.DroppedDown = True
        Next
    End Sub
    ' SELEZIONA MODALITA' CAMERA
    Private Sub RadioButton3_CheckedChanged(sender As Object, e As EventArgs) Handles RadioButton3.CheckedChanged
        MODO = True
        RadioButton3.BackColor = Color.Transparent
        RadioButton4.BackColor = Color.White
        ToolTip1.SetToolTip(Button1, "Avvia Azione Camera + Motore")
        GroupBox7.Text = "Controllo Camera e Motori Stepper"
        GroupBox5.Text = Nothing
        GroupBox9.Text = "Destinazione File"
        TextBox5.Text = ""
        TextBox6.Text = ""
        TextBox5.ReadOnly = True
        TextBox6.ReadOnly = True
        Label38.Text = "Attesa elaborazione Camera ms"

        Button10.Enabled = False
        Button16.Enabled = True
        Panel1.Visible = False
        Button13.Visible = True
        Button16.Visible = True
        TextBox6.Enabled = False
        TextBox5.Enabled = False
        Button10.Enabled = False
        Panel3.Visible = True
    End Sub
    ' SELEZIONA MODALITA' CLICK
    Private Sub RadioButton4_CheckedChanged(sender As Object, e As EventArgs) Handles RadioButton4.CheckedChanged
        MODO = False
        RadioButton3.BackColor = Color.White
        RadioButton4.BackColor = Color.Transparent
        ToolTip1.SetToolTip(Button1, "Avvia Azione Click + Motore")
        GroupBox7.Text = "Controllo Click e Motori Stepper"
        GroupBox5.Text = "Coordinate Click"
        GroupBox9.Text = Nothing
        TextBox5.ReadOnly = False
        TextBox6.ReadOnly = False
        Label38.Text = "Attesa azione click AVVIO ms"
        Button10.Enabled = True
        Button16.Enabled = False
        Panel1.Visible = True
        Button13.Visible = False
        Button16.Visible = False
        TextBox6.Enabled = True
        TextBox5.Enabled = True
        Button10.Enabled = True
        Panel3.Visible = False
        TextBox5.Text = My.Settings.SaveTitle3
        TextBox6.Text = My.Settings.SaveTitle4
    End Sub
    'SELEZIONA E AVVIO SOFTWARE DI ARDUINO 
    Private Sub Button26_MouseDow(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles Button26.MouseDown
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
    ' SELEZIONE TIPO DI FILE DA CREARE
    Private Sub ComboBox2_textchanged(sender As Object, e As EventArgs) Handles ComboBox2.TextChanged
        My.Settings.file_ext = ComboBox2.Text
        If TextBox2.Text <> Nothing And NumericUpDown4.Text <> Nothing And TextBox4.Text <> Nothing Then
            If My.Settings.foto_in = False And My.Settings.foto_in2 = False Then
                My.Settings.perc_file = (TextBox2.Text & "\" & TextBox4.Text & "_" & NumericUpDown4.Text.PadLeft(5, "0")) & "." & My.Settings.file_ext
                My.Settings.solo_file = (TextBox4.Text & "_" & NumericUpDown4.Text.PadLeft(5, "0") & "." & My.Settings.file_ext)
                TextBox10.Text = My.Settings.perc_file
                My.Settings.cart_lav = TextBox2.Text
            End If
        Else
            TextBox10.Text = ""
        End If
    End Sub
    ' APRI CREA VIDEO DA IMMAGINI
    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
        Form7.Close()
        Form8.Close()
        Form9.Show()
    End Sub
    ' ATTIVA TASTO CONFERMA AL CAMBIO DEL TEMPO DI ELABORAZIONE CAMERA
    Private Sub TextBox27_TextChanged(sender As Object, e As EventArgs) Handles TextBox27.TextChanged
        Button9.Visible = True
    End Sub
    ' CONFERMA MODIFICA TEMPO DI ELABORAZIONE CAMERA
    Private Sub Button9_Click(sender As Object, e As EventArgs) Handles Button9.Click
        My.Settings.PAUSA = TextBox27.Text
        Button9.Visible = False
    End Sub
    ' SE IL FILE DI DESINAZIONE E' ESISTENTE CAMBIA COLORE TESTO IN ROSSO
    Private Sub TextBox10_TextChanged(sender As Object, e As EventArgs) Handles TextBox10.TextChanged
        If TextBox10.Text IsNot Nothing Then
            If My.Computer.FileSystem.FileExists(My.Settings.perc_file) Then
                TextBox10.ForeColor = Color.Red
                ToolTip1.SetToolTip(TextBox10, "File Esistente!")
            Else
                TextBox10.ForeColor = Color.Green
                ToolTip1.SetToolTip(TextBox10, "File Destinazione")
            End If
        End If

    End Sub

End Class

