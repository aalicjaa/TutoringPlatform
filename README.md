TutoringPlatform

Platforma rezerwacji korepetycji – projekt inżynierski.

Wymagania systemowe

Do uruchomienia projektu wymagane jest:

Visual Studio 2022 (z obsługą .NET 8 oraz .NET MAUI)
.NET SDK 8
SQL Server (np. SQL Server Express)
SQL Server Management Studio (SSMS)
Node.js (wersja LTS) wraz z npm
(Opcjonalnie) Emulator Android lub urządzenie fizyczne
Konfiguracja bazy danych
Otworzyć SQL Server Management Studio.
Kliknąć prawym przyciskiem myszy na Databases → Restore Database.
Wybrać plik kopii zapasowej: Database/TutoringPlatformDb.bak.
Zatwierdzić przyciskiem OK.

Po przywróceniu baza danych powinna być widoczna w SSMS.

Uruchomienie backendu (API)
Otworzyć rozwiązanie w Visual Studio 2022.
Ustawić projekt startowy: backend/TutoringPlatform.Api.
Sprawdzić connection string w appsettings.json (powinien wskazywać lokalny SQL Server).
Uruchomić projekt (F5 / Run).

Backend uruchamia się domyślnie pod adresami:

https://localhost:7168
http://localhost:5033

Swagger dostępny jest pod: https://localhost:7168/swagger

Uruchomienie aplikacji webowej (React)
Otworzyć terminal w folderze tutoringplatform-web.
Wykonać (tylko za pierwszym razem):
npm install
Uruchomić aplikację:
npm run dev
Aplikacja dostępna będzie pod adresem wyświetlonym w terminalu (np. http://localhost:5173).

Uwaga: Backend API musi być uruchomiony równolegle.

Uruchomienie aplikacji mobilnej (.NET MAUI)
Ustawić projekt startowy: TutoringPlatform.Mobile.
Upewnić się, że backend API jest uruchomiony.
Wybrać emulator Android lub urządzenie fizyczne.
Uruchomić aplikację (Run).

Aplikacja mobilna łączy się z backendem działającym lokalnie.

Kolejność uruchamiania
Wersja webowa:
Uruchomić API
Uruchomić frontend (npm run dev)
Otworzyć aplikację w przeglądarce
Wersja mobilna:
Uruchomić API
Uruchomić projekt Mobile
Uwagi końcowe
Backend musi być uruchomiony przed frontendem i aplikacją mobilną.
W razie problemów z połączeniem należy sprawdzić connection string oraz adres API.
Projekt wykorzystuje wspólny backend dla aplikacji webowej i mobilnej.
