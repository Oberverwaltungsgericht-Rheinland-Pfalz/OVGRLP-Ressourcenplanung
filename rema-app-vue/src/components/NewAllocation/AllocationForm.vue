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
              label="Titel"
              required
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
              placeholder="Bitte wählen Sie einen Raum aus."
              label="Raum"
              :menu-props="{ offsetY: true }"
            />
          </v-col>
        </v-row>
        <v-divider />
        <v-row>
          <v-col cols="5">
            <!-- Datum von -->
            <date-time-picker
              label="von"
              v-model="dateFrom"
              :with-time="!fullday"
              placeholder="Bitte wählen Sie ein Datum aus."
            />
          </v-col>
          <v-col cols="5">
            <!-- Datum bis -->
            <date-time-picker
              label="bis"
              v-model="dateTo"
              :with-time="!fullday"
              placeholder="Bitte wählen Sie ein Datum aus."
            />
          </v-col>
          <v-col>
            <v-checkbox v-model="fullday" label="ganztägig"></v-checkbox>
          </v-col>
        </v-row>
        <v-divider />
        <v-row>
          <v-col v-for="(group, idx) in GadgetGroups" :key="idx + 'group'" cols="6">
            <v-select
              v-model="selectedGadgets"
              :items="getGadgets(group.Id)"
              :label="'Hilsmittel (' + group.Title + ')'"
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
      <v-btn color="red darken-1" text @click="close"
        ><v-icon>cancel</v-icon> Abbrechen</v-btn
      >
    </v-card-actions>
  </v-card>
</template>

<script lang="ts">
import Vue from 'vue'
import Component from 'vue-class-component'
import Gadget from '../../models/Gadget'
import Ressource from '../../models/Ressource'
import RessourceModel from '../../models/interfaces/RessourceModel'
import Supplier from '../../models/Supplier'
import Allocation from '../../models/Allocation'
import AllocationModel from '../../models/interfaces/AllocationModel'
import DateTimePicker from '@/components/DateTimePicker.vue'

@Component({
  components: {
    DateTimePicker
  }
})
export default class AllocationForm extends Vue {
  title: String = '';
  ressourceId: number | any = null;
  dateFrom: string = '';
  dateTo: string = '';
  fullday: boolean = false;
  notes: string = '';
  selectedGadgets: number[] = [];
  telNumber: string = '';
  contactPerson: string = '';

  // public isRepeating: boolean = false
  // public valid: boolean = false
  // public editId: number = 0
  // private Description: string = ''

  // private Email: string = ''
  // private timeFrom: string = '08:00'
  // private timeTo: string = '17:00'
  // private dateToIntern: Date = new Date(new Date().setHours(16, 0, 0, 0))
  // private dateToChanged: boolean = false
  // private multipleDates: string[] = []
  // private showMultipleDatesMenu: boolean = false

  private get permissionToEdit (): boolean {
    return this.$store.state.user.role >= 10
  }

  private async sendAllocation (status: number) {
    this.saveAllocation(status)

    // const purposeId = await this.savePurpose()
    // console.log('purposeid ' + purposeId)
    // this.saveAllocation(status, purposeId)
    /*
    if (this.isRepeating) {
      this.multipleDates.forEach(e => {
        this.saveAllocation(status, purposeId, e)
      })
    } else {
      this.saveAllocation(status, purposeId)
    }
    */
    this.close()
  }

  private async saveAllocation (
    status: number,
    date?: string
  ) {
    let newAllocation = {
      from: this.dateFrom,
      to: this.dateTo,
      title: this.title,
      notes: this.notes,
      isAllDay: this.fullday,
      ressourceId: this.ressourceId,
      gadgetsIds: [...this.selectedGadgets],
      contactName: this.contactPerson,
      contactPhone: this.telNumber
    }

    await Allocation.api().post('allocations', newAllocation)

    /*
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
    */
    /* TODO: save with date
    await Allocations.api().post('allocations', {
      From: from,
      To: to,
      IsAllDay: this.isWholeDay,
      Status: status,
      Purpose_id: purpose,
      Ressource_id: this.selectedRessourceId
    })
    */
  }

  private async savePurpose () {
    /*
    const response = await AllocationPurposes.api().post('allocationPurposes', {
      Title: this.Title,
      Description: this.Description,
      Notes: this.Notes,
      ContactPhone: this.telNumber,
      GadgetIds: [...this.seltedGadgets]
    })
    if (response && response !== null && response.entities !== null) {
      const entity = response.entities.AllocationPurposes.pop()
      if (entity === null) {
        return this.$dialog.message.warning(
          'AllocationPurpose was not saved correctly ' + response,
          { position: 'top-center' }
        )
      }
      return (entity as any).Id
    } else {
      this.$dialog.message.warning(
        'AllocationPurpose was not saved correctly ' + response,
        { position: 'top-center' }
      )
    }
    */
  }
  private close () {
    // this.clearAll()
    this.$emit('close')
  }
  public get formInvalid () {
    let rValue = true
    if (!this.ressourceId) rValue = false
    return !rValue
  }

  private get Rooms () {
    return Ressource.all()
  }
  private get GadgetGroups () {
    return Supplier.all()
  }

  // dateFrom
  /*
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
  */

  // dateTo
  /*
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
  */

  /*
  private get dateInputType (): string {
    if (this.isWholeDay) return 'date'
    else return 'datetime-local'
  }

  private get today () {
    if (this.isWholeDay) return dayjs().format('YYYY-MM-DD')
    else return dayjs().format('YYYY-MM-DDThh:mm')
  }
  */

  private getGadgets (groupId: number) {
    return Gadget.query()
      .where('SuppliedBy', groupId)
      .get()
  }

  /*
  private removeDate (item: string) {
    const idx = this.multipleDates.findIndex(v => v === item)
    this.multipleDates.splice(idx, 1)
  }
  */

  private clearAll () {
    this.title = ''
    this.notes = ''
    this.telNumber = ''
    this.contactPerson = ''
    this.dateFrom = ''
    this.dateTo = ''
    this.fullday = false
    this.ressourceId = null
    this.selectedGadgets = []
  }
}
</script>

<style lang="stylus">
</style>
