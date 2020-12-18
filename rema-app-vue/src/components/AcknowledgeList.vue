<template>
  <v-layout column>
    <h3 v-if="!$store.state.user.isRequestable" style="color:red;text-align: center;">Die Anfrageverwaltung ist deaktiviert</h3>
    <v-data-table
      v-if="hasItems"
      :headers="headers"
      :items="Requests"
      :search="search"
      hide-default-footer
      sort-by="calories"
      class="elevation-1"
    >
      <template v-slot:top>
        <v-toolbar flat color="white">
          <v-toolbar-title>Wartende Anfragen</v-toolbar-title>
          <v-divider class="mx-4" inset vertical></v-divider>

          <v-checkbox v-model="hideOld" label="Vergangene Termine" on-icon="visibility_off" off-icon="visibility_on" class="pad-top pad-right"></v-checkbox>
          <v-checkbox v-model="hideDone" label="Bestätigte Termine" on-icon="visibility_off" off-icon="visibility_on" class="pad-top"></v-checkbox>

          <v-spacer></v-spacer>
          <v-text-field
            v-model="search"
            append-icon="search"
            label="Namensfilter"
            single-line
            hide-details
          />
        </v-toolbar>
      </template>
      <template v-slot:[`item.From`]="{ item }">{{item.From | toLocal}}</template>
      <template v-slot:[`item.To`]="{ item }">{{item.To | toLocal}}</template>
      <template v-slot:[`item.DateTime`]="{ item }">{{item.DateTime | toLocal}}</template>
      <template v-slot:[`item.CreateDate`]="{ item }">{{item.CreateDate | toLocal}}</template>
      <template v-slot:[`item.action`]="{ item }">
        <v-btn @click="openDialog(item.Id)"><span>Status</span><v-icon class="pad-left">edit</v-icon></v-btn>
      </template>
      <template v-slot:no-data>
        <span>Keine Einträge zu bearbeiten</span>
      </template>
    </v-data-table>

    <h3 v-else>Es liegen keine zu bearbeitenden Terminanfragen vor</h3>
    <AcknowledgeView v-model="dialog" :viewAllocation="viewAllocation" />
  </v-layout>
</template>

<script lang="ts">
import { State, Action, Getter, Mutation } from 'vuex-class'
import { Component, Prop, Vue } from 'vue-property-decorator'
import { AllocationRequest, AllocationRequestView, UserData } from '../models/interfaces'
import AcknowledgeView from './AcknowledgeView.vue'
import { Allocation, Gadget, Ressource, Supplier } from '../models'
import { Names } from '../store/User/types'
import { refreshAllocations } from '../services/AllocationApiService'
import { refreshGadgets } from '../services/GadgetApiService'

const namespace = 'user'

@Component({
  components: { AcknowledgeView }
})
export default class AcknowledgeList extends Vue {
  @State('ContactUsers', { namespace })
  private ContactUsers!: WebApi.ContactUser[];
  @Action(Names.a.loadUsers, { namespace: 'user' })
  private loadUsers!: Function

  private dialog: boolean = false
  private viewAllocation: AllocationRequestView = {} as AllocationRequestView
  private hideOld: boolean = true
  private hideDone: boolean = true

  private search: string = ''
  private headers: object[] = [
    { text: 'Bearbeiten', value: 'action', sortable: false },
    { text: 'Bezeichnung', value: 'Title' },
    { text: 'Status', value: 'Status' },
    { text: 'Raum', value: 'Ressources' },
    { text: 'Von', value: 'From' },
    { text: 'Bis', value: 'To' },
    { text: 'Ansprechpartner', value: 'Contact' },
    { text: 'Anfragedatum', value: 'CreateDate' },
    { text: 'Letzte Veränderung', value: 'DateTime' }
  ]

  public get hasItems (): boolean {
    const allocations: Array<Allocation> = Allocation.query().get()
    return allocations.length > 0
  }
  public openDialog (id: number): void {
    const viewA: Allocation = Allocation.query()
      .withAll()
      .where('Id', id)
      .first() as any
    if (viewA == null) return
    this.dialog = true

    Object.assign(this.viewAllocation, viewA)
    this.viewAllocation.Title = viewA.Title
    this.viewAllocation.Notices = viewA.Notes
    this.viewAllocation.RessourceTitles = viewA.Ressources.map((v: Ressource) => v.Name).join(', ')
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
      .filter((a: Allocation) => {
        if (this.hideDone && (a.Status === 1 || a.Status === 3)) return false
        return Date.parse(a.To) > Date.now()
      })
      .map((v: Allocation) => ({
        Id: v.Id,
        Title: v.Title,
        CreateDate: v.CreatedAt,
        // @ts-ignore
        Status: this.$options.filters.status2string(v.Status),
        Contact: (
          this.ContactUsers.find(
            (w: WebApi.ContactUser) => w.Id === v.ReferencePersonId
          ) || { Title: '' }
        ).Title,
        Ressources: v.Ressources.map((v: Ressource) => v.Name).join(', '),
        From: v.From,
        To: v.To,
        DateTime: v.LastModified
      }))
  }
  public refreshAllocations (): void {
    refreshAllocations()
    Supplier.api().get('suppliergroups')
    Ressource.api().get('ressources')
    refreshAllocations()
  }
  public async fillContactUsers (): Promise<void> {
    let referencePersons = this.UnAcknowledgedAllocations.map((v: Allocation) => v.ReferencePersonId)
    referencePersons = [...new Set(referencePersons)] // remove duplicates
    referencePersons = referencePersons.filter((v: number) => v !== 0) // remove 0
    let hasAllUsernames = referencePersons.every((id: number) =>
      this.ContactUsers.find((v: WebApi.ContactUser) => v.Id === id))

    if (!hasAllUsernames) this.loadUsers(referencePersons)
  }
}
</script>

<style scoped lang="stylus">
.appointment-open-list:nth-of-type(2n)
  background-color lightgrey

.invalid-date
  border 1px solid red

.pad-left
  margin-left .5em
td.text-start:first-of-type
  white-space nowrap
  width 1%
.pad-top
  padding-top 2em
.pad-right
  padding-right 1em
</style>
