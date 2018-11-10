using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using TwitchLib;
using TwitchLib.Models.Client;
using TwitchLib.Events.Client;
using Newtonsoft.Json;
using System.IO;
using TwitchLib.Models.API.v5.Users;
using TwitchLib.Models.API.v5.Channels;
using System.Threading.Tasks;
using Nox_Bot.Classes;
namespace Nox_Bot
{
    public partial class MainForm : Form
    {
        #region assingingStuff
        string pathForPoints = Application.StartupPath + "\\Points.json";

        string pathForBlacklistedMessages = Application.StartupPath + "\\Blacklist.json";
        static int iForMsgs = 0;


        static string pathForCommandsList = Application.StartupPath + "\\Commands.json";

        static string pathForRandomMsgList = Application.StartupPath + "\\RandomMessages.json";

        public List<ViewerDatabase> usersPoints = new List<ViewerDatabase>();
        public List<CommandsList> commandsList = new List<CommandsList>();
        public List<RandomMessage> randomMsgList = new List<RandomMessage>();

        TwitchLib.Models.API.v3.Follows.Follows followerResult;

        public TwitchClient client = new TwitchClient(new ConnectionCredentials("noxscourgebot", "oauth:lbimtgej6cxj3en1748x8zsmlwuho5"), "noxscourge",'!','!',false,null,true);

        public MainForm()
        {
            InitializeComponent();
            TwitchAPI.Settings.ClientId = "041pi5vvek3d189pwg5bn7rvtqyhlk";
            TwitchAPI.Settings.AccessToken = "h9ghvl6bux0ju5em55y42idfysqyqh";

            client.OnChatCommandReceived += onChatCommandReceived;
            client.Connect();
            LoadViewers();

        }

