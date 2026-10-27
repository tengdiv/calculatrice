# Calculatrice

Calculatrice simple en WPF (C# + XAML, .NET 8).

## Lancer
    dotnet run

## Construire un .exe unique
    dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -o publish

## Structure
- `CalculatorEngine.cs` : la logique de calcul (sans interface)
- `MainWindow.xaml` : l'apparence
- `MainWindow.xaml.cs` : relie boutons et clavier au moteur
- `App.xaml` : couleurs et styles partagés