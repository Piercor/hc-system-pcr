namespace App;

enum Permission
{
  None,
  ViewMyJournal,
  ViewMySchedule,
  RequestAppointment,
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
  ViewPermissionList,
  PermHandlePerm,
  Logout,
  Quit,
}

