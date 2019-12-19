import { getters } from './getters';
import { actions } from './actions';
import { mutations } from './mutations';
export const state = {
    counter: 0
};
const namespaced = true;
export const counter = {
    namespaced,
    state,
    getters,
    actions,
    mutations
};
//# sourceMappingURL=index.js.map