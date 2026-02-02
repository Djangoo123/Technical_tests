export interface PartnerDto {
  id: number;
  name: string;
  website?: string | null;
  email?: string | null;
  phone?: string | null;
  description?: string | null;
  logoUrl?: string | null;
}