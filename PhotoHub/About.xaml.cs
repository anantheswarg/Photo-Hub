using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using System.Xml.Linq;
using Microsoft.Phone.Tasks;
using System.Windows.Navigation;
using Microsoft.Phone.Shell;

namespace PhotoHub
{
    public partial class About : PhoneApplicationPage
    {
        public About()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(Help_Loaded);
        }

        /// <summary>
        /// Default loaded event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Help_Loaded(object sender, RoutedEventArgs e)
        {
            this.GetHelpContent();
        }

        /// <summary>
        /// Gets the help content for a selected help menu item from XML
        /// </summary>
        /// <param name="helpItem">Selected help menu item</param>
        /// <returns></returns>
        private void GetHelpContent()
        {
            try
            {
                XDocument xmlDocument = XDocument.Load(Constants.HELP_PAGE);

                foreach (var helpItem in xmlDocument.Descendants("item"))
                {
                    this.TxbHelpHome.Text = helpItem.Element("Home").Value.Trim();
                    this.TxbHelpPhotoEffects.Text = helpItem.Element("PhotoEffects").Value.Trim();
                    this.TxbHelpShare.Text = helpItem.Element("Share").Value.Trim();
                    this.TxbHelpDisclaimer.Text = helpItem.Element("Disclaimer").Value.Trim();
                }
            }
            catch (NullReferenceException)
            {
                MessageBox.Show(Messages.SomeErrorMessage, Messages.ErrorTitle, MessageBoxButton.OK);
            }
            catch (ArgumentException)
            {
                MessageBox.Show(Messages.SomeErrorMessage, Messages.ErrorTitle, MessageBoxButton.OK);
            }
        }

        /// <summary>
        /// To redirect user to Facebook/ Twitter page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Share_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;

            WebBrowserTask browser = new WebBrowserTask();
            browser.URL = (string)button.Tag;
            browser.Show();
        }

        /// <summary>
        /// To open user Outlook on the phone
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void MySupportMail_Click(object sender, RoutedEventArgs e)
        {
            EmailComposeTask emailComposeTask = new EmailComposeTask();
            emailComposeTask.To = Constants.SUPPORT_EMAIL;
            emailComposeTask.Subject = Constants.SUPPORT_SUBJECT;
            emailComposeTask.Body = String.Empty;
            emailComposeTask.Show();
        }

        /// <summary>
        /// To open user Outlook on the phone
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void MyFeedbackMail_Click(object sender, RoutedEventArgs e)
        {
            EmailComposeTask emailComposeTask = new EmailComposeTask();
            emailComposeTask.To = Constants.FEEDBACK_EMAIL;
            emailComposeTask.Subject = Constants.FEEDBACK_SUBJECT;
            emailComposeTask.Body = String.Empty;
            emailComposeTask.Show();
        }

        /// <summary>
        /// Website Link button to take user to EdgeQ.com
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void MyWebsiteLinkButton_Click(object sender, RoutedEventArgs e)
        {
            WebBrowserTask browser = new WebBrowserTask();
            browser.URL = Constants.EDGEQ_URL;
            browser.Show();
        }

        /// <summary>
        /// Event handler to handle the device back key press
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void AnimatedBasePage_BackKeyPress(object sender, System.ComponentModel.CancelEventArgs e)
        {
            base.OnBackKeyPress(e);
        }

        /// <summary>
        /// Gets the selected help pivot index from phone application service
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (PhoneApplicationService.Current.State.ContainsKey("HelpPivotSelectedIndex"))
            {
                this.PvthelpContent.SelectedIndex = (int)PhoneApplicationService.Current.State["HelpPivotSelectedIndex"];
            }
        }

        /// <summary>
        /// Saves the help pivot selected index in phone application service state
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            PhoneApplicationService.Current.State["HelpPivotSelectedIndex"] = this.PvthelpContent.SelectedIndex;
        }
    }
}