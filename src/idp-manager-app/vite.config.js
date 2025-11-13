import { fileURLToPath, URL } from 'node:url'
import * as fs from 'fs'
import { defineConfig, loadEnv } from 'vite'
import vue from '@vitejs/plugin-vue'
import vueDevTools from 'vite-plugin-vue-devtools'

// https://vite.dev/config/
export default defineConfig(({ mode }) => {
  const env = loadEnv(mode, process.cwd(), '')

  return {
    plugins: [vue(), vueDevTools()],
    resolve: {
      alias: {
        '@': fileURLToPath(new URL('./src', import.meta.url)),
      },
    },
    // Optional: Silence Sass deprecation warnings. See note below.
    css: {
      preprocessorOptions: {
        scss: {
          silenceDeprecations: ['import', 'mixed-decls', 'color-functions', 'global-builtin'],
        },
      },
    },
    server: {
      open: true,
      https: env.VITE_DEV_KEY_FILE
        ? {
            key: fs.readFileSync(env.VITE_DEV_KEY_FILE),
            cert: fs.readFileSync(env.VITE_DEV_CERT_FILE),
          }
        : null,
      proxy: {
        '/api': {
          target: env.VITE_PROXY_URL,
          changeOrigin: true,
          secure: false,
          ssl: {
            rejectUnauthorized: false,
          },
        },
      },
    },
  }
})
