<template>
<v-layout column>
  {{Requestsx}}
 <v-data-table v-if="hasItems"
    :headers="headers"
    :items="Requests"
    :search="search"
    :disable-pagination="true" hide-default-footer
    sort-by="calories"
    class="elevation-1"
  >
    <template v-slot:top>
      <v-toolbar flat color="white">
        <v-toolbar-title>Wartende Anfragen</v-toolbar-title>
        <v-divider class="mx-4" inset vertical></v-divider>
        <v-spacer></v-spacer>
        <v-text-field v-model="search" append-icon="search" label="Filter" single-line hide-details/>
      </v-toolbar>
    </template>
    <template v-slot:item.From="{ item }">{{item.From | simpleDateTime}}</template>
    <template v-slot:item.To="{ item }">{{item.To | simpleDateTime}}</template>
    <template v-slot:item.DateTime="{ item }">{{item.DateTime | toLocal}}</template>
    <template v-slot:item.CreateDate="{ item }">{{item.CreateDate | toLocal}}</template>
    <template v-slot:item.action="{ item }">
        <v-menu bottom offset-y eager>
            <template v-slot:activator="{ on }">
                <v-btn v-on="on">Bearbeiten</v-btn>
            </template>
            <v-list>
                <v-list-item @click="acknowledge(item)">
                    <v-list-item-title>
                        <v-icon v-html="'done'"></v-icon> Bestätigen
                    </v-list-item-title>
                </v-list-item>
                <v-list-item @click="reject(item)">
                    <v-list-item-title>
                        <v-icon v-html="'close'"></v-icon> Ablehnen
                    </v-list-item-title>
                </v-list-item>
                <v-list-item @click="move(item)">
                    <v-list-item-title>
                        <v-icon v-html="'create'"></v-icon>  Verschieben
                    </v-list-item-title>
                </v-list-item>
            </v-list>
        </v-menu>
    </template>
    <template v-slot:no-data>
      <v-btn color="primary" @click="initialize">Neu laden</v-btn>
    </template>
  </v-data-table>

  <h3 v-else>Es liegen keine zu bearbeitenden Terminanfragen vor</h3>
  </v-layout>
</template>

<script lang="ts">
import dayjs from 'dayjs'
import { State, Action, Getter, Mutation } from 'vuex-class'
import { Component, Prop, Vue } from 'vue-property-decorator'
import { Names as Fnn } from '../store/Acknowledges/types'
import AllocationRequest from '../models/AllocationRequest'
import UserData, { ContactUser } from '../models/UserData'
import Allocations, { AllocationModel } from '../models/AllocationModel'
import AllocationsPurpose, { AllocationPurposeModel } from '../models/AllocationpurposeModel'
const namespace = 'acknowledges'

@Component({
  filters: {
    contactTitle (user: ContactUser): string {
      if (user && 'Title' in user && user.Title) return user.Title
      return ''
    },
    simpleDateTime (date: string): string {
      if (/00:00:00$/.test(date)) {
        return `${date[8]}${date[9]}.${date[5]}${date[6]}.${date[0]}${date[1]}${date[2]}${date[3]}`
      } else {
        const splitDate = date.split('T')
        const day = splitDate[0].split('-').reverse().join('.')
        const time = splitDate[1].substring(0, 5)
        return `${time} ${day}`
      }
    }
  }
})
export default class AcknowledgeList extends Vue {
  @State('tasks', { namespace })
  private list!: AllocationRequest[]
  @State('ContactUsers', { namespace: 'user' })
  private ContactUsers!: ContactUser[]
  @Getter('isEmpty', { namespace })
  private isEmpty!: boolean
  @Mutation('addContactUser', { namespace: 'user' })
  private addContactUser: any
  @Mutation('reserveContactUser', { namespace: 'user' })
  private reserveContactUser: any

