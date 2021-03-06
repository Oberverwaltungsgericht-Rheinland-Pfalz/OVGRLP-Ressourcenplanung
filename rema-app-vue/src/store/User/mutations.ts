import { MutationTree } from 'vuex'
import { Names } from './types'
import { UserState } from '@/models/interfaces'

export const mutations: MutationTree<UserState> = {
  [Names.m.clearUser] (state: UserState) {
    state.id = 0
    state.name = ''
    state.email = ''
    state.role = ''
    state.supplierGroups = false
    state.organisation = ''
    state.lastUpdated = new Date()
  },
  [Names.m.setUser] (state: UserState, userPayload: WebApi.UserViewModel) {
    state.id = userPayload.Id
    state.name = userPayload.Name
    state.email = userPayload.Email
    state.role = Math.max(...userPayload.Roles.map((e: WebApi.Role) => e.Level))
    state.roleNames = userPayload.Roles.reduce((last: string, e: WebApi.Role) => last + e.Name + ' ', '')
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
  },
  [Names.m.setRequestable] (state: UserState, requestable: boolean) {
    state.isRequestable = requestable && state.role > 0
  },
  [Names.m.setCalendarFrom] (state: UserState, calendarHourFrom: number) {
    state.calendarFrom = calendarHourFrom
  },
  [Names.m.setHideCalendarFrom] (state: UserState, hideCalendarFrom: boolean) {
    state.hideCalendarFrom = hideCalendarFrom
  }
}
