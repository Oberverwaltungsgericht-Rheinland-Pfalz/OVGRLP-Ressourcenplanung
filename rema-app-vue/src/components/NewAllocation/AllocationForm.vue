<template>
  <v-card>
    <v-card-title v-if="permissionToEdit" class="headline">
      Termin eintragen
    </v-card-title>
    <v-card-title v-else class="headline">
      Neue Terminanfrage stellen
    </v-card-title>
    <v-card-text>
      <v-container>

        <v-row>
          <v-col>
            <title-proposal v-model="title" :error="!title && checkForm" />
          </v-col>
          <v-col>
            <v-select
              v-model="ressourceIds"
              :items="Rooms"
              item-text="Name"
              item-value="Id"
              clearable multiple
              :error="!ressourceIds && checkForm"
              placeholder="Bitte wählen Sie einen Raum aus."
              label="Raum *"
              :menu-props="{ offsetY: true }"
            />
          </v-col>
        </v-row>
        <v-divider />
        <v-row>
          <v-col :cols="6">
            <v-checkbox v-model="isRepeating" label="Wiederkehrender Termin"></v-checkbox>
          </v-col>
          <v-col :cols="6">
            <v-checkbox v-model="fullday" label="ganztägig"></v-checkbox>
          </v-col>

          <template v-if="isRepeating">
            <v-col v-show="!fullday" :cols="6"><strong>Von: </strong><drop-down-time-picker v-model="timeFrom"/></v-col>
            <v-col v-show="!fullday" :cols="6"><strong>Bis: </strong><drop-down-time-picker v-model="timeTo"/></v-col>
            <v-col>
              <div v-show="isRepeating">
                <v-menu ref="showMultipleDatesMenu" :close-on-content-click="false" :return-value.sync="multipleDates" transition="scale-transition" offset-y min-width="290px"
                  v-model="showMultipleDatesMenu"
                >
                  <template v-slot:activator="{ on }">
                    <v-combobox multiple chips small-chips readonly prepend-icon="event" v-on="on"
                      label="Gewählte Termine"
                      v-model="multipleDates">
                      <template v-slot:selection="data">
                        <v-chip>{{data.item | toLocalDate}} <v-icon right @click="removeDate(data.item)">close</v-icon></v-chip>
                      </template>
                    </v-combobox>
                  </template>
                  <v-date-picker v-model="multipleDates" locale="de" multiple :first-day-of-week="1" no-title scrollable :min="dateMin">
                    <v-spacer></v-spacer>
                    <v-btn text color="primary" @click="showMultipleDatesMenu = false">Abbrechen</v-btn>
                    <v-btn text color="primary" @click="$refs.showMultipleDatesMenu.save(multipleDates)">OK</v-btn>
                  </v-date-picker>
                </v-menu>
              </div>
            </v-col>
          </template>
        </v-row>
        <v-row v-if="!isRepeating"> <!-- single date row -->
          <v-col cols="3">
            <v-menu ref="fromMenu" transition="scale-transition" offset-y max-width="290px" min-width="290px"
            v-model="fromMenu">
            <template v-slot:activator="{ on }">
              <v-text-field persistent-hint prepend-icon="event" v-on="on" readonly
              label="Von *"
              :error="!dateFrom && checkForm"
              :value="dateFormatted(dateFrom)"
              ></v-text-field>
            </template>
            <v-date-picker v-model="dateFrom" :min="dateMin" locale="de" :first-day-of-week="1" no-title >
              <v-btn text color="primary" @click="fromMenu = false" block>Abbrechen</v-btn>
            </v-date-picker>
          </v-menu>
          </v-col>
          <v-col cols="3" class="time-col">
            <drop-down-time-picker v-show="!fullday" v-model="timeFrom"/>
          </v-col>

          <v-col cols="3">
            <v-menu ref="toMenu" :close-on-content-click="true" transition="scale-transition" offset-y max-width="290px" min-width="290px"
            v-model="toMenu">
            <template v-slot:activator="{ on }">
              <v-text-field persistent-hint prepend-icon="event" v-on="on" readonly
              label="Bis *"
              :value="dateFormatted(dateTo)"
              :error="!dateTo && checkForm"
              ></v-text-field>
            </template>
            <v-date-picker v-model="dateTo" :min="dateToMin" locale="de" :first-day-of-week="1" no-title @input="toMenu = false">
              <v-btn text color="primary" @click="toMenu = false" block>Abbrechen</v-btn>
            </v-date-picker>
          </v-menu>
          </v-col>
          <v-col cols="3" class="time-col">
            <drop-down-time-picker v-show="!fullday" v-model="timeTo" min="timeToMin"/>
          </v-col>
        </v-row>
        <v-divider />
        <v-row  v-for="(group, idx) in GadgetGroups" :key="idx + 'group'">
          <v-col cols="6">
            <v-select
              v-model="selectedGadgets"
              :items="getGadgets(group.Id)"
              :label="'Hilfsmittel (' + group.Title + ')'"
              item-text="Title"
              item-value="Id"
              :menu-props="{ top: true, offsetY: true }"
              placeholder="Bitte wählen Sie Hilfmittel aus."
              multiple
            >
            </v-select>
          </v-col>
          <v-col>
            <v-text-field
              v-model="groupTexts[group.Id]"
              :label="'Nachricht für ' + group.Title"
              placeholder="Nachrichttext"
              required
            ></v-text-field>
          </v-col>
        </v-row>
        <v-divider />
        <v-row>
          <v-col>
            <input-reference-person @selected="setReferencePerson" :key="'re'+refreshInputReferencePerson"/>
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
            <v-textarea
              v-model="notes"
              :label="'Notizen'"
              rows="2"
              hide-details=true
              auto-grow
              clearable
              outlined
              rounded
            ></v-textarea>
          </v-col>
        </v-row>
      </v-container>
      <collision-detection :viewAllocation="RessourceChecker"/>
    </v-card-text>
    <v-card-actions>
      <div class="flex-grow-1"></div>
      <v-progress-circular v-if="loading" indeterminate color="primary"></v-progress-circular>
      <v-btn
        v-if="permissionToEdit"
        color="green darken-1"
        text
        @click="sendAllocation('Acknowledged')"
        ><v-icon>save</v-icon> Speichern</v-btn
      >
      <v-btn
        v-else
        color="green darken-1"
        text
        @click="sendAllocation('Pending')"
        ><v-icon>save</v-icon> Anfragen</v-btn
      >
      <v-btn color="red darken-1" text @click="close"
        ><v-icon>cancel</v-icon> Abbrechen</v-btn
      >
    </v-card-actions>
  </v-card>
