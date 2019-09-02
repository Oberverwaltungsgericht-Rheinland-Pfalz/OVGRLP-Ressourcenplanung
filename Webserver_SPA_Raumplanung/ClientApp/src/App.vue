<template>
  <v-app>
    <v-system-bar fixed  app>
      Raumplanung (Version {{$store.state.version}})
    </v-system-bar>
 <!--   <v-navigation-drawer persistent :mini-variant="miniVariant" :clipped="clipped" v-model="drawer" enable-resize-watcher fixed app>
      <v-list>
        <v-list-item value="true" v-for="(item, i) in items" :key="i" :to="item.path">
          <v-list-item-action>
            <v-icon v-html="item.icon"></v-icon>
          </v-list-item-action>
          <v-list-item-content>
            <v-list-item-title v-text="item.name"></v-list-item-title>
          </v-list-item-content>
        </v-list-item>
      </v-list>
    </v-navigation-drawer>-->

  <v-app-bar app>
    <v-layout justify-space-between wrap align-center>
      <div class="text-center">
          <v-menu offset-y open-on-hover>
            <template v-slot:activator="{ on }">
              <v-btn color="primary" dark v-on="on">
                <v-icon>more_vert</v-icon> 
                Navigation
              </v-btn>
            </template>
            <v-list>
              <v-list-item v-for="(item, index) in items" :key="index" :to="item.path">
                <v-list-item-title><v-icon v-html="item.icon"></v-icon> {{ item.name }}</v-list-item-title>
              </v-list-item>
            </v-list>
          </v-menu>
          &ensp;
          <new-form-modal/>
        </div>
        <v-spacer></v-spacer>
            <h3>Raumplanung - {{currentPath}}</h3>
            <v-spacer></v-spacer>
          <v-menu offset-y>
            <template v-slot:activator="{ on }">
              <v-avatar align-self-end color="red" v-on="on">
                <span class="white--text headline">{{userData.name[0]}}</span>
              </v-avatar>
            </template>
            <v-list>
              <v-list-item>
                <v-list-item-title><v-icon>supervisor_account</v-icon> {{ userData.name }}</v-list-item-title>
              </v-list-item>
              <v-list-item>
                <v-list-item-title><v-icon>email</v-icon> {{ userData.email }}</v-list-item-title>
              </v-list-item>
              <v-list-item>
                <v-list-item-title><v-icon>phone</v-icon> {{ userData.phone }}</v-list-item-title>
              </v-list-item>
              <v-list-item>
                <v-list-item-title><v-icon></v-icon>Rolle: {{ userData.role }}</v-list-item-title>
              </v-list-item>
              <v-list-item>
                <v-list-item-title><v-icon></v-icon>Domain: {{ userData.domain}}</v-list-item-title>
              </v-list-item>
              <v-list-item>
                <v-list-item-title><v-icon></v-icon>Unterstützergruppe: {{ userData.supplierGroups}}</v-list-item-title>
              </v-list-item>
            </v-list>
          </v-menu>
    </v-layout>
  </v-app-bar>

  <!--  <v-toolbar app :clipped-left="clipped">
      <v-toolbar-side-icon @click.stop="drawer = !drawer"></v-toolbar-side-icon>
      <v-btn icon @click.stop="miniVariant = !miniVariant">
        <v-icon v-html="miniVariant ? 'chevron_right' : 'chevron_left'"></v-icon>
      </v-btn>
      <v-btn icon @click.stop="clipped = !clipped">
        <v-icon>web</v-icon>
      </v-btn>
      <v-toolbar-title v-text="title"></v-toolbar-title>
      <v-spacer></v-spacer>
    </v-toolbar>
-->
    <v-content>
      <v-container fluid>
        <router-view/>
      </v-container>
    </v-content>
  </v-app>
</template>

<script lang="ts">
import HelloWorld from '@/components/HelloWorld.vue'
import { Component, Vue } from 'vue-property-decorator'
import Router from 'vue-router'
import { State, Action, Getter, Mutation } from 'vuex-class'
import { Names as Fnn, UserData } from './store/User/types'
import { MyRouteConfig } from './router'
import Gadgets from './models/GadgetModel'
import Users from './models/UserData'
import Ressources from './models/RessourceModel'
import Suppliers from './models/SupplierModel'
import Allocations from './models/AllocationModel'
import AllocationPurposes from './models/AllocationpurposeModel'
import { Getters } from '@vuex-orm/core'
import NewFormModal from '@/components/NewAllocation/NewFormModal.vue'

