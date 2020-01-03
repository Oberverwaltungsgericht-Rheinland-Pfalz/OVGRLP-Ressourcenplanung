<template>
  <v-card>
    <v-card-title v-if="permissionToEdit" class="headline">Termin eintragen</v-card-title>
    <v-card-title v-else class="headline">Neue Terminanfrage stellen</v-card-title>
    <v-card-text>
      <v-container>
        <v-text-field
            v-model="Title"
            label="Titel"
            required
        ></v-text-field>
        <v-row>
          <v-col cols="2">
            <v-switch style="margin:0"
              v-model="isWholeDay"
              :label="`Ganztägiges Ereignis`"
            ></v-switch>
          </v-col>
          <v-col v-if="!isWholeDay && isRepeating" cols="6" id="repeatingTime">
            Jeweils von: <input v-model="timeFrom" type="time"/>
            bis: <input v-model="timeTo" type="time"/>
          </v-col>
        </v-row>
        <v-switch style="margin:0"
          v-model="isRepeating"
          :label="`Wiederkehrender Termin`"
        ></v-switch>

        <div v-show="!isRepeating">
          <label for="meeting-From">Vom:
          <input :value="dateFrom" @input="dateFrom = $event.target.value"
            :type="dateInputType" step="900"
            id="meeting-from" 
            name="meeting-time" :min="today"></label>

          <label>Bis:
          <input :value="dateTo" @input="dateTo = $event.target.value"
            :type="dateInputType" @change="dateToChanged = true"
            name="meeting-time" id="meeting-to" step="900"
            :min="today"></label>
        </div>

        <div v-show="isRepeating">
          <v-menu
            ref="showMultipleDatesMenu"
            v-model="showMultipleDatesMenu"
            :close-on-content-click="false"
            :return-value.sync="multipleDates"
            transition="scale-transition"
            offset-y
            min-width="290px"
          >
            <template v-slot:activator="{ on }">
              <v-combobox
                v-model="multipleDates"
                multiple
                chips
                small-chips
                label="Gewählte Termine"
                prepend-icon="event"
                readonly
                v-on="on"
              >
              <template v-slot:selection="data">
                  <v-chip>{{data.item | toLocalDate}} <v-icon right @click="removeDate(data.item)">close</v-icon></v-chip>
              </template>
              </v-combobox>
            </template>
            <v-date-picker v-model="multipleDates" locale="de" multiple no-title scrollable>
              <v-spacer></v-spacer>
              <v-btn text color="primary" @click="showMultipleDatesMenu = false">Cancel</v-btn>
              <v-btn text color="primary" @click="$refs.showMultipleDatesMenu.save(multipleDates)">OK</v-btn>
            </v-date-picker>
          </v-menu>
        </div>

        <v-select
            v-model="selectedRessourceId"
            :items="Rooms"
            item-text="Name"
            item-value="Id"
            label="Raum"
        />

        <div v-for="(group, idx) in GadgetGroups" :key="idx+'group'">
          <v-select
            v-model="seltedGadgets"
            :items="getGadgets(group.Id)"
            :label="'Hilfsmittel der '+ group.Title"
            item-text="Title"
            item-value="Id"
            multiple chips persistent-hint 
          />
        </div>
        
        <!-- Todo: Autocomplete mit Namenseingabe und GET Webservice, welcher von AD gespeist wird
        <v-text-field
        v-model="contactPerson"
        label="Ansprechpartner/In"
        required
        ></v-text-field>
        <v-text-field
            v-model="telNumber"
            type="tel"
            label="Telefonnummer"
            required
        ></v-text-field>
        -->
        <v-textarea
            v-model="Description"
            :label="'Beschreibung'"
            auto-grow
            clearable
            outlined
            rounded
        ></v-textarea>

        <v-textarea
            v-model="Notes"
            :label="'Notizen'"
            auto-grow
            clearable
            outlined
            rounded
        ></v-textarea>
      </v-container>
    </v-card-text>
    <v-card-actions>
        <div class="flex-grow-1"></div>
        <v-btn v-if="permissionToEdit" :disabled="formInvalid" color="green darken-1" text @click="sendAllocation(1)"><v-icon>save</v-icon> Speichern</v-btn>
        <v-btn v-else :disabled="formInvalid" color="green darken-1" text @click="sendAllocation(0)"><v-icon>save</v-icon> Anfragen</v-btn>
        <v-btn color="red darken-1" text @click="close"><v-icon>cancel</v-icon> Abbrechen</v-btn>
    </v-card-actions>
  </v-card>
</template>

<script lang="ts">
import Vue from 'vue'
import Component from 'vue-class-component'
import Gadgets from '../../models/GadgetModel'
import Ressources, { RessourceModel } from '../../models/RessourceModel'
import Suppliers from '../../models/SupplierModel'
import AllocationPurposes, { AllocationPurposeModel } from '../../models/AllocationpurposeModel'
import Allocations, { AllocationModel } from '../../models/AllocationModel'
import dayjs from 'dayjs'

