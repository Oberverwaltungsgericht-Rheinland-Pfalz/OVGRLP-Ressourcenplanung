import { GetterTree } from 'vuex'
import { AcknowledgeState, RootState } from '@/models/interfaces'

export const getters: GetterTree<AcknowledgeState, RootState> = {
  isLoadingDone (state): boolean {
    return !!state.lastUpdated
  },
  isEmpty (state): boolean {
    return !!state.lastUpdated && state.tasks.length === 0
  }
}
