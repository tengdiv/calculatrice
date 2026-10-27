using System.Globalization;

namespace Calculatrice;

public sealed class CalculatorEngine
{
    static readonly CultureInfo Inv = CultureInfo.InvariantCulture;
    const int MaxDigits = 16;

    decimal? _acc;          // valeur accumulée
    char? _op;              // opération en attente : + - × ÷
    string _input = "0";    // saisie en cours (point décimal interne = '.')
    bool _fresh = true;     // la prochaine saisie remplace l'affichage
    bool _error;
    string _expr = "";

    // Texte du grand affichage (virgule française)
    public string Display => _error ? "Erreur" : _input.Replace('.', ',');

    // Petite ligne au-dessus, par exemple "12 + 3 ="
    public string Expression => _expr;

    public void Digit(char d)
    {
        if (_error) Clear();
        if (_fresh) { _input = d.ToString(); _fresh = false; }
        else if (_input == "0") _input = d.ToString();
        else if (_input == "-0") _input = "-" + d;
        else if (_input.Count(char.IsDigit) < MaxDigits) _input += d;
    }

    public void Decimal()
    {
        if (_error) Clear();
        if (_fresh) { _input = "0."; _fresh = false; }
        else if (!_input.Contains('.')) _input += ".";
    }

    public void Operator(char op)
    {
        if (_error) return;

        // Changement d'avis sur l'opérateur sans rien saisir entre-temps
        if (_op != null && _fresh)
        {
            _op = op;
            _expr = $"{Format(_acc!.Value).Replace('.', ',')} {op}";
            return;
        }

        try
        {
            var cur = Parse(_input);
            _acc = _acc == null || _op == null ? cur : Apply(_acc.Value, _op.Value, cur);
        }
        catch (Exception ex) when (ex is DivideByZeroException or OverflowException)
        {
            SetError();
            return;
        }

        _op = op;
        _input = Format(_acc!.Value);
        _expr = $"{_input.Replace('.', ',')} {op}";
        _fresh = true;
    }

    public void Compute()
    {
        if (_error || _op == null || _acc == null) return;
        try
        {
            var cur = Parse(_input);
            var res = Apply(_acc.Value, _op.Value, cur);
            _expr = $"{Format(_acc.Value).Replace('.', ',')} {_op} {Format(cur).Replace('.', ',')} =";
            _input = Format(res);
        }
        catch (Exception ex) when (ex is DivideByZeroException or OverflowException)
        {
            SetError();
            return;
        }

        _acc = null;
        _op = null;
        _fresh = true;
    }

    public void Percent()
    {
        if (_error) return;
        _input = Format(Parse(_input) / 100m);
        _fresh = true;
    }

    public void Negate()
    {
        if (_error || _input == "0") return;
        _input = _input.StartsWith('-') ? _input[1..] : "-" + _input;
    }

    public void Backspace()
    {
        if (_error) { Clear(); return; }
        if (_fresh) return;
        _input = _input[..^1];
        if (_input is "" or "-") { _input = "0"; _fresh = true; }
    }

    public void Clear()
    {
        _acc = null;
        _op = null;
        _input = "0";
        _fresh = true;
        _error = false;
        _expr = "";
    }

    void SetError()
    {
        _error = true;
        _acc = null;
        _op = null;
        _expr = "";
        _fresh = true;
    }

    static decimal Parse(string s) => decimal.Parse(s, NumberStyles.Float, Inv);

    static string Format(decimal d) => Math.Round(d, 10).ToString("0.##########", Inv);

    static decimal Apply(decimal a, char op, decimal b) => op switch
    {
        '+' => a + b,
        '-' => a - b,
        '×' => a * b,
        '÷' => a / b,   // lève DivideByZeroException si b vaut 0
        _ => b
    };
}