cd GraphqlSchema
dotnet build
cd..
Clear-History
cls
$env:GetAllContinents = 'https://www.google.com'
cdk synth --app "dotnet exec ./GraphqlSchema/bin/Debug/net5.0/GraphqlSchema.dll" 
pause
