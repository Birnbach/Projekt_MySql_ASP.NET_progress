# Projekt_MySql_ASP.NET_progress

## Spis treści
* [Informacje ogólne](#general-info)
* [Zastosowanie technologie](#technologies)
* [Uruchamianie programu](#setup)
* [Testowanie](#tests)

## Informacje ogólne
Niniejszy projekt jest aplikacją webową ASP.NET symulującą działanie systemu obsługi biblioteki. Celem projektu jest przedstawienie przykładów wykorzystania niektórych technologii stosowanych w środowisku .NET.
	
## Zastosowanie technologie
Niniejszy projekt zawiera przykłady zastosowania poniższych funkcjonalności i technologii:
* Interfejs użytkownika w postaci strony internetowej
* Funkcjonalność związaną z procesem logowania, tworzenia kont i bezpiecznego przechowywania haseł
* Obsługę bazy danych MySQL z wykorzystaniem LINQ
* Zestaw funkcji pozwalających na pełną obsługę bazy danych (CRUD)
* Komunikację z zewnętrznym API w celu wyświetlania aktualnych informacji o pogodzie - api.openweathermap.org
* Testy jednostkowe niektórych funkcji programu
* Obsługę dziennika zdarzeń (log)
* Wykorzystanie bibliotek zewnętrznych (NuGet)
	
## Uruchamianie programu
Przed uruchomieniem programu najpierw należy wczytać schemat bazy danych korzystając z skryptu SQL. Skrypty znajdują się w katalogu KopieBazy. W celu stworzenia pustej bazy, należy uruchomić odpowiedni skrypt MySQL:

```
mysql> source KopieBazy/BAZAdump.sql;
mysql> use new_schema;
```
W celu utworzenia bazy wypełnionej przykładowymi danymi, należy użyć skryptu:
```
mysql> source KopieBazy/bazadump1.sql;
mysql> use new_schema;
```

Następnie, w uruchomienia aplikacji należy wykonać polecenie:
```
$ dotnet run --project "WebApplication3/Projekt_MySql_ASP.NET.csproj"
```
Dostęp do strony głównej aplikacji możliwy jest pod adresem: 
```
https://localhost:5001/
```

## Testowanie
W celu uruchomienia testów jednostkowych należy wykonać polecenie:
```
$ dotnet test
```