        private void onChatCommandReceived(object sender, OnChatCommandReceivedArgs e)
        {
            switch (e.Command.Command)
            {

                case "komande":
                    if (e.Command.ChatMessage.UserType == TwitchLib.Enums.UserType.Moderator || e.Command.ChatMessage.UserType == TwitchLib.Enums.UserType.Broadcaster)
                        client.SendWhisper(e.Command.ChatMessage.DisplayName, "Lista običnih i moderatorskih komandi : !gamble, " +
                            "!points, !komande, !following , !uptime, !apoints(MOD), !rpoints(MOD), !acom (MOD) , !ecom (MOD) , !rcom (MOD) , !amsg (MOD) , !rmsg (MOD) !");
                    else
                        client.SendWhisper(e.Command.ChatMessage.DisplayName, "Lista komandi : !gamble , " +
                            "!points , !komande , !following , !uptime.");

                    break;
                
                case "uptime":

                        if(!isStreamLive)
                            client.SendMessage("@" + e.Command.ChatMessage.DisplayName + " - Stream nije još online! Zaprati nas na !fb da saznaš kad Nox planira stream!");
                        else if (streamUptime!= null)
                            client.SendMessage(UptimeString());

                    break;
                case "points":
                    string username = e.Command.ChatMessage.Username;
                    int points = GetUserPoints(username);
                    if (points == 0 || points < 0)
                        client.SendWhisper(e.Command.ChatMessage.Username, "Trenutno nemaš poena, svakih 10 minuta gledanja streama dobijaš po 1 poen! ");
                    else
                        client.SendWhisper(e.Command.ChatMessage.Username, "Trenutno imaš " + GetUserPoints(e.Command.ChatMessage.Username).ToString() + " poena. SeemsGood");

                    break;
                case "gamble":

                    int pointsToGamble = 0;
                    int currPoints = GetUserPoints(e.Command.ChatMessage.DisplayName);

                    if (int.TryParse(e.Command.ArgumentsAsList[0], out pointsToGamble) && pointsToGamble > 0)
                    {
                        
                        if (currPoints >= pointsToGamble)
                        {
                            GambleResult(e.Command.ChatMessage.Username, pointsToGamble);
                        }
                        else
                            client.SendWhisper(e.Command.ChatMessage.Username, "Nemaš dovoljno poena za kockanje!");
                    }
                    break;
                case "apoints":
                    if (e.Command.ChatMessage.UserType == TwitchLib.Enums.UserType.Moderator || e.Command.ChatMessage.UserType == TwitchLib.Enums.UserType.Broadcaster)
                    {
                        if (e.Command.ArgumentsAsList.Count == 2)
                        {
                            string usernameToAddPoints = e.Command.ArgumentsAsList[0];
                            if (usernameToAddPoints != e.Command.ChatMessage.DisplayName)
                            {
                                if (int.TryParse(e.Command.ArgumentsAsList[1], out int pointsToAdd))
                                {
                                    if (CheckIfUserFollowing(usernameToAddPoints))
                                    {
                                        AddPoints(usernameToAddPoints, pointsToAdd);
                                        client.SendWhisper(usernameToAddPoints, "Dodato ti je " + pointsToAdd.ToString() + " poena od strane " + e.Command.ChatMessage.DisplayName + " .");
                                        client.SendWhisper(e.Command.ChatMessage.DisplayName, "Dodao si " + pointsToAdd.ToString() + " poena korisniku " + usernameToAddPoints + " .");
                                    }
                                }
                            }
                            else
                                client.SendWhisper(usernameToAddPoints, "Ne možeš sebi dodavati poene!");
                        }
                        else
                            client.SendWhisper(e.Command.ChatMessage.DisplayName, "Korišćenje komande !apoints userName kolicinaPoenaZaDodati.");
                    }
                    break;
                case "rpoints":
                    if (e.Command.ChatMessage.UserType == TwitchLib.Enums.UserType.Moderator || e.Command.ChatMessage.UserType == TwitchLib.Enums.UserType.Broadcaster)
                    {
                        if (e.Command.ArgumentsAsList.Count == 2)
                        {
                            string usernameToRPoints = e.Command.ArgumentsAsList[0];
                            if (usernameToRPoints != e.Command.ChatMessage.DisplayName)
                            {
                                if (int.TryParse(e.Command.ArgumentsAsList[1], out int pointsToR))
                                {
                                    if (CheckIfUserFollowing(usernameToRPoints))
                                    {
                                        client.SendWhisper(usernameToRPoints, "Oduzeto ti je " + pointsToR.ToString() + " poena od strane " + e.Command.ChatMessage.DisplayName + " .");
                                        client.SendWhisper(e.Command.ChatMessage.DisplayName, "Oduzeo si " + pointsToR.ToString() + " poena korisniku " + usernameToRPoints + " .");
                                        RemovePoints(usernameToRPoints, pointsToR);
                                    }
                                }
                            }
                            else
                                client.SendWhisper(usernameToRPoints, "Ne možeš sebi dodavati poene!");
                        }
                        else
                            client.SendWhisper(e.Command.ChatMessage.DisplayName, "Korišćenje komande !rpoints userName kolicinaPoenaZaOduzeti.");
                    }
                    break;
                case "following":
                    string following = GetDaysFollowing(e.Command.ChatMessage.DisplayName);
                    if (following != null)
                        client.SendWhisper(e.Command.ChatMessage.DisplayName, following);
                    else
                        client.SendWhisper(e.Command.ChatMessage.DisplayName, "Ne pratiš NoxScourge-a , a trebalo bi ! OpieOP");
                    break;
                case "acom":
                    if (e.Command.ChatMessage.UserType == TwitchLib.Enums.UserType.Moderator || e.Command.ChatMessage.UserType == TwitchLib.Enums.UserType.Broadcaster)
                    {
                        if (e.Command.ArgumentsAsList.Count == 2)
                        {
                            string makerOfCommand = e.Command.ChatMessage.DisplayName;
                            string nameOfCommand = e.Command.ArgumentsAsList[0];
                            string textOfCommand = e.Command.ArgumentsAsList[1];
                            AddCommand(nameOfCommand, textOfCommand, makerOfCommand);

                        }
                        else
                            client.SendWhisper(e.Command.ChatMessage.DisplayName, "Korišćenje komande !acom imeKomande tekstKomande.");
                    }
                    break;
                case "ecom":
                    if (e.Command.ChatMessage.UserType == TwitchLib.Enums.UserType.Moderator || e.Command.ChatMessage.UserType == TwitchLib.Enums.UserType.Broadcaster)
                    {
                        if (e.Command.ArgumentsAsList.Count == 2)
                        {
                            string makerOfCommand = e.Command.ChatMessage.DisplayName;
                            string nameOfCommand = e.Command.ArgumentsAsList[0];
                            string textOfCommand = e.Command.ArgumentsAsList[1];

                            EditCommand(nameOfCommand, textOfCommand, makerOfCommand);

                        }
                        else
                            client.SendWhisper(e.Command.ChatMessage.DisplayName, "Korišćenje komande !ecom imeKomande noviTekstKomande.");
                    }
                    break;
                case "rcom":
                    if (e.Command.ChatMessage.UserType == TwitchLib.Enums.UserType.Moderator || e.Command.ChatMessage.UserType == TwitchLib.Enums.UserType.Broadcaster)
                    {
                        if (e.Command.ArgumentsAsList.Count == 1)
                        {
                            string nameOfCommand = e.Command.ArgumentsAsList[0];
                            DelCommand(nameOfCommand);
                        }
                        else
                            client.SendWhisper(e.Command.ChatMessage.DisplayName, "Korišćenje komande !rcom imeKomande.");
                    }
                    break;
                case "amsg":
                    if (e.Command.ChatMessage.UserType == TwitchLib.Enums.UserType.Moderator || e.Command.ChatMessage.UserType == TwitchLib.Enums.UserType.Broadcaster)
                    {
                        if (e.Command.ArgumentsAsList.Count == 2)
                        {
                            string msgName = e.Command.ArgumentsAsList[0];
                            string msgText = e.Command.ArgumentsAsList[1];
                            string usernameToWhisper = e.Command.ChatMessage.DisplayName;
                            AddRandomMessage(msgName, msgText, usernameToWhisper);
                        }
                        else
                            client.SendWhisper(e.Command.ChatMessage.DisplayName, "Korišćenje komande !amsg imePoruke tekstPoruke.");
                    }
                    break;
                case "rmsg":
                    if (e.Command.ChatMessage.UserType == TwitchLib.Enums.UserType.Moderator || e.Command.ChatMessage.UserType == TwitchLib.Enums.UserType.Broadcaster)
                    {
                        
                        if (e.Command.ArgumentsAsList.Count == 1)
                        {
                            string msgName = e.Command.ArgumentsAsList[0];
                            string usernameToWhisper = e.Command.ChatMessage.DisplayName;
                            DeleteRandomMessage(msgName, usernameToWhisper);
                        }
                        else
                            client.SendWhisper(e.Command.ChatMessage.DisplayName, "Korišćenje komande !rmsg imePoruke.");
                    }
                    break;
            }
            
            for (int i = 0; i < commandsList.Count; i++)
            {
                if(commandsList[i].NameOfCommand == e.Command.Command)
                {

                        client.SendMessage(commandsList[i].TextOfCommand);
                }
            }

        }
        #endregion
        
