import type { Locale } from '../i18n';
import type { Media } from '../types/media';

interface Props {
  media: Media[];
  locale: Locale;
}

export default function MediaList({ media, locale }: Props) {
  return (
    <ul>
      {media.map((item) => (
        <li key={item.id}>
          <h2>{item.title[locale]}</h2>
          {item.description && <p>{item.description[locale]}</p>}
          <MediaPreview item={item} locale={locale} />
        </li>
      ))}
    </ul>
  );
}

interface MediaPreviewProps {
  item: Media;
  locale: Locale;
}

function MediaPreview({ item, locale }: MediaPreviewProps) {
  switch (item.mediaType) {
    case 'Image':
      return <img src={item.fileUrl} alt={item.title[locale]} />;
    case 'Video':
      return <video src={item.fileUrl} controls />;
    case 'Model3D':
      return <a href={item.fileUrl}>{item.fileName}</a>;
  }
}
