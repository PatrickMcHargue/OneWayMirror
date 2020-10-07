<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class OneWayMirror
  Inherits System.Windows.Forms.Form

  'Form overrides dispose to clean up the component list.
  <System.Diagnostics.DebuggerNonUserCode()> _
  Protected Overrides Sub Dispose(ByVal disposing As Boolean)
    Try
      If disposing AndAlso components IsNot Nothing Then
        components.Dispose()
      End If
    Finally
      MyBase.Dispose(disposing)
    End Try
  End Sub

  'Required by the Windows Form Designer
  Private components As System.ComponentModel.IContainer

  'NOTE: The following procedure is required by the Windows Form Designer
  'It can be modified using the Windows Form Designer.  
  'Do not modify it using the code editor.
  <System.Diagnostics.DebuggerStepThrough()> _
  Private Sub InitializeComponent()
    Me.components = New System.ComponentModel.Container()
    Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(OneWayMirror))
    Me.SourceDirectoryLabel = New System.Windows.Forms.Label()
    Me.DestinationDirectoryLabel = New System.Windows.Forms.Label()
    Me.LocateSourceDirectory = New System.Windows.Forms.Button()
    Me.LocateDestinationDirectory = New System.Windows.Forms.Button()
    Me.InstructionsLabel = New System.Windows.Forms.Label()
    Me.StartCopy = New System.Windows.Forms.Button()
    Me.SetupGroupBox = New System.Windows.Forms.GroupBox()
    Me.DestinationDirectory = New UIelements.ComboBoxWithMemory()
    Me.SourceDirectory = New UIelements.ComboBoxWithMemory()
    Me.OnlyCommon = New System.Windows.Forms.CheckBox()
    Me.IncludeHidden = New System.Windows.Forms.CheckBox()
    Me.IncludeSystem = New System.Windows.Forms.CheckBox()
    Me.IncludeSubDirectories = New System.Windows.Forms.CheckBox()
    Me.ExecuteScript = New System.Windows.Forms.Button()
    Me.CancelExitButton = New UIelements.CancelExitButton()
    Me.FileProgressBar = New XpControls.XpProgressBar()
    Me.DirectoryProgressBar = New XpControls.XpProgressBar()
    Me.CommentLabel = New System.Windows.Forms.Label()
    Me.RunAsAdmin = New System.Windows.Forms.Button()
    Me.OWMfiles = New UIelements.ComboBoxWithMemory()
    Me.ToolTip1 = New System.Windows.Forms.ToolTip(Me.components)
    Me.OWMfileGroup = New System.Windows.Forms.GroupBox()
    Me.LocateScript = New System.Windows.Forms.Button()
    Me.SaveScript = New System.Windows.Forms.Button()
    Me.SetupGroupBox.SuspendLayout()
    Me.OWMfileGroup.SuspendLayout()
    Me.SuspendLayout()
    '
    'SourceDirectoryLabel
    '
    Me.SourceDirectoryLabel.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
    Me.SourceDirectoryLabel.Location = New System.Drawing.Point(8, 19)
    Me.SourceDirectoryLabel.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
    Me.SourceDirectoryLabel.Name = "SourceDirectoryLabel"
    Me.SourceDirectoryLabel.Size = New System.Drawing.Size(221, 31)
    Me.SourceDirectoryLabel.TabIndex = 10
    Me.SourceDirectoryLabel.Text = "Source Directory:"
    Me.SourceDirectoryLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight
    '
    'DestinationDirectoryLabel
    '
    Me.DestinationDirectoryLabel.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
    Me.DestinationDirectoryLabel.Location = New System.Drawing.Point(8, 62)
    Me.DestinationDirectoryLabel.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
    Me.DestinationDirectoryLabel.Name = "DestinationDirectoryLabel"
    Me.DestinationDirectoryLabel.Size = New System.Drawing.Size(221, 31)
    Me.DestinationDirectoryLabel.TabIndex = 10
    Me.DestinationDirectoryLabel.Text = "Destination Directory:"
    Me.DestinationDirectoryLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight
    '
    'LocateSourceDirectory
    '
    Me.LocateSourceDirectory.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
    Me.LocateSourceDirectory.Location = New System.Drawing.Point(773, 19)
    Me.LocateSourceDirectory.Margin = New System.Windows.Forms.Padding(4)
    Me.LocateSourceDirectory.Name = "LocateSourceDirectory"
    Me.LocateSourceDirectory.Size = New System.Drawing.Size(111, 30)
    Me.LocateSourceDirectory.TabIndex = 2
    Me.LocateSourceDirectory.Text = "Locate..."
    Me.LocateSourceDirectory.UseVisualStyleBackColor = True
    '
    'LocateDestinationDirectory
    '
    Me.LocateDestinationDirectory.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
    Me.LocateDestinationDirectory.Location = New System.Drawing.Point(773, 62)
    Me.LocateDestinationDirectory.Margin = New System.Windows.Forms.Padding(4)
    Me.LocateDestinationDirectory.Name = "LocateDestinationDirectory"
    Me.LocateDestinationDirectory.Size = New System.Drawing.Size(111, 30)
    Me.LocateDestinationDirectory.TabIndex = 3
    Me.LocateDestinationDirectory.Text = "Locate..."
    Me.LocateDestinationDirectory.UseVisualStyleBackColor = True
    '
    'InstructionsLabel
    '
    Me.InstructionsLabel.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
    Me.InstructionsLabel.Location = New System.Drawing.Point(13, 148)
    Me.InstructionsLabel.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
    Me.InstructionsLabel.Name = "InstructionsLabel"
    Me.InstructionsLabel.Size = New System.Drawing.Size(573, 72)
    Me.InstructionsLabel.TabIndex = 10
    Me.InstructionsLabel.Text = resources.GetString("InstructionsLabel.Text")
    Me.InstructionsLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
    '
    'StartCopy
    '
    Me.StartCopy.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
    Me.StartCopy.Location = New System.Drawing.Point(594, 158)
    Me.StartCopy.Margin = New System.Windows.Forms.Padding(4)
    Me.StartCopy.Name = "StartCopy"
    Me.StartCopy.Size = New System.Drawing.Size(131, 33)
    Me.StartCopy.TabIndex = 8
    Me.StartCopy.Text = "Start Copy"
    Me.StartCopy.UseVisualStyleBackColor = True
    '
    'SetupGroupBox
    '
    Me.SetupGroupBox.Controls.Add(Me.DestinationDirectory)
    Me.SetupGroupBox.Controls.Add(Me.SourceDirectory)
    Me.SetupGroupBox.Controls.Add(Me.OnlyCommon)
    Me.SetupGroupBox.Controls.Add(Me.IncludeHidden)
    Me.SetupGroupBox.Controls.Add(Me.IncludeSystem)
    Me.SetupGroupBox.Controls.Add(Me.IncludeSubDirectories)
    Me.SetupGroupBox.Controls.Add(Me.SourceDirectoryLabel)
    Me.SetupGroupBox.Controls.Add(Me.DestinationDirectoryLabel)
    Me.SetupGroupBox.Controls.Add(Me.LocateSourceDirectory)
    Me.SetupGroupBox.Controls.Add(Me.LocateDestinationDirectory)
    Me.SetupGroupBox.Dock = System.Windows.Forms.DockStyle.Top
    Me.SetupGroupBox.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
    Me.SetupGroupBox.Location = New System.Drawing.Point(0, 0)
    Me.SetupGroupBox.Margin = New System.Windows.Forms.Padding(4)
    Me.SetupGroupBox.Name = "SetupGroupBox"
    Me.SetupGroupBox.Padding = New System.Windows.Forms.Padding(4)
    Me.SetupGroupBox.Size = New System.Drawing.Size(898, 140)
    Me.SetupGroupBox.TabIndex = 7
    Me.SetupGroupBox.TabStop = False
    Me.SetupGroupBox.Text = "Source and Destination directories, and options"
    '
    'DestinationDirectory
    '
    Me.DestinationDirectory.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
    Me.DestinationDirectory.FormattingEnabled = True
    Me.DestinationDirectory.ListLength = 8
    Me.DestinationDirectory.Location = New System.Drawing.Point(230, 66)
    Me.DestinationDirectory.Name = "DestinationDirectory"
    Me.DestinationDirectory.SettingsItemList = CType(resources.GetObject("DestinationDirectory.SettingsItemList"), System.Collections.Specialized.StringCollection)
    Me.DestinationDirectory.ShowOnlyFileName = False
    Me.DestinationDirectory.Size = New System.Drawing.Size(533, 21)
    Me.DestinationDirectory.TabIndex = 1
    '
    'SourceDirectory
    '
    Me.SourceDirectory.FormattingEnabled = True
    Me.SourceDirectory.ListLength = 8
    Me.SourceDirectory.Location = New System.Drawing.Point(230, 23)
    Me.SourceDirectory.Name = "SourceDirectory"
    Me.SourceDirectory.SettingsItemList = CType(resources.GetObject("SourceDirectory.SettingsItemList"), System.Collections.Specialized.StringCollection)
    Me.SourceDirectory.ShowOnlyFileName = False
    Me.SourceDirectory.Size = New System.Drawing.Size(533, 21)
    Me.SourceDirectory.TabIndex = 0
    '
    'OnlyCommon
    '
    Me.OnlyCommon.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
    Me.OnlyCommon.Location = New System.Drawing.Point(667, 106)
    Me.OnlyCommon.Name = "OnlyCommon"
    Me.OnlyCommon.Size = New System.Drawing.Size(187, 24)
    Me.OnlyCommon.TabIndex = 6
    Me.OnlyCommon.Text = "Only Common Files"
    Me.OnlyCommon.UseVisualStyleBackColor = True
    '
    'IncludeHidden
    '
    Me.IncludeHidden.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
    Me.IncludeHidden.Location = New System.Drawing.Point(474, 106)
    Me.IncludeHidden.Name = "IncludeHidden"
    Me.IncludeHidden.Size = New System.Drawing.Size(187, 24)
    Me.IncludeHidden.TabIndex = 6
    Me.IncludeHidden.Text = "Include Hidden Files"
    Me.IncludeHidden.UseVisualStyleBackColor = True
    '
    'IncludeSystem
    '
    Me.IncludeSystem.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
    Me.IncludeSystem.Location = New System.Drawing.Point(272, 106)
    Me.IncludeSystem.Name = "IncludeSystem"
    Me.IncludeSystem.Size = New System.Drawing.Size(194, 24)
    Me.IncludeSystem.TabIndex = 5
    Me.IncludeSystem.Text = "Include System files"
    Me.IncludeSystem.UseVisualStyleBackColor = True
    '
    'IncludeSubDirectories
    '
    Me.IncludeSubDirectories.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
    Me.IncludeSubDirectories.Location = New System.Drawing.Point(60, 106)
    Me.IncludeSubDirectories.Name = "IncludeSubDirectories"
    Me.IncludeSubDirectories.Size = New System.Drawing.Size(202, 24)
    Me.IncludeSubDirectories.TabIndex = 4
    Me.IncludeSubDirectories.Text = "Include subdirectories"
    Me.IncludeSubDirectories.UseVisualStyleBackColor = True
    '
    'ExecuteScript
    '
    Me.ExecuteScript.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
    Me.ExecuteScript.Location = New System.Drawing.Point(101, 59)
    Me.ExecuteScript.Margin = New System.Windows.Forms.Padding(4)
    Me.ExecuteScript.Name = "ExecuteScript"
    Me.ExecuteScript.Size = New System.Drawing.Size(85, 30)
    Me.ExecuteScript.TabIndex = 7
    Me.ExecuteScript.Text = "Run"
    Me.ExecuteScript.UseVisualStyleBackColor = True
    '
    'CancelExitButton
    '
    Me.CancelExitButton.DialogResult = System.Windows.Forms.DialogResult.Cancel
    Me.CancelExitButton.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
    Me.CancelExitButton.IsCancel = False
    Me.CancelExitButton.IsExit = True
    Me.CancelExitButton.Location = New System.Drawing.Point(753, 158)
    Me.CancelExitButton.Name = "CancelExitButton"
    Me.CancelExitButton.Size = New System.Drawing.Size(131, 33)
    Me.CancelExitButton.TabIndex = 9
    Me.CancelExitButton.Text = "Cancel/Exit"
    Me.CancelExitButton.UseVisualStyleBackColor = True
    Me.CancelExitButton.WasCanceled = False
    '
    'FileProgressBar
    '
    Me.FileProgressBar.ColorBackGround = System.Drawing.Color.White
    Me.FileProgressBar.ColorBarBorder = System.Drawing.Color.FromArgb(CType(CType(170, Byte), Integer), CType(CType(240, Byte), Integer), CType(CType(170, Byte), Integer))
    Me.FileProgressBar.ColorBarCenter = System.Drawing.Color.FromArgb(CType(CType(10, Byte), Integer), CType(CType(150, Byte), Integer), CType(CType(10, Byte), Integer))
    Me.FileProgressBar.ColorText = System.Drawing.Color.Black
    Me.FileProgressBar.Dock = System.Windows.Forms.DockStyle.Bottom
    Me.FileProgressBar.Location = New System.Drawing.Point(0, 331)
    Me.FileProgressBar.Name = "FileProgressBar"
    Me.FileProgressBar.Position = 50
    Me.FileProgressBar.PositionMax = 100
    Me.FileProgressBar.PositionMin = 0
    Me.FileProgressBar.Size = New System.Drawing.Size(898, 28)
    Me.FileProgressBar.SteepDistance = CType(0, Byte)
    Me.FileProgressBar.SteepWidth = CType(1, Byte)
    Me.FileProgressBar.TabIndex = 10
    Me.FileProgressBar.Text = "XpProgressBar1"
    Me.FileProgressBar.TextShadow = False
    '
    'DirectoryProgressBar
    '
    Me.DirectoryProgressBar.ColorBackGround = System.Drawing.Color.White
    Me.DirectoryProgressBar.ColorBarBorder = System.Drawing.Color.FromArgb(CType(CType(170, Byte), Integer), CType(CType(240, Byte), Integer), CType(CType(170, Byte), Integer))
    Me.DirectoryProgressBar.ColorBarCenter = System.Drawing.Color.FromArgb(CType(CType(10, Byte), Integer), CType(CType(150, Byte), Integer), CType(CType(10, Byte), Integer))
    Me.DirectoryProgressBar.ColorText = System.Drawing.Color.Black
    Me.DirectoryProgressBar.Dock = System.Windows.Forms.DockStyle.Bottom
    Me.DirectoryProgressBar.Location = New System.Drawing.Point(0, 303)
    Me.DirectoryProgressBar.Name = "DirectoryProgressBar"
    Me.DirectoryProgressBar.Position = 50
    Me.DirectoryProgressBar.PositionMax = 100
    Me.DirectoryProgressBar.PositionMin = 0
    Me.DirectoryProgressBar.Size = New System.Drawing.Size(898, 28)
    Me.DirectoryProgressBar.SteepDistance = CType(0, Byte)
    Me.DirectoryProgressBar.SteepWidth = CType(1, Byte)
    Me.DirectoryProgressBar.TabIndex = 10
    Me.DirectoryProgressBar.Text = "XpProgressBar2"
    Me.DirectoryProgressBar.TextShadow = False
    '
    'CommentLabel
    '
    Me.CommentLabel.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
    Me.CommentLabel.Location = New System.Drawing.Point(5, 269)
    Me.CommentLabel.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
    Me.CommentLabel.Name = "CommentLabel"
    Me.CommentLabel.Size = New System.Drawing.Size(568, 27)
    Me.CommentLabel.TabIndex = 11
    Me.CommentLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
    '
    'RunAsAdmin
    '
    Me.RunAsAdmin.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
    Me.RunAsAdmin.Location = New System.Drawing.Point(184, 219)
    Me.RunAsAdmin.Margin = New System.Windows.Forms.Padding(4)
    Me.RunAsAdmin.Name = "RunAsAdmin"
    Me.RunAsAdmin.Size = New System.Drawing.Size(231, 33)
    Me.RunAsAdmin.TabIndex = 8
    Me.RunAsAdmin.Text = "Run As Administrator"
    Me.RunAsAdmin.UseVisualStyleBackColor = True
    '
    'OWMfiles
    '
    Me.OWMfiles.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
    Me.OWMfiles.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
    Me.OWMfiles.FormattingEnabled = True
    Me.OWMfiles.ListLength = 8
    Me.OWMfiles.Location = New System.Drawing.Point(11, 23)
    Me.OWMfiles.Name = "OWMfiles"
    Me.OWMfiles.SettingsItemList = CType(resources.GetObject("OWMfiles.SettingsItemList"), System.Collections.Specialized.StringCollection)
    Me.OWMfiles.ShowOnlyFileName = True
    Me.OWMfiles.Size = New System.Drawing.Size(263, 21)
    Me.OWMfiles.TabIndex = 12
    '
    'OWMfileGroup
    '
    Me.OWMfileGroup.Controls.Add(Me.LocateScript)
    Me.OWMfileGroup.Controls.Add(Me.SaveScript)
    Me.OWMfileGroup.Controls.Add(Me.ExecuteScript)
    Me.OWMfileGroup.Controls.Add(Me.OWMfiles)
    Me.OWMfileGroup.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
    Me.OWMfileGroup.Location = New System.Drawing.Point(600, 198)
    Me.OWMfileGroup.Name = "OWMfileGroup"
    Me.OWMfileGroup.Size = New System.Drawing.Size(284, 101)
    Me.OWMfileGroup.TabIndex = 13
    Me.OWMfileGroup.TabStop = False
    Me.OWMfileGroup.Text = "One Way Mirror Scripts"
    '
    'LocateScript
    '
    Me.LocateScript.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
    Me.LocateScript.Location = New System.Drawing.Point(8, 59)
    Me.LocateScript.Margin = New System.Windows.Forms.Padding(4)
    Me.LocateScript.Name = "LocateScript"
    Me.LocateScript.Size = New System.Drawing.Size(85, 30)
    Me.LocateScript.TabIndex = 7
    Me.LocateScript.Text = "Load..."
    Me.LocateScript.UseVisualStyleBackColor = True
    '
    'SaveScript
    '
    Me.SaveScript.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
    Me.SaveScript.Location = New System.Drawing.Point(194, 59)
    Me.SaveScript.Margin = New System.Windows.Forms.Padding(4)
    Me.SaveScript.Name = "SaveScript"
    Me.SaveScript.Size = New System.Drawing.Size(85, 30)
    Me.SaveScript.TabIndex = 7
    Me.SaveScript.Text = "Save..."
    Me.SaveScript.UseVisualStyleBackColor = True
    '
    'OneWayMirror
    '
    Me.AcceptButton = Me.StartCopy
    Me.AutoScaleDimensions = New System.Drawing.SizeF(9.0!, 16.0!)
    Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
    Me.CancelButton = Me.CancelExitButton
    Me.ClientSize = New System.Drawing.Size(898, 359)
    Me.Controls.Add(Me.OWMfileGroup)
    Me.Controls.Add(Me.CommentLabel)
    Me.Controls.Add(Me.DirectoryProgressBar)
    Me.Controls.Add(Me.FileProgressBar)
    Me.Controls.Add(Me.CancelExitButton)
    Me.Controls.Add(Me.SetupGroupBox)
    Me.Controls.Add(Me.RunAsAdmin)
    Me.Controls.Add(Me.StartCopy)
    Me.Controls.Add(Me.InstructionsLabel)
    Me.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
    Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
    Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
    Me.Margin = New System.Windows.Forms.Padding(4)
    Me.MaximizeBox = False
    Me.Name = "OneWayMirror"
    Me.Text = "One Way Mirror"
    Me.SetupGroupBox.ResumeLayout(False)
    Me.OWMfileGroup.ResumeLayout(False)
    Me.ResumeLayout(False)

  End Sub
  Friend WithEvents SourceDirectoryLabel As System.Windows.Forms.Label
  Friend WithEvents DestinationDirectoryLabel As System.Windows.Forms.Label
  Friend WithEvents LocateSourceDirectory As System.Windows.Forms.Button
  Friend WithEvents LocateDestinationDirectory As System.Windows.Forms.Button
  Friend WithEvents InstructionsLabel As System.Windows.Forms.Label
  Friend WithEvents StartCopy As System.Windows.Forms.Button
  Friend WithEvents SetupGroupBox As System.Windows.Forms.GroupBox
  Friend WithEvents ExecuteScript As System.Windows.Forms.Button
  Friend WithEvents IncludeSubDirectories As System.Windows.Forms.CheckBox
  Friend WithEvents CancelExitButton As UIelements.CancelExitButton
  Friend WithEvents IncludeHidden As System.Windows.Forms.CheckBox
  Friend WithEvents IncludeSystem As System.Windows.Forms.CheckBox
  Friend WithEvents FileProgressBar As XpControls.XpProgressBar
  Friend WithEvents DirectoryProgressBar As XpControls.XpProgressBar
  Friend WithEvents SourceDirectory As UIelements.ComboBoxWithMemory
  Friend WithEvents DestinationDirectory As UIelements.ComboBoxWithMemory
  Friend WithEvents CommentLabel As System.Windows.Forms.Label
  Friend WithEvents OnlyCommon As System.Windows.Forms.CheckBox
  Friend WithEvents RunAsAdmin As System.Windows.Forms.Button
  Friend WithEvents OWMfiles As UIelements.ComboBoxWithMemory
  Friend WithEvents ToolTip1 As System.Windows.Forms.ToolTip
  Friend WithEvents OWMfileGroup As System.Windows.Forms.GroupBox
  Friend WithEvents LocateScript As System.Windows.Forms.Button
  Friend WithEvents SaveScript As System.Windows.Forms.Button

End Class
