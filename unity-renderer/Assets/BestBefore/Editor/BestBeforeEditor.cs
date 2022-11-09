/********************************************************************
	created:	2018/11/08
	file base:	BestBeforeEditor
	file ext:	cs
	author:		Alessandro Maione
	version:	1.0.0
	
	purpose:	custom editor for BestBefore script
*********************************************************************/

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace BestBefore
{
    [CustomEditor( typeof( BestBefore ) )]
    [CanEditMultipleObjects]
    public class BestBeforeEditor : Editor
    {
        private Dictionary<string, SerializedProperty> allProperties = new Dictionary<string, SerializedProperty>();
        private FontStyle headerFontStyle = FontStyle.Bold;


        void OnEnable()
        {
            allProperties.Clear();
            if ( allProperties.Count > 0 )
                return;

            #region SETTINGS
            AddProperty( "ExpirationMode" );
            AddProperty( "RemoteDateTimeCheck" );
            AddProperty( "CheckMode" );
            AddProperty( "FreezeTime" );
            AddProperty( "QuitTime" );
            #endregion

            #region EVENTS
            AddProperty( "OnExpired" );
            AddProperty( "OnQuit" );
            #endregion

            #region FROM FIRST START
            AddProperty( "FirstStartYear" );
            AddProperty( "FirstStartMonth" );
            AddProperty( "FirstStartDay" );
            AddProperty( "FirstStartHour" );
            AddProperty( "FirstStartMinute" );
            AddProperty( "FirstStartSecond" );

            AddProperty( "Days" );
            AddProperty( "Hours" );
            AddProperty( "Minutes" );
            AddProperty( "Seconds" );
            #endregion

            #region FIXED DATE
            AddProperty( "DaysOut" );
            AddProperty( "Year" );
            AddProperty( "Month" );
            AddProperty( "Day" );
            AddProperty( "Hour" );
            AddProperty( "Minute" );
            AddProperty( "Second" );
            #endregion

        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            BestBefore bestBefore = (BestBefore)target;

            #region SETTINGS
            EditorGUILayout.PropertyField( allProperties["ExpirationMode"] );
            EditorGUILayout.PropertyField( allProperties["RemoteDateTimeCheck"] );
            EditorGUILayout.PropertyField( allProperties["CheckMode"] );
            EditorGUILayout.PropertyField( allProperties["FreezeTime"] );
            EditorGUILayout.PropertyField( allProperties["QuitTime"] );
            #endregion

            if ( bestBefore.ExpirationMode == BestBefore.ExpirationModes.FromFirstStart )
            {
                EditorGUILayout.PropertyField( allProperties["Days"] );
                EditorGUILayout.PropertyField( allProperties["Hours"] );
                EditorGUILayout.PropertyField( allProperties["Minutes"] );
                EditorGUILayout.PropertyField( allProperties["Seconds"] );

                EditorGUILayout.Space();
                EditorGUILayout.LabelField( "First Start Date (read-only - updated on play)", new GUIStyle() { fontStyle = headerFontStyle } );
                EditorGUILayout.LabelField( "First Start Year", bestBefore.FirstStartYear.ToString() );
                EditorGUILayout.LabelField( "First Start Month", bestBefore.FirstStartMonth.ToString() );
                EditorGUILayout.LabelField( "First Start Day", bestBefore.FirstStartDay.ToString() );
                EditorGUILayout.LabelField( "First Start Hour", bestBefore.FirstStartHour.ToString() );
                EditorGUILayout.LabelField( "First Start Minute", bestBefore.FirstStartMinute.ToString() );
                EditorGUILayout.LabelField( "First Start Second", bestBefore.FirstStartSecond.ToString() );

                EditorGUILayout.Space();
                EditorGUILayout.LabelField( "Expiration Date (read-only - updated on play)", new GUIStyle() { fontStyle = headerFontStyle } );
                EditorGUILayout.LabelField( "Year", bestBefore.Year.ToString() );
                EditorGUILayout.LabelField( "Month", bestBefore.Month.ToString() );
                EditorGUILayout.LabelField( "Day", bestBefore.Day.ToString() );
                EditorGUILayout.LabelField( "Hour", bestBefore.Hour.ToString() );
                EditorGUILayout.LabelField( "Minute", bestBefore.Minute.ToString() );
                EditorGUILayout.LabelField( "Second", bestBefore.Second.ToString() );
            }
            else if ( bestBefore.ExpirationMode == BestBefore.ExpirationModes.FixedDateFromBuild)
            {
                EditorGUILayout.PropertyField(allProperties["DaysOut"]);
                EditorGUILayout.PropertyField( allProperties["Year"] );
                EditorGUILayout.PropertyField( allProperties["Month"] );
                EditorGUILayout.PropertyField( allProperties["Day"] );
                EditorGUILayout.PropertyField( allProperties["Hour"] );
                EditorGUILayout.PropertyField( allProperties["Minute"] );
                EditorGUILayout.PropertyField( allProperties["Second"] );
            }
            
            else
            {
                EditorGUILayout.PropertyField( allProperties["Year"] );
                EditorGUILayout.PropertyField( allProperties["Month"] );
                EditorGUILayout.PropertyField( allProperties["Day"] );
                EditorGUILayout.PropertyField( allProperties["Hour"] );
                EditorGUILayout.PropertyField( allProperties["Minute"] );
                EditorGUILayout.PropertyField( allProperties["Second"] );
            }

            #region EVENTS
            EditorGUILayout.PropertyField( allProperties["OnExpired"] );
            EditorGUILayout.PropertyField( allProperties["OnQuit"] );
            #endregion

            serializedObject.ApplyModifiedProperties();
        }

        protected SerializedProperty AddProperty( string propertyName )
        {
            SerializedProperty prop = serializedObject.FindProperty( propertyName );
            if ( prop != null )
                allProperties.Add( propertyName, prop );
            else
                Debug.LogWarning( "property " + propertyName + " not found in " + target.GetType() );

            return prop;
        }
    }
}
