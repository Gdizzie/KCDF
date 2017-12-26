#if UNITY_3_5 || UNITY_3_4 || UNITY_3_3 || UNITY_3_2 || UNITY_3_1 || UNITY_3_0
#define UNITY_3_x
#endif

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


abstract public class EditorEx : Editor
{
    protected GUIStyle styleLabel;
    protected GUIStyle styleUnit;
    protected GUIStyle styleFloat;
    protected GUIStyle stylePopup;
    protected GUIStyle styleEnum;

    bool setFocusNextField;
    bool userChanges;
    int fieldIndex;

    virtual protected void LogUndo( string label )
    {

    }

    protected void SetFocusForNextEditableField()
    {
        setFocusNextField = true;
    }

    protected void ShowFloat( float f, string label )
    {
        EditorGUILayout.LabelField( label, f.ToString() );
    }

    protected void ShowString( string text, string label )
    {
        EditorGUILayout.LabelField( label, text );
    }

    private float GetFloat( float f, string label, string unit )
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label( label, styleLabel );
        //EditorGUILayout.Space();
        float f_ret = EditorGUILayout.FloatField( f, styleFloat );

        if ( !string.IsNullOrEmpty( unit ) )
        {
            GUILayout.Label( unit, styleUnit );
        }
        else
        {
            GUILayout.Label( " ", styleUnit );
        }
        
        EditorGUILayout.EndHorizontal();

        //float f_ret = EditorGUILayout.FloatField( label, f, styleFloat );
        return f_ret;
    }

    private int GetInt( int f, string label, string unit )
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label( label, styleLabel );
        //EditorGUILayout.Space();
        int f_ret = EditorGUILayout.IntField( f, styleFloat );
        if ( !string.IsNullOrEmpty( unit ) )
        {
            GUILayout.Label( unit, styleUnit );
        }
        else
        {
            GUILayout.Label( " ", styleUnit );
        }
        EditorGUILayout.EndHorizontal();
        return f_ret;
    }

    private float GetFloat( float f, string label, float sliderMin, float sliderMax, string unit )
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label( label, styleLabel );
        //EditorGUILayout.Space();
        float f_ret = f;
        f_ret = EditorGUILayout.FloatField( f_ret, styleFloat, GUILayout.Width( 50 ) );
        f_ret = GUILayout.HorizontalSlider( f_ret, sliderMin, sliderMax );

        if ( !string.IsNullOrEmpty( unit ) )
        {
            GUILayout.Label( unit, styleUnit );
        }
        else
        {
            GUILayout.Label( " ", styleUnit );
        }

        EditorGUILayout.EndHorizontal();
        return f_ret;
    }

    private float GetFloatPercent( float f, string label, string unit )
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label( label, styleLabel );
        //EditorGUILayout.Space();
        float f_ret = f;
        f_ret = (float)EditorGUILayout.IntField( Mathf.RoundToInt( f_ret * 100 ), styleFloat, GUILayout.Width( 50 ) ) / 100;
        f_ret = GUILayout.HorizontalSlider( f_ret, 0, 1 );

        if ( !string.IsNullOrEmpty( unit ) )
        {
            GUILayout.Label( unit, styleUnit );
        }
        else
        {
            GUILayout.Label( " ", styleUnit );
        }

        EditorGUILayout.EndHorizontal();
        return f_ret;
    }

    private float GetFloatPlusMinusPercent( float f, string label, string unit )
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label( label, styleLabel );
        //EditorGUILayout.Space();
        float f_ret = f;
        f_ret = (float) EditorGUILayout.IntField( Mathf.RoundToInt( f_ret * 100 ), styleFloat, GUILayout.Width( 50 ) ) / 100;
        f_ret = GUILayout.HorizontalSlider( f_ret, -1, 1 );
        if ( !string.IsNullOrEmpty( unit ) )
        {
            GUILayout.Label( unit, styleUnit );
        }
        else
        {
            GUILayout.Label( " ", styleUnit );
        }
        EditorGUILayout.EndHorizontal();
        return f_ret;
    }

    protected bool EditFloat( ref float f, string label )
    {
        float new_f = GetFloat( f, label, null );

        if ( new_f != f )
        {
            LogUndo( label );
            f = new_f;
            return true;
        }

        return false;
    }

    protected bool EditFloat( ref float f, string label, string unit )
    {
        float new_f = GetFloat( f, label, unit );

        if ( new_f != f )
        {
            LogUndo( label );
            f = new_f;
            return true;
        }

        return false;
    }

    private float GetFloat01( float f, string label )
    {
        return Mathf.Clamp01( GetFloatPercent( f, label, null ) );
    }

    private float GetFloat01( float f, string label, string unit )
    {
        return Mathf.Clamp01( GetFloatPercent( f, label, unit ) );
    }

    private float GetFloatPlusMinus1( float f, string label, string unit )
    {
        return Mathf.Clamp( GetFloatPlusMinusPercent( f, label, unit ), -1, 1 );
    }

    private float GetFloatWithinRange( float f, string label, float minValue, float maxValue )
    {
        return Mathf.Clamp( GetFloat( f, label, minValue, maxValue, null ), minValue, maxValue );
    }

    protected bool EditFloatWithinRange( ref float f, string label, float minValue, float maxValue )
    {
        float new_f = GetFloatWithinRange( f, label, minValue, maxValue );

        if ( new_f != f )
        {
            LogUndo( label );
            f = new_f;
            return true;
        }

        return false;
    }

    protected bool EditInt( ref int f, string label )
    {
        int new_f = GetInt( f, label, null );

        if ( new_f != f )
        {
            LogUndo( label );
            f = new_f;
            return true;
        }
        return false;
    }

    protected bool EditInt( ref int f, string label, string unit )
    {
        int new_f = GetInt( f, label, unit );

        if ( new_f != f )
        {
            LogUndo( label );
            f = new_f;
            return true;
        }
        return false;
    }
    
    protected bool EditFloat01( ref float f, string label )
    {
        float new_f = GetFloat01( f, label);

        if ( new_f != f )
        {
            LogUndo( label );
            f = new_f;
            return true;
        }

        return false;
    }

    protected bool EditFloat01( ref float f, string label, string unit )
    {
        float new_f = GetFloat01( f, label, unit );

        if ( new_f != f )
        {
            LogUndo( label );
            f = new_f;
            return true;
        }

        return false;
    }

    protected bool EditFloatPlusMinus1( ref float f, string label, string unit )
    {
        float new_f = GetFloatPlusMinus1( f, label, unit );

        if ( new_f != f )
        {
            LogUndo( label );
            f = new_f;
            return true;
        }

        return false;

    }

    private bool GetBool( bool b, string label )
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label( label, styleLabel );
        //EditorGUILayout.Space();
        bool b_ret = EditorGUILayout.Toggle( b, GUILayout.Width( 20 ) );
        EditorGUILayout.EndHorizontal();
        return b_ret;
    }

    protected bool EditBool( ref bool b, string label ) // returns was changed state
    {
        bool new_b = GetBool( b, label );

        if ( new_b != b )
        {
            LogUndo( label );
            b = new_b;
            return true;
        }

        return false;
    }

    protected bool EditPrefab<T>( ref T prefab, string label ) where T : UnityEngine.Object
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label( label, styleLabel );
        T new_f = (T) EditorGUILayout.ObjectField( prefab, typeof( T ), false );
        EditorGUILayout.EndHorizontal();

        if ( new_f != prefab )
        {
            LogUndo( label );
            prefab = new_f;
            return true;
        }
        return false;
    }

    protected bool EditString( ref string txt, string label, GUIStyle styleText = null )
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label( label, styleLabel );
        //EditorGUILayout.Space();
        BeginEditableField();

        string newTxt;
        if ( styleText != null )
        {
            newTxt = EditorGUILayout.TextField( txt, styleText );
        }
        else
        {
            newTxt = EditorGUILayout.TextField( txt );
        }
        EndEditableField();
        EditorGUILayout.EndHorizontal();

        if ( newTxt != txt )
        {
            LogUndo( label );
            txt = newTxt;
            return true;
        }
        return false;
    }

    protected int Popup( string label, int selectedIndex, string[ ] content )
    {
        return Popup( label, selectedIndex, content, stylePopup );
    }

    protected int Popup( string label, int selectedIndex, string[] content, GUIStyle style )
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label( label, styleLabel );
        int newIndex = EditorGUILayout.Popup( selectedIndex, content, style );
        EditorGUILayout.EndHorizontal();
        if ( newIndex != selectedIndex )
        {
            LogUndo( label );
        }
        return newIndex;
    }

    protected Enum EnumPopup( string label, Enum selectedEnum )
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label( label, styleLabel );
        Enum newEnum = EditorGUILayout.EnumPopup( selectedEnum, styleEnum );
        EditorGUILayout.EndHorizontal();

        if ( !object.Equals( newEnum, selectedEnum ) )
        {
            LogUndo( label );
        }
        return newEnum;
    }

    private void EndEditableField()
    {
        if ( setFocusNextField )
        {
            setFocusNextField = false;
            //GUI.FocusControl( GetCurrentFieldControlName() );  // TODO: not working for some reason
            //Debug.Log( "Set focus to: " + GetCurrentFieldControlName() );
            //Debug.Log( "Currently focused: " + GUI.GetNameOfFocusedControl() );
        }
    }

    private void BeginEditableField()
    {
        fieldIndex++;
        if ( setFocusNextField )
        {
            GUI.SetNextControlName( GetCurrentFieldControlName() );
        }
    }

    private String GetCurrentFieldControlName()
    {
        return string.Format( "field{0}", fieldIndex );
    }

    protected void BeginInspectorGUI()
    {
        serializedObject.Update();
        SetStyles();

        fieldIndex = 0;
        userChanges = false;
    }

    protected void SetStyles()
    {
        styleLabel = new GUIStyle( EditorStyles.label );
        styleUnit = new GUIStyle( EditorStyles.label );
        styleFloat = new GUIStyle( EditorStyles.numberField );
        stylePopup = new GUIStyle( EditorStyles.popup );
        styleEnum = new GUIStyle( EditorStyles.popup );
        //styleFloat.alignment = TextAnchor.MiddleRight;
        //styleFloat.fixedWidth = 100;
        //styleFloat.stretchWidth = true;
        //styleFloat.contentOffset = new Vector2( styleFloat.contentOffset.x + 50, styleFloat.contentOffset.y );
        //GUILayout.Width( 60 );
        styleLabel.fixedWidth = 180;
        styleUnit.fixedWidth = 65;
    }

    protected void EndInspectorGUI()
    {
        if ( GUI.changed || userChanges )
        {
            EditorUtility.SetDirty( target );
        }
        serializedObject.ApplyModifiedProperties();
    }

    protected void KeepChanges()
    {
        userChanges = true;
        //EditorUtility.SetDirty( target );
    }
}