        #region Gets&Sets&Checks
        private async void GetUserFollow(string username)
        {
            followerResult = await TwitchAPI.Follows.v3.GetFollowsStatusAsync(username, "noxscourge");
        }
        private bool CheckIfUserFollowing(string username)
        {
            GetUserFollow(username);
            if (followerResult != null)
            {
                return true;
            }
            return false;
        }
        private string GetDaysFollowing(string userName)
        {
            GetUserFollow(userName);
            if (followerResult != null)
            {
                DateTime followDate = followerResult.CreatedAt.Date;
                DateTime currentDate = DateTime.Now;
                TimeSpan resultDate = (currentDate - followDate);
                string daysFollowing = resultDate.Days.ToString();
                string hoursFollowing = resultDate.Hours.ToString();
                string minutesFollowing = resultDate.Minutes.ToString();
                string secondsFollowing = resultDate.Seconds.ToString();
                return string.Format("Ukupno pratiš NoxScourge-a : " + daysFollowing
                    + " dana , " + hoursFollowing + " sati , " + minutesFollowing + " minuta , " + secondsFollowing + " sekundi ! PogChamp");
            }
            return null;
        }
        
        #region StreamUptime
        static bool isStreamLive
        {
            get
            {
                try
                {
                    return TwitchAPI.Streams.v5.BroadcasterOnlineAsync(Globals.twitchChannelId.ToString()).Result;
                }
                catch { }
                return false;
            }
        }

