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

          <v-dialog v-model="showFilterModal" width="800" scrollable >
            <template v-slot:activator="{ on }">
              <v-btn v-on="on" color="primary">Suchkriterien ({{titleFilter.length}})</v-btn>
            </template>
            <v-card>
              <v-card-title>Anzeige einschränken auf:
                <v-spacer/>
                <v-btn @click="resetFilter" :style="{visibility: titleFilter.length ? 'visible':'hidden'}" color="warning" right class="ma-2">Zurücksetzen</v-btn>
                <v-btn @click="showFilterModal=false" class="ma-2" right color="success">Ok</v-btn>
              </v-card-title>
              <v-card-text id="pad-bot-twenty">
                <v-select v-model="titleFilter" :items="possibleTitles" attach chips label="Räume" multiple single-line></v-select>
              </v-card-text>
            </v-card>
          </v-dialog>
          <v-spacer/>
          <label id="showWe">
            <v-icon v-if="showWE">visibility</v-icon>
            <v-icon v-else>visibility_off</v-icon>&ensp;WE<input hidden v-model="showWE" type="checkbox"/>
            </label>
          <v-radio-group v-model="currentview" row>
            <v-radio v-for="n in types" :key="'ansicht'+n" :label="typeName(n)" :value="n"/>
          </v-radio-group>
        </v-toolbar>
      </v-sheet>

      <v-sheet id="calendar-sheet" height="600">
        <v-calendar
          ref="calendar"
          :event-name="formatEventText"
          v-model="focus"
          color="primary"
          :events="filteredItems"
          :event-color="getEventColor"
          :event-margin-bottom="3"
          :now="today"
          :type="currentview"
          :weekdays="weekdays"
          :short-weekdays="false"
          @click:event="showEvent"
          @click:more="viewDay"
          @click:date="viewDay"
          @change="updateRange"
        ></v-calendar>
        <v-menu v-if="selectedOpen"
          v-model="selectedOpen"
          :close-on-content-click="false"
          :activator="selectedElement"
          offset-x
        >
          <v-card color="grey lighten-4" min-width="350px" flat>
            <v-toolbar :color="selectedEvent.color" dark>
              <v-toolbar-title v-html="selectedEvent.name"></v-toolbar-title>
              <v-spacer></v-spacer>
                <v-btn v-show="permissionToEdit" @click="deleteAllocation" fab small outlined><v-icon small>delete</v-icon>
                </v-btn><span>&emsp;</span>
                <edit-form-modal v-if="selectedOpen && permissionToEdit" :eventId="selectedEvent.id" @updateview="selectedOpen = false">
                  <v-icon small>edit</v-icon>
                </edit-form-modal>
            </v-toolbar>
            <v-card-text>
              <p>{{selectedEvent.schedule}}</p>
              <p><strong>Notizen: </strong>{{selectedEvent.Notes}}</p>
              <p><strong>Kontaktperson: </strong>{{ReferenceUserName(selectedEvent.Contact)}}</p>
              <p><strong>Hilfsmittel: </strong><span v-html="selectedEvent.Gadgets"></span></p>
              <p><span v-html="selectedEvent.Hints"></span></p>
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
import { State, Action, Getter, Mutation } from 'vuex-class'
import { Names } from '../store/User/types'
import { deleteAllocation } from '../services/AllocationApiService'

import { Allocation, Gadget, Supplier } from '../models'
import EditFormModal from './EditFormModal.vue'
import moment from 'moment'

@Component({
  components: { EditFormModal }
})
export default class Calendar extends Vue {
  @State('ContactUsers', { namespace: 'user' })
  private ContactUsers!: WebApi.ContactUser[]
  @Action(Names.a.loadUsers, { namespace: 'user' })
  private loadUsers: any;

  private today: string = moment().format('YYYY-MM-DD')
  private focus: string = moment().format('YYYY-MM-DD')

  private types: string[] =['month', 'week', 'day', '4day']
  private currentview: string = 'month'
  private start: any = null
  private end: any = null
  private selectedEvent: object = {}
  private selectedElement: object = {}
  private selectedOpen: Boolean = false
  private titleFilter: string[] = []
  private showFilterModal: boolean = false
  private showWE: boolean = true

