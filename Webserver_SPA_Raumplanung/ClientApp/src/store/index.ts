import Vue from 'vue'
import Vuex, { StoreOptions } from 'vuex'
import { RootState } from './types'
import { counter } from './counter/index'
import { acknowledges } from './Acknowledges/index'
import { user } from './User/index'
import VuexORM from '@vuex-orm/core'
import Gadget from '../models/GadgetModel'
import Ressource from '../models/RessourceModel'
import Visibleallocation from '../models/VisibleAllocationModel'
import SupplierModel from '../models/SupplierModel'
import AllocationModel from '../models/AllocationModel'
import AllocationpurposeModel from '../models/AllocationpurposeModel'
import packageInfo from '../../package.json'

Vue.use(Vuex)

const database = new VuexORM.Database()
// Register Models to Database.
database.register(Gadget)
database.register(Ressource)
database.register(Visibleallocation)
database.register(AllocationModel)
database.register(SupplierModel)
database.register(AllocationpurposeModel)

// Vuex structure based on https://codeburst.io/vuex-and-typescript-3427ba78cfa8

const store: StoreOptions<RootState> = {
  state: {
    version: packageInfo.version
  },
  modules: {
    counter,
    acknowledges,
    user
  },
  // Create Vuex Store and register database through Vuex ORM.
  plugins: [VuexORM.install(database)]
}



export default new Vuex.Store<RootState>(store)
