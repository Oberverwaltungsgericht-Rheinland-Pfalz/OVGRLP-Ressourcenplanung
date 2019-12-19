import Vue from 'vue';
import Router from 'vue-router';
import store from './store/index';
Vue.use(Router);
/* tslint:disable:ter-indent */
export default new Router({
    mode: 'history',
    base: process.env.BASE_URL,
    routes: [
        {
            path: '/',
            name: 'Planungsübersicht',
            icon: 'business',
            authLevel: 0,
            component: () => import('./views/Calendar.vue')
        },
        /*  {
              path: '/multi-select',
              name: 'multiselector',
              icon: 'calendar_today',
              // route level code-splitting
              // this generates a separate chunk (about.[hash].js) for this route
              // which is lazy-loaded when the route is visited.
              component: () => import( './views/MutliDates.vue')
         //   } as MyRouteConfig, */
        {
            path: '/acknowledge',
            name: 'Anfragenverwaltung',
            icon: 'storage',
            authLevel: 10,
            beforeEnter: (to, from, next) => requireAuth(10, to, from, next),
            component: () => import('./views/Acknowledge.vue')
        },
        {
            path: '/mylist',
            name: 'Ihre Anfragen',
            icon: 'calendar_view_day',
            authLevel: 0,
            component: () => import('./components/MyList.vue')
        },
        {
            path: '/ressources',
            name: 'Administration',
            icon: 'dvr',
            authLevel: 100,
            beforeEnter: (to, from, next) => requireAuth(100, to, from, next),
            component: () => import('./views/Ressources.vue')
        },
        {
            path: '/occupancy',
            name: 'Scheduler',
            icon: 'schedule',
            authLevel: 0,
            component: () => import('./views/Occupancy.vue')
        },
        {
            path: '/supports',
            name: 'Aufgaben',
            icon: 'group_work',
            authLevel: 0,
            beforeEnter: (to, from, next) => requireAuth(0, to, from, next),
            component: () => import('./views/Supporters.vue')
        }
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
          */ 
    ]
});
function requireAuth(level, to, from, next) {
    // @ts-ignore
    const role = store.state.user.role;
    if (role >= level) {
        next();
    }
    else {
        next('/'); // from.path
    }
}
// const roles = { admin: 100, editor: 10 }
//# sourceMappingURL=router.js.map