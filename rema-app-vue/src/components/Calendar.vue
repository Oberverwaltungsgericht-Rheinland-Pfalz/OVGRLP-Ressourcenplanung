<template>
  <v-layout fill-height>
    <v-flex>
      <v-sheet height="64">
        <v-toolbar flat color="white">
          <v-btn outlined class="mr-4" @click="setToday" title="Ansicht auf aktuellen Tag">Heute</v-btn>
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
              <v-btn v-on="on" color="primary" title="Anzeigte Termine filtern">Suchkriterien ({{titleFilter.length}})</v-btn>
            </template>
            <v-card>
              <v-card-title>Anzeige einschränken auf:
                <v-spacer/>
                <v-btn @click="resetFilter" :style="{visibility: titleFilter.length ? 'visible':'hidden'}" color="warning" right class="ma-2">Zurücksetzen</v-btn>
                <v-btn @click="showFilterModal=false" class="ma-2" right color="success" title="Filterkriterien anwenden">Ok</v-btn>
              </v-card-title>
              <v-card-text id="pad-bot-twenty">
                <v-select v-model="titleFilter" :items="possibleTitles" item-text="Name" item-value="Id"
                  attach chips label="Räume" multiple single-line></v-select>
              </v-card-text>
            </v-card>
          </v-dialog>
          <v-spacer/>

          <v-checkbox title="Wochenenden aus/einblenden" class="showWe" v-model="showWE" :label="'WE'" on-icon="visibility" off-icon="visibility_off"></v-checkbox>
          <v-radio-group v-model="currentview" row @change="scrollToTime">
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
          :first-interval="HideCalendarFrom ? CalendarFrom : 0"
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
          <v-card color="grey lighten-4" min-width="400px" flat>
            <v-toolbar :color="selectedEvent.color" dark>
              <v-toolbar-title v-html="selectedEvent.name"></v-toolbar-title>
              <v-spacer></v-spacer>
                <v-btn @click="printAllocation(selectedEvent.id)" fab small outlined><v-icon>print</v-icon></v-btn>
                <v-btn v-show="permissionToEdit" @click="confirmDelete" fab small outlined><v-icon small>delete</v-icon>
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
              <span v-if="!selectedEvent.Contact"><strong>Erfasst von:</strong> {{ReferenceUserName(selectedEvent.Creator)}}</span>
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
import moment from 'moment'
import print from 'print-js'
import { Names } from '../store/User/types'
import Ressources from '../views/Ressources.vue'
import EditFormModal from './EditFormModal.vue'
import { Component, Prop, Vue } from 'vue-property-decorator'
import { State, Action, Getter, Mutation } from 'vuex-class'
import { CalendarEventParsed, CalendarTimestamp } from 'vuetify'
import { deleteAllocation, errorCallbackFactory } from '../services/AllocationApiService'
import { Allocation, Gadget, Ressource, Supplier } from '../models'
import { ShowToast, ConfirmData, CalendarElement } from '../models/interfaces'

@Component({
  components: { EditFormModal }
})
export default class Calendar extends Vue {
  @State('ContactUsers', { namespace: 'user' })
  ContactUsers!: WebApi.ContactUser[]
  @State('calendarFrom', { namespace: 'user' })
  CalendarFrom!: number
  @State('hideCalendarFrom', { namespace: 'user' })
  HideCalendarFrom!: boolean
  @Action(Names.a.loadUsers, { namespace: 'user' })
  loadUsers!: Function

  today: string = moment().format('YYYY-MM-DD')
  focus: string = moment().format('YYYY-MM-DD')

  types: string[] =['month', 'week', 'day', '4day']
  currentview: string = 'month'
  start: CalendarTimestamp = {} as CalendarTimestamp
  end: CalendarTimestamp = {} as CalendarTimestamp
  selectedEvent: CalendarElement = {} as CalendarElement
  selectedElement: CalendarElement = {} as CalendarElement
  selectedOpen: Boolean = false
  titleFilter: Array<number> = []
  showFilterModal: boolean = false
  showWE: boolean = true

