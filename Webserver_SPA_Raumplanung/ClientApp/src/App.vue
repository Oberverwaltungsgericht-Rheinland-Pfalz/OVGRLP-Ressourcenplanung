<template>
  <v-app>

    <v-navigation-drawer persistent :mini-variant="miniVariant" :clipped="clipped" v-model="drawer" enable-resize-watcher fixed app>
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
    </v-navigation-drawer>

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
import { MyRouteConfig } from './router'

@Component({
  components: { HelloWorld }
})
export default class App extends Vue {
  protected routes: any = []
  private clipped: boolean = true
  private drawer: boolean = true
  private miniVariant: boolean = false
  private right: boolean = true
  private title: string = 'ASP.NET Core Vue Starter'
  private items: MyRouteConfig[] = []
  public created () {
    (this.$router as any).options.routes.forEach((route: any) => {
      this.items.push({
                  // name: route.name,
                  // path: route.path,
        path: route.path,
        name: route.name,
        icon: route.icon
      })
    })
  }
}
</script>
