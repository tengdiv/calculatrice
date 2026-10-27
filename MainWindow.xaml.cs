using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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

        // Caractères tapés : chiffres, opérateurs, virgule, égal
    void Window_PreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        foreach (var c in e.Text)
        {
            switch (c)
            {
                case >= '0' and <= '9': Press(c.ToString()); break;
                case '+': Press("+"); break;
                case '-': Press("-"); break;
                case '*': case 'x': case 'X': Press("×"); break;
                case '/': Press("÷"); break;
                case ',': case '.': Press("."); break;
                case '=': Press("="); break;
                case '%': Press("%"); break;
            }
        }
        e.Handled = true;
    }

    // Touches spéciales : Entrée, Échap, Retour arrière
    void Window_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        switch (e.Key)
        {
            case Key.Enter:  Press("=");  e.Handled = true; break;
            case Key.Escape: Press("C");  e.Handled = true; break;
            case Key.Back:   Press("BS"); e.Handled = true; break;
        }
    }
}