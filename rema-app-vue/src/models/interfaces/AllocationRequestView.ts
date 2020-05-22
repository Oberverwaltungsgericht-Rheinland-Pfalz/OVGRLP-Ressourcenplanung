export interface AllocationRequestView {
  Id: number
  From: string
  To: string
  IsAllDay: boolean
  Status: number
  CreatedAt: string
  LastModified: string
  ApprovedAt: string
  ReferencePerson: string
  ReferencePersonId: number
  RessourceId: string
  Title: string
  RessourceTitle: string
  ContactTel: string
  Description: string
  Notices: string
}

export interface ShortAllocationView {
  Id: number
  From: string
  To: string
  RessourceId: string
  dates: string[] | null
}
