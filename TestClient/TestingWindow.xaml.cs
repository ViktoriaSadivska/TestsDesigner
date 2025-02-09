﻿using DBLib;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace TestClient
{
    /// <summary>
    /// Interaction logic for TestingWindow.xaml
    /// </summary>
    public partial class TestingWindow : Window
    {
        Test Test { get; set; }
        List<Question> Questions { get; set; }
        public Dictionary<int,ObservableCollection<UserAnswer>> QuestionAnswers { get; set; }

        List<Button> buttons { get; set; } = new List<Button>();
        public TestingWindow(Test test, Question[] questions, Answer[] answers)
        {
            InitializeComponent();

            Test = test;
            Questions = questions.ToList();
            QuestionAnswers = new Dictionary<int, ObservableCollection<UserAnswer>>();
            foreach (var q in questions)
            {
                var collection = new ObservableCollection<UserAnswer>();
                foreach (var a in answers.Where(x => x.idQuestion == q.Id))
                {
                    collection.Add(new UserAnswer { Answer = a, Reply = false });
                }
                QuestionAnswers.Add(q.Id, collection); 
            }
            InitializeData();
        }

        //initializer
        private void InitializeData()
        {
            AuthorTextBox.Text = Test.Author;
            TitleTextBox.Text = Test.Title;
            DescTextBox.Text = Test.Description;
            QuestCountTextBox.Text = Questions.Count.ToString();
            MaxPointTextBox.Text = Questions.Sum(x => x.Points).ToString();
            PassPercTextBox.Text = Test.PassingPercent.ToString();

            
            for(int i = 0; i < Questions.Count; ++i)
            {
                buttons.Add(new Button { Margin = new Thickness(3), Width = 20, Height = 50, Background = Brushes.Gray });
                buttons[i].Click += TestingWindow_Click;
                ButtonsStackPanel.Children.Add(buttons[i]);   
            }
        }

        //clicks
        private void TestingWindow_Click(object sender, RoutedEventArgs e)
        {
            int ind = buttons.IndexOf(sender as Button);
            QuestionLabel.Content = Questions[ind].Text;

            if (Questions[ind].Image != null)
            {
                using (MemoryStream ms = new MemoryStream(Questions[ind].Image))
                {
                    BitmapImage btmp = new BitmapImage();
                    btmp.BeginInit();
                    btmp.CacheOption = BitmapCacheOption.OnLoad;
                    btmp.StreamSource = ms;
                    btmp.EndInit();
                    TestImage.Source = btmp;
                }
            }
            AnswersDataGrid.ItemsSource = QuestionAnswers[Questions[ind].Id];
        }
        private void FinishButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            this.Close();
        }
        private void OnChecked(object sender, RoutedEventArgs e)
        {
            foreach (var item in AnswersDataGrid.ItemsSource as ObservableCollection<UserAnswer>)
            {
                if (item.Reply == true)
                {
                    buttons[Questions.IndexOf(Questions.FirstOrDefault(x => x.Id == item.Answer.idQuestion))].Background = Brushes.CornflowerBlue;
                    break;
                }
                buttons[Questions.IndexOf(Questions.FirstOrDefault(x => x.Id == item.Answer.idQuestion))].Background = Brushes.Gray;
            }
        }
    }
    public class UserAnswer
    {
        public Answer Answer { get; set; }
        public bool Reply { get; set; }
    }
}
