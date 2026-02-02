namespace TechnicalTestCS.Api.Dtos;

public record ArticleSummaryDto(
    int Id,
    string Title,
    string? Slug,
    string? ImageUrl,
    string Status);

public record PartnerDto(
    int Id,
    string Name,
    string? Website,
    string? Email,
    string? Phone,
    string? Description,
    string? LogoUrl);

public record ArticleDetailDto(
    int Id,
    string Title,
    string? Slug,
    string? Content,
    string? ImageUrl,
    string? BannerUrl,
    IReadOnlyList<PartnerDto> Partners,
    string Status);

public record UpdateStatusRequest(string Status);
