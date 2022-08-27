
namespace GameBench
{
    using UnityEngine;
    using UnityEngine.UI;
#if ENABLE_FACEBOOK
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using Facebook.Unity;
    using Facebook.MiniJSON;
    using System.Linq;
    using System.IO;
#endif
    public enum ToggleState
    {
        Unchecked,
        Partial,
        Checked
    };
    public class FBManager : MonoBehaviour
    {
        //public int testScoreValue;
        public GameObject leaderboardLoading, inviteLoading;
        public Sprite[] stateSprites;
        // List of the invite and leaderboard list items

        // List containers that list Items - (Dynamically Increasing ListView <Custom>)
        public Transform leaderParent, inviteParent;
        //Prefabs that holds items that will be places in the containers.
        public LeaderItem itemLeaderPref;
        public InviteItem itemInvitePref;

        //Reference to buttons and toggles 
        public Button btnInvite, btnSlctAll, btnLogin, btnInviteF/*, btnShare, btnPostScore*/;

        //These Two buttons will be Active One at a Time
        public ToggleState tglStateSlctAll = ToggleState.Unchecked;


        // Info Text
        //public Text infoText;
        // Delegate Responsible for Performing after picture Load Actions
        delegate void LoadPictureCallback(Texture2D texture, int index);

        private static FBManager _instance;
        public static FBManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = GameObject.FindObjectOfType<FBManager>();
                }
                return _instance;
            }
        }

        public bool LoggedIn
        {
            get
            {
#if ENABLE_FACEBOOK
                return FB.IsInitialized && FB.IsLoggedIn;
#else
                return false;
#endif
            }
        }
        public void InitNLogin()
        {
#if ENABLE_FACEBOOK
            if (!inProcess)
            {
                inProcess = true;
                if (!FB.IsInitialized)
                {
                    FB.Init(InitCallback, onHideUnity);
                }
                else
                {
                    FB.ActivateApp();
                    if (FB.IsLoggedIn)
                    {
                        LogText("Already Logged In.");
                        SetFacebookRelatedData();
                    }
                    else
                    {
                        WaitForSecAndCheck();
                    }
                }
            }
#endif
        }
