import { UserConfigExport, ConfigEnv, splitVendorChunkPlugin  } from 'vite'
import vue from '@vitejs/plugin-vue2'
import packageJson from './package.json'
import zipPack from 'vite-plugin-zip-pack'

export default ({ command }: ConfigEnv): UserConfigExport => {
  const version: string = packageJson.version
  const buildDate: string = new Date().toLocaleString()

  return {
    base: './',
    server: { open: true },
    build: {chunkSizeWarningLimit: 1000, rollupOptions: {output:{manualChunks: () => 'vendor'}}},
    plugins: [
      vue(),
      zipPack({outFileName: `RemaUI_v${version}.zip`, outDir: "dist"})
    ],
    define: {
      __VERSION__: '"' + version + '"',
      __BUILDDATE__: '"' + buildDate + '"'
    }
  }
}