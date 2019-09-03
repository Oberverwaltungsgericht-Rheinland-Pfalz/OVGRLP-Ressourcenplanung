<template>
  <div>
      <h1>Ressourcenverwaltung</h1>
      <v-spacer/>
      <ressource-management/>
      
      <br/>     
      <gadget-management/>
      <br/>
      <supplier-management/>

      <v-layout align-space-between justify-center column fill-height>
        <v-flex>
        </v-flex>
        
        <v-flex>
        </v-flex>  
      </v-layout>
  </div>
</template>

<script lang="ts">
import { Component, Vue } from 'vue-property-decorator'
import RessourceManagement from '@/components/RessourceManagement/RessourceMngt.vue'
import GadgetManagement from '@/components/RessourceManagement/GadgetManagement2.vue'
import SupplierManagement from '@/components/RessourceManagement/SupplierManagement.vue'

@Component({
  components: { RessourceManagement, GadgetManagement, SupplierManagement }
})
export default class Ressources extends Vue {


}
</script>