#if ENABLE_FACEBOOK
         //string that let you get JSON from the Facebook API calls.
        string getScoreAPIString = "app/scores?fields=score";
         List<LeaderItem> listLeaderboard = new List<LeaderItem>();
        List<InviteItem> listInvites = new List<InviteItem>();
        void Awake()
        {
            SetButtonsListners();
            SetFBItems(false);
        }
        void LogText(string msg)
        {
            print(msg);
        }
        void SetButtonsListners()
        {
            btnLogin.onClick.AddListener(() =>
            {
                InitNLogin();
            });
            btnInvite.onClick.AddListener(() =>
            {
                SendInvites();
            });
            btnSlctAll.onClick.AddListener(() =>
            {
                TglSelectAllClickHandler();
            });
        }

        public void PostScore(int scoreVal)
        {
            /*
            If you don't have facebook publish permission already, Ask for it
            Note! this is not going to work if your publish_actions permission is not approved by facebook.
            Each time you'll post score, It'll prompt user to grant publish_actions unless your app is 
            approved by facebook for publish actions.
            */
            if (!AccessToken.CurrentAccessToken.Permissions.Contains(publishPermission[0]))
            {
                // As A good Practice, You should tell your users that why you need publish permission so
                // show a dialog telling about it. or else simply go to facebook permission prompt.
                FB.LogInWithPublishPermissions(publishPermission, delegate (ILoginResult loginResult)
                {
                    if (AccessToken.CurrentAccessToken.Permissions.Contains(publishPermission[0]))
                    {
                        if (!string.IsNullOrEmpty(loginResult.Error) || loginResult.Cancelled)
                        {
                            LogText("no Publish Permission!");
                            return;
                        }
                    }

                });
            }
            var scoreData = new Dictionary<string, string>() { { "score", scoreVal.ToString() } };
            FB.API("/me/scores", HttpMethod.POST, delegate (IGraphResult r)
            {
                if (!r.Cancelled || r.Error != null)
                {
                    LogText("Score Posted Successfully!");
                    LoadLeaderboard();
                }
                else
                    LogText("Error Occured!");
            }, scoreData);
        }

        [HideInInspector]
        public int highScoreFacebook = 0;

        void GetFBScoreOfCurrentUser()
        {
            FB.API("me/score?fields=score", HttpMethod.GET, delegate (IGraphResult result)
            {
                if (string.IsNullOrEmpty(result.Error) && !result.Cancelled)
                {
                    try
                    {
                        //IDictionary data = result.ResultDictionary["data"] as IDictionary;
                        //highScoreFacebook = Convert.ToInt32(data["score"] as string);
                        Dictionary<string, object> JSON = Json.Deserialize(result.RawResult) as Dictionary<string, object>;
                        List<object> data = JSON["data"] as List<object>;

                        highScoreFacebook = int.Parse(Convert.ToString(((Dictionary<string, object>)data[0])["score"]));
                        print(highScoreFacebook);
                        ShareScoreOverFacebook();
                    }
                    catch (Exception exp)
                    {
                        LogText(exp.ToString());
                    }
                }
            });
        }
        //Method to load leaderboard
        public void LoadLeaderboard()
        {
            leaderboardLoading.SetActive(true);
            ClearLeaderboard();
            FB.API(getScoreAPIString, HttpMethod.GET, CallBackLoadLeaderboard);
        }
        //callback of from Facebook API when the leaderboard data from the server is loaded.
        void CallBackLoadLeaderboard(IGraphResult result)
        {
            if (string.IsNullOrEmpty(result.Error) && !result.Cancelled)
            {
                Dictionary<string, object> JSON = Json.Deserialize(result.RawResult) as Dictionary<string, object>;
                List<object> data = JSON["data"] as List<object>;
                for (int i = 0; i < data.Count; i++)
                {
                    string fScore;
                    try
                    {
                        fScore = Convert.ToString(((Dictionary<string, object>)data[i])["score"]);
                    }
                    catch (Exception)
                    {
                        fScore = "0";
                    }
                    Dictionary<string, object> UserInfo = ((Dictionary<string, object>)data[i])["user"] as Dictionary<string, object>;
                    string name = Convert.ToString(UserInfo["name"]);
                    string id = Convert.ToString(UserInfo["id"]);
                    CreateListItemLeaderboard(id, name, fScore, i + 1);
                    LoadFriendsAvatar(i);
                }
                leaderboardLoading.SetActive(false);
            }
            if (result.Error != null)
            {
                FB.API(getScoreAPIString, HttpMethod.GET, CallBackLoadLeaderboard);
                return;
            }
        }

        // Method to load Friends Profile Pictures
        void LoadFriendsAvatar(int index)
        {
            FB.API(GetPictureURL(listLeaderboard[index].fId), HttpMethod.GET, result =>
         {
             if (result.Error != null)
             {
                 Debug.LogError(result.Error);
                 return;
             }
             listLeaderboard[index].picUrl = DeserializePictureURLString(result.RawResult);
             StartCoroutine(LoadPicRoutine(listLeaderboard[index].picUrl, index));
         });
        }

        //Method to all items to the leaderboard dynamically scrollable list
        void CreateListItemLeaderboard(string id, string fName, string fScore = "", int rank = 0)
        {
            LeaderItem tempItem = Instantiate(itemLeaderPref) as LeaderItem;
            tempItem.AssignValues(id, fName, fScore, rank.ToString());
            tempItem.transform.SetParent(leaderParent, false);
            listLeaderboard.Add(tempItem);
        }

        //Coroutine to load Picture from the specified URL
        IEnumerator LoadPicRoutine(string url, int index)
        {
            WWW www = new WWW(url);
            yield return www;

            Texture2D texture = www.texture;
            if (texture == null)
            {
                StartCoroutine(LoadPicRoutine(listLeaderboard[index].picUrl, index));
            }
            else
            {
                listLeaderboard[index].imgPic.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            }
        }
        List<string> readPermission = new List<string>() { "public_profile", "user_friends" },
        publishPermission = new List<string>() { "publish_actions" };

        bool inProcess = false;
       
        private void InitCallback()
        {
            if (FB.IsInitialized)
            {
                WaitForSecAndCheck();
            }
            else
            {
                LogText("Failed to Initialize the Facebook SDK");
                InitNLogin();
            }

        }

        private void onHideUnity(bool isGameShown)
        {
            if (!isGameShown)
            {
                // Pause the game - we will need to hide
                Time.timeScale = 0;
            }
            else
            {
                // Resume the game - we're getting focus again
                Time.timeScale = 1;
            }
        }

        //Callback method of login
        void LoginCallback(ILoginResult result)
        {
            if (FB.IsLoggedIn)
            {
                // AccessToken class will have session details
                //var aToken = AccessToken.CurrentAccessToken;
                //foreach (string perm in aToken.Permissions)
                //{
                //    print(perm);
                //}
                LogText("Logged In Successfully!");
                SetFacebookRelatedData();
            }
            else
            {
                LogText("User cancelled login");
                SetFBItems(false);
            }
        }
        void SetFBItems(bool isLoggedIn)
        {
            inProcess = false;
            btnLogin.gameObject.SetActive(!isLoggedIn);

            //btnSlctAll.gameObject.SetActive(isLoggedIn);
            //btnInvite.gameObject.SetActive(isLoggedIn);
            //btnShare.gameObject.SetActive(isLoggedIn);
            btnInviteF.gameObject.SetActive(isLoggedIn);
            //btnPostScore.gameObject.SetActive(isLoggedIn);
        }
        void SetFacebookRelatedData()
        {
            SetFBItems(true);
            //LoadLeaderboard();
            LoadInvitableFriends();
            PostScore(GameBench.GameManager.Instance.BestScore);
        }

        // Remedy for A Facebook Plugin Bug that Returns False if FB.IsLoggedIn is called in Callback of FB.Init!
        void WaitForSecAndCheck()
        {
            if (FB.IsLoggedIn)
            {
                LogText("Already Logged In.");
                SetFacebookRelatedData();
            }
            else
            {
                StartCoroutine(CheckFbInit());
            }
        }
        IEnumerator CheckFbInit()
        {
            yield return new WaitForSeconds(1f);

            if (FB.IsLoggedIn)
            {
                //Here you should be logged in
                SetFacebookRelatedData();
            }
            else
            {
                FB.ActivateApp();
                FB.LogInWithReadPermissions(readPermission, LoginCallback);
            }

        }
        public void LogoutFB()
        {
            FB.LogOut();
            SetFBItems(false);
            ClearLeaderboard();
            ClearInvite();
        }
        void ClearLeaderboard()
        {
            listLeaderboard.Clear();
            for (int i = 0; i < leaderParent.childCount; i++)
            {
                Destroy(leaderParent.GetChild(i).gameObject);
            }
        }

        void ClearInvite()
        {
            listInvites.Clear();
            for (int i = 0; i < inviteParent.childCount; i++)
            {
                Destroy(inviteParent.GetChild(i).gameObject);
            }
            tglStateSlctAll = ToggleState.Unchecked;
            btnSlctAll.image.sprite = stateSprites[0];
        }
        public void ShareScoreOverFacebook()
        {
            if (highScoreFacebook == 0)
            {
                GetFBScoreOfCurrentUser();
            }
            else
            {
                string shareDesc = (highScoreFacebook > 0) ?
                FBSetup.Instance.shareDialogMsg + " My High Score is " + highScoreFacebook :
                 FBSetup.Instance.shareDialogMsg;

                if (FB.IsLoggedIn)
                {
                    FB.ShareLink(
                        contentURL: new Uri(FBSetup.Instance.fbShareURI),
                        contentTitle: FBSetup.Instance.shareDialogTitle,
                        contentDescription: shareDesc,
                        photoURL: new Uri(FBSetup.Instance.fbSharePicURI),
                        callback: (IShareResult result) =>
                        {
                            if (string.IsNullOrEmpty(result.Error) && !result.Cancelled)
                                LogText("Story Posted Successfully!");
                            else
                                LogText("Error Occured!");
                        }
                        );
                }
            }
        }
        // Method that Proceeds with the Invitable Friends
        void LoadInvitableFriends()
        {
            ClearInvite();
            inviteLoading.SetActive(true);
            string loadInvitableFriendsString = "me/invitable_friends?limit=" + FBSetup.Instance.InviteFriendsCount;
            FB.API(loadInvitableFriendsString, HttpMethod.GET, CallBackLoadInvitableFriends);
        }
        //Callback of Invitable Friends API Call
        void CallBackLoadInvitableFriends(IGraphResult result)
        {
            //Deserializing JSON returned from server
            Dictionary<string, object> JSON = Json.Deserialize(result.RawResult) as Dictionary<string, object>;
            List<object> data = JSON["data"] as List<object>;
            //Loop to traverse and process all the items returned from the server.
            for (int i = 0; i < data.Count; i++)
            {
                string id = Convert.ToString(((Dictionary<string, object>)data[i])["id"]);
                string name = Convert.ToString(((Dictionary<string, object>)data[i])["name"]);
                Dictionary<string, object> picInfo = ((Dictionary<string, object>)data[i])["picture"] as Dictionary<string, object>;
                string url = DeserializePictureURLObject(picInfo);
                CreateListItemInvite(id, name, url);
                StartCoroutine(LoadFPicRoutine(url, PicCallBackInvitable, i));
            }
            inviteLoading.SetActive(false);
        }
        //Method to add item to the custom invitable dynamically scrollable list
        void CreateListItemInvite(string id, string fName, string url = "")
        {
            InviteItem tempItem = Instantiate(itemInvitePref) as InviteItem;
            tempItem.AssignValues(id, url, fName);
            tempItem.transform.SetParent(inviteParent, false);
            listInvites.Add(tempItem);
        }
        //Callback of Invitable Friend API call
        void PicCallBackInvitable(Texture2D texture, int index)
        {
            if (texture == null)
            {
                StartCoroutine(LoadFPicRoutine(listInvites[index].picUrl, PicCallBackInvitable, index));
                return;
            }
            listInvites[index].imgPic.sprite = Sprite.Create(texture,
                new Rect(0, 0, texture.width, texture.height),
                new Vector2(0.5f, 0.5f)
            );
        }

        //Click Handler of Select All Buttons
        public void TglSelectAllClickHandler()
        {
            switch (tglStateSlctAll)
            {
                case ToggleState.Partial:
                case ToggleState.Unchecked:
                    foreach (var item in listInvites)
                    {
                        item.tglBtn.isOn = true;
                    }
                    tglStateSlctAll = ToggleState.Checked;
                    ChangeToggleState(ToggleState.Checked);
                    break;
                case ToggleState.Checked:
                    foreach (var item in listInvites)
                    {
                        item.tglBtn.isOn = false;
                    }
                    ChangeToggleState(ToggleState.Unchecked);
                    break;
            }
        }
       
        IEnumerator LoadFPicRoutine(string url, LoadPictureCallback Callback, int index)
        {
            WWW www = new WWW(url);
            yield return www;
            Callback(www.texture, index);
        }
        void SendInvites()
        {
            List<string> lstToSend = new List<string>();
            foreach (var item in listInvites)
            {
                if (item.tglBtn.isOn)
                {
                    lstToSend.Add(item.fId);
                }
            }
            int dialogCount = (int)Mathf.Ceil(lstToSend.Count / 50f);
            CallInvites(lstToSend, dialogCount);
        }
        //Helping method that will be recursive if you'll have to sent invites to more than 50 Friends.
        private void CallInvites(List<string> lstToSend, int dialogCount)
        {
            if (dialogCount > 0)
            {
                string[] invToSend = (lstToSend.Count >= 50) ? new string[50] : new string[lstToSend.Count];

                for (int i = 0; i < invToSend.Length; i++)
                {
                    try
                    {
                        if (lstToSend[i] != null)
                        {
                            invToSend[i] = lstToSend[i];
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.Log(e.Message);
                    }
                }
                lstToSend.RemoveRange(0, invToSend.Length);
                FB.AppRequest(
                     FBSetup.Instance.inviteDialogMsg,
                    invToSend,
                    null,
                    null,
                    null,
                    "",
                     FBSetup.Instance.inviteDialogTitle,
                    FBResult =>
                    {
                        if (--dialogCount > 0)
                        {
                            CallInvites(lstToSend, dialogCount);
                        }
                    }
                );
            }
        }
#if !UNITY_WEBGL
        string FILE_NAME = "userpic.jpg";
        string getUserPicString = "me?fields=picture.height(256)";
        string GetPath(string fileName)
        {
            return Path.Combine(Application.persistentDataPath, fileName);
        }
#endif
        public static string GetPictureURL(string facebookID, int? width = null, int? height = null, string type = null)
        {
            string url = string.Format("/{0}/picture", facebookID);
            string query = width != null ? "&width=" + width.ToString() : "";
            query += height != null ? "&height=" + height.ToString() : "";
            query += type != null ? "&type=" + type : "";
            query += "&redirect=false";
            if (query != "") url += ("?g" + query);
            return url;
        }
        public static string DeserializePictureURLString(string response)
        {
            return DeserializePictureURLObject(Json.Deserialize(response));
        }

        public static string DeserializePictureURLObject(object pictureObj)
        {
            var picture = (Dictionary<string, object>)(((Dictionary<string, object>)pictureObj)["data"]);
            object urlH = null;
            if (picture.TryGetValue("url", out urlH))
            {
                return (string)urlH;
            }

            return null;
        }
#endif
        //Method to change Toggle State On the Fly
        public void ChangeToggleState(ToggleState state)
        {
#if ENABLE_FACEBOOK
            switch (state)
            {
                case ToggleState.Unchecked:
                    tglStateSlctAll = state;
                    btnSlctAll.image.sprite = stateSprites[0];
                    break;
                case ToggleState.Partial:
                    bool flagOn = false, flagOff = false;
                    foreach (var item in listInvites)
                    {
                        if (item.tglBtn.isOn)
                        {
                            flagOn = true;
                        }
                        else
                        {
                            flagOff = true;
                        }
                    }
                    if (flagOn && flagOff)
                    {
                        tglStateSlctAll = state;
                        btnSlctAll.image.sprite = stateSprites[1];
                        //Debug.Log("Partial");
                    }
                    else if (flagOn && !flagOff)
                    {
                        ChangeToggleState(ToggleState.Checked);
                        //Debug.Log("Checked");
                    }
                    else if (!flagOn && flagOff)
                    {
                        ChangeToggleState(ToggleState.Unchecked);
                        //Debug.Log("Unchecked");
                    }
                    break;
                case ToggleState.Checked:
                    tglStateSlctAll = state;
                    btnSlctAll.image.sprite = stateSprites[2];
                    break;
            }
        }
#endif
        }
    }
}