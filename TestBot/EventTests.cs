using System;
using RevoltSharp;

namespace TestBot
{
    public class EventTests
    {
        public EventTests(RevoltClient client)
        {
            client.OnChannelCreated += Client_OnChannelCreated;
            client.OnChannelDeleted += Client_OnChannelDeleted;
            client.OnChannelUpdated += Client_OnChannelUpdated;
            client.OnCurrentUserUpdated += Client_OnCurrentUserUpdated;
            client.OnGroupJoined += Client_OnGroupJoined;
            client.OnGroupLeft += Client_OnGroupLeft;
            client.OnGroupUserJoined += Client_OnGroupUserJoined;
            client.OnGroupUserLeft += Client_OnGroupUserLeft;
            client.OnMemberJoined += Client_OnMemberJoined;
            client.OnMemberLeft += Client_OnMemberLeft;
            client.OnMessageDeleted += Client_OnMessageDeleted;
            client.OnMessageRecieved += Client_OnMessageRecieved;
            client.OnMessageUpdated += Client_OnMessageUpdated;
            client.OnReady += Client_OnReady;
            client.OnRoleCreated += Client_OnRoleCreated;
            client.OnRoleDeleted += Client_OnRoleDeleted;
            client.OnRoleUpdated += Client_OnRoleUpdated;
            client.OnServerJoined += Client_OnServerJoined;
            client.OnServerLeft += Client_OnServerLeft;
            client.OnServerUpdated += Client_OnServerUpdated;
            client.OnUserUpdated += Client_OnUserUpdated;
            client.OnWebSocketError += Client_OnWebSocketError;
        }

        private void Client_OnWebSocketError(WebSocketError value)
        {
            Console.WriteLine("--- EVENT: WebSocket Error ---");
            Console.WriteLine(value.Messaage);
        }

        private void Client_OnUserUpdated(User value, User value2)
        {
            Console.WriteLine("--- EVENT: User Updated ---");
            Console.WriteLine($"{value.Username}");
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(value, Newtonsoft.Json.Formatting.Indented));
            Console.WriteLine("---");
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(value2, Newtonsoft.Json.Formatting.Indented));
        }

        private void Client_OnServerUpdated(Server value, Server value2)
        {
            Console.WriteLine("--- EVENT: Server Updated ---");
            Console.WriteLine($"{value.Name}|{value2.Name}");
        }

        private void Client_OnServerLeft(Server value)
        {
            Console.WriteLine("--- EVENT: Server Left ---");
            Console.WriteLine(value.Name);
        }

        private void Client_OnServerJoined(Server value, SelfUser value2)
        {
            Console.WriteLine("--- EVENT: Server Joined ---");
            Console.WriteLine(value.Name);
        }

        private void Client_OnRoleUpdated(Role value, Role value2)
        {
            Console.WriteLine("--- EVENT: Role Updated ---");
            Console.WriteLine($"{value.Name} | {value2.Name}");
        }

        private void Client_OnRoleDeleted(Role value)
        {
            Console.WriteLine("--- EVENT: Role Deleted ---");
            Console.WriteLine(value.Name);
        }

        private void Client_OnRoleCreated(Role value)
        {
            Console.WriteLine("--- EVENT: Role Created ---");
            Console.WriteLine(value.Name);
        }

        private void Client_OnReady(SelfUser value)
        {
            Console.WriteLine("--- EVENT: On Ready ---");
            Console.WriteLine(value.Username);
        }

        private void Client_OnMessageUpdated(Channel value, string message, string content)
        {
            Console.WriteLine("--- EVENT: Message Updated ---");
            Console.WriteLine(content);
        }

        private void Client_OnMessageRecieved(Message value)
        {
            Console.WriteLine("--- EVENT: Message Recieved ---");
            if (value is UserMessage um)
            Console.WriteLine(um.Content);
            else
                Console.WriteLine(value.Id);
        }

        private void Client_OnMessageDeleted(Channel value, string value2)
        {
            Console.WriteLine("--- EVENT: Message Deleted ---");
            Console.WriteLine(value.Id + " | " + value2);
        }

        private void Client_OnMemberLeft(Server value, ServerMember value2)
        {
            Console.WriteLine("--- EVENT: Member Left ---");
            Console.WriteLine(value.Name);
        }

        private void Client_OnMemberJoined(Server value, ServerMember value2)
        {
            Console.WriteLine("--- EVENT: Member Joined ---");
            Console.WriteLine(value.Name);
        }

        private void Client_OnGroupUserLeft(GroupChannel value, User value2)
        {
            Console.WriteLine("--- EVENT: Group User Left ---");
            Console.WriteLine(value.Name);
        }

        private void Client_OnGroupUserJoined(GroupChannel value, User value2)
        {
            Console.WriteLine("--- EVENT: Group User Joined ---");
            Console.WriteLine(value.Name);
        }

        private void Client_OnGroupLeft(GroupChannel value, SelfUser value2)
        {
            Console.WriteLine("--- EVENT: Group Left ---");
            Console.WriteLine(value.Name);
        }

        private void Client_OnGroupJoined(GroupChannel value, SelfUser value2)
        {
            Console.WriteLine("--- EVENT: Group Joined ---");
            Console.WriteLine(value.Name);
        }

        private void Client_OnCurrentUserUpdated(SelfUser value, SelfUser value2)
        {
            Console.WriteLine("--- EVENT: Current User Updated ---");
            if (value.ProfileBio == value2.ProfileBio)
                Console.WriteLine("SAME");
            else
                Console.WriteLine("CHANGED");
        }

        private void Client_OnChannelUpdated(Channel value, Channel value2)
        {
            Console.WriteLine("--- EVENT: Channel Updated  ---");
            Console.WriteLine(value.Id);
        }

        private void Client_OnChannelDeleted(Channel value)
        {
            Console.WriteLine("--- EVENT: Channel Deleted ---");
            Console.WriteLine(value.Id);
        }

        private void Client_OnChannelCreated(Channel value)
        {
            Console.WriteLine("--- EVENT: Channel Created ---");
            Console.WriteLine(value.Id);
        }
    }
}
