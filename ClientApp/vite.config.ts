import { defineConfig } from 'vite'
import { svelte } from '@sveltejs/vite-plugin-svelte'

// https://vitejs.dev/config/
export default defineConfig({
    plugins: [svelte()],

    build: {
        outDir: "dist"
    },

    server: {
        https: false,
        proxy: {
            '/api': {
                target: process.env.ASPNETCORE_HTTPS_PORT
                    ? `https://localhost:${process.env.ASPNETCORE_HTTPS_PORT}`
                    : process.env.ASPNETCORE_URLS
                        ? process.env.ASPNETCORE_URLS.split(";")[0]
                        : "http://localhost:40457",
                changeOrigin: true,
                secure: false,
                ws: true,
                rewrite: (path) => path.replace(/^\/api/, '/api')
            }
        }
    }
})
