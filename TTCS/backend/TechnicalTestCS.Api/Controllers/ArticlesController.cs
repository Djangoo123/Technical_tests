using Microsoft.AspNetCore.Mvc;
using TechnicalTestCS.Api.Dtos;
using TechnicalTestCS.Api.Interfaces;

namespace TechnicalTestCS.Api.Controllers;

[ApiController]
[Route("api/articles")]
public sealed class ArticlesController : ControllerBase
{
    private readonly IArticlesService _svc;

    public ArticlesController(IArticlesService svc) => _svc = svc;

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status502BadGateway)]
    public async Task<IActionResult> Get([FromQuery] string? q, [FromQuery] string? status, CancellationToken ct)
    {
        var items = await _svc.GetArticles(q, status, ct);
        return Ok(items);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status502BadGateway)]
    public async Task<IActionResult> GetById(int id, CancellationToken ct)
    {
        ArticleDetailDto? item = await _svc.GetArticle(id, ct);
        return item is null
            ? NotFound(Problem(title: "Not Found", detail: $"Article {id} not found.", statusCode: 404))
            : Ok(item);
    }

}
