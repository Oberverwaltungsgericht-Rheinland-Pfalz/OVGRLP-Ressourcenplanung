import { ActionTree } from 'vuex'
import { Names } from './types'
import { UserState, RootState } from '@/models/interfaces'

export const actions: ActionTree<UserState, RootState> = {
  async [Names.a.loadUser] ({ commit }): Promise<any> {
    const response = await fetch(`/api/Users/me`, {
      method: 'GET', // *GET, POST, PUT, DELETE, etc.
      mode: 'cors', // no-cors, cors, *same-origin
      cache: 'no-cache', // *default, no-cache, reload, force-cache, only-if-cached
      credentials: 'same-origin', // include, *same-origin, omit
      headers: {
        'Content-Type': 'application/json'
      }
    })
    const responseObj = await response.json()
    commit(Names.m.setUser, responseObj)
  },
  async [Names.a.reloadUser] ({ commit, dispatch }): Promise<any> {
    commit(Names.m.clearUser)
    await dispatch(Names.a.loadUser)
    // commit(Names.m.setUser, user)
  },
  async [Names.a.loadUsers] ({ commit, state }, referencePersonsUnique: number[]): Promise<any> {
    const response = await fetch(`/api/Users/Names/${referencePersonsUnique}`)
    const responseArray = await response.json()

    responseArray.forEach((id : number) => {
      
    })
    const entryIdx = state.ContactUsers.findIndex((s) => s.Id === userPayload.Id)
    state.ContactUsers.splice(entryIdx, 1, userPayload)
    commit(Names.m.setUser, responseObj)
  }
}
