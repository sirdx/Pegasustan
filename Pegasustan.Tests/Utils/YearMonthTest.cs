﻿using Pegasustan.Utils;

namespace Pegasustan.Tests.Utils;

[TestFixture]
public sealed class YearMonthTest
{
    [Test]
    public void Parse_WhenValidFormat_ReturnsValidYearMonth()
    {
        var yearMonth = YearMonth.Parse("2024-07");

        Assert.Multiple(() =>
        {
            Assert.That(yearMonth.Year, Is.EqualTo(2024));
            Assert.That(yearMonth.Month, Is.EqualTo(7));
        });
    }

    [TestCase("")]
    [TestCase("-")]
    [TestCase("2024-13")]
    [TestCase("0000000")]
    public void Parse_WhenInvalidFormat_ThrowsException(string input)
    {
        Assert.Throws<ArgumentException>(() => YearMonth.Parse(input));
    }

    [Test]
    public void ToYearMonth_WhenValidDateTime_ReturnsValidYearMonth()
    {
        var yearMonth = new DateTime(2023, 12, 31).ToYearMonth();

        Assert.Multiple(() =>
        {
            Assert.That(yearMonth.Year, Is.EqualTo(2023));
            Assert.That(yearMonth.Month, Is.EqualTo(12));
        });
    }
}