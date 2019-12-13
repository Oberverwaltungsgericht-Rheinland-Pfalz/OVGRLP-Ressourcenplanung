<template>
  <v-app>
    <v-system-bar fixed app>
      Raumplanung (Version {{ $store.state.version }})
    </v-system-bar>
    <v-navigation-drawer
      persistent
      :mini-variant="miniVariant"
      :clipped="clipped"
      v-model="drawer"
      enable-resize-watcher
      fixed
      app
    >
      <v-list>
        <v-list-item
          value="true"
          v-for="(item, i) in items"
          :key="i"
          :to="item.path"
        >
          <v-list-item-action>
            <v-icon>{{ item.icon }}</v-icon>
          </v-list-item-action>
          <v-list-item-content>
            <v-list-item-title>{{ item.name }}</v-list-item-title>
          </v-list-item-content>
        </v-list-item>
      </v-list>
    </v-navigation-drawer>

    <v-app-bar app>
      <v-layout justify-space-between wrap align-center>
        <div class="text-center">
          <new-form-modal />
          &ensp;
          <v-menu v-if="!drawer" offset-y open-on-hover>
            <template v-slot:activator="{ on }">
              <v-btn color="primary" dark v-on="on">
                <v-icon>more_vert</v-icon>
                Navigation
              </v-btn>
            </template>
            <v-list>
              <v-list-item
                v-for="(item, index) in items"
                :key="index"
                :to="item.path"
              >
                <v-list-item-title
                  ><v-icon>{{ item.icon }}</v-icon>
                  {{ item.name }}</v-list-item-title
                >
              </v-list-item>
            </v-list>
          </v-menu>
        </div>
        <v-spacer></v-spacer>
        <h3>Raumplanung - {{ currentPath }}</h3>
        <v-spacer></v-spacer>
        <v-menu offset-y>
          <template v-slot:activator="{ on }">
            <v-avatar
              align-self-end
              color="red"
              v-on="on"
              class="action-avatar"
            >
              <span class="white--text headline">{{ userData.name[0] }}</span>
            </v-avatar>
          </template>
          <v-list>
            <v-list-item>
              <v-list-item-title
                ><v-icon>supervisor_account</v-icon>
                {{ userData.name }}</v-list-item-title
              >
            </v-list-item>
            <v-list-item>
              <v-list-item-title
                ><v-icon>email</v-icon> {{ userData.email }}</v-list-item-title
              >
            </v-list-item>
            <v-list-item>
              <v-list-item-title
                ><v-icon></v-icon>Rolle:
                {{ userData.roleNames }}</v-list-item-title
              >
            </v-list-item>
            <v-list-item>
              <v-list-item-title
                ><v-icon></v-icon>Organisation:
                {{ userData.organisation }}</v-list-item-title
              >
            </v-list-item>
            <v-list-item>
              <v-list-item-title
                ><v-icon></v-icon>Unterstützergruppe:
                {{ userData.supplierGroups }}</v-list-item-title
              >
            </v-list-item>
          </v-list>
        </v-menu>
      </v-layout>
    </v-app-bar>

    <v-content>
      <v-container fluid>
        <router-view />
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
    await this.loadUser();
    (this.$router as any).options.routes.forEach((route: any) => {
      if (this.$store.state.user.role >= route.authLevel) {
        this.items.push({
          path: route.path,
          name: route.name,
          icon: route.icon,
          authLevel: route.authLevel
        })
      }
    })

    Gadgets.api().get('gadgets')
    Suppliers.api().get('SupplierGroups')
    Ressources.api().get('ressources')
    Allocations.api().get('allocations')

    const allpResp = await AllocationPurposes.api().get('allocationpurposes')
    console.dir(allpResp)
  }
}
</script>

<style lang="stylus" scoped>
.action-avatar
  cursor pointer
</style>
