import { Model } from '@vuex-orm/core';
export default class Gadget extends Model {
    // List of all fields (schema) of the post model. `this.attr` is used
    // for the generic field type. The argument is the default value.
    static fields() {
        return {
            Id: this.attr(null),
            Title: this.attr(''),
            SuppliedBy: this.attr(null)
        };
    }
}
// This is the name used as module name of the Vuex Store.
Gadget.entity = 'gadgets';
Gadget.primaryKey = 'Id';
//# sourceMappingURL=GadgetModel.js.map