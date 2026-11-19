using System;
using System.Drawing;
using System.Windows.Forms;

namespace WindowsCalculator
{
    public partial class CalculatorForm : Form
    {
        // Calculator state
        private string currentInput = "0";
        private string previousInput = "";
        private string currentOperator = null;
        private bool shouldResetDisplay = false;
        private bool isScientificMode = false;

        // UI Colors (Windows 11 style)
        private readonly Color bgDark = Color.FromArgb(32, 32, 32);
        private readonly Color bgMedium = Color.FromArgb(45, 45, 45);
        private readonly Color bgLight = Color.FromArgb(60, 60, 60);
        private readonly Color fgPrimary = Color.White;
        private readonly Color fgSecondary = Color.FromArgb(204, 204, 204);
        private readonly Color accentOrange = Color.FromArgb(255, 149, 0);
        private readonly Color accentOrangeHover = Color.FromArgb(204, 119, 0);
        private readonly Color btnDark = Color.FromArgb(51, 51, 51);
        private readonly Color btnDarkHover = Color.FromArgb(74, 74, 74);
        private readonly Color btnGray = Color.FromArgb(80, 80, 80);
        private readonly Color btnGrayHover = Color.FromArgb(104, 104, 104);

        // UI Controls
        private Label lblHistory;
        private TextBox txtDisplay;
        private TableLayoutPanel pnlButtons;
        private MenuStrip menuStrip;

        public CalculatorForm()
        {
            InitializeComponent();
            SetupUI();
            SetupKeyboardShortcuts();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            // Form settings
            this.Text = "Calculator";
            this.Size = new Size(360, 520);
            this.MinimumSize = new Size(360, 520);
            this.MaximumSize = new Size(720, 520); // Allow widening for scientific mode
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = bgDark;
            this.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            this.ForeColor = fgPrimary;

            // Menu strip
            menuStrip = new MenuStrip
            {
                BackColor = bgMedium,
                ForeColor = fgPrimary,
                RenderMode = ToolStripRenderMode.Professional,
                Font = new Font("Segoe UI", 9F)
            };
            menuStrip.Renderer = new ToolStripProfessionalRenderer(new CustomColorTable());

            var viewMenu = new ToolStripMenuItem("View");
            viewMenu.DropDownItems.Add("Standard", null, (s, e) => SwitchMode(false));
            viewMenu.DropDownItems.Add("Scientific", null, (s, e) => SwitchMode(true));
            viewMenu.DropDownItems.Add(new ToolStripSeparator());
            viewMenu.DropDownItems.Add("History", null, (s, e) => { /* TODO */ });
            viewMenu.DropDownItems.Add("Memory", null, (s, e) => { /* TODO */ });
            menuStrip.Items.Add(viewMenu);

            var editMenu = new ToolStripMenuItem("Edit");
            editMenu.DropDownItems.Add("Copy", null, (s, e) => CopyToClipboard());
            editMenu.DropDownItems.Add("Paste", null, (s, e) => PasteFromClipboard());
            editMenu.DropDownItems.Add(new ToolStripSeparator());
            editMenu.DropDownItems.Add("Clear History", null, (s, e) => { /* TODO */ });
            menuStrip.Items.Add(editMenu);

            var helpMenu = new ToolStripMenuItem("Help");
            helpMenu.DropDownItems.Add("About", null, (s, e) => ShowAbout());
            menuStrip.Items.Add(helpMenu);

            this.MainMenuStrip = menuStrip;
            this.Controls.Add(menuStrip);

            // History label
            lblHistory = new Label
            {
                Text = "",
                Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point),
                ForeColor = fgSecondary,
                BackColor = bgDark,
                Dock = DockStyle.Top,
                Height = 30,
                TextAlign = ContentAlignment.MiddleRight,
                Padding = new Padding(0, 0, 12, 0)
            };
            this.Controls.Add(lblHistory);

            // Display textbox
            txtDisplay = new TextBox
            {
                Text = "0",
                Font = new Font("Segoe UI", 36F, FontStyle.Regular, GraphicsUnit.Point),
                ForeColor = fgPrimary,
                BackColor = bgDark,
                BorderStyle = BorderStyle.None,
                ReadOnly = true,
                Dock = DockStyle.Top,
                Height = 80,
                TextAlign = HorizontalAlignment.Right,
                Padding = new Padding(12, 10, 12, 0)
            };
            this.Controls.Add(txtDisplay);

