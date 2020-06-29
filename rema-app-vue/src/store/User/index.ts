import { Module } from 'vuex'
import { getters } from './getters'
import { actions } from './actions'
import { mutations } from './mutations'
import { UserState, RootState } from '../../models/interfaces'

export const state: UserState = {
  id: 0,
  name: '',
  email: '',
  role: 0,
  roleNames: '',
  organisation: '',
  supplierGroups: false,
  lastUpdated: false,
  ContactUsers: [],
  isRequestable: false,
  calendarFrom: 0
}

export const user: Module<UserState, RootState> = {
  namespaced: true,
  state,
  getters,
  actions,
  mutations
}
