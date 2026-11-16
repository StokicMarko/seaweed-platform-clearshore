import { defineConfig, envField } from 'astro/config';
import react from '@astrojs/react';
import node from '@astrojs/node';

export default defineConfig({
  output: 'server',
  adapter: node({ mode: 'standalone' }),
  integrations: [react()],
  i18n: {
    defaultLocale: 'en',
    locales: ['en', 'es', 'fr'],
  },
  env: {
    schema: {
      BACKEND_URL: envField.string({ context: 'server', access: 'secret', default: 'http://localhost:5242' }),
    },
  },
});
