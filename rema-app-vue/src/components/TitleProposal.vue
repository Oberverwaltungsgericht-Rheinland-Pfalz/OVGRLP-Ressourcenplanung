<template>
  <v-autocomplete
    v-model="internalModel"
    :items="titleEntries"
    :search-input.sync="search"
    color="black"
    label="Titel*"
    :error="error"
    placeholder="Bitte tippen Sie einen Titel für den Termin ein."
  ></v-autocomplete>
</template>

<script lang="ts">
import { Component, Prop, Vue, Watch } from 'vue-property-decorator'
import { Gadget, Ressource, Supplier, Allocation } from '../models'
import DropDownTimePicker from '@/components/DropdownTimePicker.vue'
import { RessourceModel, AllocationModel, AdUsers, HintsForSuppliers } from '../models/interfaces'

@Component({})
export default class TitleProposal extends Vue {
  @Prop(String) private readonly value!: string
  @Prop(Boolean) private readonly error!: boolean

  public internalModel: string = ''
  public search: string = ''
  public get titleEntries () : string[] {
    var rList = Allocation.query()
    //      .where('title', (s: string) => s && s.startsWith(this.search))
      .get().map((v:any) => v.Title)
    return rList
  }

  public mounted () {
    this.internalModel = this.value
  }
  @Watch('value')
  private watchValue (newVal: string) {
    this.internalModel = newVal
  }

  @Watch('internalModel')
  private watchInternalModel (newVal: string, oldValue: string) {
    if (newVal !== oldValue) { this.$emit('input', newVal) }
  }
}
</script>