[CustomEditor( typeof(AudioController) )]
public class AudioController_Editor : EditorEx
{
    AudioController AC;

    int currentCategoryIndex
    {
        get
        {
            return AC._currentInspectorSelection.currentCategoryIndex;
        }
        set
        {
            AC._currentInspectorSelection.currentCategoryIndex = value;
        }
    }
    int currentItemIndex
    {
        get
        {
            return AC._currentInspectorSelection.currentItemIndex;
        }
        set
        {
            AC._currentInspectorSelection.currentItemIndex = value;
        }
    }

    int currentSubitemIndex
    {
        get
        {
            return AC._currentInspectorSelection.currentSubitemIndex;
        }
        set
        {
            AC._currentInspectorSelection.currentSubitemIndex = value;
        }
    }
    
    int currentPlaylistIndex
    {
        get
        {
            return AC._currentInspectorSelection.currentPlaylistIndex;
        }
        set
        {
            AC._currentInspectorSelection.currentPlaylistIndex = value;
        }
    }

    public static bool globalFoldout = true;
    public static bool playlistFoldout = true;
    public static bool categoryFoldout = true;
    public static bool itemFoldout = true;
    public static bool subitemFoldout = true;

    GUIStyle foldoutStyle;
    GUIStyle centeredTextStyle;
    GUIStyle popupStyleColored;
    GUIStyle styleChooseItem;
    GUIStyle textAttentionStyle;
    GUIStyle textAttentionStyleLabel;
    GUIStyle textInfoStyleLabel;

    int lastCategoryIndex = -1;
    int lastItemIndex = -1;
    int lastSubItemIndex = -1;

    AudioCategory currentCategory
    {
        get
        {
            if ( currentCategoryIndex < 0 || AC.AudioCategories == null || currentCategoryIndex >= AC.AudioCategories.Length )
            {
                return null;
            }
            return AC.AudioCategories[ currentCategoryIndex ];
        }
    }
    AudioItem currentItem
    {
        get
        {
            AudioCategory curCategory = currentCategory;

            if ( currentCategory == null )
            {
                return null;
            }

            if ( currentItemIndex < 0 || curCategory.AudioItems == null || currentItemIndex >= curCategory.AudioItems.Length )
            {
                return null;
            }
            return currentCategory.AudioItems[ currentItemIndex ];
        }
    }

