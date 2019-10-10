<template>
  <div>
    <!--<h1>Bitte bearbeiten Sie die eingegangenen Terminanfragen</h1>-->
    <acknowledge-list/>
  </div>
</template>

<script lang="ts">
import { Component, Vue } from 'vue-property-decorator'
import AcknowledgeList from '@/components/AcknowledgeList.vue'

@Component({
  components: { AcknowledgeList }
})
export default class Acknowledge extends Vue {}
</script>


