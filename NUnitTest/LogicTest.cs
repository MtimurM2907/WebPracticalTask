using NUnit.Framework;
using WebPracticalTask.ProgramLogic;

namespace NUnitTest
{
    [TestFixture]
    public class LogicTest
    {
        //все поля класса сбрасываются (пустые строки)
        [SetUp]
        public void Setup()
        {
            Logics.text = string.Empty;
            Logics.finalText = string.Empty;
            Logics.finalMessage = string.Empty;
            Logics.sortSelection = string.Empty;
        }


        //тесты метода TextCheck()
        [Test]
        public void TextCheck_ValidInput_ReturnsTrue()
        {
            Logics.text = "abc";

            bool result = Logics.TextCheck();

            Assert.That(result, Is.True);
        }

        [Test]
        public void TextCheck_InvalidInput_ReturnsFalse()
        {
            Logics.text = "abc123";

            bool result = Logics.TextCheck();

            Assert.That(result, Is.False);
            Assert.That(Logics.finalMessage, Does.Contain("не подходящие символы"));
        }


        //тесты метода StringActions()
        [Test]
        public void StringActions_EvenLength_ReversesCorrectly()
        {
            Logics.text = "abcd";
            Logics.finalText = string.Empty;

            Logics.StringActions();

            Assert.That(Logics.finalText, Is.EqualTo("badc"));
        }

        [Test]
        public void StringActions_OddLength_ReversesAndAppends()
        {
            Logics.text = "abc";
            Logics.finalText = string.Empty;

            Logics.StringActions();

            Assert.That(Logics.finalText, Is.EqualTo("cbaabc"));
        }


        //тесты метода RepeatCharacters()
        [Test]
        public void RepeatCharacters_CountsCorrectly()
        {
            Logics.finalText = "aabbc";
            Logics.finalMessage = string.Empty;

            Logics.RepeatCharacters();

            Assert.That(Logics.finalMessage, Does.Contain("Символ a повторялся 2 раз(а)"));
            Assert.That(Logics.finalMessage, Does.Contain("Символ b повторялся 2 раз(а)"));
            Assert.That(Logics.finalMessage, Does.Contain("Символ c повторялся 1 раз(а)"));
        }


        //тесты метода FindingLargestSubstring()
        [Test]
        public void FindingLargestSubstring_WithVowels_FindsCorrectSubstring()
        {
            Logics.finalText = "abcde";
            Logics.finalMessage = string.Empty;

            Logics.FindingLargestSubstring();

            Assert.That(Logics.finalMessage, Is.EqualTo("abcde"));
        }

        [Test]
        public void FindingLargestSubstring_NoVowels_ReturnsEmpty()
        {
            Logics.finalText = "bcd";
            Logics.finalMessage = string.Empty;

            Logics.FindingLargestSubstring();

            Assert.That(Logics.finalMessage, Is.Empty);
        }


        //тесты метода SortSelection()
        [Test]
        public void SortSelection_Quicksort_AppliesQuicksort()
        {
            Logics.finalText = "cba";
            Logics.sortSelection = "1";
            Logics.finalMessage = string.Empty;

            Logics.SortSelection();

            Assert.That(Logics.finalMessage, Does.Contain("abc"));
        }

        [Test]
        public void SortSelection_TreeSort_AppliesTreeSort()
        {
            Logics.finalText = "cba";
            Logics.sortSelection = "2";
            Logics.finalMessage = string.Empty;

            Logics.SortSelection();

            Assert.That(Logics.finalMessage, Does.Contain("abc"));
        }

        [Test]
        public void SortSelection_InvalidSelection_ShowsErrorMessage()
        {
            Logics.finalText = "cba";
            Logics.sortSelection = "3";
            Logics.finalMessage = string.Empty;

            Logics.SortSelection();

            Assert.That(Logics.finalMessage, Does.Contain("Выберите вид сортировки (1 или 2)"));
        }
    }
}
