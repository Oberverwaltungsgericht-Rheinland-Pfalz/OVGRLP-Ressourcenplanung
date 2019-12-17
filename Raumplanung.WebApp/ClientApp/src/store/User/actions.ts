import { ActionTree } from 'vuex'
import axios from 'axios'
import { UserState, UserData, Names } from './types'
import { RootState } from '../types'

export const actions: ActionTree<UserState, RootState> = {
  async [Names.a.loadUser] ({ commit }): Promise<any> {
    const response = await fetch(`/api/Users/me`, {
      method: 'GET', // *GET, POST, PUT, DELETE, etc.
      mode: 'cors', // no-cors, cors, *same-origin
      cache: 'no-cache', // *default, no-cache, reload, force-cache, only-if-cached
      credentials: 'same-origin', // include, *same-origin, omit
      headers: {
        'Content-Type': 'application/json'
      }})
    const responseObj = await response.json()
    commit(Names.m.setUser, responseObj)
  },
  async [Names.a.reloadUser] ({ commit, dispatch }): Promise<any> {
    commit(Names.m.clearUser)
    await dispatch(Names.a.loadUser)
    // commit(Names.m.setUser, user)
  }
}
