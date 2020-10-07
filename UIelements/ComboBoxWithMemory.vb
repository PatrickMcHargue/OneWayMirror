Imports System.Windows.Forms.ComboBox

''' <summary>
''' This class is used to provide a ComboBox that can remember it's previously listed items.
''' </summary>
Public Class ComboBoxWithMemory
  Inherits System.Windows.Forms.ComboBox

  ''' <summary>
  ''' Get/Set the length of the list as it is maintained in the application's MySettings. 
  ''' </summary>
  ''' <remarks>This is deafulted to 8.</remarks>
  Property ListLength As Integer = 8
  ''' <summary>
  ''' A flag to show that the list entry shown is a FilePathName, and should be listed as a Filename only.
  ''' </summary>
  ''' <remarks>This is deafulted to False.</remarks>
  Property ShowOnlyFileName As Boolean = False

  ''' <summary>
  ''' Add an entry to the list of items.
  ''' </summary>
  ''' <param name="newEntry">The text of the entry to add.</param>
  ''' <remarks></remarks>
  Sub AddNewEntry(newEntry As String)

    Try
      'Insert the new entry at the top of the list.
      Me.Items.Insert(0, New ListEntry(newEntry, ShowOnlyFileName))

      'Get a list of strings based on the current ListEntry objects in the collection.
      Dim allListEntires As New Generic.List(Of String)
      For Each addListEntry As ListEntry In Me.Items
        allListEntires.Add(addListEntry.StringValue)
      Next addListEntry

      'Now start scanning for, and removing duplicates as we iterate over the list.
      For testListEntryIndex As Integer = 0 To (allListEntires.Count - 2) 'Start at the top, and go to just shy of the bottom,
        For removeListEntryIndex As Integer = (allListEntires.Count - 1) To (testListEntryIndex + 1) Step -1 'and then back up from the bottom to just shy of the testListEntryIndex.
          If (allListEntires(testListEntryIndex).Equals(allListEntires(removeListEntryIndex), StringComparison.CurrentCultureIgnoreCase)) Then 'If we find a duplicate,
            allListEntires.RemoveAt(removeListEntryIndex) 'remove the duplicate from below the testListEntryIndex.
          End If
        Next removeListEntryIndex
      Next testListEntryIndex

      'Now load the list of entries back into the current collection.
      Me.Items.Clear() 'Clear the current list.
      For Each itemToAdd As String In allListEntires 'Lop over the remaining entries,
        Me.Items.Add(New ListEntry(itemToAdd, ShowOnlyFileName)) 'and add each as a ListEntry.
      Next itemToAdd
      If (Me.Items.Count > 0) Then Me.SelectedIndex = 0 'If we have a collection with entries, select the top-most.

      Refresh() 'now refresh the list.

    Catch ex As Exception
      MsgBox("Opps! " & ex.Message, MsgBoxStyle.Information Or MsgBoxStyle.Critical Or MsgBoxStyle.OkOnly, Me.Text)
    End Try

  End Sub

  ''' <summary>
  ''' get/Set the list of ComboBox items using the 'System.Collections.Specialized.StringCollection' class
  ''' compatible with an application's saved setting.  Note that the length of this list is limited.
  ''' </summary>
  ''' <value>The 'System.Collections.Specialized.StringCollection' retreived from the application's MySettings.</value>
  ''' <returns>A 'System.Collections.Specialized.StringCollection' suitable for storing in the application's MySettings.</returns>
  ''' <remarks>The length of this list is limited by the 'ListLength' property.</remarks>
  Property SettingsItemList As System.Collections.Specialized.StringCollection

    Get
      Dim savedList As New System.Collections.Specialized.StringCollection
      For Each listItem As ListEntry In Me.Items
        savedList.Add(listItem.StringValue)
      Next
      If (savedList.Count > _ListLength) Then
        For listIndex As Integer = (savedList.Count - 1) To _ListLength
          savedList.RemoveAt(listIndex)
        Next
      End If
      Return savedList
    End Get

    Set(savedList As System.Collections.Specialized.StringCollection)
      Me.Items.Clear()
      If (savedList Is Nothing) Then Return
      For Each listItem As String In savedList
        Me.Items.Add(New ListEntry(listItem, ShowOnlyFileName))
      Next
      If (Me.Items.Count > 0) Then Me.SelectedIndex = 0
    End Set

  End Property

End Class

Public Class ListEntry

  Property StringValue As String
  Private _ShowOnlyFileName As Boolean

  Sub New()
    _StringValue = ""
    _ShowOnlyFileName = False
  End Sub
  Sub New(newStringValue As String)
    MyBase.new()
    _StringValue = newStringValue
  End Sub
  Sub New(newStringValue As String, newShowOnlyFileName As Boolean)
    MyBase.new()
    _StringValue = newStringValue
    _ShowOnlyFileName = newShowOnlyFileName
  End Sub

  Overrides Function ToString() As String
    If (_ShowOnlyFileName) Then
      Return FileIO.FileSystem.GetName(_StringValue)
    End If
    Return _StringValue
  End Function

End Class

