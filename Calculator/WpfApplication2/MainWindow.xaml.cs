using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Calculator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        float opMem; // Operand memory
        float opMem1; // Operand memory
        float mem; // User memory
        String currentOp; // Current operation
        bool finishedOp = false; // True if operation is complete
        bool clearOnNext = false; // True if the display will be replaced with the next entry
        bool buttonOp = false; // True if operation triggered by an operator

        public MainWindow()
        {
           InitializeComponent();
        }
        
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender; // Convert sender to useful parameters
            String buttonName = btn.Name;
            String buttonContent = (String)btn.Content;
            buttonEval(buttonContent);
        }

        private void buttonEval(string buttonContent)
        {
            int buttonValue;
            if (int.TryParse(buttonContent, out buttonValue)) // If the button was a number
                if (clearOnNext || display.Text.Equals("0")) // Text needs to be replaced
                {
                    display.Text = buttonContent; // Replace content
                    clearOnNext = false;
                    buttonOp = false;
                }
                else if (finishedOp) // If an equation was just evaluated
                {
                    display.Text = buttonContent;
                    opMem = 0; // Clear opMem
                    opMem1 = 0; // Clear opMem2
                    finishedOp = false;
                }
                else // General case
                    display.Text += buttonContent; // Append number to display
            else if (buttonContent.Equals(".")) // Decimal support
            { 
                if (display.Text.Equals("")) // Display is empty
                    display.Text = "0.";
                else
                    display.Text += buttonContent;
            }
            else if (display.Text != "")
            { // Trigger operation storage, so long as display is not empty
                switch (buttonContent) {
                    case "+":
                    case "-":
                    case "*":
                    case "/":
                        currentOp = buttonContent; // Assign currentOp

                        if (opMem != 0 && !buttonOp) // If there's already something stored
                        { // Do the calculation
                            opMem1 = float.Parse(display.Text); // opMem1 = current display
                            display.Text = evaluate(currentOp, opMem, opMem1).ToString(); // display = evaluated equation
                            opMem = float.Parse(display.Text); // replace opMem with result
                            buttonOp = true;
                        }
                        else
                        {
                            opMem = float.Parse(display.Text); // Store displayed number
                        }
                        clearOnNext = true;
                        break;

                    case "AC": // All Clear
                        clear();
                        break;
                    case "C CE": // Clear current diaplay
                        display.Text = "0";
                        break;
                    case "MC": // Memory clear
                        mem = 0;
                        break;
                    case "MR": // Memory Recall
                        display.Text = mem.ToString();
                        opMem = 0;
                        break;
                    case "M+": // Memory Add
                        mem += int.Parse(display.Text);
                        break;
                    case "M-": // Memory Subtract
                        mem -= int.Parse(display.Text);
                        break;
                    case "%": // Percent
                        evaluate(currentOp, opMem, (float.Parse(display.Text) * opMem / 100));
                        break;
                    case "Back": // Go Back one digit
                        display.Text = display.Text.Remove(display.Text.Length - 1);
                        break;
                    case "=": // Equals
                        if (!finishedOp)
                        {
                            opMem1 = float.Parse(display.Text);
                            display.Text = (evaluate(currentOp, opMem, opMem1)).ToString();
                            finishedOp = true;
                            buttonOp = false;
                        }
                        else
                        {
                            display.Text = (evaluate(currentOp, float.Parse(display.Text), opMem1)).ToString();
                        }
                        break;
                }
             }
            operation.Text = currentOp;
            memory.Text = mem.ToString();
          }

        // Returns the value of the completed operation
        private static float evaluate (string symbol, float operand1, float operand2)
        {
            float output = 0;
            switch (symbol)
            {
                case "+":
                    output = (operand1 + operand2);
                    break;
                case "-":
                    output = (operand1 - operand2);
                    break;
                case "*":
                    output = (operand1 * operand2);
                    break;
                case "/":
                    output = (operand1 / operand2);
                    break;
            }
            return output;
        }

        private void clear()
        {
            opMem = 0;
            opMem1 = 0;
            mem = 0;
            display.Text = "0";
            currentOp = "";
            finishedOp = false;
            buttonOp = false;
        }

        private void keyPressed(object sender, KeyEventArgs e)
        {
            //Button btn = (Button)(sender);
            Button btn = e.Source as Button;
            string keyContent = btn.Content.ToString();
            if (Content != null)
            {
                buttonEval(keyContent);
            }
        }
    }
}
