using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System;
using Xamarin.Forms.Xaml;
using Xamarin.Forms;

namespace PopUP
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PopUp_Page : ContentPage
    {
        private int questionIndex;
        private int score;
        private List<int> numbers;
        private Random random;

        public PopUp_Page()
        {
            InitializeComponent();
            random = new Random();
        }

        private async void OnStartTestClicked(object sender, EventArgs e)
        {
            var name = NameEntry.Text;

            if (string.IsNullOrWhiteSpace(name))
            {
                await DisplayAlert("Error", "Enter your name.", "OK");
                return;
            }

            questionIndex = 0;
            score = 0;
            numbers = new List<int>();

            for (int i = 0; i < 10; i++)
            {
                numbers.Add(random.Next(1, 11));
            }

            await DisplayQuestion();
        }

        private async System.Threading.Tasks.Task DisplayQuestion()
        {
            var number1 = numbers[questionIndex];
            var number2 = random.Next(1, 11);
            var answer = number1 + number2;

            var response = await DisplayPromptAsync($"Question {questionIndex + 1}", $"How much it will be {number1} + {number2}?", placeholder: "Answer");

            await SaveDataToFileAsync("math.txt", $"{number1} + {number2} = {answer}\n");

            if (int.TryParse(response, out int userAnswer))
            {
                if (userAnswer == answer)
                {
                    score++;
                }
            }

            if (questionIndex < 9)
            {
                questionIndex++;
                await DisplayQuestion();
            }
            else
            {
                var percentageScore = (double)score / 10 * 100;
                var letterGrade = GetLetterGrade(percentageScore);

                await DisplayAlert("Test completed", $"{NameEntry.Text}, you have got {score} / 10. ({percentageScore}%)\nScore is: {letterGrade}", "OK");
            }
        }

        private string GetLetterGrade(double percentageScore)
        {
            if (percentageScore >= 90)
                return "A";
            else if (percentageScore >= 75)
                return "B";
            else if (percentageScore >= 50)
                return "C";
            else
                return "D";
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        async Task SaveDataToFileAsync(string fileName, string data)
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string fullPath = Path.Combine(path, fileName);

            using (StreamWriter sw = new StreamWriter(fullPath, true))
            {
                await sw.WriteAsync(data).ConfigureAwait(false);
            }
        }

        async void OnDeleteButtonClicked(object sender, EventArgs e)
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string fullPath = Path.Combine(path, "math.txt");

            if (File.Exists(fullPath))
            {
                try
                {
                    File.Delete(fullPath);
                    await DisplayAlert("Done", $"File deleted", "OK");
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error", $"Erasing aborted: {ex.Message}", "OK");
                }
            }
            else
            {
                await DisplayAlert("Error", "File not found", "OK");
            }
        }
    }
}