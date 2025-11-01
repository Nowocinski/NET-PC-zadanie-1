# Ogólny opis

Prosta aplikacji w technologiach Angular, .NET oraz PostgreSQL reprezentująca książkę adresową z listą kontaktów.

## Struktura plików
Struktura plików projektu:
- **backend-zadanie-1** - część backendowa w .NET
  - **API** - podprojekt zawierający kontrolery REST API
  - **Infrastructure** - podprojekt zawierający implementacje repozytoriów, serwisów oraz konfigurację bazy danych
  - **Core** - posiada definicje encji oraz interfejsów do operownia na nich
- **frontend-zadanie-1** - część frontendowa w Angular'ze. Zawartość katalogu src/app:
  - **contacts** - zawiera logikę jedynego z dwóch komponentów aplikacji ContactsComponent
  - **interceptors** - zawiera logikę przechwytywania i modyfikowania żądań HTTP wychodzących z aplikacji
  - **login** - komponent strony logowania
  - **services** - posiada serwisy do autoryzacji oraz pobierania kontaktów z ich kategoriami i subkategoriami

## Instalacja

Aplikacja korzysta z wersji Angulara 20.3, .NET w wersji 9.0 oraz PostgreSQL 2.8.2.

Aby uruchomić projekt należy

```bash
pip install foobar
```