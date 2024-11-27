using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;
using Rocket.Core.Utils;
using UnityEngine.Networking;

namespace Rocket.Core.Steam;

[TypeForwardedFrom(RocketCompatIntl.RocketCoreAssembly)]
public class Profile
{
    public ulong SteamID64 { get; set; }

    public string SteamID { get; set; }

    public string OnlineState { get; set; }

    public string StateMessage { get; set; }

    public string PrivacyState { get; set; }

    public ushort? VisibilityState { get; set; }

    public Uri AvatarIcon { get; set; }

    public Uri AvatarMedium { get; set; }

    public Uri AvatarFull { get; set; }

    public bool? IsVacBanned { get; set; }

    public string TradeBanState { get; set; }

    public bool? IsLimitedAccount { get; set; }

    public string CustomURL { get; set; }

    public DateTime? MemberSince { get; set; }

    public double? HoursPlayedLastTwoWeeks { get; set; }

    public string Headline { get; set; }

    public string Location { get; set; }

    public string RealName { get; set; }

    public string Summary { get; set; }

    public List<MostPlayedGame> MostPlayedGames { get; set; }

    public List<Group> Groups { get; set; }

    public Profile(ulong steamID64)
    {
        SteamID64 = steamID64;
        Reload();
    }

    public void Reload()
    {
        string lastField = "unknown";
        try
        {
            XmlDocument xmlDocument = new XmlDocument();
            string url = $"http://steamcommunity.com/profiles/{SteamID64}?xml=1";
            if (ThreadUtil.gameThread == Thread.CurrentThread)
            {
                for (int i = 0; i < 3; ++i)
                {
                    using UnityWebRequest request = UnityWebRequest.Get(url);
                    request.timeout = 2;
                    UnityWebRequestAsyncOperation operation = request.SendWebRequest();
                    while (!operation.isDone)
                        Thread.Sleep(15);

                    if (request.result != UnityWebRequest.Result.Success)
                    {
                        if (i == 2)
                            throw new Exception($"Failed to fetch steam profile for {SteamID64} - {request.responseCode} - \"{request.error}\".");

                        Logger.Log($"Retrying fetching Steam profile for {SteamID64} - {request.responseCode} - \"{request.error}\".");
                        Thread.Sleep(100);
                        continue;
                    }

                    xmlDocument.Load(request.downloadHandler.text);
                    break;
                }
            }
            else
            {
                using WebClient webClient = new RocketWebClient(2000);

                for (int i = 0; i < 3; ++i)
                {
                    try
                    {
                        xmlDocument.LoadXml(webClient.DownloadString(url));
                        break;
                    }
                    catch (WebException ex)
                    {
                        string content;
                        if (ex.Response.ContentLength < 512)
                        {
                            Stream st = ex.Response.GetResponseStream();
                            byte[] buffer = new byte[ex.Response.ContentLength];
                            int l = st!.Read(buffer, 0, (int)ex.Response.ContentLength);
                            content = Encoding.UTF8.GetString(buffer, 0, l);
                        }
                        else
                        {
                            content = "??";
                        }

                        if (i == 2)
                            throw new Exception($"Failed to fetch steam profile for {SteamID64} - {ex.Status} - \"{content}\".");

                        Logger.Log($"Retrying fetching Steam profile for {SteamID64} - {ex.Status} - \"{content}\".");
                        Thread.Sleep(100);
                    }
                }
            }

            XmlElement profile = xmlDocument["profile"];
            if (profile == null)
                return;

            SteamID = profile["steamID"]?.ParseString();
            lastField = "SteamID";

            OnlineState = profile["onlineState"]?.ParseString();
            lastField = "OnlineState";

            StateMessage = profile["stateMessage"]?.ParseString();
            lastField = "StateMessage";

            PrivacyState = profile["privacyState"]?.ParseString();
            lastField = "PrivacyState";

            VisibilityState = profile["visibilityState"]?.ParseUInt16(CultureInfo.InvariantCulture);
            lastField = "VisibilityState";

            AvatarIcon = profile["avatarIcon"]?.ParseUri();
            lastField = "AvatarIcon";

            AvatarMedium = profile["avatarMedium"]?.ParseUri();
            lastField = "AvatarMedium";

            AvatarFull = profile["avatarFull"]?.ParseUri();
            lastField = "AvatarFull";

            IsVacBanned = profile["vacBanned"]?.ParseBool();
            lastField = "IsVacBanned";

            TradeBanState = profile["tradeBanState"]?.ParseString();
            lastField = "TradeBanState";

            IsLimitedAccount = profile["isLimitedAccount"]?.ParseBool();
            lastField = "IsLimitedAccount";

            CustomURL = profile["customURL"]?.ParseString();
            lastField = "CustomURL";

            MemberSince = profile["memberSince"]?.ParseDateTime(CultureInfo.InvariantCulture);
            lastField = "MemberSince";

            HoursPlayedLastTwoWeeks = profile["hoursPlayed2Wk"].ParseDouble(CultureInfo.InvariantCulture);
            lastField = "HoursPlayedLastTwoWeeks";

            Headline = profile["headline"]?.ParseString();
            lastField = "Headline";

            Location = profile["location"]?.ParseString();
            lastField = "Location";

            RealName = profile["realname"]?.ParseString();
            lastField = "RealName";

            Summary = profile["summary"]?.ParseString();
            lastField = "Summary";

            XmlElement mostPlayedGame = profile["mostPlayedGames"];
            if (mostPlayedGame != null)
            {
                MostPlayedGames = new List<MostPlayedGame>();
                lastField = "MostPlayedGames";
                foreach (XmlElement childNode in mostPlayedGame.ChildNodes)
                {
                    // ReSharper disable once UseObjectOrCollectionInitializer
                    MostPlayedGame game = new MostPlayedGame();

                    game.Name = childNode["gameName"]?.ParseString();
                    lastField = "MostPlayedGame.Name";

                    game.Link = childNode["gameLink"]?.ParseUri();
                    lastField = "MostPlayedGame.Link";

                    game.Icon = childNode["gameIcon"]?.ParseUri();
                    lastField = "MostPlayedGame.Icon";

                    game.Logo = childNode["gameLogo"]?.ParseUri();
                    lastField = "MostPlayedGame.Logo";

                    game.LogoSmall = childNode["gameLogoSmall"]?.ParseUri();
                    lastField = "MostPlayedGame.LogoSmall";

                    game.HoursPlayed = childNode["hoursPlayed"]?.ParseDouble(CultureInfo.InvariantCulture);
                    lastField = "MostPlayedGame.HoursPlayed";

                    game.HoursOnRecord = childNode["hoursOnRecord"]?.ParseDouble(CultureInfo.InvariantCulture);
                    lastField = "MostPlayedGame.HoursOnRecord";

                    MostPlayedGames.Add(game);
                }
            }

            XmlElement groups = profile["groups"];
            if (groups == null)
                return;

            Groups = new List<Group>();
            lastField = "Groups";
            foreach (XmlElement childNode in groups.ChildNodes)
            {
                Group group = new Group();

                group.IsPrimary = childNode.Attributes["isPrimary"] is { InnerText: "1" };
                lastField = "Group.IsPrimary";

                group.SteamID64 = childNode["groupID64"]?.ParseUInt64(CultureInfo.InvariantCulture);
                lastField = "Group.SteamID64";

                group.Name = childNode["groupName"]?.ParseString();
                lastField = "Group.Name";

                group.URL = childNode["groupURL"]?.ParseString();
                lastField = "Group.URL";

                group.Headline = childNode["headline"]?.ParseString();
                lastField = "Group.Headline";

                group.Summary = childNode["summary"]?.ParseString();
                lastField = "Group.Summary";

                group.AvatarIcon = childNode["avatarIcon"]?.ParseUri();
                lastField = "Group.AvatarIcon";

                group.AvatarMedium = childNode["avatarMedium"]?.ParseUri();
                lastField = "Group.AvatarMedium";

                group.AvatarFull = childNode["avatarFull"]?.ParseUri();
                lastField = "Group.AvatarFull";

                group.MemberCount = childNode["memberCount"]?.ParseUInt32(CultureInfo.InvariantCulture);
                lastField = "Group.MemberCount";

                group.MembersInChat = childNode["membersInChat"]?.ParseUInt32(CultureInfo.InvariantCulture);
                lastField = "Group.MembersInChat";

                group.MembersInGame = childNode["membersInGame"]?.ParseUInt32(CultureInfo.InvariantCulture);
                lastField = "Group.MembersInGame";

                group.MembersOnline = childNode["membersOnline"]?.ParseUInt32(CultureInfo.InvariantCulture);
                lastField = "Group.MembersOnline";

                Groups.Add(group);
            }
        }
        catch (Exception ex)
        {
            string message = "Error reading Steam Profile after field: " + lastField;
            Logger.LogException(ex, message);
        }
    }

    [TypeForwardedFrom(RocketCompatIntl.RocketCoreAssembly)]
    public class MostPlayedGame
    {
        public string Name { get; set; }

        public Uri Link { get; set; }

        public Uri Icon { get; set; }

        public Uri Logo { get; set; }

        public Uri LogoSmall { get; set; }

        public double? HoursPlayed { get; set; }

        public double? HoursOnRecord { get; set; }
    }

    [TypeForwardedFrom(RocketCompatIntl.RocketCoreAssembly)]
    public class Group
    {
        public ulong? SteamID64 { get; set; }

        public bool IsPrimary { get; set; }

        public string Name { get; set; }

        public string URL { get; set; }

        public Uri AvatarIcon { get; set; }

        public Uri AvatarMedium { get; set; }

        public Uri AvatarFull { get; set; }

        public string Headline { get; set; }

        public string Summary { get; set; }

        public uint? MemberCount { get; set; }

        public uint? MembersInGame { get; set; }

        public uint? MembersInChat { get; set; }

        public uint? MembersOnline { get; set; }
    }
}