    AudioSubItem currentSubItem
    {
        get
        {
            AudioItem curItem = currentItem;

            if ( curItem == null )
            {
                return null;
            }

            if ( currentSubitemIndex < 0 || curItem.subItems == null || currentSubitemIndex >= curItem.subItems.Length )
            {
                return null;
            }
            return curItem.subItems[ currentSubitemIndex ];
        }
    }

    public int currentCategoryCount
    {
        get {
            if( AC.AudioCategories != null )
            {
                return AC.AudioCategories.Length;
            }
            else 
                return 0;
        }
    }

    public int currentItemCount
    {
        get
        {
            if ( currentCategory != null )
            {
                if ( currentCategory.AudioItems != null )
                {
                    return currentCategory.AudioItems.Length;
                }
                return 0;
            }
            else
                return 0;
        }
    }

    public int currentSubItemCount
    {
        get
        {
            if ( currentItem != null )
            {
                if ( currentItem.subItems != null )
                {
                    return currentItem.subItems.Length;
                }
                return 0;
            }
            else
                return 0;
        }
    }

    const string _playWithInspectorNotice = "Volume and pitch of audios are only correct when played during playmode. You can ignore the following Unity warning (if any).";
    const string _playNotSupportedOnMac = "On MacOS playing audios is only supported during play mode.";
    const string _nameForNewCategoryEntry = "!!! Enter Unique Category Name Here !!!";
    const string _nameForNewAudioItemEntry = "!!! Enter Unique Audio ID Here !!!";

    //public void OnEnable()
    //{
        
    //}

    protected override void LogUndo( string label )
    {
        //Debug.Log( "Undo: " + label );
        Undo.RegisterUndo( AC, "AudioToolkit: " + label );
    }

    public new void SetStyles()
    {
        base.SetStyles();

        foldoutStyle = new GUIStyle( EditorStyles.foldout );

        //var foldoutColor = new UnityEngine.Color( 0.3f, 0.75f, 0.75f );
        //var foldoutColor = new UnityEngine.Color( 0.1f, 0.6f, 0.05f );
        var foldoutColor = new UnityEngine.Color( 0.0f, 0.0f, 0.2f );

        //foldoutStyle.normal.background = EditorStyles.boldLabel.onNormal.background;
        //foldoutStyle.focused.background = EditorStyles.boldLabel.onNormal.background;
        //foldoutStyle.active.background = EditorStyles.boldLabel.onNormal.background;
        //foldoutStyle.hover.background = EditorStyles.boldLabel.onNormal.background;

        foldoutStyle.onNormal.background = EditorStyles.boldLabel.onNormal.background;
        foldoutStyle.onFocused.background = EditorStyles.boldLabel.onNormal.background;
        foldoutStyle.onActive.background = EditorStyles.boldLabel.onNormal.background;
        foldoutStyle.onHover.background = EditorStyles.boldLabel.onNormal.background;


        foldoutStyle.normal.textColor = foldoutColor;
        foldoutStyle.focused.textColor = foldoutColor;
        foldoutStyle.active.textColor = foldoutColor;
        foldoutStyle.hover.textColor = foldoutColor;
        foldoutStyle.fixedWidth = 500;

        //foldoutStyle.onNormal.textColor = foldoutColor;
        //foldoutStyle.onFocused.textColor = foldoutColor;
        //foldoutStyle.onActive.textColor = foldoutColor;
        //foldoutStyle.onHover.textColor = foldoutColor;

        centeredTextStyle = new GUIStyle( EditorStyles.label );
        centeredTextStyle.alignment = TextAnchor.UpperCenter;
        centeredTextStyle.stretchWidth = true;

        popupStyleColored = new GUIStyle( stylePopup );
        styleChooseItem = new GUIStyle( stylePopup );

        bool isDarkSkin = popupStyleColored.normal.textColor.grayscale > 0.5f;

        if ( isDarkSkin )
        {
            popupStyleColored.normal.textColor = new Color( 0.9f, 0.9f, 0.5f );
        } else
            popupStyleColored.normal.textColor = new Color( 0.6f, 0.1f, 0.0f );


        textAttentionStyle = new GUIStyle( EditorStyles.textField );

        if ( isDarkSkin )
        {
            textAttentionStyle.normal.textColor = new Color( 1, 0.3f, 0.3f );
        } else
            textAttentionStyle.normal.textColor = new Color( 1, 0f, 0f );

        textAttentionStyleLabel = new GUIStyle( EditorStyles.label );

        if ( isDarkSkin )
        {
            textAttentionStyleLabel.normal.textColor = new Color( 1, 0.3f, 0.3f );
        }
        else
            textAttentionStyleLabel.normal.textColor = new Color( 1, 0f, 0f );

        textInfoStyleLabel = new GUIStyle( EditorStyles.label );

        if ( isDarkSkin )
        {
            textInfoStyleLabel.normal.textColor = new Color( 0.4f, 0.4f, 0.4f );
        }
        else
            textInfoStyleLabel.normal.textColor = new Color( 0.6f, 0.6f, 0.6f );
    }


