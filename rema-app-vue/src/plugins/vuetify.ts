import Vue from 'vue'
import Vuetify from 'vuetify/lib'
import de from 'vuetify/src/locale/de'
import VuetifyDialog from 'vuetify-dialog'

import 'material-design-icons-iconfont/dist/material-design-icons.css'
import 'vuetify-dialog/dist/vuetify-dialog.css'

Vue.use(Vuetify)
Vue.use(VuetifyDialog, {
  context: {
    Vuetify
  }
})

export default new Vuetify({
  icon: {
    iconfont: 'md'
  },
  lang: {
    locales: { de },
    current: 'de'
  }
})
