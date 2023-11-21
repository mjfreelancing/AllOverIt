rem /S  :: Copy subfolders
rem /IS :: Include Same files. (Includes same size files)
rem /IT :: Include Tweaked files. (Includes same files with different Attributes)
rem /IM :: Include Modified files (Includes same files with different times).

robocopy ..\Docs\Dependencies .\images\dependencies *.png /S /IS /IT /IM

docfx --open-browser --serve

