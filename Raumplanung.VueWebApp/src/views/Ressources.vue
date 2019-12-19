<template>
  <div> 
      <v-tabs v-model="tab" fixed-tabs background-color="white" light>
        <v-tab>
          Ressourcen
        </v-tab>
        <v-tab>
          Hilfsmittel
        </v-tab>
        <v-tab>
          Unterstützergruppen
        </v-tab>
      </v-tabs>

      <ressource-management v-show="tab===0"/>     
      <gadget-management v-show="tab===1"/>
      <supplier-management v-show="tab===2"/>
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
  public tab: any = null

}
</script>
