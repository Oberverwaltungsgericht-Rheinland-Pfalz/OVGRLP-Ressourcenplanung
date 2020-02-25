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
              v-on="on"
            ></v-text-field>
          </template>
          <v-date-picker
            :value="date"
            @change="modal=false"
            @input="dateChanged"
            scrollable />
        </v-dialog>
      </v-col>
      <v-col cols="3" v-if="withTime">
        <v-text-field :value="time" @input="timeChanged" type="time"></v-text-field>
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

  private internalValue = ''
  private lastTime: string = ''

  modal: Boolean = false;

  get date () {
    if (this.internalValue.indexOf('T') > 0) {
      return this.internalValue.split('T')[0]
    } else {
      return ''
    }
  }
  dateChanged (val: string) {
    this.updateValue(val, this.time)
  }

  get time () {
    if (this.internalValue.indexOf('T') > 0) {
      return this.internalValue.split('T')[1]
    } else {
      return this.defaultTime
    }
  }
  timeChanged (val: string) {
    this.updateValue(this.date, val)
  }

  updateValue (date: string, time: string) {
    this.internalValue = `${date}T${time}`

    const newValue = `${date}T${this.withTime ? time : this.defaultTimeFullDay}`
    this.$emit('input', newValue)
  }

  @Watch('value')
  public valueChanged (val: string) {
    this.internalValue = val
  }

  @Watch('withTime')
  public withTimeChange (val: Boolean) {
    if (val) {
      this.updateValue(this.date, this.lastTime)
    } else {
      this.lastTime = this.time
      this.updateValue(this.date, this.defaultTimeFullDay)
    }
  }

  get formatedDate (): string {
    return this.date ? moment(this.date).format(this.dateFormat) : ''
  }
}
</script>