        private static TimeSpan? streamUptime
        {
            get
            {
                return TwitchAPI.Streams.v5.GetUptimeAsync(Globals.twitchChannelId.ToString()).Result;
            }
        }
        private string UptimeString()
        {
            string
                hours = streamUptime.Value.Hours.ToString(),
                minutes = streamUptime.Value.Minutes.ToString();

            return string.Format("Stream je online ukupno - {0} sata, {1} minuta! PogChamp",
                hours,minutes);

        }
        #endregion  
        private string GetUserId(string username)
        {
            User[] users = TwitchAPI.Users.v5.GetUserByNameAsync(username).Result.Matches;
            if (users == null || users.Length == 0)
                return null;
            else
                return users[0].Id;
        }
        private long GetChannelId(string channel)
        {
            ChannelAuthed channelAuthed = TwitchAPI.Channels.v5.GetChannelAsync(client.ConnectionCredentials.TwitchOAuth).Result;
            if (channelAuthed == null)
                return -1;
            else
                return channelAuthed.Id;
        }
        public bool UserExists(string username)
        {

            bool t = false;
            try
            {
                for (int i = 0; i < usersPoints.Count; i++)
                {
                    if (usersPoints[i].Username == username)
                    {
                        t = true;
                        break;
                    }
                }
                return t;
            }
            catch(NullReferenceException n)
            {
                return t;
            }
        }
        public int GetUserPoints(string username)
        {
            var user = usersPoints.Find(x => x.Username.Equals(username));
            if (user == null)
                return 0;
            return user.Points;
        }
        #endregion

        #region ChatterMethods
        private void LoadViewers()
        {
            chatViewersListBox.DataSource = TwitchAPI.Undocumented.GetChattersAsync("noxscourge").Result;
            chatViewersListBox.DisplayMember = "Username";
        }
        private void buttonReloadViewers_Click(object sender, EventArgs e)
        {
            LoadViewers();
        }
        #endregion

        
        #region Events
        private void buttonExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        #region moveTopPanelWithMouse
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [System.Runtime.InteropServices.DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();
        public void topPanel_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }
        #endregion

        private void buttonMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

       
        private void iconButton_Click(object sender, EventArgs e)
        {
            Process.Start("https://www.twitch.tv/noxscourge");
        }

        private void buttonAddPoints_Click(object sender, EventArgs e)
        {
            if (numericForPoints.Value == 0)
                return;

            AddPoints(chatViewersListBox.Text, (int)numericForPoints.Value);
            client.SendWhisper(chatViewersListBox.Text, "Dodato ti je " + numericForPoints.Value.ToString() + " poena od strane NoxScourge Bot-a. OpieOP");

        }

        private void AddPoints(string username, int points)
        {
            if (points <= 0)
                return;

            ViewerDatabase userToOperate = new ViewerDatabase()
            {
                Username = username,
                Points = points
            };
            if (UserExists(username))
            {
                int index = usersPoints.IndexOf(userToOperate) + 1;
                usersPoints[index].Points += userToOperate.Points;
            }
            else
                usersPoints.Add(userToOperate);

            SerializePoints(usersPoints);
            
        }
        private void RemovePoints(string username, int points)
        {
            if (points > 0)
            {
                ViewerDatabase userToOperate = new ViewerDatabase();
                userToOperate.Username = username;
                userToOperate.Points = points;
                if (UserExists(userToOperate.Username))
                {
                    int index = usersPoints.IndexOf(userToOperate) + 1;
                    usersPoints[index].Points -= userToOperate.Points;
                    if (usersPoints[index].Points < 0)
                        usersPoints[index].Points = 0;
                }
                else
                    usersPoints.Add(userToOperate);

                SerializePoints(usersPoints);
            }
        }
        private void buttonRemovePoints_Click(object sender, EventArgs e)
        {
            if (numericForPoints.Value == 0)
                return;
            RemovePoints(chatViewersListBox.Text, (int)numericForPoints.Value);
            client.SendWhisper(chatViewersListBox.Text, "Oduzeto ti je " + numericForPoints.Value.ToString() + " poena od strane NoxScourge Bot-a. BibleThump");

        }
        private void MainForm_Load(object sender, EventArgs e)
        {
            Load_ViewerDatabaseToList();
            Load_UsersToGrid();
        }

