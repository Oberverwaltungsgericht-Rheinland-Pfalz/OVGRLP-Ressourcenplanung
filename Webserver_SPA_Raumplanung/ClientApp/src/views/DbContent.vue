<template>
  <article>
      <h3>Querys</h3>
    <div>
      <h3>Gadgets</h3>
      <div>{{gadgetItems}}
          <p v-for="(item, idx) in gadgetItems" :key="'gadget'+idx">
              id: {{item.id}} Title: <input type="text" v-model="item.title"/> 
              <v-btn @click="updateGadget(item)">Save</v-btn>
              <v-btn @click="deleteGadget(item)">Delete</v-btn>
          </p>
      </div>
      <div>Title: <input type="text" v-model="gadgetTitle"/> <v-btn @click="saveGadget">Create</v-btn></div>
    </div>
    <div>
      <h3>Ressources</h3>
      <div>{{ressourceItems}}</div>
    </div>
    <div>
      <h3>Allocations</h3>
      <div>{{allocationItems}}</div>
    </div>
    <div>
      <h3>AllocationPurposes</h3>
      <div>{{allocationPurposeItems}}</div>
    </div>
    <div>
      <h3>Suppliers</h3>
      <div>{{supplierItems}}</div>
    </div>
  </article>
</template>

<script lang="ts">
import { Component, Vue } from 'vue-property-decorator'
import Gadgets from '../models/GadgetModel'
import Ressources from '../models/RessourceModel'
import Allocations from '../models/AllocationModel'
import AllocationPurposes from '../models/AllocationpurposeModel'
import Suppliers from '../models/SupplierModel'

@Component({
})
export default class DbContentView extends Vue {
  private gadgetTitle: string = ''
  public mounted () {
    // @ts-ignore
    Ressources.$fetch()// .then((e: any) => Ressources.insert(e[0]))
    // @ts-ignore
    Gadgets.$get()// .then((e: any) => Gadgets.insert(e[0]))
    // @ts-ignore
    Allocations.$fetch()// .then((e: any) => Allocations.insert(e[0]))
    // @ts-ignore
    AllocationPurposes.$get()
    // @ts-ignore
    Suppliers.$get()
  }
  private get supplierItems () {
    return Suppliers.all()
  }
  private get gadgetItems () {
    return Gadgets.all()
  }
  private get ressourceItems () {
    return Ressources.all()
  }
  private get allocationItems () {
    return Allocations.all()
  }
  private get allocationPurposeItems () {
    return AllocationPurposes.all()
  }
  private saveGadget () {
    const data = { title: this.gadgetTitle }
    // @ts-ignore
    Gadgets.$create({ data })
  }
  private updateGadget (item: any) {
    // @ts-ignore
    Gadgets.$update({
      params: { id: item.id },
      data: { ...item, title: item.title }
    })
  }
  private deleteGadget (item: any) {
    // @ts-ignore
    Gadgets.$delete({
      params: { id: item.id }
    })
  }
}
</script>

<style lang="stylus" scoped>
input
  border 1px solid darkgrey
  margin 0 .5em
  padding 0 .5em
button
  margin 0 .5em
</style>