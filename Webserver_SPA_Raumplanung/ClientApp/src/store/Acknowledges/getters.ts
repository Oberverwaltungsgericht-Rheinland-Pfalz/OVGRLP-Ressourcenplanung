import { GetterTree } from 'vuex'
import { AcknowledgeState } from './types'
import { RootState } from '../types'

export const getters: GetterTree<AcknowledgeState, RootState> = {
  isLoadingDone (state): boolean {
    return !!state.lastUpdated
  },
  isEmpty (state): boolean {
    return !!state.lastUpdated && state.tasks.length === 0
  }
}
