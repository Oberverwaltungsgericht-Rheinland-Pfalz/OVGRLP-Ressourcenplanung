<template>
  <div id="ressources-admin">
    <v-tabs v-model="tab" fixed-tabs background-color="white" light>
      <v-tab title="Ressourcen verwalten">Ressourcen</v-tab>
      <v-tab title="Hilfsmittel verwalten">Hilfsmittel</v-tab>
      <v-tab title="Unterstützergruppen verwalten">Unterstützergruppen</v-tab>
    </v-tabs>

    <ressource-management v-show="tab===0" />
    <gadget-management v-show="tab===1" />
    <supplier-management v-show="tab===2" />
  </div>
</template>

<script lang="ts">
import { Component, Vue } from 'vue-property-decorator'
import RessourceManagement from '@/components/RessourceManagement/RessourceMngt.vue'
import GadgetManagement from '@/components/RessourceManagement/GadgetManagement.vue'
import SupplierManagement from '@/components/RessourceManagement/SupplierManagement.vue'

@Component({
  components: { RessourceManagement, GadgetManagement, SupplierManagement }
})
export default class Ressources extends Vue {
  public tab: number = 0
}
</script>

<style lang="stylus">
#ressources-admin
  tr td:last-of-type
    white-space nowrap
    width 1%
</style>
