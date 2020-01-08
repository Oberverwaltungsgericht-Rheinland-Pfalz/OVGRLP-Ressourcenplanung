export default interface AllocationRequest {
  id: number,
  Title: string,
  Status: string,
  Description: string,
  DateTime: Date
}

export interface AllocationRequestView {
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
  ReferencePersonId: number

  PurposeTitle: string
  RessourceTitle: string
  ContactTel: string
  Description: string
  Notices: string
}
