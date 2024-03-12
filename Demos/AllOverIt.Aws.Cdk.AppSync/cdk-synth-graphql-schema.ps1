cd GraphqlSchema
dotnet build
cd..
Clear-History
cls
$env:GetAllContinentsUrl = 'https://www.abc.com'
cdk synth AppSyncDemoV1Stack2 --app "dotnet exec ./GraphqlSchema/bin/Debug/net8.0/GraphqlSchema.dll" > output.txt
pause
