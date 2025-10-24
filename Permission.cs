namespace App;

enum Permission
{
  None,
  ViewMyJournal,
  ViewMySchedule,
  RequestAppointment,
  ViewPermissionList,
  HandleAccount,
  // - Create Account
  HandleRegistration,
  // - Accept 
  // - Deny   
  HandleAppointment,
  // - Register
  // - Modify 
  // - Approve
  JournalEntries,
  // - Mark with Read Permission
  AddLocation,
  // - View Patient Journal
  ScheduleOfLocation,
  AssignRegion,
  PermHandlePerm,
}

