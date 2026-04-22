# Aplikacja CompAdmin

## Opis
 
Aplikacja umożliwia zdalne blokowanie i odblokowywanie komputerów w sieci lokalnej. Składa się z dwóch programów:
 
- **AdminApp** – używany przez administratora
- **ClientApp** – uruchamiany na komputerach użytkowników
 
Administrator może z poziomu panelu wybrać komputer i zablokować lub odblokować jego ekran, a także ustawić ilość czasu za którą komputer będzie zablokowany.
 
---
 
## Jak to działa
 
1. Administrator uruchamia aplikację **AdminApp** i loguje się.
2. Program uruchamia serwer i czeka na połączenia.
3. Na komputerach użytkowników uruchamiana jest aplikacja **ClientApp**.
4. Klienci automatycznie łączą się z serwerem.
5. Administrator widzi listę podłączonych komputerów.
6. Może wybrać dowolny komputer i:
   - zablokować go
   - odblokować go
   - ustawić ilość czasu za którą komputer będzie zablokowany
 
---
 
## Instrukcja uruchomienia
 
### Krok 1 – uruchomienie panelu administratora
 <img width="731" height="486" alt="image" src="https://github.com/user-attachments/assets/ec56f262-4953-465b-8a20-3fb14ae511f2" />

1. Uruchom aplikację **AdminApp**
2. Wprowadź hasło: `admin`
3. Kliknij przycisk **Zaloguj**
 <img width="727" height="486" alt="image" src="https://github.com/user-attachments/assets/1e3a1b96-fe17-4c12-a3d4-c1f18dd0670d" />

Po zalogowaniu serwer zostanie uruchomiony automatycznie.
 
---
 
### Krok 2 – uruchomienie klientów
 <img width="732" height="486" alt="image" src="https://github.com/user-attachments/assets/068ea4ea-7c29-4bf3-9a13-29bbf3670058" />

1. Uruchom aplikację **ClientApp** na komputerach użytkowników
2. Aplikacja automatycznie połączy się z serwerem
 
---
 
### Krok 3 – zarządzanie komputerami
 
1. W panelu administratora pojawi się lista komputerów
2. Wybierz komputer z listy
3. Kliknij:
- **Zablokuj** – aby zablokować ekran
- **Odblokuj** – aby przywrócić dostęp
 
---
 
## Efekt blokady
 
Po zablokowaniu:
 
- na ekranie użytkownika pojawi się ciemna nakładka
- wyświetli się komunikat:
- użytkownik nie może korzystać z klawiatury i myszki
 
Po odblokowaniu ekran wraca do normalnego stanu.
 
---
 
## Wymagania
 
- komputery muszą być w tej samej sieci
- aplikacje muszą mieć możliwość komunikacji przez port 5000
- system Windows 10/11
 
---
 
## Konfiguracja
 
Jeśli aplikacja nie działa, upewnij się że w **ClientApp** ustawiony jest poprawny adres IP serwera (komputera administratora).
