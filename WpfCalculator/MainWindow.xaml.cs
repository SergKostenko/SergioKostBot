using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace WpfCalculator
{
    public partial class MainWindow : Window
    {
        private string currentInput = "0";
        private double? accumulator = null;
        private char? pendingOperator = null;
        private bool isNewEntry = true;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void UpdateDisplay()
        {
            DisplayTextBox.Text = currentInput;
        }

        private void Digit_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                string digit = button.Content.ToString()!;
                if (isNewEntry || currentInput == "0")
                {
                    currentInput = digit;
                }
                else
                {
                    currentInput += digit;
                }
                isNewEntry = false;
                UpdateDisplay();
            }
        }

        private void Decimal_Click(object sender, RoutedEventArgs e)
        {
            string decimalSeparator = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
            if (isNewEntry)
            {
                currentInput = "0" + decimalSeparator;
                isNewEntry = false;
            }
            else if (!currentInput.Contains(decimalSeparator))
            {
                currentInput += decimalSeparator;
            }
            UpdateDisplay();
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            currentInput = "0";
            accumulator = null;
            pendingOperator = null;
            isNewEntry = true;
            UpdateDisplay();
        }

        private void Backspace_Click(object sender, RoutedEventArgs e)
        {
            if (isNewEntry) return;

            if (currentInput.Length > 1)
            {
                currentInput = currentInput[..^1];
                if (currentInput == "-" || currentInput.Length == 0)
                {
                    currentInput = "0";
                    isNewEntry = true;
                }
            }
            else
            {
                currentInput = "0";
                isNewEntry = true;
            }
            UpdateDisplay();
        }

        private void Sign_Click(object sender, RoutedEventArgs e)
        {
            if (currentInput.StartsWith("-", StringComparison.Ordinal))
            {
                currentInput = currentInput[1..];
            }
            else if (currentInput != "0")
            {
                currentInput = "-" + currentInput;
            }
            UpdateDisplay();
        }

        private void Operator_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                char op = MapOperator(button.Content.ToString());

                if (!isNewEntry)
                {
                    if (!ApplyPendingOperation())
                    {
                        ShowErrorAndReset();
                        return;
                    }
                }
                else if (accumulator is null)
                {
                    accumulator = ParseCurrentInput();
                }

                pendingOperator = op;
                isNewEntry = true;
            }
        }

        private void Equals_Click(object sender, RoutedEventArgs e)
        {
            if (!ApplyPendingOperation())
            {
                ShowErrorAndReset();
                return;
            }
            pendingOperator = null;
            isNewEntry = true;
            UpdateDisplay();
        }

        private bool ApplyPendingOperation()
        {
            try
            {
                double current = ParseCurrentInput();
                if (accumulator is null)
                {
                    accumulator = current;
                    currentInput = FormatNumber(accumulator.Value);
                    return true;
                }

                if (pendingOperator is null)
                {
                    accumulator = current;
                    currentInput = FormatNumber(accumulator.Value);
                    return true;
                }

                double result = pendingOperator switch
                {
                    '+' => accumulator.Value + current,
                    '-' => accumulator.Value - current,
                    '*' => accumulator.Value * current,
                    '/' => Math.Abs(current) < double.Epsilon ? throw new DivideByZeroException() : accumulator.Value / current,
                    _ => current
                };

                accumulator = result;
                currentInput = FormatNumber(result);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private static char MapOperator(string? content)
        {
            return content switch
            {
                "×" => '*',
                "÷" => '/',
                "−" => '-',
                "+" => '+',
                "*" => '*',
                "/" => '/',
                "-" => '-',
                _ => '+'
            };
        }

        private double ParseCurrentInput()
        {
            if (double.TryParse(currentInput, NumberStyles.Float, CultureInfo.CurrentCulture, out var value))
            {
                return value;
            }
            throw new FormatException("Invalid number");
        }

        private static string FormatNumber(double value)
        {
            return value.ToString("G12", CultureInfo.CurrentCulture);
        }

        private void ShowErrorAndReset()
        {
            DisplayTextBox.Text = "Error";
            accumulator = null;
            pendingOperator = null;
            isNewEntry = true;
            currentInput = "0";
        }
    }
}