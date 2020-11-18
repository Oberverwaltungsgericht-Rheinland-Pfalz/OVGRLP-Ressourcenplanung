<template>
  <v-snackbar
      v-model="snackbar"
      :multi-line="multiline"
      :timeout="timeout"
      :color="color"
      :centered="center"
      bottom
    >
      {{ text }}
    </v-snackbar>
</template>

<script lang="ts">
import { Component, Prop, Vue, Watch } from 'vue-property-decorator'
import { ShowToast } from '../models/interfaces'

@Component
export default class Toasts extends Vue {
  private timeout: number = 3000
  private text: string = ''
  private multiline: boolean = true
  private snackbar: boolean = false
  private color: string = 'primary'
  private center: boolean = false

  private openToast (payload: ShowToast) {
    this.text = payload.text
    this.color = payload.color || 'primary'
    this.center = !!payload.center
    if (payload.timeout) {
      this.timeout = payload.timeout
      setTimeout(() => { this.timeout = 3000 }, payload.timeout * 1.1)
    }

    this.snackbar = true
  }
  @Watch('snackbar')
  public watchSnackbar (newV: boolean) {
    if (newV) return

    this.timeout = 3000
    this.text = ''
    this.multiline = true
    this.color = 'primary'
    this.center = false
  }

  private created () {
    this.$root.$on('notify-user', this.openToast)
  }
  private destroyed () {
    this.$root.$off('notify-user', this.openToast)
  }
}
</script>
