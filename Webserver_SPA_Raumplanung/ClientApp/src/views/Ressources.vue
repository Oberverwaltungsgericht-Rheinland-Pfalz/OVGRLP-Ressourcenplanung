<template>
  <div>
      <h1>Ressourcenverwaltung</h1>
      <v-spacer/>
      <ressource-management/>
      
      <v-spacer/>
      
      <gadget-management/>
      
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
import GadgetManagement from '@/components/RessourceManagement/GadgetManagement.vue'


@Component({
  components: { RessourceManagement, GadgetManagement }
})
export default class Ressources extends Vue {


}
</script>
