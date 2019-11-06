// vue.config.js
module.exports = {
  devServer: {
    proxy: {
      "/api": {
        changeOrigin: true,
        secure: false,
        target: "http://localhost:65163"
      }
    },
  },

  pwa: {
    name: "Raumplanung"
  },
};  // Now any call to (assuming your dev server is at localhost:8080)
     //http://localhost:8080/gists will be redirected to https://api.github.com/gists.
