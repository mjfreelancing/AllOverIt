cd GraphqlSchema
dotnet build
cd..
Clear-History
cls
cdk synth --app "dotnet exec ./GraphqlSchema/bin/Debug/net5.0/GraphqlSchema.dll" 
pause
