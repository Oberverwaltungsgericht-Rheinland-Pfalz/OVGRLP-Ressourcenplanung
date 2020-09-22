<template>
  <v-dialog v-model="dialog" scrollable persistent max-width="1200px">
    <template v-slot:activator="{ on }">
      <v-btn v-show="permissionToEdit || requestsAllowed" color="success" dark v-on="on">
        <span v-if="permissionToEdit"><v-icon>add</v-icon>Termin anlegen</span>
        <span v-else-if="requestsAllowed"><v-icon>add</v-icon>Termin anfragen</span>
      </v-btn>
    </template>

    <AllocationForm v-if="dialog" v-on:close="dialog=false" :init-values="dataToInit"></AllocationForm>
  </v-dialog>
</template>

<script lang="ts">
import { Component, Prop, Vue, Watch } from 'vue-property-decorator'
import AllocationForm from './AllocationForm.vue'
import { State, Action, Getter, Mutation } from 'vuex-class'
import { InitAllocation } from '../../models/interfaces/index'

const emptyInit = (): InitAllocation => ({ RessourceId: 0, From: '', Day: '' })
const copyInit = (a: InitAllocation, b: InitAllocation) => { a.RessourceId = b.RessourceId; a.From = b.From; a.Day = b.Day }

@Component({
  components: { AllocationForm }
})
export default class NewFormModal extends Vue {
  @State('isRequestable', { namespace: 'user' })
  private requestsAllowed!: boolean

  public dataToInit: InitAllocation = emptyInit()
  public dialog: boolean = false;
  public get title () {
    return 'Neue Terminanfrage stellen / Termin eintragen'
  }
  public openNewForm (payload: InitAllocation) {
    copyInit(this.dataToInit, payload)
    this.dialog = true
  }
  @Watch('dialog')
  private watchDialog () {
    if (!this.dialog) copyInit(this.dataToInit, emptyInit())
  }
  created () {
    this.$root.$on('open-allocation-form', this.openNewForm)
  }
  destroyed () {
    this.$root.$off('open-allocation-form', this.openNewForm)
  }
}
</script>
