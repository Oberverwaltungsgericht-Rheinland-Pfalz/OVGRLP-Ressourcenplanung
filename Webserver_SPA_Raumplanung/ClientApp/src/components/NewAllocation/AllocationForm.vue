<template>
 <v-container>
    <v-text-field
        v-model="Title"
        :counter="15"
        label="Title"
        required
    ></v-text-field>

    <v-switch
      v-model="isWholeDay"
      :label="`Ganztägiges Ereignis`"
    ></v-switch>
    <v-switch
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
      full-width
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
      <v-date-picker v-model="multipleDates" multiple no-title scrollable>
        <v-spacer></v-spacer>
        <v-btn text color="primary" @click="showMultipleDatesMenu = false">Cancel</v-btn>
        <v-btn text color="primary" @click="$refs.showMultipleDatesMenu.save(multipleDates)">OK</v-btn>
      </v-date-picker>
    </v-menu>
  </div>

    <v-select
        v-model="selectedRessourceId"
        :items="Rooms"
        item-text="Title"
        item-value="id"
        label="Raum"
    />

    <div v-for="(group, idx) in GadgetGroups" :key="idx+'group'">
      <v-select
        v-model="seltedGadgets"
        :items="getGadgets(group.id)"
        :label="'Hilfsmittel der '+ group.Title"
        item-text="Title"
        item-value="id"
        multiple chips persistent-hint
      />
    </div>    

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
</template>

<script lang="ts">
import Vue from 'vue'
import Component from 'vue-class-component'
import Gadgets from '../../models/GadgetModel'
import Ressources from '../../models/RessourceModel'
import Suppliers from '../../models/SupplierModel'
import dayjs from 'dayjs'

@Component
export default class AllocationForm extends Vue {
  public isWholeDay: boolean = true
  public isRepeating: boolean = false
  public valid: boolean = false
  private Title: string = ''
  private Description: string = ''
  private Notes: string = ''
  private Email: string = ''
  private dateFromIntern: Date = new Date(new Date().setHours(8,0,0,0))
  private dateToIntern: Date = new Date(new Date().setHours(16,0,0,0))
  private dateToChanged: boolean = false
  private selectedRessourceId: number = 0
  private seltedGadgets: number[] = []
  private telNumber: string = ''
  private contactPerson: string = ''
  private multipleDates: string[] = []
  private showMultipleDatesMenu: boolean = false

  private get Rooms () {
    return Ressources.query().where('Title', (v: string) => v.length).get()
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
    return Gadgets.query().where('suppliedBy', groupId).get()
  }

  private removeDate (item: string) {
    const idx = this.multipleDates.findIndex((v) => v === item)
    this.multipleDates.splice(idx, 1)
  }
}
</script>
