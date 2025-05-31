using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Moq.Protected;
using NUnit.Framework;

[TestFixture]
public class LogicsTests
{
    [SetUp]
    public void Setup()
    {
        Logics.ResetState();
    }

    [TearDown]
    public void Teardown()
    {
        Logics.clientHttp = new HttpClient();
    }

    // Тесты для TextCheck
    [Test]
    [TestCase("hello", true)]
    [TestCase("world", true)]
    [TestCase("test123", false)]
    [TestCase("space test", false)]
    [TestCase("", false)]
    public void TextCheck_ValidatesInputCorrectly(string input, bool expected)
    {
        // Arrange
        Logics.text = input;

        // Act
        bool result = Logics.TextCheck();

        // Assert
        Assert.AreEqual(expected, result);

        if (!expected)
        {
            StringAssert.Contains("не подходящие символы", Logics.finalMessage);
        }
    }

    [Test]
    public void TextCheck_WithInvalidCharacters_ShowsCorrectErrorMessage()
    {
        // Arrange
        Logics.text = "abc123@";

        // Act
        Logics.TextCheck();

        // Assert
        StringAssert.Contains("1", Logics.finalMessage);
        StringAssert.Contains("2", Logics.finalMessage);
        StringAssert.Contains("3", Logics.finalMessage);
        StringAssert.Contains("@", Logics.finalMessage);
    }

    // Тесты для StringActions
    [Test]
    [TestCase("abcd", "badc")]
    [TestCase("abcde", "edcbaabcde")]
    [TestCase("a", "aa")]
    [TestCase("", "")]
    public void StringActions_TransformsStringCorrectly(string input, string expected)
    {
        // Arrange
        Logics.text = input;

        // Act
        Logics.StringActions();

        // Assert
        Assert.AreEqual(expected, Logics.finalText);
    }

    [Test]
    public void StringActions_AfterTextCheck_AppendsToFinalMessage()
    {
        // Arrange
        Logics.text = "test";

        // Act
        Logics.StringActions();

        // Assert
        StringAssert.Contains(Logics.finalText, Logics.finalMessage);
    }

    // Тесты для RepeatCharacters
    [Test]
    public void RepeatCharacters_CountsSymbolsCorrectly()
    {
        // Arrange
        Logics.finalText = "aabbccc";

        // Act
        Logics.RepeatCharacters();

        // Assert
        StringAssert.Contains("Символ a повторялся 2 раз(а)", Logics.finalMessage);
        StringAssert.Contains("Символ b повторялся 2 раз(а)", Logics.finalMessage);
        StringAssert.Contains("Символ c повторялся 3 раз(а)", Logics.finalMessage);
    }

    [Test]
    public void RepeatCharacters_WithEmptyString_DoesNothing()
    {
        // Arrange
        Logics.finalText = "";

        // Act
        Logics.RepeatCharacters();

        // Assert
        Assert.IsEmpty(Logics.finalMessage);
    }

    // Тесты для FindingLargestSubstring
    [Test]
    [TestCase("aeiou", "aeiou")]
    [TestCase("test", "e")]
    [TestCase("bcd", "")]
    [TestCase("apple", "apple")]
    [TestCase("banana", "anana")]
    public void FindingLargestSubstring_FindsCorrectSubstring(string input, string expected)
    {
        // Arrange
        Logics.finalText = input;

        // Act
        Logics.FindingLargestSubstring();

        // Assert
        StringAssert.Contains(expected, Logics.finalMessage);
    }

    // Тесты для SortSelection
    [Test]
    [TestCase("1", "edcba", "abcde")]
    [TestCase("2", "edcba", "abcde")]
    [TestCase("3", "test", "Выберите вид сортировки")]
    public void SortSelection_AppliesCorrectSorting(string sortOption, string input, string expected)
    {
        // Arrange
        Logics.sortSelection = sortOption;
        Logics.finalText = input;

        // Act
        Logics.SortSelection();

        // Assert
        StringAssert.Contains(expected, Logics.finalMessage);
    }

