using Android.App;
using Android.Content;
using Android.Gms.Extensions;
using Android.OS;
using Android.Runtime;
using AndroidX.AppCompat.App;
using Firebase;
using Firebase.Iid;
using Firebase.Messaging;
using Java.Lang;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PushNotifTest
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            CreateNotificationChannel();
            Intent firebaseIntent = new Intent(this, typeof(MyFirebaseMessagingService));
            StartService(firebaseIntent);
            var fapp = FirebaseApp.InitializeApp(this);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);

            Task.Run(async () =>
            {
                try
                {
                    await FirebaseMessaging.Instance.GetToken();
                    var token = FirebaseInstanceId.Instance.Token;
                    System.Diagnostics.Debug.WriteLine($"token: {token}");
                    //await FirebaseMessaging.Instance.SubscribeToTopic("HelloCampaign");
                }
                catch (Exception ex)
                {

                }
            });
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        void CreateNotificationChannel()
        {
            if (Build.VERSION.SdkInt < BuildVersionCodes.O)
            {
                // Notification channels are new in API 26 (and not a part of the
                // support library). There is no need to create a notification
                // channel on older versions of Android.
                return;
            }

            var channel = new NotificationChannel("NotificationChannel_ID",
                                                  "FCM Notifications",
                                                  NotificationImportance.Default)
            {

                Description = "Firebase Cloud Messages appear in this channel"
            };

            var notificationManager = (NotificationManager)GetSystemService(Android.Content.Context.NotificationService);
            notificationManager.CreateNotificationChannel(channel);
        }
    }

    [Service(Exported = true)]
    [IntentFilter(new[] { "com.google.firebase.MESSAGING_EVENT" })]
    public class MyFirebaseMessagingService : FirebaseMessagingService
    {
        public override void OnNewToken(string token)
        {
            base.OnNewToken(token);
        }

        public override void OnMessageReceived(RemoteMessage message)
        {
            var body = message.GetNotification().Body;
            var title = message.GetNotification().Title;
            System.Diagnostics.Debug.WriteLine($"message received {title} {body}");
            SendNotification(body, title, message.Data);
        }

        void SendNotification(string messageBody, string Title, IDictionary<string, string> data)
        {

        }
    }
}