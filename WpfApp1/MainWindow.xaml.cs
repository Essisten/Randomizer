using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Resources;

namespace RandomizerApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            names = FindResource("Names").ToString().Split(';');
            surnames = FindResource("Surnames").ToString().Split(';');
        }

        private Random randomer = new Random();
        private int fNum, sNum, times, sucRangeNum1, sucRangeNum2, luck = 20, counter = 0;
        private bool isCanceled = false, warning = true;
        //Я устану переносить ВСЕ существующие приставки и окончания. Довольствуйтесь тем, что есть.
        private readonly string gEng = "eyuioa", sEng = "rtpsdfghjklcvbnm", specialEng = "qwzx",
            gRus = "уеыаоэяиюё", sRus = "цкнгшщзхфвпрлджчсмтб";
        private string word = "", result = "";
        private readonly string[] prefixEng = new string[] { "pre", "re", "un", "de", "in", "a", "an", "anti", "undo", "auto", "co", "ex", "extra", "hyper" },
            endingEng = new string[] { "ing", "er", "ed", "ist", "ity", "ty", "ment", "ness", "ful", "less", "acy", "al", "ance", "ence", "ate", "en", "fy", "ify", "esque" },
            prefixRus = new string[] { "пре", "при", "про", "пере", "над", "под", "в", "во", "вы", "до", "за", "о", "об", "от", "по", "пред", "на", "не", "у" },
            endingRus = new string[] { "ая", "ое", "ые", "ой", "ый", "ть", "ла", "ли" },
            names, surnames;
        private void randomizeNameButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Your name is:\n" + names[randomer.Next(0, names.Length)] + " " + surnames[randomer.Next(0, surnames.Length)]);
        }

        private void advancedCheckbox2_Click(object sender, RoutedEventArgs e) => advancedGroup2.Visibility = advancedCheckbox2.IsChecked.Value ? Visibility.Visible : Visibility.Hidden;

        private void advancedOptionsCheckbox_Click(object sender, RoutedEventArgs e)
        {
            TimesBox.Text = ParseInt(TimesBox.Text).ToString();
            SucNumBox1.Text = ParseInt(SucNumBox1.Text).ToString();
            SucNumBox2.Text = ParseInt(SucNumBox2.Text).ToString();
            advancedGroup.Visibility = advancedOptionsCheckbox.IsChecked.Value ? Visibility.Visible : Visibility.Hidden;
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e) => isCanceled = true;

        private void randomizeWordButton_Click(object sender, RoutedEventArgs e)
        {
            bool isEng = LangSelectEng.IsChecked.Value;
            if (prefixCheckbox.IsChecked.Value)
                word += isEng ? prefixEng[randomer.Next(0, prefixEng.Length)] : prefixRus[randomer.Next(0, prefixRus.Length)];
            for (int i = 0; i < randomer.Next(2, 14); i++)
            {
                if (absoluteRandomCheckbox.IsChecked.Value)
                {
                    word += randomer.Next(0, 2) == 1 ? GetGChar(isEng) : GetSChar(isEng);
                    continue;
                }
                if (word.Length < 2)
                {
                    word += randomer.Next(0, 2) == 1 ? GetGChar(isEng) : GetSChar(isEng);
                    continue;
                }
                if ((isEng && gEng.Contains(word[word.Length - 1]) && gEng.Contains(word[word.Length - 2])) ||
                    (!isEng && gRus.Contains(word[word.Length - 1]) && gRus.Contains(word[word.Length - 2])))
                    word += GetSChar(isEng);
                else if ((isEng && sEng.Contains(word[word.Length - 1]) && sEng.Contains(word[word.Length - 2])) ||
                    (!isEng && sRus.Contains(word[word.Length - 1]) && sRus.Contains(word[word.Length - 2])))
                    word += GetGChar(isEng);
                else
                    word += randomer.Next(0, 2) == 1 ? GetGChar(isEng) : GetSChar(isEng);
            }
            if (endingCheckbox.IsChecked.Value)
                word += isEng ? endingEng[randomer.Next(0, endingEng.Length)] : endingRus[randomer.Next(0, endingRus.Length)];
            MessageBox.Show($"Your random word is: \"{word}\"");
            word = "";
        }
          
        private async void RandomizeNumButton_Click(object sender, RoutedEventArgs e)
        {
            if (warning && ((new string[] { fNumBox.Text, sNumBox.Text }).Contains("") ||
                (advancedOptionsCheckbox.IsChecked.Value && (new string[] { SucNumBox1.Text, SucNumBox2.Text }).Contains(""))))
            {
                MessageBox.Show("Empty fields become '1' by default");
                warning = false;
            }
            fNum = ParseInt(fNumBox.Text);
            sNum = ParseInt(sNumBox.Text) + 1;
            times = ParseInt(TimesBox.Text);
            if (advancedOptionsCheckbox.IsChecked.Value)
            {
                sucRangeNum1 = ParseInt(SucNumBox1.Text);
                sucRangeNum2 = ParseInt(SucNumBox2.Text);
            }
            Progressbar.Value = 0;
            Progressbar.Visibility = Visibility.Visible;
            ProgressbarLabel.Visibility = Visibility.Visible;
            randomizeNumButton.Visibility = Visibility.Hidden;
            cancelButton.Visibility = Visibility.Visible;
            Progressbar.Maximum = times;
            counter = 0;
            result = "";
            isCanceled = false;
            await Task.Run(() => Calculating(times));
        }

        private int ParseInt(string s) => int.TryParse(s, out int _) ? int.Parse(s) : 1;

        private void Calculating(int t)
        {
            for (int i = 0; i < t; i++)
            {
                if (isCanceled)
                {
                    Dispatcher.Invoke(new Action(delegate ()
                    {
                        Progressbar.Visibility = Visibility.Hidden;
                        ProgressbarLabel.Visibility = Visibility.Hidden;
                        cancelButton.Visibility = Visibility.Hidden;
                        randomizeNumButton.Visibility = Visibility.Visible;
                    }));
                    return;
                }
                int r = randomer.Next(Math.Min(fNum, sNum), Math.Max(fNum, sNum));
                Dispatcher.Invoke(new Action(delegate ()
                {
                    if (!advancedOptionsCheckbox.IsChecked.Value || resultShowCheckbox.IsChecked.Value)
                        result += r.ToString() + ' ';
                }));
                if (r >= Math.Min(sucRangeNum1, sucRangeNum2) && r <= Math.Max(sucRangeNum1, sucRangeNum2))
                    counter++;
                Dispatcher.Invoke(new Action(delegate ()
                {
                    Progressbar.Value++;
                    ProgressbarLabel.Content = $"{i}/{times}";
                }));
            }
            Dispatcher.Invoke(new Action(delegate ()
            {
                Progressbar.Visibility = Visibility.Hidden;
                ProgressbarLabel.Visibility = Visibility.Hidden;
                cancelButton.Visibility = Visibility.Hidden;
                randomizeNumButton.Visibility = Visibility.Visible;
                Results resultsWindow = new Results();
                if (!advancedOptionsCheckbox.IsChecked.Value || resultShowCheckbox.IsChecked.Value)
                    resultsWindow.resultsTextbox.Text += result;
                if (advancedOptionsCheckbox.IsChecked.Value)
                    resultsWindow.resultsTextbox.Text += $"\nCount of succesful numbers in range {Math.Min(sucRangeNum1, sucRangeNum2)}-{Math.Max(sucRangeNum1, sucRangeNum2)}: {counter}";
                resultsWindow.Show();
            }));
        }

        private char GetSChar(bool isEng)
        {
            if (isEng)
                return randomer.Next(0, 100) < luck ? specialEng[randomer.Next(0, specialEng.Length)] : sEng[randomer.Next(0, sEng.Length)];
            else
                return sRus[randomer.Next(0, sRus.Length)];
        }

        private char GetGChar(bool isEng)
        {
            if (isEng)
                return gEng[randomer.Next(0, gEng.Length)];
            else
                return gRus[randomer.Next(0, gRus.Length)];
        }
    }
}
