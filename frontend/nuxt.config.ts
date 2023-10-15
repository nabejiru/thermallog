// https://nuxt.com/docs/api/configuration/nuxt-config
export default defineNuxtConfig({
  ssr: false,
  target: 'static',
  publicRuntimeConfig: {
    appName: "tempreture-chart",
  },
  app: {
    baseURL: process.env.BASE_URL
  },
  css: [
    "vuetify/lib/styles/main.sass"
  ],
  build: {
      transpile: ["vuetify"]
  },
  devtools: { enabled: true },
  
  vite : {
    define: {
      "process.env.DEBUG": false
    },
    server: {
      watch: {
        usePolling: true
      },
      proxy: {
        '^/api': {
          target: process.env.API_ROOT_URL, 
          secure: false,
          changeOrigin: true,
        }
      },
    },
    base: "_nuxt"
  }

})
