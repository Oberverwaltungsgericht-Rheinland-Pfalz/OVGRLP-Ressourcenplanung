export const getters = {
    isLoadingDone(state) {
        return !!state.lastUpdated;
    },
    isEmpty(state) {
        return !!state.lastUpdated && state.tasks.length === 0;
    }
};
//# sourceMappingURL=getters.js.map