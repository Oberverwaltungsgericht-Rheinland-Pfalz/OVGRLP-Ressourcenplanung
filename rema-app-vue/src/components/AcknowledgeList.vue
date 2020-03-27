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
          <label>
            <input type="checkbox" v-model="hideOld" /> vergangene Termine ausblenden
          </label>
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
      <template v-slot:item.From="{ item }">{{item.From | toLocal}}</template>
      <template v-slot:item.To="{ item }">{{item.To | toLocal}}</template>
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
import { AllocationRequest, AllocationRequestView, UserData, ContactUser, AllocationModel } from '../models/interfaces'
import AcknowledgeView from './AcknowledgeView.vue'
import { Allocation, Gadget, Ressource, Supplier } from '../models'
import { Names } from '../store/user/types'

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
  @Action(Names.a.loadUsers, { namespace: 'user' })
  private loadUsers: any;

  private dialog: boolean = false;
  private viewAllocation: AllocationRequestView = {} as AllocationRequestView;
  private hideOld: boolean = true

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
    const allocations = Allocation.query()
      .withAll()
      .get()
    return allocations.length
  }
  public openDialog (id: number) {
    const viewA = Allocation.query()
      .withAll()
      .where('Id', id)
      .first() as any
    if (viewA == null) return
    this.dialog = true
    this.viewAllocation = {
      ...viewA,
      RessourceTitle: viewA.Ressource.Name,
      Title: viewA.Title,
      Notices: viewA.Notes
    }
  }
  public get UnAcknowledgedAllocations (): Allocation[] {
    return Allocation.query()
      .withAll()
      .get()
  }
  public get Requests () {
    if (!Allocation.all().length) return []
    this.fillContactUsers()
    return this.UnAcknowledgedAllocations
      .filter((a: any) => {
        return !this.hideOld || Date.parse(a.To) > Date.now()
      })
      .map((v: any) => ({
        Id: v.Id,
        Title: v.Title,
        CreateDate: v.CreatedAt,
        // @ts-ignore
        Status: this.$options.filters.status2string(v.Status),
        Contact: (
          this.ContactUsers.find(
            (w: ContactUser) => w.Id === v.ReferencePersonId
          ) || { Title: '' }
        ).Title,
        Ressource: (v.Ressource || {}).Name,
        From: v.From,
        To: v.To,
        DateTime: v.LastModified
      }))
  }
  public refreshAllocations () {
    Gadget.api().get('gadgets')
    Supplier.api().get('suppliergroups')
    Ressource.api().get('ressources')
    Allocation.api().get('allocations')
  }
  public async fillContactUsers () {
    const referencePersons = this.UnAcknowledgedAllocations.map((v: any) => v.ReferencePersonId)
    let hasAllUsernames = referencePersons.every((id: number) => this.ContactUsers.find((v: ContactUser) => v.Id === id))

    if (!hasAllUsernames) this.loadUsers(referencePersons)
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
