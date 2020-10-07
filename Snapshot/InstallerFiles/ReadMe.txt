One Way Mirror will mirror all Files and Directories in the Source Directory into those same-named Files and Directories in the Destination Directory.  Only newer files will be copied, and no files or directories will be deleted in the Destination Directory.

Current release: v1.0.2012.0919

Instructions.

One Way Mirror is intended for use as a clean and simple backup mechanism for non-system, and non-hidden, files & folders.  It is used to copy directories and files that exist in the Source directory to those same-named directories and files in the Destination directory.

No directories or files in the Destination directory will be deleted!  No files in the Destination directory that are newer than their counterparts from the Source directory will be replaced!  However, any files in the Destination directory that are older than the files from the Source directory will be replaced.

In this manner, One Way Mirror can be used to maintain a set of file copies in the Destination directory that mirror the files that exist in the Source directory.

Note that One Way Mirror can be set to recurse directories in the Source directory, or not, as the need arises.

One Way Mirror can also read a script file (*.OWM) associated with One Way Mirror during installation.  Each line of the script file can contain up to 3 comma-delimited parameters, the first two of which are required:

One Way Mirror can be set to  run with Administrator rights in order to give it access to more directories, and files.   The button to re-launch One Way Mirror will be available if it is not already running with Administrative privilege, and will re-launch the application with a request for those elevated rights.

  <Source directory>, <Destination directory> 
												[, <'Recurse'>/<'NoRecurse'>] 	- Default = <'Recurse'>
												[, <'NoSystem'>/<'System'>]		- Default = <'NoSystem'>
												[, <'NoHidden'>/<'Hidden'>]		- Default = <'NoHidden'>
												[, <'AllFiles'>/<'OnlyCommon'>]	- Default = <'AllFiles'>

Recurse/NoRecurse 	Check all files in the source directory. If recursing, 
					do the same for all sub-directories below the  source directory.
					
System/NoSystem 	Copy those files maked as 'System' between the source and 
					destination directories.
					
Hidden/NoHidden		Copy those files maked as 'Hidden' between the source and 
					destination directories.
					
AllFiles/OnlyCommon	Either copy all files between the source and destination 
					directories, or only files that are already located in 
					both directory trees.

Examples:
  C:\My Documents, Z:\Backup\My Documents (If not specified, <'Recurse'> is assumed)
    This will copy all files in 'C:\My Documents' to 'Z:\Backup\My Documents', and then recurse into all SubDirectories under 'C:\My Documents' and do the same.
	'System' Directories or Files WILL NOT be copied.
	'Hidden' Directories or Files WILL NOT be copied.

  C:\My Documents, Z:\Backup\My Documents, Recurse
    This will copy all Files in 'C:\My Documents' to 'Z:\Backup\My Documents', and then recurse into all SubDirectories under 'C:\My Documents' and do the same.
	'System' Directories or Files WILL NOT be copied.
	'Hidden' Directories or Files WILL NOT be copied.

  C:\My Documents, Z:\Backup\My Documents, NoRecurse, System, Hidden
    This will copy all files in 'C:\My Documents' to 'Z:\Backup\My Documents'
	'System' Directories or Files WILL be copied.
	'Hidden' Directories or Files WILL be copied.
	
  C:\My Documents, Z:\Backup\My Documents, Recurse, Common
    This will copy only thtse files that exist in both the 'C:\My Documents' to 'Z:\Backup\My Documents', directories, and then recurse into all SubDirectories under 'C:\My Documents' and do the same.
	'System' Directories or Files WILL NOT be copied.
	'Hidden' Directories or Files WILL NOT be copied.

When One Way Mirror is called using command-line parameters, the same conventions apply for the arguments that are passed in.	

Note that any text that appears after a "//" in the .OWM file will be treated as a comment, and ignored.  Also, that comments are limited to a single line.

Examples:
  //This line is a comment. 
  C:\My Documents, Z:\Backup\My Documents, Recurse //This potion of the line is a comment.

ChangeLog:
  See 'Revisions.Txt' in this directory.