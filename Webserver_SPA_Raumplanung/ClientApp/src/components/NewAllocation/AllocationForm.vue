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
  public valid: boolean = false
  private Title: string = ''
  private Description: string = ''
  private Email: string = ''
  private dateFromIntern: Date = new Date(new Date().setHours(8,0,0,0))
  private dateToIntern: Date = new Date(new Date().setHours(16,0,0,0))
  private dateToChanged: boolean = false
  private selectedRessource: number = 0
  private telNumber: string = ''
  private contactPerson: string = ''
  private pickerFrom: boolean = false
  private pickerTo: boolean = false

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
}
</script>
