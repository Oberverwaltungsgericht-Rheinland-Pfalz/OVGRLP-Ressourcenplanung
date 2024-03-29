<template>
  <v-dialog v-model="dialog" scrollable :persistent="!readonly" max-width="1200px">
    <template v-slot:activator="{ on }">
      <v-btn v-on="on" fab small outlined v-show="!show">
        <slot></slot>
      </v-btn>
    </template>
    <v-card>
    <v-card-title v-if="!readonly" class="headline">Termin bearbeiten</v-card-title>
    <v-card-title v-else class="headline">Termin ansehen</v-card-title>

    <v-card-text class="no-bottom-padding">
      <v-container class="no-bottom-padding">
        <v-row>
          <v-col>
            <title-proposal v-model="title" :error="!title && checkForm" :readonly="readonly"/>
          </v-col>
          <v-col>
            <v-select
              v-model="ressourceIds"
              :items="RoomsActivated"
              item-disabled="IsDeactivated"
              :error="!ressourceIds && checkForm"
              item-text="Name"
              item-value="Id"
              clearable multiple
              placeholder="Bitte wählen Sie einen Raum aus."
              label="Raum"
              :menu-props="{ offsetY: true }"
              :disabled="readonly"
            >
                <template v-slot:item="{ active, item, attrs, on }">
                  <v-list-item v-on="on" v-bind="attrs" #default="{ active }">
                    <v-list-item-action>
                      <v-checkbox :input-value="active"></v-checkbox>
                    </v-list-item-action>
                    <v-list-item-content>
                      <v-list-item-title>
                        <v-row no-gutters align="center">
                        <span>{{ item.Name }}</span>
                        <v-spacer></v-spacer>
                        <v-chip text-color="white" color="blue" small v-if="item.FunctionDescription" :title="'Funktion: '+item.FunctionDescription"
                        >F</v-chip>
                        <v-chip text-color="white" color="blue" class="ressourcedetails" small v-if="item.SpecialsDescription" :title="'Details: '+item.SpecialsDescription"
                        >D</v-chip>
                        </v-row>
                      </v-list-item-title>
                    </v-list-item-content>
                  </v-list-item>
                </template>
            </v-select>
          </v-col>
        </v-row>
        <v-divider />
        <v-row>
          <v-col v-if="wasRepeating" :cols="6" class="no-top-padding no-bottom-padding">
            <v-checkbox v-model="isRepeating" label="Belassen in Terminserie" :disabled="readonly"></v-checkbox>
          </v-col>
          <v-col :cols="6" class="no-top-padding no-bottom-padding">
            <v-checkbox v-model="fullday" label="ganztägig" :disabled="readonly"></v-checkbox>
          </v-col>

          <template v-if="isRepeating">
            <v-col v-show="!fullday" :cols="6">
              <strong>Von: </strong>
              <drop-down-time-picker v-model="timeFrom" :readonly="readonly"/>
            </v-col>
            <v-col v-show="!fullday" :cols="6">
              <strong>Bis: </strong><drop-down-time-picker v-model="timeTo" :readonly="readonly"/>
            </v-col>
            <v-col v-show="isRepeating" class="no-top-padding"><!-- Serientermindatum -->
                <v-menu ref="menu1" :close-on-content-click="true" transition="scale-transition" offset-y max-width="290px" min-width="290px"
                v-model="menu1">
                <template v-slot:activator="{ on }">
                  <v-text-field persistent-hint prepend-icon="event" v-on="on" readonly
                  label="Datum im Serientermin"
                  :value="dateFormatted(dateOfSeries)"
                  ></v-text-field>
                </template>
                <v-date-picker v-model="dateOfSeries" :min="dateMin" locale="de" :first-day-of-week="1" no-title @input="menu1 = false" :readonly="readonly">
                  <v-btn text color="primary" @click="menu1 = false" block>Abbrechen</v-btn>
                </v-date-picker>
              </v-menu>
            </v-col>
          </template>
        </v-row>
        <v-row v-if="!isRepeating"> <!-- single date row -->
          <v-col cols="3">
            <v-menu ref="fromMenu" :close-on-content-click="true" transition="scale-transition" offset-y max-width="290px" min-width="290px"
            v-model="fromMenu">
            <template v-slot:activator="{ on }">
              <v-text-field persistent-hint prepend-icon="event" v-on="on" readonly
              label="Von"
              :value="dateFormatted(dateFrom)"
              ></v-text-field>
            </template>
            <v-date-picker v-model="dateFrom" :min="dateMin" locale="de" :first-day-of-week="1" no-title @input="fromMenu = false" :readonly="readonly">
              <v-btn text color="primary" @click="fromMenu = false" block>Abbrechen</v-btn>
            </v-date-picker>
          </v-menu>
          </v-col>
          <v-col cols="3" class="time-col">
            <drop-down-time-picker v-show="!fullday" v-model="timeFrom" :readonly="readonly"/>
          </v-col>

          <v-col cols="3">
            <v-menu ref="toMenu" :close-on-content-click="true" transition="scale-transition" offset-y max-width="290px" min-width="290px"
            v-model="toMenu">
            <template v-slot:activator="{ on }">
              <v-text-field persistent-hint prepend-icon="event" v-on="on" readonly
              label="Bis"
              :value="dateFormatted(dateTo)"
              ></v-text-field>
            </template>
            <v-date-picker v-model="dateTo" :min="dateToMin" locale="de" :first-day-of-week="1" no-title @input="toMenu = false" :readonly="readonly">
              <v-btn text color="primary" @click="toMenu = false" block>Abbrechen</v-btn>
            </v-date-picker>
          </v-menu>
          </v-col>
          <v-col cols="3" class="time-col">
            <drop-down-time-picker v-show="!fullday" v-model="timeTo" min="timeToMin" :readonly="readonly"/>
          </v-col>
        </v-row>
        <v-divider />
        <v-row v-for="(group, idx) in GadgetGroups" :key="idx + 'group'">
          <v-col cols="6">
            <v-select
              v-model="selectedGadgets"
              :items="getGadgets(group.Id)"
              item-disabled="IsDeactivated"
              :label="'Hilfsmittel (' + group.Title + ')'"
              item-text="Title"
              item-value="Id"
              :menu-props="{ top: true, offsetY: true }"
              placeholder="Bitte wählen Sie Hilfmittel aus."
              multiple
              :disabled="readonly"
            >
            </v-select>
          </v-col>
          <v-col>
            <v-text-field
              v-model="groupTexts[group.Id]"
              :label="'Nachricht für ' + group.Title"
              placeholder="Nachrichttext"
              required :disabled="readonly"
            ></v-text-field>
          </v-col>
        </v-row>
        <v-divider />
        <v-row>
          <v-col>
            <input-reference-person v-if="dialog" :userid="ReferencePersonId" @selected="setReferencePerson" :key="'re'+refreshInputReferencePerson" :readonly="readonly"/>
          </v-col>
          <v-col>
            <v-text-field
              v-model="telNumber"
              type="tel"
              label="Telefonnummer"
              placeholder="Bitte geben Sie eine Telefonnummer an."
              required :disabled="readonly"
            ></v-text-field>
          </v-col>
        </v-row>
        <v-divider />
        <v-row>
          <v-col>
            <v-textarea v-model="Notes" :label="'Notizen'" auto-grow clearable outlined :disabled="readonly" hide-details=true rows="2"></v-textarea>
          </v-col>
        </v-row>
        <collision-detection :viewAllocation="RessourceChecker" @has-collisions="hasCollisions = $event" />
        <v-row><v-col class="no-top-padding"><span class="right-head">Terminstatus: {{Status | status2string}}</span></v-col></v-row>
      </v-container>
    </v-card-text>
    <v-card-actions>
      <div class="flex-grow-1"></div>
      <v-icon v-if="hasCollisions" color="orange" title="Mögliche Kollisionen">warning</v-icon>
      <v-progress-circular v-if="loading" indeterminate color="primary"></v-progress-circular>
      <v-btn
        v-if="permissionToEdit && !readonly "
        color="green darken-1" text
        @click="saveAllocation"
        ><v-icon>save</v-icon> Speichern</v-btn>
      <v-btn color="red darken-1" text @click="dialog = false">
        <v-icon>cancel</v-icon>
         <span v-if="readonly">Schließen</span><span v-else>Schließen</span></v-btn>
    </v-card-actions>
  </v-card>

  </v-dialog>