  public formatEventText (e:any): string {
    if (this.currentview === 'month') return e.input.name
    let start = e.input.start.endsWith('00') ? e.input.start.substring(10, 13) : e.input.start.substring(10, 16)
    let end = ''
    if (e.input.end) end = e.input.end.endsWith('00') ? e.input.end.substring(10, 13) : e.input.end.substring(10, 16)
    if (e.input.longDate) {
      return `<strong>${e.input.name}</strong><br>${moment(e.input.start).format('DD.MM.YYYY')} ${start} Uhr -<br> ${moment(e.input.end).format('DD.MM.YYYY')} ${end} Uhr`
    } else {
      return `<strong>${e.input.name}</strong><br>${start} Uhr - ${end} Uhr`
    }
  }
  public get weekdays (): number[] {
    return this.showWE ? [1, 2, 3, 4, 5, 6, 0] : [1, 2, 3, 4, 5]
  }
  public typeName (s: string) {
    switch (s) {
      case 'month': return 'Monat'
      case 'week': return 'Woche'
      case 'day': return 'Tag'
      case '4day': return '4 Tage'
    }
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
    this.loadUsers(formatedItems.map((e:any) => e.ReferencePersonId))
    return formatedItems.map(transfer2Calendar)
  }
  public ReferenceUserName (id:number) {
    if (!id) return ''

    let user = this.ContactUsers.find(e => e.Id === id)
    return user ? user.Title : id
  }

  public get possibleTitles () {
    return Allocation.query()
      .with('Ressource')
      .get().map((v:any) => v.Ressource).map((v:any) => (v || { Name: '' }).Name)
  }
  public get title () {
    const { start, end } = this
    if (!start || !end) return moment().format('MMMM YYYY')

    const startMonth = this.monthFormatter(start)
    const endMonth = this.monthFormatter(end)
    const suffixMonth = startMonth === endMonth ? '' : endMonth

    const startYear = start.year
    const endYear = end.year
    const suffixYear = startYear === endYear ? '' : endYear

    const startDay = start.day
    const endDay = end.day
    const isoWeek = moment(start).format('W')

    switch (this.currentview) {
      case 'month':
        return `${startMonth} ${startYear}`
      case 'week': return `Woche ${isoWeek}`
      case '4day':
        if (startMonth === endMonth) {
          return `${startDay}. - ${endDay}. ${startMonth} ${startYear}`
        } else {
          return `${startDay}. ${startMonth} ${startYear} - ${endDay}. ${suffixMonth} ${suffixYear}`
        }
      case 'day':
        return `${startDay}. ${startMonth} ${startYear}`
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

  public resetFilter () {
    this.titleFilter.splice(0, Infinity)
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
    let success = await deleteAllocation(id)
    if (success) this.$dialog.message.success('Eintrag gelöscht', { position: 'center-left' })
    else this.$dialog.error({ text: 'Löschen fehlgeschlagen', title: 'Fehler' })
  }

  public viewDay ({ date }: any) {
    this.focus = date
    this.currentview = 'day'
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
}
function transfer2Calendar (v: any) {
  let rVal:any = {}
  rVal.longDate = v.From.substr(0, 10) !== v.To.substr(0, 10)
  if (v.IsAllDay) {
    rVal.start = v.From.substring(0, 10)
    rVal.schedule = `ganztägig`
    rVal.name = (v.Ressource || {}).Name + ' - ' + v.Title

    if (rVal.longDate) {
      rVal.start = v.From.substring(0, 10).replace('T', ' ')
      rVal.end = v.To.substring(0, 10).replace('T', ' ')
      rVal.schedule = `Von ${moment(v.From).format('DD.MM.YYYY')} bis ${moment(v.To).format('DD.MM.YYYY')} ganztägig`
    }
  } else {
    rVal.start = v.From.substring(0, 16).replace('T', ' ')
    rVal.end = v.To.substring(0, 16).replace('T', ' ')
    rVal.schedule = `
      Von ${moment(v.From).format('LT')} 
      bis ${moment(v.To).format('LT')}`
    if (rVal.longDate) rVal.schedule += ` zwischen dem ${moment(v.From).format('DD.MM.YYYY')} und ${moment(v.To).format('DD.MM.YYYY')}`
    rVal.name = `${(v.Ressource || {}).Name} ab ${moment(v.From).format('LT')}: ${v.Title}`
  }

  rVal.color = 'success'
  rVal.id = v.Id
  rVal.Notes = v.Notes
  rVal.Contact = v.ReferencePersonId
  // rVal.Original = v
  rVal.RessourceName = (v.Ressource || {}).Name

  rVal.Hints = ''
  v.HintsForSuppliers.forEach((e: WebApi.SimpleSupplierHint) => {
    rVal.Hints += `<strong>${GetGroupName(e.GroupId)}</strong>: ${e.Message} <br/>`
  })

  rVal.Gadgets = '<br>'
  if (Gadget.all().length) {
    v.GadgetsIds.map((e:any) => (Gadget.find(e) as any).Title)
      .forEach((e: any) => { rVal.Gadgets += e + '<br> ' })
  }
  rVal.Gadgets.slice(0, -1)

  return rVal
}
function GetGroupName (id : number) : string {
  return (Supplier.find(id) as any || { Title: '' }).Title
}
</script>

<style lang="stylus" >
#pad-bot-twenty
  padding-bottom 20em

#showWe
  margin-top -1.5em
  padding-right .5em
  i
    color: #82b1ff !important;
</style>
