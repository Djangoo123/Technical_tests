import type { ArticleDetailDto } from "../types/ArticleDetailDto";
import type { ArticleStatus } from "../types/ArticleStatus";
import type { ArticleSummaryDto } from "../types/ArticleSummaryDto";

async function http<T>(input: RequestInfo, init?: RequestInit): Promise<T> {
  const res = await fetch(input, {
    ...init,
    headers: {
      "Content-Type": "application/json",
      ...(init?.headers || {}),
    },
  });

  if (!res.ok) {
    const text = await res.text();
    throw new Error(text || `HTTP ${res.status}`);
  }
  return (await res.json()) as T;
}

export function getArticles(params?: { q?: string; status?: ArticleStatus }) {
  const qs = new URLSearchParams();
  if (params?.q) qs.set("q", params.q);
  if (params?.status) qs.set("status", params.status);
  const suffix = qs.toString() ? `?${qs.toString()}` : "";
  return http<ArticleSummaryDto[]>(`/api/articles${suffix}`);
}

export function getArticle(id: number) {
  return http<ArticleDetailDto>(`/api/articles/${id}`);
}

export function getAdminArticles(params?: { q?: string; status?: ArticleStatus }) {
  const qs = new URLSearchParams();
  if (params?.q) qs.set("q", params.q);
  if (params?.status) qs.set("status", params.status);
  const suffix = qs.toString() ? `?${qs.toString()}` : "";
  return http<ArticleSummaryDto[]>(`/api/admin/articles${suffix}`);
}

export function updateArticleStatus(id: number, status: ArticleStatus) {
  return http<{ articleId: number; status: ArticleStatus }>(`/api/admin/articles/${id}/status`, {
    method: "POST",
    body: JSON.stringify({ status }),
  });
}
