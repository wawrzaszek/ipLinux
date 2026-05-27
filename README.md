# Kalkulator IP (.NET 9 / Avalonia UI)

Pulpitowa aplikacja służąca do obliczeń sieciowych (Kalkulator IP) stworzona z myślą o systemie Linux (działa też w pełni na macOS i Windows). Umożliwia szybkie, automatyczne (w czasie rzeczywistym) wyliczanie maski, adresu sieci, adresu rozgłoszeniowego (broadcast), a także pierwszego/ostatniego hosta oraz klasy adresu IP na podstawie podanego adresu i maski CIDR.

## Wymagania wstępne

Aby uruchomić aplikację, musisz posiadać zainstalowane w systemie środowisko **.NET 9.0 SDK**.
Sprawdzisz to wpisując w terminalu komendę:
```bash
dotnet --version
```
Jeśli środowisko jest zainstalowane, powinieneś zobaczyć wersję `9.x.x`.

### Instalacja na świeżym systemie Ubuntu

Jeśli dopiero co postawiłeś system (Ubuntu/Debian) i nie masz jeszcze narzędzi programistycznych, wklej w terminal poniższe polecenia:

```bash
# 1. Instalacja podstawowych narzędzi systemowych
sudo apt-get update
sudo apt-get install -y wget apt-transport-https software-properties-common

# 2. Pobranie kluczy repozytorium Microsoftu
wget https://packages.microsoft.com/config/ubuntu/$(lsb_release -rs)/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
rm packages-microsoft-prod.deb

# 3. Instalacja właściwego środowiska .NET 9.0 SDK
sudo apt-get update
sudo apt-get install -y dotnet-sdk-9.0
```
## Instrukcja uruchomienia (dewelopersko)

Aby uruchomić aplikację w trybie deweloperskim bezpośrednio z kodu źródłowego:

1. Otwórz terminal.
2. Przejdź do katalogu projektu:
   ```bash
   cd /Users/user/ipLinux
   ```
3. Uruchom aplikację komendą:
   ```bash
   dotnet run
   ```

Aplikacja zbuduje się i otworzy jako natywne okno desktopowe w ciemnym motywie graficznym.

## Budowanie i publikacja (na produkcję)

Jeśli chcesz stworzyć pojedynczy, gotowy plik wykonywalny, z którego można korzystać bez uruchamiania komendy `dotnet run` (np. aby przenieść aplikację na inną maszynę z systemem Linux lub macOS):

**Dla systemu Linux (np. Ubuntu):**
```bash
dotnet publish -c Release -r linux-x64 --self-contained true -p:PublishSingleFile=true
```
Gotowy plik wykonywalny `IpCalculatorLinux` znajdziesz w katalogu `bin/Release/net9.0/linux-x64/publish/`.

**Dla systemu macOS:**
```bash
dotnet publish -c Release -r osx-x64 --self-contained true -p:PublishSingleFile=true
# lub osx-arm64 dla procesorów Apple Silicon (M1/M2/M3)
```

## Architektura
Projekt zbudowany jest w oparciu o framework **Avalonia UI** przy użyciu wzorca projektowego **MVVM** (Model-View-ViewModel). Logika obliczeniowa napisana jest w języku C#, a interfejs zbudowano przy pomocy znaczników XAML.
