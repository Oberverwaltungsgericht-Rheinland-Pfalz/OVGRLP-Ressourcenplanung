import { ActionTree } from 'vuex'
import { Names } from './types'
import { UserState, RootState } from '../../models/interfaces'

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
    let requestable = response.headers.get('Requests-Allowed') || ''
    let showCalendarFrom = response.headers.get('Calendar-From') || 0
    let hideCalendarFrom = response.headers.get('Calendar-From-Hide') || 0
    let siteTitle = response.headers.get('Site-Title') || 'Raumplanung'
    document.title = siteTitle

    commit(Names.m.setCalendarFrom, showCalendarFrom)
    commit(Names.m.setHideCalendarFrom, hideCalendarFrom)
    commit(Names.m.setUser, responseObj)
    commit(Names.m.setRequestable, requestable)
  },
  async [Names.a.reloadUser] ({ commit, dispatch }): Promise<any> {
    commit(Names.m.clearUser)
    await dispatch(Names.a.loadUser)
    // commit(Names.m.setUser, user)
  },
  async [Names.a.loadUsers] ({ commit }, ids: number[]): Promise<any> {
    ids = ids.filter(v => !isNaN(v))
    const referencePersonsUnique = [...new Set(ids)]
    const response = await fetch(`/api/Users/Names/${referencePersonsUnique.join('_')}`)
    const newContacts = await response.json()
    let userResponse = newContacts as WebApi.ContactUser[]
    if (userResponse && userResponse != null) {
      for (let user of userResponse) {
        commit(Names.m.addContactUser, user)
      }
    }
  }
}
