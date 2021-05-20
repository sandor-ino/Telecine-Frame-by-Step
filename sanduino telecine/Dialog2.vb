Imports System.Windows.Forms

Public Class Dialog2

    ' BOTTONE SALVA E CHIUDI
    Private Sub OK_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OK_Button.Click
        My.Settings.SaveTitle = TextBox16.Text
        My.Settings.SaveTitle1 = TextBox1.Text
        My.Settings.SaveTitle2 = TextBox15.Text
        My.Settings.SaveTitle7 = TextBox18.Text
        My.Settings.SaveTitle8 = TextBox17.Text
        My.Settings.SaveTitle9 = TextBox7.Text
        My.Settings.SaveTitle10 = TextBox9.Text
        My.Settings.SaveTitle11 = TextBox3.Text
        My.Settings.SaveTitle12 = TextBox5.Text
        My.Settings.SaveTitle13 = TextBox13.Text
        My.Settings.SaveTitle14 = TextBox11.Text
        My.Settings.SaveTitle15 = TextBox14.Text
        My.Settings.SaveTitle16 = TextBox12.Text
        My.Settings.SaveTitle17 = TextBox8.Text
        My.Settings.SaveTitle18 = TextBox10.Text
        My.Settings.SaveTitle19 = TextBox4.Text
        My.Settings.SaveTitle20 = TextBox6.Text
        My.Settings.SaveTitle23 = TextBox2.Text
        If CheckBox1.Checked = True Then
            My.Settings.SaveTile25 = 1
        Else
            My.Settings.SaveTile25 = 0
        End If
        If CheckBox2.Checked = True Then
            My.Settings.SaveTile26 = 1
        Else
            My.Settings.SaveTile26 = 0
        End If
        Me.Close()
    End Sub
    ' APERTURA PROGRAMMA
    Private Sub Dialog2_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        TextBox16.Text = My.Settings.SaveTitle
        TextBox1.Text = My.Settings.SaveTitle1
        TextBox15.Text = My.Settings.SaveTitle2
        TextBox18.Text = My.Settings.SaveTitle7
        TextBox17.Text = My.Settings.SaveTitle8
        TextBox7.Text = My.Settings.SaveTitle9
        TextBox9.Text = My.Settings.SaveTitle10
        TextBox3.Text = My.Settings.SaveTitle11
        TextBox5.Text = My.Settings.SaveTitle12
        TextBox13.Text = My.Settings.SaveTitle13
        TextBox11.Text = My.Settings.SaveTitle14
        TextBox14.Text = My.Settings.SaveTitle15
        TextBox12.Text = My.Settings.SaveTitle16
        TextBox8.Text = My.Settings.SaveTitle17
        TextBox10.Text = My.Settings.SaveTitle18
        TextBox4.Text = My.Settings.SaveTitle19
        TextBox6.Text = My.Settings.SaveTitle20
        TextBox2.Text = My.Settings.SaveTitle23
        If My.Settings.SaveTile25 <> 0 Then
            CheckBox1.Checked = True
        Else
            CheckBox1.Checked = False
        End If
        If My.Settings.SaveTile26 <> 0 Then
            CheckBox2.Checked = True
        Else
            CheckBox2.Checked = False
        End If
    End Sub
    ' APRE INFO
    Private Sub PictureBox5_Click(sender As Object, e As EventArgs) Handles PictureBox5.Click
        Form2.Show()
    End Sub
    ' APRE INFO
    Private Sub PictureBox2_Click(sender As Object, e As EventArgs) Handles PictureBox2.Click
        Form3.Show()
    End Sub
    ' APRE INFO
    Private Sub PictureBox4_Click(sender As Object, e As EventArgs) Handles PictureBox4.Click
        Form4.Show()
    End Sub
    ' APRE INFO
    Private Sub PictureBox3_Click(sender As Object, e As EventArgs) Handles PictureBox3.Click
        Form5.Show()
    End Sub
    ' APRE INFO
    Private Sub PictureBox6_Click(sender As Object, e As EventArgs) Handles PictureBox6.Click
        Form6.Show()
    End Sub
    ' CARICA DATI IN MEMORIA
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        TextBox16.Text = My.Settings.SaveTitle
        TextBox1.Text = My.Settings.SaveTitle1
        TextBox15.Text = My.Settings.SaveTitle2
        TextBox18.Text = My.Settings.SaveTitle7
        TextBox17.Text = My.Settings.SaveTitle8
        TextBox7.Text = My.Settings.SaveTitle9
        TextBox9.Text = My.Settings.SaveTitle10
        TextBox3.Text = My.Settings.SaveTitle11
        TextBox5.Text = My.Settings.SaveTitle12
        TextBox13.Text = My.Settings.SaveTitle13
        TextBox11.Text = My.Settings.SaveTitle14
        TextBox14.Text = My.Settings.SaveTitle15
        TextBox12.Text = My.Settings.SaveTitle16
        TextBox8.Text = My.Settings.SaveTitle17
        TextBox10.Text = My.Settings.SaveTitle18
        TextBox4.Text = My.Settings.SaveTitle19
        TextBox6.Text = My.Settings.SaveTitle20
        TextBox2.Text = My.Settings.SaveTitle23
        If My.Settings.SaveTile25 <> 0 Then
            CheckBox1.Checked = True
        Else
            CheckBox1.Checked = False
        End If
        If My.Settings.SaveTile26 <> 0 Then
            CheckBox2.Checked = True
        Else
            CheckBox2.Checked = False
        End If
    End Sub

    Private Sub PictureBox7_Click(sender As Object, e As EventArgs) Handles PictureBox7.Click
        Form10.Show()
    End Sub
End Class
