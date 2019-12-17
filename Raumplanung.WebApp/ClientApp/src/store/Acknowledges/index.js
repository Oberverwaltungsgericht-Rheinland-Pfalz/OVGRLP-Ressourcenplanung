import { getters } from './getters';
import { actions } from './actions';
import { mutations } from './mutations';
export const state = {
    tasks: [],
    lastUpdated: false
};
export const acknowledges = {
    namespaced: true,
    state,
    getters,
    actions,
    mutations
};
//# sourceMappingURL=index.js.map