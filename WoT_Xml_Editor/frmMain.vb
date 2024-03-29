﻿
Imports System.Globalization
Imports System.IO
Imports System.Text.RegularExpressions
Imports FastColoredTextBoxNS

Public Class frmMain

	Dim f As Integer

	Dim position As Integer = 1
	Public Shared WTbuild As String = ""
	Public Shared opened_file_name As String = ""
	Dim keyWords As String = ""
	Public Shared TempPath As String = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\AppData\\Local\\Coffee_\Update"
	Public Shared ModSource As String = ""
	Public Shared ModName As String = ""
	Public Shared ModDestStr As String = ""
	'----------------------------------------------
	Public numeric_ As New TextStyle(Brushes.PaleGreen, Nothing, FontStyle.Regular)
	Public tags_ As New TextStyle(Nothing, Nothing, FontStyle.Regular Or FontStyle.Underline)
	Public text_ As New TextStyle(Brushes.Gray, Nothing, FontStyle.Regular)
	Public properties_ As New TextStyle(Brushes.PowderBlue, Nothing, FontStyle.Regular)
	Public allother_ As New TextStyle(Brushes.LightSeaGreen, Nothing, FontStyle.Regular)

	Dim SameWordsStyle As MarkerStyle = New MarkerStyle(New SolidBrush(Color.FromArgb(40, Color.Gray)))

	Private Sub frmMain_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown
		If e.Modifiers = Keys.Control AndAlso e.KeyCode = Keys.S Then
			SaveToolStripButton.PerformClick()
		End If
		If e.Modifiers = Keys.Control AndAlso e.KeyCode = Keys.O Then
			OpenToolStripButton.PerformClick()
		End If
	End Sub



	Private Sub frmMain_Load(sender As Object, e As EventArgs) Handles Me.Load

		Me.Text = "Last Mod Path: "
		' Create new temp folder
		If Not System.IO.Directory.Exists(TempPath) Then
			System.IO.Directory.CreateDirectory(TempPath)
		End If

		'Read last mod path
		If System.IO.File.Exists(TempPath + "\\DestPath.txt") Then
			Dim fileReader As System.IO.StreamReader
			fileReader =
			My.Computer.FileSystem.OpenTextFileReader(TempPath + "\\DestPath.txt")
			Dim stringReader As String
			stringReader = fileReader.ReadLine()
			ModDestStr = stringReader
			Me.Text += ModDestStr
			fileReader.Close()
		Else
			MsgBox("Last Path not found")
		End If

		'Read last mod name
		If System.IO.File.Exists(TempPath + "\\ModName.txt") Then
			Dim fileReader1 As System.IO.StreamReader
			fileReader1 =
			My.Computer.FileSystem.OpenTextFileReader(TempPath + "\\ModName.txt")
			Dim stringReader1 As String
			stringReader1 = fileReader1.ReadLine()
			ToolStripTextBox1.Text = stringReader1
			ModName = stringReader1

			Me.Text += "\" + ModName + ".wotmod"

			fileReader1.Close()
		Else
			MsgBox("Last Mod name not found")
		End If

		'Read last mod name
		If System.IO.File.Exists(TempPath + "\\ModSource.txt") Then
			Dim fileReader3 As System.IO.StreamReader
			fileReader3 =
			My.Computer.FileSystem.OpenTextFileReader(TempPath + "\\ModSource.txt")
			Dim stringReader3 As String
			stringReader3 = fileReader3.ReadLine()
			ModSource = stringReader3
			fileReader3.Close()

		Else
			MsgBox("Last Mod source not found")
		End If

		My.Settings.Upgrade() ' upgrades to keep old settings

		Dim nonInvariantCulture As System.Globalization.CultureInfo = New CultureInfo("en-US")
		nonInvariantCulture.NumberFormat.NumberDecimalSeparator = "."
		System.Threading.Thread.CurrentThread.CurrentCulture = nonInvariantCulture

		Dim arguments() As String = Environment.GetCommandLineArgs()

		Dim ts = IO.File.ReadAllText(Application.StartupPath + "\filtered_strings.txt")
		filterlist = ts.Split(ControlChars.CrLf.ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
		set_keywords()
		Me.Show()
		Application.DoEvents()
		Application.DoEvents()
		Application.DoEvents()
		Application.DoEvents()
		'frmSetColors.Show()
		get_color_settings()
		set_styles()

		'Me.Text += " Version:" + Application.ProductVersion
		Temp_Storage = Path.GetTempPath ' this gets the user temp storage folder
		Temp_Storage += "wot_xml_temp"
		If Not System.IO.Directory.Exists(Temp_Storage) Then
			System.IO.Directory.CreateDirectory(Temp_Storage)
		End If

		associate_files() 'broken in windows 10
		Me.KeyPreview = True    'so i catch keyboard before despatching it
		'For i = 1 To arguments.Length - 1
		'	fctb.Text += i.ToString + ":" + arguments(i) + vbCrLf
		'Next
		'If (Not System.IO.Directory.Exists(Application.StartupPath + "\temp")) Then
		'	System.IO.Directory.CreateDirectory(Application.StartupPath + "\temp")
		'End If
		Dim fs As String = ""
		If arguments IsNot Nothing Then
			For i = 0 To arguments.Length - 1
				fs += arguments(i) + vbCrLf
			Next
			If arguments.Length > 1 Then
				Dim s1 = arguments(1)
				If s1 <> String.Empty Then
					opened_file_name = s1
					Me.Text = "File: " + opened_file_name
					'IO.File.WriteAllText("C:\temp_\test.txt", fs + vbCrLf + "Made it")
					openVisual(s1)
				End If
			End If

		End If

	End Sub
	Private Sub get_color_settings()
		colors(0) = My.Settings.numeric
		colors(1) = My.Settings.tags
		colors(2) = My.Settings.textures
		colors(3) = My.Settings.props
		colors(4) = My.Settings.allothers
		For i = 0 To 4
			Dim state As Boolean = False
			Select Case i
				Case 0
					state = My.Settings.n_checked
				Case 1
					state = My.Settings.t_checked
				Case 2
					state = My.Settings.tex_c
				Case 3
					state = My.Settings.prop_c
				Case 4
					state = My.Settings.all_c
			End Select
			If state Then
				text_style(i) = 1
			Else
				text_style(i) = 0
			End If
		Next
	End Sub

	Private Sub associate_files()
		CreateFileAssociation(".xml", "wot_visual_file", "XML File", Application.StartupPath + "\" + "WoT_Xml_Editor.exe")
		CreateFileAssociation(".model", "wot_visual_file", "Wot model file", Application.StartupPath + "\" + "WoT_Xml_Editor.exe")
		CreateFileAssociation(".visual_processed", "wot_visual_file", "Wot visual_processed file", Application.StartupPath + "\" + "WoT_Xml_Editor.exe")
		CreateFileAssociation(".visual", "wot_visual_file", "Wot Visual File", Application.StartupPath + "\" + "WoT_Xml_Editor.exe")
		CreateFileAssociation(".settings", "wot_visual_file", "Settings File", Application.StartupPath + "\" + "WoT_Xml_Editor.exe")
	End Sub

	<System.Runtime.InteropServices.DllImport("shell32.dll")> Shared Sub _
	 SHChangeNotify(ByVal wEventId As Integer, ByVal uFlags As Integer,
	 ByVal dwItem1 As Integer, ByVal dwItem2 As Integer)
	End Sub
	' Create the new file association
	'
	' Extension is the extension to be registered (eg ".cad"
	' ClassName is the name of the associated class (eg "CADDoc")
	' Description is the textual description (eg "CAD Document"
	' ExeProgram is the app that manages that extension (eg "c:\Cad\MyCad.exe")
	Function CreateFileAssociation(ByVal extension As String,
	 ByVal className As String, ByVal description As String,
	 ByVal exeProgram As String) As Boolean

		Const SHCNE_ASSOCCHANGED = &H8000000
		Const SHCNF_IDLIST = 0

		' ensure that there is a leading dot
		If extension.Substring(0, 1) <> "." Then
			extension = "." & extension
		End If

		Dim key1, key2, key3, key4 As Microsoft.Win32.RegistryKey
		Try
			' create a value for this key that contains the classname
			key1 = Microsoft.Win32.Registry.ClassesRoot.CreateSubKey(extension)
			key1.SetValue("", className)
			' create a new key for the Class name
			key2 = Microsoft.Win32.Registry.ClassesRoot.CreateSubKey(className)
			key2.SetValue("", description)
			' associate the program to open the files with this extension
			key3 = Microsoft.Win32.Registry.ClassesRoot.CreateSubKey(className &
				 "\Shell\Open\Command")
			key3.SetValue("", exeProgram & " ""%1""")
			Dim extstr = "Software\Microsoft\Windows\CurrentVersion\ Explorer\FileExts\" + extension + "\UserChoice"
			key4 = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(extstr, True)
			If key4 IsNot Nothing Then
				key4.SetValue("", "application\" + exeProgram)
			End If
		Catch e As Exception
			Return False
		Finally
			If Not key1 Is Nothing Then key1.Close()
			If Not key2 Is Nothing Then key2.Close()
			If Not key3 Is Nothing Then key3.Close()
			If Not key4 Is Nothing Then key3.Close()
		End Try

		' notify Windows that file associations have changed
		SHChangeNotify(SHCNE_ASSOCCHANGED, SHCNF_IDLIST, 0, 0)
		Return True
	End Function


	Private Sub OpenToolStripButton_Click(sender As Object, e As EventArgs) Handles OpenToolStripButton.Click
		OpenFileDialog1.Filter = "All Files (*.*)|*.*|XML (*.xml)|*.xml|visual_processed (*.visual_processed)|*.visual_processed|model (*.model)|*.model|visual (*.visual)|*.visual"
		OpenFileDialog1.FileName = ""
		position = 1
		Try
			fctb.Text = ""
		Catch ex As Exception

		End Try
		If OpenFileDialog1.ShowDialog = Windows.Forms.DialogResult.OK Then
			opened_file_name = OpenFileDialog1.FileName
			Me.Text = "File: " + opened_file_name
			openVisual(OpenFileDialog1.FileName)
			frmMain.opened_file_name = opened_file_name

		End If
	End Sub

	Private Sub SaveToolStripButton_Click(sender As Object, e As EventArgs) Handles SaveToolStripButton.Click
		SaveFileDialog1.Filter = "All Files (*.*)|*.*|XML (*.xml)|*.xml|visual_processed (*.visual_processed)|*.visual_processed|model (*.model)|*.model|visual (*.visual)|*.visual"
		SaveFileDialog1.FileName = opened_file_name
		If SaveFileDialog1.ShowDialog(Me) = Windows.Forms.DialogResult.OK Then
			Dim txt = fctb.Text.Replace("<shared>shared</shared>", "shared")
			txt = txt.Replace("  ", "")
			For i = 90 To 0 Step -1
				Dim ast = txt.Replace("<primitiveGroup>" + ControlChars.CrLf.ToCharArray() + "<PG_ID>" + i.ToString + "</PG_ID>", "<primitiveGroup>" + i.ToString)
				txt = ast
			Next
			txt = txt.Replace("><", ">" + vbCrLf + "<")
			txt = txt.Replace(vbCrLf, vbLf)
			If Not txt.Contains("!--<xmlref>") Then
				txt = txt.Replace("<xmlref>", "<!--<xmlref>")
				txt = txt.Replace("</xmlref>", "<xmlref>-->")
			End If
			'txt = txt + vbLf

			txt = txt.Replace("rect1x4direction",
							  "rect1x4 direction")

			txt = txt.Replace("squaredirection",
							  "square direction")

			txt = txt.Replace("rightformfactor",
							  "right formfactor")

			txt = txt.Replace("rect1x3direction",
							  "rect1x3 direction")

			txt = txt.Replace("rect1x2direction",
							  "rect1x2 direction")

			txt = txt.Replace("rect1x6direction",
							  "rect1x6 direction")



			IO.File.WriteAllText(SaveFileDialog1.FileName, txt)
		End If

	End Sub

	Private Sub CutToolStripButton_Click(sender As Object, e As EventArgs) Handles CutToolStripButton.Click
		If fctb.Text.Length > 0 Then
			fctb.Cut()
		End If
	End Sub

	Private Sub CopyToolStripButton_Click(sender As Object, e As EventArgs) Handles CopyToolStripButton.Click
		If fctb.Text.Length > 0 Then
			fctb.Copy()
		End If
	End Sub

	Private Sub PasteToolStripButton_Click(sender As Object, e As EventArgs) Handles PasteToolStripButton.Click
		If fctb.Text.Length > 0 Then
			If Clipboard.ContainsText Then
				fctb.Paste()
			End If
		End If
	End Sub

	Private Sub m_Delete_Click(sender As Object, e As EventArgs) Handles m_Delete.Click
		If fctb.Text.Length > 0 Then
			fctb.ClearSelected()
		End If

	End Sub

	Private Sub m_undo_Click(sender As Object, e As EventArgs) Handles m_undo.Click
		If fctb.Text.Length > 0 Then
			fctb.Undo()
		End If
	End Sub

	Private Sub m_redo_Click(sender As Object, e As EventArgs) Handles m_redo.Click
		If fctb.Text.Length > 0 Then
			fctb.Redo()
		End If
	End Sub

	Private Sub m_search_Click(sender As Object, e As EventArgs) Handles m_search.Click
		search()
	End Sub

	Private Sub m_search_text_KeyDown(sender As Object, e As KeyEventArgs) Handles m_search_text.KeyDown
		If e.KeyCode = Keys.Enter Then
			If fctb.Text.Length > 0 Then
				search()
			End If
		End If
		e.Handled = True
	End Sub


	Private Sub m_search_text_TextChanged(sender As Object, e As EventArgs) Handles m_search_text.TextChanged
		position = 1
	End Sub
	Private Sub search()
		Dim s = InStr(position, fctb.Text.ToLower, m_search_text.Text.ToLower)
		If s > 0 Then
			fctb.Focus()
			fctb.SelectionStart = s - 1
			Application.DoEvents()
			fctb.DoSelectionVisible()
			fctb.SelectionLength = m_search_text.Text.Length
			Application.DoEvents()
			position = s + m_search_text.Text.Length
		Else
			position = 1
		End If
	End Sub

	Private Sub m_edit_Filter_Click(sender As Object, e As EventArgs) Handles m_edit_Filter.Click
		fctb.Text = ""
		fctb.Text = IO.File.ReadAllText(Application.StartupPath + "\filtered_strings.txt")
		opened_file_name = Application.StartupPath + "\filtered_strings.txt"

		Dim ts = IO.File.ReadAllText(Application.StartupPath + "\filtered_strings.txt")
		filterlist = ts.Split(ControlChars.CrLf.ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
		set_keywords()
	End Sub

	Public Sub set_styles()

		fctb.Styles(0) = New TextStyle(getBrush(0), Nothing, text_style(0))
		fctb.Styles(1) = New TextStyle(getBrush(1), Nothing, text_style(1))
		fctb.Styles(2) = New TextStyle(getBrush(2), Nothing, text_style(2))
		fctb.Styles(3) = New TextStyle(getBrush(3), Nothing, text_style(3))
		fctb.Styles(4) = New TextStyle(getBrush(4), Nothing, text_style(4))
		Dim e As New TextChangedEventArgs(New FastColoredTextBoxNS.Range(fctb))
		'fctb.ForeColor = colors(4)
		'If text_style(4) = CheckState.Checked Then
		'	fctb.Font = New Font(fctb.Font, FontStyle.Bold)
		'Else
		'	fctb.Font = New Font(fctb.Font, FontStyle.Regular)
		'End If

		SyntaxHighlight(fctb, e)
		fctb.Refresh()
		Application.DoEvents()

	End Sub
	Private Sub set_keywords()
		keyWords = "\b("
		For Each s In filterlist
			If InStr(s, "#") = 0 Then
				If s.Length > 2 Then
					keyWords += s + "|"
				End If
			End If
		Next
		keyWords += "diffuseMap2|primitiveGroup|/primitiveGroup)\b"
	End Sub

	Private Function getBrush(Id As Integer) As SolidBrush
		Dim br As SolidBrush
		Dim c As Color
		c = colors(Id)
		br = New SolidBrush(c)
		Return br
	End Function

	Private Sub fctb_TextChanged(sender As Object, e As FastColoredTextBoxNS.TextChangedEventArgs) Handles fctb.TextChanged
		SyntaxHighlight(sender, e)
	End Sub
	Private Sub SyntaxHighlight(ByRef sender As FastColoredTextBox, e As TextChangedEventArgs)
		e.ChangedRange.SetFoldingMarkers("", "")
		sender.LeftBracket = "("c
		sender.RightBracket = ")"c
		sender.LeftBracket2 = ControlChars.NullChar
		sender.RightBracket2 = ControlChars.NullChar
		'clear style of changed range
		e.ChangedRange.ClearStyle(fctb.Styles(0), fctb.Styles(1), fctb.Styles(2), fctb.Styles(3), fctb.Styles(4))

		'string highlighting
		e.ChangedRange.SetStyle(fctb.Styles(4), "(.*?)")
		e.ChangedRange.SetStyle(fctb.Styles(2), "(?<=\<Texture\>).*?(?=\<\/Texture\>)", RegexOptions.Multiline)

		'XML tags
		e.ChangedRange.SetStyle(fctb.Styles(1), "(<.[^(><.)]+>)", RegexOptions.Multiline)

		'keyword highlighting
		e.ChangedRange.SetStyle(fctb.Styles(3), keyWords)


		'number highlighting
		e.ChangedRange.SetStyle(fctb.Styles(0), "\b\d+[\.]?\d*([eE]\-?\d+)?[lLdDfF]?\b|\b0x[a-fA-F\d]+\b")
		'clear folding markers
		e.ChangedRange.ClearFoldingMarkers()


	End Sub



	Private Sub ToolStripButton1_Click(sender As Object, e As EventArgs) Handles ToolStripButton1.Click
		FontDialog1.FixedPitchOnly = True
		If FontDialog1.ShowDialog = Windows.Forms.DialogResult.OK Then
			Dim f As System.Drawing.Font = FontDialog1.Font
			fctb.Font = f
		End If
		fctb.Update()
	End Sub


	Private Sub cm_cut_Click(sender As Object, e As EventArgs) Handles cm_cut.Click
		If fctb.Text.Length > 0 Then
			fctb.Cut()
		End If
	End Sub

	Private Sub cm_copy_Click(sender As Object, e As EventArgs) Handles cm_copy.Click
		If fctb.Text.Length > 0 Then
			fctb.Copy()
		End If
	End Sub

	Private Sub cm_paste_Click(sender As Object, e As EventArgs) Handles cm_paste.Click
		If fctb.Text.Length > 0 Then
			If Clipboard.ContainsText Then
				fctb.Paste()
			End If
		End If
	End Sub

	Private Sub cm_delete_Click(sender As Object, e As EventArgs) Handles cm_delete.Click
		If fctb.Text.Length > 0 Then
			fctb.ClearSelected()
		End If
	End Sub

	Private Sub cm_undo_Click(sender As Object, e As EventArgs) Handles cm_undo.Click
		If fctb.Text.Length > 0 Then
			fctb.Undo()
		End If
	End Sub

	Private Sub cm_redo_Click(sender As Object, e As EventArgs) Handles cm_redo.Click
		If fctb.Text.Length > 0 Then
			fctb.Redo()
		End If
	End Sub

	Private Sub OpenToolStripButton_MouseEnter(sender As Object, e As EventArgs) Handles OpenToolStripButton.MouseEnter
		ToolStrip1.Focus()
	End Sub

	Private Sub ToolStrip1_Enter(sender As Object, e As EventArgs) Handles ToolStrip1.Enter
		ToolStrip1.Focus()
	End Sub


	Private Sub m_set_colors_Click(sender As Object, e As EventArgs) Handles m_set_colors.Click
		frmSetColors.Show()
	End Sub

	Private Sub m_help_Click(sender As Object, e As EventArgs) Handles m_help.Click
		Dim s As String = Application.StartupPath
		System.Diagnostics.Process.Start(s + "\html\help.html")
	End Sub

	Private Sub fctb_Load(sender As Object, e As EventArgs) Handles fctb.Load

	End Sub

	Private Sub ToolStripButton3_Click(sender As Object, e As EventArgs) Handles ToolStripButton3.Click

		Dim fbd As New FolderBrowserDialog
		fbd.RootFolder = Environment.SpecialFolder.MyComputer
		fbd.SelectedPath = ModDestStr
		If fbd.ShowDialog = DialogResult.OK Then
			WTbuild = fbd.SelectedPath
			Me.Text += " | Now Build to => " + WTbuild

		End If

		If f = 0 Then
			ToolStripButton3.BackColor = Color.Red
		End If

		ModDestStr = WTbuild

		Dim file As System.IO.StreamWriter
		file = My.Computer.FileSystem.OpenTextFileWriter(TempPath + "\\DestPath.txt", False)
		file.WriteLine(WTbuild)
		file.Close()

	End Sub

	Private Sub ToolStripButton2_Click(sender As Object, e As EventArgs) Handles ToolStripButton2.Click

		'Read last mod path
		If System.IO.File.Exists(TempPath + "\\DestPath.txt") Then
			Dim fileReader As System.IO.StreamReader
			fileReader =
			My.Computer.FileSystem.OpenTextFileReader(TempPath + "\\DestPath.txt")
			Dim stringReader As String
			stringReader = fileReader.ReadLine()
			fileReader.Close()
			stringReader = ModDestStr
		Else
			Dim fbd As New FolderBrowserDialog
			fbd.RootFolder = Environment.SpecialFolder.MyComputer
			If fbd.ShowDialog = DialogResult.OK Then
				WTbuild = fbd.SelectedPath
				Me.Text += " | Build to => " + WTbuild + "\" + ModName

			End If

			If f = 0 Then
				ToolStripButton3.BackColor = Color.Red
			End If

			Dim file As System.IO.StreamWriter
			file = My.Computer.FileSystem.OpenTextFileWriter(TempPath + "\\DestPath.txt", False)
			file.WriteLine(WTbuild)
			file.Close()
			WTbuild = ModDestStr
		End If

		'Read last mod name
		If System.IO.File.Exists(TempPath + "\\ModName.txt") Then
			Dim fileReader1 As System.IO.StreamReader
			fileReader1 =
			My.Computer.FileSystem.OpenTextFileReader(TempPath + "\\ModName.txt")
			Dim stringReader1 As String
			stringReader1 = fileReader1.ReadLine()
			fileReader1.Close()
			stringReader1 = ModName
		Else
			Dim ans As String = Nothing
			ModName = InputBox("Mod Name not found", "NAME!")
			Dim file As System.IO.StreamWriter
			file = My.Computer.FileSystem.OpenTextFileWriter(TempPath + "\\ModName.txt", False)
			file.WriteLine(ModName)
			file.Close()
			ToolStripTextBox1.Text = ModName

		End If

		If System.IO.File.Exists("C:\Program Files\7-Zip\7zFM.exe") Then
			Dim exePath As String = "C:\Program Files\7-Zip\7z.exe"
			Dim args As String = " a -mx=0 " + """" + TempPath + "\mod.zip""" + " " + """" + ModSource + """"
			Process.Start(exePath, args).WaitForExit()
			My.Computer.FileSystem.CopyFile(TempPath + "\mod.zip", ModDestStr + "\" + ModName + ".wotmod", True)
			My.Computer.FileSystem.DeleteFile(TempPath + "\mod.zip")

			MsgBox("Mod builded to: " + ModDestStr + "\" + ModName + ".wotmod")
		Else
			MsgBox("7-Zip not found " + Environment.NewLine + "You can be downloaded here: www.7-zip.org")
		End If

	End Sub

	Private Sub ToolStripTextBox1_Click(sender As Object, e As EventArgs) Handles ToolStripTextBox1.Click
		If String.IsNullOrEmpty(ToolStripTextBox1.Text) Then
			MsgBox("Mod Name Empty!")
		End If

		Dim s As String
		s = ToolStripTextBox1.Text
		Dim file As System.IO.StreamWriter
		file = My.Computer.FileSystem.OpenTextFileWriter(TempPath + "\\ModName.txt", False)
		file.WriteLine(s)
		file.Close()
		ModName = s
		MsgBox("New mod name: " + s)

		If f = 0 Then
			ToolStripTextBox1.BackColor = Color.Red
		End If

	End Sub

	Private Sub ToolStripButton4_Click(sender As Object, e As EventArgs) Handles ToolStripButton4.Click

		MsgBox("Select the folder ""res"" which is located in your mod folder." + Environment.NewLine + "Example: C:\temp\My Mod Name\res")
		Dim fbd As New FolderBrowserDialog
		fbd.RootFolder = Environment.SpecialFolder.MyComputer
		fbd.SelectedPath = ModSource

		If fbd.ShowDialog = DialogResult.OK Then
			ModSource = fbd.SelectedPath
			Dim file As System.IO.StreamWriter
			file = My.Computer.FileSystem.OpenTextFileWriter(TempPath + "\\ModSource.txt", False)
			file.WriteLine(ModSource)
			file.Close()
		End If

		If f = 0 Then
			ToolStripButton4.BackColor = Color.Red
		End If

	End Sub

	'Private Function Left(sFolder As String, p2 As Integer) As Object
	'	Throw New NotImplementedException
	'End Function

End Class
