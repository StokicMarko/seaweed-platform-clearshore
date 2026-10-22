# Seaweed by DESMI: Public Website

Public website for DESMI Seaweed, available in English, Spanish and French. Built with Astro (server rendered with the Node adapter) and React islands for the interactive parts: the references map, the media gallery filter and the language switcher.

The site talks only to the .NET REST API. It never talks to the data server.

## Requirements

- Node.js 22.23.4 or newer

## Commands

Run from the `frontend` folder.

| Command           | Action                                      |
| :---------------- | :------------------------------------------ |
| `npm install`     | Install dependencies                        |
| `npm run dev`     | Start the dev server at `localhost:4321`    |
| `npm run build`   | Build the production server to `./dist/`    |
| `npm run preview` | Preview the production build locally        |

## Structure

```text
frontend/
├── public/        static assets
└── src/
    ├── i18n/      translation strings, one file per language
    └── pages/     routes
```

## Languages

Routes are served under `/en`, `/es` and `/fr`. All interface text lives in `src/i18n/en.json`, `src/i18n/es.json` and `src/i18n/fr.json`, and the three files must always contain the same keys. No interface text is hardcoded in components.
