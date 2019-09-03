import Vue from 'vue'
import Router, { RouteConfig } from 'vue-router'
import Home from './views/Home.vue'

Vue.use(Router)

export interface MyRouteConfig extends RouteConfig {
  icon: string
}

/* tslint:disable:ter-indent */
export default new Router({
  mode: 'history',
  base: process.env.BASE_URL,
  routes: [
    {
      path: '/',
      name: 'Planungsübersicht',
      icon: 'business',
      component: () => import('./views/Calendar.vue')
    } as MyRouteConfig,
/*    {
      path: '/new',
      name: 'Eintrag',
      icon: 'add',
      component: () => import('./components/NewAllocation/FormDialog.vue')
    } as MyRouteConfig,
  {
      path: '/multi-select',
      name: 'multiselector',
      icon: 'calendar_today',
      // route level code-splitting
      // this generates a separate chunk (about.[hash].js) for this route
      // which is lazy-loaded when the route is visited.
      component: () => import( './views/MutliDates.vue')
 //   } as MyRouteConfig,
*/  {
    path: '/acknowledge',
      name: 'Bearbeiter',
      icon: 'storage',
      component: () => import('./views/Acknowledge.vue')
    } as MyRouteConfig,
    {
      path: '/ressources',
      name: 'Verwaltung',
      icon: 'dvr',
        component: () => import('./views/Ressources.vue')
    } as MyRouteConfig,
    {
      path: '/occupancy',
      name: 'Scheduler',
      icon: 'schedule',
      component: () => import('./views/Occupancy.vue')
    } as MyRouteConfig,
    {
      path: '/supports',
      name: 'Aufgaben',
      icon: 'group_work',
      component: () => import('./views/Supporters.vue')
    } as MyRouteConfig
/*    {
      path: '/fetch-data',
      name: 'fetch-data',
      icon: 'get_app',
      component: () => import('./views/FetchData.vue')
    } as MyRouteConfig,
    {
      path: '/dbcontent',
      name: 'querys',
      icon: 'database',
      component: () => import('./views/DbContent.vue')
    } as MyRouteConfig
  */ ]
})
