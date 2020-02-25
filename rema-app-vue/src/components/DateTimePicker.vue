<template>
  <div>
    <v-row dense>
      <v-col>
        <v-dialog
          ref="dialog"
          v-model="modal"
          persistent
          width="290px"
        >
          <template v-slot:activator="{ on }">
            <v-text-field
              :value="formatedDate"
              prepend-icon="event"
              :placeholder="placeholder"
              :label="label"
              readonly
              clearable
              @click:clear="date = null"
              v-on="on"
            ></v-text-field>
          </template>
          <v-date-picker
            v-model="date"
            scrollable
            @change="modal = false" />
        </v-dialog>
      </v-col>
      <v-col cols="3" v-if="withTime">
        <v-text-field v-model="time" type="time"></v-text-field>
      </v-col>
    </v-row>
    <v-row>
      <span v-if="debug">
        {{ value }}
      </span>
    </v-row>
  </div>
</template>

<script lang="ts">
import Vue from 'vue'
import { Component, Prop, Watch } from 'vue-property-decorator'
var moment = require('moment')

@Component
export default class DateTimePicker extends Vue {
  @Prop() placeholder!: String;
  @Prop() label!: string;
  @Prop(Boolean) debug!: Boolean;
  @Prop(Boolean) withTime!: Boolean;
  @Prop({ default: '08:00' }) defaultTime!: string;
  @Prop({ default: '00:00' }) defaultTimeFullDay!: string

  @Prop({ default: 'L' }) dateFormat!: string;
  @Prop({ default: '' }) value!: string;

  private date: string = ''
  private time: string = this.defaultTime

  modal: Boolean = false;

  @Watch('withTime')
  public withTimeChange (val: Boolean) {
    this.$emit('input', this.getValue())
  }

  @Watch('value')
  public valueChange (val: string) {
    let datePart = ''
    let timePart = this.defaultTime

    if (val && val.length > 0 && val.indexOf('T')) {
      datePart = val.split('T')[0]
      timePart = val.split('T')[1]
    }

    this.date = datePart
    this.time = timePart
  }

  @Watch('date')
  public dateChange (val: string) {
    this.$emit('input', this.getValue())
  }

  @Watch('time')
  public timeChange (val: string) {
    this.$emit('input', this.getValue())
  }

  private getValue () : string {
    let newValue = ''

    if (this.date && this.date.length > 0) {
      if (!this.withTime) {
        newValue = this.date + 'T' + this.defaultTimeFullDay
      } else if (this.time && this.time.length > 0) {
        newValue = this.date + 'T' + this.time
      }
    }

    return newValue
  }

  get formatedDate (): string {
    return this.date ? moment(this.date).format(this.dateFormat) : ''
  }
}
</script>
