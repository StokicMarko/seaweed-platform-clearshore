import { BACKEND_URL } from 'astro:env/server';
import type { Media } from '../types/media';

async function getJson<T>(path: string): Promise<T> {
  const response = await fetch(new URL(path, BACKEND_URL));
  if (!response.ok) {
    throw new Error(`Backend returned ${response.status} for ${path}`);
  }
  return response.json();
}

export async function getMedia(): Promise<Media[]> {
  const media = await getJson<Media[]>('/api/media');
  return media.map((item) => ({ ...item, fileUrl: new URL(item.fileUrl, BACKEND_URL).toString() }));
}
