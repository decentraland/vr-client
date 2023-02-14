/********************************************************************
	created:	2017/06/07
	file base:	BestBefore
	file ext:	cs
	author:		Alexandros
	version:	1.4.1

	purpose:	This script manages the expiration of an application. If it's expired the script simulates a freeze and a crash.
*********************************************************************/
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace BestBefore
{

    /// <summary>
    /// This script manages the expiration of an application.
    /// If it's expired the script simulates a freeze and a crash.
    /// </summary>
    public class BestBefore : MonoBehaviour
    {
        public enum ExpirationModes
        {
            FixedDate,
            FixedDateFromBuild,
            FromFirstStart
        }

        public enum MonthsOfYear
        {
            January = 1,
            Februar=2,
            March=3,
            April=4,
            May=5,
            June=6,
            July=7,
            August=8,
            September=9,
            October=10,
            November=11,
            December=12
        }

        public enum CheckModes
        {
            OnStart = 0,
            EachFrame
        }

        private const int numberOfTries = 10;
        private const string bestBeforeFirstRun = "bestbeforefirststart";


        #region SETTINGS
        [Header( "Settings" )]
        public ExpirationModes ExpirationMode = ExpirationModes.FixedDateFromBuild;
        /// <summary>
        /// [REQUIRES INTERNET CONNECTION] If checked, the current date and time will be fetched from a remote web server, else local system time will be used
        /// </summary>
        [Tooltip( "[REQUIRES INTERNET CONNECTION] If checked, the current date and time will be fetched from a remote web server, else local system time will be used" )]
        public bool RemoteDateTimeCheck = false;
        /// <summary>
        /// If Check Mode is set to On Start the expiration check is performed only when Start is called. If Each Frame is selected, the check is performed once per frame
        /// </summary>
        [Tooltip( "If Check Mode is set to On Start the expiration check is performed only when Start is called. If Each Frame is selected, the check is performed once per frame" )]
        public CheckModes CheckMode = CheckModes.OnStart;
        /// <summary>
        /// [SECONDS] If application is expired, it's possible to set a time period while the application will look as freezed. If Freeze Time has a negative value, no freeze effect will be applied
        /// </summary>
        [Tooltip( "[SECONDS] If application is expired, it's possible to set a time period while the application will look as freezed. If Freeze Time has a negative value, no freeze effect will be applied" )]
        public float FreezeTime = -1;
        /// <summary>
        /// [SECONDS] If application is expired, Quit Time defines the time to wait before the application will quit. If Quit Time has a negative value, the application will not quit
        /// </summary>
        [Tooltip( "[SECONDS] If application is expired, Quit Time defines the time to wait before the application will quit. If Quit Time has a negative value, the application will not quit" )]
        public float QuitTime = -1;
        #endregion

        #region EVENTS
        [Header( "Events" )]
        /// <summary>
        /// Custom event raised when the application is expired. It is played just before freezing and quitting
        /// </summary>
        [Tooltip( "Custom event raised when the application is expired. It is played just before freezing and quitting" )]
        public BestBeforeEvent OnExpired;
        /// <summary>
        /// Operations to be done before the application quits. You might, for instance, save some data on local disk or send any data to a remote server
        /// </summary>
        [Tooltip( "Operations to be done before the application quits. You might, for instance, save some data on local disk or send any data to a remote server" )]
        public BestBeforeEvent OnQuit;
        #endregion


        #region FROM FIRST START
        [Header( "First Start Date (read-only)" )]
        [Range( 1, 9999 )]
        public int FirstStartYear = 2017;
        public MonthsOfYear FirstStartMonth = MonthsOfYear.January;
        [Range( 1, 31 )]
        public int FirstStartDay = 1;
        [Range( 0, 23 )]
        public int FirstStartHour = 0;
        [Range( 0, 59 )]
        public int FirstStartMinute = 0;
        [Range( 0, 59 )]
        public int FirstStartSecond = 0;

        [Header( "Expiration Period From First Start" )]
        public int Days = 1;
        public int Hours = 0;
        public int Minutes = 0;
        public int Seconds = 0;
        #endregion

        #region FIXED DATE
        [Header( "Expiration Date" )]
        public int DaysOut = 40;
        [Range( 2022, 9999 )]
        public int Year = 2022;
        public MonthsOfYear Month = MonthsOfYear.January;
        [Range( 1, 31 )]
        public int Day = 1;
        [Range( 0, 23 )]
        public int Hour = 0;
        [Range( 0, 59 )]
        public int Minute = 0;
        [Range( 0, 59 )]
        public int Second = 0;
        #endregion
 
        // #region FIXED DATE FROM BUILD
        //
        // #endregion

        private bool checkingRemoteTime
        {
            get;
            set;
        }
        private DateTime Now
        {
            get
            {
                if ( RemoteDateTimeCheck )
                    return remoteBaseNow.AddSeconds( Time.realtimeSinceStartup - remoteBaseNowTime );

                return DateTime.Now;

            }
        }
        private DateTime FirstStart
        {
            get
            {
                if ( firstStartTicks >= 0 )
                    return new DateTime( firstStartTicks );

                if ( !AlreadyStarted )
                {
                    DateTime now = Now;
                    firstStartTicks = now.Ticks;
                    PlayerPrefs.SetString( bestBeforeFirstRun, firstStartTicks.ToString() );
                    enabled = false;
                }
                else
                {
                    string firstRunString = PlayerPrefs.GetString( bestBeforeFirstRun );
                    firstStartTicks = long.Parse( firstRunString );
                }

                return new DateTime( firstStartTicks );
            }
        }
        private bool AlreadyStarted
        {
            get
            {
                return PlayerPrefs.HasKey( bestBeforeFirstRun );
            }
        }
        private long firstStartTicks = -1;

        private bool expired = false;
        private float expiredCounter = 0;
        private float originalTimeScale = 1;
        private DateTime remoteBaseNow;
        private float remoteBaseNowTime = 0;
        private bool remoteDateTimeOK = false;
        private bool readyForExpirationCheck = false;


        void Awake()
        {
            originalTimeScale = Time.timeScale;
        }

        private void OnEnable()
        {
            StartCoroutine( InitStart() );
        }

        void Start()
        {
            StartCoroutine( InitStart() );
        }

        void OnDisable()
        {
            Time.timeScale = originalTimeScale;
        }

        void OnDestroy()
        {
            Time.timeScale = originalTimeScale;
        }

        void Update()
        {
            if ( ( expired ) ||
               ( !readyForExpirationCheck ) ||
               ( ( RemoteDateTimeCheck ) && ( !remoteDateTimeOK ) ) )
                return;

            if ( CheckMode == CheckModes.EachFrame )
                CheckExpiration();

            if ( !expired )
                return;

            if ( OnExpired != null )
                OnExpired.Invoke();

            if ( FreezeTime > 0 )
                StartCoroutine( WaitAndFreeze() );

            if ( QuitTime > 0 )
                StartCoroutine( WaitAndQuit() );
        }

        private IEnumerator WaitAndFreeze()
        {
            if ( FreezeTime > 0 )
            {
                Time.timeScale = 0;

                yield return new WaitForSecondsRealtime( FreezeTime );

                Time.timeScale = originalTimeScale;
            }
        }

        private IEnumerator WaitAndQuit()
        {
            if ( QuitTime > 0 )
            {
                yield return new WaitForSecondsRealtime( QuitTime + Math.Max( 0, FreezeTime ) );

#if UNITY_EDITOR
                Debug.Log( "quitting" );
#endif
                Application.Quit();
            }
        }

#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WII || UNITY_IOS || UNITY_PS4 || UNITY_SAMSUNGTV || UNITY_XBOXONE || UNITY_TIZEN || UNITY_TVOS || UNITY_WEBGL || UNITY_FACEBOOK
        void OnApplicationQuit()
        {
            //HIGH TODO:  aspettare sempre che checkingRemoteTime sia true prima di eseguire il codice seguente (per tutte le callback)

            if ( !CheckExpiration() )
                return;
#if UNITY_IOS
            if ( PlayerSettings.iOS.exitOnSuspend)
#endif
            if ( OnQuit != null )
                OnQuit.Invoke();
        }
#endif

#if UNITY_EDITOR || UNITY_IOS || UNITY_ANDROID
        void OnApplicationPause( bool pause )
        {
            if ( pause )
            {
                if ( !CheckExpiration() )
                    return;

#if UNITY_IOS
            if (! PlayerSettings.iOS.exitOnSuspend)
#endif
                if ( OnQuit != null )
                    OnQuit.Invoke();
            }
        }
#endif

#if UNITY_EDITOR || UNITY_ANDROID || UNITY_WSA || UNITY_WSA_10_0 || UNITY_WINRT || UNITY_WINRT_10_0
        void OnApplicationFocus( bool focus )
        {
            if ( !focus )
            {
                if ( !CheckExpiration() )
                    return;

                if ( OnQuit != null )
                    OnQuit.Invoke();
            }
        }
#endif
        //HIGH TODO: ripristinare OnApplicationFocus UNITY_EDITOR


        private IEnumerator InitStart()
        {
            readyForExpirationCheck = false;
            if ( RemoteDateTimeCheck )
            {
                int tries = 0;
                while ( ( !remoteDateTimeOK ) && ( tries < numberOfTries ) )
                {
                    StartCoroutine( GetRemoteDateTime() );
                    yield return new WaitForSeconds( 3 );
                    tries++;
                }

                if ( !remoteDateTimeOK )
                    RemoteDateTimeCheck = false;
            }

            InitExpirationPeriod();
            readyForExpirationCheck = true;

            if ( CheckMode == CheckModes.OnStart )
            {
                if ( CheckExpiration() )
                {
                    if ( OnExpired != null )
                        OnExpired.Invoke();

                    if ( FreezeTime > 0 )
                        StartCoroutine( WaitAndFreeze() );

                    if ( QuitTime > 0 )
                        StartCoroutine( WaitAndQuit() );
                }
            }

            yield return null;
        }

        private void InitExpirationPeriod()
        {
            if ( ExpirationMode != ExpirationModes.FromFirstStart )
                return;

            UpdateFirstStartFields( FirstStart );

            TimeSpan expirationTimeSpan = new TimeSpan( Days, Hours, Minutes, Seconds );
            DateTime expirationDate = FirstStart + expirationTimeSpan;
            Year = expirationDate.Year;
            Month = (MonthsOfYear)expirationDate.Month;
            Day = expirationDate.Day;
            Hour = expirationDate.Hour;
            Minute = expirationDate.Minute;
            Second = expirationDate.Second;
        }

        private IEnumerator GetRemoteDateTime()
        {
            checkingRemoteTime = true;
            var url = @"http://epiccube.org/BestBefore/GetDate.php";
#pragma warning disable CS0618 // Il tipo o il membro è obsoleto
            WWW www = new WWW( url );
#pragma warning restore CS0618 // Il tipo o il membro è obsoleto

            // Wait for download to complete
            yield return www;

            try
            {
                string remoteTime = www.text;
                remoteTime = remoteTime.Trim( '"' );
                string[] timeComponents = remoteTime.Split( ',' );

                int year = int.Parse( timeComponents[0] );
                int month = int.Parse( timeComponents[1] );
                int day = int.Parse( timeComponents[2] );
                int hours = int.Parse( timeComponents[3] );
                int minutes = int.Parse( timeComponents[4] );
                int seconds = int.Parse( timeComponents[5] );

                remoteBaseNow = new DateTime( year, month, day, hours, minutes, seconds, DateTimeKind.Utc ).ToLocalTime();
                remoteBaseNowTime = Time.realtimeSinceStartup;
                remoteDateTimeOK = true;
            }
            catch ( System.Exception ex )
            {
                Debug.LogException( ex );
                Debug.LogError( "an error occurred while fetching remote date and time. Using local date time." );
                remoteBaseNow = DateTime.Now;
            }
            finally
            {
                checkingRemoteTime = false;
            }
        }

        private bool CheckExpiration()
        {
            if ( !readyForExpirationCheck )
                return false;

            DateTime referenceTime;

            if ( ExpirationMode == ExpirationModes.FromFirstStart )
                referenceTime = FirstStart + new TimeSpan( Days, Hours, Minutes, Seconds );
            else
                referenceTime = new DateTime( Year, (int)Month, Day, Hour, Minute, Second );

            expired = ( Now.Ticks >= referenceTime.Ticks );
            expiredCounter = Time.realtimeSinceStartup;


            return expired;
        }

        private void UpdateFirstStartFields( DateTime time )
        {
            FirstStartYear = time.Year;
            FirstStartMonth = (MonthsOfYear)time.Month;
            FirstStartDay = time.Day;
            FirstStartHour = time.Hour;
            FirstStartMinute = time.Minute;
            FirstStartSecond = time.Second;
        }

    }

    [System.Serializable]
    public class BestBeforeEvent : UnityEvent
    { }

}