    // Тесты для RandomNumberGenerate
    [Test]
    public async Task RandomNumberGenerate_WhenApiWorks_RemovesCorrectCharacter()
    {
        // Arrange
        Logics.finalText = "hello";
        int originalLength = Logics.finalText.Length;

        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("[2]", Encoding.UTF8, "application/json")
            });

        Logics.clientHttp = new HttpClient(handlerMock.Object);

        // Act
        await Logics.RandomNumberGenerate();

        // Assert
        Assert.AreEqual(originalLength - 1, Logics.finalText.Length);
        Assert.AreEqual("helo", Logics.finalText);
        StringAssert.Contains(Logics.finalText, Logics.finalMessage);
    }

    [Test]
    public async Task RandomNumberGenerate_WhenApiFails_UsesLocalRandom()
    {
        // Arrange
        Logics.finalText = "hello";
        int originalLength = Logics.finalText.Length;

        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ThrowsAsync(new HttpRequestException("API error"));

        Logics.clientHttp = new HttpClient(handlerMock.Object);

        // Act
        await Logics.RandomNumberGenerate();

        // Assert
        Assert.AreEqual(originalLength - 1, Logics.finalText.Length);
        StringAssert.Contains(Logics.finalText, Logics.finalMessage);
    }

    [Test]
    public async Task RandomNumberGenerate_WithEmptyString_DoesNothing()
    {
        // Arrange
        Logics.finalText = "";

        // Act
        await Logics.RandomNumberGenerate();

        // Assert
        Assert.IsEmpty(Logics.finalText);
        Assert.IsEmpty(Logics.finalMessage);
    }

    // Интеграционные тесты для StartLogic
    [Test]
    public async Task StartLogic_WithValidInput_ReturnsCompleteResult()
    {
        // Arrange
        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("[3]", Encoding.UTF8, "application/json")
            });

        Logics.clientHttp = new HttpClient(handlerMock.Object);

        // Act
        string result = await Logics.StartLogic("hello", "1");

        // Assert
        StringAssert.Contains("ollehhello", result);
        StringAssert.Contains("Символ", result);
        StringAssert.Contains("ll", result);
        StringAssert.Contains("abcde", result);
        StringAssert.Contains("helo", result);
    }

    [Test]
    public async Task StartLogic_WithInvalidInput_ReturnsErrorMessage()
    {
        // Act
        string result = await Logics.StartLogic("hello123", "1");

        // Assert
        StringAssert.Contains("не подходящие символы", result);
        StringAssert.Contains("1", result);
        StringAssert.Contains("2", result);
        StringAssert.Contains("3", result);
    }
}

[TestFixture]
public class SortingTests
{
    [SetUp]
    public void Setup()
    {
        Sorting.ResetState();
    }

    [Test]
    [TestCase("edcba", "abcde")]
    [TestCase("hello", "ehllo")]
    [TestCase("banana", "aaabnn")]
    public void Quicksort_SortsCorrectly(string input, string expected)
    {
        // Arrange
        Sorting.finalTextIndex = input.ToCharArray().ToList();

        // Act
        Sorting.Quicksort();

        // Assert
        Assert.AreEqual(expected, new string(Sorting.finalTextIndex.ToArray()));
    }

    [Test]
    [TestCase("edcba", "abcde")]
    [TestCase("hello", "ehllo")]
    [TestCase("banana", "aaabnn")]
    public void TreeSort_SortsCorrectly(string input, string expected)
    {
        // Arrange
        Sorting.finalTextIndex = input.ToCharArray().ToList();

        // Act
        Sorting.TreeSort();

        // Assert
        Assert.AreEqual(expected, new string(Sorting.finalTextIndex.ToArray()));
    }

    [Test]
    public void Quicksort_WithEmptyList_DoesNothing()
    {
        // Arrange
        Sorting.finalTextIndex = new List<char>();

        // Act
        Sorting.Quicksort();

        // Assert
        Assert.IsEmpty(Sorting.finalTextIndex);
    }
}