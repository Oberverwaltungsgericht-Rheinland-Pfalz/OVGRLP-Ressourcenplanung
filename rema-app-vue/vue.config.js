// vue.config.js
const Agent = require('agentkeepalive')

module.exports = {
  devServer: {
    port: 5002,
    proxy: {
      '/api': {
        target: 'http://localhost:52575',
        changeOrigin: true,
        pathRewrite: {
          '^/api': ''
        },
        logLevel: 'debug',
        agent: new Agent({
          maxSockets: 100,
          keepAlive: true,
          maxFreeSockets: 10,
          keepAliveMsecs: 1000,
          timeout: 60000,
          freeSocketTimeout: 30000
        }),
        onProxyRes: proxyRes => {
          const key = 'www-authenticate'
          proxyRes.headers[key] = proxyRes.headers[key] && proxyRes.headers[key].split(',')
        }
      }
    }
  },
  /*  devServer: {
    proxy: {
      "/api": {
        changeOrigin: true,
        secure: false,
        target: "http://localhost:65163",
      },
    },
  },
*/
  pwa: {
    name: 'Raumplanung'
  }
} // Now any call to (assuming your dev server is at localhost:8080)
// http://localhost:8080/gists will be redirected to https://api.github.com/gists.
