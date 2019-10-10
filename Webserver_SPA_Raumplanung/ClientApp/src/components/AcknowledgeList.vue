<template>
<v-layout column>
 <v-data-table v-if="hasItems"
    :headers="headers"
    :items="Requests"
    :search="search"
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
    <template v-slot:item.DateTime="{ item }">{{item.DateTime | toLocal}}</template>
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
import Allocations, { AllocationModel } from '../models/AllocationModel'
const namespace = 'acknowledges'

@Component
export default class AcknowledgeList extends Vue {
  @State('tasks', { namespace })
  private list!: AllocationRequest[]
  @Getter('isEmpty', { namespace })
  private isEmpty!: boolean
  @Action(Fnn.a.loadTasks, { namespace })
  private loadTasks: any
  @Action(Fnn.a.updateTask, { namespace })
  private updateTask: any

  private search: string = ''
  private headers: object[] = [
      { text: 'Bearbeiten', value: 'action', sortable: false },
      { text: 'Bezeichnung', value: 'Title' },
      { text: 'Status' , value: 'Status' },
      { text: 'Raum', value: 'Ressource' },
      { text: 'Datum', value: 'DateTime' }
  ]

  public mounted () {
    this.initialize()
  }
  public get hasItems () {
    const allocations = Allocations.query().withAll().get()
    return allocations.length
  }
  public get Requests () {
    if (!Allocations.all().length) return []
    return Allocations.query().withAll().get().map((v: any) => ({
      Id: v.Id,
      Title: (v.Purpose || {}).Title,
      Status: this.$options.filters.status2string(v.Status),
      Ressource: (v.Ressource || {}).Name,
      DateTime: v.LastModified}))
  }
  public initialize () {
    this.loadTasks()
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
    const editableTask = { ...task }
    editableTask.Status = 'Verschoben'

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
      this.updateTask(editableTask)

    }
  }
  public async saveStatus (task: AllocationModel, status: number) {
    const response = await fetch(`http://localhost:8080/api/Allocations/Status/${task.Id}?status=${status}`, {
      method: 'PUT', // *GET, POST, PUT, DELETE, etc.
      mode: 'cors', // no-cors, cors, *same-origin
      cache: 'no-cache', // *default, no-cache, reload, force-cache, only-if-cached
      credentials: 'same-origin', // include, *same-origin, omit
      headers: {
        'Content-Type': 'application/json'
      }
    })

    this.$store.dispatch('entities/update', {
      entity: 'allocations',
      where: task.Id,
      data: { Status: status, LastModified: new Date() }
    })
  }
}
</script>

<style scoped lang="stylus">
.appointment-open-list:nth-of-type(2n)
  background-color lightgrey

</style>

