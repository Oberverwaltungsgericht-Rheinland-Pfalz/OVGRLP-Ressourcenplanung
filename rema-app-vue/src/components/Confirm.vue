<template>
    <v-dialog v-model="dialogOpen" max-width="290">
      <v-card>
        <v-card-title class="headline">{{title}}</v-card-title>

        <v-card-text>{{content}}</v-card-text>

        <v-card-actions>
          <v-btn color="red darken-1" text @click="dialogOpen = false">Nein</v-btn>
          <v-spacer></v-spacer>
          <v-btn color="green darken-1" text @click="callCallback">Ja</v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>
</template>

<script lang="ts">
import { Component, Prop, Vue, Watch } from 'vue-property-decorator'
import { ConfirmData } from '../models/interfaces'

@Component
export default class Confirm extends Vue {
  private title: string = ''
  private content: string = ''
  private dialogOpen: boolean = false
  private id: number = 0
  private callback!: Function

  private create (payload: ConfirmData) {
    this.title = payload.title
    this.content = payload.content
    this.callback = payload.callback
    this.id = payload.id

    this.dialogOpen = true
  }
  private callCallback () {
    this.callback(this.id)
    this.dialogOpen = false
  }
  @Watch('dialogOpen')
  public watchDialog (newV: boolean) {
    if (newV) return

    this.title = ''
    this.content = ''
    this.dialogOpen = false
    this.id = 0
    this.callback = () => {}
  }

  private created () {
    this.$root.$on('user-confirm', this.create)
  }
  private destroyed () {
    this.$root.$off('user-confirm', this.create)
  }
}
</script>
