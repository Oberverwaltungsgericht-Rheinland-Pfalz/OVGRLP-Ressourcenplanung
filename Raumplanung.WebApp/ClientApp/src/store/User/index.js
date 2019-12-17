import { getters } from './getters';
import { actions } from './actions';
import { mutations } from './mutations';
export const state = {
    id: 0,
    name: '',
    email: '',
    role: 0,
    roleNames: '',
    supplierGroups: false,
    organisation: '',
    lastUpdated: false,
    ContactUsers: []
};
export const user = {
    namespaced: true,
    state,
    getters,
    actions,
    mutations
};
//# sourceMappingURL=index.js.map