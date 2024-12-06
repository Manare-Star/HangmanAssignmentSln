using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using System.Runtime.CompilerServices;

namespace HangmanAssignment
{
    public class myViewModel : INotifyPropertyChanged
    {
        private const int MAX_ATTEMPTS = 7;  // Changed from 8 to 7
        private string _currentGuess = string.Empty;
        private string _displayWord = string.Empty;
        private string _hangmanImage = "hang1.png";
        private int _incorrectGuesses = 0;
        private string _chosenWord = "HELLO";
        private string _attemptsMessage = "8 attempts left";
        private bool _isGameOver = false;
        private HashSet<char> _guessedLetters;

        // List of words to choose from
        private static readonly List<string> Words = new List<string>
        {
            "PROGRAMMING", "COMPUTER", "MOBILE", "DEVELOPER", "HANGMAN", "MAUI", "CLOUD", "GITHUB"
        };

        public event PropertyChangedEventHandler? PropertyChanged;

        // Constructor initializes the game state
        public myViewModel()
        {
            InitializeGame();
        }

        private void InitializeGame()
        {
            Random random = new Random();
            _chosenWord = Words[random.Next(Words.Count)].ToUpper(); // Random word selection

            // Create spaced underscores
            _displayWord = string.Join(" ", Enumerable.Repeat("_", _chosenWord.Length));

            _guessedLetters = new HashSet<char>();
            _incorrectGuesses = 0;
            _isGameOver = false;
            AttemptsMessage = $"{MAX_ATTEMPTS} attempts left";
            UpdateHangmanImage(); // Initialize hangman image

            // Debug output
            System.Diagnostics.Debug.WriteLine($"Chosen Word: {_chosenWord}");
        }

        public string CurrentGuess
        {
            get => _currentGuess;
            set
            {
                if (_currentGuess != value)
                {
                    _currentGuess = value.ToUpper(); // Ensure uppercase
                    System.Diagnostics.Debug.WriteLine($"Current Guess Updated: {_currentGuess}");
                    OnPropertyChanged();
                }
            }
        }

        public string DisplayWord
        {
            get => _displayWord;
            set
            {
                if (_displayWord != value)
                {
                    _displayWord = value;
                    System.Diagnostics.Debug.WriteLine($"Display Word Updated: {_displayWord}");
                    OnPropertyChanged();
                }
            }
        }

        public string HangmanImage
        {
            get => _hangmanImage;
            set
            {
                if (_hangmanImage != value)
                {
                    _hangmanImage = value;
                    System.Diagnostics.Debug.WriteLine($"Hangman Image Updated: {_hangmanImage}");
                    OnPropertyChanged();
                }
            }
        }

        public string AttemptsMessage
        {
            get => _attemptsMessage;
            set
            {
                if (_attemptsMessage != value)
                {
                    _attemptsMessage = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsGameOver
        {
            get => _isGameOver;
            set
            {
                if (_isGameOver != value)
                {
                    _isGameOver = value;
                    OnPropertyChanged();
                }
            }
        }

        public ICommand GuessCommand => new Command(GuessLetter);
        public ICommand RestartCommand => new Command(InitializeGame);

        // Method to process the guessed letter
        private void GuessLetter()
        {
            // Check if game is already over
            if (_isGameOver)
            {
                return;
            }

            // Extensive debugging
            System.Diagnostics.Debug.WriteLine($"GuessLetter Method Called");
            System.Diagnostics.Debug.WriteLine($"Current Guess: {CurrentGuess}");
            System.Diagnostics.Debug.WriteLine($"Chosen Word: {_chosenWord}");

            if (string.IsNullOrEmpty(CurrentGuess) || CurrentGuess.Length != 1)
            {
                System.Diagnostics.Debug.WriteLine("Invalid guess - empty or multiple characters");
                return;
            }

            char guess = CurrentGuess[0];

            // Check if the letter has already been guessed
            if (_guessedLetters.Contains(guess))
            {
                System.Diagnostics.Debug.WriteLine($"Letter {guess} already guessed");
                return;
            }

            _guessedLetters.Add(guess);

            if (_chosenWord.Contains(guess))
            {
                // Correct guess, update the display word
                char[] updatedWord = _displayWord.Replace(" ", "").ToCharArray();

                for (int i = 0; i < _chosenWord.Length; i++)
                {
                    if (_chosenWord[i] == guess)
                    {
                        updatedWord[i] = guess;
                    }
                }

                // Reinsert spaces
                DisplayWord = string.Join(" ", updatedWord);
                System.Diagnostics.Debug.WriteLine($"Correct Guess: {guess}");
            }
            else
            {
                // Incorrect guess, increment incorrect guesses and update image
                _incorrectGuesses++;
                UpdateHangmanImage();
                AttemptsMessage = $"{MAX_ATTEMPTS - _incorrectGuesses} attempts left";
                System.Diagnostics.Debug.WriteLine($"Incorrect Guess: {guess}");
            }

            // Clear the guess input field
            CurrentGuess = string.Empty;

            // Check for game over or win
            if (_incorrectGuesses >= MAX_ATTEMPTS)
            {
                DisplayGameOver();
            }
            else if (_displayWord.Replace(" ", "") == _chosenWord)
            {
                DisplayWin();
            }
        }

        // Method to update the hangman image based on incorrect guesses
        private void UpdateHangmanImage()
        {
            // Ensure the image index doesn't exceed 8
            int imageIndex = Math.Min(_incorrectGuesses + 1, 8);

            // Debug: Print out full details
            System.Diagnostics.Debug.WriteLine($"Updating Hangman Image");
            System.Diagnostics.Debug.WriteLine($"Incorrect Guesses: {_incorrectGuesses}");
            System.Diagnostics.Debug.WriteLine($"Calculated Image Index: {imageIndex}");

            // Set the image filename
            HangmanImage = $"hang{imageIndex}.png";

            // Additional debug to verify image change
            System.Diagnostics.Debug.WriteLine($"Set HangmanImage to: {HangmanImage}");
        }

        // Method to display a game over message
        private void DisplayGameOver()
        {
            _isGameOver = true;
            AttemptsMessage = "You've run out of attempts!";
            Application.Current.MainPage.DisplayAlert("Game Over", $"Sorry, you've died! The word was {_chosenWord}", "OK");
        }

        // Method to display a win message
        private void DisplayWin()
        {
            _isGameOver = true;
            Application.Current.MainPage.DisplayAlert("You Win!", "Congratulations, you've guessed the word!", "OK");
        }

        // Method to notify the UI of property changes
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            System.Diagnostics.Debug.WriteLine($"Property Changed: {propertyName}");
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}