</template>

<script lang="ts">
import moment from 'moment'
import TitleProposal from '../TitleProposal.vue'
import CollisionDetection from '../CollisionDetection.vue'
import { State, Action, Getter, Mutation } from 'vuex-class'
import DropDownTimePicker from '@/components/DropdownTimePicker.vue'
import { Gadget, Ressource, Supplier, Allocation } from '../../models'
import AllocationFormService from '../../services/AllocationFormServices'
import { Component, Prop, Vue, Watch, Mixins } from 'vue-property-decorator'
import InputReferencePerson from '@/components/NewAllocation/InputReferencePerson.vue'
import { InitAllocation, ShortAllocationView, ShowToast } from '../../models/interfaces'
import { submitAllocation, submitAllocations, refreshAllocations } from '../../services/AllocationApiService'

@Component({
  components: {
    DropDownTimePicker, InputReferencePerson, TitleProposal, CollisionDetection
  }
})
export default class AllocationForm extends Mixins(AllocationFormService) {
  @Prop(Object) private readonly initValues!: InitAllocation
  public title: string = ''
  public ressourceIds: Array<number> = []
  public fullday: boolean = false
  public notes: string = ''
  public selectedGadgets: number[] = []
  private multipleDates: string[] = []
  private showMultipleDatesMenu: boolean = false
  public isRepeating: boolean = false
  private loading: boolean = false

  beforeMount () {
    if (this.initValues && this.initValues.RessourceIds.length) {
      this.ressourceIds.push(...this.initValues.RessourceIds)
      this.timeFrom = this.initValues.From
      this.dateFrom = this.initValues.Day
    }
  }