            // Button panel
            pnlButtons = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                BackColor = bgDark,
                ColumnCount = 4,
                RowCount = 6,
                Padding = new Padding(10)
            };
            pnlButtons.SuspendLayout();
            for (int i = 0; i < 4; i++)
                pnlButtons.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            for (int i = 0; i < 6; i++)
                pnlButtons.RowStyles.Add(new RowStyle(SizeType.Percent, 16.66F));

            this.Controls.Add(pnlButtons);

            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private void SetupUI()
        {
            CreateStandardButtons();
        }

        private void CreateStandardButtons()
        {
            pnlButtons.Controls.Clear();
            pnlButtons.RowCount = 6;
            pnlButtons.RowStyles.Clear();
            for (int i = 0; i < 6; i++)
                pnlButtons.RowStyles.Add(new RowStyle(SizeType.Percent, 16.66F));

            string[,] buttons = {
                { "%", "CE", "C", "⌫" },
                { "¹/x", "x²", "√x", "÷" },
                { "7", "8", "9", "×" },
                { "4", "5", "6", "−" },
                { "1", "2", "3", "+" },
                { "±", "0", ".", "=" }
            };

            for (int row = 0; row < 6; row++)
            {
                for (int col = 0; col < 4; col++)
                {
                    string text = buttons[row, col];
                    var btn = CreateButton(text, row, col);
                    pnlButtons.Controls.Add(btn, col, row);
                }
            }
        }

