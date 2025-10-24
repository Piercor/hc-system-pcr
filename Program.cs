using System.Diagnostics;
using System.Reflection;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using App;

HCSystem sys = new();
User? activeUser = null;
Menu currentMenu = Menu.Default;

bool isRunning = true;
while (isRunning)
{
  if (activeUser == null)
  {
    switch (currentMenu)
    {
      case Menu.Default:
        try { Console.Clear(); } catch { }
        Console.WriteLine("\n[1] Login \n[2] Request registration as a patient\n[3] Quit");
        Console.Write("\n► ");
        string? input = Console.ReadLine();

        switch (input)
        {
          case "1":
            Console.Write("\nPlease input your SSN: ");
            string? ssn = Console.ReadLine();
            Console.Write("\nPlease input a password: ");
            string? password = Console.ReadLine();

            Debug.Assert(ssn != null);
            Debug.Assert(password != null);

            foreach (User user in sys.users)
            {
              if (user.TryLogin(ssn, password))
              {
                activeUser = user;
                currentMenu = Menu.Main;
                break;
              }
            }
            break;
          case "2":

            bool foundSSN = false;

            Console.Write("\nPlease input your SSN: ");
            string? newSSN = Console.ReadLine()?.Trim();

            if (string.IsNullOrWhiteSpace(newSSN))
            {
              Console.WriteLine("\nInvalid input");
              Console.ReadKey(true);
              break;
            }

            // int newSSNlenght = newSSN.Length;

            foreach (Event events in sys.eventList)
            {
              if (events.Title.StartsWith(newSSN))
              {
                Console.WriteLine("\nThere is already a patient request with the given SSN.");
                Console.Write("\nPress ENTER to go back to previous menu. ");
                Console.ReadKey(true);
                foundSSN = true;
                break;
              }
            }

            if (!foundSSN)
            {
              Console.Write("\nPlease input an email: ");
              string? newEmail = Console.ReadLine();
              Console.Write("\nWhat is your name? ");
              string? newName = Console.ReadLine();
              if (string.IsNullOrWhiteSpace(newName))
              {
                Console.WriteLine("\nInvalid input");
                Console.ReadKey(true);
                break;
              }
              Debug.Assert(newSSN != null);
              Debug.Assert(newEmail != null);
              Debug.Assert(newName != null);

              string newDescription = $"{newSSN} request to be a patient. Name: {newName} - Email: {newEmail}";
              Event? newEvent = new($"{newSSN} PatientRequest", Event.EventType.Request);
              newEvent.Description = newDescription;

              sys.eventList.Add(newEvent);
              sys.SaveEventsToFile();

              Console.WriteLine($"\nYour request have been registered. We'll let you know at {newEmail} when we have made a decision.");
              Console.Write("\nPress ENTER to continue. ");
              Console.ReadKey(true);
            }
            break;

          case "3":
            isRunning = false;
            break;

          case "CheatersNeverLearn":
            sys.CheatersDelight();
            break;

          default:
            Console.Write("\nPlease enter a valid input. ");
            Console.ReadKey(true);
            break;
        }
        break;
    }
  }
  else
  {
    switch (currentMenu)
    {
      case Menu.Main:

        try { Console.Clear(); } catch { }
        Console.WriteLine($"\nWelcome, {activeUser?.Name}\n");

        Dictionary<string, Permission> menuOptions = new();
        int index = 1;

        Debug.Assert(activeUser != null);
        // foreach (Permission permission in activeUser.Permissions)
        // {
        for (int i = 1; i < activeUser.Permissions.Count - 3; ++i)
        {
          menuOptions[index.ToString()] = activeUser.Permissions[i];
          string menuText = $"[{index}] ";

          switch (activeUser.Permissions[i])
          {
            case Permission.ViewMyJournal:
              menuText += "View my journal.";
              break;
            case Permission.ViewMySchedule:
              menuText += "View my schedule.";
              break;
            case Permission.RequestAppointment:
              menuText += "Request an appointment.";
              break;

            case Permission.HandleAccount:
              menuText += "Handle accounts.";
              break;
            case Permission.HandleRegistration:
              menuText += "Handle registrations.";
              break;
            case Permission.HandleAppointment:
              menuText += "Handle appointments.";
              break;
            case Permission.JournalEntries:
              menuText += "View patients journals.";
              break;
            case Permission.AddLocation:
              menuText += "Add a location.";
              break;
            case Permission.ScheduleOfLocation:
              menuText += "Schedule of a location.";
              break;
            case Permission.AssignRegion:
              menuText += "Assing user to region.";
              break;
            case Permission.ViewPermissionList:
              menuText += "View permissions.";
              break;
              // case Permission.Logout:
              //   menuText += "Logout.";
              //   break;
              // case Permission.Quit:
              //   menuText += "Quit.";
              //   break;
          }
          Console.WriteLine(menuText);
          index += 1;
        }

        // menuOptions[index.ToString()] = Permission.Logout;
        Console.WriteLine($"[X] Log out.");
        index += 1;
        // menuOptions[index.ToString()] = Permission.Quit;
        Console.WriteLine($"[Q] Quit.");

        Console.Write("\n► ");
        string? menuInput = Console.ReadLine();
        Debug.Assert(menuInput != null);

        if (menuInput.ToLower() == "x")
        {
          activeUser = null;
          currentMenu = Menu.Default;
        }
        else if (menuInput.ToLower() == "q")
        { isRunning = false; }
        else if (!menuOptions.ContainsKey(menuInput))
        {
          Console.Write("\nInvalid input. Press ENTER to continue. ");
          Console.ReadKey(true);
        }
        else
        {
          switch (menuOptions[menuInput])
          {
            case Permission.ViewMyJournal:
              try { Console.Clear(); } catch { }
              Debug.Assert(activeUser != null);
              sys.ViewEvent(Event.EventType.Entry, activeUser);
              Console.ReadKey(true);
              break;
            case Permission.ViewMySchedule:
              try { Console.Clear(); } catch { }
              Debug.Assert(activeUser != null);
              sys.ViewEvent(Event.EventType.Appointment, activeUser);
              Console.ReadKey(true);
              break;
            case Permission.RequestAppointment:
              try { Console.Clear(); } catch { }
              sys.RequestAppointment(activeUser);
              break;
            case Permission.HandleAccount:
              try { Console.Clear(); } catch { }
              sys.CreateAccount();
              break;
            case Permission.HandleRegistration:
              try { Console.Clear(); } catch { }
              sys.ViewUserRequests();
              break;
            case Permission.HandleAppointment:
              try { Console.Clear(); } catch { }
              sys.HandleAppointment();
              Console.ReadKey(true);
              break;
            case Permission.AddLocation:
              try { Console.Clear(); } catch { }
              sys.AddLocation();
              Console.ReadKey(true);
              break;
            case Permission.ScheduleOfLocation:
              try { Console.Clear(); } catch { }
              sys.ScheduleOfLocation();
              Console.ReadKey(true);
              break;
            case Permission.PermHandlePerm:
              try { Console.Clear(); } catch { }
              sys.PermissionSystem(activeUser);
              break;
            case Permission.AssignRegion:
              try { Console.Clear(); } catch { }
              sys.AssignToRegion();
              Console.ReadKey(true);
              break;
          }
        }
        /* try { Console.Clear(); } catch { }
        Console.WriteLine($"\nWelcome, {activeUser?.Name}");
        Debug.Assert(activeUser != null);

        Console.WriteLine("\n[1] View My Journal");
        Console.WriteLine("\n[2] View My Schedule");
        Console.WriteLine("\n[3] Request an appointment.");
        if (!activeUser.HasPermission(Permission.None))
        {
          Console.WriteLine("\n[4] Handle Accounts");
          Console.WriteLine("\n[5] Handle Registrations");
          Console.WriteLine("\n[6] Handle Appointment");
          Console.WriteLine("\n[7] Add a Location");
          Console.WriteLine("\n[8] Schedule of a Location");
          Console.WriteLine("\n[9] View Permissions");
          Console.WriteLine("\n[10] Assign User To Region");
        }
        Console.WriteLine("\n[x] Logout");
        Console.Write("\n► ");

        switch (Console.ReadLine())
        {

          // View My Journal 
          case "1":
            try { Console.Clear(); } catch { }
            Debug.Assert(activeUser != null);
            sys.ViewEvent(Event.EventType.Entry, activeUser);
            Console.ReadKey(true);
            break;

          // View My Schedule 
          case "2":
            try { Console.Clear(); } catch { }
            Debug.Assert(activeUser != null);
            sys.ViewEvent(Event.EventType.Appointment, activeUser);
            Console.ReadKey(true);
            break;

          // Request Appointment 
          case "3":
            sys.RequestAppointment(activeUser);
            break;

          // Handle Account 
          case "4":
            try { Console.Clear(); } catch { }
            if (!activeUser!.HasPermission(Permission.HandleAccount))
            { Console.WriteLine("You do not have permission for this."); Console.ReadKey(true); break; }
            sys.CreateAccount();
            break;

          // Handle Registration 
          case "5":
            try { Console.Clear(); } catch { }
            if (!activeUser!.HasPermission(Permission.HandleRegistration))
            { Console.WriteLine("You do not have permission for this."); Console.ReadKey(true); break; }
            sys.ViewUserRequests();
            break;

          // Handle Appointment 
          case "6":
            try { Console.Clear(); } catch { }
            if (!activeUser!.HasPermission(Permission.HandleAppointment))
            { Console.WriteLine("You do not have permission for this."); Console.ReadKey(true); break; }
            sys.HandleAppointment();
            Console.ReadKey(true);
            break;

          // Add Location 
          case "7":
            try { Console.Clear(); } catch { }
            if (!activeUser!.HasPermission(Permission.AddLocation))
            { Console.WriteLine("You do not have permission for this."); Console.ReadKey(true); break; }
            sys.AddLocation();
            Console.ReadKey(true);
            break;

          // Schedule Of Location 
          case "8":
            if (!activeUser!.HasPermission(Permission.ScheduleOfLocation))
            { Console.WriteLine("You do not have permission for this."); Console.ReadKey(true); break; }
            try { Console.Clear(); } catch { }
            sys.ScheduleOfLocation();
            Console.ReadKey(true);
            break;

          // View Permissions 
          case "9":
            try { Console.Clear(); } catch { }
            if (!activeUser!.HasPermission(Permission.PermHandlePerm) && !activeUser!.HasPermission(Permission.ViewPermissionList))
            {
              Console.WriteLine("\nYou do not have permission to view others permissions.");
              Console.WriteLine("\nYour permissions are:");
              foreach (Permission perm in activeUser.Permissions)
              {
                Console.WriteLine($"\n[{activeUser.Permissions.IndexOf(perm) + 1}] {perm}");
              }
              Console.Write("\nPress ENTER to go back to previos menu. ");
              Console.ReadKey(true); break;
            }
            sys.PermissionSystem(activeUser);
            break;
          case "10":
            try { Console.Clear(); } catch { }
            if (!activeUser!.HasPermission(Permission.AssignRegion))
            { Console.WriteLine("You do not have permission for this."); Console.ReadKey(true); break; }
            sys.AssignToRegion();
            Console.ReadKey(true);
            break;
          // Log out
          case "x":
            activeUser = null;
            currentMenu = Menu.Default;
            break;

          default:
            Console.Write("\nInvalid input. Press ENTER to continue. ");
            Console.ReadKey(true);
            break;
        } */
        break;
    }
  }
}

