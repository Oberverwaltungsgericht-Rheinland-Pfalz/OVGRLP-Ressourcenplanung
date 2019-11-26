import { MutationTree } from 'vuex'
import { UserState, UserData, Names } from './types'
import { stat } from 'fs'

export const mutations: MutationTree<UserState> = {
  [Names.m.clearUser] (state: any) {
    state.name = ''
    state.email = ''
    state.role = false
    state.supplierGroups = false
    state.organisation = ''
    state.lastUpdated = new Date()
  },
  [Names.m.setUser] (state: any, userPayload: any) {
    for (const key in userPayload) {
      if (userPayload.hasOwnProperty(key)) state[key] = userPayload[key]
    }

    state.lastUpdated = new Date()
  }
}