  public formatEventText (e: CalendarEventParsed): string {
    // if (this.currentview === 'month') return e.input.name
    let start = e.input.start.endsWith('00') ? e.input.start.substring(10, 13) : e.input.start.substring(10, 16)
    let end = ''
    if (e.input.end) end = e.input.end.endsWith('00') ? e.input.end.substring(10, 13) : e.input.end.substring(10, 16)
    //  if (e.input.name) return 'x'
    if (e.input.longDate) {
      return `<span class="input-name"><strong>${e.input.name}</strong><br>
      <span class="not-bold">${moment(e.input.start).format('DD.MM.YYYY')} ${start} Uhr -<br> ${moment(e.input.end).format('DD.MM.YYYY')} ${end} Uhr</span></span>`
    } else {
      return `<span class="input-name"><strong>${e.input.name}</strong><br><span class="not-bold">${start} Uhr - ${end} Uhr</span></span>`
    }
  }
  public get weekdays (): number[] {
    return this.showWE ? [1, 2, 3, 4, 5, 6, 0] : [1, 2, 3, 4, 5]
  }
  public typeName (s: string): undefined | string {
    switch (s) {
      case 'month': return 'Monat'
      case 'week': return 'Woche'
      case 'day': return 'Tag'
      case '4day': return '4 Tage'
    }
  }
  public get filteredItems (): Array<CalendarElement> {
    let isEmpty = !Allocation.all().length
    if (isEmpty) return []

    return this.itemsFormated
      .filter((v: CalendarElement) => {
        if (this.titleFilter.length) {
          return this.titleFilter.some((rId: number) => v.RessourceIds.includes(rId))
        }
        return true
      })
  }
  public get itemsFormated (): Array<CalendarElement> {
    var roleLvl = this.$store.state.user.role
    let myId = this.$store.state.user.id

    var formatedItems: Array<Allocation> = []
    if (roleLvl >= 10) {
      formatedItems = Allocation.query()
        .with('Ressources')
        .with('Gadgets')
        .get()
    } else {
      formatedItems = Allocation.query()
        .where((al: Allocation) => (al.Status === 1 || al.Status === 3) || al.CreatedById === myId || al.ReferencePersonId === myId)
        .with('Ressources')
        .with('Gadgets')
        .get()
    }
    let userIds2Load = formatedItems.map((e: Allocation) => e.ReferencePersonId)
    userIds2Load.push(...formatedItems.map((e: Allocation) => e.CreatedById))
    this.loadUsers(userIds2Load)
    return formatedItems.map(transfer2Calendar)
  }

  public ReferenceUserName (id:number): string | number {
    if (!id) return ''

    let user = this.ContactUsers.find(e => e.Id === id)
    return user ? user.Title : id
  }

  public get possibleTitles (): Array<SelectedRessource> {
    const resIds = new Set<number>()
    const ressources: Array<SelectedRessource> = []
    Allocation.query()
      .with('Ressources')
      .get().forEach((v:Allocation) => {
        v.Ressources.forEach((r: Ressource) => {
          if (!resIds.has(r.Id)) {
            resIds.add(r.Id)
            ressources.push({ Name: r.Name, Id: r.Id })
          }
        })
      })
    return ressources
  }

