cd GraphqlSchema
dotnet build
cd..
Clear-History
cls
$env:GetAllContinents = 'https://www.google.com'
cdk bootstrap "aws://550269505143/ap-southeast-2" --profile personal 
cdk deploy --app "dotnet exec ./GraphqlSchema/bin/Debug/net5.0/GraphqlSchema.dll" --profile personal --require-approval never --verbose
pause
