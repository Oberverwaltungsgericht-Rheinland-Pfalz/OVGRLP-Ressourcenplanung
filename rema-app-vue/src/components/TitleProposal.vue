<template>
  <v-combobox
    v-model="internalModel"
    placeholder="Bitte tippen Sie einen Titel für den Termin ein."
    :items="titleEntries"
    :search-input.sync="search"
    label="Titel*"
    color="black"
    :error="error"
    :disabled="readonly"
  ></v-combobox>
</template>

<script lang="ts">
import { Component, Prop, Vue, Watch } from 'vue-property-decorator'
import { Gadget, Ressource, Supplier, Allocation } from '../models'
import DropDownTimePicker from '@/components/DropdownTimePicker.vue'

@Component({})
export default class TitleProposal extends Vue {
  @Prop(String) private readonly value!: string
  @Prop(Boolean) private readonly error!: boolean
  @Prop(Boolean) private readonly readonly!: boolean

  public internalModel: string = ''
  public searchInternal: string = ''
  public get titleEntries () : string[] {
    var rList: Array<string> = Allocation.query()
    //      .where('title', (s: string) => s && s.startsWith(this.search))
      .get().map((v:Allocation) => v.Title)
    return rList
  }

  public get search () {
    return this.searchInternal
  }
  public set search (s: string) {
    this.searchInternal = s
    this.watchInternalModel(s, this.internalModel)
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
