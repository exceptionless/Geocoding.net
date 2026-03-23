# Geocoding.net Documentation

This folder contains the VitePress documentation site for Geocoding.net.

## Prerequisites

- Node.js 20.19.x LTS or 22.12+
- npm

## Getting Started

Install dependencies from the `docs/` directory:

```bash
npm ci
```

Start the development server:

```bash
npm run dev
```

Build the static site:

```bash
npm run build
```

Preview the built site locally:

```bash
npm run preview
```

## Structure

```text
docs/
├── .vitepress/
│   └── config.ts
├── guide/
│   ├── getting-started.md
│   ├── providers/
│   │   ├── azure-maps.md
│   │   ├── bing-maps.md
│   │   ├── google.md
│   │   ├── here.md
│   │   ├── mapquest.md
│   │   └── yahoo.md
│   ├── providers.md
│   ├── sample-app.md
│   └── what-is-geocoding-net.md
├── index.md
├── package-lock.json
├── package.json
└── README.md
```

## Notes

- `guide/` contains the published consumer and contributor documentation.
- Keep `README.md`, `AGENTS.md`, and the guide pages aligned when provider support or contributor workflows change.
