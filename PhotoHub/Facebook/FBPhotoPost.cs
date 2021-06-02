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
using System.Text;
using System.IO;
using System.Windows.Media.Imaging;
using System.IO.IsolatedStorage;
using System.Globalization;
using System.Diagnostics;

namespace PhotoHub
{
    public class FBPhotoPost
    {
        #region class setup

        //local property variables

        private string ms_boundary = string.Empty;

        private string ms_accesstoken = string.Empty;

        private BitmapImage mimg_phototopost;

        private string ms_caption = "WP7 Upload Photo";

        // some multipart constants we will use in the strings we create

        private const string Prefix = "--";

        private const string NewLine = "\r\n";

        public FBPhotoPost()
        {
        }
        public BitmapImage PhotoToPost
        {

            get { return mimg_phototopost; }

            set { mimg_phototopost = value; }

        }
        public string PhotoCaption
        {

            get { return ms_caption; }

            set { ms_caption = value; }

        }

        public String BoundaryString
        {
            get
            {
                if (String.IsNullOrEmpty(ms_boundary))

                { ResetBoundaryString(); }

                return ms_boundary;
            }

        }

        public void ResetBoundaryString()
        {
            ms_boundary = "----------------------------" + DateTime.Now.Ticks.ToString("x", CultureInfo.InvariantCulture);
        }

        public String AccessToken
        {
            get

            { return ms_accesstoken; }

            set

            { ms_accesstoken = value; }

        }
        #endregion

        #region Post Data Stuff

        public void PostPhoto()

        //builds a byte array with the image in it and passes it to PostPhotoHTTPWebRequest

        //which sends the data via a POST to facebook
        {

            if ((mimg_phototopost != null) && (!String.IsNullOrEmpty(ms_accesstoken)))

                PostPhotoHTTPWebRequest(BuildPostData());

        }

        //Uses the PhotoToPost and BoundaryString to build a byte array we can use as the data in the POST message
        private byte[] BuildPostData()
        {
            // Build up the post message header

            var sb = new StringBuilder();

            //accepts a message parameter which will be the caption to go with the photo
            sb.Append(Prefix).Append(BoundaryString).Append(NewLine);

            sb.Append("Content-Disposition: form-data; name=\"message\"");

            sb.Append(NewLine);

            sb.Append(NewLine);

            sb.Append(PhotoCaption);

            sb.Append(NewLine);

            //data for the image

            string filename = @"FacebookUpload.jpg";

            string contenttype = @"image/jpeg";

            sb.Append(Prefix).Append(BoundaryString).Append(NewLine);

            sb.Append("Content-Disposition: form-data; filename=\"").Append(filename).Append("\"").Append(NewLine);

            sb.Append("Content-Type: ").Append(contenttype).Append(NewLine).Append(NewLine);

            byte[] ba_Header = Encoding.UTF8.GetBytes(sb.ToString());

            byte[] ba_photo = ImageToByteArray(PhotoToPost);

            byte[] ba_footer = Encoding.UTF8.GetBytes(String.Concat(NewLine, Prefix, BoundaryString, Prefix, NewLine));


            // Combine all the byte arrays into one - this is the data we will post 

            byte[] postData = new byte[ba_Header.Length + ba_photo.Length + ba_footer.Length];

            Buffer.BlockCopy(ba_Header, 0, postData, 0, ba_Header.Length);

            Buffer.BlockCopy(ba_photo, 0, postData, ba_Header.Length, ba_photo.Length);

            Buffer.BlockCopy(ba_footer, 0, postData, ba_Header.Length + ba_photo.Length, ba_footer.Length);

            //Set the last posted data variable to we can show it

            ms_lastposteddata = ByteArrayToString(postData);

            //return the data as a byte array

            return postData;

        }

        private static string ByteArrayToString(byte[] bytes)
        {

            //Converts the raw post data so you can display it for testing, etc...

            System.Text.Encoding enc = System.Text.Encoding.UTF8;

            string PostString = enc.GetString(bytes, 0, bytes.Length);

            return PostString;

        }

