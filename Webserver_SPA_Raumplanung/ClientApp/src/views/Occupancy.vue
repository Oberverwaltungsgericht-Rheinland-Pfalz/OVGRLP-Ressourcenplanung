<template>
    <div>
        <h1>Belegung</h1>
        <scheduler/>
    </div>
</template>

<script lang="ts">
import { Component, Vue } from 'vue-property-decorator'
import Scheduler from '@/components/RessourceScheduler.vue'

@Component({
  components: { Scheduler }
})
export default class Occupancy extends Vue {
}
</script>
