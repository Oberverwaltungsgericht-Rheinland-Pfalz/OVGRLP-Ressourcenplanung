<template>
  <v-app>
    <v-progress-linear v-if="loading" style="z-index: 99;" absolute top indeterminate color="blue"/>
    <v-system-bar fixed app>
      Raumplanung (Version {{ $store.state.version }})
    </v-system-bar>
    <v-navigation-drawer
      v-if="showNav"
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
          <new-form-modal />&ensp;
          <v-menu v-if="!drawer && showNav" offset-y open-on-hover>
            <template v-slot:activator="{ on }">
              <v-btn color="primary" dark v-on="on">
                <v-icon>more_vert</v-icon>Navigation
              </v-btn>
            </template>
            <v-list>
              <v-list-item
                v-for="(item, index) in items"
                :key="index"
                :to="item.path"
              >
                <v-list-item-title>
                  <v-icon>{{ item.icon }}</v-icon>
                  {{ item.name }}
                </v-list-item-title>
              </v-list-item>
            </v-list>
          </v-menu>
        </div>
        <v-spacer></v-spacer>
        <h3 v-if="!loading"><span id="bigTitle">Raumplanung - </span>{{ currentPath }}</h3>
        <h3 v-else>Programm wird gestartet...</h3>
        <v-spacer></v-spacer>
        <v-avatar id="helplink"
          align-self-end
          color="blue"
          class="action-avatar"
          @click="openHandbook"
        ><v-icon id="helpicon">help_outline</v-icon>
        </v-avatar>
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
              <v-list-item-title>
                <v-icon>supervisor_account</v-icon>
                {{ userData.name }}
              </v-list-item-title>
            </v-list-item>
            <v-list-item>
              <v-list-item-title>
                <v-icon>email</v-icon>
                {{ userData.email }}
              </v-list-item-title>
            </v-list-item>
            <v-list-item>
              <v-list-item-title>
                <v-icon></v-icon>
                Rolle:
                {{ userData.roleNames }}
              </v-list-item-title>
            </v-list-item>
            <v-list-item>
              <v-list-item-title>
                <v-icon></v-icon>
                Organisation:
                {{ userData.organisation }}
              </v-list-item-title>
            </v-list-item>
            <v-list-item>
              <v-list-item-title>
                <v-icon></v-icon>
                Termine reservierbar:
                {{ isRequestable }}
              </v-list-item-title>
            </v-list-item>
          </v-list>
        </v-menu>
      </v-layout>
    </v-app-bar>

    <v-main>
      <v-container v-if="!loading" fluid>
        <router-view />
      </v-container>
    </v-main>
    <toasts/>
  </v-app>
</template>

<script lang="ts">
import { Component, Vue } from 'vue-property-decorator'
import Router from 'vue-router'
import { State, Action, Getter, Mutation } from 'vuex-class'
import { Names as Fnn } from './store/User/types'
import { UserData } from './models/interfaces'
import { Gadget, Ressource, Supplier, Allocation } from './models'
import { Getters } from '@vuex-orm/core'
import NewFormModal from '@/components/NewAllocation/NewFormModal.vue'
import Toasts from '@/components/Toasts.vue'
// import { RemaRouteConfig } from './models/interfaces/RemaRouteConfig'
import { refreshAllocations } from './services/AllocationApiService'
// import handbook from '../public/Raumplanung Handbuch_v1.0.1.pdf'

@Component({
  components: { NewFormModal, Toasts }
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
  private items: object[] = []
  private loading: boolean = true

  public get isRequestable () : string {
    return this.$store.state.user.isRequestable ? 'Ja' : 'Nein'
  }
  public get showNav () : boolean {
    if (this.loading) return false
    return this.userData.roleNames.length > 0
  }
  public async created () {
    let promise1 = Gadget.api().get('gadgets')
    let promise2 = Supplier.api().get('suppliergroups')
    let promise3 = Ressource.api().get('ressources')
    let promise4 = refreshAllocations()

    await this.loadUser()
    ;(this.$router as any).options.routes.forEach((route: any) => {
      if (this.$store.state.user.role >= route.authLevel) {
        this.items.push({
          path: route.path,
          name: route.name,
          icon: route.icon,
          authLevel: route.authLevel
        })
      }
    })

    await Promise.all([promise1, promise2, promise3, promise4])
    this.loading = false
  }

  private openHandbook () {
    window.open('/Raumplanung_Handbuch.pdf', '_blank')
  }
}
</script>

<style lang="stylus" scoped>
.action-avatar
  cursor pointer

@media (max-width: 768px)
  #bigTitle
   display none
#helplink
  margin-right .5em
#helpicon
  color white
</style>

<style lang="stylus">
.alignRight
  margin-left auto
</style>
