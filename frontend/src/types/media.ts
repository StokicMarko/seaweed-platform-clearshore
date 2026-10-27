import type { LocalizedText } from './localized-text';
import type { Machine } from './machine';

export type MediaType = 'Image' | 'Video' | 'Model3D';

export interface Media {
  id: number;
  machine: Machine | null;
  mediaType: MediaType;
  title: LocalizedText;
  description: LocalizedText | null;
  fileName: string;
  contentType: string;
  fileUrl: string;
  publishedAt: string;
}
