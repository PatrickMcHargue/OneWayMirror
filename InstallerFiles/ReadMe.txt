One Way Mirror will mirror all Files and Directories in the Source Directory into those same-named Files and Directories in the Destination Directory.  Only newer files will be copied, and no files or directories will be deleted in the Destination Directory.

Script files are also supported.  These can be used to maintain a list of Source and Destination directories, and the options for copy files from the Source to the Destination.

The drop-down lists used on One Way Mirror's interface will be maintained between application starts.  In this way,  a list of directories, and Script files, can be easily remembered, and re-used.

Current release: v1.0.2013.0222

Instructions.

One Way Mirror is intended for use as a clean and simple backup mechanism for files & folders.  It is used to copy directories and files that exist in, and under, the Source directory to those same-named directories and files in, and under, the Destination directory.

No directories or files in the Source or Destination directories will be deleted!  No files in the Destination directory tree that are newer than their counterparts from the Source directory will be replaced!  However, any files in the Destination directory that are older than the files from the Source directory tree will be replaced.

In this manner, One Way Mirror can be used to maintain a set of file copies in the Destination directory that mirror the files that exist in the Source directory.

Note that One Way Mirror can be set to recurse directories in the Source directory, or not, as the need arises.  System and Hidden files can be included, or not.

Another option is to copy only files that are common between the Source and Destination directories.  With this option, you can maintain just a certain set of files in the Destination directory that are refreshed from the files in the Source directory.

One Way Mirror can be set to  run with Administrator rights in order to give it access to more directories, and files.   The button to re-launch One Way Mirror will be available if it is not already running with Administrative privilege, and will re-launch the application with a request for those elevated rights.

If you need to always run One Way Mirror with elevated permissions, you can edit the shortcut to the application to run the application with elevated permissions, or you can tell windows to mark the file as one to run with elevated permissions.

One Way Mirror also support (*.owm) script files.  Each line of the script file follows the same syntax shown below for the designation of the Source and Destination directories, and the options used.  

  <["]Source directory>["], ["]<Destination directory>["] 
												[, <Recurse>/<NoRecurse>] 	- Default = <Recurse>
												[, <NoSystem>/<System>]		  - Default = <NoSystem>
												[, <NoHidden>/<Hidden>]		  - Default = <NoHidden>
												[, <AllFiles>/<OnlyCommon>]	- Default = <AllFiles>

Recurse/NoRecurse 	Check all files in the source directory. If recursing, 
					do the same for all sub-directories below the  source directory.
					
System/NoSystem 	Copy those files marked as 'System' between the source and 
					destination directories.
					
Hidden/NoHidden		Copy those files marked as 'Hidden' between the source and 
					destination directories.
					
AllFiles/OnlyCommon	Either copy all files between the source and destination 
					directories, or only files that are already located in 
					both directory trees.

Note: Do not include the angle brackets ("<" or ">") shown above, they are used to document items in the command line.

Note: Quotes must surround path names that contain a comma (",") character.  Quotes must be matched at the start & end of any path that uses them.

Examples:
  C:\My Documents, Z:\Backup\My Documents (If not specified, <'Recurse'> is assumed)
  This will copy all files in 'C:\My Documents' to 'Z:\Backup\My Documents', and then recurse into all SubDirectories under 'C:\My Documents' and do the same.
	'System' Directories or Files WILL NOT be copied.
	'Hidden' Directories or Files WILL NOT be copied.

  "C:\My Documents", Z:\Backup\My Documents, Recurse
  This will copy all Files in 'C:\My Documents' to 'Z:\Backup\My Documents', and then recurse into all SubDirectories under 'C:\My Documents' and do the same.
	'System' Directories or Files WILL NOT be copied.
	'Hidden' Directories or Files WILL NOT be copied.

  C:\My Documents, "Z:\Backup\My Documents", NoRecurse, System, Hidden
  This will copy all files in 'C:\My Documents' to 'Z:\Backup\My Documents'
	'System' Directories or Files WILL be copied.
	'Hidden' Directories or Files WILL be copied.
	
  "C:\My Documents", "Z:\Backup\My Documents", Common
  This will copy only those files that exist in both the 'C:\My Documents' and 'Z:\Backup\My Documents', directories, and then recurse into all SubDirectories under 'C:\My Documents' and do the same.
	'System' Directories or Files WILL NOT be copied.
	'Hidden' Directories or Files WILL NOT be copied.

When One Way Mirror is called using command-line parameters, the same conventions apply for the arguments that are passed in.	

Note that any text that appears after a "//" in the .OWM file will be treated as a comment, and ignored.  Also, that comments are limited to a single line.

Examples:
  //This line is a comment. 
  C:\My Documents, Z:\Backup\My Documents, Recurse //This potion of the line is a comment and will appear on the form wen the line is being used.

ChangeLog:
  See 'Revisions.Txt' in this directory.