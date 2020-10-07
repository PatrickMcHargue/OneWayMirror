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
  Dim sharedCorrectDestinationReadOnly As Boolean = True 'Should we correct the destination ReadOnly flag?

  Private isAdministrator As Boolean = False,toldAboutAdministratorRights As Boolean = False
  Private DirectoryProgressBarHdc, FileProgressBarHdc As IntPtr

  'A list of OWM commands buffered for execution.
  Private queueCommandList As New System.Collections.Queue()

#End Region 'Form-level Variables

#Region "Form events"

  Private Sub OneWayMirror_Load(sender As Object, e As System.EventArgs) Handles Me.Load

    Try
      isAdministrator = VerifyAdminPrivilege()
      toldAboutAdministratorRights = isAdministrator
      DirectoryProgressBarHdc = GetWindowDC(Me.DirectoryProgressBar.Handle)
      FileProgressBarHdc = GetWindowDC(Me.FileProgressBar.Handle)
      '
      CommandTimer.Tag = False 'Note that we are not processing from the queue.
      updateUI(sharedCopyInformation)
      Call LabelTheForm()
      If (Not (My.Settings.WasUpgraded)) Then 'If the settings file is new, 
        My.Settings.Upgrade() 'get the settings from the previous install,
        My.Settings.WasUpgraded = True 'and indicate that the upgrade was completed.
        My.Settings.Save() 'Save the upgraded settings now.
      End If

      'Load settings from the application's setting.
      Call LoadOurSettings()
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
      MsgBox("Opps! " & ex.Message, MsgBoxStyle.Information Or MsgBoxStyle.Critical Or MsgBoxStyle.OkOnly, Me.Text)
    End Try

  End Sub
  Private Sub OneWayMirror_FormClosing(sender As Object, eventArgs As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
    Try
      If (CancelExitButton.IsCancel) Then 'If we're showing a 'Canel', 
        Call CancelExitButton_Click(Nothing, New EventArgs) 'then we do this to stop the thread.
      End If
      Call CancelExitButton_Click(Nothing, New EventArgs) 'Do this to cleanup.
      ReleaseDC(Me.DirectoryProgressBar.Handle, DirectoryProgressBarHdc)
      ReleaseDC(Me.FileProgressBar.Handle, FileProgressBarHdc)

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
        Dim queuedCommand As String = queueCommandList.Dequeue.ToString 'Dequeue a command line.
        If (queuedCommand.ToUpper.EndsWith(textClose) OrElse queuedCommand.ToUpper.EndsWith(textClose1)) Then 'If we're done,
          Me.Enabled = False 'file processing is over, so no more touches!
          Me.Text = ("Finished " & Me.Text & " (closing)")  'note that we're done in the title,
          sharedCopyInformation.Directory_Text &= " (closing)" 'and in the progress bar.
          updateUI(sharedCopyInformation) 'Show it,
          Me.Refresh() 'and make it happen.
          System.Threading.Thread.Sleep(5000) 'Wait for 5 seconds so we can be read,
          CancelExitButton_Click(Nothing, New System.EventArgs) 'and we're done. 
        Else
          CommandTimer.Tag = True 'Note that we are processing from the queue, and that when done we come back here to end.
          If (ParseCommandLineOntoForm(queuedCommand)) Then ExecuteBackup() 'If we can parse the CommandLine, we start the copy process.
        End If

      Else 'Otherwise, there are no more items to process,
        LabelTheForm() 'clear any label on the form.
      End If

    Catch ex As Exception
      MsgBox("Opps! " & ex.Message, MsgBoxStyle.Information Or MsgBoxStyle.Critical Or MsgBoxStyle.OkOnly, Me.Text)
    End Try

  End Sub

  Private Sub LocateDirectory_Click(sender As System.Object, eventArgs As System.EventArgs) Handles LocateSourceDirectory.Click, LocateDestinationDirectory.Click
    '
    'Alow the user to select a directory for the source, and destination root directories.
    Try
      Dim directorySelector As New Windows.Forms.FolderBrowserDialog
      Dim currentPath As String = My.Application.Info.DirectoryPath, currentMessage As String = "Select directory for "
      Select Case (True) 'One thing for source, another for destination.
        Case sender Is LocateSourceDirectory
          currentMessage = "Source"
          currentPath = SourceDirectory.Text
          directorySelector.ShowNewFolderButton = False 'No new source folder.

        Case sender Is LocateDestinationDirectory
          currentMessage = "Destination"
          currentPath = DestinationDirectory.Text
          directorySelector.ShowNewFolderButton = True 'Yes to a new destination folder.

      End Select
      currentPath = GetValidDirectory(currentPath)

      'Show the selector, and set the default path to the current path.
      directorySelector.Description = currentMessage
      If (IO.Directory.Exists(currentPath)) Then
        directorySelector.SelectedPath = currentPath
      End If
      If (directorySelector.ShowDialog() <> Windows.Forms.DialogResult.Cancel) Then 'If we didn't cancel,
        currentPath = directorySelector.SelectedPath 'get the path set
        If ((currentPath.Length > 0) AndAlso Directory.Exists(currentPath)) Then 'and if it's valid,
          Select Case (True) 'One thing for source, another for destination.
            Case sender Is LocateSourceDirectory
              SourceDirectory.AddNewEntry(currentPath)
            Case sender Is LocateDestinationDirectory
              DestinationDirectory.AddNewEntry(currentPath)
          End Select
        End If
      End If

    Catch ex As Exception
      MsgBox("Opps! " & ex.Message, MsgBoxStyle.Critical Or MsgBoxStyle.Information Or MsgBoxStyle.OkOnly, Me.Text)
    End Try

  End Sub
  Private Sub StartCopy_Click(sender As System.Object, eventArgs As System.EventArgs) Handles StartCopy.Click
    '
    'Start the copy process.
    Try
      'Clear the statistics.
      sharedScannedDirectoryCount = 0
      sharedScannedFileCount = 0
      sharedCopiedFileCount = 0

      'Now set the data together as a command line, and call the routine to test and parse it.
      Dim commandLine As String = GetFormEntryAsString()
      If (ParseCommandLineOntoForm(commandLine)) Then ExecuteBackup() 'If we can parse the CommandLine, we start the copy process.

    Catch ex As Exception
      MsgBox("Opps! " & ex.Message, MsgBoxStyle.Critical Or MsgBoxStyle.Information Or MsgBoxStyle.OkOnly, Me.Text)
    Finally
      Call updateUI(sharedCopyInformation) 'Set the button as 'Exit'
    End Try

  End Sub

  ''' <summary>
  ''' Retrn the data entered on the form as a string.
  ''' </summary>
  Private Function GetFormEntryAsString(Optional includeAdministratorFlag As Boolean = False) As String
    Dim commandLine As String = (GetPathInQuotes(SourceDirectory.Text) & textComma & GetPathInQuotes(DestinationDirectory.Text))
    If (IncludeSubDirectories.Checked) Then commandLine &= (textComma & textRecurse)
    If (IncludeSystem.Checked) Then commandLine &= (textComma & textSystem)
    If (IncludeHidden.Checked) Then commandLine &= (textComma & textHidden)
    If (OnlyCommon.Checked) Then commandLine &= (textComma & textCommon)
    If (includeAdministratorFlag AndAlso isAdministrator) Then commandLine &= (textComma & textAdmin1)
    Return commandLine
  End Function

  Private Sub LocateScript_Click(sender As System.Object, eventArgs As System.EventArgs) Handles LocateScript.Click

    Try
      Dim previousFilePathName As String = "" 'Default this to noprevious script.
      If (Me.OWMfiles.SelectedItem IsNot Nothing) Then 'If we have something in the OWMfiles list,
        previousFilePathName = DirectCast(Me.OWMfiles.SelectedItem, UIelements.ListEntry).StringValue 'use that.
      End If

      'Define a FileDialog to use as a file locator.
      Dim openFileDialog As New System.Windows.Forms.OpenFileDialog
      openFileDialog.Title = "Load a One Way Mirror script"
      openFileDialog.Filter = ("One Way Mirror script | " & textStarDotOWMext)
      If (Not (String.IsNullOrEmpty(previousFilePathName))) Then 'If we have a FilePathName,
        openFileDialog.InitialDirectory = Path.GetDirectoryName(previousFilePathName) 'use its FilePath, and the name if it exists.
        If (File.Exists(previousFilePathName)) Then openFileDialog.FileName = Path.GetFileName(previousFilePathName)
      End If

      'Open the dialog to locate the file we need.
      If ((openFileDialog.ShowDialog() = DialogResult.OK) AndAlso File.Exists(openFileDialog.FileName)) Then 'If we locate a valid file,
        OWMfiles.AddNewEntry(openFileDialog.FileName) 'add it to the top of list of script files.
      End If

    Catch ex As Exception
      MsgBox("Opps! " & ex.Message, MsgBoxStyle.Information Or MsgBoxStyle.Critical Or MsgBoxStyle.OkOnly, Me.Text)
    End Try

  End Sub
  Private Sub SaveScript_Click(sender As System.Object, e As System.EventArgs) Handles SaveScript.Click

    Try
      'Define a FileDialog to use as a file save.
      Dim saveFileDialog As New System.Windows.Forms.SaveFileDialog
      saveFileDialog.Title = "Save a One Way Mirror script"
      saveFileDialog.Filter = ("One Way Mirror script | " & textStarDotOWMext)
      If (Me.OWMfiles.SelectedItem IsNot Nothing) Then
        Dim fullFilePathName As String = DirectCast(Me.OWMfiles.SelectedItem, UIelements.ListEntry).StringValue
        If (File.Exists(fullFilePathName)) Then
          saveFileDialog.FileName = System.IO.Path.GetFileName(fullFilePathName)
          saveFileDialog.InitialDirectory = System.IO.Path.GetDirectoryName(saveFileDialog.FileName)
        Else
          saveFileDialog.InitialDirectory = GetValidDirectory("")
        End If
      End If

      'Open the dialog to locate the file we need.
      If (saveFileDialog.ShowDialog() = DialogResult.OK) Then
        'Now set the data together as a command line, and call the routine to test and parse it.
        Dim commandLine As String = GetFormEntryAsString(True)
        If (ParseCommandLineOntoForm(commandLine)) Then 'If we can parse the CommandLine, we start the write.
          'If (File.Exists(saveFileDialog.FileName)) Then 'If we locate an existing file,
          '  File.Delete(saveFileDialog.FileName) 'delete it so we can overwrite it.
          'End If
          File.WriteAllLines(saveFileDialog.FileName, New String() {commandLine}) 'Write the OWN information.
          OWMfiles.AddNewEntry(saveFileDialog.FileName) 'add it to the top of list of script files.
        End If
      End If

    Catch ex As Exception
      MsgBox("Opps! " & ex.Message, MsgBoxStyle.Information Or MsgBoxStyle.Critical Or MsgBoxStyle.OkOnly, Me.Text)
    End Try

  End Sub
  Private Sub ExecuteScript_Click(sender As System.Object, eventArgs As System.EventArgs) Handles ExecuteScript.Click

    Try
      If (Me.OWMfiles.SelectedItem IsNot Nothing) Then
        Dim OWMfilePathName As String = DirectCast(Me.OWMfiles.SelectedItem, UIelements.ListEntry).StringValue
        If (FileDoesExist(OWMfilePathName)) Then
          Call QueueOwmFileCommands(queueCommandList, OWMfilePathName) 'Call the routine to start the copy process.
          If (queueCommandList.Count > 0) Then 'Here, if we have anything in the CommandListQueue,
            EnableQueueCommandListProcessing() 'we set the timer to execute what backup commands it has.
          Else
            MsgBox("No commands in the OWM script file!" & vbCrLf & OWMfilePathName, MsgBoxStyle.Information Or MsgBoxStyle.Critical Or MsgBoxStyle.OkOnly, Me.Text)
          End If
        Else
          MsgBox("Can't find OWM script file on disk!" & vbCrLf & OWMfilePathName, MsgBoxStyle.Information Or MsgBoxStyle.Critical Or MsgBoxStyle.OkOnly, Me.Text)
        End If
      Else
        MsgBox("No OWM script file selected!", MsgBoxStyle.Information Or MsgBoxStyle.Critical Or MsgBoxStyle.OkOnly, Me.Text)
      End If

    Catch ex As Exception
      MsgBox("Opps! " & ex.Message, MsgBoxStyle.Information Or MsgBoxStyle.Critical Or MsgBoxStyle.OkOnly, Me.Text)
    End Try

  End Sub

  Private Sub CancelExitButton_Click(sender As System.Object, eventArgs As System.EventArgs) Handles CancelExitButton.Click
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
        Call SaveOurSettings()
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
  Private Sub OWMfiles_SelectedIndexChanged(sender As Object, eventArgs As System.EventArgs) Handles OWMfiles.SelectedIndexChanged
    If (Me.OWMfiles.SelectedItem IsNot Nothing) Then
      Me.ToolTip1.SetToolTip(Me.OWMfiles, DirectCast(Me.OWMfiles.SelectedItem, UIelements.ListEntry).StringValue)
    Else
      Me.ToolTip1.SetToolTip(Me.OWMfiles, "")
    End If
  End Sub

#End Region 'Form control event routines

#Region "Helper routines"

  ''' <summary>
  ''' This routine is used to load all setings from the user area of the application's settings.
  ''' </summary>
  ''' <remarks></remarks>
  Private Sub LoadOurSettings()

    'Do the checks.
    IncludeSubDirectories.Checked = My.Settings.LastIncludeSubdirectories
    IncludeSystem.Checked = My.Settings.LastIncludeSystem
    IncludeHidden.Checked = My.Settings.LastIncludeHidden
    OnlyCommon.Checked = My.Settings.LastOnlyCommon

    'Load the SourceDirectory list, and verify that the items represent real DirectoryPaths.
    VerifyDirectoryAndFile_PathNames(Me.SourceDirectory, My.Settings.LastSourceList)

    'Load the DestinationDirectory list, and verify that the items represent real DirectoryPaths.
    VerifyDirectoryAndFile_PathNames(Me.DestinationDirectory, My.Settings.LastDestinationList)

    'Load the OWM file list,  and verify that the items represent real FilePathNames.
    VerifyDirectoryAndFile_PathNames(Me.OWMfiles, My.Settings.LastOWMfiles)

  End Sub
  Private Sub VerifyDirectoryAndFile_PathNames(testComboBox As UIelements.ComboBoxWithMemory, stringCollection As System.Collections.Specialized.StringCollection)

    Try
      testComboBox.Items.Clear()
      If (stringCollection IsNot Nothing) Then
        Dim validFilePathNames As New Generic.List(Of UIelements.ListEntry)
        For Each testListEntry As String In stringCollection
          If (Not (testComboBox.ShowOnlyFileName) OrElse FileDoesExist(testListEntry)) Then
            validFilePathNames.Add(New UIelements.ListEntry(testListEntry, testComboBox.ShowOnlyFileName))
          End If
        Next testListEntry
        testComboBox.Items.AddRange(validFilePathNames.ToArray)
        If (testComboBox.Items.Count > 0) Then testComboBox.SelectedIndex = 0
      End If
    Catch ex As Exception
      MsgBox("Opps! " & ex.Message, MsgBoxStyle.Information Or MsgBoxStyle.Critical Or MsgBoxStyle.OkOnly, Me.Text)
    End Try

  End Sub
  ''' <summary>
  ''' This routine is used to save all setings into the user area of the application's settings.
  ''' </summary>
  ''' <remarks></remarks>
  Private Sub SaveOurSettings()

    'Do the checks.
    My.Settings.LastIncludeSubdirectories = IncludeSubDirectories.Checked
    My.Settings.LastIncludeSystem = IncludeSystem.Checked
    My.Settings.LastIncludeHidden = IncludeHidden.Checked
    My.Settings.LastOnlyCommon = OnlyCommon.Checked

    'Now do the lists.
    My.Settings.LastSourceList = Me.SourceDirectory.SettingsItemList
    My.Settings.LastDestinationList = Me.DestinationDirectory.SettingsItemList
    My.Settings.LastOWMfiles = Me.OWMfiles.SettingsItemList

    My.Settings.Save() 'Save the settings!

  End Sub
  ''' <summary>
  ''' This routine sets the CancelExit button to act either as a Cancel button, or as an Exit button. 
  ''' </summary>
  ''' <param name="SetAsCancel">If True, set Cancel button, else set Exit button.</param>
  ''' <remarks>This also corrects the enable for other of the form's controls.</remarks>
  Private Sub ConfigureCancelExit(ByVal SetAsCancel As Boolean)
    With CancelExitButton
      .IsCancel = SetAsCancel                       'Note the button we are: 'Cancel' or 'Exit'.
      Me.SetupGroupBox.Enabled = Not (SetAsCancel)  'Initialize these to match so that they
      Me.StartCopy.Enabled = Not (SetAsCancel)      'are disabled while we're 'Cancel'.
      Me.OWMfileGroup.Enabled = Not (SetAsCancel)   '
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
      sharedCopyInformation.Initialize()
      updateUI(sharedCopyInformation)

      commandLine = ParseCommentIntoLabel(Me.CommentLabel, commandLine) 'Strip off, and show, the comment for the line.
      Dim tmpCopyInformation As CopyInformation = ParseCommandLineIntoCopyInformation(commandLine) 'Now parse the copy information from the text line.

      'Now we populate the form with information from the command line if it was correctly parsed.
      If (tmpCopyInformation IsNot Nothing) Then
        sharedCopyInformation = tmpCopyInformation
        With sharedCopyInformation
          Me.IncludeSubDirectories.Checked = .SubDirectories : Me.IncludeSubDirectories.Refresh()
          Me.IncludeSystem.Checked = .System : Me.IncludeSystem.Refresh()
          Me.IncludeHidden.Checked = .Hidden : Me.IncludeHidden.Refresh()
          Me.OnlyCommon.Checked = .OnlyCommon : Me.OnlyCommon.Refresh()
          '
          Me.SourceDirectory.AddNewEntry(.SourceRoot.FullName)
          '
          Me.DestinationDirectory.AddNewEntry(.DestinationRoot.FullName)
        End With
        Return True 'Here, everything worked, so we succeed back.
      End If

    Catch ex As Exception
      errorMessage = ("Bad command! " & vbCrLf & GetTextInTicks(commandLine) & vbCrLf & vbCrLf & ex.Message)
    Finally
      updateUI(sharedCopyInformation)
    End Try

    Return False 'If we get here, we failed.

  End Function
  ''' <summary>
  ''' This routine returns a line stripped of a comment section.  That comment section is loaded into the
  ''' .Text field of the passed in Label control.
  ''' </summary>
  ''' <param name="labelToLoad">The Label control to load with the comment.</param>
  ''' <param name="lineToParse">The line to parse for a command.</param>
  ''' <returns>The line to parse without the comment section.</returns>
  ''' <remarks></remarks>
  Private Function ParseCommentIntoLabel(labelToLoad As System.Windows.Forms.Label, ByVal lineToParse As String) As String
    Dim lineComment As String = "" 'No comment!
    Try
      If (Not (String.IsNullOrEmpty(lineToParse))) Then
        If (lineToParse.Contains(textCommentDelimiter)) Then 'If we have a comment on the line,
          Dim commentPosition As Integer = lineToParse.IndexOf(textCommentDelimiter)
          lineComment = lineToParse.Substring(commentPosition + 2) 'get the comment,
          lineToParse = lineToParse.Substring(0, commentPosition).Trim 'and strip of the comment.
        End If
      End If

    Catch ex As Exception
      MsgBox("Opps! " & ex.Message, MsgBoxStyle.Information Or MsgBoxStyle.Critical Or MsgBoxStyle.OkOnly, Me.Text)
    Finally
      labelToLoad.Text = lineComment
    End Try
    Return lineToParse 'Now trim the lineToParse,

  End Function
  Private Function ParseCommandLineIntoCopyInformation(ByVal CommandLine As String) As CopyInformation

    Dim returnCopyInformation As New CopyInformation()
    Dim secondCharPos As Integer, errorMessage As String = ""
    Dim tmpCommandLine As String = CommandLine

    With returnCopyInformation
      If (String.IsNullOrEmpty(tmpCommandLine)) Then
        errorMessage = "Empty command line data."
      End If
      '
      'Pull apart the command line for the Source directory.
      If (String.IsNullOrEmpty(errorMessage)) Then 'We process if we have no error.
        tmpCommandLine = tmpCommandLine.Trim
        If (tmpCommandLine.StartsWith(ControlChars.Quote)) Then 'If we start with a quote,
          secondCharPos = tmpCommandLine.IndexOf(ControlChars.Quote, 1) 'then we have to end with a quote.
          If (secondCharPos > 0) Then 'If we have a second delimiter position, get the directory for the path, and strip the text.
            returnCopyInformation.SourceRoot = GetDirectoryInfoFromPathName(tmpCommandLine.Substring(1, secondCharPos - 1))
            tmpCommandLine = tmpCommandLine.Substring(secondCharPos + 2)
          Else 'Otherwise, we have no path, and we say so.
            errorMessage &= ("Ill-formed Source directory" & vbCrLf)
          End If
        Else 'If we don't start with a quote, then we must end with a comma.
          secondCharPos = tmpCommandLine.IndexOf(textComma, 0)
          If (secondCharPos > 0) Then 'If we have a second delimiter position, get the directory for the path, and strip the text.
            returnCopyInformation.SourceRoot = GetDirectoryInfoFromPathName(tmpCommandLine.Substring(0, secondCharPos))
            tmpCommandLine = tmpCommandLine.Substring(secondCharPos + 1)
          Else 'Otherwise, we have no path, and we say so.
            errorMessage &= ("Ill-formed Source directory" & vbCrLf)
          End If
        End If
        If (String.IsNullOrEmpty(errorMessage) AndAlso Not (returnCopyInformation.SourceRoot.Exists)) Then
          errorMessage &= ("Source directory doesn't exist:" & vbCrLf & returnCopyInformation.SourceRoot.FullName)
        End If
        '
        'Pull apart the command line for the Destination directory.
        If (String.IsNullOrEmpty(errorMessage)) Then 'We process if we have no error.
          tmpCommandLine = tmpCommandLine.Trim
          If (tmpCommandLine.StartsWith(ControlChars.Quote)) Then 'If we start with a quote,
            secondCharPos = tmpCommandLine.IndexOf(ControlChars.Quote, 1) 'then we have to end with a quote.
            If (secondCharPos > 0) Then 'If we have a second delimiter position, get the directory for the path, and strip the text.
              returnCopyInformation.DestinationRoot = GetDirectoryInfoFromPathName(tmpCommandLine.Substring(1, secondCharPos - 1))
              tmpCommandLine = tmpCommandLine.Substring(secondCharPos + 2)
            Else 'Otherwise, we have no path, and we say so.
              errorMessage &= ("Ill-formed Destination directory" & vbCrLf)
            End If
          Else 'If we don't start with a quote, then we must end with a comma.
            secondCharPos = tmpCommandLine.IndexOf(textComma, 0)
            If (secondCharPos > 0) Then 'If we have a second delimiter position, get the directory for the path, and strip the text.
              returnCopyInformation.DestinationRoot = GetDirectoryInfoFromPathName(tmpCommandLine.Substring(0, secondCharPos))
              tmpCommandLine = tmpCommandLine.Substring(secondCharPos + 1)
            Else 'Otherwise, we have no path, and we say so.
              errorMessage &= ("Ill-formed Destination directory" & vbCrLf)
            End If
          End If
          If (String.IsNullOrEmpty(errorMessage) AndAlso Not (returnCopyInformation.DestinationRoot.Exists)) Then
            errorMessage &= ("Destination directory doesn't exist:" & vbCrLf & returnCopyInformation.DestinationRoot.FullName)
          End If
        End If
        '
        'With the source & destination directories stripped off the command line, we pull apart the string for optional arguments.
        tmpCommandLine = tmpCommandLine.Trim
        If (String.IsNullOrEmpty(errorMessage) AndAlso Not (String.IsNullOrEmpty(tmpCommandLine))) Then 'We process if we have no error.
          Dim commandArguments() As String = tmpCommandLine.Split(CChar(textComma)) 'Split the command line into 'Source', 'Destination', [and optional 'Recurse/Norecurse']
          Dim tmpString As String = ""
          '
          If (commandArguments.Length >= 0) Then 'Next, we pull of all the comma-delimted switches to set the internal flags.
            For switchIndex As Integer = 0 To (commandArguments.Length - 1)
              tmpString = commandArguments(switchIndex).Replace(ControlChars.Quote, "").Trim() 'strip characters we don't need,
              Select Case (tmpString.ToUpper) 'Now, based on the UPPERCASE switch, set the associated flag.
                Case textRecurse, textRecurse1, textRecurse2, textRecurse3, textRecurse4 : .SubDirectories = True
                Case textNoRecurse, textNoRecurse1, textNoRecurse2, textNoRecurse3, textNoRecurse4 : .SubDirectories = False
                Case textSystem, textSystem1, textSystem2 : .System = True
                Case textNoSystem, textNoSystem1 : .System = False
                Case textHidden, textHidden1, textHidden2 : .Hidden = True
                Case textNoHidden, textNoHidden1 : .Hidden = False
                Case textCommon, textCommon1, textCommon2 : .OnlyCommon = True
                Case textNotCommon, textNotCommon1 : .OnlyCommon = False
                Case textAdmin, textAdmin1 : .RunAsAdmin = True
                Case Else : errorMessage &= ("Option unrecognized: " & GetTextInTicks(tmpString) & vbCrLf) : Exit For 'If nothing, or not recognized, report the error.
              End Select
            Next switchIndex
          End If
        End If
      End If
    End With

    'If we have an error message, we failed, and we talk about it. (and return Nothing)
    If (Not (String.IsNullOrEmpty(errorMessage))) Then 'If there was an error, we display that.
      errorMessage &= (vbCrLf & vbCrLf & "Input line was:" & vbCrLf & GetTextInTicks(CommandLine)) 'If there's an error, add the CommandLine to the explanation.
      MsgBox(errorMessage, MsgBoxStyle.Critical Or MsgBoxStyle.Information Or MsgBoxStyle.OkOnly, Me.Text)
      Return Nothing
    End If

    Return returnCopyInformation 'If we get here, we succeeded, so return what we parsed.

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
        If (Not (CorrectDirectoryAttributes(sharedCopyInformation, .DestinationRoot, True))) Then 'Correct? (it can NOT be read-only)
          messageString &= (vbCrLf & "The Destination directory does not exist, is marked 'System', 'Hidden', 'Offline', 'Temporary' or is a link:" & vbCrLf & "   " & GetTextInTicks(.DestinationRoot.FullName) & vbCrLf)
        End If
        If (Not (String.IsNullOrEmpty(messageString))) Then
          messageString = ("Error with directories!" & vbCrLf & vbCrLf & messageString)
          MsgBox(messageString, MsgBoxStyle.Exclamation Or MsgBoxStyle.Critical Or MsgBoxStyle.OkOnly, Me.Text)

        Else 'Otherwise, we have good root directories...
          If (.RunAsAdmin And Not (isAdministrator)) Then 'If we need to run with elevated privilege, and we're not elevated,
            Call RunAsAdmin_Click(Nothing, New System.EventArgs) 'seek the elevated status that we need. We don't return if this works, but continue in a new, elevated, incarnation.
          End If
          'Here, elevated or not, we run to recurse directories, and make copies.
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
      MsgBox("Not a valid directory: " & vbCrLf & PathName & vbCrLf & vbCrLf & ex.Message & vbCrLf, MsgBoxStyle.Information Or MsgBoxStyle.Critical Or MsgBoxStyle.OkOnly, Me.Text)
    End Try
    Return Nothing 'Opps!

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
  ''' <summary>
  ''' Return the entire 'pathToParse' in quotes.
  ''' </summary>
  Private Function GetPathInQuotes(ByVal pathToParse As String) As String
    Return (ControlChars.Quote & pathToParse.Replace(ControlChars.Quote, "").Trim & ControlChars.Quote)
  End Function
  Private Declare Function GetWindowDC Lib "user32.dll" (ByVal hwnd As IntPtr) As IntPtr
  Private Declare Function ReleaseDC Lib "user32.dll" (ByVal hwnd As IntPtr, ByVal hdc As IntPtr) As IntPtr
  Private Declare Function PathCompactPath Lib "shlwapi.dll" Alias "PathCompactPathA" (ByVal hDC As IntPtr, ByVal lpszPath As String, ByVal dx As Int32) As Int32

#End Region 'Helper routines

#Region "Directory recusion, and File copy routines"

  ''' <summary>
  ''' This function will either locate a '.OWM' file to run as a script, or use the .OWM FilePathName
  ''' passed into it.
  ''' </summary>
  ''' <param name="useFilePathName">If set, it is the FilePathName of the '.OWM' script file to run.</param>
  ''' <remarks></remarks>
  Private Sub QueueOwmFileCommands(queueBuffer As System.Collections.Queue, ByVal useFilePathName As String)
    '
    'Here, we load an '.owm' script file to finish all backups in the list.
    Try
      queueBuffer.Clear() 'Clear the queue we will be loading.

      'If we have a valid Owm FilePathName, we process it into the queueBuffer.
      If (FileDoesExist(useFilePathName)) Then
        Me.OWMfiles.AddNewEntry(useFilePathName)

        Dim theOwnFileName As String = Path.GetFileName(useFilePathName)
        Me.Text = ("Processing: " & theOwnFileName)
        '
        Dim readLine As String, queueReverseOrder As New System.Collections.Queue
        Dim hasData As Boolean, isComment As Boolean
        Dim backupFileStream As New FileStream(useFilePathName, FileMode.Open, FileAccess.Read, FileShare.Read) 'Open a shared read file stream, (to not lock the file)
        If (backupFileStream.CanRead) Then 'If it can be read,
          Dim readBackupsFile As New StreamReader(backupFileStream) 'open a stream reader to the file stream.
          While Not (readBackupsFile.EndOfStream) 'While the stream still has data to read, 
            readLine = readBackupsFile.ReadLine.Trim 'read the line.

            'Characterize the line.
            hasData = Not (String.IsNullOrEmpty(readLine))
            isComment = (hasData AndAlso readLine.ToUpper.StartsWith(textCommentDelimiter))
            isComment = (isComment AndAlso Not (readLine.ToUpper.Equals(textClose)))
            isComment = (isComment AndAlso Not (readLine.ToUpper.Equals(textClose1)))

            'If the line isn't blank, or a comment line,
            If (hasData And Not (isComment)) Then
              queueBuffer.Enqueue(readLine) 'include it in the queue.
            End If
          End While
          'Close the reader.
          readBackupsFile.Close()
          readBackupsFile.Dispose()
        End If
        'Close the stream.
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
          If (VerifyOrCreateDirectory(myCopyInformation, copyToDirectoryPath)) Then 'Destination good, or can be made so? 
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
  ''' <returns>True if the Directory exists, else False.</returns>
  ''' <remarks>Only Source Directories can be read-only, and only Destination
  ''' Directories are created.</remarks>
  Friend Function VerifyOrCreateDirectory(myCopyInformation As CopyInformation, newDirectoryName As String) As Boolean
    'Verify the directory, or create it if it doesn't exist.
    Static directoryIsOK As Boolean, tmpDirectoryInfo As DirectoryInfo
    Try
      If (Not (Directory.Exists(newDirectoryName))) Then 'Does the directory exist?
        Directory.CreateDirectory(newDirectoryName) 'If not, try creating it.
      End If
      directoryIsOK = CorrectDirectoryAttributes(myCopyInformation, newDirectoryName, False) 'Is the directory valid?
      If (Not (directoryIsOK) AndAlso sharedCorrectDestinationReadOnly) Then 'If it's not OK, and we're to correct ReadOnly in the destination,
        tmpDirectoryInfo = New DirectoryInfo(newDirectoryName) 'get info on the destination directory,
        If ((tmpDirectoryInfo.Attributes And FileAttributes.ReadOnly) <> 0) Then 'and if ReadOnly is lit,
          tmpDirectoryInfo.Attributes = (tmpDirectoryInfo.Attributes And Not (FileAttributes.ReadOnly))
        End If
      End If
      Return directoryIsOK 'Return what we found.

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
        UpdateProgressbar(Me.DirectoryProgressBar, .Directory_Maximum, .Directory_Value, truncatedFilePathName(.Directory_Text, DirectoryProgressBarHdc, Me.DirectoryProgressBar.Width))
        UpdateProgressbar(Me.FileProgressBar, .File_Maximum, .File_Value, truncatedFilePathName(.File_Text, FileProgressBarHdc, Me.FileProgressBar.Width))

        '
        'UpdateProgressbar(Me.DirectoryProgressBar, .Directory_Maximum, .Directory_Value, .Directory_Text)
        'UpdateProgressbar(Me.FileProgressBar, .File_Maximum, .File_Value, .File_Text)
        '
      End With

    Catch ex As Exception
      MsgBox("Opps! " & ex.Message, MsgBoxStyle.Information Or MsgBoxStyle.Critical Or MsgBoxStyle.OkOnly, Me.Text)
    End Try
  End Sub
  Friend Sub UpdateProgressbar(forProgressBar As XpControls.XpProgressBar, setMaximum As Integer, setValue As Integer, setText As String)
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
  Friend Function truncatedFilePathName(fullFilePathName As String, controlHdc As IntPtr, controlWidth As Integer) As String
    Dim tmpText As String = fullFilePathName
    If (PathCompactPath(controlHdc, tmpText, controlWidth) > 0) Then
      controlWidth = InStr(1, tmpText, vbNullChar)
      If (controlWidth > 0) Then 'Trim off characters after null
        tmpText = tmpText.Substring(0, controlWidth - 1)
      End If
    End If
    Return tmpText
  End Function

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
      'Save everything into settings that was set on this run.
      Call SaveOurSettings()

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
        'we will end up here with nothing to do but continue.
      End Try

    Catch ex As Exception

    End Try

  End Sub

#End Region 'Administrator access

End Class
