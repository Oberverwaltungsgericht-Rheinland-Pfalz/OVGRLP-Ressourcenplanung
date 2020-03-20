<template>
  <v-layout fill-height>
    <v-flex>
      <v-sheet height="64">
        <v-toolbar flat color="white">
          <v-btn outlined class="mr-4" @click="setToday">Heute</v-btn>
          <v-btn fab text small @click="prev">
            <v-icon small>arrow_back_ios</v-icon>
          </v-btn>
          <v-btn fab text small @click="next">
            <v-icon small>arrow_forward_ios</v-icon>
          </v-btn>
          <v-toolbar-title>{{ title }}</v-toolbar-title>
          <v-spacer></v-spacer>
          <v-select v-model="titleFilter" :items="possibleTitles" attach chips label="Räume" multiple single-line></v-select>
          <v-switch v-model="viewType" inset :label="typeToLabel[type]"></v-switch>
        </v-toolbar>
      </v-sheet>

      <v-sheet height="600">
        <v-calendar
          ref="calendar"
          v-model="focus"
          color="primary"
          :events="filteredItems"
          :event-color="getEventColor"
          :event-margin-bottom="3"
          :now="today"
          :type="type"
          :weekdays="[1, 2, 3, 4, 5, 6, 0]"
          :short-weekdays="false"
          @click:event="showEvent"
          @click:more="viewDay"
          @click:date="viewDay"
          @change="updateRange"
        ></v-calendar>
        <v-menu
          v-model="selectedOpen"
          :close-on-content-click="false"
          :activator="selectedElement"
          offset-x
        >
          <v-card color="grey lighten-4" min-width="350px" flat>
            <v-toolbar :color="selectedEvent.color" dark>
              <v-toolbar-title v-html="selectedEvent.name"></v-toolbar-title>
              <v-spacer></v-spacer>
              <v-btn @click="deleteAllocation" fab small outlined><v-icon small>delete</v-icon>
              </v-btn><span>&emsp;</span>
              <edit-form-modal v-if="selectedOpen" :eventId="selectedEvent.id" @updateview="selectedOpen = false">
                <v-icon small>edit</v-icon>
              </edit-form-modal>
            </v-toolbar>
            <v-card-text>
              <p>{{selectedEvent.schedule}}</p>
              <p><strong>Notizen: </strong>{{selectedEvent.Notes}}</p>
              <p><strong>Kontaktperson: </strong>{{selectedEvent.Contact || ReferenceUserName}}</p>
              <p><strong>Hilfsmittel: </strong><span v-html="selectedEvent.Gadgets"></span></p>
            </v-card-text>
            <v-card-actions>
              <v-btn text color="secondary" @click="selectedOpen = false">Schließen</v-btn>
            </v-card-actions>
          </v-card>
        </v-menu>
      </v-sheet>
    </v-flex>
  </v-layout>
</template>

<script lang="ts">
import { Component, Prop, Vue } from 'vue-property-decorator'
import { AllocationRequestView, GadgetModel, ContactUser } from '../models/interfaces'
import DateTimePicker from '@/components/DateTimePicker.vue'
import { State, Action, Getter, Mutation } from 'vuex-class'

import dayjs from 'dayjs'
import { Allocation, Gadget } from '../models'
import EditFormModal from './EditFormModal.vue'
import moment from 'moment'

@Component({
  components: { EditFormModal },
  filters: { ReferenceUserName (id:number) {
    // @ts-ignore
    let user = this.ContactUsers.find(id)
    return user ? user.Title : id
  } }
})
export default class Calendar extends Vue {
  @State('ContactUsers', { namespace: 'user' })
  private ContactUsers!: ContactUser[]
  @Mutation('addContactUser', { namespace: 'user' })
  private addContactUser: any;
  private viewType: boolean = true
  private today: string = dayjs().format('YYYY-MM-DD')
  private focus: string = dayjs().format('YYYY-MM-DD')
  private typeToLabel: object = {
    month: 'Monat',
    week: 'Woche',
    day: 'Tag',
    '4day': '4 Tage'
  }
  private start: any = null
  private end: any = null
  private selectedEvent: object = {}
  private selectedElement: object = {}
  private selectedOpen: Boolean = false
  private titleFilter: string[] = []

