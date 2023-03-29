using System.Windows;

namespace RandomizerApp
{
    /// <summary>
    /// Логика взаимодействия для Results.xaml
    /// </summary>
    public partial class Results : Window
    {
        public Results()
        {
            InitializeComponent();
        }

        private void okButton_Click(object sender, RoutedEventArgs e) => Close();

        private void copyButton_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(resultsTextbox.Text);
            MessageBox.Show("Done");
        }
    }
}
