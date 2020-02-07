<template>
  <article>
    <h3>Querys</h3>
    <div>
      <h3>Gadgets</h3>
      <div>
        {{gadgetItems}}
        <p v-for="(item, idx) in gadgetItems" :key="'gadget'+idx">
          id: {{item.Id}} Title:
          <input type="text" v-model="item.Title" />
          <v-btn @click="updateGadget(item)">Save</v-btn>
          <v-btn @click="deleteGadget(item)">Delete</v-btn>
        </p>
      </div>
      <div>
        Title:
        <input type="text" v-model="gadgetTitle" />
        <v-btn @click="saveGadget">Create</v-btn>
      </div>
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
import Suppliers from '../models/SupplierModel'

@Component({})
export default class DbContentView extends Vue {
  private gadgetTitle: string = '';
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
    return Allocations.query()
      .withAll()
      .get()
  }
  private saveGadget () {
    const data = { Title: this.gadgetTitle }
    // @ts-ignore
    Gadgets.$create({ data })
  }
  private updateGadget (item: any) {
    // @ts-ignore
    Gadgets.$update({
      params: { Id: item.Id },
      data: { ...item, Title: item.Title }
    })
  }
  private deleteGadget (item: any) {
    // @ts-ignore
    Gadgets.$delete({
      params: { Id: item.Id }
    })
  }
}
</script>

<style lang="stylus" scoped>
input {
  border: 1px solid darkgrey;
  margin: 0 0.5em;
  padding: 0 0.5em;
}

button {
  margin: 0 0.5em;
}
</style>
