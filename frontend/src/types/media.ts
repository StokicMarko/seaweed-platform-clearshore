import type { LocalizedText } from './localized-text';
import type { Technology } from './technology';

export type MediaType = 'Image' | 'Video' | 'Model3D';

export interface Media {
  id: number;
  technologies: Technology[];
  mediaType: MediaType;
  title: LocalizedText;
  description: LocalizedText | null;
  fileName: string;
  contentType: string;
  fileUrl: string;
  publishedAt: string;
}