        private string ms_lastposteddata = "No Data Posted Yet";

        public string LastPostedData
        {
            get { return ms_lastposteddata; }

        }

        #endregion

        #region HTTPWebRequest

        //Posts the data to Facebook - can only post to me/photos (no access to users wall)

        private void PostPhotoHTTPWebRequest(byte[] postData)
        {
            try
            {

                //Fire up an HttpWebRequest and pass in the facebook url for posting as well as the AccessToken

                //The AccessToken has to be in the URL - it didn't work just passing it as part of the POST data

                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(string.Format(CultureInfo.CurrentCulture, "https://graph.facebook.com/me/photos?access_token={0}", App.AccessToken));

                httpWebRequest.ContentType = String.Concat("multipart/form-data; boundary=", BoundaryString);

                httpWebRequest.Method = "POST";

                // start the asynchronous operation

                httpWebRequest.BeginGetRequestStream((ar) => { GetRequestStreamCallback(ar, postData); }, httpWebRequest);

            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.ToString());
            }

        }

        private void GetRequestStreamCallback(IAsyncResult asynchronousResult, byte[] postData)
        {
            try
            {

                HttpWebRequest request = (HttpWebRequest)asynchronousResult.AsyncState;
                // End the operation

                Stream postStream = request.EndGetRequestStream(asynchronousResult);

                // Write to the request stream.

                postStream.Write(postData, 0, postData.Length);

                postStream.Close();

                // Start the asynchronous operation to get the response

                request.BeginGetResponse(new AsyncCallback(GetResponseCallback), request);
            }

            catch (IOException ex)
            {
                Deployment.Current.Dispatcher.BeginInvoke(() => { UpdatePostStatus("Error Uploading [GetRequestStreamCallback]:", ex.ToString()); });
            }
            catch (WebException ex)
            {
                Deployment.Current.Dispatcher.BeginInvoke(() => { UpdatePostStatus("Error Uploading [GetRequestStreamCallback]:", ex.ToString()); });
            }
        }
        private void GetResponseCallback(IAsyncResult asynchronousResult)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)asynchronousResult.AsyncState;

                // End the operation

                HttpWebResponse response = (HttpWebResponse)request.EndGetResponse(asynchronousResult);

                Stream streamResponse = response.GetResponseStream();

                StreamReader streamRead = new StreamReader(streamResponse);

                string responseString = streamRead.ReadToEnd();

                System.Diagnostics.Debug.WriteLine(responseString);

                //Update the UI
                Deployment.Current.Dispatcher.BeginInvoke(() => { UpdatePostStatus("Upload Success! Picture ID:", responseString); });

                // Close the stream object
                streamResponse.Close();

                // Release the HttpWebResponse
                response.Close();
            }
            catch (IOException ex)
            {
                Deployment.Current.Dispatcher.BeginInvoke(() => { UpdatePostStatus("Error Uploading [GetResponseCallback]:", ex.ToString()); });
            }
            catch (WebException ex)
            {
                Deployment.Current.Dispatcher.BeginInvoke(() => { UpdatePostStatus("Error Uploading [GetResponseCallback]:", ex.ToString()); });
            }
        }

        #endregion

        #region ImageManipulation

        /// <summary>
        /// Opens an image file in isolated storage and returns it as a 
        /// BitmapImage object we could use as the image in the PostData method
        /// </summary>
        /// <param name="ImageFileName"></param>
        /// <returns></returns>
        public static BitmapImage GetImageFromIsolatedStorage(String ImageFileName)
        {
            using (IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication())
            {
                // Open the file
                using (IsolatedStorageFileStream isfs = isf.OpenFile(ImageFileName, FileMode.Open, FileAccess.Read))
                {
                    // Use the stream as the source of the Bitmap Image
                    BitmapImage bmpimg = new BitmapImage();
                    bmpimg.SetSource(isfs);
                    //                    isfs.Close();
                    return bmpimg;
                }
            }
        }

        /// <summary>
        /// ImageToByteArray(BitmapImage) accepts a BitmapImage object and
        /// converts it to a byte array we can use in the POST buffer
        /// </summary>
        /// <param name="bm_Image"></param>
        /// <returns></returns>
        private static byte[] ImageToByteArray(BitmapImage bm_Image)
        {
            //byte[] data;
            //{
            //    // Get an Image Stream
            //    MemoryStream imageStream = new MemoryStream();

            //    // write an image into the stream
            //    System.Windows.Media.Imaging.Extensions.SaveJpeg(new WriteableBitmap(bm_Image), imageStream, bm_Image.PixelWidth, bm_Image.PixelHeight, 0, 100);

            //    // reset the stream pointer to the beginning
            //    imageStream.Seek(0, 0);

            //    //read the stream into a byte array
            //    data = new byte[imageStream.Length];
            //    imageStream.Read(data, 0, data.Length);
            //    imageStream.Close();

            //}

            ////data now holds the bytes of the image 
            //return data;

            // Get an Image Stream
            
            MemoryStream imageStream = new MemoryStream();
            
            byte[] data = null;
            {
                try
                {
                    // write an image into the stream
                    System.Windows.Media.Imaging.Extensions.SaveJpeg(new WriteableBitmap(bm_Image), imageStream, bm_Image.PixelWidth, bm_Image.PixelHeight, 0, 100);

                    // reset the stream pointer to the beginning
                    imageStream.Seek(0, 0);

                    //read the stream into a byte array
                    data = new byte[imageStream.Length];
                    imageStream.Read(data, 0, data.Length);

                }
                catch (IOException exception)
                {
                    Debug.WriteLine(exception.Message);
                }
                finally
                {
                    imageStream.Close();
                }
            }
            return data;
        }

        /// <summary>
        /// GetBitmapImageFromUIElement accepts a UIElement (canvas, grid, image, etc...)
        /// and builds a BitmapImage of it including any child elements
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public BitmapImage GetBitmapImageFromUIElement(UIElement element)
        {
            if (element != null)
            {
                // Get an Image Stream
                MemoryStream stream = new MemoryStream();
                try
                {
                    WriteableBitmap bmp = new WriteableBitmap((int)element.RenderSize.Width, (int)element.RenderSize.Height);
                    bmp.Render(element, null);
                    bmp.Invalidate();

                    // write the image into the stream
                    bmp.SaveJpeg(stream, (int)element.RenderSize.Width, (int)element.RenderSize.Height, 0, 100);

                    // reset the stream pointer to the beginning
                    stream.Seek(0, 0);

                    // Use the stream as the source of the Bitmap Image
                    BitmapImage bmpimg = new BitmapImage();
                    bmpimg.SetSource(stream);

                    return bmpimg;
                }
                catch (BadImageFormatException ex)
                {
                    UpdatePostStatus("Error Getting Bitmap from UIElement:", ex.Message);

                }
                catch (ArgumentException ex)
                {
                    UpdatePostStatus("Error Getting Bitmap from UIElement:", ex.Message);

                }
                finally
                {
                    stream.Close();
                }
            }
            return null;
        }

        #endregion

        #region Events code
        //Raise the PostStatus Event so the calling code knows whats going on

        private void UpdatePostStatus(string strHeader, string strContents)
        {

            if (PostStatus != null)
            {

                PostStatus(this, new PostStatusEventArgs(strHeader, strContents));

            }

        }

        public class PostStatusEventArgs : EventArgs
        {

            private readonly string msg_header = string.Empty;

            private readonly string msg_contents = string.Empty;

            // Constructor.
            public PostStatusEventArgs(string msgHeader, string msgContents)
            {

                this.msg_header = msgHeader;

                this.msg_contents = msgContents;

            }
            // Properties.

            public string MessageHeader { get { return msg_header; } }

            public string MessageContents { get { return msg_contents; } }
        }
        //the PostStatus Event sends status updates 

        public delegate void PostStatusEventHandler(object sender, PostStatusEventArgs e);

        public event PostStatusEventHandler PostStatus;

        #endregion
    }
}
