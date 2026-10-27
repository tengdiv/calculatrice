using System.Windows;
using System.Windows.Controls;

namespace Calculatrice;

public partial class MainWindow : Window
{
    readonly CalculatorEngine _calc = new();

    public MainWindow()
    {
        InitializeComponent();
    }

    // Appelée à chaque clic sur un bouton du clavier
    void Key_Click(object sender, RoutedEventArgs e)
    {
        if (e.OriginalSource is Button { Tag: string tag })
            Press(tag);
    }

    // Point d'entrée unique : chaque touche passe par ici
    void Press(string key)
    {
        switch (key)
        {
            case "C":   _calc.Clear(); break;
            case "BS":  _calc.Backspace(); break;
            case "%":   _calc.Percent(); break;
            case "NEG": _calc.Negate(); break;
            case ".":   _calc.Decimal(); break;
            case "=":   _calc.Compute(); break;
            case "+" or "-" or "×" or "÷":
                _calc.Operator(key[0]);
                break;
            default:
                if (key.Length == 1 && char.IsDigit(key[0]))
                    _calc.Digit(key[0]);
                break;
        }

        // On relit l'état du moteur et on rafraîchit l'écran
        DisplayText.Text = _calc.Display;
        ExprText.Text = _calc.Expression;
    }
}