</template>

<script lang="ts">
import { mixins } from 'vue-class-component'
import TitleProposal from './TitleProposal.vue'
import CollisionDetection from './CollisionDetection.vue'
import { Gadget, Allocation } from '../models'
import DropDownTimePicker from '@/components/DropdownTimePicker.vue'
import { Component, Prop, Watch } from 'vue-property-decorator'
import { ShortAllocationView, ShowToast } from '../models/interfaces'
import AllocationFormService from '../services/AllocationFormServices'
import { refreshAllocations, editAllocation, errorCallbackFactory } from '../services/AllocationApiService'
import InputReferencePerson from '@/components/NewAllocation/InputReferencePerson.vue'

@Component({
  components: { DropDownTimePicker, InputReferencePerson, TitleProposal, CollisionDetection }
})
export default class EditFormModal extends mixins(AllocationFormService) {
  @Prop(Number) private eventId!: number
  @Prop(Boolean) private show! : boolean
  @Prop(Boolean) private readonly! : boolean

  private menu1: boolean = false
  private title: string = ''
  private Notes: string = ''
  private fullday: boolean = false
  private selectedGadgets: number[] = []
  private multipleDates: string[] = []
  private showMultipleDatesMenu: boolean = false
  private ReferencePersonId: number = 0
  private Status: number = 0
  private Id: number = 0
  private CreatedById: number = 0
  private CreatedAt: string = ''
  private ScheduleSeries: string = ''
  private dateFromFocus: boolean = false
  private dateToFocus: boolean = false
  private dateOfSeries: string = ''
  private eventAllocation: Allocation = {} as Allocation

