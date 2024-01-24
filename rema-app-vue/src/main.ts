import Vue from 'vue'
import App from './App.vue'
import router from './router'
import store from './store/index'
//import Vuetify from 'vuetify/lib'
import '@mdi/font/css/materialdesignicons.css'
import de from 'vuetify/src/locale/de'
import 'moment/locale/de'
import vue_moment from 'vue-moment'
import Vuetify from 'vuetify'
import 'vuetify/dist/vuetify.min.css'


import 'material-design-icons-iconfont/dist/material-design-icons.css'
import moment from 'moment'

Vue.config.productionTip = false

Vue.filter('toLocal', (dateVal: Date) =>
  moment(dateVal).format('DD.MM.YYYY') + ' ' + moment(dateVal).format('LT')
)
Vue.filter('onlyDay', (dateVal: Date) => moment(dateVal).format('DD.MM.'))
// new Date(dateVal).toLocaleTimeString())
Vue.filter('toLocalDate', (dateVal: Date) =>
  moment(dateVal).format(' DD.MM.YYYY')
)
Vue.filter('status2string', (status: number) => statusEnums[status])
Vue.filter('2digits', (num: number) => `${num}`.length > 1 ? num : '0' + num)
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
Vue.mixin({
  computed: {
    permissionToEdit (): Boolean {
      // @ts-ignore
      return this.$store.state.user.role >= 10
    }
  }
})

Vue.use(Vuetify)


moment.locale('de')
Vue.use(vue_moment, {moment})

new Vue({
}).$mount('#app')


new Vue({
  router,
  store,
  vuetify: new Vuetify({
    icon: {
      iconfont: 'md'
    },
    lang: {
      locales: { de },
      current: 'de'
    }
  }),
  render: h => h(App)
}).$mount('#app')

const statusEnums: string[] = ['offen', 'bestätigt', 'abgelehnt', 'verschoben']
