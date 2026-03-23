import { defineConfig } from 'vitepress'
import llmstxt from 'vitepress-plugin-llms'

export default defineConfig({
  title: 'Geocoding.net',
  description: 'Provider-agnostic geocoding documentation for consumers and contributors',
  base: '/Geocoding.net/',
  srcExclude: ['README.md'],
  vite: {
    plugins: [
      llmstxt({
        title: 'Geocoding.net Documentation',
        ignoreFiles: ['node_modules/**', '.vitepress/**']
      })
    ]
  },
  head: [
    ['meta', { name: 'theme-color', content: '#0f766e' }]
  ],
  themeConfig: {
    nav: [
      { text: 'Guide', link: '/guide/what-is-geocoding-net' },
      { text: 'Providers', link: '/guide/providers' },
      { text: 'GitHub', link: 'https://github.com/exceptionless/Geocoding.net' }
    ],
    sidebar: {
      '/guide/': [
        {
          text: 'Introduction',
          items: [
            { text: 'What is Geocoding.net?', link: '/guide/what-is-geocoding-net' },
            { text: 'Getting Started', link: '/guide/getting-started' }
          ]
        },
        {
          text: 'Providers',
          items: [
            { text: 'Provider Overview', link: '/guide/providers' },
            { text: 'Google Maps', link: '/guide/providers/google' },
            { text: 'Azure Maps', link: '/guide/providers/azure-maps' },
            { text: 'HERE', link: '/guide/providers/here' },
            { text: 'MapQuest', link: '/guide/providers/mapquest' },
            { text: 'Bing Maps Compatibility', link: '/guide/providers/bing-maps' },
            { text: 'Yahoo Compatibility', link: '/guide/providers/yahoo' }
          ]
        },
        {
          text: 'Operations',
          items: [
            { text: 'Sample App', link: '/guide/sample-app' }
          ]
        }
      ]
    },
    socialLinks: [
      { icon: 'github', link: 'https://github.com/exceptionless/Geocoding.net' }
    ],
    footer: {
      message: 'Provider-agnostic geocoding for .NET.'
    },
    editLink: {
      pattern: 'https://github.com/exceptionless/Geocoding.net/edit/main/docs/:path'
    },
    search: {
      provider: 'local'
    }
  },
  markdown: {
    lineNumbers: false
  }
})