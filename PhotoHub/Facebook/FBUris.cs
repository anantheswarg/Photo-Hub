using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Globalization;

namespace PhotoHub {
	public static class FBUris {
		
        private static string appID = "146393132076904";
		
        private static string appSecret = "72ef9edb4bcfc0dc42043c0fa51cc681";
		//the correct url - but not working due to the WebBrowser fragment bug
        //private static string m_strLoginURL = "https://graph.facebook.com/oauth/authorize?client_id={0}&redirect_uri=http://www.facebook.com/connect/login_success.html&type=user_agent&display=touch&scope=publish_stream,user_hometown,user_birthday,friends_birthday,user_photos,read_stream ";
        private static string LoginURL = "https://graph.facebook.com/oauth/authorize?client_id={0}&redirect_uri=http://www.facebook.com/connect/login_success.html&display=touch&scope=publish_stream,user_hometown,user_birthday,friends_birthday,user_photos,read_stream ";
		private static string AccessTokenURL = "https://graph.facebook.com/oauth/access_token?client_id={0}&redirect_uri=http://www.facebook.com/connect/login_success.html&client_secret={1}&code={2}&fileupload=true&cookie=true";
		private static string QueryUserURL = "https://graph.facebook.com/me?fields=id,name,gender,link,hometown,picture&locale=en_US&access_token={0}";
        private static string LoadFriendsURL = "https://graph.facebook.com/me/friends?fields=id,name,birthday&access_token={0}";
        private static string PostMessageURL = "https://graph.facebook.com/me/feed";
        //private static string m_strPostImage = "https://graph.facebook.com/me/2093280/photos?access_token={0}";
        //private static string m_strGalleryUrl = "https://graph.facebook.com/me/photos?access_token={0}";

		public static Uri GetLoadFriendsUri(string accessToken) {
            return (new Uri(string.Format(CultureInfo.CurrentCulture, LoadFriendsURL, accessToken), UriKind.Absolute));
		}

		public static Uri GetPostMessageUri() {
			return (new Uri(PostMessageURL, UriKind.Absolute));
		}
		public static Uri GetQueryUserUri(string accessToken) {
            return (new Uri(string.Format(CultureInfo.CurrentCulture, QueryUserURL, accessToken), UriKind.Absolute));
		}
		public static Uri GetLoginUri() {
            return (new Uri(string.Format(CultureInfo.CurrentCulture, LoginURL, appID), UriKind.Absolute));
		}

		public static Uri GetTokenLoadUri(string code) {
            return (new Uri(string.Format(CultureInfo.CurrentCulture, AccessTokenURL, appID, appSecret, code), UriKind.Absolute));
		}
	}
}