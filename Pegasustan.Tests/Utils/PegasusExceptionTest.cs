using Pegasustan.Utils;

namespace Pegasustan.Tests.Utils;

[TestFixture]
public sealed class PegasusExceptionTest
{
    [Test]
    public void Constructor_WhenNoArgs_ReturnsValidException()
    {
        var exception = new PegasusException();
        
        Assert.Multiple(() =>
        {
            Assert.That(exception.Message, Is.Not.Null.And.Not.Empty);
            Assert.That(exception.InnerException, Is.EqualTo(null));
        });
    }
    
    [Test]
    public void Constructor_WhenMessageArg_ReturnsValidException()
    {
        var exception = new PegasusException("Test");
        
        Assert.Multiple(() =>
        {
            Assert.That(exception.Message, Is.EqualTo("Test"));
            Assert.That(exception.InnerException, Is.EqualTo(null));
        });
    }
    
    [Test]
    public void Constructor_WhenBothArgs_ReturnsValidException()
    {
        var inner = new Exception();
        var exception = new PegasusException("Test", inner);
        
        Assert.Multiple(() =>
        {
            Assert.That(exception.Message, Is.EqualTo("Test"));
            Assert.That(exception.InnerException, Is.EqualTo(inner));
        });
    }
}
