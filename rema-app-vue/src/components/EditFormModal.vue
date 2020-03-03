<template>
  <v-dialog v-model="dialog" scrollable persistent max-width="1200px">
    <template v-slot:activator="{ on }">
      <v-btn v-on="on" fab small outlined>
        <slot></slot>
      </v-btn>
    </template>
    <v-card>
    <v-card-title class="headline">Termin bearbeiten</v-card-title>

    <v-card-text class="no-bottom-padding">
      <v-container class="no-bottom-padding">
        <v-row>
          <v-col>
            <v-text-field
              v-model="title"
              label="Titel"
              required
              placeholder="Der Termin benötigt einen Titel"
            ></v-text-field>
          </v-col>
          <v-col>
            <v-select
              v-model="ressourceId"
              :items="Rooms"
              item-text="Name"
              item-value="Id"
              clearable
              placeholder="Bitte wählen Sie einen Raum aus."
              label="Raum"
              :menu-props="{ offsetY: true }"
            />
          </v-col>
        </v-row>
        <v-divider />
        <v-row>
          <v-col v-if="wasRepeating" :cols="6" class="no-top-padding no-bottom-padding">
            <v-checkbox v-model="isRepeating" label="Belassen in Terminserie"></v-checkbox>
          </v-col>
          <v-col :cols="6" class="no-top-padding no-bottom-padding">
            <v-checkbox v-model="fullday" label="ganztägig"></v-checkbox>
          </v-col>

          <template v-if="isRepeating">
            <v-col v-show="!fullday" :cols="6">
              <strong>Von: </strong>
              <drop-down-time-picker v-model="timeFrom"/>
            </v-col>
            <v-col v-show="!fullday" :cols="6">
              <strong>Bis: </strong><drop-down-time-picker v-model="timeTo"/>
            </v-col>
            <v-col>
              <div v-show="isRepeating">
                <v-icon>event</v-icon>
                  <!-- Serientermindatum -->
                  <label class="date-label">
                    Datum im Serientermin
                    <input v-if="dateFromFocus" type="date" :value="dateOfSeries" @change="changeDateFrom" @blur="dateFromFocus=false" ref="dateSeries" autofocus>
                    <span v-else @click="dateFromFocus=true" class="span-input">{{dateOfSeries | toLocalDate}}</span>
                  </label> &emsp;
              </div>
            </v-col>
          </template>
        </v-row>
        <v-row v-if="!isRepeating"> <!-- single date row -->
          <v-col cols="6">
            <v-icon>event</v-icon>
            <!-- Datum von -->
            <label class="date-label">
              von
              <input v-if="dateFromFocus" type="date" :value="dateFrom" @change="changeDateFrom" @blur="dateFromFocus=false" autofocus>
              <span v-else @click="dateFromFocus=true" class="span-input">{{dateFrom | toLocalDate}}</span>
            </label> &emsp;
            <drop-down-time-picker v-model="timeFrom"/>
          </v-col>
          <v-col cols="6">
            <v-icon>event</v-icon>
            <!-- Datum bis -->
            <label class="date-label">
              von
              <input v-if="dateToFocus" type="date" :value="dateTo" @change="changeDateTo" @blur="dateToFocus=false" autofocus>
              <span v-else @click="dateToFocus=true" class="span-input">{{dateTo | toLocalDate}}</span>
            </label> &emsp;
            <drop-down-time-picker v-model="timeTo"/>
          </v-col>
        </v-row>
        <v-divider />
        <v-row>
          <v-col v-for="(group, idx) in GadgetGroups" :key="idx + 'group'" cols="6">
            <v-select
              v-model="selectedGadgets"
              :items="getGadgets(group.Id)"
              :label="'Hilfsmittel (' + group.Title + ')'"
              item-text="Title"
              item-value="Id"
              :menu-props="{ top: true, offsetY: true }"
              placeholder="Bitte wählen Sie Hilfmittel aus."
              multiple
              clearable
            >
            </v-select>
          </v-col>
        </v-row>
        <v-divider />
        <v-row>
          <v-col>
            <v-text-field
              v-model="contactPerson"
              label="Ansprechpartner(in)"
              placeholder="Bitte geben Sie eine(n) Ansprechpartner(in) an."
              required
            ></v-text-field>
          </v-col>
          <v-col>
            <v-text-field
              v-model="telNumber"
              type="tel"
              label="Telefonnummer"
              placeholder="Bitte geben Sie eine Telefonnummer an."
              required
            ></v-text-field>
          </v-col>
        </v-row>
        <v-divider />
        <v-row>
          <v-col>
            <v-textarea v-model="Notes" :label="'Notizen'" auto-grow clearable outlined></v-textarea>
          </v-col>
        </v-row>
        <v-row><v-col class="no-top-padding"><span class="right-head">Terminstatus: {{Status | status2string}}</span></v-col></v-row>
      </v-container>
    </v-card-text>
    <v-card-actions>
      <div class="flex-grow-1"></div>
      <v-btn
        v-if="permissionToEdit"
        :disabled="formInvalid"
        color="green darken-1" text
        @click="saveAllocation"
        ><v-icon>save</v-icon> Speichern</v-btn>
      <v-btn color="red darken-1" text @click="dialog = false">
        <v-icon>cancel</v-icon> Abbrechen</v-btn>
    </v-card-actions>
  </v-card>

  </v-dialog>