  public get permissionToEdit (): Boolean {
    return this.$store.state.user.role >= 10
  }
  public get type (): string {
    return this.viewType ? 'month' : 'day'
  }
  public set type (v: string) {
    this.viewType = Boolean(v)
  }
  public get filteredItems () {
    let isEmpty = !Allocation.all().length
    if (isEmpty) return []
    return this.itemsFormated
      .filter((v: any) => {
        if (this.titleFilter.length) return this.titleFilter.includes(v.RessourceName)
        return true
      })
  }
  public get itemsFormated () {
    let formatedItems = Allocation.query()
      .with('Ressource')
      .with('Gadget')
      .get()
    this.loadUsernames(formatedItems.map((e:any) => e.ReferencePersonId))
    return formatedItems.map(transfer2Calendar)
  }
  public async loadUsernames (ids: string[]) {
    const referencePersonsUnique = [...new Set(ids)]
    const response = await fetch(`/api/Users/Names/${referencePersonsUnique}`)
    const newContacts = await response.json()
    this.addContactUser(newContact)
  }
  public get possibleTitles () {
    return Allocation.query()
      .with('Ressource')
      .with('Gadget')
      .get().map((v:any) => v.Ressource).map((v:any) => v.Name)
  }
  public get title () {
    const { start, end } = this
    if (!start || !end) return dayjs().format('MMMM YYYY')

    const startMonth = this.monthFormatter(start)
    const endMonth = this.monthFormatter(end)
    const suffixMonth = startMonth === endMonth ? '' : endMonth

    const startYear = start.year
    const endYear = end.year
    const suffixYear = startYear === endYear ? '' : endYear

    const startDay = start.day + this.nth(start.day)
    const endDay = end.day + this.nth(end.day)

    switch (this.type) {
      case 'month':
        return `${startMonth} ${startYear}`
      case 'week':
      case '4day':
        return `${startMonth} ${startDay} ${startYear} - ${suffixMonth} ${endDay} ${suffixYear}`
      case 'day':
        return `${startMonth} ${startDay} ${startYear}`
    }
    return ''
  }

  public get monthFormatter () {
    // @ts-ignore
    return this.$refs.calendar.getFormatter({
      timeZone: 'UTC',
      month: 'long'
    })
  }

  public async deleteAllocation () {
    const { id, name } = this.selectedEvent as any
    const confirmation = await this.$dialog.confirm({
      text: `Möchten sie dem Termin ${name} wirklich löschen?`,
      title: 'Löschen bestätigen',
      persistent: true,
      actions: [
        {
          text: 'Nein',
          color: 'blue',
          key: false
        },
        {
          text: 'Löschen',
          color: 'red',
          key: true
        }
      ]
    })

    if (confirmation !== true) return

    const responseDeleteAllocation = await Allocation.api().delete(
      `allocations/${id}`,
      { delete: id }
    )
  }

  public viewDay ({ date }: any) {
    this.focus = date
    this.type = 'day'
  }
  public getEventColor (event: any) {
    return event.color
  }
  public setToday () {
    this.focus = this.today
  }
  public prev () {
    // @ts-ignore
    this.$refs.calendar.prev()
  }
  public next () {
    // @ts-ignore
    this.$refs.calendar.next()
  }
  public showEvent ({ nativeEvent, event }: any) {
    const open = () => {
      this.selectedEvent = event
      this.selectedElement = nativeEvent.target
      setTimeout(() => (this.selectedOpen = true), 10)
    }

    if (this.selectedOpen) {
      this.selectedOpen = false
      setTimeout(open, 10)
    } else {
      open()
    }

    nativeEvent.stopPropagation()
  }
  public updateRange ({ start, end }: any) {
    // You could load events from an outside source (like database)
    // now that we have the start and end dates on the calendar
    this.start = start
    this.end = end
  }
  public nth (d:number) {
    return d > 3 && d < 21
      ? 'th'
      : ['th', 'st', 'nd', 'rd', 'th', 'th', 'th', 'th', 'th', 'th'][d % 10]
  }
}
function transfer2Calendar (v: any) {
  let rVal:any = {}
  if (v.IsAllDay) {
    rVal.start = v.From.substring(0, 10)
    rVal.schedule = `ganztägig`
    rVal.name = (v.Ressource || {}).Name + ' in ' + v.Title
  } else {
    rVal.start = v.From.substring(0, 16).replace('T', ' ') // dayjs(v.From).format('YYYY-MM-DD hh:mm'),
    rVal.end = v.To.substring(0, 16).replace('T', ' ') // dayjs(v.To).format('YYYY-MM-DD hh:mm'),
    rVal.schedule = `
      Von ${moment(v.From).format('LT')} 
      bis ${moment(v.To).format('LT')}`
    rVal.name = `${(v.Ressource || {}).Name} ab ${moment(v.From).format('LT')} in ${v.Title}`
  }

  rVal.color = 'success'
  rVal.id = v.Id
  rVal.Notes = v.Notes
  rVal.Contact = v.ReferencePersonId
  rVal.Original = v
  rVal.RessourceName = (v.Ressource || {}).Name

  rVal.Gadgets = '<br>'
  if (Gadget.all().length) {
    v.GadgetsIds.map((e:any) => (Gadget.find(e) as any).Title)
      .forEach((e: any) => { rVal.Gadgets += e + '<br> ' })
  }
  rVal.Gadgets.slice(0, -1)

  return rVal
}
</script>
