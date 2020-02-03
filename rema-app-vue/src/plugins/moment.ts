import Vue from 'vue'

const moment = require('moment')
require('moment/locale/de')

moment.locale('de')

Vue.use(require('vue-moment'), {
  moment
})