  public isRepeating: boolean = false
  public dialog: boolean = false
  public loading: boolean = false

  private mounted () {
    this.dialog = this.show
  }
  @Watch('show')
  public toggle (newVal: boolean) {
    this.dialog = newVal
  }
  @Watch('dialog')
  public async watchDialog (newVal:boolean) {
    if (newVal) {
      let all : Allocation = Allocation.find(this.eventId) || {} as Allocation
      this.eventAllocation = all

      this.Id = all.Id
      this.title = all.Title
      this.ressourceIds.splice(0, Infinity, ...all.RessourceIds)
      this.isRepeating = !!all.ScheduleSeries
      this.ScheduleSeries = all.ScheduleSeries
      this.fullday = all.IsAllDay
      this.ReferencePersonId = all.ReferencePersonId
      this.referencePerson.ActiveDirectoryID = `${all.ReferencePersonId}`
      this.dateOfSeries = all.From.substring(0, 10)
      this.dateFrom = all.From.substring(0, 10)
      this.timeFrom = all.From.substring(11, 16)
      this.dateTo = all.To.substring(0, 10)
      this.timeTo = all.To.substring(11, 16)
      this.selectedGadgets = all.GadgetsIds
      this.telNumber = all.ContactPhone
      this.Notes = all.Notes
      for (let key in all.HintsForSuppliers as WebApi.SimpleSupplierHint[]) {
        let obj = all.HintsForSuppliers[key] as WebApi.SimpleSupplierHint
        this.groupTextsInternal[obj.GroupId] = obj.Message
      }

      this.Status = all.Status
      this.CreatedById = all.CreatedById
      this.CreatedAt = all.CreatedAt
    } else {
      this.$emit('close')
    }
  }
  public get RessourceChecker () : ShortAllocationView {
    let from, to
    if (this.isRepeating) {
      from = this.dateOfSeries + 'T' + (this.fullday ? '00:00' : this.timeFrom)
      to = this.dateOfSeries + 'T' + (this.fullday ? '23:59' : this.timeTo)
    } else {
      from = this.dateFrom + 'T' + (this.fullday ? '00:00' : this.timeFrom)
      to = this.dateTo + 'T' + (this.fullday ? '23:59' : this.timeTo)
    }
    return { Id: this.Id, From: from, To: to, RessourceIds: this.ressourceIds, dates: null }
  }

