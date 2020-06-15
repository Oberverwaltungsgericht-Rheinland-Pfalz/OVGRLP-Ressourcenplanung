<template>
    <div id="printview" v-show="show" class="print">
      <div >
        <v-container>
           <v-row>
              <v-col cols="6" v-for="(value, name) in input" :key="'print'+name">
                <strong>{{name}}:</strong>
                <p><span>{{value}}</span></p>
              </v-col>
            </v-row>
        </v-container>
      </div>
    <v-dialog v-model="show" scrollable>
      <v-card>
        <v-card-title>
          <span class="headline">Termin</span>
          <v-btn @click="show=false" color="red darken-1" rounded outlined class="alignRight"><v-icon>close</v-icon></v-btn>
        </v-card-title>
        <v-card-text>
          <table id="killer-modal">
            <tbody>
              <tr v-for="(value, name) in input" :key="'print'+name">
                <td>{{name}}:</td><td>{{value}}</td>
              </tr>
            </tbody>
          </table>
          <v-container v-show="false">
            <v-row>
              <v-col cols="6" >
                <strong>{{name}}:</strong>
                <p><span>{{value}}</span></p>
              </v-col>
            </v-row>
          </v-container>
        </v-card-text>
      </v-card>
    </v-dialog>
    </div>
</template>

<script lang="ts">
import { Component, Prop, Vue } from 'vue-property-decorator'
import { Allocation } from '../models'
import { deleteAllocation } from '../services/AllocationApiService'
import print from 'print-js'

@Component
export default class PrintViewAllocation extends Vue {
  // @Prop({ type: Object, default: {} }) private input!: object
  public input : object = { ja: 'lkj' }
  public show:boolean = false
  public async created () {
    this.$root.$on('print-object', this.setInput)
  }
  public setInput (newInput: any) {
    this.input = Object.assign({}, newInput)
    this.show = true
    let keys = Object.keys(newInput)
    // let a = print({ printable: 'killer-modal', type: 'html', onPrintDialogClose: () => { this.show = false } })
    print('api/debug/h')
  }
}
</script>

<style lang="stylus" scoped>
@media only print {
    body {
        display none
    }
}
</style>