        private void CreateScientificButtons()
        {
            pnlButtons.Controls.Clear();
            pnlButtons.RowCount = 8;
            pnlButtons.RowStyles.Clear();
            for (int i = 0; i < 8; i++)
                pnlButtons.RowStyles.Add(new RowStyle(SizeType.Percent, 12.5F));

            string[,] buttons = {
                { "%", "CE", "C", "⌫" },
                { "sin", "cos", "tan", "÷" },
                { "sin⁻¹", "cos⁻¹", "tan⁻¹", "×" },
                { "log", "ln", "x!", "−" },
                { "xʸ", "x²", "√x", "+" },
                { "π", "e", "±", "=" },
                { "7", "8", "9", "" },
                { "4", "5", "6", "" },
                { "1", "2", "3", "" },
                { "0", ".", "", "" }
            };

            // We'll use a more flexible layout for scientific mode
            // Row 0
            AddButton("%", 0, 0);
            AddButton("CE", 0, 1);
            AddButton("C", 0, 2);
            AddButton("⌫", 0, 3);

            // Row 1
            AddButton("sin", 1, 0);
            AddButton("cos", 1, 1);
            AddButton("tan", 1, 2);
            AddButton("÷", 1, 3);

            // Row 2
            AddButton("sin⁻¹", 2, 0);
            AddButton("cos⁻¹", 2, 1);
            AddButton("tan⁻¹", 2, 2);
            AddButton("×", 2, 3);

            // Row 3
            AddButton("log", 3, 0);
            AddButton("ln", 3, 1);
            AddButton("x!", 3, 2);
            AddButton("−", 3, 3);

            // Row 4
            AddButton("xʸ", 4, 0);
            AddButton("x²", 4, 1);
            AddButton("√x", 4, 2);
            AddButton("+", 4, 3);

            // Row 5
            AddButton("π", 5, 0);
            AddButton("e", 5, 1);
            AddButton("±", 5, 2);
            AddButton("=", 5, 3);

            // Row 6-7: Number pad (spanning)
            // 7,8,9
            AddButton("7", 6, 0);
            AddButton("8", 6, 1);
            AddButton("9", 6, 2);
            // 4,5,6
            AddButton("4", 7, 0);
            AddButton("5", 7, 1);
            AddButton("6", 7, 2);
            // 1,2,3
            AddButton("1", 8, 0);
            AddButton("2", 8, 1);
            AddButton("3", 8, 2);
            // 0, .
            var btn0 = CreateButton("0", 9, 0);
            pnlButtons.SetColumnSpan(btn0, 2);
            pnlButtons.Controls.Add(btn0, 0, 9);
            AddButton(".", 9, 2);

            // Update row count
            pnlButtons.RowCount = 10;
            pnlButtons.RowStyles.Clear();
            for (int i = 0; i < 10; i++)
                pnlButtons.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));
        }

        private void AddButton(string text, int row, int col)
        {
            var btn = CreateButton(text, row, col);
            pnlButtons.Controls.Add(btn, col, row);
        }

        private Button CreateButton(string text, int row, int col)
        {
            bool isOperator = text == "÷" || text == "×" || text == "−" || text == "+" || text == "=";
            bool isFunction = text == "%" || text == "CE" || text == "C" || text == "⌫" ||
                             text == "¹/x" || text == "x²" || text == "√x" || text == "±" ||
                             text == "sin" || text == "cos" || text == "tan" ||
                             text == "sin⁻¹" || text == "cos⁻¹" || text == "tan⁻¹" ||
                             text == "log" || text == "ln" || text == "x!" || text == "xʸ" ||
                             text == "π" || text == "e";

            Color bg = isOperator ? accentOrange : (isFunction ? bgLight : btnDark);
            Color hoverBg = isOperator ? accentOrangeHover : (isFunction ? btnGrayHover : btnDarkHover);
            Font font = new Font("Segoe UI", isOperator ? 16F : 14F, isOperator || (!isFunction && char.IsDigit(text[0])) ? FontStyle.Bold : FontStyle.Regular, GraphicsUnit.Point);

            var btn = new Button
            {
                Text = text,
                Font = font,
                ForeColor = fgPrimary,
                BackColor = bg,
                FlatStyle = FlatStyle.Flat,
                Dock = DockStyle.Fill,
                Margin = new Padding(2),
                Cursor = Cursors.Hand,
                Tag = text
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.MouseOverBackColor = hoverBg;
            btn.FlatAppearance.MouseDownBackColor = Color.FromArgb(
                Math.Min(hoverBg.R + 20, 255),
                Math.Min(hoverBg.G + 20, 255),
                Math.Min(hoverBg.B + 20, 255)
            );
            btn.Click += Btn_Click;

            return btn;
        }

        private void SwitchMode(bool scientific)
        {
            isScientificMode = scientific;
            if (scientific)
            {
                this.Width = 640;
                CreateScientificButtons();
            }
            else
            {
                this.Width = 360;
                CreateStandardButtons();
            }
            this.CenterToScreen();
        }

        private void Btn_Click(object sender, EventArgs e)
        {
            var btn = sender as Button;
            string text = btn.Tag as string;
            ProcessInput(text);
        }

        private void ProcessInput(string input)
        {
            // Guard against null or empty input to avoid null reference exceptions.
            if (string.IsNullOrEmpty(input))
                return;

            if (char.IsDigit(input[0]) || input == ".")
            {
                InputNumber(input);
            }
            else if (input == "+" || input == "−" || input == "×" || input == "÷" || input == "^")
            {
                InputOperator(input);
            }
            else
            {
                switch (input)
                {
                    case "=": Calculate(); break;
                    case "C": ClearAll(); break;
                    case "CE": ClearEntry(); break;
                    case "⌫": Backspace(); break;
                    case "±": Negate(); break;
                    case "%": Percent(); break;
                    case "¹/x": Reciprocal(); break;
                    case "x²": Square(); break;
                    case "√x": Sqrt(); break;
                    case "sin": TrigFunction("sin"); break;
                    case "cos": TrigFunction("cos"); break;
                    case "tan": TrigFunction("tan"); break;
                    case "sin⁻¹": InverseTrig("sin"); break;
                    case "cos⁻¹": InverseTrig("cos"); break;
                    case "tan⁻¹": InverseTrig("tan"); break;
                    case "log": Log10(); break;
                    case "ln": Ln(); break;
                    case "x!": Factorial(); break;
                    case "xʸ": InputOperator("^"); break;
                    case "π": InputConstant(Math.PI); break;
                    case "e": InputConstant(Math.E); break;
                }
            }
            UpdateDisplay();
        }

        private void InputNumber(string num)
        {
            if (shouldResetDisplay)
            {
                currentInput = "0";
                shouldResetDisplay = false;
            }

            if (num == ".")
            {
                if (!currentInput.Contains("."))
                    currentInput += ".";
            }
            else
            {
                if (currentInput == "0")
                    currentInput = num;
                else
                    currentInput += num;
            }
        }

        private void InputOperator(string op)
        {
            if (currentOperator != null && !shouldResetDisplay)
                Calculate();

            previousInput = currentInput;
            currentOperator = op;
            shouldResetDisplay = true;
            UpdateHistory();
        }

        private void Calculate()
        {
            if (currentOperator == null || shouldResetDisplay)
                return;

            try
            {
                double prev = double.Parse(previousInput);
                double curr = double.Parse(currentInput);
                double result = 0;

                switch (currentOperator)
                {
                    case "+": result = prev + curr; break;
                    case "−": result = prev - curr; break;
                    case "×": result = prev * curr; break;
                    case "÷":
                        if (curr == 0) { ShowError("Cannot divide by zero"); return; }
                        result = prev / curr; break;
                    case "^": result = Math.Pow(prev, curr); break;
                }

                currentInput = FormatResult(result);
                previousInput = "";
                currentOperator = null;
                shouldResetDisplay = true;
                lblHistory.Text = "";
            }
            catch
            {
                ShowError("Error");
            }
        }

        private void ClearAll()
        {
            currentInput = "0";
            previousInput = "";
            currentOperator = null;
            shouldResetDisplay = false;
            lblHistory.Text = "";
        }

        private void ClearEntry()
        {
            currentInput = "0";
        }

        private void Backspace()
        {
            if (currentInput.Length > 1)
                currentInput = currentInput.Substring(0, currentInput.Length - 1);
            else
                currentInput = "0";
        }

        private void Negate()
        {
            try
            {
                double value = double.Parse(currentInput);
                currentInput = FormatResult(-value);
            }
            catch { }
        }

        private void Percent()
        {
            try
            {
                double value = double.Parse(currentInput);
                currentInput = FormatResult(value / 100);
            }
            catch { }
        }

        private void Reciprocal()
        {
            try
            {
                double value = double.Parse(currentInput);
                if (value == 0) { ShowError("Cannot divide by zero"); return; }
                currentInput = FormatResult(1 / value);
            }
            catch { }
        }

        private void Square()
        {
            try
            {
                double value = double.Parse(currentInput);
                currentInput = FormatResult(value * value);
            }
            catch { ShowError("Error"); }
        }

        private void Sqrt()
        {
            try
            {
                double value = double.Parse(currentInput);
                if (value < 0) { ShowError("Invalid input"); return; }
                currentInput = FormatResult(Math.Sqrt(value));
            }
            catch { ShowError("Error"); }
        }

        private void TrigFunction(string func)
        {
            try
            {
                double value = double.Parse(currentInput);
                double result = 0;
                switch (func)
                {
                    case "sin": result = Math.Sin(value * Math.PI / 180); break;
                    case "cos": result = Math.Cos(value * Math.PI / 180); break;
                    case "tan": result = Math.Tan(value * Math.PI / 180); break;
                }
                currentInput = FormatResult(result);
            }
            catch { ShowError("Error"); }
        }

        private void InverseTrig(string func)
        {
            try
            {
                double value = double.Parse(currentInput);
                double result = 0;
                switch (func)
                {
                    case "sin":
                        if (value < -1 || value > 1) { ShowError("Invalid input"); return; }
                        result = Math.Asin(value) * 180 / Math.PI; break;
                    case "cos":
                        if (value < -1 || value > 1) { ShowError("Invalid input"); return; }
                        result = Math.Acos(value) * 180 / Math.PI; break;
                    case "tan": result = Math.Atan(value) * 180 / Math.PI; break;
                }
                currentInput = FormatResult(result);
            }
            catch { ShowError("Error"); }
        }

        private void Log10()
        {
            try
            {
                double value = double.Parse(currentInput);
                if (value <= 0) { ShowError("Invalid input"); return; }
                currentInput = FormatResult(Math.Log10(value));
            }
            catch { ShowError("Error"); }
        }

        private void Ln()
        {
            try
            {
                double value = double.Parse(currentInput);
                if (value <= 0) { ShowError("Invalid input"); return; }
                currentInput = FormatResult(Math.Log(value));
            }
            catch { ShowError("Error"); }
        }

        private void Factorial()
        {
            try
            {
                int value = (int)double.Parse(currentInput);
                if (value < 0 || value > 170) { ShowError("Invalid input"); return; }
                double result = 1;
                for (int i = 2; i <= value; i++) result *= i;
                currentInput = FormatResult(result);
            }
            catch { ShowError("Error"); }
        }

        private void InputConstant(double constValue)
        {
            currentInput = FormatResult(constValue);
            shouldResetDisplay = true;
        }

        private string FormatResult(double value)
        {
            if (double.IsInfinity(value) || double.IsNaN(value))
                return "Error";

            if (value == Math.Floor(value) && Math.Abs(value) < 1e15)
                return ((long)value).ToString("N0");
            else
                return value.ToString("G10");
        }

        private void ShowError(string message)
        {
            currentInput = "Error";
            lblHistory.Text = message;
            shouldResetDisplay = true;
        }

        private void UpdateDisplay()
        {
            try
            {
                if (currentInput == "Error")
                {
                    txtDisplay.Text = "Error";
                }
                else
                {
                    double value = double.Parse(currentInput);
                    if (value == Math.Floor(value) && Math.Abs(value) < 1e15)
                        txtDisplay.Text = ((long)value).ToString("N0");
                    else
                        txtDisplay.Text = value.ToString("G10");
                }
            }
            catch
            {
                txtDisplay.Text = currentInput;
            }
        }

        private void UpdateHistory()
        {
            string opSymbol = "";
            switch (currentOperator)
            {
                case "+": opSymbol = "+"; break;
                case "−": opSymbol = "−"; break;
                case "×": opSymbol = "×"; break;
                case "÷": opSymbol = "÷"; break;
                case "^": opSymbol = "^"; break;
            }
            if (!string.IsNullOrEmpty(previousInput) && !string.IsNullOrEmpty(opSymbol))
            {
                double prevVal = double.Parse(previousInput);
                lblHistory.Text = (prevVal == Math.Floor(prevVal) ? ((long)prevVal).ToString("N0") : prevVal.ToString("G10")) + " " + opSymbol;
            }
        }

        private void SetupKeyboardShortcuts()
        {
            this.KeyPreview = true;
            this.KeyDown += (s, e) =>
            {
                switch (e.KeyCode)
                {
                    case Keys.Enter: Calculate(); e.SuppressKeyPress = true; break;
                    case Keys.Back: Backspace(); e.SuppressKeyPress = true; break;
                    case Keys.Escape: ClearAll(); e.SuppressKeyPress = true; break;
                    case Keys.Delete: ClearEntry(); e.SuppressKeyPress = true; break;
                }
            };
            this.KeyPress += (s, e) =>
            {
                char c = e.KeyChar;
                if (char.IsDigit(c) || c == '.')
                {
                    InputNumber(c.ToString());
                    UpdateDisplay();
                    e.Handled = true;
                }
                else if (c == '+' || c == '-' || c == '*' || c == '/')
                {
                    string op = c == '+' ? "+" : c == '-' ? "−" : c == '*' ? "×" : "÷";
                    InputOperator(op);
                    UpdateDisplay();
                    e.Handled = true;
                }
            };
        }

        private void CopyToClipboard()
        {
            try { Clipboard.SetText(txtDisplay.Text); } catch { }
        }

        private void PasteFromClipboard()
        {
            try
            {
                string text = Clipboard.GetText();
                if (double.TryParse(text, out _))
                {
                    currentInput = text;
                    UpdateDisplay();
                }
            }
            catch { }
        }

        private void ShowAbout()
        {
            MessageBox.Show(
                "Windows Calculator\nVersion 1.0.0\n\nA modern calculator application for Windows.\nBuilt with .NET 8 and Windows Forms.",
                "About Calculator",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
        }

        // Custom color table for menu strip
        private class CustomColorTable : ProfessionalColorTable
        {
            public override Color MenuStripGradientBegin => Color.FromArgb(45, 45, 45);
            public override Color MenuStripGradientEnd => Color.FromArgb(45, 45, 45);
            public override Color ToolStripDropDownBackground => Color.FromArgb(45, 45, 45);
            public override Color MenuItemSelected => Color.FromArgb(60, 60, 60);
            public override Color MenuItemSelectedGradientBegin => Color.FromArgb(60, 60, 60);
            public override Color MenuItemSelectedGradientEnd => Color.FromArgb(60, 60, 60);
            public override Color MenuItemPressedGradientBegin => Color.FromArgb(74, 74, 74);
            public override Color MenuItemPressedGradientEnd => Color.FromArgb(74, 74, 74);
            public override Color MenuItemBorder => Color.FromArgb(80, 80, 80);
            public override Color SeparatorDark => Color.FromArgb(80, 80, 80);
            public override Color SeparatorLight => Color.FromArgb(80, 80, 80);
        }
    }
}