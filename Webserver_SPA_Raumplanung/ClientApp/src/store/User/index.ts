import { Module } from 'vuex'
import { getters } from './getters'
import { actions } from './actions'
import { mutations } from './mutations'
import { UserState } from './types'
import { RootState } from '../types'

export const state: UserState = {
  id: 0,
  name: '',
  email: '',
  role: 0,
  roleNames: '',
  supplierGroups: false,
  organisation: '',
  lastUpdated: false,
  ContactUsers: []
}

export const user: Module<UserState, RootState> = {
  namespaced: true,
  state,
  getters,
  actions,
  mutations
}
