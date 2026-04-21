# Dokumentacja techniczna aplikacji do zarządzania komputerami

## 1. Architektura systemu

Aplikacja składa się z dwóch komponentów:

- **AdminApp (serwer)** – aplikacja WPF pełniąca rolę serwera TCP
- **ClientApp (klient)** – aplikacja WPF łącząca się z serwerem i wykonująca jego polecenia

### Model komunikacji

- Komunikacja odbywa się w oparciu o protokół TCP
- Serwer nasłuchuje na porcie `5000`
- Klient łączy się z serwerem po adresie IP

---

## 2. Technologie

- Język: C#
- Framework: .NET (WPF)
- Komunikacja: `System.Net.Sockets` (TCP)
- UI: WPF

---

## 3. AdminApp (serwer)

### 3.1 Główne komponenty

#### Klasa: `MainWindow`

Odpowiada za:
- logowanie administratora
- uruchomienie serwera
- zarządzanie klientami
- wysyłanie komend

---

### 3.2 Pola

```csharp
TcpListener server;
ObservableCollection<TcpClient> clients = new ObservableCollection<TcpClient>();
bool logged = false;
```
- **server** -> instancja serwera TCP
- **clients** -> lista aktualnie podłączonych klientów
- **logged** -> flaga logowania

---

## 4. ClientApp (klient)

### 4.1 Główne komponenty

#### Klasa: `MainWindow`

Odpowiada za:
- połączenie z serwerem TCP
- nasłuchiwanie komend od serwera
- reakcję na polecenia (BLOCK / UNBLOCK)
- aktualizację interfejsu użytkownika

---

### 4.2 Pola

```csharp
TcpClient client;
```

- **client** -> instancja połączenia TCP z serwerem
