using System.Runtime.InteropServices;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Windows;
using System;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ZadanieXD2.ViewModel;
using ZadanieXD2.Services;


namespace ZadanieXD2;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private MainWindowViewModel viewModel;
    private readonly ClipboardService _clipboardService;

    public MainWindow()
    {
        InitializeComponent();

        _clipboardService = new ClipboardService();
        viewModel = new MainWindowViewModel(_clipboardService);
        DataContext = viewModel;
        Loaded += (s, e) => _clipboardService.StartMonitoring(this);
        Closed += (s, e) => _clipboardService.StopMonitoring();
    }

    private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed)
        {
            this.DragMove();
        }
    }
}