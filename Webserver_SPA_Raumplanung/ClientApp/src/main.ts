import '@babel/polyfill'
import Vue from 'vue'
import './plugins/axios'
import './plugins/vuetify'
import App from './App.vue'
import router from './router'
import store from '@/store/index'
import './registerServiceWorker'
import dayjs from 'dayjs'

import VuetifyDialog from 'vuetify-dialog'
import 'vuetify-dialog/dist/vuetify-dialog.css'
import Vuetify from 'vuetify'
import 'vuetify/dist/vuetify.min.css'
Vue.use(Vuetify)

Vue.config.productionTip = false

const vuetify = new Vuetify({})
Vue.use(VuetifyDialog, {
  context: {
    vuetify
  }
})

Vue.filter('toLocal', (dateVal: Date) => dayjs(dateVal).format(' DD.MM.YYYY hh:mm'))
Vue.filter('toLocalDate', (dateVal: Date) => dayjs(dateVal).format(' DD.MM.YYYY'))

new Vue({
  router,
  store,
  vuetify,
  render: (h) => h(App)
}).$mount('#app')
