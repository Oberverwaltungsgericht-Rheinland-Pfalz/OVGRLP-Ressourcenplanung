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
import 'dayjs/locale/de' // load on demand

dayjs.locale('de')
Vue.use(Vuetify)

Vue.config.productionTip = false

const vuetify = new Vuetify({})
Vue.use(VuetifyDialog, {
  context: {
    vuetify
  }
})

Vue.filter('toLocal', (dateVal: Date) => dayjs(dateVal).format(' DD.MM.YYYY hh:mm'))
          // new Date(dateVal).toLocaleTimeString())
Vue.filter('toLocalDate', (dateVal: Date) => dayjs(dateVal).format(' DD.MM.YYYY'))
Vue.filter('status2string', (status: number) => statusEnums[status])

new Vue({
  router,
  store,
  vuetify,
  render: (h) => h(App)
}).$mount('#app')

const statusEnums: string[] = ['offen', 'bestätigt', 'abgelehnt', 'verschoben']
