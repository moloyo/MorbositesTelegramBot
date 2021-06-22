dotnet publish -c Release .\MorbositesTelegramBot.sln
cd bin\Release\net5.0\publish
heroku container:push web
heroku container:release web