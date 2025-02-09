using System.Text.Json;
using System.Text.Json.Nodes;
using Pegasustan.Domain;

namespace Pegasustan.Tests.Domain;

[TestFixture]
public sealed class BestDealsCityTest
{
    [Test]
    public void Parse_WhenValidJsonNode_ReturnsValidBestDealsCity()
    {
        const string testJson =
        """
        {
            "code": "BDC",
            "title": "Best Deals City"
        }
        """;

        var jsonObject = JsonSerializer.Deserialize<JsonObject>(testJson);
        var city = BestDealsCity.Parse(jsonObject);

        Assert.Multiple(() =>
        {
            Assert.That(city.Code, Is.EqualTo("BDC"));
            Assert.That(city.Title, Is.EqualTo("Best Deals City"));
        });
    }

    [Test]
    public void Parse_WhenInvalidJsonNode_ThrowsException()
    {
        const string testJson =
            """
            {
                "code": "BDC"
            }
            """;

        var jsonObject = JsonSerializer.Deserialize<JsonObject>(testJson);
        Assert.Throws<ArgumentException>(() => BestDealsCity.Parse(jsonObject));
    }
}
