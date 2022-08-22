$env:GetAllContinents = 'https://www.google.com'
cdk bootstrap "aws://550269505143/ap-southeast-2" 
cdk deploy --app "dotnet exec ./GraphqlSchema/bin/Release/net6.0/GraphqlSchema.dll" --require-approval never --verbose --all
pause
