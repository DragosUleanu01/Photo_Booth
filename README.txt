Photo Booth este un editor de poze realizat de către inginerul Uleanu Dragoș - Florin.

Instrucțiuni de utilizare:

1. Porniți serverul (rulați API-ul).
2. Deschideți aplicația utilizând executabilul oferit.
3. Creați un account folosind pagina de register (dacă nu aveți deja).
4. Logați-vă folosind datele oferite în pagina de register.
5. Adăugați poze folosind butonul upload image.
6. Adăugați un subiect și o cheie de encriptie.
7. Apăsați pe ... pentru a activa meniul de opțiuni.

Pentru pornirea clientului:

1. Modificati in appsettings.json connection string-ul cu cel oferit de SQL Server
2. dotnet restore
3. dotnet ef database update
4. Modificati launchSettings.json cu "applicationUrl": "https://localhost:7043;http://localhost:5176"