import { UserConfigExport, ConfigEnv, splitVendorChunkPlugin  } from 'vite'
import vue from '@vitejs/plugin-vue2'
import packageJson from './package.json'
import zipPack from 'vite-plugin-zip-pack'
import mockSimple from 'vite-plugin-mock-simple'
import mockRoutes from './mockRoutes'
import Components from 'unplugin-vue-components/vite'
import {VuetifyResolver} from 'unplugin-vue-components/resolvers'

export default ({ command }: ConfigEnv): UserConfigExport => {
  const version: string = packageJson.version
  const buildDate: string = new Date().toLocaleString()

  return {
    resolve: {
      alias: {
        vue: 'vue/dist/vue.esm.js',
      }
    },
    base: './',
    server: { open: true },
    build: {chunkSizeWarningLimit: 1000, rollupOptions: {output:{manualChunks: () => 'vendor'}}},
    plugins: [
      vue(),
      mockSimple(mockRoutes),
      Components({
        dts: true,
        resolvers: [ VuetifyResolver()],
        types: [
          {
            from: 'vue-router',
            names: ['RouterLink', 'RouterView'],
          },
        ],
        dirs: ['src/components'],
        // auto import for directives
        directives: false,
      }),
      zipPack({outFileName: `RemaUI_v${version}.zip`, outDir: "dist"})
    ],
    define: {
      __VERSION__: '"' + version + '"',
      __BUILDDATE__: '"' + buildDate + '"'
    }
  }
}
