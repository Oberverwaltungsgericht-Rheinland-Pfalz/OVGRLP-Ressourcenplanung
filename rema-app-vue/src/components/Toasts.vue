<template>
<div>
  <v-snackbar v-for="(toast, idx) in toasts" :key="'toast'+idx"
      v-model="toast.snackbar"
      :multi-line="toast.multiline"
      :timeout="toast.timeout"
      :color="toast.color"
      :centered="toast.center"
      bottom
    >
      {{ toast.text }}
    </v-snackbar>
</div>
</template>

<script lang="ts">
import { Component, Prop, Vue, Watch } from 'vue-property-decorator'
import { ShowToast } from '../models/interfaces'

@Component
export default class Toasts extends Vue {
  toasts: Array<ToastSettings> = []

  private openToast (payload: ShowToast) {
    let toast = new ToastSettings()
    toast.text = payload.text
    toast.color = payload.color || 'primary'
    toast.center = !!payload.center
    toast.multiline = true
    toast.timeout = payload.timeout || 3000

    toast.snackbar = true
    this.toasts.push(toast)
  }

  private created () {
    this.$root.$on('notify-user', this.openToast)
  }
  private destroyed () {
    this.$root.$off('notify-user', this.openToast)
  }
}

class ToastSettings {
  public timeout: number = 3000
  public text: string = ''
  public multiline: boolean = true
  public snackbar: boolean = false
  public color: string = 'primary'
  public center: boolean = false
}
</script>
