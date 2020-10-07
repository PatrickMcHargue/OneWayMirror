'
'Release 1.0.2012.0904
'Recoded for .Net 2.0
'
'Release 1.0.2012.0818
'Removed a dialog messaging the user about the inability to read protected directories.
'Added Administrator rights elevation to the application start.
'
'
'Initial release 1.0.2012.0817
'
Imports System
Imports System.IO
Imports System.ComponentModel
Imports System.Reflection
Imports System.Security.Principal

Public Class OneWayMirror

#Region "Form-level Variables"

  ''' <summary>
  ''' This timer is used to allow the form to show before the command line parameter
  ''' is processed via the timers 'tick'.
  ''' </summary>
  ''' <remarks></remarks>
  Private WithEvents CommandTimer As New System.Windows.Forms.Timer()

  'Make a BackgroundWorker to work for us behind the UI.
  Dim WithEvents myWorkerThread As New BackgroundWorker With {.WorkerReportsProgress = True, .WorkerSupportsCancellation = True}

  'Use events if True, else use the 'myCopyInformation' in the 'ProgressChanged' event if False.
  Dim sharedCopyInformation As New CopyInformation 'Setup the object shared between the UI and the worker thread.
  Dim sharedScannedDirectoryCount, sharedScannedFileCount, sharedCopiedFileCount As Integer 'Statistics.

  Private isAdministrator As Boolean = False, toldAboutAdministratorRights As Boolean = False

  'A list of OWM commands buffered for execution.
  Private queueCommandList As New System.Collections.Queue()

#End Region 'Form-level Variables

#Region "Form events"

  Private Sub OneWayMirror_Load(sender As Object, e As System.EventArgs) Handles Me.Load

 
    Try
      isAdministrator = VerifyAdminPrivilege()
      toldAboutAdministratorRights = isAdministrator
      '
      CommandTimer.Tag = False 'Note that we are not processing from the queue.
      updateUI(sharedCopyInformation)
      Call LabelTheForm()
      If (Not (My.Settings.WasUpgraded)) Then 'If the settings file is new, 
        My.Settings.Upgrade() 'get the settings from the previous install,
        My.Settings.WasUpgraded = True 'and indicate that the upgrade was completed.
        My.Settings.Save() 'Save the upgraded settings now.
      End If
      Me.SourceDirectory.SettingsItemList = My.Settings.LastSourceList
      Me.DestinationDirectory.SettingsItemList = My.Settings.LastDestinationList
      '
      IncludeSubDirectories.Checked = My.Settings.LastIncludeSubdirectories '
      IncludeSystem.Checked = My.Settings.LastIncludeSystem                 '
      IncludeHidden.Checked = My.Settings.LastIncludeHidden                 '
      '
      Dim commandString As String = "" 'Default this to an empty string to show no command line parameters on program start.
      Try 'Now we try to get the command line parameters into the command string bereft of quotation marks, and white-space.
        If (Environment.GetCommandLineArgs.Length > 1) Then
          commandString = Environment.GetCommandLineArgs(1).Replace(ControlChars.Quote, "").Trim()
        End If
      Catch ex As Exception
        commandString = "" 'If we fail, then start like we had no parameters.
      End Try
      If (Not (String.IsNullOrEmpty(commandString))) Then 'If we did get some command line parameters,
        If (commandString.ToUpper.EndsWith(textDotOWMext.ToUpper)) Then 'If the comand line parameter is a valid '.OWM' file, 
          QueueOwmFileCommands(queueCommandList, commandString) 'we process the file into the CommandListQueue.
        Else 'Otherwise, we queue up the line into the CommandListQueue directly.
          queueCommandList.Enqueue(commandString)
        End If
        'Here, if we have anything in the CommandListQueue, we set the timer to execute what backup commands it has.
        EnableQueueCommandListProcessing()
      End If
    Catch ex As Exception
      'MsgBox("Opps! " & ex.Message, MsgBoxStyle.Information Or MsgBoxStyle.Critical Or MsgBoxStyle.OkOnly, Me.Text)
    End Try

  End Sub
  Private Sub OneWayMirror_FormClosing(sender As Object, eventArgs As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
    Try
      If (CancelExitButton.IsCancel) Then 'If we're showing a 'Canel', 
        Call CancelExitButton_Click(Nothing, New EventArgs) 'then we do this to stop the thread.
      End If
      Call CancelExitButton_Click(Nothing, New EventArgs) 'Do this to cleanup.

    Catch ex As Exception
      MsgBox("Opps! " & ex.Message, MsgBoxStyle.Information Or MsgBoxStyle.Critical Or MsgBoxStyle.OkOnly, Me.Text)
    End Try
  End Sub

#End Region 'Form events

#Region "Form control event routines"

  Private Sub CommandTimer_Tick(sender As Object, e As System.EventArgs) Handles CommandTimer.Tick
    '
    'Now that the form is visible, we can parse the 'Command' we started with.
    Try
      CommandTimer.Stop() 'Stop the timer from firing again.
      '
      'If we have an item in the queue for processing, we do that now.
      If (queueCommandList.Count > 0) Then
        CommandTimer.Tag = True 'Note that we are processing from the queue, and that when done we come back here to end.
        If (ParseCommandLineOntoForm(queueCommandList.Dequeue.ToString)) Then ExecuteBackup() 'If we can parse the CommandLine, we start the copy process.

      Else 'Otherwise, there are no more items to process,
        Me.Text = ("Finished! " & Me.Text)  'note that we're done,
        Me.Refresh()
        Me.Enabled = False
        System.Threading.Thread.Sleep(1000)
        CancelExitButton_Click(Nothing, New System.EventArgs) 'and we're done. 
      End If

    Catch ex As Exception
      MsgBox("Opps! " & ex.Message, MsgBoxStyle.Information Or MsgBoxStyle.Critical Or MsgBoxStyle.OkOnly, Me.Text)
    End Try

  End Sub

  Private Sub LocateDirectory_Click(sender As System.Object, e As System.EventArgs) Handles LocateSourceDirectory.Click, LocateDestinationDirectory.Click
    '
    'Alow the user to select a directory for the source, and destination root directories.
    Try
      Dim directorySelector As New Windows.Forms.FolderBrowserDialog
      Dim currentPath As String = My.Application.Info.DirectoryPath, currentMessage As String = "Select directory for "
      Select Case (True) 'One thing for source, another for destination.
        Case sender Is LocateSourceDirectory
          currentMessage = "Source"
          currentPath = SourceDirectory.Text
        Case sender Is LocateDestinationDirectory
          currentMessage = "Destination"
          currentPath = DestinationDirectory.Text
      End Select
      currentPath = GetValidDirectory(currentPath)

      'Show the selector, and set the default path to the current path.
      directorySelector.Description = currentMessage
      directorySelector.ShowNewFolderButton = False
      If (IO.Directory.Exists(currentPath)) Then
        directorySelector.SelectedPath = currentPath
      End If
      If (directorySelector.ShowDialog() <> Windows.Forms.DialogResult.Cancel) Then 'If we didn't cancel,
        currentPath = directorySelector.SelectedPath 'get the path set
        If ((currentPath.Length > 0) AndAlso Directory.Exists(currentPath)) Then 'and if it's valid,
          Select Case (True) 'One thing for source, another for destination.
            Case sender Is LocateSourceDirectory
              SourceDirectory.Text = currentPath
              SourceDirectory.UpdateListOrder()
            Case sender Is LocateDestinationDirectory
              DestinationDirectory.Text = currentPath
              DestinationDirectory.UpdateListOrder()
          End Select
        End If
      End If

    Catch ex As Exception
      MsgBox("Opps! " & ex.Message, MsgBoxStyle.Critical Or MsgBoxStyle.Information Or MsgBoxStyle.OkOnly, Me.Text)
    End Try

  End Sub
  Private Sub StartCopy_Click(sender As System.Object, e As System.EventArgs) Handles StartCopy.Click
    '
    'Start the copy process.
    Try
      'Record the directories set into our settings.
      My.Settings.LastIncludeSubdirectories = IncludeSubDirectories.Checked
      My.Settings.LastIncludeSystem = IncludeSystem.Checked
      My.Settings.LastIncludeHidden = IncludeHidden.Checked
      My.Settings.Save() 'Save the settings!

      'Clear the statistics.
      sharedScannedDirectoryCount = 0
      sharedScannedFileCount = 0
      sharedCopiedFileCount = 0

      'Now set the data together as a command line, and call the routine to test and parse it.
      Dim commandLine As String = (SourceDirectory.Text & textComma & DestinationDirectory.Text)
      If (IncludeSubDirectories.Checked) Then commandLine &= (textComma & "Recurse") Else commandLine &= (textComma & "NoRecurse")
      If (IncludeSystem.Checked) Then commandLine &= (textComma & "System") Else commandLine &= (textComma & "NoSystem")
      If (IncludeHidden.Checked) Then commandLine &= (textComma & "Hidden") Else commandLine &= (textComma & "NoHidden")
      If (OnlyCommon.Checked) Then commandLine &= (textComma & "OnlyCommon") Else commandLine &= (textComma & "AllFiles")
      If (ParseCommandLineOntoForm(commandLine)) Then ExecuteBackup() 'If we can parse the CommandLine, we start the copy process.

    Catch ex As Exception
      MsgBox("Opps! " & ex.Message, MsgBoxStyle.Critical Or MsgBoxStyle.Information Or MsgBoxStyle.OkOnly, Me.Text)
    Finally
      Call updateUI(sharedCopyInformation) 'Set the button as 'Exit'
    End Try

  End Sub
  Private Sub RunBackupCopyFile_Click(sender As System.Object, e As System.EventArgs) Handles RunBackupCopyFile.Click
    Try
      Call QueueOwmFileCommands(queueCommandList) 'Call the routine to start the copy process.
    Catch ex As Exception
      MsgBox("Opps! " & ex.Message, MsgBoxStyle.Information Or MsgBoxStyle.Critical Or MsgBoxStyle.OkOnly, Me.Text)
    End Try
  End Sub
  Private Sub CancelExitButton_Click(sender As System.Object, e As System.EventArgs) Handles CancelExitButton.Click
    Try
      'If the user pushes this button...
      If (CancelExitButton.IsCancel) Then 'If we're 'Cancel' now, then ask the user if they truly wish to cancel...
        'We can canel if there's an active background task, or if we have items queued up for later execution.
        If ((myWorkerThread.IsBusy AndAlso Not (myWorkerThread.CancellationPending)) OrElse (queueCommandList.Count > 0) OrElse CBool(CommandTimer.Tag)) Then
          CancelExitButton.WasCanceled = (MsgBox("Do you want to stop the copy?", MsgBoxStyle.Question Or MsgBoxStyle.YesNo Or MsgBoxStyle.DefaultButton2, Me.Text) = MsgBoxResult.Yes)
          'Stop all queued items from being processed.
          queueCommandList.Clear()
          CommandTimer.Tag = False

          'If we have a background task active, we can kill it.
          If (myWorkerThread.IsBusy AndAlso Not (myWorkerThread.CancellationPending)) Then
            If (CancelExitButton.WasCanceled) Then
              myWorkerThread.CancelAsync()
            End If
          End If
        End If
      Else 'Otherwise, we're 'Exit' now,
        My.Settings.LastSourceList = Me.SourceDirectory.SettingsItemList
        My.Settings.LastDestinationList = Me.DestinationDirectory.SettingsItemList
        My.Settings.Save()
        myWorkerThread = Nothing
        Try
          End
        Catch ex As Exception
        End Try
        End 'so we're done.
      End If
    Catch ex As Exception
      MsgBox("Opps! " & ex.Message, MsgBoxStyle.Information Or MsgBoxStyle.Critical Or MsgBoxStyle.OkOnly, Me.Text)
    End Try
  End Sub

#End Region 'Form control event routines

#Region "Helper routines"

  ''' <summary>
  ''' This routine sets the CancelExit button to act either as a Cancel button, or as an Exit button. 
  ''' </summary>
  ''' <param name="SetAsCancel">If True, set Cancel button, else set Exit button.</param>
  ''' <remarks>This also corrects the enable for other of the form's controls.</remarks>
  Private Sub ConfigureCancelExit(ByVal SetAsCancel As Boolean)
    With CancelExitButton
      .IsCancel = SetAsCancel                       'Note the button we are: 'Cancel' or 'Exit'.
      Me.SetupGroupBox.Enabled = Not (SetAsCancel)     'Initialize these to match so that they
      Me.RunBackupCopyFile.Enabled = Not (SetAsCancel) 'are disabled while we're 'Cancel'.
      Me.StartCopy.Enabled = Not (SetAsCancel)         '
      Me.RunAsAdmin.Enabled = Not (isAdministrator OrElse SetAsCancel) 'This only works when we're not Cancel, and we're not already Admin.
    End With
  End Sub

  ''' <summary> 
  ''' This routine will parse the CommandLine input argument into the Source and Destination directories, and determine
  ''' what options are set.  If this can be done without error, then the form will be setup according to the CommandLine.
  ''' </summary>
  ''' <param name="commandLine">The line that contains the comma-delimited Source and Destination directories, as
  ''' well as the other option parameters.</param>
  ''' <returns>True if the CommandLine can be parsed into the 'SourceDirectory', 'DestinationDirectory', and options
  ''' flag.  Otherwise False, with the 'ErrorMessage' set to the reason.</returns>
  ''' <remarks></remarks>
  Private Function ParseCommandLineOntoForm(ByVal commandLine As String) As Boolean

    Dim errorMessage As String = "" 'No error!
    Try
      'If we have something in the commandLine, we process it.
      If (Not (String.IsNullOrEmpty(commandLine))) Then
        sharedCopyInformation.Initialize()
        updateUI(sharedCopyInformation)

        Dim lineComment As String = "" 'No comment!
        If (commandLine.Contains(textCommentDelimiter)) Then 'If we have a comment on the line,
          lineComment = commandLine.Substring(commandLine.IndexOf(textCommentDelimiter) + 2) 'get the comment,
          Me.CommentLabel.Text = lineComment 'place it on the form,
          commandLine = commandLine.Substring(0, commandLine.IndexOf(textCommentDelimiter)) 'and strip of the comment.
        End If
        commandLine = commandLine.Trim 'Now trim the commandLine,
        If (commandLine.Length = 0) Then Return False 'and if we have nothing, return False.
        '
        Dim commandArguments() As String = commandLine.Split(CChar(textComma)) 'Split the command line into 'Source', 'Destination', [and optional 'Recurse/Norecurse']
        Dim tmpString As String = ""
        '
        Dim tmpSourceDirectory As String = "" 'Set no Source directory to start with.
        If (commandArguments.Length >= 1) Then 'If we have a command line argument for 'source', 
          tmpString = commandArguments(0).Replace(ControlChars.Quote, "").Trim() 'strip characters we don't need,
          If (DirectoryDoesExist(tmpString)) Then tmpSourceDirectory = tmpString 'Set it as the Source if it exists.
        End If
        If (String.IsNullOrEmpty(tmpSourceDirectory)) Then errorMessage &= ("Source unrecognized:" & vbCrLf & GetTextInTicks(tmpString) & vbCrLf & vbCrLf) 'If nothing, report the error.
        '
        Dim tmpDestinationDirectory As String = "" 'Set no Destination directory to start with.
        If (commandArguments.Length >= 2) Then 'If we have a command line argument for 'destination', 
          tmpString = commandArguments(1).Replace(ControlChars.Quote, "").Trim() 'strip characters we don't need,
          If (DirectoryDoesExist(tmpString)) Then tmpDestinationDirectory = tmpString 'Set it as the Destination if it exists.
        End If
        If (String.IsNullOrEmpty(tmpDestinationDirectory)) Then errorMessage &= ("Destination unrecognized:" & vbCrLf & GetTextInTicks(tmpString) & vbCrLf & vbCrLf) 'If nothing, report the error.
        '
        If (commandArguments.Length >= 3) Then 'Next, we pull of all the comma-delimted switches to set the internal flags.
          With sharedCopyInformation
            For switchIndex As Integer = 2 To (commandArguments.Length - 1)
              tmpString = commandArguments(switchIndex).Replace(ControlChars.Quote, "").ToUpper.Trim() 'strip characters we don't need, and set UPPERCASE
              Select Case (tmpString) 'Now, based on the switch, set the associated flag.
                Case "RECURSE", "INCLUDE", "INCLUDEDIRECTORIES", "INCLUDESUBDIRECTORIES", "INCLUDEFOLDERS" : .SubDirectories = True
                Case "SYSTEM", "DOSYSTEM", "INCLUDESYSTEM" : .System = True
                Case "HIDDEN", "DOHIDDEN", "INCLUDEHIDDEN" : .Hidden = True
                Case "COMMON", "ONLYCOMMON", "DOCOMMON" : .OnlyCommon = True
                Case "NORECURSE", "EXCLUDE", "EXCLUDEDIRECTORIES", "EXCLUDESUBDIRECTORIES", "EXCLUDEFOLDERS" : .SubDirectories = False
                Case "NOSYSTEM", "EXCLUDESYSTEM" : .System = False
                Case "NOHIDDEN", "EXCLUDEHIDDEN" : .Hidden = False
                Case "NotCommon", "ALLFILES" : .OnlyCommon = False
                Case Else : errorMessage &= ("Option unrecognized: " & GetTextInTicks(tmpString) & vbCrLf) : Exit For 'If nothing, or not recognized, report the error.
              End Select
            Next switchIndex
          End With
        End If

        'Now we populate the form with information from the command line.
        With sharedCopyInformation
          Me.IncludeSubDirectories.Checked = .SubDirectories : Me.IncludeSubDirectories.Refresh()
          Me.IncludeSystem.Checked = .System : Me.IncludeSystem.Refresh()
          Me.IncludeHidden.Checked = .Hidden : Me.IncludeHidden.Refresh()
          Me.OnlyCommon.Checked = .OnlyCommon : Me.OnlyCommon.Refresh()
          '
          .SourceRoot = New DirectoryInfo(tmpSourceDirectory)
          Me.SourceDirectory.Text = .SourceRoot.FullName : Me.SourceDirectory.Refresh()
          Me.SourceDirectory.UpdateListOrder()
          '
          .DestinationRoot = New DirectoryInfo(tmpDestinationDirectory)
          Me.DestinationDirectory.Text = .DestinationRoot.FullName : Me.DestinationDirectory.Refresh()
          Me.DestinationDirectory.UpdateListOrder()
        End With

      Else 'Otherwise, nothing in the commandLine, so we error.
        errorMessage = "Empty command line"
      End If

    Catch ex As Exception
      errorMessage = ("Bad command! " & GetTextInTicks(commandLine) & vbCrLf & vbCrLf & ex.Message)
    Finally
      updateUI(sharedCopyInformation)
    End Try

    If (Not (String.IsNullOrEmpty(errorMessage))) Then 'If there was an error, we display that.
      errorMessage &= ("Input line:" & vbCrLf & GetTextInTicks(commandLine)) 'If there's an error, add the CommandLine to the explanation.
      MsgBox(errorMessage, MsgBoxStyle.Critical Or MsgBoxStyle.Information Or MsgBoxStyle.OkOnly, Me.Text)
    End If
    Return String.IsNullOrEmpty(errorMessage) 'If we have no error message, it worked!

  End Function
  ''' <summary>
  ''' This routine wil initiate a backup using the data that's on the form.
  ''' </summary>
  Private Sub ExecuteBackup()

    Try
      With sharedCopyInformation
        Dim messageString As String = ""
        If (Not (CorrectDirectoryAttributes(sharedCopyInformation, .SourceRoot, True))) Then 'Correct? (it can be read-only)
          messageString &= (vbCrLf & "The Source directory does not exist, is marked 'System', 'Hidden', 'Offline', 'Temporary', or is a link:" & vbCrLf & "   " & GetTextInTicks(.SourceRoot.FullName) & vbCrLf)
        End If
        If (Not (CorrectDirectoryAttributes(sharedCopyInformation, .DestinationRoot, False))) Then 'Correct? (it can NOT be read-only)
          messageString &= (vbCrLf & "The Destination directory does not exist, is marked 'System', 'Hidden', 'Offline', 'Temporary', 'Read-Only', or is a link:" & vbCrLf & "   " & GetTextInTicks(.DestinationRoot.FullName) & vbCrLf)
        End If
        If (Not (String.IsNullOrEmpty(messageString))) Then
          messageString = ("Error with directories!" & vbCrLf & vbCrLf & messageString)
          MsgBox(messageString, MsgBoxStyle.Exclamation Or MsgBoxStyle.Critical Or MsgBoxStyle.OkOnly, Me.Text)

        Else 'Otherwise, we have good root directories, so we can start by getting a count of directories in/under the source directory.
          'See :http://msdn.microsoft.com/en-us/library/system.threading.threadpool.queueuserworkitem(v=vs.71).aspx
          myWorkerThread.RunWorkerAsync(sharedCopyInformation)
        End If
      End With

    Catch ex As Exception
      MsgBox("Opps! " & ex.Message, MsgBoxStyle.Information Or MsgBoxStyle.Critical Or MsgBoxStyle.OkOnly, Me.Text)
    End Try

  End Sub
  ''' <summary>
  ''' This function will take an arbitrary string, and determine if it represents a valid file.
  ''' </summary>
  ''' <param name="testFilePathName">The directory path to test.</param>
  ''' <returns>True if valid, else False.</returns>
  ''' <remarks></remarks>
  Private Function FileDoesExist(testFilePathName As String) As Boolean
    Return (Not (String.IsNullOrEmpty(testFilePathName)) AndAlso File.Exists(testFilePathName))
  End Function
  ''' <summary>
  ''' This routine labels the form with it's name, and version information.
  ''' </summary>
  Private Sub LabelTheForm()
    With (My.Application.Info.Version)
      Me.Text = (textWindowName & (" (v" & .Major.ToString & "." & .MajorRevision & "." & .Build.ToString & "." & .MinorRevision.ToString("0000") & ")"))
    End With
  End Sub
  ''' <summary>
  ''' This will set the CommandTimer to fire if there are any items left in the queueCommandList.
  ''' </summary>
  Private Sub EnableQueueCommandListProcessing()
    If (CancelExitButton.WasCanceled) Then 'If we got canceled,
      queueCommandList.Clear() 'we clear the queue

    Else 'Otherwise, we continue processig queuued commands, or process the last timer command at the end of the queue.
      If ((queueCommandList.Count > 0) OrElse CBool(CommandTimer.Tag)) Then
        CommandTimer.Interval = 10 'set a short interval,
        CommandTimer.Start() 'and start the timer.  After we leave this routine, but before the timer fires, the form will become visible.
      End If
    End If
  End Sub
  ''' <summary>
  ''' This function will take an arbitrary string, and determine if it represents a valid directory.
  ''' </summary>
  ''' <param name="testDirectoryPathName">The directory path to test.</param>
  ''' <returns>True if valid, else False.</returns>
  ''' <remarks></remarks>
  Private Function DirectoryDoesExist(testDirectoryPathName As String) As Boolean
    Return (Not (String.IsNullOrEmpty(testDirectoryPathName)) AndAlso Directory.Exists(testDirectoryPathName))
  End Function
  ''' <summary>
  ''' Returns the directory passed in, if it exist, od the usder's docmuents folder on the system.
  ''' </summary>
  ''' <param name="directoryPathName">The FilePath to test.</param>
  ''' <returns>The Fileath is it exists, or the user's documents folder.</returns>
  ''' <remarks></remarks>
  Private Function GetValidDirectory(ByVal directoryPathName As String) As String
    If (Not (DirectoryDoesExist(directoryPathName))) Then
      directoryPathName = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
    End If
    Return directoryPathName
  End Function
  ''' <summary>
  ''' This routine is used to get the destination directory for a file given the sourse root, and
  ''' destination root directories.
  ''' </summary>
  ''' <param name="myCopyInformation"></param>
  ''' <param name="filePathName">The file path name of thje file to copy.</param>
  ''' <returns>A complete file path name in the destination directory.</returns>
  ''' <remarks></remarks>
  Private Function GetDestinationDirectoryPath(myCopyInformation As CopyInformation, ByVal filePathName As String) As String
    'We may do more in this routine in the future.
    Return filePathName.Replace(myCopyInformation.SourceRoot.FullName, myCopyInformation.DestinationRoot.FullName)
  End Function

  Friend Function GetDirectoryInfoFromPathName(ByVal PathName As String) As DirectoryInfo

    Try
      PathName = PathName.Trim
      If (Not (PathName.EndsWith(textSlash))) Then PathName &= textSlash
      Dim tmpDirectoryInfo As New DirectoryInfo(PathName)
      Return tmpDirectoryInfo

    Catch ex As Exception
      MsgBox("Opps! " & ex.Message, MsgBoxStyle.Information Or MsgBoxStyle.Critical Or MsgBoxStyle.OkOnly, Me.Text)
    End Try
    Return New DirectoryInfo("C:\") 'Default this if we exit from here

  End Function

  ''' <summary>
  ''' Return the text in ticks.
  ''' </summary>
  ''' <param name="textToTick">The text to format.</param>
  ''' <returns></returns>
  ''' <remarks></remarks>
  Private Function GetTextInTicks(textToTick As String) As String
    Return (textTick & textToTick & textTick)
  End Function
#End Region 'Helper routines

#Region "Directory recusion, and File copy routines"

  ''' <summary>
  ''' This function will either locate a '.OWM' file to run as a script, or use the .OWM FilePathName
  ''' passed into it.
  ''' </summary>
  ''' <param name="useFilePathName">If set, it is the FilePathName of the '.OWM' script file to run.</param>
  ''' <remarks></remarks>
  Private Sub QueueOwmFileCommands(queueBuffer As System.Collections.Queue, Optional ByVal useFilePathName As String = "")
    '
    'Here, we locate & execute a '.owm' script file to finish all backups in the list.
    Try
      queueBuffer.Clear() 'Clear the queue we will be loading.
      'If we have no FilePathName, or it doesn't exist, we will ask for one.
      If (Not (FileDoesExist(useFilePathName))) Then
        Dim openFileDialog As New System.Windows.Forms.OpenFileDialog
        openFileDialog.Title = "Run a One Way Mirror script"
        openFileDialog.Filter = ("One Way Mirror script | " & textStarDotOWMext)
        '
        useFilePathName = My.Settings.LastScriptFilePathName 'Use this, as the other didn't work.
        If (Not (String.IsNullOrEmpty(useFilePathName))) Then 'If we have a FilePathName,
          openFileDialog.InitialDirectory = Path.GetDirectoryName(useFilePathName) 'use its FilePath, and the name if it exists.
          If (File.Exists(useFilePathName)) Then openFileDialog.FileName = Path.GetFileName(useFilePathName)
        End If
        'Open the dialog to locate the file we need.
        If ((openFileDialog.ShowDialog() = DialogResult.OK) AndAlso File.Exists(openFileDialog.FileName)) Then
          useFilePathName = openFileDialog.FileName 'Did we locate a valid file?
        Else
          useFilePathName = "" 'Nope!
        End If
      End If

      'If we have a valid Owm FilePathName, we process it into the queueBuffer.
      If (FileDoesExist(useFilePathName)) Then
        My.Settings.LastScriptFilePathName = useFilePathName
        Dim theOwnFileName As String = Path.GetFileName(useFilePathName)
        Me.Text = ("Processing: " & theOwnFileName)
        '
        Dim readLine As String, queueReverseOrder As New System.Collections.Queue
        Dim backupFileStream As New FileStream(useFilePathName, FileMode.Open, FileAccess.Read, FileShare.Read) 'Open a shared read file stream, (to not lock the file)
        If (backupFileStream.CanRead) Then 'If it can be read,
          Dim readBackupsFile As New StreamReader(backupFileStream) 'open a stream reader to the file stream.
          While Not (readBackupsFile.EndOfStream) 'While the stream still has data to read, 
            readLine = readBackupsFile.ReadLine
            If (Not (String.IsNullOrEmpty(readLine))) Then queueBuffer.Enqueue(readLine)
          End While
          readBackupsFile.Close()
          readBackupsFile.Dispose()
        End If
        backupFileStream.Close()
        backupFileStream.Dispose()
      End If

    Catch ex As Exception
      MsgBox("Bad File or Directory", MsgBoxStyle.Information Or MsgBoxStyle.OkOnly, Me.Text)
    End Try

  End Sub

  ''' <summary>
  ''' This function is use to recurse through the Source Directories.  As it does this, it may either copy
  ''' files from the Source Directory to the DestinationDirectory, or it may simple count the number of
  ''' Source Directories.
  ''' </summary>
  ''' <param name="myCopyInformation"></param>
  ''' <param name="currentDirectory">The current Directory to start recursing from.</param>
  ''' <param name="directoryCount">When set non-zero, this function is being used to get a count of Source
  ''' Directories.  Wehn it returns, this value will be the count of Source Directories.</param>
  ''' <returns></returns>
  ''' <remarks>This is used to count Source Directories in order to get a completion count.</remarks>
  Friend Function RecurseSorceDirectories(myCopyInformation As CopyInformation, ByVal currentDirectory As DirectoryInfo, ByRef directoryCount As Integer) As Boolean
    '
    'Here, if the currentDirectory is valid, we add it to our master list of directories.  After we add it, we 
    'build the list using each of its directories.
    Try
      Static copyToDirectoryPath As String, DirectoryWasReadOnly As Boolean 'For use by all levels of recusion.
      If (CorrectDirectoryAttributes(myCopyInformation, currentDirectory, True)) Then 'If this directory is valid, (it can be read-only)
        If (directoryCount <> 0) Then 'If this is non-zero, we're counting directories,
          directoryCount += 1 'so we increment the count.
          If (myWorkerThread.CancellationPending) Then Return False 'If we canceled, we're done.

        Else 'Otherwise, we're copying files in this directory, and we do that now.
          myCopyInformation.Directory_Text = currentDirectory.FullName
          sharedScannedDirectoryCount += 1 'Note that we just scanned another directory.
          myCopyInformation.Directory_Value += 1
          UpdateStatus(myCopyInformation)

          copyToDirectoryPath = GetDestinationDirectoryPath(myCopyInformation, currentDirectory.FullName) 'get the destination directory path,
          If (VerifyOrCreateDirectory(myCopyInformation, copyToDirectoryPath, True)) Then 'Destination good, or can be made so? (it can NOT be read-only)
            Dim destinationDirectoryInfo As New DirectoryInfo(copyToDirectoryPath)
            DirectoryWasReadOnly = ((destinationDirectoryInfo.Attributes And FileAttributes.ReadOnly) <> 0)
            If (DirectoryWasReadOnly) Then 'If the Destination Directory is read-only, unmark it.
              destinationDirectoryInfo.Attributes = (destinationDirectoryInfo.Attributes And Not (FileAttributes.ReadOnly))
            End If
            'If the destination directory is Read-Only, unmark it
            CopyFilesBetweenDirectories(myCopyInformation, currentDirectory) 'Copy the files.
            If (myWorkerThread.CancellationPending) Then Return False 'If we canceled, we're done.
            If (DirectoryWasReadOnly) Then 'If the Destination Directory was read-only, mark it back.
              destinationDirectoryInfo.Attributes = (destinationDirectoryInfo.Attributes Or FileAttributes.ReadOnly)
            End If
          End If
        End If
        '
        If (myCopyInformation.SubDirectories) Then 'If we're to recuse directories, we do that now.
          For Each tmpDirectoryInfo As DirectoryInfo In currentDirectory.GetDirectories 'Now, for each directory in the current one, we recurse futher.
            RecurseSorceDirectories(myCopyInformation, tmpDirectoryInfo, directoryCount)
            If (myWorkerThread.CancellationPending) Then Return False 'If we canceled, we're done.
          Next
        End If
      End If
      Return True

    Catch ex As Exception
      If (Not (toldAboutAdministratorRights)) Then
        toldAboutAdministratorRights = True
        Dim message As String = ("Error! " & ex.Message & vbCrLf & vbCrLf & "While some directories are protected, and can not be accessed, some access issues can be resolved by starting the application as an administrator.")
        MsgBox(message, MsgBoxStyle.Information Or MsgBoxStyle.Critical Or MsgBoxStyle.OkOnly, Me.Text)
      End If
    End Try
    Return False 'If we get here, that's bad.

  End Function

  ''' <summary>
  ''' This function is used to copy files from the current Source Directory to the associated Destination
  ''' Directory.
  ''' </summary>
  ''' <param name="myCopyInformation"></param>
  ''' <param name="currentDirectory">The current Directory to copy files from.</param>
  ''' <returns>True if the copies are successful, else False.</returns>
  ''' <remarks>If the user cancels, False will be rfeturnes.</remarks>
  Friend Function CopyFilesBetweenDirectories(myCopyInformation As CopyInformation, ByVal currentDirectory As DirectoryInfo) As Boolean

    Try
      Static destinationFilePathName As String 'Set this up once.
      Dim sourceFileInfo() As FileInfo = currentDirectory.GetFiles()
      myCopyInformation.File_Maximum = sourceFileInfo.Length
      myCopyInformation.File_Value = 0
      UpdateStatus(myCopyInformation)
      '
      If (sourceFileInfo.Length > 0) Then
        For Each tmpsourceFileInfo As FileInfo In sourceFileInfo 'Loop over the source files, 
          If (CorrectFileAttributes(myCopyInformation, tmpsourceFileInfo)) Then 'and if the Source file is good,
            destinationFilePathName = GetDestinationDirectoryPath(myCopyInformation, tmpsourceFileInfo.FullName) 'get the destination directory path,
            CopyTheFileBetweenDirectories(myCopyInformation, tmpsourceFileInfo, destinationFilePathName) 'and copy the Source file to it Destination.
            If (myWorkerThread.CancellationPending) Then Return False 'If we canceled, we're done.
          End If
        Next
      End If
      Return True 'Success!

    Catch ex As Exception
      If (Not (toldAboutAdministratorRights)) Then
        toldAboutAdministratorRights = True
        Dim message As String = ("Error! " & ex.Message & vbCrLf & vbCrLf & "While some directories are protected, and can not be accessed, some access issues can be resolved by starting the application as an administrator.")
        MsgBox(message, MsgBoxStyle.Information Or MsgBoxStyle.Critical Or MsgBoxStyle.OkOnly, Me.Text)
      End If
    Finally
      myCopyInformation.File_Maximum = 0
      myCopyInformation.File_Text = textFourDash
      UpdateStatus(myCopyInformation)
    End Try
    Return False 'If we get here, that's bad.

  End Function

  ''' <summary>
  ''' This routine is responsible for copying a file from the source, to the destination if the destination file is
  ''' older than the source file, 
  ''' </summary>
  ''' <param name="myCopyInformation"></param>
  ''' <param name="sourceFileInfo">Infomration on the Source file.</param>
  ''' <param name="destinationFilePathName">The complete FilePathName of the Destination file.</param>
  ''' <remarks></remarks>
  Friend Sub CopyTheFileBetweenDirectories(myCopyInformation As CopyInformation, ByRef sourceFileInfo As FileInfo, ByRef destinationFilePathName As String)

    Static FileExists, DoCopy As Boolean
    Try
      FileExists = File.Exists(destinationFilePathName) 'See if the file exists, and do the copy if it doesn't, or the destrination file is older than the source.
      DoCopy = ( _
                (Not (FileExists OrElse myCopyInformation.OnlyCommon)) OrElse _
                (FileExists AndAlso My.Computer.FileSystem.GetFileInfo(destinationFilePathName).LastWriteTimeUtc.Subtract(sourceFileInfo.LastWriteTimeUtc).Seconds < 0) _
               )
      If (DoCopy) Then 'If we're to do the copy,
        myCopyInformation.File_Text = sourceFileInfo.Name 'show what we're copying.
        If (FileExists) Then 'If the file already exists,
          Dim destinationFileInfo As New FileInfo(destinationFilePathName) 'get information about it.
          If ((destinationFileInfo.Attributes And FileAttributes.ReadOnly) <> 0) Then 'If the destination file is read-only, 
            destinationFileInfo.Attributes = (destinationFileInfo.Attributes And Not (FileAttributes.ReadOnly)) 'remove the ReadOnly attribute
            File.SetAttributes(destinationFilePathName, destinationFileInfo.Attributes) 'from the file so that we can copy over it.
          End If
        End If
        sharedCopiedFileCount += 1 'Note that we found a file to be copied.
        UpdateStatus(myCopyInformation) 'Show the user what file we're going to copy,
        File.Copy(sourceFileInfo.FullName, destinationFilePathName, True) 'and copy the Source to the Destination allowing overwrite.

      Else
        'myCopyInformation.File_Text = textFourDash 'No file copied.
      End If
      sharedScannedFileCount += 1 'Note that we scanned another file.
      myCopyInformation.File_Value += 1 'Step the counter formward,
      UpdateStatus(myCopyInformation) 'and show progress.

    Catch ex As Exception
      'If we can't copy, maybe we should keep a log...
      If (Not (toldAboutAdministratorRights)) Then
        toldAboutAdministratorRights = True
        Dim message As String = ("Error! " & ex.Message & vbCrLf & vbCrLf & "While some directories are protected, and can not be accessed, some access issues can be resolved by starting the application as an administrator.")
        MsgBox(message, MsgBoxStyle.Information Or MsgBoxStyle.Critical Or MsgBoxStyle.OkOnly, Me.Text)
      End If
    End Try

  End Sub

#End Region 'Directory recusion, and File copy routines

#Region "Directory attribute testing, and creation"

  ''' <summary>
  ''' This routine is used to test a Directory to see if it has the proper attributes.  Unless it is a root
  ''' Directory, if can not be marked as 'Hidden' or 'System'.  It must always be marked as 'Directory', and 
  ''' must never be marked as 'Offline', 'Temporary', or 'ReparsePoint'.  Finally, unless it is a Source
  ''' Directory, it can not be marked as 'ReadOnly'.
  ''' </summary>
  ''' <param name="myCopyInformation"></param>
  ''' <param name="testDirectoryInfo">The Directory to test.</param>
  ''' <param name="CanBeReadOnly">A flag to indicate that it can be marked 'ReadOnly'.</param>
  ''' <returns>True if the attributes are correct, else False.</returns>
  ''' <remarks></remarks>
  Friend Function CorrectDirectoryAttributes(myCopyInformation As CopyInformation, testDirectoryInfo As DirectoryInfo, ByVal canBeReadOnly As Boolean) As Boolean
    '
    'Test to make sure it's a we have a valid directory descriptor, that it describes a directory,
    'and that it is not: Hidden, Offline, System, Temporary, or a ReparsePoint. (ReparsePoint = link)
    Return (testDirectoryInfo IsNot Nothing AndAlso testDirectoryInfo.Exists _
            AndAlso ((testDirectoryInfo.Attributes And FileAttributes.Directory) <> 0) _
            AndAlso (canBeReadOnly OrElse ((testDirectoryInfo.Attributes And FileAttributes.ReadOnly) = 0)) _
            AndAlso (myCopyInformation.System OrElse (testDirectoryInfo.Parent Is Nothing) OrElse ((testDirectoryInfo.Attributes And FileAttributes.System) = 0)) _
            AndAlso (myCopyInformation.Hidden OrElse (testDirectoryInfo.Parent Is Nothing) OrElse ((testDirectoryInfo.Attributes And FileAttributes.Hidden) = 0)) _
            AndAlso ((testDirectoryInfo.Attributes And FileAttributes.Offline) = 0) _
            AndAlso ((testDirectoryInfo.Attributes And FileAttributes.Temporary) = 0) _
            AndAlso ((testDirectoryInfo.Attributes And FileAttributes.ReparsePoint) = 0) _
            AndAlso (Not (testDirectoryInfo.Name.ToUpper.Equals("$RECYCLE.BIN"))) _
            AndAlso (Not (testDirectoryInfo.Name.ToUpper.Equals("SYSTEM VOLUME INFORMATION"))) _
            )
  End Function
  Friend Function CorrectDirectoryAttributes(myCopyInformation As CopyInformation, testDirectoryFilePathName As String, ByVal CanBeReadOnly As Boolean) As Boolean
    Return CorrectDirectoryAttributes(myCopyInformation, New DirectoryInfo(testDirectoryFilePathName), CanBeReadOnly)
  End Function

  ''' <summary>
  ''' Returns a flag indicating that a Directory exists with the proper attributes, even if it  
  ''' needs to be created.
  ''' </summary>
  ''' <param name="myCopyInformation"></param>
  ''' <param name="newDirectoryName">The name of the Directory that must exist, or be created.</param>
  ''' <param name="canBeReadOnly">A flag to indicate if the Direfctory can be read-only.</param>
  ''' <returns>True if the Directory exists, else False.</returns>
  ''' <remarks>Only Source Directories can be read-only, and only Destination
  ''' Directories are created.</remarks>
  Friend Function VerifyOrCreateDirectory(myCopyInformation As CopyInformation, newDirectoryName As String, ByVal canBeReadOnly As Boolean) As Boolean
    'Verify the directory, or create it if it doesn't exist.
    Try
      If (Not (Directory.Exists(newDirectoryName))) Then 'Does the directory exist?
        Directory.CreateDirectory(newDirectoryName) 'If not, try creating it.
      End If
      Return CorrectDirectoryAttributes(myCopyInformation, newDirectoryName, canBeReadOnly) 'Return based on it's validity.

    Catch ex As Exception
      MsgBox("Opps! " & ex.Message, MsgBoxStyle.Information Or MsgBoxStyle.Critical Or MsgBoxStyle.OkOnly, Me.Text)
    End Try
    Return False

  End Function

#End Region 'Directory attribute testing, and creation

#Region "File attribute testing"

  ''' <summary>
  ''' This routine is used to test a File to see if it has the proper attributes.  It can not be marked
  ''' as 'Directory', 'Hidden', OffLine', 'System', 'Temporary', or 'ReparsePoint'.
  ''' </summary>
  ''' <param name="myCopyInformation"></param>
  ''' <param name="testFileInfo">The file to test.</param>
  ''' <returns>True if the attributes are correct, else False.</returns>
  ''' <remarks></remarks>
  Friend Function CorrectFileAttributes(myCopyInformation As CopyInformation, testFileInfo As FileInfo) As Boolean
    '
    'Test to make sure it's a we have a valid file descriptor, that it describes a file,
    'and that it not: Hidden, Offline, System, Temporary, or a ReparsePoint. (ReparsePoint = link)
    Return (testFileInfo IsNot Nothing AndAlso testFileInfo.Exists _
            AndAlso ((testFileInfo.Attributes And FileAttributes.Directory) = 0) _
            AndAlso (myCopyInformation.System OrElse (testFileInfo.Attributes And FileAttributes.System) = 0) _
            AndAlso (myCopyInformation.Hidden OrElse (testFileInfo.Attributes And FileAttributes.Hidden) = 0) _
            AndAlso ((testFileInfo.Attributes And FileAttributes.Offline) = 0) _
            AndAlso ((testFileInfo.Attributes And FileAttributes.Temporary) = 0) _
            AndAlso ((testFileInfo.Attributes And FileAttributes.ReparsePoint) = 0) _
            )
  End Function
  Friend Function CorrectFileAttributes(myCopyInformation As CopyInformation, testFileFilePathName As String) As Boolean
    Return CorrectFileAttributes(myCopyInformation, New FileInfo(testFileFilePathName))
  End Function

#End Region 'File attribute testing

  ''' <summary>
  ''' This starts the process in the background thread of recursing directories, and copy files between directories.
  ''' </summary>
  Private Sub myWorkerThread_DoWork(sender As Object, eventArgs As System.ComponentModel.DoWorkEventArgs) Handles myWorkerThread.DoWork
    Try
      Dim myCopyInformation As CopyInformation = DirectCast(eventArgs.Argument, CopyInformation)
      With myCopyInformation
        .IsFinished = False
        '
        .Directory_Text = "Reading source directories"
        .Directory_Maximum = 0
        .Directory_Value = 0
        .File_Text = textFourDash
        .File_Maximum = 0
        UpdateStatus(myCopyInformation)
        '
        Dim MaxDirectoryCount As Integer = 1
        RecurseSorceDirectories(myCopyInformation, .SourceRoot, MaxDirectoryCount)
        If (Not (myWorkerThread.CancellationPending) AndAlso (MaxDirectoryCount > 1)) Then 'If we weren't canceled, we continue
          .Directory_Maximum = (MaxDirectoryCount - 1)
          UpdateStatus(myCopyInformation)
          '
          RecurseSorceDirectories(myCopyInformation, .SourceRoot, 0&)
        End If
        '
        If (myWorkerThread.CancellationPending) Then
          eventArgs.Cancel = True
          .Directory_Text = "Canceled"
        Else
          .Directory_Text = "Finished"
        End If
        .File_Text = ("Directories scanned = " & sharedScannedDirectoryCount.ToString & _
                      "  Files copied/scanned = " & sharedCopiedFileCount.ToString & "/" & sharedScannedFileCount.ToString)
        'Setup the maximums to clear those displays.
        .Directory_Maximum = 0
        .File_Maximum = 0
        .IsFinished = True
        UpdateStatus(sharedCopyInformation)

      End With

    Catch ex As Exception
      MsgBox("Opps! " & ex.Message, MsgBoxStyle.Information Or MsgBoxStyle.Critical Or MsgBoxStyle.OkOnly, Me.Text)
    End Try

  End Sub

  ''' <summary>
  ''' Each time the background thread issues an update, we come here, along the UI thread, so that we can display information.
  ''' </summary>
  Private Sub myWorkerThread_ProgressChanged(sender As Object, eventArgs As System.ComponentModel.ProgressChangedEventArgs) Handles myWorkerThread.ProgressChanged
    Try
      'Report progress by displaying the data on the form.
      updateUI(DirectCast(eventArgs.UserState, CopyInformation))

    Catch ex As Exception
      MsgBox("Opps! " & ex.Message, MsgBoxStyle.Information Or MsgBoxStyle.Critical Or MsgBoxStyle.OkOnly, Me.Text)
    Finally
      'Here, if we have anything in the CommandListQueue, we set the timer to execute what backup commands it has.
      If (DirectCast(eventArgs.UserState, CopyInformation).IsFinished) Then 'We only do this if the last thread finished!
        EnableQueueCommandListProcessing()
      End If
    End Try
  End Sub

  Friend Sub updateUI(forCopyInformation As CopyInformation)
    Try
      With forCopyInformation
        ConfigureCancelExit(Not (.IsFinished))
        '
        UpdateProgressbar(Me.DirectoryProgressBar, .Directory_Maximum, .Directory_Value, .Directory_Text)
        UpdateProgressbar(Me.FileProgressBar, .File_Maximum, .File_Value, .File_Text)
      End With

    Catch ex As Exception
      MsgBox("Opps! " & ex.Message, MsgBoxStyle.Information Or MsgBoxStyle.Critical Or MsgBoxStyle.OkOnly, Me.Text)
    End Try
  End Sub
  Friend Sub UpdateProgressbar(forProgressBar As Controls.XpProgressBar, setMaximum As Integer, setValue As Integer, setText As String)
    Try
      If (setMaximum < 0) Then setMaximum = 0
      With forProgressBar
        If ((setMaximum >= 0) AndAlso (setMaximum <> .PositionMax)) Then
          .Position = 0
          .PositionMax = setMaximum
        End If
        If (setValue > setMaximum) Then setValue = setMaximum
        Select Case (setValue)
          Case Is >= 0, Is <= setMaximum : If (.Position <> setValue) Then .Position = setValue
        End Select
        If ((setText IsNot Nothing) AndAlso (setText <> .Text)) Then .Text = setText
        '.Refresh()
      End With
    Catch ex As Exception
      MsgBox("Opps! " & ex.Message, MsgBoxStyle.Information Or MsgBoxStyle.Critical Or MsgBoxStyle.OkOnly, Me.Text)
    End Try
  End Sub
  Friend Sub UpdateStatus(myCopyInformation As CopyInformation)
    Try
      myWorkerThread.ReportProgress(0, myCopyInformation)
      System.Threading.Thread.Sleep(5)

    Catch ex As Exception
      MsgBox("Opps! " & ex.Message, MsgBoxStyle.Information Or MsgBoxStyle.Critical Or MsgBoxStyle.OkOnly, Me.Text)
    End Try
  End Sub

#Region "Administrator access"

  Private Function VerifyAdminPrivilege() As Boolean
    Dim alreadyAdmin As Boolean = False 'Default to no Administrator access.
    Try 'Get the level of access this program has.
      Dim identity = WindowsIdentity.GetCurrent()
      Dim principal = New WindowsPrincipal(identity)
      alreadyAdmin = principal.IsInRole(WindowsBuiltInRole.Administrator)
    Catch ex As Exception
      alreadyAdmin = False
    End Try
    'If we're an Administrator, note it.
    If (alreadyAdmin) Then Me.RunAsAdmin.Text = "Running as Administrator"
    Return alreadyAdmin
  End Function

  Private Sub RunAsAdmin_Click(sender As System.Object, eventArgs As System.EventArgs) Handles RunAsAdmin.Click

    Try
      'Save eberything into settings that was sey on this run.
      My.Settings.LastSourceList = Me.SourceDirectory.SettingsItemList
      My.Settings.LastDestinationList = Me.DestinationDirectory.SettingsItemList
      My.Settings.Save()
      'Get the FilePathName to executable for this program.
      Dim executableFilePathName As String = Assembly.GetExecutingAssembly().Location
      'Setup the descriptor to start the program again with elevated privilege.
      Dim processInfo As ProcessStartInfo = New ProcessStartInfo()
      Dim commandString As String = "" 'Default this to an empty string to show no command line parameters on program start.
      Try 'Now we try to get the command line parameters into the command string bereft of quotation marks, and white-space.
        If (Environment.GetCommandLineArgs.Length > 1) Then
          commandString = Environment.GetCommandLineArgs(1).Replace(ControlChars.Quote, "").Trim()
        End If
      Catch ex As Exception
        commandString = "" 'If we fail, then start like we had no parameters.
      End Try
      processInfo.Arguments = commandString 'Restart with the same command line arguments we started with.
      processInfo.Verb = "runas"
      processInfo.FileName = executableFilePathName

      Try 'Now try to start the program with elevated privilge.
        Process.Start(processInfo)
        'If the re-start worked, then we can end oursleves.
        Me.Close()
        End
      Catch ex As Exception 'If they cancel the admin start, 
        'we will end up here with nothing to do.
      End Try

    Catch ex As Exception

    End Try

  End Sub

#End Region 'Administrator access

End Class
