<template>
  <v-layout column>
    <v-row class="text-center">
      <v-col cols="2">
        <v-text-field type="text" placeholder="Namensfilter" v-model="nameFilter"/>
      </v-col>
      <v-col cols="1">
        <v-btn v-if="hideEmptyRessources" @click="hideEmptyRessources = false" title="Freie Ressourcen anzeigen"><v-icon>blur_circular</v-icon></v-btn>
        <v-btn v-else @click="hideEmptyRessources = true" title="Freie Ressourcen ausblenden"><v-icon>blur_linear</v-icon></v-btn>
      </v-col>
      <v-col cols="6">
        <v-btn @click="jumpDays(-7)" fab text small><v-icon>first_page</v-icon></v-btn>
        <v-btn @click="jumpDays(-1)" fab text small><v-icon>arrow_back_ios</v-icon></v-btn>
        <v-menu ref="todayMenu" :close-on-content-click="true" transition="scale-transition" offset-y max-width="290px" min-width="290px"
          v-model="daypickerOpen">
          <template v-slot:activator="{ on }">
            <v-btn v-if="dayOrWeek" outlined v-on="on">{{today | toLocalDate}}</v-btn>
            <v-btn v-else outlined v-on="on">{{todayPlus(0) | toLocalDate}} - {{todayPlus(4) | toLocalDate}}</v-btn>
          </template>
          <v-date-picker v-model="pickedDate" locale="de" :first-day-of-week="1" no-title @input="daypickerOpen = false">
            <v-btn text color="primary" @click="daypickerOpen = false" block>Abbrechen</v-btn>
          </v-date-picker>
        </v-menu>

      <v-btn @click="jumpDays(1)" fab text small><v-icon>arrow_forward_ios</v-icon></v-btn>
      <v-btn @click="jumpDays(7)" fab text small><v-icon>last_page</v-icon></v-btn></v-col>
      <v-col cols="3" class="items-right">
        <v-btn @click="dayOrWeek = Boolean(dayOrWeek^=1)" :title="dayOrWeek ? 'Wochenansicht' : 'Tagesansicht'">
          <v-icon v-if="!dayOrWeek" >date_range</v-icon>
          <v-icon v-else>today</v-icon>
        </v-btn>

        <v-btn @click="hideLateEarly = Boolean(hideLateEarly^=1)" class="mar-right" :title="hideLateEarly ? 'Alle Stunden anzeigen' : 'Nur Arbeitszeit'">
          <v-icon v-if="!hideLateEarly" >visibility_off</v-icon>
          <v-icon v-else>visibility</v-icon>
        </v-btn>
      </v-col>
    </v-row>
    <h24-table
      v-if="dayOrWeek" :Day="today"
        :HideLateEarly="hideLateEarly"
        :NameFilter="nameFilter"
        :HideEmptyRessources="hideEmptyRessources"
    />
    <div v-else :class="{'day': dayOrWeek, 'week': !dayOrWeek}">
      <h24-table
        v-for="idx in 5" :key="'dayScheduler'+idx"
        :HideLateEarly="hideLateEarly"
        :NameFilter="nameFilter"
        :HideEmptyRessources="hideEmptyRessources"
        :Day="todayPlus(idx-1)"
      />
    </div>
  </v-layout>
</template>

<script lang="ts">
import { Component, Vue } from 'vue-property-decorator'
import { Allocation, Ressource } from '../../models'
import { ScheduledRessource } from '../../models/interfaces'
import { State } from 'vuex-class'
import h24Table from './h24Table.vue'
import moment from 'moment'

@Component({
  components: { h24Table }
})
export default class SchedulerOverview extends Vue {
  private dayOrWeek: boolean = true
  private hideLateEarly: boolean = true
  private nameFilter: string = ''
  private hideEmptyRessources: boolean = false

  private daypickerOpen: boolean = false
  private get allocationPossible () {
    return moment(moment().add(-1, 'days')).isBefore(this.today) &&
      // @ts-ignore
      (this.$store.state.user.isRequestable || this.permissionToEdit)
  }
  private today: string = moment().format('YYYY-MM-DD')
  private get pickedDate (): string {
    return this.today
  }
  private todayPlus (addition: number) {
    return moment(this.today).add(addition, 'days').format('YYYY-MM-DD')
  }
  private set pickedDate (v: string) {
    this.today = moment(v).format('YYYY-MM-DD')
  }
  private jumpDays (days: number) {
    this.today = moment(this.today).add(days, 'd').format('YYYY-MM-DD')
  }
}
</script>

<style lang="stylus" scoped>
.week
  display flex
.mar-right
  margin-right 1em
.items-right > button
  float right
</style>
