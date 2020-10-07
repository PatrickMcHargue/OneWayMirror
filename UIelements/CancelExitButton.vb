''' <summary>
''' This class is used to provide a Button that can be an 'Exit' or a 'Cancel' button.  Also,
''' flags are used to communicate the state of the button, and how the user has used it.
''' </summary>
Public Class CancelExitButton
  Inherits System.Windows.Forms.Button 'It's a button, just with more stuff.

  Private _IsCancel As Boolean
  ''' <summary>
  ''' Get/Set if the button is a Cancel button, or an Exit button.
  ''' </summary>
  Property IsCancel As Boolean
    Get
      Return _IsCancel
    End Get
    Set(value As Boolean)
      _IsCancel = value 'Set the value,
      WasCanceled = False 'always clear this flag, and set the text for the button.
      If (_IsCancel) Then Me.Text = "Cancel" Else Me.Text = "Exit"
      Me.Refresh() 'Make sure we allow this to refresh!
    End Set
  End Property

  ''' <summary>
  ''' Get/Set if the button is a Exit button, or an Cancel button.
  ''' </summary>
  Property IsExit As Boolean
    Get
      Return Not (IsCancel)
    End Get
    Set(value As Boolean)
      IsCancel = Not (value)
    End Set
  End Property

  Private _WasCanceled As Boolean
  ''' <summary>
  ''' An indication that the button was clicked on while it was a Cancel button.
  ''' </summary>
  ''' <value></value>
  ''' <returns></returns>
  ''' <remarks></remarks>
  Property WasCanceled As Boolean
    Get
      Return (_IsCancel AndAlso _WasCanceled) 'This flag can only ever be true if we are a 'Cancel' button.
    End Get
    Set(value As Boolean)
      _WasCanceled = (_IsCancel AndAlso value) 'We can only set this flag true if we are a 'Cancel' button.
    End Set
  End Property

End Class
