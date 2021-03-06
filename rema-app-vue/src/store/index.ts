import Vue from 'vue'
import Vuex, { StoreOptions } from 'vuex'
import { RootState } from '@/models/interfaces'
import { user } from './User/index'
import {
  Gadget,
  Ressource,
  Supplier,
  Allocation
} from '../models'
import packageInfo from '../../package.json'
import axios from 'axios'
import VuexORM from '@vuex-orm/core'
import VuexORMAxios from '@vuex-orm/plugin-axios'

Vue.use(Vuex)

const database = new VuexORM.Database()
// Register Models to Database.
database.register(Gadget)
database.register(Ressource)
database.register(Allocation)
database.register(Supplier)

VuexORM.use(VuexORMAxios, {
  axios,
  headers: {
    'Accept': 'application/json',
    'Content-Type': 'application/json'
  },
  baseURL: './api/'
})
// Vuex structure based on https://codeburst.io/vuex-and-typescript-3427ba78cfa8

export const store: StoreOptions<RootState> = {
  state: {
    version: packageInfo.version
  },
  modules: {
    user
  },
  // Create Vuex Store and register database through Vuex ORM.
  plugins: [VuexORM.install(database)]
}

export default new Vuex.Store<RootState>(store)
