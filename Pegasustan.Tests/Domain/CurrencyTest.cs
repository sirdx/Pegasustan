using System.Text.Json;
using System.Text.Json.Nodes;
using Pegasustan.Domain;

namespace Pegasustan.Tests.Domain;

[TestFixture]
public sealed class CurrencyTest
{
    [Test]
    public void Parse_WhenValidJsonNode_ReturnsValidCurrency()
    {
        const string testJson = "\"TRY\"";

        var jsonNode = JsonSerializer.Deserialize<JsonNode>(testJson);
        var currency = Currency.Parse(jsonNode, ["TRY"]);

        Assert.Multiple(() =>
        {
            Assert.That(currency.Code, Is.EqualTo("TRY"));
            Assert.That(currency.SupportsCheapestFare, Is.EqualTo(true));
        });
    }

    [TestCase("true")]
    [TestCase("123")]
    [TestCase("null")]
    public void Parse_WhenInvalidJsonNode_ThrowsException(string input)
    {
        var jsonNode = JsonSerializer.Deserialize<JsonNode>(input);
        Assert.Throws<ArgumentException>(() => Currency.Parse(jsonNode, []));
    }
}
