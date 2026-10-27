import type { Locale } from '../i18n';
import type { Media } from '../types/media';

interface Props {
  items: Media[];
  locale: Locale;
}

export default function MediaList({ items, locale }: Props) {
  return (
    <ul>
      {items.map((item) => {
        const title = { en: item.titleEn, es: item.titleEs, fr: item.titleFr }[locale];
        const description = {
          en: item.descriptionEn,
          es: item.descriptionEs,
          fr: item.descriptionFr,
        }[locale];

        return (
          <li key={item.id}>
            <h2>{title}</h2>
            {description && <p>{description}</p>}
            <a href={item.url}>{item.fileName}</a> ({item.contentType})
          </li>
        );
      })}
    </ul>
  );
}
