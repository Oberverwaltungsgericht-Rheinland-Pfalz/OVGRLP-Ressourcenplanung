export const getters = {
    isLoadingDone(state) {
        return !!state.lastUpdated;
    },
    getUserData(state) {
        const returnObject = {};
        for (const key in state) {
            if (state.hasOwnProperty(key))
                returnObject[key] = state[key];
        }
        return returnObject;
    }
};
//# sourceMappingURL=getters.js.map