@Component
export default class AllocationForm extends Vue {
  public isRepeating: boolean = false
  public valid: boolean = false
  public editId: number = 0
  public isWholeDay: boolean = true
  private Title: string = ''
  private Description: string = ''
  private Notes: string = ''
  private Email: string = ''
  private timeFrom: string = '08:00'
  private timeTo: string = '17:00'
  private dateFromIntern: Date = new Date(new Date().setHours(8,0,0,0))
  private dateToIntern: Date = new Date(new Date().setHours(16,0,0,0))
  private dateToChanged: boolean = false
  private selectedRessourceId: number = 0
  private seltedGadgets: number[] = []
  private telNumber: string = ''
  private contactPerson: number = 0
  private multipleDates: string[] = []
  private showMultipleDatesMenu: boolean = false

  private get permissionToEdit (): boolean {
    return this.$store.state.user.role >= 10
  }

  private async sendAllocation (status: number) {

    const purposeId = await this.savePurpose()
    console.log('purposeid ' + purposeId)
    if (this.isRepeating) {
      this.multipleDates.forEach((e) => {
        this.saveAllocation(status, purposeId, e)
      })
    } else {
      this.saveAllocation(status, purposeId)
    }
    this.close()
  }

  private async saveAllocation (status: number, purpose: AllocationPurposeModel, date?: string) {
    let from = this.dateFrom
    let to = this.dateTo
    if (date) {
      from = date
      to = date

      if (!this.isWholeDay) {
        from = `${date}T${this.timeFrom}`
        to = `${date}T${this.timeTo}`
      }
    }

    await Allocations.api().post('allocations', {
      From: from, To: to, IsAllDay: this.isWholeDay, Status: status,
      Purpose_id: purpose, Ressource_id: this.selectedRessourceId }
    )
  }

  private async savePurpose () {
    const response = await AllocationPurposes.api().post('allocationPurposes', {
      Title: this.Title,
      Description: this.Description,
      Notes: this.Notes,
      ContactPhone: this.telNumber,
      GadgetIds: [...this.seltedGadgets] }
    )
    if (response && response !== null && response.entities !== null) {
      const entity = response.entities.AllocationPurposes.pop()
      if (entity === null) {
        return this.$dialog.message.warning(
          'AllocationPurpose was not saved correctly ' + response, { position: 'top-center' }
          )
      }
      return (entity as any).Id
    } else this.$dialog.message.warning('AllocationPurpose was not saved correctly ' + response, { position: 'top-center' })
  }
  private close () {
    this.clearAll()
    this.$emit('close')
  }
  public get formInvalid () {
    let rValue = true
    if (!this.selectedRessourceId) rValue = false
    return !rValue
  }

  private get Rooms () {
    return Ressources.all()
  }
  private get GadgetGroups () {
    return Suppliers.all()
  }

  // dateFrom
  private get dateFrom (): string {
    if (this.isWholeDay) {
      return dayjs(this.dateFromIntern).format('YYYY-MM-DD')
    } else {
      return dayjs(this.dateFromIntern).format('YYYY-MM-DDThh:mm')
    }
  }
  private set dateFrom (val: string) {
    this.dateFromIntern = new Date(val)
    if (!this.dateToChanged) this.dateTo = val
  }

  // dateTo
  private get dateTo (): string {
    if (this.isWholeDay) {
      return dayjs(this.dateToIntern).format('YYYY-MM-DD')
    } else {
      return dayjs(this.dateToIntern).format('YYYY-MM-DDThh:mm')
    }
  }
  private set dateTo (val: string) {
    this.dateToIntern = new Date(val)
  }

  private get dateInputType (): string {
    if (this.isWholeDay) return 'date'
    else return 'datetime-local'
  }

  private get today () {
    if (this.isWholeDay) return dayjs().format('YYYY-MM-DD')
    else return dayjs().format('YYYY-MM-DDThh:mm')
  }

  private getGadgets (groupId: number) {
    return Gadgets.query().where('SuppliedBy', groupId).get()
  }

  private removeDate (item: string) {
    const idx = this.multipleDates.findIndex((v) => v === item)
    this.multipleDates.splice(idx, 1)
  }
  private clearAll () {
    this.Title = ''
    this.Description = ''
    this.Notes = ''
    this.telNumber = ''
    this.contactPerson = 0
    this.multipleDates = []
    this.isRepeating = false
    this.dateFromIntern = new Date(new Date().setHours(8,0,0,0))
    this.dateToIntern = new Date(new Date().setHours(16,0,0,0))
    this.timeFrom = '08:00'
    this.timeTo = '17:00'
    this.selectedRessourceId = 0
  }
}
</script>

<style lang="stylus">
#repeatingTime input[type="time"]
  border-bottom 1px solid darkgray
</style>