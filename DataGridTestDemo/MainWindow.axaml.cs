using Avalonia.Controls;

namespace DataGridTestDemo
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = new MainViewModel(datagrid);
        }
    }
}