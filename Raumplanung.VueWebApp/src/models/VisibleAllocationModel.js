import { Model } from '@vuex-orm/core';
export default class Visibleallocation extends Model {
    // List of all fields (schema) of the post model. `this.attr` is used
    // for the generic field type. The argument is the default value.
    static fields() {
        return {
            id: this.attr(null),
            Title: this.attr(''),
            Start: this.attr(''),
            End: this.attr(''),
            RessourceName: this.attr(''),
            Notes: this.attr('')
        };
    }
}
// This is the name used as module name of the Vuex Store.
Visibleallocation.entity = 'visibleallocation';
//# sourceMappingURL=VisibleAllocationModel.js.map