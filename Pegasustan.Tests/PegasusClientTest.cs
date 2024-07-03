namespace Pegasustan.Tests;

[TestFixture]
public sealed class PegasusClientTest
{
    private PegasusClient _pegasusClient;
    
    [SetUp]
    public async Task Setup()
    {
        _pegasusClient = await PegasusClient.CreateAsync();
    }

    [Test]
    public void MakeSureDefaultLanguageIsEnglish()
    {
        Assert.That(_pegasusClient.DefaultLanguage.Code.ToLower(), Is.EqualTo("en"));
    }
    
    // TODO: More tests
}