        private void Load_UsersToGrid()
        {
            var source = new BindingSource(usersPoints, null);
            pointsDataGrid.DataSource = source;

        }

        private void buttonPointsTable_Click(object sender, EventArgs e)
        {
            MainMenuShowing(false);
            PointsTableShowing(true);
        }
        private void MainMenuShowing(bool showOrNot)
        {
            if(showOrNot)
            {
                panel1.Show();
                panel2.Show();
                panel3.Show();
                panel4.Show();
                buttonPointsTable.Show();
                chatViewersListBox.Show();
                chatWindow.Show();
                buttonRemovePoints.Show();
                buttonReloadViewers.Show();
                buttonAddPoints.Show();
                numericForPoints.Show();
                label2.Show();
                label3.Show();
            }
            else
            {
                panel1.Hide();
                panel2.Hide();
                panel3.Hide();
                panel4.Hide();
                buttonPointsTable.Hide();
                chatViewersListBox.Hide();
                chatWindow.Hide();
                buttonRemovePoints.Hide();
                buttonReloadViewers.Hide();
                buttonAddPoints.Hide();
                numericForPoints.Hide();
                label2.Hide();
                label3.Hide();
            }
        }
        private void PointsTableShowing(bool show)
        {
            if(show)
            {
                buttonMainMenu.Show();
                pointsDataGrid.Show();
            }
            else
            {
                buttonMainMenu.Hide();
                pointsDataGrid.Hide();
            }
        }
        private void buttonMainMenu_Click(object sender, EventArgs e)
        {
            MainMenuShowing(true);
            PointsTableShowing(false);
        }

        #endregion

        #region DataSaving
        private void SerializePoints(List<ViewerDatabase> usersPoints)
        {
            string output = JsonConvert.SerializeObject(usersPoints);

            using (StreamWriter sw = new StreamWriter(pathForPoints))
            {
                JsonSerializer serializer = new JsonSerializer();

                if (File.Exists(pathForPoints))
                {

                    serializer.Serialize(sw, usersPoints);
                }
                else
                {
                    File.Create(pathForPoints);
                    serializer.Serialize(sw, usersPoints);
                }
            }
        }
        private void DeserializePoints()
        {
            if (File.Exists(pathForPoints))
            {
                using (StreamReader sR = new StreamReader(pathForPoints))
                {
                    string json = sR.ReadToEnd();
                    usersPoints = JsonConvert.DeserializeObject<List<ViewerDatabase>>(json);
                }
                if (usersPoints == null)
                    usersPoints = new List<ViewerDatabase>();

            }
            else
            {
                usersPoints = new List<ViewerDatabase>();
            }
        }
        private void SerializeCommandList(List<CommandsList> commandsList)
        {
            string output = JsonConvert.SerializeObject(commandsList);

            using (StreamWriter sw = new StreamWriter(pathForCommandsList))
            {
                JsonSerializer serializer = new JsonSerializer();

                if (File.Exists(pathForCommandsList))
                {

                    serializer.Serialize(sw, commandsList);
                }
                else
                {
                    File.Create(pathForCommandsList);
                    serializer.Serialize(sw, commandsList);
                }
            }
        }
        private void DeserializeCommandList()
        {
            if (File.Exists(pathForCommandsList))
            {
                using (StreamReader sR = new StreamReader(pathForCommandsList))
                {
                    string json = sR.ReadToEnd();
                    commandsList = JsonConvert.DeserializeObject<List<CommandsList>>(json);
                }


            }
            else
            {
                commandsList = new List<CommandsList>();
            }
        }
        private void SerializeRandomMsgList(List<RandomMessage> randomMsgList)
        {
            string output = JsonConvert.SerializeObject(randomMsgList);

            using (StreamWriter sw = new StreamWriter(pathForRandomMsgList))
            {
                JsonSerializer serializer = new JsonSerializer();

                if (File.Exists(pathForRandomMsgList))
                {

                    serializer.Serialize(sw, randomMsgList);
                }
                else
                {
                    File.Create(pathForRandomMsgList);
                    serializer.Serialize(sw, randomMsgList);
                }
            }
        }
        private void DeserializeRandomMsgList()
        {
            if (File.Exists(pathForRandomMsgList))
            {
                using (StreamReader sR = new StreamReader(pathForRandomMsgList))
                {
                    string json = sR.ReadToEnd();
                    randomMsgList = JsonConvert.DeserializeObject<List<RandomMessage>>(json);
                }


            }
            else
            {
                randomMsgList = null;
                randomMsgList = new List<RandomMessage>();
            }
        }
        private void Load_ViewerDatabaseToList()
        {
            DeserializeStuff();
            if (usersPoints == null)
            {
                usersPoints = new List<ViewerDatabase>();
            }
        }

