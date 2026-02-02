
using TechnicalTestCS.Api.Services;
using TechnicalTestCS.Domain;
using Xunit;

namespace TechnicalTestCS.Tests;

public class StatusParsingTests
{
    [Theory]
    [InlineData("draft", ArticleStatus.Draft)]
    [InlineData("DRAFT", ArticleStatus.Draft)]
    [InlineData("pending", ArticleStatus.Pending)]
    [InlineData("Accepted", ArticleStatus.Accepted)]
    [InlineData("rejected", ArticleStatus.Rejected)]
    public void TryParse_accepts_case_insensitive_values(string input, ArticleStatus expected)
    {
        Assert.True(StatusParsing.TryParse(input, out var parsed));
        Assert.Equal(expected, parsed);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("nope")]
    public void TryParse_rejects_invalid_values(string input)
    {
        Assert.False(StatusParsing.TryParse(input, out _));
    }
}

