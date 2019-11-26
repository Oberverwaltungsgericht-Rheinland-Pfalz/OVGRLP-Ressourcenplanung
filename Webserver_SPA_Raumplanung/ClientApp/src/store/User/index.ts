import { Module } from 'vuex'
import { getters } from './getters'
import { actions } from './actions'
import { mutations } from './mutations'
import { UserState } from './types'
import { RootState } from '../types'

export const state: UserState = {
  name: '',
  email: '',
  role: 0,
  supplierGroups: false,
  organisation: '',
  lastUpdated: false
}

export const user: Module<UserState, RootState> = {
  namespaced: true,
  state,
  getters,
  actions,
  mutations
}
