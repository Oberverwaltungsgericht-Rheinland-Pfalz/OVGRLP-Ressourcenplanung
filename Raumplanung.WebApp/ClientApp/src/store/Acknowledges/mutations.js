import { Names } from './types';
export const mutations = {
    [Names.m.updateTask](state, payload) {
        const task = state.tasks.find((el, idx) => el.id === payload.id);
        if (!task) {
            throw new Error('task id not found');
        }
        task.Status = payload.Status;
        task.DateTime = payload.DateTime;
    },
    [Names.m.loadTasks](state, tasks) {
        while (state.tasks.length)
            state.tasks.pop();
        tasks.forEach((el) => state.tasks.push(el));
        state.lastUpdated = new Date();
    },
    [Names.m.addTask](state, task) {
        state.tasks.push(task);
    }
};
//# sourceMappingURL=mutations.js.map