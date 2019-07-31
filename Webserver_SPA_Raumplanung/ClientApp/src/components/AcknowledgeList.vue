<template>
<v-layout column>
 <v-data-table
    :headers="headers"
    :items="list"
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
    <template v-slot:item.DateTime="{ item }">{{item.DateTime}}</template>
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

  <v-divider/>
  <v-spacer/>
  <h3 v-if="isEmpty">Es liegen keine zu bearbeitenden Terminanfragen vor</h3>
  </v-layout>
</template>

<script lang="ts">
import dayjs from 'dayjs'
import { State, Action, Getter, Mutation } from 'vuex-class'
import { Component, Prop, Vue } from 'vue-property-decorator'
import { Names as Fnn} from '../store/Acknowledges/types'
import AllocationRequest from '../models/AllocationRequest'
const namespace = 'acknowledges'

@Component
export default class AcknowledgeList extends Vue {
  @State('tasks', { namespace })
  private list!: AllocationRequest[]
  @Getter('isEmpty', {namespace})
  private isEmpty!: boolean
  @Action(Fnn.a.loadTasks, { namespace })
  private loadTasks: any
  @Action(Fnn.a.updateTask, {namespace})
  private updateTask: any

  private search: string = ''
  private headers: object[] = [
      { text: 'Bearbeiten', value: 'action', sortable: false },
      { text: 'Bezeichnung', value: 'Title' },
      { text: 'Status' , value: 'Status' },
      { text: 'Raum', value: 'Description' },
      { text: 'Datum', value: 'DateTime', class: 'Date'}
    ]

  public mounted() {
      this.initialize()
  }

  public initialize() {
      this.loadTasks()
  }

  public acknowledge(task: AllocationRequest) {
    // change appointment status to accepted
    const editableTask = {...task}
    editableTask.Status = 'Bestätigt'
    this.updateTask(editableTask)
  }
  public reject(task: AllocationRequest) {
            // change appointment status to rejected
    const editableTask = {...task}
    editableTask.Status = 'Abgelehnt'
    this.updateTask(editableTask)
  }
  public async move(task: AllocationRequest) {
    // change appointment status to changed and change the date
    const editableTask = {...task}
    editableTask.Status = 'Verschoben'

    const res = await this.$dialog.prompt({
      text: 'Datum setzen',
      title: 'Bitte neues Datum und ggf. Uhrzeit eingeben. Format jjjj-mm-dd ss:mm oder jjjj-mm-dd',
      persistent: true
    })
    if (res) {
        if (!/(\d{4}-\d{2}-\d{2}\s\d{2}:\d{2})|(\d{4}-\d{2}-\d{2})/.test(res)) {
            this.$dialog.message.warning(`Das eigegebene Datum ${res} ist nicht gültig`, {position: 'top-center'})
            return false
        }

        const newDate = dayjs(res)
        const newDateTime = new Date(newDate.format())
        editableTask.DateTime = newDateTime
        this.updateTask(editableTask)

    }
  }
}
</script>

<style scoped lang="stylus">
.appointment-open-list:nth-of-type(2n)
  background-color lightgrey

</style>

