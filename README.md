# TutoringPlatform

Platforma rezerwacji korepetycji – projekt inżynierski.

---

## Spis treści
1. [Opis projektu](#opis-projektu)  
2. [Wymagania systemowe](#wymagania-systemowe)  
3. [Konfiguracja bazy danych](#konfiguracja-bazy-danych)  
4. [Uruchomienie backendu (API)](#uruchomienie-backendu-api)  
5. [Uruchomienie aplikacji webowej (React)](#uruchomienie-aplikacji-webowej-react)  
6. [Uruchomienie aplikacji mobilnej (.NET MAUI)](#uruchomienie-aplikacji-mobilnej-net-maui)  
7. [Kolejność uruchamiania](#kolejnosc-uruchamiania)  
8. [Uwagi końcowe](#uwagi-końcowe)  
9. [Diagram bazy danych](#diagram-bazy-danych)  

---

## Opis projektu
TutoringPlatform to platforma umożliwiająca rezerwację korepetycji.  
Projekt obejmuje:  

- Backend w **.NET 8** (API REST)  
- Aplikację webową w **React**  
- Aplikację mobilną w **.NET MAUI**  
- Wspólną bazę danych w **SQL Server**  

---

## Wymagania systemowe
Do uruchomienia projektu wymagane jest:  

- **Visual Studio 2022** (z obsługą .NET 8 oraz .NET MAUI)  
- **.NET SDK 8**  
- **SQL Server** (np. SQL Server Express)  
- **SQL Server Management Studio (SSMS)**  
- **Node.js** (wersja LTS) wraz z **npm**  
- *(Opcjonalnie)* Emulator Android lub urządzenie fizyczne  

---

## Konfiguracja bazy danych
1. Otworzyć **SQL Server Management Studio**.  
2. Kliknąć prawym przyciskiem myszy na `Databases` → `Restore Database`.  
3. Wybrać plik kopii zapasowej: `Database/TutoringPlatformDb.bak`.  
4. Zatwierdzić przyciskiem **OK**.  

Po przywróceniu baza danych powinna być widoczna w SSMS.  

---

## Uruchomienie backendu (API)
1. Otworzyć rozwiązanie w **Visual Studio 2022**.  
2. Ustawić projekt startowy: `backend/TutoringPlatform.Api`.  
3. Sprawdzić `connection string` w `appsettings.json` (powinien wskazywać lokalny SQL Server).  
4. Uruchomić projekt (**F5 / Run**).  

Backend uruchamia się domyślnie pod adresami:  

- `https://localhost:7168`  
- `http://localhost:5033`  

Swagger dostępny jest pod: `https://localhost:7168/swagger`  

---

## Uruchomienie aplikacji webowej (React)
1. Otworzyć terminal w folderze **tutoringplatform-web**:  

```bash
cd tutoringplatform-web
npm install      # tylko za pierwszym razem
npm run dev
```

2. Aplikacja dostępna będzie pod adresem wyświetlonym w terminalu (np. `http://localhost:5173`).  

> **Uwaga:** Backend API musi być uruchomiony równolegle.  

---

## Uruchomienie aplikacji mobilnej (.NET MAUI)
1. Ustawić projekt startowy: `TutoringPlatform.Mobile`.  
2. Upewnić się, że backend API jest uruchomiony.  
3. Wybrać emulator Android lub urządzenie fizyczne.  
4. Uruchomić aplikację (**Run**).  

Aplikacja mobilna łączy się z backendem działającym lokalnie.  

---

## Kolejność uruchamiania
### Wersja webowa:
1. Uruchomić backend (API).  
2. Uruchomić frontend (`npm run dev`).  
3. Otworzyć aplikację w przeglądarce.  

### Wersja mobilna:
1. Uruchomić backend (API).  
2. Uruchomić projekt Mobile.  

---

## Uwagi końcowe
- Backend musi być uruchomiony przed frontendem i aplikacją mobilną.  
- W razie problemów z połączeniem należy sprawdzić **connection string** oraz adres API.  
- Projekt wykorzystuje wspólny backend dla aplikacji webowej i mobilnej.  

---

## Diagram bazy danych
Plik diagramu znajduje się w repozytorium:  
`Database/TutoringPlatformDb_Diagram.pdf`  

Można go również wyświetlić lub pobrać bezpośrednio z GitHub.  

---

## Technologie
- **Backend:** C#, .NET 8, Entity Framework  
- **Frontend:** React, TypeScript  
- **Mobilna:** .NET MAUI  
- **Baza danych:** SQL Server  
- **Inne:** Node.js, npm