    public override void OnInspectorGUI()
    {
        SetStyles();

        BeginInspectorGUI();

        AC = (AudioController)target;

        _ValidateCurrentCategoryIndex();
        _ValidateCurrentItemIndex();
        _ValidateCurrentSubItemIndex();

        if( lastCategoryIndex != currentCategoryIndex ||
            lastItemIndex != currentItemIndex ||
            lastSubItemIndex != currentSubitemIndex )
        {
            GUIUtility.keyboardControl = 0; // workaround for Unity weirdness not changing the value of a focused GUI element when changing a category/item
            lastCategoryIndex = currentCategoryIndex;
            lastItemIndex = currentItemIndex;
            lastSubItemIndex = currentSubitemIndex;
        }

        

        EditorGUILayout.Space();

        if ( globalFoldout = EditorGUILayout.Foldout( globalFoldout, "Global Audio Settings", foldoutStyle ) )
        {
            EditBool( ref AC.Persistent, "Persist Scene Loading" );

            bool currentlyDisabled = AC.DisableAudio;

            bool changed = EditBool( ref currentlyDisabled, "Disable Audio" );
            if( changed )
            {
                AC.DisableAudio = currentlyDisabled;
                if ( currentlyDisabled && AudioController.DoesInstanceExist() )
                {
                    AudioController.StopAll();
                }
            }

            float vol = AC.Volume;
            
            EditFloat01( ref vol, "Volume", "%" );

            AC.Volume = vol;

            EditPrefab( ref AC.AudioObjectPrefab, "Audio Object Prefab" );
            EditBool( ref AC.UsePooledAudioObjects, "Use Pooled AudioObjects" );
            EditBool( ref AC.PlayWithZeroVolume, "Play With Zero Volume" );
        }

        VerticalSpace();

        // playlist specific
        if ( playlistFoldout = EditorGUILayout.Foldout( playlistFoldout, "Music & Playlist Settings", foldoutStyle ) )
        {
            EditorGUILayout.BeginHorizontal();
            currentPlaylistIndex = Popup( "Playlist", currentPlaylistIndex, GetPlaylistNames() );
            if ( GUILayout.Button( "Up", GUILayout.Width( 35 ) ) && AC.musicPlaylist != null && AC.musicPlaylist.Length > 0 )
            {
                if ( SwapArrayElements( AC.musicPlaylist, currentPlaylistIndex, currentPlaylistIndex - 1 ) )
                {
                    currentPlaylistIndex--;
                    KeepChanges();
                }
            }
            if ( GUILayout.Button( "Dwn", GUILayout.Width( 40 ) ) && AC.musicPlaylist != null && AC.musicPlaylist.Length > 0 )
            {
                if ( SwapArrayElements( AC.musicPlaylist, currentPlaylistIndex, currentPlaylistIndex + 1 ) )
                {
                    currentPlaylistIndex++;
                    KeepChanges();
                }
            }
            if ( GUILayout.Button( "-", GUILayout.Width( 25 ) ) && AC.musicPlaylist != null && AC.musicPlaylist.Length > 0 )
            {
                ArrayHelper.DeleteArrayElement( ref AC.musicPlaylist, currentPlaylistIndex );
                currentPlaylistIndex = Mathf.Clamp( currentPlaylistIndex - 1, 0, AC.musicPlaylist.Length - 1 );
                KeepChanges();
            }

            EditorGUILayout.EndHorizontal();

            string itemToAdd = _ChooseItem( "Add to Playlist" );
            if ( !string.IsNullOrEmpty( itemToAdd ) )
            {
                AddToPlayList( itemToAdd );
            }
            
            EditBool( ref AC.loopPlaylist, "Loop Playlist" );
            EditBool( ref AC.shufflePlaylist, "Shuffle Playlist" );
            EditBool( ref AC.crossfadePlaylist, "Crossfade Playlist" );
            EditBool( ref AC.specifyCrossFadeInAndOutSeperately, "Separate crossfade in/out" );
            if ( AC.specifyCrossFadeInAndOutSeperately )
            {
                float v_in = AC.musicCrossFadeTime_In;
                EditFloat( ref v_in, "   Music Crossfade-in Time", "sec" ); AC.musicCrossFadeTime_In = v_in;

                float v_out = AC.musicCrossFadeTime_Out;
                EditFloat( ref v_out, "   Music Crossfade-out Time", "sec" ); AC.musicCrossFadeTime_Out = v_out;
            }
            else
            {
                EditFloat( ref AC.musicCrossFadeTime, "Music Crossfade Time", "sec" );
            }
            EditFloat( ref AC.delayBetweenPlaylistTracks, "Delay Betw. Playlist Tracks", "sec" );
        }

        VerticalSpace();

        int categoryCount = AC.AudioCategories != null ? AC.AudioCategories.Length : 0;
        currentCategoryIndex = Mathf.Clamp( currentCategoryIndex, 0, categoryCount - 1 );

        if ( categoryFoldout = EditorGUILayout.Foldout( categoryFoldout, "Category Settings", foldoutStyle ) )
        {

            // Audio Items 
            EditorGUILayout.BeginHorizontal();

            bool justCreatedNewCategory = false;

            int newCategoryIndex = Popup( "Category", currentCategoryIndex, GetCategoryNames(), popupStyleColored );
            if ( GUILayout.Button( "+", GUILayout.Width( 30 ) ) )
            {
                bool lastEntryIsNew = false;

                if ( categoryCount > 0 )
                {
                    lastEntryIsNew = AC.AudioCategories[ currentCategoryIndex ].Name == _nameForNewCategoryEntry;
                }

                if ( !lastEntryIsNew )
                {
                    newCategoryIndex = AC.AudioCategories != null ? AC.AudioCategories.Length : 0;
                    ArrayHelper.AddArrayElement( ref AC.AudioCategories, new AudioCategory( AC ) );
                    AC.AudioCategories[ newCategoryIndex ].Name = _nameForNewCategoryEntry;
                    justCreatedNewCategory = true;
                    KeepChanges();
                }
            }

            if ( GUILayout.Button( "-", GUILayout.Width( 30 ) ) && categoryCount > 0 )
            {

                if ( currentCategoryIndex < AC.AudioCategories.Length - 1 )
                {
                    newCategoryIndex = currentCategoryIndex;
                }
                else
                {
                    newCategoryIndex = Mathf.Max( currentCategoryIndex - 1, 0 );
                }
                ArrayHelper.DeleteArrayElement( ref AC.AudioCategories, currentCategoryIndex );
                KeepChanges();
            }

            EditorGUILayout.EndHorizontal();

            if ( newCategoryIndex != currentCategoryIndex )
            {
                currentCategoryIndex = newCategoryIndex;
                currentItemIndex = 0;
                currentSubitemIndex = 0;
                _ValidateCurrentItemIndex();
                _ValidateCurrentSubItemIndex();
            }


            AudioCategory curCat = currentCategory;

            if ( curCat != null )
            {
                if ( curCat.audioController == null )
                {
                    curCat.audioController = AC;
                }
                if ( justCreatedNewCategory )
                {
                    SetFocusForNextEditableField();
                }
                EditString( ref curCat.Name, "Name", curCat.Name == _nameForNewCategoryEntry ? textAttentionStyle : null );

                float volTmp = curCat.Volume;
                EditFloat01( ref volTmp, "Volume", " %" );
                curCat.Volume = volTmp;

                EditPrefab( ref curCat.AudioObjectPrefab, "Audio Object Prefab Override" );

                int selectedParentCategoryIndex;

                var catList = _GenerateCategoryListIncludingNone( out selectedParentCategoryIndex, curCat.parentCategory );

                int newIndex = Popup( "Parent Category", selectedParentCategoryIndex < 0 ? 0 : selectedParentCategoryIndex, catList );
                if ( newIndex != selectedParentCategoryIndex )
                {
                    KeepChanges();

                    if ( newIndex <= 0 )
                    {
                        curCat.parentCategory = null;
                    }
                    else
                        curCat.parentCategory = _GetCategory( catList[ newIndex ] );
                }

                int itemCount = currentItemCount;
                _ValidateCurrentItemIndex();

                /*if ( GUILayout.Button( "Add all items in this category to playlist" ) )
                {
                    for ( int i = 0; i < itemCount; i++ )
                    {
                        ArrayHelper.AddArrayElement( ref AC.musicPlaylist, curCat.AudioItems[i].Name );
                    }
                    currentPlaylistIndex = AC.musicPlaylist.Length - 1;
                    KeepChanges();
                }*/



                VerticalSpace();

                AudioItem curItem = currentItem;

                if ( itemFoldout = EditorGUILayout.Foldout( itemFoldout, "Audio Item Settings", foldoutStyle ) )
                {
                    EditorGUILayout.BeginHorizontal();
                    if ( GUILayout.Button( "Add selected audio clips", EditorStyles.miniButton ) )
                    {
                        AudioClip[ ] audioClips = GetSelectedAudioclips();
                        if ( audioClips.Length > 0 )
                        {
                            int firstIndex = itemCount;
                            currentItemIndex = firstIndex;
                            foreach ( AudioClip audioClip in audioClips )
                            {
                                ArrayHelper.AddArrayElement( ref curCat.AudioItems );
                                AudioItem audioItem = curCat.AudioItems[ currentItemIndex ];
                                audioItem.Name = audioClip.name;
                                ArrayHelper.AddArrayElement( ref audioItem.subItems ).Clip = audioClip;
                                currentItemIndex++;
                            }
                            currentItemIndex = firstIndex;
                            KeepChanges();
                        }
                    }

                    GUILayout.Label( "use inspector lock!" );
                    EditorGUILayout.EndHorizontal();

                    // AudioItems

                    EditorGUILayout.BeginHorizontal();

                    int newItemIndex = Popup( "Item", currentItemIndex, GetItemNames(), popupStyleColored );
                    bool justCreatedNewItem = false;


                    if ( GUILayout.Button( "+", GUILayout.Width( 30 ) ) )
                    {
                        bool lastEntryIsNew = false;

                        if ( itemCount > 0 )
                        {
                            lastEntryIsNew = curCat.AudioItems[ currentItemIndex ].Name == _nameForNewAudioItemEntry;
                        }

                        if ( !lastEntryIsNew )
                        {
                            newItemIndex = curCat.AudioItems != null ? curCat.AudioItems.Length : 0;
                            ArrayHelper.AddArrayElement( ref curCat.AudioItems );
                            curCat.AudioItems[ newItemIndex ].Name = _nameForNewAudioItemEntry;
                            justCreatedNewItem = true;
                            KeepChanges();
                        }
                    }

                    if ( GUILayout.Button( "-", GUILayout.Width( 30 ) ) && itemCount > 0 )
                    {
                        if ( currentItemIndex < curCat.AudioItems.Length - 1 )
                        {
                            newItemIndex = currentItemIndex;
                        }
                        else
                        {
                            newItemIndex = Mathf.Max( currentItemIndex - 1, 0 );
                        }
                        ArrayHelper.DeleteArrayElement( ref curCat.AudioItems, currentItemIndex );
                        KeepChanges();
                    }



                    if ( newItemIndex != currentItemIndex )
                    {
                        currentItemIndex = newItemIndex;
                        currentSubitemIndex = 0;
                        _ValidateCurrentSubItemIndex();
                    }

                    curItem = currentItem;
                  
                    EditorGUILayout.EndHorizontal();

                    if ( curItem != null )
                    {
                        GUILayout.BeginHorizontal();
                        if ( justCreatedNewItem )
                        {
                            SetFocusForNextEditableField();
                        }

                        bool isNewDummyName = curItem.Name == _nameForNewAudioItemEntry;

                        string originalName = curItem.Name;

                        if ( EditString( ref curItem.Name, "Name", isNewDummyName ? textAttentionStyle : null ) )
                        {
                            if ( !isNewDummyName )
                            {
                                _RenamePlaylistEntries( originalName, curItem.Name );
                            }
                        }


                        /*if ( GUILayout.Button( "Add to playlist" ) )
                        {
                            AddToPlayList( curItem.Name );
                        }*/

                        GUILayout.EndHorizontal();

                        EditFloat01( ref curItem.Volume, "Volume", " %" );
                        EditFloat( ref curItem.Delay, "Delay", "sec" );
                        EditFloat( ref curItem.MinTimeBetweenPlayCalls, "Min Time Between Play", "sec" );
                        EditInt( ref curItem.MaxInstanceCount, "Max Instance Count" );

                        EditBool( ref curItem.DestroyOnLoad, "Stop When Scene Loads" );
                        curItem.Loop = (AudioItem.LoopMode)EnumPopup( "Loop Mode", curItem.Loop );
                        if ( curItem.Loop == AudioItem.LoopMode.LoopSequenceGapless )
                        {
                            EditBool( ref curItem.use3DForGaplessLoop, "   Use 3D AudioClip" );
                            _CheckCorrectAudioClipsForGaplessLoop();
                        }
                        EditBool( ref curItem.overrideAudioSourceSettings, "Override AudioSource Settings" );

                        if ( curItem.overrideAudioSourceSettings )
                        {
                            //EditorGUI.indentLevel++;

                            EditFloat( ref curItem.audioSource_MinDistance, "   Min Distance", "" );
                            EditFloat( ref curItem.audioSource_MaxDistance, "   Max Distance", "" );

                            //EditorGUI.indentLevel--;
                        }

                        curItem.SubItemPickMode = (AudioPickSubItemMode)EnumPopup( "Pick Subitem Mode", curItem.SubItemPickMode );

                        //EditString( ref curItem.PlayAdditional, "Play Additional" );
                        //EditString( ref curItem.PlayInstead, "Play Instead" );

                        EditorGUILayout.BeginHorizontal();

                        GUILayout.Label( "" );

                        bool isItemNotLooping = ( curItem != null && curItem.Loop == AudioItem.LoopMode.DoNotLoop );

#if UNITY_3_x
                        GUI.enabled = isItemNotLooping;
#else
                        GUI.enabled = _IsAudioControllerInPlayMode() &&  isItemNotLooping;
#endif

                        if ( GUILayout.Button( "Play", GUILayout.Width( 60 ) ) && curItem != null )
                        {
                            if ( _IsAudioControllerInPlayMode() )
                            {
                                AudioController.Play( curItem.Name );
                            }
                            else
                            {
                                if ( Application.platform == RuntimePlatform.OSXEditor )
                                {
                                    Debug.Log( _playNotSupportedOnMac );
                                }
                                else
                                {
                                    AC.InitializeAudioItems();
                                    Debug.Log( _playWithInspectorNotice );
                                    AC.PlayAudioItem( curItem, 1, Vector3.zero, null, 0, 0, true, null );
                                }
                            }
                        }

                        GUI.enabled = true;


                        EditorGUILayout.EndHorizontal();

                        VerticalSpace();

                        int subItemCount = curItem.subItems != null ? curItem.subItems.Length : 0;
                        currentSubitemIndex = Mathf.Clamp( currentSubitemIndex, 0, subItemCount - 1 );
                        AudioSubItem subItem = currentSubItem;

                        if ( subitemFoldout = EditorGUILayout.Foldout( subitemFoldout, "Audio Sub-Item Settings", foldoutStyle ) )
                        {
                            EditorGUILayout.BeginHorizontal();
                            if ( GUILayout.Button( "Add selected audio clips", EditorStyles.miniButton ) )
                            {
                                AudioClip[ ] audioClips = GetSelectedAudioclips();
                                if ( audioClips.Length > 0 )
                                {
                                    int firstIndex = subItemCount;
                                    currentSubitemIndex = firstIndex;
                                    foreach ( AudioClip audioClip in audioClips )
                                    {
                                        ArrayHelper.AddArrayElement( ref curItem.subItems ).Clip = audioClip;
                                        currentSubitemIndex++;
                                    }
                                    currentSubitemIndex = firstIndex;
                                    KeepChanges();
                                }
                            }
                            GUILayout.Label( "use inspector lock!" );
                            EditorGUILayout.EndHorizontal();

                            EditorGUILayout.BeginHorizontal();

                            currentSubitemIndex = Popup( "SubItem", currentSubitemIndex, GetSubitemNames(), popupStyleColored );

                            if ( GUILayout.Button( "+", GUILayout.Width( 30 ) ) )
                            {
                                bool lastEntryIsNew = false;

                                AudioSubItemType curSubItemType = AudioSubItemType.Clip;

                                if ( subItemCount > 0 )
                                {
                                    curSubItemType = curItem.subItems[ currentSubitemIndex ].SubItemType;
                                    if ( curSubItemType == AudioSubItemType.Clip )
                                    {
                                        lastEntryIsNew = curItem.subItems[ currentSubitemIndex ].Clip == null;
                                    }
                                    if ( curSubItemType == AudioSubItemType.Item )
                                    {
                                        lastEntryIsNew = curItem.subItems[ currentSubitemIndex ].ItemModeAudioID == null ||
                                                         curItem.subItems[ currentSubitemIndex ].ItemModeAudioID.Length == 0;
                                    }
                                }

                                if ( !lastEntryIsNew )
                                {
                                    currentSubitemIndex = subItemCount;
                                    ArrayHelper.AddArrayElement( ref curItem.subItems );
                                    curItem.subItems[ currentSubitemIndex ].SubItemType = curSubItemType;
                                    KeepChanges();
                                }
                            }

                            if ( GUILayout.Button( "-", GUILayout.Width( 30 ) ) && subItemCount > 0 )
                            {
                                ArrayHelper.DeleteArrayElement( ref curItem.subItems, currentSubitemIndex );
                                if ( currentSubitemIndex >= curItem.subItems.Length )
                                {
                                    currentSubitemIndex = Mathf.Max( curItem.subItems.Length - 1, 0 );
                                }
                                KeepChanges();
                            }
                            EditorGUILayout.EndHorizontal();

                            subItem = currentSubItem;

                            if ( subItem != null )
                            {
                                _SubitemTypePopup( subItem );
                                

                                if ( subItem.SubItemType == AudioSubItemType.Item )
                                {
                                    _DisplaySubItem_Item( subItem );

                                }
                                else
                                {
                                    _DisplaySubItem_Clip( subItem, subItemCount, curItem );
                                }
                            } 
                        }
                    }
                }
            }
        }

        VerticalSpace();

        EditorGUILayout.BeginHorizontal();

        if ( GUILayout.Button( "Show Audio Log" ) )
        {
            var win = EditorWindow.GetWindow( typeof( AudioLogView ) );
            win.Show();
        }

        if ( GUILayout.Button( "Show Item Overview" ) )
        {
            AudioItemOverview win = EditorWindow.GetWindow( typeof( AudioItemOverview ) ) as AudioItemOverview;
            win.Show( AC );
        }
       
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();

        if ( EditorApplication.isPlaying )
        {
            EditorGUILayout.BeginHorizontal();
            if ( GUILayout.Button( "Stop All Sounds" ) )
            {
                if ( EditorApplication.isPlaying && AudioController.DoesInstanceExist() )
                {
                    AudioController.StopAll();
                }
            }
            if ( GUILayout.Button( "Stop Music Only" ) )
            {
                if ( EditorApplication.isPlaying && AudioController.DoesInstanceExist() )
                {
                    AudioController.StopMusic();
                }
            }
            EditorGUILayout.EndHorizontal();
        }
        
        EditorGUILayout.Space();
        GUILayout.Label( string.Format( "----- ClockStone Audio Toolkit v{0} -----  ", AudioController.AUDIO_TOOLKIT_VERSION ), centeredTextStyle );

        EndInspectorGUI();

        //Debug.Log( "currentCategoryIndex: " + currentCategoryIndex );
    }

