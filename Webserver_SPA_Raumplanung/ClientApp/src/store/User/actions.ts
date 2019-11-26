import { ActionTree } from 'vuex'
import axios from 'axios'
import { UserState, UserData, Names } from './types'
import { RootState } from '../types'

const exampleUser: UserData = {
  name: 'Reiner',
  email: 'e@mail.ovg.jm.rlp.de',
  role: '2',
  phone: '012345',
  domain: 'OVGVG',
  supplierGroups: [1]
}

export const actions: ActionTree<UserState, RootState> = {
  async [Names.a.loadUser] ({ commit }): Promise<any> {
    const response = fetch(`/api/Users/me`, {
      method: 'GET', // *GET, POST, PUT, DELETE, etc.
      mode: 'cors', // no-cors, cors, *same-origin
      cache: 'no-cache', // *default, no-cache, reload, force-cache, only-if-cached
      credentials: 'same-origin', // include, *same-origin, omit
      headers: {
        'Content-Type': 'application/json'
      }})
    console.dir(response)
    commit(Names.m.setUser, response)
  },
  [Names.a.reloadUser] ({ commit }, task): any {
    // todo: load from server
    const user: UserData = exampleUser

    commit(Names.m.clearUser)
    commit(Names.m.setUser, user)
  }
}
