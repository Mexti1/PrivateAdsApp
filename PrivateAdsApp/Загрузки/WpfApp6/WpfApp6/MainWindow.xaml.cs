using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WpfApp6
{
    public partial class MainWindow : Window
    {
        private TextBox[] objectiveFunctionBoxes;
        private TextBox[,] constraintBoxes;
        private TextBox[] rightSideBoxes;
        private int variables;
        private int constraints;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void BtnCreateSimplex_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(txtVariables.Text, out variables) || variables < 1 || variables > 10)
            {
                MessageBox.Show("Введите корректное количество переменных (1-10)", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!int.TryParse(txtConstraints.Text, out constraints) || constraints < 1 || constraints > 10)
            {
                MessageBox.Show("Введите корректное количество ограничений (1-10)", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            CreateSimplexInput();
            btnSolveSimplex.IsEnabled = true;
        }

        private void CreateSimplexInput()
        {
            pnlSimplexContent.Children.Clear();

            // Целевая функция
            Border objectiveBorder = new Border
            {
                Background = Brushes.White,
                BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#BDC3C7")),
                BorderThickness = new Thickness(2),
                Padding = new Thickness(15),
                Margin = new Thickness(0, 0, 0, 15)
            };

            StackPanel objectivePanel = new StackPanel();

            TextBlock objectiveTitle = new TextBlock
            {
                Text = "Целевая функция F(x)",
                FontSize = 16,
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2C3E50")),
                Margin = new Thickness(0, 0, 0, 10)
            };
            objectivePanel.Children.Add(objectiveTitle);

            StackPanel objFuncPanel = new StackPanel { Orientation = Orientation.Horizontal };
            TextBlock fLabel = new TextBlock
            {
                Text = "F = ",
                FontSize = 14,
                FontWeight = FontWeights.Bold,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0, 0, 10, 0),
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2C3E50"))
            };
            objFuncPanel.Children.Add(fLabel);

            objectiveFunctionBoxes = new TextBox[variables];
            for (int i = 0; i < variables; i++)
            {
                if (i > 0)
                {
                    TextBlock plusSign = new TextBlock
                    {
                        Text = "+",
                        FontSize = 14,
                        VerticalAlignment = VerticalAlignment.Center,
                        Margin = new Thickness(8, 0, 8, 0)
                    };
                    objFuncPanel.Children.Add(plusSign);
                }

                objectiveFunctionBoxes[i] = CreateTextBox();
                objFuncPanel.Children.Add(objectiveFunctionBoxes[i]);

                TextBlock varLabel = new TextBlock
                {
                    Text = $"·x{i + 1}",
                    FontSize = 14,
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(5, 0, 0, 0),
                    Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#34495E"))
                };
                objFuncPanel.Children.Add(varLabel);
            }

            TextBlock arrow = new TextBlock
            {
                Text = rbMax.IsChecked == true ? "→ max" : "→ min",
                FontSize = 14,
                FontWeight = FontWeights.Bold,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(15, 0, 0, 0),
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#27AE60"))
            };
            objFuncPanel.Children.Add(arrow);

            objectivePanel.Children.Add(objFuncPanel);
            objectiveBorder.Child = objectivePanel;
            pnlSimplexContent.Children.Add(objectiveBorder);

            // Ограничения
            Border constraintsBorder = new Border
            {
                Background = Brushes.White,
                BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#BDC3C7")),
                BorderThickness = new Thickness(2),
                Padding = new Thickness(15),
                Margin = new Thickness(0, 0, 0, 15)
            };

            StackPanel constraintsMainPanel = new StackPanel();

            TextBlock constraintsTitle = new TextBlock
            {
                Text = "Система ограничений",
                FontSize = 16,
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2C3E50")),
                Margin = new Thickness(0, 0, 0, 10)
            };
            constraintsMainPanel.Children.Add(constraintsTitle);

            constraintBoxes = new TextBox[constraints, variables];
            rightSideBoxes = new TextBox[constraints];

            for (int i = 0; i < constraints; i++)
            {
                StackPanel constraintRow = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 5, 0, 5) };

                for (int j = 0; j < variables; j++)
                {
                    if (j > 0)
                    {
                        TextBlock plusSign = new TextBlock
                        {
                            Text = "+",
                            FontSize = 14,
                            VerticalAlignment = VerticalAlignment.Center,
                            Margin = new Thickness(8, 0, 8, 0)
                        };
                        constraintRow.Children.Add(plusSign);
                    }

                    constraintBoxes[i, j] = CreateTextBox();
                    constraintRow.Children.Add(constraintBoxes[i, j]);

                    TextBlock varLabel = new TextBlock
                    {
                        Text = $"·x{j + 1}",
                        FontSize = 14,
                        VerticalAlignment = VerticalAlignment.Center,
                        Margin = new Thickness(5, 0, 0, 0),
                        Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#34495E"))
                    };
                    constraintRow.Children.Add(varLabel);
                }

                TextBlock leqSign = new TextBlock
                {
                    Text = "≤",
                    FontSize = 16,
                    FontWeight = FontWeights.Bold,
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(15, 0, 10, 0),
                    Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3498DB"))
                };
                constraintRow.Children.Add(leqSign);

                rightSideBoxes[i] = CreateTextBox();
                constraintRow.Children.Add(rightSideBoxes[i]);

                constraintsMainPanel.Children.Add(constraintRow);
            }

            TextBlock nonNegativeLabel = new TextBlock
            {
                Text = "x₁, x₂, ... ≥ 0",
                FontSize = 13,
                FontStyle = FontStyles.Italic,
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#7F8C8D")),
                Margin = new Thickness(0, 15, 0, 0)
            };
            constraintsMainPanel.Children.Add(nonNegativeLabel);

            constraintsBorder.Child = constraintsMainPanel;
            pnlSimplexContent.Children.Add(constraintsBorder);
        }

        private TextBox CreateTextBox()
        {
            return new TextBox
            {
                Width = 60,
                Height = 30,
                Margin = new Thickness(2),
                Text = "0",
                TextAlignment = TextAlignment.Center,
                VerticalContentAlignment = VerticalAlignment.Center,
                FontSize = 12
            };
        }

        private void BtnSolveSimplex_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                double[] c = new double[variables];
                for (int j = 0; j < variables; j++)
                {
                    if (!double.TryParse(objectiveFunctionBoxes[j].Text, out c[j]))
                    {
                        MessageBox.Show($"Ошибка в коэффициенте целевой функции x{j + 1}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }

                double[,] A = new double[constraints, variables];
                double[] b = new double[constraints];

                for (int i = 0; i < constraints; i++)
                {
                    for (int j = 0; j < variables; j++)
                    {
                        if (!double.TryParse(constraintBoxes[i, j].Text, out A[i, j]))
                        {
                            MessageBox.Show($"Ошибка в ограничении {i + 1}, переменная x{j + 1}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }
                    }

                    if (!double.TryParse(rightSideBoxes[i].Text, out b[i]))
                    {
                        MessageBox.Show($"Ошибка в правой части ограничения {i + 1}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    if (b[i] < 0)
                    {
                        MessageBox.Show($"Правая часть ограничения {i + 1} должна быть ≥ 0", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }

                bool isMax = rbMax.IsChecked == true;
                var result = SolveSimplex(c, A, b, isMax);
                ShowSimplexResult(result);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при решении: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private SimplexResult SolveSimplex(double[] c, double[,] A, double[] b, bool isMax)
        {
            var result = new SimplexResult();
            int m = constraints;
            int n = variables;

            if (!isMax)
            {
                for (int j = 0; j < n; j++)
                    c[j] = -c[j];
            }

            int totalVars = n + m;
            double[,] tableau = new double[m + 1, totalVars + 1];

            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++)
                    tableau[i, j] = A[i, j];

                tableau[i, n + i] = 1;
                tableau[i, totalVars] = b[i];
            }

            for (int j = 0; j < n; j++)
                tableau[m, j] = -c[j];

            int[] basis = new int[m];
            for (int i = 0; i < m; i++)
                basis[i] = n + i;

            result.Tables.Add(CopyTableau(tableau, basis, totalVars, m));

            int iteration = 0;
            int maxIterations = 100;

            while (iteration < maxIterations)
            {
                int pivotCol = -1;
                double minValue = 0;
                for (int j = 0; j < totalVars; j++)
                {
                    if (tableau[m, j] < minValue)
                    {
                        minValue = tableau[m, j];
                        pivotCol = j;
                    }
                }

                if (pivotCol == -1)
                    break;

                int pivotRow = -1;
                double minRatio = double.MaxValue;
                for (int i = 0; i < m; i++)
                {
                    if (tableau[i, pivotCol] > 0)
                    {
                        double ratio = tableau[i, totalVars] / tableau[i, pivotCol];
                        if (ratio < minRatio)
                        {
                            minRatio = ratio;
                            pivotRow = i;
                        }
                    }
                }

                if (pivotRow == -1)
                {
                    result.IsUnbounded = true;
                    return result;
                }

                result.PivotElements.Add(new Tuple<int, int>(pivotRow, pivotCol));

                double pivotElement = tableau[pivotRow, pivotCol];

                for (int j = 0; j <= totalVars; j++)
                    tableau[pivotRow, j] /= pivotElement;

                for (int i = 0; i <= m; i++)
                {
                    if (i != pivotRow)
                    {
                        double factor = tableau[i, pivotCol];
                        for (int j = 0; j <= totalVars; j++)
                            tableau[i, j] -= factor * tableau[pivotRow, j];
                    }
                }

                basis[pivotRow] = pivotCol;
                result.Tables.Add(CopyTableau(tableau, basis, totalVars, m));

                iteration++;
            }

            result.Solution = new double[n];
            for (int i = 0; i < m; i++)
            {
                if (basis[i] < n)
                    result.Solution[basis[i]] = tableau[i, totalVars];
            }

            result.OptimalValue = tableau[m, totalVars];
            if (!isMax)
                result.OptimalValue = -result.OptimalValue;

            result.IsOptimal = true;
            return result;
        }

        private SimplexTable CopyTableau(double[,] tableau, int[] basis, int totalVars, int m)
        {
            var table = new SimplexTable
            {
                Data = (double[,])tableau.Clone(),
                Basis = (int[])basis.Clone(),
                Rows = m + 1,
                Cols = totalVars + 1
            };
            return table;
        }

        private void ShowSimplexResult(SimplexResult result)
        {
            if (result.IsUnbounded)
            {
                MessageBox.Show("Задача не имеет решения (целевая функция неограничена)", "Результат", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (!result.IsOptimal)
            {
                MessageBox.Show("Не удалось найти оптимальное решение", "Результат", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            for (int t = 0; t < result.Tables.Count; t++)
            {
                var table = result.Tables[t];

                Border tableBorder = new Border
                {
                    Background = Brushes.White,
                    BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#BDC3C7")),
                    BorderThickness = new Thickness(2),
                    Padding = new Thickness(15),
                    Margin = new Thickness(0, 0, 0, 15)
                };

                StackPanel tablePanel = new StackPanel();

                TextBlock tableTitle = new TextBlock
                {
                    Text = t == 0 ? "Начальная симплекс-таблица" : $"Симплекс-таблица {t}",
                    FontSize = 16,
                    FontWeight = FontWeights.Bold,
                    Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2C3E50")),
                    Margin = new Thickness(0, 0, 0, 10)
                };
                tablePanel.Children.Add(tableTitle);

                Grid grid = CreateSimplexTableGrid(table, t > 0 ? result.PivotElements[t - 1] : null);
                tablePanel.Children.Add(grid);

                tableBorder.Child = tablePanel;
                pnlSimplexContent.Children.Add(tableBorder);
            }

            // Итоговый результат
            StackPanel horizontalPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 10, 0, 0)
            };

            Border resultBorder = new Border
            {
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ECF0F1")),
                BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#BDC3C7")),
                BorderThickness = new Thickness(2),
                CornerRadius = new CornerRadius(8),
                Padding = new Thickness(25),
                MinWidth = 350
            };

            StackPanel resultPanel = new StackPanel();

            TextBlock resultTitle = new TextBlock
            {
                Text = "📊 ОПТИМАЛЬНОЕ РЕШЕНИЕ",
                FontSize = 20,
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2C3E50")),
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 0, 0, 20)
            };
            resultPanel.Children.Add(resultTitle);

            Border sep1 = new Border
            {
                Height = 2,
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#BDC3C7")),
                Margin = new Thickness(0, 0, 0, 15)
            };
            resultPanel.Children.Add(sep1);

            TextBlock solutionLabel = new TextBlock
            {
                Text = "Оптимальный план:",
                FontSize = 14,
                FontWeight = FontWeights.SemiBold,
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#7F8C8D")),
                Margin = new Thickness(0, 0, 0, 10)
            };
            resultPanel.Children.Add(solutionLabel);

            for (int i = 0; i < result.Solution.Length; i++)
            {
                TextBlock varValue = new TextBlock
                {
                    Text = $"x{i + 1} = {result.Solution[i]:F2}",
                    FontSize = 16,
                    FontWeight = FontWeights.Bold,
                    Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3498DB")),
                    Margin = new Thickness(20, 5, 0, 5)
                };
                resultPanel.Children.Add(varValue);
            }

            Border sep2 = new Border
            {
                Height = 2,
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#BDC3C7")),
                Margin = new Thickness(0, 15, 0, 15)
            };
            resultPanel.Children.Add(sep2);

            TextBlock optValueLabel = new TextBlock
            {
                Text = "Значение целевой функции:",
                FontSize = 14,
                FontWeight = FontWeights.SemiBold,
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#7F8C8D")),
                Margin = new Thickness(0, 0, 0, 10)
            };
            resultPanel.Children.Add(optValueLabel);

            TextBlock optValue = new TextBlock
            {
                Text = $"F = {result.OptimalValue:F2}",
                FontSize = 26,
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E67E22")),
                HorizontalAlignment = HorizontalAlignment.Center
            };
            resultPanel.Children.Add(optValue);

            resultBorder.Child = resultPanel;
            horizontalPanel.Children.Add(resultBorder);
            pnlSimplexContent.Children.Add(horizontalPanel);
        }

        private Grid CreateSimplexTableGrid(SimplexTable table, Tuple<int, int> pivot)
        {
            Grid grid = new Grid();
            int m = table.Rows - 1;
            int n = table.Cols - 1;
            int totalVars = variables + constraints;

            for (int i = 0; i <= m + 1; i++)
                grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            for (int j = 0; j <= totalVars + 2; j++)
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            AddTableCell(grid, "Базис", 0, 0, true, "#ECF0F1");

            for (int j = 0; j < variables; j++)
                AddTableCell(grid, $"x{j + 1}", 0, j + 1, true, "#ECF0F1");

            for (int j = 0; j < constraints; j++)
                AddTableCell(grid, $"x{variables + j + 1}", 0, variables + j + 1, true, "#ECF0F1");

            AddTableCell(grid, "Св.член", 0, totalVars + 1, true, "#ECF0F1");

            for (int i = 0; i < m; i++)
            {
                string basisVar = $"x{table.Basis[i] + 1}";
                AddTableCell(grid, basisVar, i + 1, 0, true, "#ECF0F1");
            }

            AddTableCell(grid, "F", m + 1, 0, true, "#D5DBDB");

            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    bool isPivot = pivot != null && i == pivot.Item1 && j == pivot.Item2;
                    string bgColor = isPivot ? "#F9E79F" : "#FFFFFF";
                    AddTableCell(grid, table.Data[i, j].ToString("F2"), i + 1, j + 1, false, bgColor);
                }
                AddTableCell(grid, table.Data[i, n].ToString("F2"), i + 1, totalVars + 1, false, "#D5F4E6");
            }

            for (int j = 0; j < n; j++)
                AddTableCell(grid, table.Data[m, j].ToString("F2"), m + 1, j + 1, false, "#D5F4E6");

            AddTableCell(grid, table.Data[m, n].ToString("F2"), m + 1, totalVars + 1, false, "#AED6F1");

            return grid;
        }

        private void AddTableCell(Grid grid, string text, int row, int col, bool isHeader, string bgColor)
        {
            TextBlock tb = new TextBlock
            {
                Text = text,
                Width = 65,
                Height = 30,
                Margin = new Thickness(2),
                TextAlignment = TextAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                FontWeight = isHeader ? FontWeights.Bold : FontWeights.Normal,
                FontSize = 11,
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(bgColor)),
                Padding = new Thickness(5),
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2C3E50"))
            };

            Grid.SetRow(tb, row);
            Grid.SetColumn(tb, col);
            grid.Children.Add(tb);
        }

        private void BtnResetSimplex_Click(object sender, RoutedEventArgs e)
        {
            pnlSimplexContent.Children.Clear();
            txtVariables.Text = "2";
            txtConstraints.Text = "3";
            btnSolveSimplex.IsEnabled = false;
            rbMax.IsChecked = true;
            txtTitleSimplex.Text = "Задача линейного программирования - Симплекс-метод";
        }
    }

    public class SimplexResult
    {
        public bool IsOptimal { get; set; }
        public bool IsUnbounded { get; set; }
        public double[] Solution { get; set; }
        public double OptimalValue { get; set; }
        public List<SimplexTable> Tables { get; set; } = new List<SimplexTable>();
        public List<Tuple<int, int>> PivotElements { get; set; } = new List<Tuple<int, int>>();
    }

    public class SimplexTable
    {
        public double[,] Data { get; set; }
        public int[] Basis { get; set; }
        public int Rows { get; set; }
        public int Cols { get; set; }
    }
}
