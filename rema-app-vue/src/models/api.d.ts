declare module WebApi {
    // ..\Rema.WebApi\ViewModels\AllocationFilter.cs
    export interface AllocationFilter {
        UserId: number;
        From: string;
        To: string;
    }

    // ..\Rema.WebApi\ViewModels\AllocationRequestEdition.cs
    export interface AllocationRequestEdition {
        Id: number;
        status: number;
        From?: string;
        To?: string;
        IsAllDay: Boolean;
    }

    // ..\Rema.WebApi\ViewModels\AllocationViewModel.cs
    export interface AllocationViewModel {
        Id: number;
        Title: string;
        Notes: string;
        From: string;
        To: string;
        IsAllDay: Boolean;
        ContactPhone: string;
        Status: MeetingStatus;
        RessourceIds: number[];
        GadgetsIds: number[];
        CreatedById: number;
        CreatedAt: string;
        LastModified: string;
        LastModifiedById: number;
        ApprovedById: number;
        ApprovedAt: string;
        ReferencePersonId: string;
        ScheduleSeries: string;
        HintsForSuppliers: SimpleSupplierHint[];
    }

    // ..\Rema.WebApi\ViewModels\GadgetViewModel.cs
    export interface GadgetViewModel {
        Id: number;
        Title: string;
        SuppliedBy: number;
        IsDeactivated: boolean;
    }

    // ..\Rema.WebApi\ViewModels\MultipleAllocationsViewModel.cs
    export interface MultipleAllocationsViewModel extends AllocationViewModel {
        Dates: string[];
    }

    // ..\Rema.WebApi\ViewModels\RessourceViewModel.cs
    export interface RessourceViewModel {
        Id: number;
        Name: string;
        FunctionDescription: string;
        SpecialsDescription: string;
        Type: string;
        IsDeactivated: boolean;
    }

    // ..\Rema.WebApi\ViewModels\Role.cs
    export interface Role {
        Name: string;
        Level: number;
    }

    // ..\Rema.WebApi\ViewModels\UserViewModel.cs
    export interface UserViewModel extends User {
        Roles: Role[];
        SupportGroupIds: number[];
    }

    // ..\Rema.Infrastructure\Models\MeetingStatus.cs
    export enum MeetingStatus {
        Draft = 'Draft',
        Pending = 'Pending',
        Approved = 'Approved',
        Clarification = 'Clarification',
        Moved = 'Moved',
        Hidden = 'Hidden',
        Archived = 'Archived',
        Deleted = 'Deleted',
    }

    // ..\Rema.Infrastructure\Models\User.cs
    export interface User {
        Id: number;
        ActiveDirectoryID: string;
        Name: string;
        Organisation: string;
        Email: string;
    }

    // ..\Rema.Infrastructure\Models\SupplierGroup.cs
    export interface SupplierGroup {
        Id: number;
        Title: string;
        GroupEmail: string;
        RemindTime: string;
    }

    // ..\Rema.Infrastructure\Models\SimpleSupplierHint.cs
    export interface SimpleSupplierHint {
        GroupId: number;
        Message: string;
    }

    // ..\Rema.Infrastructure\LDAP\AdUserViewModel.cs
    export interface AdUserViewModel {
        ActiveDirectoryID: string;
        Name: string;
        Email: string;
        Phone: string;
    }

    // ..\Rema.Infrastructure\ViewModels\ContactUser.cs
    export interface ContactUser {
        Id: number;
        Title: string;
        Email: string;
        Organisation: string;
    }

}