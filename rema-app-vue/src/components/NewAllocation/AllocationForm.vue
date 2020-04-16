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
            <v-text-field
              v-model="title"
              label="Titel *"
              required
              :error="!title && checkForm"
              placeholder="Bitte geben Sie einen Titel für den Termin an."
            ></v-text-field>
          </v-col>
          <v-col>
            <v-select
              v-model="ressourceId"
              :items="Rooms"
              item-text="Name"
              item-value="Id"
              clearable
              :error="!ressourceId && checkForm"
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
                  <v-date-picker v-model="multipleDates" locale="de" multiple no-title scrollable>
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
            <v-menu ref="fromMenu" :close-on-content-click="true" transition="scale-transition" offset-y max-width="290px" min-width="290px"
            v-model="fromMenu">
            <template v-slot:activator="{ on }">
              <v-text-field persistent-hint prepend-icon="event" v-on="on" readonly
              label="Von *"
              :error="!dateFrom && checkForm"
              :value="dateFormatted(dateFrom)"
              ></v-text-field>
            </template>
            <v-date-picker v-model="dateFrom" :min="dateMin" locale="de" no-title @input="fromMenu = false">
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
            <v-date-picker v-model="dateTo" :min="dateToMin" locale="de" no-title @input="toMenu = false">
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
              clearable
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
              auto-grow
              clearable
              outlined
              rounded
            ></v-textarea>
          </v-col>
        </v-row>
      </v-container>
    </v-card-text>
    <v-card-actions>
      <div class="flex-grow-1"></div>
      <v-btn
        v-if="permissionToEdit"
        color="green darken-1"
        text
        @click="sendAllocation(1)"
        ><v-icon>save</v-icon> Speichern</v-btn
      >
      <v-btn
        v-else
        color="green darken-1"
        text
        @click="sendAllocation(0)"
        ><v-icon>save</v-icon> Anfragen</v-btn
      >
      <v-btn color="red darken-1" text @click="close"
        ><v-icon>cancel</v-icon> Abbrechen</v-btn
      >
    </v-card-actions>
  </v-card>
</template>

<script lang="ts">
import { Component, Prop, Vue, Watch, Mixins } from 'vue-property-decorator'
import { Gadget, Ressource, Supplier, Allocation } from '../../models'
import DropDownTimePicker from '@/components/DropdownTimePicker.vue'
import { RessourceModel, AllocationModel, AdUsers, HintsForSuppliers } from '../../models/interfaces'
import InputReferencePerson from '@/components/NewAllocation/InputReferencePerson.vue'
import AllocationFormService from '../../services/AllocationFormServices'
import moment from 'moment'

@Component({
  components: {
    DropDownTimePicker, InputReferencePerson
  }
})
export default class AllocationForm extends Mixins(AllocationFormService) {
  public title: String = ''
  public ressourceId: number | any = null
  public fullday: boolean = false
  public notes: string = ''
  public selectedGadgets: number[] = []
  private multipleDates: string[] = []
  private showMultipleDatesMenu: boolean = false
  public isRepeating: boolean = false

  private get permissionToEdit (): boolean {
    return this.$store.state.user.role >= 10
  }

  private async sendAllocation (status: number) {
    if (this.isFormInvalid()) return
    this.saveAllocation(status)
    this.close()
  }

  private async saveAllocation (
    status: number,
    date?: string
  ) {
    let newAllocation = {
      status: status,
      from: this.dateFrom,
      to: this.dateTo,
      title: this.title,
      notes: this.notes,
      isAllDay: this.fullday,
      ressourceId: this.ressourceId,
      gadgetsIds: [...this.selectedGadgets],
      contactPhone: this.telNumber,
      ReferencePersonId: this.referencePerson.ActiveDirectoryID,
      dates: [''],
      HintsForSuppliers: this.GetHintsForSuppliers
    }

    if (!this.isRepeating) {
      await Allocation.api().post('allocations', newAllocation)
    } else {
      newAllocation.from = this.timeFrom
      newAllocation.to = this.timeTo
      newAllocation.dates = this.multipleDates

      const response = await fetch(`/api/Allocations/PostAllocations`, {
        method: 'POST', // *GET, POST, PUT, DELETE, etc.
        mode: 'cors', // no-cors, cors, *same-origin
        cache: 'no-cache', // *default, no-cache, reload, force-cache, only-if-cached
        credentials: 'same-origin', // include, *same-origin, omit
        headers: {
          'Content-Type': 'application/json'
        },
        body: JSON.stringify(newAllocation)
      }).catch(() => this.$dialog.error({ text: 'Speichern fehlgeschlagen', title: 'Warning' }))
      await Allocation.api().get('allocations')
    }
  }

  private close () {
    this.clearAll()
    this.$emit('close')
  }
  public get formInvalid () {
    let rValue = true
    if (!this.ressourceId) rValue = false
    if (!this.dateFrom && !this.isRepeating) rValue = false
    if (!this.dateTo && !this.isRepeating && this.dateTo.length < 7) rValue = false
    if (this.isRepeating && !this.multipleDates.length) rValue = false
    if (this.dateTo === this.dateFrom && (this.timeFrom >= this.timeTo)) rValue = false
    return !rValue
  }

  private getGadgets (groupId: number) {
    return Gadget.query()
      .where('SuppliedBy', groupId)
      .get()
  }

  private removeDate (item: string) {
    const idx = this.multipleDates.findIndex(v => v === item)
    this.multipleDates.splice(idx, 1)
  }

  private clearAll () {
    this.checkForm = false
    this.title = ''
    this.notes = ''
    this.telNumber = ''
    this.dateFrom = ''
    this.dateTo = ''
    this.timeTo = '17:00'
    this.timeFrom = '08:00'
    this.fullday = false
    this.ressourceId = null
    this.selectedGadgets = []
    this.multipleDates = []
    this.isRepeating = false
    this.referencePerson = { ActiveDirectoryID: '', Name: '', Email: '', Phone: '' }
    this.refreshInputReferencePerson++
    this.groupTextsInternal.splice(0, Infinity)
  }
}
</script>
