using Microsoft.AspNetCore.Mvc;
using TechnicalTestCS.Api.Dtos;
using TechnicalTestCS.Api.Interfaces;
using TechnicalTestCS.Api.Services;
using TechnicalTestCS.Domain;
using TechnicalTestCS.Infrastructure.External.Dtos;
using TechnicalTestCS.Infrastructure.External.Interfaces;

namespace TechnicalTestCS.Api.Controllers;

[ApiController]
[Route("api/admin/articles")]
public sealed class AdminController : ControllerBase
{
    private readonly IArticlesService _articles;
    private readonly IArticleStatusService _status;
    private readonly IExternalArticlesClient _external;

    public AdminController(IArticlesService articles, IArticleStatusService status, IExternalArticlesClient external)
    {
        _articles = articles;
        _status = status;
        _external = external;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Get([FromQuery] string? q, [FromQuery] string? status, CancellationToken ct)
    {
        try
        {
            var items = await _articles.GetAdminArticles(q, status, ct);
            return Ok(items);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(Problem(title: "Invalid query parameter", detail: ex.Message, statusCode: 400));
        }
    }

    [HttpPost("{id:int}/status")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateStatus([FromRoute] int id, [FromBody] UpdateStatusRequest req, CancellationToken ct)
    {
        if (req is null || string.IsNullOrWhiteSpace(req.Status))
            return BadRequest(Problem(title: "Invalid body", detail: "status is required.", statusCode: 400));

        if (!StatusParsing.TryParse(req.Status, out var next))
            return BadRequest(Problem(title: "Invalid status", detail: $"Unknown status '{req.Status}'.", statusCode: 400));

        ArticleExternalDto? exists = await _external.GetArticle(id, ct);
        if (exists is null)
            return NotFound(Problem(title: "Not Found", detail: $"Article {id} not found in external API.", statusCode: 404));

        try
        {
            ArticleStatus updated = await _status.UpdateStatus(id, next, ct);
            return Ok(new { articleId = id, status = updated.ToApiString() });
        }
        catch (InvalidTransitionException ex)
        {
            return BadRequest(Problem(title: "Invalid transition", detail: ex.Message, statusCode: 400));
        }
    }
}
