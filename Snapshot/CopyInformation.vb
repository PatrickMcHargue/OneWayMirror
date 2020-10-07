Imports System
Imports System.IO

Public Class CopyInformation

  Property SourceRoot As DirectoryInfo       'Information on the source directory.
  Property DestinationRoot As DirectoryInfo  'Information on the destination directory.

  Property SubDirectories As Boolean 'A flag to indicate that we should recurse into SubDirectories
  Property System As Boolean         'Include System Directories and Files. 
  Property Hidden As Boolean         'Include Hidden Directories and Files.
  Property OnlyCommon As Boolean     'Only copy files that are in source & destination directories.

  Property IsFinished As Boolean

  Property Directory_Maximum As Integer
  Property Directory_Value As Integer
  Property Directory_Text As String

  Property File_Maximum As Integer
  Property File_Value As Integer
  Property File_Text As String

  Sub New()
    MyBase.New()
    initialize()
  End Sub

  Sub Initialize()

    Dim tmpDirectory As String = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
    SourceRoot = New DirectoryInfo(tmpDirectory)
    DestinationRoot = New DirectoryInfo(tmpDirectory)

    SubDirectories = True
    System = False
    Hidden = False
    OnlyCommon = False
    '
    IsFinished = True

    Directory_Maximum = 0
    Directory_Value = 0
    Directory_Text = textFourDash

    File_Maximum = 0
    File_Value = 0
    File_Text = textFourDash

  End Sub

End Class
