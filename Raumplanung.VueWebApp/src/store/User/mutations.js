import { Names } from './types';
export const mutations = {
    [Names.m.clearUser](state) {
        state.id = 0;
        state.name = '';
        state.email = '';
        state.role = false;
        state.supplierGroups = false;
        state.organisation = '';
        state.lastUpdated = new Date();
    },
    [Names.m.setUser](state, userPayload) {
        state.id = userPayload.Id;
        state.name = userPayload.Name;
        state.email = userPayload.Email;
        state.role = Math.max(...userPayload.Roles.map((e) => e.Level));
        state.roleNames = userPayload.Roles.reduce(((last, e) => last + e.Name + ' '), '');
        state.supplierGroups = userPayload.SupplierGroups;
        state.organisation = userPayload.Organisation;
        state.lastUpdated = new Date();
        /* for (const key in userPayload) {
          if (userPayload.hasOwnProperty(key)) state[key] = userPayload[key]
        } */
    },
    [Names.m.addContactUser](state, userPayload) {
        const entryIdx = state.ContactUsers.findIndex((s) => s.Id === userPayload.Id);
        state.ContactUsers.splice(entryIdx, 1, userPayload);
    },
    [Names.m.reserveContactUser](state, id) {
        const hasEntry = state.ContactUsers.find((s) => s.Id === id && s.Title);
        if (hasEntry)
            return;
        state.ContactUsers.push({ Id: id, Title: '', Email: '', Organisation: '' });
    }
};
//# sourceMappingURL=mutations.js.map