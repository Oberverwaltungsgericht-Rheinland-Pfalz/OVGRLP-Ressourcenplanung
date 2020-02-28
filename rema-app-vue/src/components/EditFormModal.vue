<template>
  <v-dialog v-model="dialog" scrollable persistent max-width="1200px">
    <template v-slot:activator="{ on }">
      <v-btn v-on="on" fab small outlined>
        <slot></slot>
      </v-btn>
    </template>
    <v-card>
    <v-card-title class="headline">Termin bearbeiten</v-card-title>
    <v-card-text>
      <v-container>
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
          <v-col v-if="wasRepeating" :cols="6">
            <v-checkbox v-model="isRepeating" label="Belassen in Terminserie"></v-checkbox>
          </v-col>
          <v-col :cols="6">
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
            <v-textarea v-model="Notes" :label="'Notizen'" auto-grow clearable outlined rounded></v-textarea>
          </v-col>
        </v-row>
      </v-container>
    </v-card-text>
    <v-card-actions>
      <div class="flex-grow-1"></div>
      <v-btn
        v-if="permissionToEdit"
        :disabled="formInvalid"
        color="green darken-1"
        text
        @click="sendAllocation(1)"
        ><v-icon>save</v-icon> Speichern</v-btn
      >
      <v-btn
        v-else
        :disabled="formInvalid"
        color="green darken-1"
        text
        @click="sendAllocation(0)"
        ><v-icon>save</v-icon> Anfragen</v-btn
      >
      <v-btn color="red darken-1" text @click="dialog = false"
        ><v-icon>cancel</v-icon> Abbrechen</v-btn
      >
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
  @Prop(Object) private viewAllocation!: any

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
  private CreatedAt: string = ''
  private ScheduleSeries: string = ''
  private dateFromFocus: boolean = false
  private dateToFocus: boolean = false
  private dateOfSeries: string = ''

  public isRepeating: boolean = false
  // public valid: boolean = false
  // public editId: number = 0
  // private Description: string = ''

  public dialog: boolean = false

  @Watch('dialog')
  private watchDialog (newVal:boolean) {
    if (newVal) {
      this.title = this.viewAllocation.Original.Title
      this.fullday = this.viewAllocation.Original.IsAllDay
      this.Notes = this.viewAllocation.Original.Notes
      this.ScheduleSeries = this.viewAllocation.Original.ScheduleSeries
      this.isRepeating = !!this.viewAllocation.Original.ScheduleSeries
      this.ressourceId = this.viewAllocation.Original.RessourceId
      this.ReferencePersonId = this.viewAllocation.Original.ReferencePersonId
      this.Status = this.viewAllocation.Original.Status
      this.Id = this.viewAllocation.Original.Id
      this.dateFrom = this.viewAllocation.Original.From.substring(0, 10)
      this.timeFrom = this.viewAllocation.Original.From.substring(11, 16)
      this.dateTo = this.viewAllocation.Original.To.substring(0, 10)
      this.timeTo = this.viewAllocation.Original.To.substring(11, 16)
      this.contactPerson = this.viewAllocation.Original.ContactName
      this.CreatedBy = this.viewAllocation.CreatedBy
      this.CreatedAt = this.viewAllocation.CreatedAt
      this.telNumber = this.viewAllocation.Original.ContactPhone
      this.selectedGadgets = this.viewAllocation.Original.GadgetsIds
      this.dateOfSeries = this.viewAllocation.start.substring(0, 10)
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
    return !!this.viewAllocation.Original.ScheduleSeries
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
}
</script>

<style lang="stylus" scoped>
.span-input
  border-bottom 1px solid black
  padding 3px

</style>
