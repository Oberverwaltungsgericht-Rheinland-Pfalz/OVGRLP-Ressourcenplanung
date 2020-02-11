import { Module } from 'vuex'
import { getters } from './getters'
import { actions } from './actions'
import { mutations } from './mutations'
import { AcknowledgeState, RootState } from '@/models/interfaces'

export const state: AcknowledgeState = {
  tasks: [],
  lastUpdated: false
}

export const acknowledges: Module<AcknowledgeState, RootState> = {
  namespaced: true,
  state,
  getters,
  actions,
  mutations
}
