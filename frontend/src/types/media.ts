export interface Media {
  id: number;
  machine: string | null;
  titleEn: string;
  titleFr: string;
  titleEs: string;
  descriptionEn: string | null;
  descriptionFr: string | null;
  descriptionEs: string | null;
  fileName: string;
  url: string;
  contentType: string;
  createdAt: string;
}
