using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using static System.Net.Mime.MediaTypeNames;

namespace TickerTape_Stocks;

/// <summary>
///     Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private int scrollOffset = 0;
    String preTabbedText = "";
    private DispatcherTimer tickerTimer;

    public MainWindow()
    {
        InitializeComponent();
    }

    private void wdw_Main_ContentRendered(object sender, EventArgs e)
    {
        // When loading up the window, it renders the timer to come into existence.
        tickerTimer_Setup();
    }

    private void tickerTimer_Setup()
    {
        // The three lines that set up the timer
        tickerTimer = new DispatcherTimer();
        tickerTimer.Tick += tickerTimer_Tick;
        tickerTimer.Interval = TimeSpan.FromMilliseconds(sld_movementTick1.Value);
    }

    private void tickerTimer_Tick(object sender, EventArgs e)
    {
        // For every "tick", scroll the text
        if (txt_tickerTape.Text.Length > 0 && txt_tickerTape.Text.Length > txt_tickerTape.ActualWidth / (txt_tickerTape.FontSize / 2))
        {
            txt_tickerTape.Text = txt_tickerTape.Text.Substring(1) + txt_tickerTape.Text[0];
        }
        else
        {
            lbl_warning.Content = "Add more stocks to make the textbox scroll!";
        }
    }

    private void btn_startStop_Click(object sender, RoutedEventArgs e)
    { 

        // If the button at the time of the press says "Start", change its label to "Stop" and start the timer
        if (btn_startStop.Content.ToString() == "Start")
        {
            preTabbedText = txt_tickerTape.Text;
            lst_currentList.IsEnabled = false;
            lst_allStocks.IsEnabled = false;
            btn_clear.IsEnabled = false;
            btn_startStop.Content = "Stop";
            txt_tickerTape.Text += "        ";
            tickerTimer.Start();
        }

        // If the button at the time of the press says "Stop", change its label to "Start" and stop the timer
        else
        {
            lst_currentList.IsEnabled = true;
            lst_allStocks.IsEnabled = true;
            btn_clear.IsEnabled = true;
            btn_startStop.Content = "Start";
            txt_tickerTape.Text = preTabbedText;
            txt_tickerTape.Text.Trim();
            lbl_warning.Content = "";
            tickerTimer.Stop();
        }
    }

    private void btn_add_Click(object sender, RoutedEventArgs e)
    {
        // Adds the stock value to the "all stocks" list and textbox string when clicked
        var selectedItem = lst_allStocks.SelectedItem as ListBoxItem;
        if (lst_allStocks.SelectedItem != null)
        {
            lst_allStocks.Items.Remove(selectedItem);
            lst_currentList.Items.Add(selectedItem);
        }

        if (lst_currentList.Items.Count > 0) btn_startStop.IsEnabled = true;

        var stockInformation = selectedItem.Content.ToString();
        txt_tickerTape.AppendText(stockInformation + "   ");

        btn_add.IsEnabled = false;
    }

    private void btn_remove_Click(object sender, RoutedEventArgs e)
    {
        // Removes the stock value from the "current" list and textbox string when clicked
        var selectedItem = lst_currentList.SelectedItem as ListBoxItem;
        if (lst_currentList.SelectedItem != null)
        {
            lst_currentList.Items.Remove(selectedItem);
            lst_allStocks.Items.Add(selectedItem);
        }

        if (lst_currentList.Items.Count == 0) btn_startStop.IsEnabled = false;

        var currentText = txt_tickerTape.Text;
        var newText = currentText.Replace(selectedItem.Content + "   ", "");

        txt_tickerTape.Text = newText;

        btn_remove.IsEnabled = false;
    }

    private void lst_currentList_GotFocus(object sender, RoutedEventArgs e)
    {
        // Resets the selection cursor and disables the add button
        if (lst_allStocks.SelectedItem != null) lst_allStocks.SelectedIndex = -1;
        btn_add.IsEnabled = false;
    }

    private void lst_allStocks_GotFocus(object sender, RoutedEventArgs e)
    {
        // Resets the selection cursor and disables the remove button
        if (lst_currentList.SelectedItem != null) lst_currentList.SelectedIndex = -1;
        btn_remove.IsEnabled = false;
    }

    private void btn_clear_Click(object sender, RoutedEventArgs e)
    {
        // Resets the program to its default state
        txt_tickerTape.Text = string.Empty;

        for (var i = lst_currentList.Items.Count - 1; i >= 0; i--)
        {
            var item = lst_currentList.Items[i];
            var itemBox = lst_currentList.ItemContainerGenerator.ContainerFromItem(item) as ListBoxItem;
            if (itemBox != null)
            {
                lst_currentList.Items.RemoveAt(i);
                lst_allStocks.Items.Add(itemBox);
            }
        }

        btn_remove.IsEnabled = false;
        btn_add.IsEnabled = false;
        btn_startStop.IsEnabled = false;
    }

    private void sld_movementTick1_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        // Adjusts how fast the tape scrolls
        if (tickerTimer != null) tickerTimer.Interval = TimeSpan.FromMilliseconds(sld_movementTick1.Value);
    }

    private void lst_allStocks_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        // When any item box is selected in the all stocks list, the add button is enabled
        if (lst_allStocks.Items.Count > 0) btn_add.IsEnabled = true;
    }

    private void lst_currentList_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        // When any item box is selected in the current list, the remove button is enabled
        if (lst_currentList.Items.Count > 0) btn_remove.IsEnabled = true;
    }

    private void btn_startStop_GotFocus(object sender, RoutedEventArgs e)
    {
        // When this button gains focus, it disables the add and remove buttons so that nothing can be added to
        // the textbox's text while it is scrolling. This needed to be here since there was a bug with tab navigating
        // through the whole window 
        btn_add.IsEnabled = false;
        btn_remove.IsEnabled = false;
    }
}