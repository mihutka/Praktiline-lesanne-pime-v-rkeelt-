using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Ulesanne_keel
{
    public partial class MainPage : ContentPage
    {
        public ObservableCollection<Sona> Sonad { get; set; } = new();

        private Sona currentSona;
        private bool isRevealed = false;
        private Random random = new Random();

        public MainPage()
        {
            InitializeComponent();

            EnsureFileExists();
            LoadWords();

            // После загрузки слов сразу отображаем случайную карточку
            ShowRandomCard();
        }

        private void EnsureFileExists()
        {
            var filePath = Path.Combine(FileSystem.AppDataDirectory, "words.txt");
            if (File.Exists(filePath)) return;

            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "Ulesanne_keel.Resources.words.txt";
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                writer.Write(reader.ReadToEnd());
            }
        }

        private void LoadWords()
        {
            var filePath = Path.Combine(FileSystem.AppDataDirectory, "words.txt");
            if (File.Exists(filePath))
            {
                using (var reader = new StreamReader(filePath))
                {
                    string? line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        var parts = line.Split(';');
                        if (parts.Length == 4)
                        {
                            Sonad.Add(new Sona(parts[0], parts[1], parts[2], parts[3]));
                        }
                    }
                }
            }
        }

        private void SaveWords()
        {
            var filePath = Path.Combine(FileSystem.AppDataDirectory, "words.txt");
            using (var writer = new StreamWriter(filePath))
            {
                foreach (var sõna in Sonad)
                {
                    var line = $"{sõna.SonaTekst};{sõna.Tolge};{sõna.Selgitus};{sõna.Kategooria}";
                    writer.WriteLine(line);
                }
            }
        }

        // Показать случайную карточку
        private void ShowRandomCard()
        {
            if (Sonad.Count == 0)
            {
                WordLabel.Text = "Нет слов!";
                TranslationLabel.IsVisible = false;
                ExplanationLabel.IsVisible = false;
                CategoryLabel.IsVisible = false;
                return;
            }

            int index = random.Next(Sonad.Count);
            currentSona = Sonad[index];

            WordLabel.Text = currentSona.SonaTekst;
            TranslationLabel.Text = currentSona.Tolge;
            ExplanationLabel.Text = currentSona.Selgitus;
            CategoryLabel.Text = currentSona.Kategooria;

            // Изначально показываем только слово
            isRevealed = false;
            TranslationLabel.IsVisible = false;
            ExplanationLabel.IsVisible = false;
            CategoryLabel.IsVisible = false;
        }

        private void OnCardTapped(object sender, EventArgs e)
        {
            if (currentSona == null) return;

            // Переключаем состояние видимости
            isRevealed = !isRevealed;
            TranslationLabel.IsVisible = isRevealed;
            ExplanationLabel.IsVisible = isRevealed;
            CategoryLabel.IsVisible = isRevealed;
        }

        private void OnNextCardClicked(object sender, EventArgs e)
        {
            ShowRandomCard();
        }

        private async void OnAddWordClicked(object sender, EventArgs e)
        {
            string sõna = await DisplayPromptAsync("Lisa sõna", "Sisesta sõna:");
            string tõlge = await DisplayPromptAsync("Lisa tõlge", "Sisesta tõlge:");
            string selgitus = await DisplayPromptAsync("Lisa selgitus", "Sisesta selgitus:");
            string kategooria = await DisplayPromptAsync("Lisa kategooria", "Sisesta kategooria (õppimisel, õpitud, kordamisel):");

            if (!string.IsNullOrEmpty(sõna) && !string.IsNullOrEmpty(tõlge))
            {
                Sonad.Add(new Sona(sõna, tõlge, selgitus, kategooria));
                SaveWords();
            }
        }

        private async void OnEditWordClicked(object sender, EventArgs e)
        {
            if (currentSona != null)
            {
                string uusKategooria = await DisplayPromptAsync("Muuda kategooria", "Sisesta uus kategooria:");
                if (!string.IsNullOrEmpty(uusKategooria))
                {
                    currentSona.Kategooria = uusKategooria;
                    SaveWords();
                }
            }
        }

        private void OnDeleteWordClicked(object sender, EventArgs e)
        {
            if (currentSona != null)
            {
                Sonad.Remove(currentSona);
                SaveWords();
                ShowRandomCard();
            }
        }
    }
}
