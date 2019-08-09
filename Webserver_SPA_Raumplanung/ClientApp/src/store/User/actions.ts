import { ActionTree } from 'vuex'
import axios from 'axios'
import { UserState, UserData, Names } from './types'
import { RootState } from '../types'

const exampleUser: UserData = {
  name: 'Reiner',
  email: 'e@mail.ovg.jm.rlp.de',
  role: 'admin',
  phone: '012345',
  domain: 'OVGVG',
  supplierGroups: [1]
}

export const actions: ActionTree<UserState, RootState> = {
  [Names.a.loadUser] ({ commit }): any {

    // todo: load from server
    const user: UserData = exampleUser

    commit(Names.m.setUser, user)
  },
  [Names.a.reloadUser] ({ commit }, task): any {
    // todo: load from server
    const user: UserData = exampleUser

    commit(Names.m.clearUser)
    commit(Names.m.setUser, user)
  }
}
