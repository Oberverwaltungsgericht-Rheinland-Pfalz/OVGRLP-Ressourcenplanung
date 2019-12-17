import { Model } from '@vuex-orm/core';
import Ressource from './RessourceModel';
import Allocationpurpose from './AllocationpurposeModel';
export default class Allocation extends Model {
    // List of all fields (schema) of the post model. `this.attr` is used
    // for the generic field type. The argument is the default value.
    static fields() {
        return {
            Id: this.attr(null),
            From: this.attr(null),
            To: this.attr(null),
            IsAllDay: this.attr(true),
            Status: this.attr(0),
            CreatedBy: this.attr(null),
            CreatedAt: this.attr(null),
            LastModified: this.attr(null),
            LastModifiedBy: this.attr(''),
            ApprovedBy: this.attr(null),
            ApprovedAt: this.attr(null),
            ReferencePerson: this.attr(''),
            Ressource_id: this.attr(null),
            Ressource: this.belongsTo(Ressource, 'Ressource_id', 'Id'),
            Purpose: this.belongsTo(Allocationpurpose, 'Purpose_id', 'Id'),
            Purpose_id: this.attr(null)
        };
    }
}
// This is the name used as module name of the Vuex Store.
Allocation.entity = 'allocations';
Allocation.primaryKey = 'Id';
//# sourceMappingURL=AllocationModel.js.map