</template>

<script lang="ts">
import { Component, Prop, Vue, Watch } from 'vue-property-decorator'
import { AllocationRequestView } from '../models/interfaces'
import DateTimePicker from '@/components/DateTimePicker.vue'
import DropDownTimePicker from '@/components/DropdownTimePicker.vue'
import { Gadget, Ressource, Supplier, Allocation } from '../models'

@Component({
  components: { DateTimePicker, DropDownTimePicker }
})
export default class EditFormModal extends Vue {
  @Prop(Number) private eventId!: number

  private title: String = ''
  private Notes: string = ''
  private ressourceId: number | any = null
  private dateFrom: string = ''
  private timeFrom: string =''
  private dateTo: string = ''
  private timeTo: string = ''
  private fullday: boolean = false
  private selectedGadgets: number[] = []
  private telNumber: string = ''
  private contactPerson: string = ''
  private multipleDates: string[] = []
  private showMultipleDatesMenu: boolean = false
  private ReferencePersonId: number = 0
  private Status: number = 0
  private Id: number = 0
  private CreatedBy: number = 0
  private CreatedById: number = 0
  private CreatedAt: string = ''
  private ScheduleSeries: string = ''
  private dateFromFocus: boolean = false
  private dateToFocus: boolean = false
  private dateOfSeries: string = ''
  private eventAllocation: any = {}

  public isRepeating: boolean = false
  public dialog: boolean = false

  @Watch('dialog')
  private watchDialog (newVal:boolean) {
    if (newVal) {
      let all : any = Allocation.find(this.eventId)
      this.eventAllocation = all

      this.Id = all.Id
      this.title = all.Title
      this.ressourceId = all.RessourceId
      this.isRepeating = !!all.ScheduleSeries
      this.ScheduleSeries = all.ScheduleSeries
      this.fullday = all.IsAllDay
      this.ReferencePersonId = all.ReferencePersonId
      this.dateOfSeries = all.From.substring(0, 10)
      this.dateFrom = all.From.substring(0, 10)
      this.timeFrom = all.From.substring(11, 16)
      this.dateTo = all.To.substring(0, 10)
      this.timeTo = all.To.substring(11, 16)
      this.selectedGadgets = all.GadgetsIds
      this.contactPerson = all.ContactName
      this.telNumber = all.ContactPhone
      this.Notes = all.Notes

      this.Status = all.Status
      this.CreatedBy = all.CreatedBy
      this.CreatedById = all.CreatedById
      this.CreatedAt = all.CreatedAt
    }
  }
  private changeDateFrom (e:any) {
    if (e.target.value) { this.dateFrom = e.target.value.substring(0, 10) }
  }
  private changeDateTo (e:any) {
    if (e.target.value) { this.dateTo = e.target.value.substring(0, 10) }
  }
  private getGadgets (groupId: number) {
    return Gadget.query()
      .where('SuppliedBy', groupId)
      .get()
  }

  private get wasRepeating () : boolean {
    return !!this.eventAllocation.ScheduleSeries
  }
  private get Rooms () {
    function compareNumbers (a: any, b: any) {
      return (a.Name > b.Name) ? 1 : -1
    }
    return Ressource.all().sort(compareNumbers)
  }
  private get GadgetGroups () {
    return Supplier.all()
  }

  private get permissionToEdit (): boolean {
    return this.$store.state.user.role >= 10
  }

  public get formInvalid () {
    let rValue = true
    if (!this.title) rValue = false
    if (!this.ressourceId) rValue = false
    if (!this.dateFrom && !this.isRepeating) rValue = false
    if (!this.dateTo && !this.isRepeating) rValue = false
    if (!this.dateOfSeries && this.isRepeating) rValue = false
    if (!this.timeTo && !this.fullday) rValue = false
    if (!this.timeFrom && !this.fullday) rValue = false

    return !rValue
  }
  public async saveAllocation () {
    let data = {} as any
    data.Id = this.eventId

    data.Title = this.title
    data.RessourceId = this.ressourceId
    data.IsAllDay = this.fullday
    data.ReferencePersonId = this.ReferencePersonId
    data.gadgetsIds = this.selectedGadgets
    data.ContactName = this.contactPerson
    data.ContactPhone = this.telNumber
    data.Notes = this.Notes

    if (this.isRepeating) data.ScheduleSeries = this.ScheduleSeries
    else data.ScheduleSeries = ''

    if (this.isRepeating) {
      data.from = this.dateOfSeries + 'T' + (this.fullday ? '00:00' : this.timeFrom)
      data.to = this.dateOfSeries + 'T' + (this.fullday ? '23:59' : this.timeTo)
    } else {
      data.from = this.dateFrom + 'T' + (this.fullday ? '00:00' : this.timeFrom)
      data.to = this.dateTo + 'T' + (this.fullday ? '23:59' : this.timeTo)
    }

    const response = await Allocation.api().put(`allocations/${this.eventId}`,
      data
    )
    await Allocation.update(data)
    Allocation.api().get('allocations')
    this.$emit('updateview')
    this.dialog = false
  }
}
</script>

<style lang="stylus" scoped>
.span-input
  border-bottom 1px solid black
  padding 3px
.no-top-padding
  padding-top 0
.no-bottom-padding
  padding-bottom 0 !important
</style>