  public get title (): string {
    const { start, end } = this
    if (!start || !end) return moment().format('MMMM YYYY')

    const isoWeek = moment(start.date).format('W')
    const startMonth = moment(start.date).format('MMMM') // start.month
    const endMonth = moment(end.date).format('MMMM')
    const suffixMonth = startMonth === endMonth ? '' : endMonth

    const startYear = start.year
    const endYear = end.year
    const suffixYear = startYear === endYear ? '' : endYear

    const startDay = start.day
    const endDay = end.day

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

  public resetFilter (): void {
    this.titleFilter.splice(0, Infinity)
  }

  public printAllocation (id: number): void {
    print('api/Allocations/print/' + id)
  }
  confirmDelete (): void {
    const { id, name } = this.selectedEvent as any
    let data: ConfirmData = { title: 'Löschen bestätigen',
      content: `Möchten sie dem Termin ${name} wirklich löschen?`,
      callback: this.deleteAllocation,
      id
    }
    this.$root.$emit('user-confirm', data)
  }
  public async deleteAllocation (): Promise<void> {
    let errorCallback = errorCallbackFactory(this)
    const { id, name } = this.selectedEvent as any
    let success = await deleteAllocation(id, errorCallback)

    if (success) this.$root.$emit('notify-user', { text: 'Eintrag gelöscht', color: 'success' } as ShowToast)
    else this.$root.$emit('notify-user', { text: 'Löschen fehlgeschlagen', color: 'error' } as ShowToast)
  }

  public scrollToTime (): void {
    this.$nextTick(() => {
      // @ts-ignore
      this.$refs.calendar.scrollToTime(this.CalendarFrom.length > 1 ? this.CalendarFrom + ':00' : '0' + this.CalendarFrom + ':00')
    })
  }

  public viewDay ({ date }: CalendarTimestamp): void {
    this.focus = date
    this.currentview = 'day'

    this.scrollToTime()
  }
  public getEventColor ({ color }: any): string {
    return color
  }
  public setToday (): void {
    this.focus = this.today
  }
  public prev (): void {
    // @ts-ignore
    this.$refs.calendar.prev()
  }
  public next (): void {
    // @ts-ignore
    this.$refs.calendar.next()
  }
  public showEvent ({ nativeEvent, event }: any): void {
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
  public updateRange ({ start, end }: {start: CalendarTimestamp, end: CalendarTimestamp}): void {
    // You could load events from an outside source (like database)
    // now that we have the start and end dates on the calendar
    this.start = start
    this.end = end

    this.scrollToTime()
  }
}
function transfer2Calendar (v: Allocation): CalendarElement {
  let rVal: CalendarElement = {} as CalendarElement
  rVal.longDate = v.From.substr(0, 10) !== v.To.substr(0, 10)
  rVal.RessourceIds = v.Ressources.map((r: Ressource) => r.Id)

  let ressourceNames = v.Ressources.map((v: Ressource) => v.Name).join(', ')

  if (v.IsAllDay) {
    rVal.start = v.From.substring(0, 10)
    rVal.schedule = `ganztägig`
    rVal.RessourceNames = ressourceNames
    rVal.name = ressourceNames + ' - ' + v.Title

    if (rVal.longDate) {
      rVal.start = v.From.substring(0, 10).replace('T', ' ') // ,10) -> kleine Darstellung; ,16) maximal große Darstellung
      rVal.end = v.To.substring(0, 10).replace('T', ' ') // ,10) -> kleine Darstellung; ,16) maximal große Darstellung
      rVal.schedule = `Von ${moment(v.From).format('DD.MM.YYYY')} bis ${moment(v.To).format('DD.MM.YYYY')} ganztägig`
    }
  } else {
    rVal.start = v.From.substring(0, 16).replace('T', ' ')
    rVal.end = v.To.substring(0, 16).replace('T', ' ')
    rVal.schedule = `
      Von ${moment(v.From).format('LT')} 
      bis ${moment(v.To).format('LT')}`
    if (rVal.longDate) rVal.schedule += ` zwischen dem ${moment(v.From).format('DD.MM.YYYY')} und ${moment(v.To).format('DD.MM.YYYY')}`
    rVal.name = `${ressourceNames} ab ${moment(v.From).format('LT')}: ${v.Title}`
  }

  if (v.Status === 1 || v.Status === 3) {
    rVal.color = 'success'
  } else if (v.Status === 2) {
    rVal.color = 'warning'
  } else {
    rVal.color = 'info'
  }
  rVal.id = v.Id
  rVal.Notes = v.Notes
  rVal.Contact = v.ReferencePersonId
  if (!rVal.Contact) rVal.Creator = v.CreatedById

  rVal.Hints = ''
  v.HintsForSuppliers.forEach((e: WebApi.SimpleSupplierHint) => {
    rVal.Hints += `<strong>${GetGroupName(e.GroupId)}</strong>: ${e.Message} <br/>`
  })

  rVal.Gadgets = '<br>'
  if (Gadget.all().length) {
    v.GadgetsIds.map((e: number) => {
      let gadget = (Gadget.find(e) as Gadget)
      let title = (gadget || { Title: '' }).Title
      return title
    }).forEach((e: string) => { rVal.Gadgets += e + '<br> ' })
  }
  rVal.Gadgets.slice(0, -1)

  return rVal
}
function GetGroupName (id : number) : string {
  return (Supplier.find(id) as any || { Title: '' }).Title
}
interface SelectedRessource {
  Name: string
  Id: number
}
</script>

<style lang="stylus" >
#pad-bot-twenty
  padding-bottom 20em

.showWe
  margin-right .5em

.pl-1 > strong, .input-name
    visibility visible

.pl-1:not(.v-event-more)
  visibility hidden
.not-bold
  font-weight normal

</style>
