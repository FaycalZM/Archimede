﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

using System.Windows.Input;

using System.Windows.Media.Animation;

using ArchimedeFront.Styles;
using Archimède;
using dnf;
using System.Windows.Media;
using DetectionErreurs;

namespace ArchimedeFront.Pages

{
    /// <summary>
    /// Interaction logic for InputFormula.xaml
    /// </summary>
    public partial class InputFormula : Page
    {
        int caretPosition = 0;

        public InputFormula()
        {
            InitializeComponent();
            errorsContainer.Children.Clear();
            numberOfVariablesInput.Width = new GridLength(0, GridUnitType.Star);
            guidePopUp.Visibility = Visibility.Collapsed;
            expression.Text = "A.B + !A.B.C";
            caretPosition=expression.Text.Length -1;
            Data.literal = true;


            AlignableWrapPanel buttons = new AlignableWrapPanel();
            Button operatorButton;
            string[] operators = { "ET", "OU", "NON", "NAND", "NOR", "XOR", "XNOR", "( )" };

            buttons.HorizontalContentAlignment = HorizontalAlignment.Center;
            buttons.MaxWidth = 500;
            foreach(string o in operators)
            {
                operatorButton = new Button() { Style = FindResource("operatorButton") as Style, Margin = new Thickness(14, 6, 14, 6)  , Content = o , HorizontalAlignment = HorizontalAlignment.Center};
                operatorButton.Click += new RoutedEventHandler( operator_Click );
                buttons.Children.Add(operatorButton);
            }
            
            operatorButtonsContainer.Children.Add(buttons);
        
        }

        private void simplifyButton_Click(object sender, RoutedEventArgs e)
        {
           errorsContainer.Children.Clear();
           Data.resete();
           Data.expression = expression.Text.Replace(" ","");
            
            if ( numerique.IsChecked == true)
            {
                Data.literal = false;
                Data.nbVariables = int.Parse(nbVariables.Text);
                Data.expression = expression.Text.Replace(" ","");
                Data.listMintermesString = Data.expression.Split(",").Distinct().ToList();
                long parsedInt;

                foreach(string minterm in Data.listMintermesString)
                {

                    try
                    {
                        parsedInt = long.Parse(minterm);
                        Data.mintermes.Add(new Minterme(parsedInt));
                    }
                    catch (OverflowException)
                    {
                        Data.mintermes.Add(new Minterme(minterm));
                    }

                }

                int maxNbUns = Minterme.maxNbUns;
                

                if (Minterme.maxNbVariables > Data.nbVariables)
                {
                    errorsContainer.Children.Add(generateNewError("La liste de mintermes introduite depasse le nombre maximal de variables introduit "));
                    return;
                }
            }
            else
            {
                Data.literal = true;
                List<string> errorMessages = new List<string>();
                errorMessages= Erreurs.detectionErreurs(expression.Text);
                if(errorMessages.Count > 0)
                {
                    foreach(string error in errorMessages)
                    {
                        errorsContainer.Children.Add(generateNewError(error));
                    }
                    return;
                }
                Data.expressionTransforme = ExprBool.transformerDNF(Data.expression);
                Data.variables = ExprBool.getVariables(Data.expressionTransforme).OrderBy(ch => ch).ToList();
                Data.nbVariables = Data.variables.Count;
                Data.stringListMinterm = ExprBool.getMinterms(Data.expressionTransforme, Data.variables);

                if( Data.stringListMinterm.Count == 0)
                {
                    Data.resultatFaux = true;
                    NavigationService.Navigate(new Uri("pack://application:,,,/Pages/Step6.xaml", UriKind.Absolute));
                    return;
                }
            }
            NavigationService.Navigate(new Uri("pack://application:,,,/Pages/Step1.xaml", UriKind.Absolute));
        }

