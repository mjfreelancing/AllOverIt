robocopy ..\Docs\Dependencies .\images\dependencies *.png /S

docfx --serve
robocopy .\_site ..\..\AllOverIt-gh-pages /S