    private void _RenamePlaylistEntries( string originalName, string newName )
    {
        if ( AC.musicPlaylist == null ) return;

        for ( int i = 0; i < AC.musicPlaylist.Length; i++ )
        {
            if ( AC.musicPlaylist[ i ] == originalName )
            {
                AC.musicPlaylist[ i ] = newName;
            }
        }
    }

    private void _CheckCorrectAudioClipsForGaplessLoop()
    {
        var item = currentItem;

        string errorTxt = null;

        if ( item.subItems != null  && item.subItems.Length > 0 )
        {
            int i;

            bool firstFound = false;

            int frequ = 0;
            int channels = 0;

            for( i=0; i<item.subItems.Length; i++ )
            {
                var si = item.subItems[ i ];

                if ( si.Clip == null ) continue;

                if ( !firstFound )
                {
                    frequ = si.Clip.frequency;
                    channels = si.Clip.channels;
                    firstFound = true;
                    continue;
                }
            
                if ( si.Clip.channels != channels )
                {
                    errorTxt = "Number of channels must match for gapless looping!";
                }

                if ( si.Clip.frequency != frequ)
                {
                    errorTxt = "Frequencies must match for gapless looping!";
                }
            }
        }

        const string SPACE = "   ";

        if ( errorTxt != null )
        {
            GUILayout.Label( SPACE + errorTxt, textAttentionStyleLabel );
        }
        else
        {
            GUILayout.Label( SPACE + "Info: 'compressed in memory' not allowed for clips" , textInfoStyleLabel );
        }
    }