  private get wasRepeating () : boolean {
    return !!this.eventAllocation.ScheduleSeries
  }

  public get formInvalid () {
    let rValue = true
    if (!this.title) rValue = false
    if (!this.ressourceIds.length) rValue = false
    if (!this.dateFrom && !this.isRepeating) rValue = false
    if (!this.dateTo && !this.isRepeating) rValue = false
    if (!this.dateOfSeries && this.isRepeating) rValue = false
    if (!this.timeTo && !this.fullday) rValue = false
    if (!this.timeFrom && !this.fullday) rValue = false
    if (this.dateTo === this.dateFrom && (this.timeFrom >= this.timeTo)) rValue = false

    return !rValue
  }
  public async saveAllocation () {
    if (this.isFormInvalid()) return

    let callbackFn = async () => { // eigentlich Funktion, wird aufgerufen wenn Bestätigung erteilt wurde
      let data = {} as WebApi.AllocationViewModel
      data.Id = this.eventId

      data.Title = this.title
      data.RessourceIds = [...this.ressourceIds]
      data.IsAllDay = this.fullday

      if (this.referencePerson.ActiveDirectoryID) { // reference person was set in the ui
        data.ReferencePersonId = this.referencePerson.ActiveDirectoryID
      } else if (!this.referencePerson.ActiveDirectoryID && this.ReferencePersonId) { // no reference person was set
        data.ReferencePersonId = ''
      }

      data.GadgetsIds = this.selectedGadgets
      data.ContactPhone = this.telNumber
      data.Notes = this.Notes
      data.HintsForSuppliers = this.GetHintsForSuppliers

      if (this.isRepeating) data.ScheduleSeries = this.ScheduleSeries
      else data.ScheduleSeries = ''

      if (this.isRepeating) {
        data.From = this.dateOfSeries + 'T' + (this.fullday ? '00:00' : this.timeFrom)
        data.To = this.dateOfSeries + 'T' + (this.fullday ? '23:59' : this.timeTo)
      } else {
        data.From = this.dateFrom + 'T' + (this.fullday ? '00:00' : this.timeFrom)
        data.To = this.dateTo + 'T' + (this.fullday ? '23:59' : this.timeTo)
      }

      this.loading = true
      let success = await editAllocation(data, errorCallbackFactory(this))
      this.loading = false
      if (success) this.$root.$emit('notify-user', { text: 'Bearbeitung gespeichert', color: 'success' } as ShowToast)
      else this.$root.$emit('notify-user', { text: 'Bearbeitung speichern fehlgeschlagen', color: 'error' } as ShowToast)

      await Allocation.update(data)
      await refreshAllocations()
      this.$emit('updateview')
      this.dialog = false
    }
    if (this.hasCollisions) this.saveDateWithWarning(callbackFn)
    else callbackFn()
  }
}
</script>

<style lang="stylus">
.no-top-padding
  padding-top 0
.no-bottom-padding
  padding-bottom 0 !important
.time-col
  font-size 16px
  padding-top 1.35em
  padding-bottom 0em

  option:disabled
    background-color #8b000078

</style>