        private void DeserializeStuff()
        {
            DeserializePoints();
            DeserializeCommandList();
            DeserializeRandomMsgList();
        }




        #endregion

        #region PointsTimer
        private void timerForPoints_Tick(object sender, EventArgs e)
        {
            for(int i = 0; i< chatViewersListBox.Items.Count; i++)
            {
                chatViewersListBox.SelectedIndex = i;
                AddPoints(chatViewersListBox.Text, 1);
            }
        }

        #endregion

        #region GambleSystem
        private Random r = new Random();
        void GambleResult(string userName, int points)
        {

            if (!CheckIfUserFollowing(userName))
            {
                client.SendWhisper(userName, "Zaprati Noxa pa ćeš moći da se kockaš!");
                return;
            }

            int number = r.Next(100);
            int winnings = points;
            if (number >= 60 && number < 95)
            {
                winnings *= 2;

                AddPoints(userName, winnings);

                client.SendWhisper(userName, "Izvukao si broj " + number.ToString() + " | Tvoj dobitak je " + winnings.ToString() + ".");
                return;
            }
            else if (number <= 100 && number >= 95)
            {
                winnings *= 3;
                AddPoints(userName, winnings);
                client.SendWhisper(userName, "Izvukao si broj " + number.ToString() + " | Tvoj dobitak je " + winnings.ToString() + ".");
                return;
            }
            else
            {
                RemovePoints(userName, winnings);
                client.SendWhisper(userName, "Izvukao si broj " + number.ToString() + " | Izgubio si svoj ulog.");
                return;
            }
            

        }
        #endregion

