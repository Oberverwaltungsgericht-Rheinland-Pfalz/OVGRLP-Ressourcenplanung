export interface AllocationModel {
  Id: number
  From: string
  To: string
  IsAllDay: boolean
  Status: number
  CreatedBy: string
  CreatedAt: string
  LastModified: string
  LastModifiedBy: string
  ApprovedBy: string
  ApprovedAt: string
  ReferencePerson: string
}
export interface AllocationShortModel {
  Id: number
  Title: string
  Timespan: string
  Notes: string
  ContactPhone: string
  Gadgets: string
}