  private async sendAllocation (status: string): Promise<void> {
    if (this.isFormInvalid()) return
    this.saveAllocation(status)
    this.close()
  }

  public get RessourceChecker () : ShortAllocationView {
    let from, to
    let dates = null
    if (this.isRepeating) {
      from = 'T' + (this.fullday ? '00:00' : this.timeFrom)
      to = 'T' + (this.fullday ? '23:59' : this.timeTo)
      dates = this.multipleDates
    } else {
      from = this.dateFrom + 'T' + (this.fullday ? '00:00' : this.timeFrom)
      to = this.dateTo + 'T' + (this.fullday ? '23:59' : this.timeTo)
    }
    return { Id: 0, From: from, To: to, RessourceIds: this.ressourceIds, dates }
  }

  private async saveAllocation (status: string, date?: string): Promise<void> {
    let newAllocation: WebApi.MultipleAllocationsViewModel = {} as WebApi.MultipleAllocationsViewModel

    let newAllocationData: Partial<WebApi.MultipleAllocationsViewModel> = {
      // nAC.Status = status as unknown as WebApi.MeetingStatus
      Status: status as WebApi.MeetingStatus, // WebApi.MeetingStatus[status as WebApi.MeetingStatus],
      From: `${this.dateFrom}T${this.timeFrom}`,
      To: `${this.dateTo}T${this.timeTo}`,
      Title: this.title,
      Notes: this.notes,
      IsAllDay: this.fullday,
      GadgetsIds: [...this.selectedGadgets],
      ContactPhone: this.telNumber,
      ReferencePersonId: this.referencePerson.ActiveDirectoryID,
      Dates: [''],
      HintsForSuppliers: this.GetHintsForSuppliers,
      RessourceIds: this.ressourceIds
    }

    Object.assign(newAllocation, newAllocationData)

    var success : boolean = false
    this.loading = true
    if (!this.isRepeating) {
      success = await submitAllocation(newAllocation)
      if (success) this.$root.$emit('notify-user', { text: 'Speichern erfolgreich', color: 'success' } as ShowToast)
      else this.$root.$emit('notify-user', { text: 'Speichern fehlgeschlagen', color: 'error' } as ShowToast)
    } else {
      newAllocation.From = this.timeFrom
      newAllocation.To = this.timeTo
      newAllocation.Dates = this.multipleDates

      success = await submitAllocations(newAllocation)
    }
    if (success) {
      this.$root.$emit('notify-user', { text: 'Speichern erfolgreich', color: 'success' } as ShowToast)
      await refreshAllocations()
    } else {
      this.$root.$emit('notify-user', { text: 'Speichern fehlgeschlagen' })
    }
    this.loading = false
  }

  private close () {
    this.clearAll()
    this.$emit('close')
  }
  public get formInvalid (): boolean {
    let rValue = true
    if (!this.title) rValue = false
    if (!this.ressourceIds.length) rValue = false
    if (!this.dateFrom && !this.isRepeating) rValue = false
    if (!this.dateTo && !this.isRepeating && this.dateTo.length < 7) rValue = false
    if (this.isRepeating && !this.multipleDates.length) rValue = false
    if (this.dateTo === this.dateFrom && (this.timeFrom >= this.timeTo)) rValue = false
    return !rValue
  }

  private getGadgets (groupId: number): Array<Gadget> {
    return Gadget.query()
      .where('SuppliedBy', groupId)
      .get()
  }

  private removeDate (item: string): void {
    const idx = this.multipleDates.findIndex(v => v === item)
    this.multipleDates.splice(idx, 1)
  }

  private clearAll (): void {
    this.checkForm = false
    this.title = ''
    this.notes = ''
    this.telNumber = ''
    this.dateFrom = ''
    this.dateTo = ''
    this.timeTo = '17:00'
    this.timeFrom = '08:00'
    this.fullday = false
    this.ressourceIds.splice(this.ressourceIds.length)
    this.selectedGadgets = []
    this.multipleDates = []
    this.isRepeating = false
    this.referencePerson = { ActiveDirectoryID: '', Name: '', Email: '', Phone: '' }
    this.refreshInputReferencePerson++
    this.groupTextsInternal.splice(0, Infinity)
  }
}
</script>
