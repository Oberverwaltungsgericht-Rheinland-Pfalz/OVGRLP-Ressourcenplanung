import { MutationTree } from 'vuex'
import { UserState, UserData, Names } from './types'
import { stat } from 'fs'

export const mutations: MutationTree<UserState> = {
  [Names.m.clearUser] (state: any) {
    state.id = 0
    state.name = ''
    state.email = ''
    state.role = false
    state.supplierGroups = false
    state.organisation = ''
    state.lastUpdated = new Date()
  },
  [Names.m.setUser] (state: any, userPayload: any) {
    state.id = userPayload.Id
    state.name = userPayload.Name
    state.email = userPayload.Email
    state.role = Math.max(...userPayload.Roles.map((e: any) => e.Level))
    state.roleNames = userPayload.Roles.reduce(((last: string, e: any) => last + e.Name + ' '), '')
    state.supplierGroups = userPayload.SupplierGroups
    state.organisation = userPayload.Organisation
    state.lastUpdated = new Date()
    /* for (const key in userPayload) {
      if (userPayload.hasOwnProperty(key)) state[key] = userPayload[key]
    } */
  }
}

