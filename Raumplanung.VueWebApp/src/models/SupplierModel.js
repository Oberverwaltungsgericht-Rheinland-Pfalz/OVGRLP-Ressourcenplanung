import { Model } from '@vuex-orm/core';
import Gadget from './GadgetModel';
export default class Supplier extends Model {
    // List of all fields (schema) of the post model. `this.attr` is used
    // for the generic field type. The argument is the default value.
    static fields() {
        return {
            Id: this.attr(null),
            Title: this.attr(''),
            GroupEmail: this.attr(''),
            Gadgets: this.hasMany(Gadget, 'SuppliedBy', 'Id')
        };
    }
}
// This is the name used as module name of the Vuex Store.
Supplier.entity = 'SupplierGroups';
Supplier.primaryKey = 'Id';
//# sourceMappingURL=SupplierModel.js.map