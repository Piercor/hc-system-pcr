using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Reflection;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using App;

HCSystem sys = new();
User? activeUser = null;
Menu currentMenu = Menu.Default;
int menuInput = 0;

bool isRunning = true;
while (isRunning)
{
  if (activeUser == null)
  {
    switch (currentMenu)
    {
      case Menu.Default:

        bool subRunning = true;
        string selectedOption = "";
        int selectedIndex = 0;
        string[] mainOptions = ["Login", "Create account", "Quit"];
        Dictionary<string, string> menuOptions = new();
        menuOptions.Add(mainOptions[0], "1");
        menuOptions.Add(mainOptions[1], "2");
        menuOptions.Add(mainOptions[2], "3");


        // Console.WriteLine("\n[1] Login \n[2] Create account\n[3] Quit");
        // Console.Write("\n► ");
        // string? input = Console.ReadLine();
        while (subRunning)
        {
          try { Console.Clear(); } catch { }
          sys.NavigateMenu(selectedIndex, mainOptions);
          switch (Console.ReadKey().Key)
          {
            case ConsoleKey.UpArrow:
              selectedIndex--;
              if (selectedIndex < 0)
              {
                selectedIndex = mainOptions.Length - 1;
              }
              break;
            case ConsoleKey.DownArrow:
              selectedIndex++;
              if (selectedIndex >= mainOptions.Length)
              {
                selectedIndex = 0;
              }
              break;
            case ConsoleKey.Enter:
              subRunning = false;
              selectedOption = menuOptions[mainOptions[selectedIndex]];
              break;
            case ConsoleKey.C:
              subRunning = false;
              selectedOption = "CheatersNeverLearn";
              break;
          }
        }

        switch (selectedOption)
        {
          case "1":
            Console.Write("\nPlease input your SSN: ");
            string? ssn = Console.ReadLine();
            Console.Write("\nPlease input a password: ");
            string? password = Console.ReadLine();
            bool foundUser = false;
            Debug.Assert(ssn != null);
            Debug.Assert(password != null);

            foreach (User user in sys.users)
            {
              if (user.TryLogin(ssn, password))
              {
                activeUser = user;
                currentMenu = Menu.Main;
                foundUser = true;
                break;
              }
            }

            if (!foundUser)
            {
              Console.WriteLine("\nNo user was found with those credentials.");
              Console.Write("\nPress ENTER to continue. ");
              Console.ReadKey(true);
            }
            break;

          case "2":
            sys.CreateAccount();
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

        Dictionary<int, Permission> menuOptions = new();
        int index = 0;

        Debug.Assert(activeUser != null);

        string[] userMenu = new string[activeUser.Permissions.Count];

        for (int i = 0; i < activeUser.Permissions.Count; ++i)
        {
          menuOptions[index] = activeUser.Permissions[i];
          string menuText = "";

          switch (activeUser.Permissions[i])
          {
            case Permission.None:
              menuText += "Send patient request";
              break;
            case Permission.ViewMyJournal:
              menuText += "View my journal";
              break;
            case Permission.ViewMySchedule:
              menuText += "View my schedule";
              break;
            case Permission.RequestAppointment:
              menuText += "Request an appointment";
              break;
            case Permission.ViewPermissionList:
              menuText += "View your permissions";
              break;
            case Permission.HandleAccount:
              menuText += "Handle accounts";
              break;
            case Permission.HandleRegistration:
              menuText += "Handle registrations";
              break;
            case Permission.HandleAppointment:
              menuText += "Handle appointments";
              break;
            case Permission.JournalEntries:
              menuText += "Manage journals";
              break;
            case Permission.AddLocation:
              menuText += "Add a location";
              break;
            case Permission.ScheduleOfLocation:
              menuText += "Schedule of a location";
              break;
            case Permission.AssignRegion:
              menuText += "Assing user to region";
              break;
            case Permission.PermHandlePerm:
              menuText += "Manage permissions";
              break;
            case Permission.Logout:
              menuText += "Logout";
              break;
            case Permission.Quit:
              menuText += "Quit";
              break;
          }
          //Console.WriteLine(menuText);
          userMenu[i] = menuText;
          index += 1;
        }


        int subIndex = menuInput;
        bool subRunning = true;
        while (subRunning)
        {
          try { Console.Clear(); } catch { }
          Console.WriteLine($"\nWelcome, {activeUser?.Name}\n");
          sys.NavigateMenu(subIndex, userMenu);
          switch (Console.ReadKey().Key)
          {
            case ConsoleKey.UpArrow:
              subIndex--;
              if (subIndex < 0)
              {
                subIndex = userMenu.Length - 1;
              }
              break;
            case ConsoleKey.DownArrow:
              subIndex++;
              if (subIndex >= userMenu.Length)
              {
                subIndex = 0;
              }
              break;
            case ConsoleKey.Enter:
              subRunning = false;
              menuInput = subIndex;
              break;
          }
        }

        // Console.Write("\n► ");
        // string? menuInput = Console.ReadLine();
        // Debug.Assert(menuInput != null);

        // if (menuInput.ToLower() == "l")
        // {
        //   activeUser = null;
        //   currentMenu = Menu.Default;
        // }
        // else if (menuInput.ToLower() == "q")
        // { isRunning = false; }
        // else if (!menuOptions.ContainsKey(menuInput))
        // {
        //   Console.Write("\nInvalid input. Press ENTER to continue. ");
        //   Console.ReadKey(true);
        // }
        // else
        // {
        switch (menuOptions[menuInput])
        {
          case Permission.None:
            try { Console.Clear(); } catch { }
            bool foundRequest = false;
            foreach (Event events in sys.eventList)
            {
              if (events.MyEventType == Event.EventType.Request && events.Title == "PatientRequest" && events.Participants[0].User == activeUser)
              {
                foundRequest = true;
                Console.WriteLine("\nYou have already sent a patient request.");
                Console.Write("\nPress ENTER to continue.");
                Console.ReadLine();
                break;
              }
            }
            if (!foundRequest)
            {
              string newDescription = $"User with SSN '{activeUser.SSN}' and name '{activeUser.Name}' request to be a patient.";
              Event? newEvent = new("PatientRequest", Event.EventType.Request);
              newEvent.Description = newDescription;
              newEvent.Participants.Add(new(activeUser, Role.None));
              sys.eventList.Add(newEvent);
              sys.SaveEventsToFile();
              Console.Write("\nPatient request sent. Press ENTER to continue. ");
              Console.ReadLine();
            }
            break;
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
          case Permission.ViewPermissionList:
            try { Console.Clear(); } catch { }
            sys.ViewMyPermissions(activeUser);
            break;
          case Permission.HandleAccount:
            try { Console.Clear(); } catch { }
            sys.CreateAccount();
            break;
          case Permission.HandleRegistration:
            try { Console.Clear(); } catch { }
            sys.ViewUserRequests(activeUser);
            break;
          case Permission.HandleAppointment:
            try { Console.Clear(); } catch { }
            sys.HandleAppointment();
            Console.ReadKey(true);
            break;
          case Permission.JournalEntries:
            try { Console.Clear(); } catch { }
            sys.JournalEntries(activeUser);
            //Console.ReadKey(true);
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
          case Permission.AssignRegion:
            try { Console.Clear(); } catch { }
            sys.AssignToRegion();
            Console.ReadKey(true);
            break;
          case Permission.PermHandlePerm:
            try { Console.Clear(); } catch { }
            sys.PermissionSystem(activeUser);
            break;
          case Permission.Logout:
            activeUser = null;
            currentMenu = Menu.Default;
            break;
          case Permission.Quit:
            isRunning = false;
            break;
        }
        //}
        break;
    }
  }
}

