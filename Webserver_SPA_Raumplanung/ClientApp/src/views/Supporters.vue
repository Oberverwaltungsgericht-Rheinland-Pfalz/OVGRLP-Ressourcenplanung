<template>
  <div>
      <h1>Bald bereit zu stellen von Ihrer Abteilung:</h1>
      <div>
        <gadget-list/>
      </div>
  </div>
</template>

<script lang="ts">
import { Component, Vue } from 'vue-property-decorator'
import GadgetList from '@/components/GadgetList.vue'

@Component({
  components: { GadgetList }
})
export default class Supporters extends Vue {}
</script>


