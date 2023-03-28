using Cledev.Core.Extensions;
using FluentAssertions;
using NUnit.Framework;

namespace Cledev.Core.Tests;

public class FormatCodeTests
{
    [Test]
    public void GivenContentWithoutCode_ThenResultWithoutTagsIsReturned()
    {
        const string content = "Blah blah blah Foo foo";
        var actual = content.FormatCode();
        actual.Should().Be("Blah blah blah Foo foo");
    }

    [Test]
    public void GivenContentWithCode_WhenIncludesSingleValue_ThenResultWithOpenTagIsReturned()
    {
        const string content = "Blah blah blah ```Foo foo";
        var actual = content.FormatCode();
        actual.Should().Be("Blah blah blah <pre>Foo foo");
    }

    [Test]
    public void GivenContentWithCode_WhenIncludesTwoValues_ThenResultWithOpenAndCloseTagsIsReturned()
    {
        const string content = "Blah blah blah ```Foo foo``` blah blah blah";
        var actual = content.FormatCode();
        actual.Should().Be("Blah blah blah <pre>Foo foo</pre> blah blah blah");
    }

    [Test]
    public void GivenContentWithCode_WhenIncludesOddValues_ThenResultWithAnOddOpenTagIsReturned()
    {
        const string content = "Blah blah blah ```Foo foo``` blah ```blah blah";
        var actual = content.FormatCode();
        actual.Should().Be("Blah blah blah <pre>Foo foo</pre> blah <pre>blah blah");
    }

    [Test]
    public void GivenContentWithCode_WhenIncludesMultipleMatchingValues_ThenResultWithMultipleOpenAndCloseTagsIsReturned()
    {
        const string content = "Blah blah blah ```Foo foo``` blah ```blah``` blah";
        var actual = content.FormatCode();
        actual.Should().Be("Blah blah blah <pre>Foo foo</pre> blah <pre>blah</pre> blah");
    }

    [Test]
    public void GivenContentWithoutCode_ThenItShouldReturnFalse()
    {
        const string content = "Blah blah blah Foo foo";
        var actual = content.ContainsCode();
        actual.Should().BeFalse();
    }
    
    [Test]
    public void GivenContentWithCode_ThenItShouldReturnTrue()
    {
        const string content = "Blah blah blah ```Foo foo``` blah ```blah``` blah ```blah```";
        var actual = content.ContainsCode();
        actual.Should().BeTrue();
    }
}