    private string[] _GenerateCategoryListIncludingNone( out int selectedParentCategoryIndex, AudioCategory selectedAudioCategory )
    {
        string[] names;
        selectedParentCategoryIndex = -1;

        if ( AC.AudioCategories != null )
        {
            names = new string[ AC.AudioCategories.Length ];

            int index = 1;

            var curCat = currentCategory;

            for (int i=0; i< AC.AudioCategories.Length; i++)
            {
                if ( _IsCategoryChildOf( AC.AudioCategories[ i ], curCat ) ) // prevent loops in tree
                {
                    continue;
                }
                names[index] = AC.AudioCategories[i].Name;
                if ( selectedAudioCategory == AC.AudioCategories[ i ] )
                {
                    selectedParentCategoryIndex = index;
                }

                index++;
                if ( index == names.Length )
                {
                    break; // in case currentCategory is not found
                }
            }

            if ( index < names.Length )
            {
                var newNames = new string[ index ];
                Array.Copy( names, newNames, index );
                names = newNames;
            }
        }
        else
        {
            names = new string[ 1 ];
        }

        names[ 0 ] = "*none*";
        return names;
    }

    bool _IsCategoryChildOf( AudioCategory toTest, AudioCategory parent )
    {
        var cat = toTest;
        while ( cat != null )
        {
            if ( cat.audioController == null )
            {
                cat.audioController = AC;
            }

            if ( cat == parent ) return true;

            cat = cat.parentCategory;
        }
        return false;
    }

