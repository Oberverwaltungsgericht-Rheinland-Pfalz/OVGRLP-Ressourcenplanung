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
import Ressources from './models/RessourceModel'
import Suppliers from './models/SupplierModel'
import { Getters } from '@vuex-orm/core'

@Component({
  components: { HelloWorld }
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
  private title: string = 'ASP.NET Core Vue Starter'
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
    const gadgets = [
            { id: 1, Title: 'Beamer', Gadget: 'Hilfsmittel', Supplier_id: 1 },
            { id: 2, Title: 'Visualisierer', Gadget: 'Hilfsmittel', Supplier_id: 2 },
            { id: 3, Title: 'Monitor', Gadget: 'Ausstattung', Supplier_id: 2 }
    ]
    await Gadgets.insert({ data: gadgets })
    const ressourcen = [
      {
        id: 1,
        Title: 'Sitzungssal E022',
        Type: 'Gerichtssaal',
        FunctionDescription: 'Saal des OVG',
        SpecialDescription: 'Großer Gerichtssaal'
      },{
        id: 2,
        Title: 'Multifunktionsraum',
        Type: 'Gemeinschaftsraum',
        FunctionDescription: 'Allgemeiner Raum des NJZ',
        SpecialDescription: '20 Sitzplätze und Beamer'
      }
    ]
    await Ressources.insert({ data: ressourcen })
    const gruppen = [
            { id: 1, Title: 'Wachtmeister', GroupEmail: 'NJZ.Wachtmeister@ovg.jm.rlp.de' },
            { id: 2, Title: 'EDV', GroupEmail: 'edv.support@ovg.jm.rlp.de' }
    ]
    await Suppliers.insert({ data: gruppen })
  }
}
</script>
