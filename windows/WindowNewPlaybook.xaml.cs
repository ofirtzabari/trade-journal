using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using TradeApp.Data;
using TradeApp.Model;

namespace TradeApp.windows
{
    public partial class WindowNewPlaybook : Window
    {
        private ObservableCollection<Playbook> playbooks;
        private Playbook existingPlaybook;
        private bool isEditMode = false;
        public WindowNewPlaybook(ObservableCollection<Playbook> p)
        {
            InitializeComponent();
            playbooks = p;
        }

        public WindowNewPlaybook(ObservableCollection<Playbook> p, Playbook playbookToEdit) : this(p)
        {
            existingPlaybook = playbookToEdit;
            isEditMode = true;
            LoadPlaybookData(playbookToEdit);
            this.Title = "Edit Playbook";
            saveButton.Content = "Save Changes";
        }

        private void LoadPlaybookData(Playbook playbook)
        {
            PlaybookNameTextBox.Text = playbook.Name;

            // ניקוי הקריטריונים והכנסתם מחדש
            CriteriaContainer.Children.Clear();
            foreach (var criterion in playbook.Criteria)
            {
                AddCriterion(criterion);
            }
        }

        // 📌 פונקציה להוספת קריטריון חדש
        private void NewCriterion_Click(object sender, RoutedEventArgs e)
        {
            var stackPanel = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 5, 0, 5) };

            var rectangle = new Rectangle
            {
                Width = 24,
                Height = 24,
                Stroke = Brushes.White,
                StrokeThickness = 2,
                Margin = new Thickness(0, 0, 10, 0)
            };

            var textBox = new TextBox
            {
                Width = 460,
                Height = 30,
                Foreground = Brushes.White,
                Background = Brushes.Transparent,
                BorderBrush = Brushes.White,
                Text = "Enter criterion...",
                FontSize = 18
            };
            textBox.PreviewMouseDown += myTextBox_PreviewMouseDown;

            var removeButton = new Button
            {
                Content = "❌",
                Foreground = Brushes.White,
                Background = Brushes.Transparent,
                BorderBrush = Brushes.Transparent,
                FontSize = 16,
                Cursor = Cursors.Hand,
                Margin = new Thickness(5, 0, 0, 0)
            };
            removeButton.Click += RemoveCriterion_Click;

            stackPanel.Children.Add(rectangle);
            stackPanel.Children.Add(textBox);
            stackPanel.Children.Add(removeButton);

            CriteriaContainer.Children.Add(stackPanel);
            UpdateDeleteButtons();
        }

        // 📌 פונקציה למחיקת קריטריון
        private void RemoveCriterion_Click(object sender, RoutedEventArgs e)
        {
            if (CriteriaContainer.Children.Count > 1)
            {
                var button = sender as Button;
                var parent = button?.Parent as StackPanel;
                if (parent != null)
                {
                    CriteriaContainer.Children.Remove(parent);
                    UpdateDeleteButtons();
                }
            }
        }

        private void AddCriterion(string criterionText)
        {
            var stackPanel = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 5, 0, 5) };

            var rectangle = new Rectangle
            {
                Width = 24,
                Height = 24,
                Stroke = Brushes.White,
                StrokeThickness = 2,
                Margin = new Thickness(0, 0, 10, 0)
            };

            var textBox = new TextBox
            {
                Width = 460,
                Height = 30,
                Foreground = Brushes.White,
                Background = Brushes.Transparent,
                BorderBrush = Brushes.White,
                Text = criterionText,
                FontSize = 18
            };
            textBox.PreviewMouseDown += myTextBox_PreviewMouseDown;

            var removeButton = new Button
            {
                Content = "❌",
                Foreground = Brushes.White,
                Background = Brushes.Transparent,
                BorderBrush = Brushes.Transparent,
                FontSize = 16,
                Cursor = Cursors.Hand,
                Margin = new Thickness(5, 0, 0, 0)
            };
            removeButton.Click += RemoveCriterion_Click;

            stackPanel.Children.Add(rectangle);
            stackPanel.Children.Add(textBox);
            stackPanel.Children.Add(removeButton);

            CriteriaContainer.Children.Add(stackPanel);
            UpdateDeleteButtons();
        }


        // 📌 פונקציה שמעדכנת אם אפשר למחוק או לא (לפחות קריטריון אחד יישאר)
        private void UpdateDeleteButtons()
        {
            foreach (var child in CriteriaContainer.Children)
            {
                if (child is StackPanel stackPanel && stackPanel.Children.Count > 2)
                {
                    var deleteButton = stackPanel.Children[2] as Button;
                    if (deleteButton != null)
                        deleteButton.IsEnabled = CriteriaContainer.Children.Count > 1;
                }
            }
        }



        // 📌 סימון כל הטקסט בלחיצה על TextBox
        private void myTextBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox != null && !textBox.IsKeyboardFocused)
            {
                textBox.Focus();
                textBox.SelectAll();
                e.Handled = true;
            }
        }

        private void SavePlaybook_Click(object sender, RoutedEventArgs e)
        {
            string playbookName = PlaybookNameTextBox.Text.Trim();

            if (string.IsNullOrEmpty(playbookName))
            {
                MessageBox.Show("Playbook name cannot be empty!", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            List<string?> criteriaList = CriteriaContainer.Children
                .OfType<StackPanel>()
                .Select(sp => sp.Children.OfType<TextBox>().FirstOrDefault()?.Text.Trim())
                .Where(text => !string.IsNullOrEmpty(text))
                .ToList();

            if (isEditMode && existingPlaybook != null)
            {
                // עדכון פלייבוק קיים
                existingPlaybook.Name = playbookName;
                existingPlaybook.Criteria = criteriaList;

                PlaybookData.UpdatePlaybookInDb(existingPlaybook);
            }
            else
            {
                // יצירת פלייבוק חדש
                var newPlaybook = new Playbook(playbookName, criteriaList);
                PlaybookData.AddPlaybookToDb(newPlaybook);
                playbooks.Add(newPlaybook);
            }

            MessageBox.Show("Playbook saved successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            Close();
        }


    }
}
