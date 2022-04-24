cd GraphqlSchema
dotnet build
cd..
Clear-History
cls
$env:GetAllContinents = 'https://www.google.com'
cdk synth AppSyncDemoV1Stack2 --app "dotnet exec ./GraphqlSchema/bin/Debug/net5.0/GraphqlSchema.dll" > output.txt
pause
