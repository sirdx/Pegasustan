using System.Text.Json;
using System.Text.Json.Nodes;
using Pegasustan.Domain;

namespace Pegasustan.Tests.Domain;

[TestFixture]
public sealed class LanguageTest
{
    [Test]
    public void Parse_WhenValidJsonNode_ReturnsValidLanguage()
    {
        const string testJson =
        """
        {
            "lang": {
                "code": "PG",
                "name": "Pegasus"
            }
        }
        """;

        var jsonObject = JsonSerializer.Deserialize<JsonObject>(testJson);
        var langNode = jsonObject["lang"];
        var language = Language.Parse(langNode);

        Assert.Multiple(() =>
        {
            Assert.That(language.Code, Is.EqualTo("PG"));
            Assert.That(language.Name, Is.EqualTo("Pegasus"));
        });
    }

    [Test]
    public void Parse_WhenInvalidJsonNode_ThrowsException()
    {
        const string testJson =
            """
            {
                "lang": {
                    "code": "PG"
                }
            }
            """;

        var jsonObject = JsonSerializer.Deserialize<JsonObject>(testJson);
        var langNode = jsonObject["lang"];
        Assert.Throws<ArgumentException>(() => Language.Parse(langNode));
    }
}