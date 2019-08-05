import Vue from 'vue'
import Vuex, { StoreOptions } from 'vuex'
import { RootState } from './types'
import { counter } from './counter/index'
import { acknowledges } from './Acknowledges/index'
import VuexORM from '@vuex-orm/core'
import Gadget from '../models/GadgetModel'
import Ressource from '../models/RessourceModel'

Vue.use(Vuex)

const database = new VuexORM.Database()
// Register Models to Database.
database.register(Gadget)
database.register(Ressource)

// Vuex structure based on https://codeburst.io/vuex-and-typescript-3427ba78cfa8

const store: StoreOptions<RootState> = {
  state: {
    version: '1.0.0' // a simple property
  },
  modules: {
    counter,
    acknowledges
  },
  // Create Vuex Store and register database through Vuex ORM.
  plugins: [VuexORM.install(database)]
}



export default new Vuex.Store<RootState>(store)