    private bool _IsAudioControllerInPlayMode()
    {
        return EditorApplication.isPlaying && AudioController.DoesInstanceExist();
    }
    private void _ValidateCurrentCategoryIndex()
    {
        int categoryCount = currentCategoryCount;
        if ( categoryCount > 0 ) currentCategoryIndex = Mathf.Clamp( currentCategoryIndex, 0, categoryCount - 1 );
        else currentCategoryIndex = -1;
    }

    private void _ValidateCurrentSubItemIndex()
    {
        int subitemCount = currentSubItemCount;
        if ( subitemCount > 0 ) currentSubitemIndex = Mathf.Clamp( currentSubitemIndex, 0, subitemCount - 1 );
        else currentSubitemIndex = -1;
    }

    private void _ValidateCurrentItemIndex()
    {
        int itemCount = currentItemCount;
        if ( itemCount > 0 ) currentItemIndex = Mathf.Clamp( currentItemIndex, 0, itemCount - 1 );
        else currentItemIndex = -1;
    }

    private void _SubitemTypePopup( AudioSubItem subItem )
    {
        var typeNames = new string[ 2 ];
        typeNames[ 0 ] = "Single Audio Clip";
        typeNames[ 1 ] = "Other Audio Item";

        int curIndex = 0;
        switch( subItem.SubItemType )
        {
        case AudioSubItemType.Clip: curIndex = 0; break;
        case AudioSubItemType.Item: curIndex = 1; break;
        }

        switch( Popup( "SubItem Type", curIndex, typeNames ) )
        {
        case 0: subItem.SubItemType = AudioSubItemType.Clip; break;
        case 1: subItem.SubItemType = AudioSubItemType.Item; break;
        }

        //subItem.SubItemType = (AudioSubItemType) EnumPopup( "SubItem Type", subItem.SubItemType );
    }

    public void AddToPlayList( string name )
    {
        ArrayHelper.AddArrayElement( ref AC.musicPlaylist, name );
        currentPlaylistIndex = AC.musicPlaylist.Length - 1;
        KeepChanges();
    }

