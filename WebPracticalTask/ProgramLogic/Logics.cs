using Microsoft.AspNetCore.Http.HttpResults;
using System.Text.Json;
using static System.Net.Mime.MediaTypeNames;

namespace WebPracticalTask.ProgramLogic
{
    public class Logics
    {
        public static string englishAlphabet = "abcdefghijklmnopqrstuvwxyz";
        public static string text;
        public static string finalText;
        public static string sortSelection;

        public static string finalMessage;

        public static readonly HttpClient clientHttp;
        public static readonly Random localRandom = new Random();

        public static async Task<string> StartLogic(string enteredText, string sort)
        {
            text = enteredText;
            sortSelection = sort;

            if (TextCheck() == false)
                return finalMessage;
            TextCheck();
            StringActions();
            RepeatCharacters();
            FindingLargestSubstring();
            SortSelection();
            await RandomNumberGenerate();
            return finalMessage;
        }

        //Проверка на корректный ввод
        public static bool TextCheck()
        {
            try
            {
                bool[] textBools = new bool[text.Length];
                bool checkSymbol = false;
                for (int i = 0; i < text.Length; i++)
                {
                    foreach (char j in englishAlphabet)
                    {
                        if (text[i] == j)
                        {
                            checkSymbol = true;
                        }
                    }
                    textBools[i] = checkSymbol;
                    checkSymbol = false;
                }
                string inappropriateCharacters = null;
                for (int i = 0; i < textBools.Length; i++)
                {
                    if (textBools[i] == false)
                    {
                        inappropriateCharacters += $" {text[i]} ";
                    }
                }
                if (inappropriateCharacters != null)
                {
                    throw new Exception($"Были введены не подходящие символы:{inappropriateCharacters}");
                }
                return true;
            }
            catch (Exception ex)
            {
                finalMessage = "Произошла ошибка: " + ex.Message;
                return false;
            }
        }

        //Действия со строкой
        public static void StringActions()
        {
            if (text.Length % 2 == 0)
            {
                for (int i = text.Length / 2 - 1; i >= 0; i--)
                {
                    finalText += text[i];
                }
                for (int i = text.Length - 1; i >= text.Length / 2; i--)
                {
                    finalText += text[i];
                }
            }
            else
            {
                for (int i = text.Length - 1; i >= 0; i--)
                {
                    finalText += text[i];
                }
                finalText += text;
            }
            finalMessage += $"{finalText}\n";
        }

        //Подсчёт каждого символа в обработанной строке
        public static void RepeatCharacters()
        {
            var uniqueSymbol = new HashSet<char>();
            for (int i = 0; i < finalText.Length; i++)
            {
                uniqueSymbol.Add(finalText[i]);
            }
            var uniqueSymbolList = new List<char>();
            foreach (var i in uniqueSymbol)
            {
                uniqueSymbolList.Add(i);
            }

            int symbolCount = 0;
            for (int i = 0; i < uniqueSymbolList.Count; i++)
            {
                for (int j = 0; j < finalText.Length; j++)
                {
                    if (uniqueSymbolList[i] == finalText[j])
                    {
                        symbolCount++;
                    }
                }
                finalMessage += $"Символ {uniqueSymbolList[i]} повторялся {symbolCount} раз(а)\n";
                symbolCount = 0;
            }
        }

        //Нахождение наибольшей подстроки, которая начинается и заканчивается на гласную букву из «aeiouy»
        public static void FindingLargestSubstring()
        {
            string vowelsLetters = "aeiouy";
            bool findIndex = false;
            int startIndex = 0;
            for (int i = 0; i < finalText.Length; i++)
            {
                foreach (char j in vowelsLetters)
                {
                    if (finalText[i] == j)
                    {
                        startIndex = i;
                        findIndex = true;
                        break;
                    }
                }
                if (findIndex == true)
                {
                    findIndex = false;
                    break;
                }
            }
            int finishIndex = 0;
            for (int i = finalText.Length - 1; i >= 0; i--)
            {
                foreach (char j in vowelsLetters)
                {
                    if (finalText[i] == j)
                    {
                        finishIndex = i;
                        findIndex = true;
                        break;
                    }
                }
                if (findIndex == true)
                {
                    findIndex = false;
                    break;
                }
            }
            for (int i = startIndex; i <= finishIndex; i++)
            {
                finalMessage += $"{finalText[i]}";
            }
        }

        //пользователь выбирает сортировку
        public static void SortSelection()
        {
            switch (sortSelection)
            {
                case "1":
                    Sorting.Quicksort();
                    finalMessage += $"\n{finalText}";
                    break;
                case "2":
                    Sorting.TreeSort();
                    finalMessage += $"\n{finalText}";
                    break;
                default:
                    finalMessage += "\nВыберите вид сортировки (1 или 2)";
                    break;
            }
        }

        //программа получает случайное число, которое меньше чем число символов в обработанной строке и удаляет символ в той позиции, номер которой вернёт случайный генератор.
        public static async Task RandomNumberGenerate()
        {
            int randomIndex = await GetRandomIndexAsync();
            finalText = finalText.Remove(randomIndex, 1);
            finalMessage += $"\n{finalText}";
        }

        public static async Task<int> GetRandomIndexAsync()
        {
            try
            {
                string apiUrl = $"https://www.randomnumberapi.com/api/v1.0/random?min=0&max={finalText.Length - 1}&count=1";
                var response = await clientHttp.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var numbers = JsonSerializer.Deserialize<int[]>(jsonString);

                    if (numbers != null && numbers.Length > 0)
                    {
                        return numbers[0];
                    }
                }
            }
            catch
            {
                int result = localRandom.Next(0, finalText.Length);
                return result;
            }

            return localRandom.Next(0, finalText.Length);
        }
    }
}
