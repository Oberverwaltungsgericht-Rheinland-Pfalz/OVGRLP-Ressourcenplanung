<template>
  <span class="ddtp-border">
    <span class="ddtp-inner-border">
      <select v-model="hour">
        <option v-for="n in hours" :key="n+'t1'" :value="n">{{n | toTwo}}</option>
      </select>

      <strong>:&ensp;</strong>

      <select v-model="minute" v-show="precise">
        <option v-for="n in minutes" :key="n+'m1'" :value="n">{{n | toTwo}}</option>
      </select>
      <select v-model="minute" v-show="!precise">
        <option v-for="n of quarters" :key="n+'q1'" :value="n">{{n | toTwo}}</option>
      </select>
    </span>

    <label>&emsp;
    <input type="checkbox" v-model="precise"/>&ensp;Genau &ensp;</label>
  </span>
</template>

<script lang="ts">
import Vue from 'vue'
import { Component, Prop } from 'vue-property-decorator'

@Component({
  filters: {
    toTwo (nr: number) {
      return (nr > 9 ? nr : '0' + nr)
    }
  }
})
export default class DropDownTimePicker extends Vue {
  @Prop({ default: '00:00' }) value!: string;

  private precise: boolean = false
  private hourInternal: number = 0
  private minuteInternal: number = 0
  private hours = Array.from(Array(24).keys())
  private minutes = Array.from(Array(60).keys())
  private quarters = [0, 15, 30, 45]
  // private minutes = ['00', '01', '02', '03', '04', '05', '06', '07', '08', '09', '10', '11', '12', '13', '14', '15', '16', '17', '18', '19', '20', '21', '22', '23', '24', '25', '26', '27', '28', '29', '30', '31', '32', '33', '34', '35', '36', '37', '38', '39', '40', '41', '42', '43', '44', '45', '46', '47', '48', '49', '50', '51', '52', '53', '54', '55', '56', '57', '58', '59']

  private mounted () {
    this.hourInternal = parseInt(!this.value ? '0' : this.value.substring(0, 2))
    this.minuteInternal = parseInt(!this.value ? '0' : this.value.substring(3, 5))
  }

  get outputValue () :string {
    return (this.hourInternal > 9 ? this.hourInternal : '0' + this.hourInternal) +
    ':' +
    (this.minuteInternal > 9 ? this.minuteInternal : '0' + this.minuteInternal)
  }

  get hour (): number {
    return this.hourInternal
  }
  set hour (h: number) {
    this.hourInternal = h
    this.$emit('input', this.outputValue)
  }
  get minute (): number {
    return this.minuteInternal
  }
  set minute (h: number) {
    this.minuteInternal = h
    this.$emit('input', this.outputValue)
  }
}
</script>

<style lang="stylus" scoped>
.ddtp-inner-border
  border-bottom 1px solid darkgrey
  padding 3px
</style>