    protected void EditAudioClip( ref AudioClip clip, string label ) 
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label( label, styleLabel );
        clip = (AudioClip) EditorGUILayout.ObjectField( clip, typeof( AudioClip ), false );
        if ( clip )
        {
            EditorGUILayout.Space();
            GUILayout.Label( string.Format( "{0:0.0} sec", clip.length ), GUILayout.Width( 60 ) );
        }
        EditorGUILayout.EndHorizontal();
    }

    private void _DisplaySubItem_Clip( AudioSubItem subItem, int subItemCount, AudioItem curItem )
    {

        // AudioSubItems

        if ( subItem != null )
        {
            EditAudioClip( ref subItem.Clip, "Audio Clip" );

            if ( curItem.Loop != AudioItem.LoopMode.LoopSequenceGapless )
            {
                EditFloat01( ref subItem.Volume, "Volume", " %" );

                EditFloat01( ref subItem.RandomVolume, "Random Volume", "±%" );

                EditFloat( ref subItem.Delay, "Delay", "sec" );
                //EditFloatWithinRange( ref subItem.Pan2D, "Pan2D [left..right]", -1.0f, 1.0f);
                EditFloatPlusMinus1( ref subItem.Pan2D, "Pan2D", "%left/right" );
                if( _IsRandomItemMode( curItem.SubItemPickMode ) )
                {
                    EditFloat01( ref subItem.Probability, "Probability", " %" );
                }
                EditFloat( ref subItem.PitchShift, "Pitch Shift", "semitone" );
                EditFloat( ref subItem.RandomPitch, "Random Pitch", "±semitone" );
                EditFloat( ref subItem.RandomDelay, "Random Delay", "sec" );
                EditFloat( ref subItem.FadeIn, "Fade-in", "sec" );
                EditFloat( ref subItem.FadeOut, "Fade-out", "sec" );           
                EditFloat( ref subItem.ClipStartTime, "Start at", "sec" );
                EditFloat( ref subItem.ClipStopTime, "Stop at", "sec" );
                EditBool( ref subItem.RandomStartPosition, "Random Start Position" );
            }
        }

        EditorGUILayout.BeginHorizontal();

        GUILayout.Label( " " );

#if UNITY_3_x
#else
        GUI.enabled = _IsAudioControllerInPlayMode();
#endif 

        if ( GUILayout.Button( "Play", GUILayout.Width( 60 ) ) && subItem != null )
        {
            if ( _IsAudioControllerInPlayMode() )
            {
                var audioListener = AudioController.GetCurrentAudioListener();
                Vector3 pos;
                if ( audioListener != null )
                {
                    pos = audioListener.transform.position + audioListener.transform.forward;
                }
                else
                    pos = Vector3.zero;

                AudioController.Instance.PlayAudioSubItem( subItem, 1, pos, null, 0, 0, false, null );

            }
            else
            {
                if ( Application.platform == RuntimePlatform.OSXEditor )
                {
                    Debug.Log( _playNotSupportedOnMac );
                }
                else
                {
                    Debug.Log( _playWithInspectorNotice );
                    AC.InitializeAudioItems();
                    AC.PlayAudioSubItem( subItem, 1, Vector3.zero, null, 0, 0, true, null );
                }
            }
        }

        GUI.enabled = true;

        EditorGUILayout.EndHorizontal();

        
    }

    private bool _IsRandomItemMode( AudioPickSubItemMode audioPickSubItemMode )
    {
        switch ( audioPickSubItemMode )
        {
        case AudioPickSubItemMode.Random: return true;
        case AudioPickSubItemMode.RandomNotSameTwice: return true;
        case AudioPickSubItemMode.TwoSimultaneously: return true;
        }
        return false;
    }

    private string _ChooseItem( string label )
    {
        string[ ] possibleAudioIDs_withCategory = _GetPossibleAudioIDs( true, "Choose Audio Item..." );

        int selected  = Popup( label, 0, possibleAudioIDs_withCategory, styleChooseItem );
        if( selected != 0 )
        {
            string[ ] possibleAudioIDs = _GetPossibleAudioIDs( false, "Choose Audio Item..." );
            return possibleAudioIDs[ selected ];
        }
        return null;
    }

    private void _DisplaySubItem_Item( AudioSubItem subItem )
    {
        EditFloat01( ref subItem.Probability, "Probability", " %" );
        int audioIndex = 0;
        string[ ] possibleAudioIDs = _GetPossibleAudioIDs( false, "*undefined*" );
        string[ ] possibleAudioIDs_withCategory = _GetPossibleAudioIDs( true, "*undefined*" );

        if ( subItem.ItemModeAudioID != null && subItem.ItemModeAudioID.Length > 0 )
        {
            string idToSearch = subItem.ItemModeAudioID.ToLowerInvariant();

            for ( int i = 1; i < possibleAudioIDs.Length; i++ )
            {
                if ( possibleAudioIDs[ i ].ToLowerInvariant() == idToSearch )
                {
                    audioIndex = i; break;
                }
            }
        }

        bool wasUndefinedBefore = ( audioIndex == 0 );

        audioIndex = Popup( "AudioItem", audioIndex, possibleAudioIDs_withCategory );
        if ( audioIndex > 0 )
        {
            subItem.ItemModeAudioID = possibleAudioIDs[ audioIndex ];
        }
        else
        {
            if ( !wasUndefinedBefore )
            {
                subItem.ItemModeAudioID = null;
            }
        }
    }

    private string[] _GetPossibleAudioIDs( bool withCategoryName, string firstEntryName )
    {
        var audioIDs = new List<string>();
        audioIDs.Add( firstEntryName );
        if ( AC.AudioCategories != null )
        {
            foreach ( var category in AC.AudioCategories )
            {
                _GetAllAudioIDs( audioIDs, category, withCategoryName );
            }
        }
        return audioIDs.ToArray();
    }

    private void _GetAllAudioIDs( List<string> audioIDs, AudioCategory c, bool withCategoryName )
    {
        if ( c.AudioItems != null )
        {
            foreach ( var audioItem in c.AudioItems )
            {
                if ( audioItem.Name.Length > 0 )
                {
                    if ( withCategoryName )
                    {
                        audioIDs.Add( string.Format( "{0}/{1}", c.Name, audioItem.Name ) );
                    }
                    else
                        audioIDs.Add( audioItem.Name );
                }
            }
        }
    }

    private bool SwapArrayElements<T>( T[ ] array, int index1, int index2 )
    {
        if ( array == null || index1 < 0 || index2 < 0 || index1 >= array.Length || index2 >= array.Length )
        {
            return false;
        }

        T tmp = array[ index1 ];
        array[ index1 ] = array[ index2 ];
        array[ index2 ] = tmp;
        return true;
    }

    private void VerticalSpace()
    {
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
    }

    private string[] GetCategoryNames()
    {
        if ( AC.AudioCategories == null )
        {
            return new string[ 0 ];
        }
        var names = new string[ AC.AudioCategories.Length ];
        for( int i=0; i< AC.AudioCategories.Length; i++ )
        {
            names[ i ] = AC.AudioCategories[ i ].Name;

            if ( names[ i ] == _nameForNewCategoryEntry )
            {
                names[ i ] = "---";
            }
        }
        return names;
    }

    private string[] GetItemNames()
    {
        AudioCategory curCat = currentCategory;
        if ( curCat == null || curCat.AudioItems == null )
        {
            return new string[0];
        }

        var names = new string[ curCat.AudioItems.Length ];
        for ( int i = 0; i < curCat.AudioItems.Length; i++ )
        {
            names[ i ] = curCat.AudioItems[ i ] != null ? curCat.AudioItems[ i ].Name : "";

            if ( names[ i ] == _nameForNewAudioItemEntry )
            {
                names[ i ] = "---";
            }
        }
        return names;
    }

    private string[] GetSubitemNames()
    {
        AudioItem curItem = currentItem;
        if ( curItem == null || curItem.subItems == null )
        {
            return new string[ 0 ];
        }

        var names = new string[ curItem.subItems.Length ];
        for ( int i = 0; i < curItem.subItems.Length; i++ )
        {
            AudioSubItemType subitemType = curItem.subItems[ i ] != null ? curItem.subItems[ i ].SubItemType : AudioSubItemType.Clip;

            if ( subitemType == AudioSubItemType.Item )
            {
                names[ i ] = string.Format( "ITEM {0}: {1}", i, ( curItem.subItems[ i ].ItemModeAudioID ?? "*undefined*" ) );
            }
            else
            {
                names[ i ] = string.Format( "CLIP {0}: {1}", i, ( curItem.subItems[ i ] != null ? curItem.subItems[ i ].Clip ? curItem.subItems[ i ].Clip.name : "*unset*" : "" ) );
            }
        }
        return names;
    }

    private string[ ] GetPlaylistNames()
    {
        if ( AC.musicPlaylist == null )
        {
            return new string[ 0 ];
        }

        var names = new string[ AC.musicPlaylist.Length ];
        for ( int i = 0; i < AC.musicPlaylist.Length; i++ )
        {
            names[ i ] = string.Format( "{0}: {1}", i, AC.musicPlaylist[ i ] );
        }
        return names;
    }

    static AudioClip[ ] GetSelectedAudioclips()
    { 
        var objList = Selection.GetFiltered( typeof( AudioClip ), SelectionMode.DeepAssets );
        var clipList = new AudioClip[objList.Length];

        for ( int i = 0; i < objList.Length; i++ )
        {
            clipList[ i ] = (AudioClip) objList[ i ];
        }
         
        return clipList;
    }

    AudioCategory _GetCategory( string name )
    {
        foreach ( AudioCategory cat in AC.AudioCategories )
        {
            if ( cat.Name == name )
            {
                return cat;
            }
        }
        return null;
    }
}
