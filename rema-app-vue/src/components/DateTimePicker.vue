<template>
  <div>
    <v-row dense>
      <v-col>
        <v-dialog
          ref="dialog"
          v-model="modal"
          :return-value.sync="date"
          persistent
          width="290px"
        >
          <template v-slot:activator="{ on }">
            <v-text-field
              :value="date | moment(dateFormat)"
              prepend-icon="event"
              :placeholder="placeholder"
              :label="label"
              readonly
              v-on="on"
            ></v-text-field>
          </template>
          <v-date-picker
            v-model="date"
            scrollable
            @dblclick:date="$refs.dialog.save(date)"
          >
            <v-btn text color="primary" @click="modal = false">Abbrechen</v-btn>
            <v-btn text color="primary" @click="$refs.dialog.save(date)"
              >OK</v-btn
            >
          </v-date-picker>
        </v-dialog>
      </v-col>
      <v-col cols="3" v-if="withTime">
        <v-text-field v-model="time" type="time"></v-text-field>
      </v-col>
    </v-row>
    <v-row>
      <span v-if="debug">
        {{ value }}
      </span></v-row
    >
  </div>
</template>

<script lang="ts">
import Vue from 'vue'
import { Component, Prop } from 'vue-property-decorator'

@Component
export default class DateTimePicker extends Vue {
  @Prop() placeholder!: String;
  @Prop() label!: string;
  @Prop(Boolean) debug!: Boolean;
  @Prop(Boolean) withTime!: Boolean;

  @Prop({ default: 'L' }) dateFormat!: string;
  @Prop({ default: new Date(Date.now()).toISOString() }) value!: string;

  private timeOffset: string = '';

  modal: Boolean = false;

  get date (): string {
    if (this.value.indexOf('T') > 0) {
      return this.value.split('T')[0]
    } else {
      return this.value
    }
  }

  set date (isoDatePart: string) {
    let timePart = this.value.split('T')[1]
    let newValue = isoDatePart + 'T' + timePart
    this.$emit('input', newValue)
  }

  get time (): string {
    if (this.value.indexOf('T') > 0) {
      this.timeOffset = this.value.split('T')[1].substring(12)
      return this.value.split('T')[1].substring(0, 5)
    } else {
      return this.value
    }
  }
  set time (isoTimePart: string) {
    let newValue = this.date + 'T' + isoTimePart + ':00.000' + this.timeOffset
    this.$emit('input', newValue)
  }
}
</script>
