<template>
    <v-slide-y-transition mode="out-in">
      <v-layout column>
        <h1>Mehrfachauswahl</h1>
        <p>An welchen Tagen möchten sie diesen Termin eintragen?</p>
        <multi-dates-date/>
       
        <multi-dates/>
      </v-layout>
    </v-slide-y-transition>
</template>

<script lang="ts">
import { Component, Vue } from 'vue-property-decorator'
import MultiDates from '@/components/NewAllocation/MultiDates.vue'
import MultiDatesDate from '@/components/NewAllocation/MultiDatesDate.vue'

@Component({
  components: { MultiDates, MultiDatesDate }
})
export default class MultiDatesView extends Vue {
  private date: any = { title: 'Bezeichnung', creator: 'createdby' }
}
</script>


