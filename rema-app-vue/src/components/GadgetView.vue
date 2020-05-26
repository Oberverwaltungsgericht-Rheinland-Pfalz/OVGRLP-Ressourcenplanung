<template>
  <v-layout column>
    <v-data-table
      :headers="headers"
      :items="items"
      :disable-pagination="true"
      hide-default-footer
      height="75vh"
      fixed-header
      sort-by="SuppliedBy"
    >
      <template v-slot:item.SuppliedBy="{ item }">{{
        item.SuppliedBy | supplierName
      }}</template>
    </v-data-table>
  </v-layout>
  </template>

<script lang="ts">
import { SupplierGroupModel, GadgetModel, RessourceModel } from '../models/interfaces'
import { submitGadget, editGadget, deleteGadget } from '../services/GadgetApiService'
import { Component, Prop, Vue, Watch } from 'vue-property-decorator'
import { Ressource, Gadget, Supplier } from '../models'

@Component({
  filters: {
    supplierName (id: number) {
      return ((Supplier.find(id) as any) || { Title: '' }).Title
    }
  }
})
export default class GadgetView extends Vue {
  private headers: object[] = [
    { text: 'Bezeichnung', value: 'Title' },
    { text: 'Unterstützergruppe', value: 'SuppliedBy' }
  ]

  private get supplierItems () {
    return Supplier.all()
  }
  private get items () {
    const suppNames: any = {}
    Supplier.all().forEach((e: any) => (suppNames[e.Id] = e.Title))
    return Gadget.all().map((v: any) => ({
      ...v,
      supplierTitle: suppNames[v.SuppliedBy]
    }))
  }
}
</script>
