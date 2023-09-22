robocopy ..\Docs\Dependencies .\images\dependencies *.png /S

docfx --open-browser --serve
robocopy .\_site ..\..\AllOverIt-gh-pages /S
