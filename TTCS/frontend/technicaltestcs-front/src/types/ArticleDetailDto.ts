import type { ArticleStatus } from "./ArticleStatus";
import type { PartnerDto } from "./PartnerDto";

export interface ArticleDetailDto {
  id: number;
  title: string;
  slug?: string | null;
  content?: string | null;
  imageUrl?: string | null;
  bannerUrl?: string | null;
  partners: PartnerDto[];
  status: ArticleStatus;
}