  private search: string = ''
  private headers: object[] = [
      { text: 'Bearbeiten', value: 'action', sortable: false },
      { text: 'Bezeichnung', value: 'Title' },
      { text: 'Status' , value: 'Status' },
      { text: 'Raum', value: 'Ressource' },
      { text: 'Von', value: 'From' },
      { text: 'Bis', value: 'To' },
      { text: 'Ansprechpartner', value: 'Contact' },
      { text: 'Anfragedatum', value: 'CreateDate' },
      { text: 'Letzte Veränderung', value: 'DateTime' }
  ]

  public get hasItems () {
    const allocations = Allocations.query().withAll().get()
    return allocations.length
  }
  public get Requestsx () {
    if (!Allocations.all().length) return []
    return AllocationsPurpose.query().withAll().get()
  }
  public get UnAcknowledgedAllocations (): Allocations[] {
    return Allocations.query().withAll().where('Status', (v: AllocationModel) => v.Status !== 1).get()
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
      Contact: (this.ContactUsers.find((w: ContactUser) => w.Id === v.ReferencePerson) || { Title: '' }).Title,
      Ressource: (v.Ressource || {}).Name,
      From: v.From,
      To: v.To,
      DateTime: v.LastModified}))
  }
  public async fillContactUsers () {
    const referencePersons = this.UnAcknowledgedAllocations.map((v: any) => v.ReferencePerson)
    const referencePersonsUnique = [...new Set(referencePersons)]

    const requestFunc = async (id: number) => {
      const tmpUser = this.ContactUsers.find((v: ContactUser) => v.Id === id)
      if (tmpUser) return tmpUser

      this.reserveContactUser(id)
      const response = await fetch(`/api/Users/Name/${id}`, {
        method: 'GET', mode: 'cors', cache: 'no-cache', credentials: 'same-origin',
        headers: { 'Content-Type': 'application/json' }
      })
      const newContact = await response.json()
      this.addContactUser(newContact)
    }
    referencePersonsUnique.forEach(requestFunc)
  }

  public async acknowledge (task: AllocationModel) {
    this.saveStatus(task, 1)
  }
  public reject (task: AllocationModel) {
    // change appointment status to rejected
    this.saveStatus(task, 2)
  }
  public async move (task: AllocationModel) {
    this.saveStatus(task, 3)

    // change appointment status to changed and change the date
    const editableTask = { ...task, DateTime: new Date() }
    editableTask.Status = 3

    const res = await this.$dialog.prompt({
      text: 'Datum setzen',
      title: 'Bitte neues Datum und ggf. Uhrzeit eingeben. Format jjjj-mm-dd ss:mm oder jjjj-mm-dd',
      persistent: true
    })
    if (res) {
      if (!/(\d{4}-\d{2}-\d{2}\s\d{2}:\d{2})|(\d{4}-\d{2}-\d{2})/.test(res)) {
        await this.$dialog.message.warning(`Das eigegebene Datum ${res} ist nicht gültig`, { position: 'top-center' })
        return false
      }

      const newDate = dayjs(res)
      const newDateTime = new Date(newDate.format())
      editableTask.DateTime = newDateTime
      //this.updateTask(editableTask)

    }
  }
  public async saveStatus (task: AllocationModel, status: number) {
    const editedRequest = { Id: task.Id, status, From: task.From, To: task.To }
    const response = await fetch(`/api/Allocations/EditRequest`, {
      method: 'PUT', // *GET, POST, PUT, DELETE, etc.
      mode: 'cors', // no-cors, cors, *same-origin
      cache: 'no-cache', // *default, no-cache, reload, force-cache, only-if-cached
      credentials: 'same-origin', // include, *same-origin, omit
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify(editedRequest)
    })

    this.$store.dispatch('entities/update', {
      entity: 'allocations',
      where: task.Id,
      data: { Status: status, LastModified: new Date(), From: task.From, To: task.To }
    })
  }
}
</script>

<style scoped lang="stylus">
.appointment-open-list:nth-of-type(2n)
  background-color lightgrey

</style>
