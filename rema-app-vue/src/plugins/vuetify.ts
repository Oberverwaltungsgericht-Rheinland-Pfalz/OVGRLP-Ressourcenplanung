import Vue from 'vue'
import Vuetify from 'vuetify/lib'
import de from 'vuetify/src/locale/de'

import 'material-design-icons-iconfont/dist/material-design-icons.css'

Vue.use(Vuetify)

export default new Vuetify({
  icon: {
    iconfont: 'md'
  },
  lang: {
    locales: { de },
    current: 'de'
  }
})
