import { Names } from './types';
export const actions = {
    async [Names.a.loadUser]({ commit }) {
        const response = await fetch(`/api/Users/me`, {
            method: 'GET',
            mode: 'cors',
            cache: 'no-cache',
            credentials: 'same-origin',
            headers: {
                'Content-Type': 'application/json'
            }
        });
        const responseObj = await response.json();
        commit(Names.m.setUser, responseObj);
    },
    async [Names.a.reloadUser]({ commit, dispatch }) {
        commit(Names.m.clearUser);
        await dispatch(Names.a.loadUser);
        // commit(Names.m.setUser, user)
    }
};
//# sourceMappingURL=actions.js.map