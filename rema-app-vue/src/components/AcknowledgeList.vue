<template>
  <v-layout column>
    <v-data-table
      v-if="hasItems"
      :headers="headers"
      :items="Requests"
      :search="search"
      :disable-pagination="true"
      hide-default-footer
      sort-by="calories"
      class="elevation-1"
    >
      <template v-slot:top>
        <v-toolbar flat color="white">
          <v-toolbar-title>Wartende Anfragen</v-toolbar-title>
          <v-divider class="mx-4" inset vertical></v-divider>
          <v-spacer></v-spacer>
          <v-text-field
            v-model="search"
            append-icon="search"
            label="Filter"
            single-line
            hide-details
          />
        </v-toolbar>
      </template>
      <template v-slot:item.From="{ item }">{{item.From | simpleDateTime}}</template>
      <template v-slot:item.To="{ item }">{{item.To | simpleDateTime}}</template>
      <template v-slot:item.DateTime="{ item }">{{item.DateTime | toLocal}}</template>
      <template v-slot:item.CreateDate="{ item }">{{item.CreateDate | toLocal}}</template>
      <template v-slot:item.action="{ item }">
        <v-btn @click="openDialog(item.Id)">Bearbeiten</v-btn>
      </template>
      <template v-slot:no-data>
        <v-btn color="primary" @click="refreshAllocations">Neu laden</v-btn>
      </template>
    </v-data-table>

    <h3 v-else>Es liegen keine zu bearbeitenden Terminanfragen vor</h3>
    <AcknowledgeView v-model="dialog" :viewAllocation="viewAllocation" />
  </v-layout>
</template>

<script lang="ts">
import dayjs from 'dayjs'
import { State, Action, Getter, Mutation } from 'vuex-class'
import { Component, Prop, Vue } from 'vue-property-decorator'
import { Names as Fnn } from '../store/Acknowledges/types'
import AllocationRequest from '../models/AllocationRequest'
import AllocationRequestView from '../models/AllocationRequestView'
import UserData, { ContactUser } from '../models/UserData'
import Allocations, { AllocationModel } from '../models/AllocationModel'
import AllocationsPurpose, {
  AllocationPurposeModel
} from '../models/AllocationpurposeModel'
import AcknowledgeView from './AcknowledgeView.vue'
import Gadgets from '../models/GadgetModel'
import Ressources from '../models/RessourceModel'
import Suppliers from '../models/SupplierModel'
const namespace = 'user'

@Component({
  components: { AcknowledgeView }
})
export default class AcknowledgeList extends Vue {
  @State('ContactUsers', { namespace })
  private ContactUsers!: ContactUser[];
  @Mutation('addContactUser', { namespace })
  private addContactUser: any;
  @Mutation('reserveContactUser', { namespace })
  private reserveContactUser: any;
  private dialog: boolean = false;
  private viewAllocation: AllocationRequestView = {} as AllocationRequestView;

  private search: string = '';
  private headers: object[] = [
    { text: 'Bearbeiten', value: 'action', sortable: false },
    { text: 'Bezeichnung', value: 'Title' },
    { text: 'Status', value: 'Status' },
    { text: 'Raum', value: 'Ressource' },
    { text: 'Von', value: 'From' },
    { text: 'Bis', value: 'To' },
    { text: 'Ansprechpartner', value: 'Contact' },
    { text: 'Anfragedatum', value: 'CreateDate' },
    { text: 'Letzte Veränderung', value: 'DateTime' }
  ];

  public get hasItems () {
    const allocations = Allocations.query()
      .withAll()
      .get()
    return allocations.length
  }
  public openDialog (id: number) {
    const viewA = Allocations.query()
      .withAll()
      .where('Id', id)
      .first() as any
    if (viewA == null) return
    this.dialog = true
    this.viewAllocation = {
      ...viewA,
      RessourceTitle: viewA.Ressource.Name,
      PurposeTitle: viewA.Purpose.Title,
      ContactTel: viewA.Purpose.ContactPhone,
      Description: viewA.Purpose.Description,
      Notices: viewA.Purpose.Notes
    }
  }
  public get UnAcknowledgedAllocations (): Allocations[] {
    return Allocations.query()
      .withAll()
      .get()
  }
  public get Requests () {
    if (!Allocations.all().length) return []
    this.fillContactUsers()
    return this.UnAcknowledgedAllocations.map((v: any) => ({
      Id: v.Id,
      Title: (v.Purpose || {}).Title,
      CreateDate: v.CreatedAt,
      // @ts-ignore
      Status: this.$options.filters.status2string(v.Status),
      Contact: (
        this.ContactUsers.find(
          (w: ContactUser) => w.Id === v.ReferencePerson
        ) || { Title: '' }
      ).Title,
      Ressource: (v.Ressource || {}).Name,
      From: v.From,
      To: v.To,
      DateTime: v.LastModified
    }))
  }
  public refreshAllocations () {
    Gadgets.api().get('gadgets')
    Suppliers.api().get('SupplierGroups')
    Ressources.api().get('ressources')
    Allocations.api().get('allocations')
    AllocationsPurpose.api().get('allocationpurposes')
  }
  public async fillContactUsers () {
    const referencePersons = this.UnAcknowledgedAllocations.map(
      (v: any) => v.ReferencePerson
    )
    const referencePersonsUnique = [...new Set(referencePersons)]

    const requestFunc = async (id: number) => {
      const tmpUser = this.ContactUsers.find((v: ContactUser) => v.Id === id)
      if (tmpUser) return tmpUser

      this.reserveContactUser(id)
      const response = await fetch(`/api/Users/Name/${id}`, {
        method: 'GET',
        mode: 'cors',
        cache: 'no-cache',
        credentials: 'same-origin',
        headers: { 'Content-Type': 'application/json' }
      })
      const newContact = await response.json()
      this.addContactUser(newContact)
    }
    referencePersonsUnique.forEach(requestFunc)
  }
}
</script>

<style scoped lang="stylus">
.appointment-open-list:nth-of-type(2n) {
  background-color: lightgrey;
}

.invalid-date {
  border: 1px solid red;
}
</style>
