rem /IS :: Include Same files. (Includes same size files)
rem /IT :: Include Tweaked files. (Includes same files with different Attributes)
rem /IM :: Include Modified files (Includes same files with different times).

robocopy "C:\Data\GitHub\mjfreelancing\AllOverIt\Docs\Dependencies\net8.0" "C:\Data\GitHub\mjfreelancing\AllOverIt\DocFx\images\dependencies" *.png /IS /IT /IM
