using System.Collections.Generic;

namespace AldurSoft.WurmApi.Impl.WurmServerGroupsImpl
{
    public class ServerGroups : IWurmServerGroups
    {
        private readonly List<ServerGroup> groups = new List<ServerGroup>();

        ServerGroups()
        {
            groups.Add(new EpicServerGroup());
            groups.Add(new FreedomServerGroup());
            groups.Add(new ChallengeServerGroup());
        }

        public IEnumerable<ServerGroup> AllValid
        {
            get { return groups; }
        }

        public ServerGroup Get(ServerGroupId serverGroupId)
        {
            switch (serverGroupId)
            {
                case ServerGroupId.Epic:
                    return new EpicServerGroup();
                case ServerGroupId.Freedom:
                    return new FreedomServerGroup();
                case ServerGroupId.Challenge:
                    return new ChallengeServerGroup();
                case ServerGroupId.Unknown:
                    return new UnknownServerGroup();
                default:
                    return new UnknownServerGroup();
            }
        }
    }

    public class UnknownServerGroup : ServerGroup
    {
        public override ServerGroupId ServerGroupId
        {
            get { return ServerGroupId.Unknown; }
        }
    }

    public class EpicServerGroup : ServerGroup
    {
        public override ServerGroupId ServerGroupId
        {
            get { return ServerGroupId.Epic; }
        }
    }

    public class FreedomServerGroup : ServerGroup
    {
        public override ServerGroupId ServerGroupId
        {
            get { return ServerGroupId.Freedom; }
        }
    }

    public class ChallengeServerGroup : ServerGroup
    {
        public override ServerGroupId ServerGroupId
        {
            get { return ServerGroupId.Challenge; }
        }
    }
}