        private void syntheseButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("pack://application:,,,/Pages/Synthese.xaml", UriKind.Absolute));
        }

        private void operator_Click(object sender, RoutedEventArgs e)
        {
            
            string res ;
            

            switch (((Button)sender).Content)
            {
                
                case "ET":
                    res = ".";
                    break;
                case "OU":
                    res = "+";
                    break;
                case "NON":
                    res = "!";
                    break;
                case "NAND":
                    res = "↑";
                    break;
                case "NOR":
                    res = "↓";
                    break;
                case "XOR":
                    res = "⊕";
                    break;
                case "XNOR":
                    res = "⊙";
                    break;
                case "( )":
                    res = "()";
                    break;
                default:
                    return;
                   
               
            }


            caretPosition = expression.CaretIndex;
            expression.Text = expression.Text.Substring(0,caretPosition) + res + expression.Text.Substring(caretPosition);
            expression.Focus();
            expression.CaretIndex = caretPosition + 1;
            
            

            
            
        }

        
       
        private void literale_Checked(object sender, RoutedEventArgs e)

        {
            if (numberOfVariablesInput == null) return;
            errorsContainer.Children.Clear();
            numberOfVariablesInput.Width = new GridLength(0, GridUnitType.Star);
            expression.Text = "A.B + !A.B.C";
            operatorButtonsContainer.Visibility = Visibility.Visible;
            buttonsContainer.Margin = new Thickness(0, 24, 0, 24);
            guidePopUp.Visibility = Visibility.Collapsed;
        }

        private void numerique_Checked(object sender, RoutedEventArgs e)
        {
            if (numberOfVariablesInput == null) return;

            errorsContainer.Children.Clear();
            numberOfVariablesInput.Width = new GridLength(60, GridUnitType.Pixel);
            expression.Text = "0,1,2,3,10";
            
            operatorButtonsContainer.Visibility = Visibility.Collapsed;
            guidePopUp.Visibility = Visibility.Visible;
            buttonsContainer.Margin = new Thickness(0, 58, 0, 24);
            DoubleAnimation da = new DoubleAnimation();
            da.From = 1;
            da.To = 0;
            da.Duration = new Duration(TimeSpan.FromSeconds(4));
            guidePopUp.BeginAnimation(OpacityProperty, da);
        }

        
        

        private StackPanel generateNewError(string message)
        {

            //generate exclamation mark
            Border border = new Border()
            {
                BorderThickness = new Thickness(2, 2, 2, 2),
                BorderBrush = Brushes.Red ,
                Width = 24 ,
                Height = 24 ,
                CornerRadius = new CornerRadius(24) ,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Child = new TextBlock() { Style= FindResource("fontProductSans") as Style ,FontSize =18 , Foreground=Brushes.Red,FontWeight= FontWeights.SemiBold ,VerticalAlignment = VerticalAlignment.Center , HorizontalAlignment = HorizontalAlignment.Center , Text="!" }
                

            };
            TextBlock textBlock = 
            new TextBlock() { Style = FindResource("paragraphe") as Style, FontSize = 20, Margin = new Thickness(8, 0, 4, 0), Foreground = Brushes.Red, FontWeight = FontWeights.SemiBold, VerticalAlignment = VerticalAlignment.Center, Text = "Erreur signalée :" };
            StackPanel errorSignal = new StackPanel()
            {
                Orientation = Orientation.Horizontal,
                VerticalAlignment = VerticalAlignment.Top,
            };
            errorSignal.Children.Add(border);
            errorSignal.Children.Add(textBlock);

            TextBlock errorMessage = new TextBlock() { Style = FindResource("paragraphe") as Style, FontSize = 20, Margin = new Thickness(0, 0, 0, 0), VerticalAlignment = VerticalAlignment.Top, TextWrapping = TextWrapping.Wrap, Text = message};

            StackPanel error = new StackPanel() { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Left, Margin = new Thickness(10, 2, 10, 8) };
            error.Children.Add(errorSignal);
            error.Children.Add(errorMessage);
            return error;
        }

     
    }



}
