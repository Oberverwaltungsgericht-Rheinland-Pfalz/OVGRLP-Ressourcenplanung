import { MutationTree } from 'vuex'
import { Names } from './types'
import { UserState } from '@/models/interfaces'

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
  [Names.m.setUser] (state: UserState, userPayload: WebApi.UserViewModel) {
    state.id = userPayload.Id
    state.name = userPayload.Name
    state.email = userPayload.Email
    state.role = Math.max(...userPayload.Roles.map((e: any) => e.Level))
    state.roleNames = userPayload.Roles.reduce((last: string, e: any) => last + e.Name + ' ', '')
    state.supplierGroups = userPayload.SupportGroupIds
    state.organisation = userPayload.Organisation
    state.lastUpdated = new Date()
  },
  [Names.m.addContactUser] (state: UserState, userPayload: WebApi.ContactUser) {
    const entryIdx = state.ContactUsers.findIndex((s) => s.Id === userPayload.Id)
    let deleteAnEntry = entryIdx < 0 ? 0 : 1
    state.ContactUsers.splice(entryIdx, deleteAnEntry, userPayload)
  },
  [Names.m.reserveContactUser] (state: UserState, id: number) {
    const hasEntry = state.ContactUsers.find((s) => s.Id === id && s.Title)
    if (hasEntry) return
    state.ContactUsers.push({ Id: id, Title: '', Email: '', Organisation: '' })
  }
}
