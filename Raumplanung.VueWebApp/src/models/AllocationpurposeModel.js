import { Model } from '@vuex-orm/core';
import Allocation from './AllocationModel';
import Gadget from './GadgetModel';
export default class Allocationpurpose extends Model {
    // List of all fields (schema) of the post model. `this.attr` is used
    // for the generic field type. The argument is the default value.
    static fields() {
        return {
            Id: this.attr(null),
            Title: this.attr(''),
            Description: this.attr(''),
            Notes: this.attr(''),
            ContactPhone: this.attr(''),
            Allocations: this.hasMany(Allocation, 'Purpose_id', 'Id'),
            GadgetIds: this.attr(null),
            Gadgets: this.hasManyBy(Gadget, 'GadgetIds', 'Id')
        };
    }
}
// This is the name used as module name of the Vuex Store.
Allocationpurpose.entity = 'AllocationPurposes';
Allocationpurpose.primaryKey = 'Id';
//# sourceMappingURL=AllocationpurposeModel.js.map