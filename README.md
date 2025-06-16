## 📦 TrainingPlansApi.Tests – Dokumentacja testów jednostkowych

## 🧪 Cel projektu testowego
Ten projekt zawiera testy jednostkowe i integracyjne dla aplikacji TrainingPlansApi. Pokrywa testami:
API i kontrolery MVC
Modele (TrainingPlan, ErrorViewModel)
Walidację danych i błędne scenariusze

## 🧪 Jak uruchomić testy w Visual Studio?
## ✅ Krok 1: Otwórz projekt w Visual Studio
Upewnij się, że masz otwarty plik (.csproj) zawierający zarówno projekt główny (TrainingPlansApi), jak i projekt testów (TrainingPlansApi.Tests).

## ✅ Krok 2: Przejdź do zakładki Test
W górnym pasku kliknij:
Test -> Uruchom wszystkie testy

Możesz też użyć skrótu klawiaturowego:
Ctrl + R, A

## ✅ Alternatywnie: użyj Test Explorer
Otwórz panel: Test -> Test Explorer

Kliknij Run All Tests lub wybierz testy pojedynczo.

## ✅ Interpretacja wyników
Wyniki pojawią się w okienku Test Explorer:
✔️ zielony – test zakończył się sukcesem
❌ czerwony – test zakończył się błędem
⚠️ szary – test został pominięty (np. [Ignore])

Kliknięcie w nazwę testu pokaże pełen ślad stosu (stack trace) i szczegóły błędu.

<details>
📂 Struktura folderów testowych
TrainingPlansApi.Tests/
│
├── ControllersTests/
│   ├── HomeControllerTests.cs
│   ├── PlansControllerTests.cs
│   └── TrainingPlansControllerTests.cs
│
├── ModelsTests/
│   ├── ErrorViewModelTests.cs
│   └── TrainingPlanModelTests.cs
</details>

## 📌 Uwagi
Wszystkie testy bazują na InMemoryDatabase – nie wymagają SQL Servera
Testy są izolowane – każdy działa na świeżej instancji bazy danych
Kod testów pisany jest z użyciem NUnit i Microsoft.AspNetCore.Mvc
