import Vue from 'vue'
import Router, { RouteConfig } from 'vue-router'
import store from '@/store/index'
// import { RemaRouteConfig } from '@/models/interfaces/RemaRouteConfig'

Vue.use(Router)

/* tslint:disable:ter-indent */
export default new Router({
  mode: 'hash',
  base: process.env.BASE_URL,
  routes: [
    {
      path: '/',
      name: 'Planungsübersicht',
      icon: 'business',
      authLevel: 0,
      component: () => import('@/views/Calendar.vue')
    } as any,
    {
      path: '/overview',
      name: 'Terminübersicht',
      icon: 'storage',
      authLevel: 1,
      component: () => import('@/components/AllList.vue')
    } as any,
    {
      path: '/acknowledge',
      name: 'Anfragenverwaltung',
      icon: 'storage',
      authLevel: 10,
      beforeEnter: (to: any, from: any, next: any) => requireAuth(10, to, from, next),
      component: () => import('@/views/Acknowledge.vue')
    } as any,
    {
      path: '/schedule',
      name: 'Ressourcenplanung',
      icon: 'storage',
      authLevel: 0,
      beforeEnter: (to: any, from: any, next: any) => requireAuth(0, to, from, next),
      component: () => import('@/views/Schedules.vue')
    } as any,
     {
       path: '/ressources',
       name: 'Administration',
       icon: 'dvr',
       authLevel: 100,
       beforeEnter: (to: any, from: any, next: any) => requireAuth(100, to, from, next),
       component: () => import('@/views/Ressources.vue')
     } as any,
    {
      path: '/roomlist',
      name: 'Raumübersicht',
      icon: 'art_track',
      authLevel: 1,
      beforeEnter: (to: any, from: any, next: any) => requireAuth(1, to, from, next),
      component: () => import('@/components/RoomView.vue')
    } as any,
    {
      path: '/gadgetlist',
      name: 'Hilfsmittelübersicht',
      icon: 'library_add',
      authLevel: 1,
      beforeEnter: (to :any, from: any, next: any) => requireAuth(1, to, from, next),
      component: () => import('@/components/GadgetView.vue')
    } as any
    /*
    {
      path: '/occupancy',
      name: 'Scheduler',
      icon: 'schedule',
      authLevel: 1,
      component: () => import('@/views/Occupancy.vue')
    } as RemaRouteConfig,
    {
      path: '/supports',
      name: 'Aufgaben',
      icon: 'group_work',
      authLevel: 1,
      beforeEnter: (to, from, next) => requireAuth(0, to, from, next),
      component: () => import('@/views/Supporters.vue')
    } as RemaRouteConfig
    */
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

function requireAuth (level: number, to: any, from: any, next: any) {
  // @ts-ignore
  const role: number = store.state.user.role
  if (role >= level) {
    next()
  } else {
    next('/') // from.path
  }
}

// const roles = { admin: 100, editor: 10, reader: 1, 'unknown':0 }
