import 'material-design-icons-iconfont/dist/material-design-icons.css'
import Vue from 'vue'
import './plugins/axios'
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
Vue.use(Vuetify, {
  iconfont: 'md'
})

Vue.config.productionTip = false

const vuetify = new Vuetify({})
Vue.use(VuetifyDialog, {
  context: {
    vuetify
  }
})

Vue.filter('toLocal', (dateVal: Date) =>
  dayjs(dateVal).format(' DD.MM.YYYY hh:mm')
)
// new Date(dateVal).toLocaleTimeString())
Vue.filter('toLocalDate', (dateVal: Date) =>
  dayjs(dateVal).format(' DD.MM.YYYY')
)
Vue.filter('status2string', (status: number) => statusEnums[status])
Vue.filter('boolean2word', (bit: boolean) => (bit ? 'Ja' : 'Nein'))
Vue.filter('simpleDateTime', (date: string) => {
  if (!date) return ''
  if (/00:00:00$/.test(date) || /23:59:00$/.test(date)) {
    return `${date[8]}${date[9]}.${date[5]}${date[6]}.${date[0]}${date[1]}${date[2]}${date[3]}`
  } else {
    const splitDate = date.split('T')
    const day = splitDate[0]
      .split('-')
      .reverse()
      .join('.')
    const time = splitDate[1].substring(0, 5)
    return `${time} ${day}`
  }
})

new Vue({
  router,
  store,
  vuetify,
  render: h => h(App)
}).$mount('#app')

const statusEnums: string[] = ['offen', 'bestätigt', 'abgelehnt', 'verschoben']