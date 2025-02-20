using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using System.Collections.ObjectModel;
using TradeApp.windows;
using TradeApp.Model;
using TradeApp.Data;

public class PlaybookComponent : UserControl
{
    private Grid playbookGrid;
    private Button addPlaybookButton;
    private TextBlock titleText;
    private ListBox playbookListBox;
    private ObservableCollection<Playbook> playbooks;

    public PlaybookComponent()
    {
        playbooks = new ObservableCollection<Playbook>(PlaybookData.GetPlaybookList());
        InitializePlaybook();
    }

    private void InitializePlaybook()
    {
        playbookGrid = new Grid
        {
            Background = new SolidColorBrush(Color.FromRgb(30, 30, 47)),
            Margin = new Thickness(10),
        };

        // הגדרת עמודות
        playbookGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }); // כותרת במרכז
        playbookGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) }); // כפתור בצד ימין

        // הגדרת שורות
        playbookGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(80) }); // שורת הכותרת + הכפתור
        playbookGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) }); // רשימת הפלייבוקים

        // יצירת כותרת במרכז
        titleText = new TextBlock
        {
            Text = "Playbooks",
            FontSize = 36, // גודל כותרת גדול יותר
            FontWeight = FontWeights.Bold,
            Foreground = Brushes.White,
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Center // מיישר למרכז
        };
        Grid.SetColumn(titleText, 0);
        playbookGrid.Children.Add(titleText);

        // יצירת כפתור להוספת פלייבוק חדש
        addPlaybookButton = new Button
        {
            Content = "Add Playbook",
            Width = 180,
            Height = 50,
            FontSize = 18,
            FontWeight = FontWeights.Bold,
            Background = new SolidColorBrush(Color.FromRgb(120, 80, 180)),
            Foreground = Brushes.White,
            BorderThickness = new Thickness(2),
            Padding = new Thickness(10),
            HorizontalAlignment = HorizontalAlignment.Right, // מצמיד לצד ימין
            Margin = new Thickness(0,0,20,0),
        };
        addPlaybookButton.Click += OpenNewPlaybookWindow; // חיבור הכפתור לפונקציה

        Grid.SetColumn(addPlaybookButton, 1);
        playbookGrid.Children.Add(addPlaybookButton);

        // יצירת StackPanel להצגת הפלייבוקים עם כפתורי עריכה ומחיקה
        var playbookPanel = new StackPanel
        {
            Margin = new Thickness(20),
            Width = 700
        };

        foreach (var playbook in playbooks)
        {
            var rowGrid = new Grid { Margin = new Thickness(5) };
            rowGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            rowGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(80) });
            rowGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(120) });
            rowGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(120) });

            // שם הפלייבוק
            var textBlock = new TextBlock
            {
                Text = playbook.Name,
                Foreground = Brushes.White,
                FontSize = 22,
                VerticalAlignment = VerticalAlignment.Center
            };
            Grid.SetColumn(textBlock, 0);
            rowGrid.Children.Add(textBlock);

            // מספר קריטריונים בפלייבוק
            var criteriaCountText = new TextBlock
            {
                Text = $"{playbook.Criteria.Count} criteria",
                Foreground = Brushes.LightGray,
                FontSize = 18,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center
            };
            Grid.SetColumn(criteriaCountText, 1);
            rowGrid.Children.Add(criteriaCountText);

            // כפתור עדכון
            var updateButton = new Button
            {
                Content = "✏️ Edit",
                FontSize = 18,
                Background = Brushes.Transparent,
                Foreground = Brushes.LightBlue,
                Margin = new Thickness(5),
                Width = 100,
                Height = 40
            };
            updateButton.Click += (s, e) => EditPlaybook(playbook);
            Grid.SetColumn(updateButton, 2);
            rowGrid.Children.Add(updateButton);

            // כפתור מחיקה
            var deleteButton = new Button
            {
                Content = "❌ Delete",
                FontSize = 18,
                Background = Brushes.Transparent,
                Foreground = Brushes.Red,
                Margin = new Thickness(5),
                Width = 100,
                Height = 40
            };
            deleteButton.Click += (s, e) => DeletePlaybook(playbook);
            Grid.SetColumn(deleteButton, 3);
            rowGrid.Children.Add(deleteButton);

            playbookPanel.Children.Add(rowGrid);
        }

        // הוספת הרשימה לגריד
        var scrollViewer = new ScrollViewer { Content = playbookPanel, VerticalScrollBarVisibility = ScrollBarVisibility.Auto };
        Grid.SetRow(scrollViewer, 1);
        Grid.SetColumnSpan(scrollViewer, 2);
        playbookGrid.Children.Add(scrollViewer);

        Content = playbookGrid;
    }

    private void EditPlaybook(Playbook playbook)
    {
        WindowNewPlaybook editPlaybookWindow = new WindowNewPlaybook(playbooks, playbook);
        editPlaybookWindow.ShowDialog(); // פתיחת החלון בעריכה

        // לאחר סגירת החלון, נטען מחדש את הפלייבוקים
        RefreshPlaybooks();
    }


    private void OpenNewPlaybookWindow(object sender, RoutedEventArgs e)
    {
        WindowNewPlaybook newPlaybookWindow = new WindowNewPlaybook(playbooks);
        newPlaybookWindow.ShowDialog(); // פותח את החלון כמודאלי

        // לאחר שהמשתמש סוגר את החלון, טוען מחדש את הפלייבוקים מהדאטהבייס
        RefreshPlaybooks();
    }

    private void DeletePlaybook(Playbook playbook)
    {
        var result = MessageBox.Show($"Are you sure you want to delete '{playbook.Name}'?",
            "Confirm Deletion", MessageBoxButton.YesNo, MessageBoxImage.Warning);

        if (result == MessageBoxResult.Yes)
        {
            PlaybookData.DeletePlaybook(playbook);
            playbooks.Remove(playbook);
            RefreshPlaybooks(); // רענון הרשימה
        }
    }

    private void RefreshPlaybooks()
    {
        playbooks = new ObservableCollection<Playbook>(PlaybookData.GetPlaybookList());
        InitializePlaybook(); // יצירת ה-UI מחדש כדי לשקף את השינויים
    }

}