        #region Commands
        private void AddCommand(string nameOfCommand, string textOfCommand, string creatorOfCommand)
        {

            CommandsList cList = new CommandsList()
            {
                NameOfCommand = nameOfCommand,
                TextOfCommand = textOfCommand,
                MakerOfCommand = creatorOfCommand,
                Deletable = true
            };
            if (commandsList != null)
            {
                CommandsList check;

                check = commandsList.Find(x => x.NameOfCommand.Equals(nameOfCommand));

                if (check != null && !check.Deletable)
                {
                    client.SendWhisper(creatorOfCommand, "Komanda pod tim imenom već postoji!");

                }
                else
                {
                    commandsList.Add(cList);
                    client.SendWhisper(creatorOfCommand, "Uspešno si napravio komandu : !" + nameOfCommand);
                    SerializeCommandList(commandsList);
                }
            }
            else
            {
                
                commandsList.Add(cList);
                client.SendWhisper(creatorOfCommand, "Uspešno si napravio komandu : !" + nameOfCommand);
                SerializeCommandList(commandsList);
            }
        }
        private void EditCommand(string nameOfCommand, string textOfCommand,string creatorOfCommand)
        {
            CommandsList editCommand = new CommandsList()
            {
                NameOfCommand = nameOfCommand,
                TextOfCommand = textOfCommand,
                MakerOfCommand = creatorOfCommand,
                Deletable = true
            };
            CommandsList editCom = commandsList.Find(x => x.NameOfCommand.Equals(nameOfCommand));
            if(editCom != null && editCom.Deletable)
            {
                commandsList[commandsList.IndexOf(editCom)] = editCommand;
                SerializeCommandList(commandsList);
                client.SendMessage("Komanda !" + nameOfCommand + " uspešno izmenjena.");
            }
            else
                client.SendMessage("Komanda ne postoji ili se ne može editovati!");
        }
        private void DelCommand(string nameOfCommand)
        {
            CommandsList delComm = commandsList.Find(x => x.NameOfCommand.Equals(nameOfCommand));
            if(delComm != null && delComm.Deletable)
            {
                commandsList.RemoveAt(commandsList.IndexOf(delComm));
                SerializeCommandList(commandsList);
                client.SendMessage("Komanda !" + nameOfCommand + " uspešno izbrisana.");
            }
            else
                client.SendMessage("Komanda ne postoji ili se ne može obrisati!");
        }
        #endregion

        #region RandomMessages
        private void timerRandomMessages_Tick(object sender, EventArgs e)
        {
            SendRandomMessage();
            
        }

        private void SendRandomMessage()
        {
            string messageToSend = PickRandomMessageFromList();
            iForMsgs++;
            
            client.SendMessage(messageToSend);
        }

        private string PickRandomMessageFromList()
        {
            if (iForMsgs > randomMsgList.Count -1)
                iForMsgs = 0;
            if (randomMsgList.Count == 1)
                return randomMsgList[0].MessageSays;
            else
            {
                return randomMsgList[iForMsgs].MessageSays;
            }
        }
        private void AddRandomMessage(string nameOfMessage, string textForMessage,string usernameToWhisper)
        {
            RandomMessage msgToAdd = new RandomMessage()
            {
                MessageName = nameOfMessage,
                MessageSays = textForMessage,
                Cooldown = null
            };
            if (randomMsgList != null)
            {
                RandomMessage check;

                check = randomMsgList.Find(x => x.MessageName.Equals(nameOfMessage));

                if (check != null)
                {
                    client.SendWhisper(usernameToWhisper, "Random poruka pod tim imenom već postoji!");

                }
                else
                {
                    randomMsgList.Add(msgToAdd);
                    client.SendWhisper(usernameToWhisper, "Uspešno si napravio random poruku sa sledećim textom : " + textForMessage + " , pod imenom "+nameOfMessage+" .");
                    SerializeRandomMsgList(randomMsgList);
                }
            }
            else
            {

                randomMsgList.Add(msgToAdd);
                client.SendWhisper(usernameToWhisper, "Uspešno si napravio random poruku sa sledećim textom :\" " + textForMessage + " \" , pod imenom " + nameOfMessage + " .");
                SerializeRandomMsgList(randomMsgList);
            }
        }
        private void DeleteRandomMessage(string nameOfMessage, string usernameToWhisper)
        {

            RandomMessage delMsg = randomMsgList.Find(x => x.MessageName.Equals(nameOfMessage));
            if (delMsg != null)
            {
                randomMsgList.RemoveAt(randomMsgList.IndexOf(delMsg));
                SerializeRandomMsgList(randomMsgList);
                client.SendMessage("Random poruka " + nameOfMessage + " uspešno izbrisana.");
            }
            else
                client.SendMessage("Random poruka pod tim imenom ne postoji!");
        }
        #endregion
    }


    #region Classes
    public class ViewerDatabase
    {
        public string Username { get; set; }
        public int Points { get; set; }
    }
    public class RandomMessage
    {
        public string MessageName { get; set; }
        public string MessageSays { get; set; }
        public int? Cooldown { get; set; }
    }
    
    #endregion

}
