import type { ArticleStatus } from "./ArticleStatus";

export interface ArticleSummaryDto {
  id: number;
  title: string;
  slug?: string | null;
  imageUrl?: string | null;
  status: ArticleStatus;
}