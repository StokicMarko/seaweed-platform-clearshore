import en from './en.json';
import es from './es.json';
import fr from './fr.json';

export const messages = { en, es, fr };

export type Locale = keyof typeof messages;