@Component({
  components: { HelloWorld, NewFormModal }
})
export default class App extends Vue {

  public get currentPath () {
    return this.$route.name
  }

  protected routes: any = []
  @Action(Fnn.a.loadUser, { namespace: 'user' })
  private loadUser: any
  @Getter('getUserData', { namespace: 'user' })
  private userData!: UserData

  private clipped: boolean = true
  private drawer: boolean = true
  private miniVariant: boolean = false
  private right: boolean = true
  private title: string = 'NJZ Raumplanung'
  private items: MyRouteConfig[] = []

  public async created () {
    this.loadUser();
    (this.$router as any).options.routes.forEach((route: any) => {
      this.items.push({
                  // name: route.name,
                  // path: route.path,
        path: route.path,
        name: route.name,
        icon: route.icon
      })
    })
    // initSampleData
/*    const gadgets = [
            { id: 31, title: 'Beamer', suppliedBy: 1 },
            { id: 32, title: 'Visualisierer', suppliedBy: 2 },
            { id: 33, title: 'Monitor', suppliedBy: 2 }
    ]
    await Gadgets.insert({ data: gadgets })*/
    // @ts-ignore
    Gadgets.$get()
    // @ts-ignore
    Suppliers.$get()
    // @ts-ignore
    Ressources.$get()
    // @ts-ignore
    Allocations.$get()
    // @ts-ignore
    AllocationPurposes.$get()
/*    const ressourcen = [
      {
        id: 1,
        name: 'Sitzungssal E022',
        type: 'Gerichtssaal',
        functionDescription: 'Saal des OVG',
        specialDescription: 'Großer Gerichtssaal'
      },{
        id: 2,
        name: 'Multifunktionsraum',
        type: 'Gemeinschaftsraum',
        functionDescription: 'Allgemeiner Raum des NJZ',
        specialDescription: '20 Sitzplätze und Beamer'
      },
      {
        id: 3,
        name: 'Sitzungssaal A008',
        type: 'Gerichtssaal',
        functionDescription: 'Saal des ArbG',
        specialDescription: 'Kleiner Gerichtssaal'
      }
    ]
    await Ressources.insert({ data: ressourcen })
/*    const gruppen = [
            { id: 1, Title: 'Wachtmeister', GroupEmail: 'NJZ.Wachtmeister@ovg.jm.rlp.de' },
            { id: 2, Title: 'EDV', GroupEmail: 'edv.support@ovg.jm.rlp.de' }
    ]
    await Suppliers.insert({ data: gruppen })
*
    const allocations = [
      {
        id: 111,
        Start: new Date('2019-08-11'),
        End: new Date('2019-08-11'),
        IsAllDay: true,
        Status: 'erledigt',
        CreatedBy: 'Müller',
        CreatedAt: '2019-07-03',
        LastModified: '2019-07-30',
        LastModifiedBy: 'Müller',
        ApprovedBy: 'Müller',
        ApprovedAt: '2019-07-30',
        ReferencePerson: 'Schmidt',
        Ressource_id: 1,
        Purpose_id: 12
      },
      {
        id: 112,
        Start: new Date('2019-08-12 11:00'),
        End: new Date('2019-08-14 12:30'),
        IsAllDay: false,
        Status: 'erledigt',
        CreatedBy: 'Müller',
        CreatedAt: '2019-07-30',
        LastModified: '2019-07-30',
        LastModifiedBy: 'Müller',
        ApprovedBy: 'Müller',
        ApprovedAt: '2019-07-30',
        ReferencePerson: 'Schmidt',
        Ressource_id: 2,
        Purpose_id: 11
      }
    ]
    await Allocations.insert({ data: allocations })
*/
/*    const allocationPurposes = [
      { id: 11, Title: 'VG 1. Kammer', Description: 'Verhandlungstag', Notes: '', ContactPhone: '', Gadget_ids: [1] },
      { id: 12, Title: 'OVG 3. Kammer', Description: 'Verhandlungstag', Notes: '', ContactPhone: '10', Gadget_ids: [2] }
   //   { id: 13, Title: 'ArbG 0. Kammer', Description: 'Verhandlung', Notes: '', ContactPhone: '31', Gadget_ids: [] }
    ]
    await AllocationPurposes.insert({ data: allocationPurposes })
*/
  }
}
</script>
