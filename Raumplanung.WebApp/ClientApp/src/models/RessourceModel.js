import { Model } from '@vuex-orm/core';
import AllocationModel from './AllocationModel';
export default class Ressource extends Model {
    // List of all fields (schema) of the post model. `this.attr` is used
    // for the generic field type. The argument is the default value.
    static fields() {
        return {
            Id: this.attr(null),
            Name: this.attr(''),
            Type: this.attr('Raum'),
            FunctionDescription: this.attr(''),
            SpecialsDescription: this.attr(''),
            allocations: this.hasMany(AllocationModel, 'Ressource_id')
        };
    }
}
// This is the name used as module name of the Vuex Store.
Ressource.entity = 'ressources';
Ressource.primaryKey = 'Id';
//# sourceMappingURL=RessourceModel.js.map