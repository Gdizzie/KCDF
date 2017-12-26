// AudioObject - copyright by ClockStone 2011
// part of the ClockStone Unity Audio Framework, see AudioController.cs

using UnityEngine;
using System.Collections;

/// <summary>
/// The object playing the audio clip associated with a <see cref="AudioSubItem"/>
/// </summary>
/// <remarks>
/// If audio object pooling is enabled make sure you store references to an AudioObject
/// by using <see cref="PoolableReference{T}"/>
/// </remarks>
/// <example>
/// <code>
/// var soundFX = new PoolableReference( AudioController.Play( "someSFX" ) );
/// 
/// // some other part of the code executed later when the sound may have stopped playing 
/// // and was moved back to the pool
/// AudioObject audioObject = soundFX.Get();
/// if( audioObject != null )
/// {
///     // it is safe to access audioObject here
///     audioObject.Stop();
/// }
/// </code>
/// </example>
[RequireComponent( typeof( AudioSource ) )]
[AddComponentMenu("ClockStone/Audio/AudioObject")]
public class AudioObject : RegisteredComponent
{

	// **************************************************************************************************/
	//          public functions
	// **************************************************************************************************/

	/// <summary>
	/// Gets the audio ID.
	/// </summary>
	public string audioID
	{
		get;
		internal set;
	}

	/// <summary>
	/// Gets the category.
	/// </summary>
	public AudioCategory category
	{
		get;
		internal set;
	}

    /// <summary>
    /// Gets the corresponding AudioSubItem
    /// </summary>
    public AudioSubItem subItem
    {
        get;
        internal set;
    }

    /// <summary>
    /// Gets the corresponding AudioSubItem
    /// </summary>
    public AudioItem audioItem
    {
        get
        {
            if( subItem != null )
            {
                return subItem.item;
            }
            return null;
        }
    }

    /// <summary>
    /// The audio event delegate type.
    /// </summary>
    public delegate void AudioEventDelegate( AudioObject audioObject );

    /// <summary>
    /// Gets or sets the delegate to be called once an audio clip was completely played.
    /// </summary>
    public AudioEventDelegate completelyPlayedDelegate
    { 
        set
        {
            _completelyPlayedDelegate = value;
         }
        get
        { 
            return _completelyPlayedDelegate;
        }
    }

    private AudioEventDelegate _completelyPlayedDelegate;

	/// <summary>
	/// Gets or sets the volume.
	/// </summary>
    /// <remarks>
    /// This is the adjusted volume volume value with which the Audio clip is currently playing.
    /// It is the value resulting from multiplying the volume of the subitem, item, and the category. 
    /// It does not contain the global volume or the fading value.<para/>
    /// "Adjusted" means that the value does not equal Unity's internal audio clip volume value,
    /// because Unity's volume range is not distributed is a perceptually even manner.<para/>
    /// <c>unityVolume = Mathf.Pow( adjustedVolume, 1.6 )</c>
    /// </remarks>
	public float volume
	{
		get
		{
			return _volumeWithCategory;
		}
		set
		{
			// sets _volumeExcludingCategory so that _volumeWithCategory = volume = value 

			float volCat = _volumeFromCategory;

			if ( volCat > 0 )
			{
				_volumeExcludingCategory = value / volCat;
			}
			else
				_volumeExcludingCategory = value;

			_ApplyVolume();

		}
	}

    /// <summary>
    /// Gets the total volume.
    /// </summary>
    /// <remarks>
    /// This is the adjusted volume volume value with which the Audio clip is currently playing.
    /// It is the value resulting from multiplying the volume of the subitem, item, the category,  
    /// the global volume, and the fading value.<para/>
    /// "Adjusted" means that the value does not equal Unity's internal audio clip volume value,
    /// because Unity's volume range is not distributed is a perceptually even manner.<para/>
    /// <c>unityVolume = Mathf.Pow( adjustedVolume, 1.6 )</c>
    /// </remarks>
    public float volumeTotal
    {
        get
        {
            float vol = _volumeWithCategory * _volumeFromFade;
            
            if ( _audioController )
            {
                vol *= AudioController.GetGlobalVolume();
            }
            return vol;  
        }

    }

    /// <summary>
    /// Gets the Unity game time at which the audio started playing. 
    /// </summary>
    /// <remarks>
    /// Does not include a possible delay.
    /// </remarks>
    public float startedPlayingAtTime
    {
        get
        {
            return _playInitialTime;
        }
    }

    /// <summary>
    /// Gets the length of the clip. 
    /// </summary>
    /// <remarks>
    /// Is effected by <see cref="AudioSubItem.ClipStopTime"/> and <see cref="AudioSubItem.ClipStartTime"/>
    /// </remarks>
    public float clipLength
    {
        get
        {
            if ( _stopClipAtTime > 0 )
            {
                return _stopClipAtTime - _startClipAtTime;
            }
            else
            {
                if ( GetComponent<AudioSource>().clip != null )
                {
                    return GetComponent<AudioSource>().clip.length - _startClipAtTime;
                }
                else return 0;
            }
        }
    }

    /// <summary>
    /// Sets or gets the current audio time relative to <see cref="AudioSubItem.ClipStartTime"/> 
    /// </summary>
    public float audioTime
    {
        get
        {
            return GetComponent<AudioSource>().time - _startClipAtTime;
        }
        set
        {
            GetComponent<AudioSource>().time = value + _startClipAtTime;
        }
    }
    /// <summary>
    /// return <c>true</c> if the audio is currently fading out
    /// </summary>
    public bool isFadingOut
    {
        get
        {
            return _fadeOutTotalTime >= 0 && AudioController.systemTime > _fadeOutStartTime;
        }
    }

    /// <summary>
    /// return <c>true</c> if the audio is currently fading in
    /// </summary>
    public bool isFadingIn
    {
        get
        {
            return _fadeInTotalTime > 0;
        }
    }

	/// <summary>
	/// Fades-in a playing audio.
	/// </summary>
	/// <param name="fadeInTime">The fade time in seconds.</param>
	public void FadeIn( float fadeInTime )
	{
		_fadeInTotalTime = fadeInTime;
		_fadeInStartTime = AudioController.systemTime;
        _UpdateFadeVolume();
	}

	/// <summary>
	/// Plays the audio clip with the specified delay.
	/// </summary>
	/// <param name="delay">The delay.</param>
	public void Play( float delay )
	{
		_PlayInitial( delay );
	}

	/// <summary>
	/// Plays the audio clip.
	/// </summary>
	public void Play()
	{
		if ( _destroyIfNotPlaying )
		{
			GetComponent<AudioSource>().Play();
			_paused = false;
		}
		else
		{
			_PlayInitial( 0 );
		}
	}

	/// <summary>
	/// Stops playing this instance.
	/// </summary>
    /// <remarks>
    /// Uses fade out as specified in the corresponding <see cref="AudioSubItem.FadeOut"/>.
    /// </remarks>
	public void Stop()
	{
		Stop( subItem.FadeOut );
	}

	/// <summary>
	/// Stops a playing audio with fade-out.
	/// </summary>
    /// <param name="fadeOutLength">The fade time in seconds.</param>
    public void Stop( float fadeOutLength )
	{
        Stop( fadeOutLength, 0 );
	}

    /// <summary>
    /// Stops a playing audio with fade-out at a specified time.
    /// </summary>
    /// <param name="fadeOutLength">The fade time in seconds.</param>
    /// <param name="startToFadeTime">Fade out starts after <c>startToFadeTime</c> seconds have passed</param>
    public void Stop( float fadeOutLength, float startToFadeTime )
    {
        if ( fadeOutLength > 0 || startToFadeTime > 0 )
        {
            _fadeOutTotalTime = fadeOutLength;
            _fadeOutStartTime = AudioController.systemTime + startToFadeTime;
        }
        else
        {
            _Stop();
        }
    }

    void OnTriggerEnter( Collider collider )
    {
        AudioController.Play( "mySound", gameObject.transform );
    }

	/// <summary>
	/// Pauses the audio clip.
	/// </summary>
	public void Pause()
	{
		GetComponent<AudioSource>().Pause();
		_paused = true;
	}

	/// <summary>
	/// Determines whether the audio clip is paused.
	/// </summary>
	/// <returns>
	///   <c>true</c> if paused; otherwise, <c>false</c>.
	/// </returns>
	public bool IsPaused()
	{
		return _paused;
	}

	/// <summary>
	/// Determines whether the audio clip is playing.
	/// </summary>
	/// <returns>
	///   <c>true</c> if the audio clip is playing; otherwise, <c>false</c>.
	/// </returns>
	public bool IsPlaying()
	{
		return GetComponent<AudioSource>().isPlaying;
	}


	// **************************************************************************************************/
	//          private / protected functions and properties
	// **************************************************************************************************/

	internal float _volumeFromCategory
	{
		get
		{
			if ( category != null )
			{
				return category.VolumeTotal;
			}
			return 1.0f;
		}
	}

	internal float _volumeWithCategory
	{
		get
		{
			return _volumeFromCategory * _volumeExcludingCategory;
		}
	}

	internal float _volumeExcludingCategory = 1;  // untransformed volume
    private float _volumeFromFade = 1;
    internal float _volumeFromScriptCall = 1;

	bool _paused = false;
	bool _applicationPaused = false;

	float _fadeOutTotalTime = -1;
	double _fadeOutStartTime = -1;

	float _fadeInTotalTime = -1;
	double _fadeInStartTime = -1;

#pragma warning disable 414
	float _playInitialTime = -1;
#pragma warning restore 414

	bool _destroyIfNotPlaying = false;

    AudioController _audioController;
    internal float _stopClipAtTime = 0;
    internal float _startClipAtTime = 0;

    internal bool _isCurrentPlaylistTrack = false;

    internal float _audioSource_MinDistance_Saved = 1;
    internal float _audioSource_MaxDistance_Saved = 500;

#if !UNITY_FLASH
    internal GaplessAudioClipTransition _audioTransition;
    bool _setNextGaplessLoopingClipNextFrame = false;
#endif

    internal int _lastChosenSubItemIndex = -1;

	/// <summary>
	/// Initializes this instance.
	/// </summary>
	protected override void Awake()
	{
		base.Awake();

        _audioController = AudioController.Instance;
		//Debug.Log( "AudioObject.Awake" );

		GetComponent<AudioSource>().playOnAwake = false;
		GetComponent<AudioSource>().clip = null;
		category = null;
        _completelyPlayedDelegate = null;

        _lastChosenSubItemIndex = -1;
        _fadeOutTotalTime = -1;
		_fadeOutStartTime = -1;
		_fadeInTotalTime = -1;
		_fadeInStartTime = -1;
		_playInitialTime = -1;
        _volumeFromFade = 1;
        _volumeFromScriptCall = 1;
		_destroyIfNotPlaying = false;
		_volumeExcludingCategory = 1;
		_paused = false;
		_applicationPaused = false;
        _isCurrentPlaylistTrack = false;
        _stopClipAtTime = 0;
        _startClipAtTime = 0;

#if !UNITY_FLASH
        _setNextGaplessLoopingClipNextFrame = false;
#endif
	}

	private void _PlayInitial( float delay )
	{
		//Debug.Log( "_PlayInitial:" + audioID + " sub:" + _subItemID );

        if ( !GetComponent<AudioSource>().clip )
        {
            Debug.LogError( "audio.clip == null in " + gameObject.name );
            return;
        }

		ulong d = (ulong) ( ( 44100.0f / GetComponent<AudioSource>().clip.frequency ) * delay * GetComponent<AudioSource>().clip.frequency ); // http://unity3d.com/support/documentation/ScriptReference/AudioSource.Play.html

		//Debug.Log( "Play Clip:" + audio.clip.name + " S/N:"+ GetComponent<PoolableObject>().GetSerialNumber());

		GetComponent<AudioSource>().Play( d );

		_playInitialTime = Time.time;

		_destroyIfNotPlaying = true;
		_paused = false;
	}

	private void _Stop()
	{
        _fadeOutTotalTime = -1;
        _fadeOutStartTime = -1;
		//Debug.Log( "Stop " );
		GetComponent<AudioSource>().Stop();
	}

	private void Update()
	{
		if ( !_destroyIfNotPlaying ) return;

		if ( !_paused && !_applicationPaused )
		{
            if ( !GetComponent<AudioSource>().isPlaying )
            {
                bool destroy;
                if ( completelyPlayedDelegate != null )
                {
                    var audItem = audioItem;

                    bool isComplete = true;

                    if ( audItem != null )
                    {
                        switch ( audItem.Loop )
                        {
                        case AudioItem.LoopMode.LoopSequence:
                            _PlayNextInLoopSequence();
                            isComplete = false;
                            break;
                        case AudioItem.LoopMode.LoopSequenceGapless:
                            isComplete = false;
                            break;
                        }
                    }

                    if ( isComplete )
                    {
                        completelyPlayedDelegate( this );
                    }

                    destroy = !GetComponent<AudioSource>().isPlaying;
                }
                else
                    destroy = true;

                if ( _isCurrentPlaylistTrack )
                {
                    if( _audioController ) _audioController._NotifyPlaylistTrackCompleteleyPlayed( this );
                }

                if ( destroy )
                {
                    DestroyAudioObject();
                    return;
                }
            }
            else
            {
                if( !GetComponent<AudioSource>().loop )
                {
                    if ( _isCurrentPlaylistTrack &&  
                         _audioController && _audioController.crossfadePlaylist &&
                         audioTime > clipLength - _audioController.musicCrossFadeTime_Out )
                    {
                        _audioController._NotifyPlaylistTrackCompleteleyPlayed( this );
                    }
                    else
                    {
                        if ( !isFadingOut && audioTime >= clipLength - subItem.FadeOut )
                        {
                            Stop( subItem.FadeOut ); return;
                        }
                    }
                }
            }
		}

        _UpdateFadeVolume();

#if !UNITY_FLASH
        if ( _setNextGaplessLoopingClipNextFrame )
        {
            _setNextGaplessLoopingClipNextFrame = false;
            _SetNextClipForGaplessLoop();
        }
#endif

	}

    private void _PlayNextInLoopSequence()
    {
        _audioController.PlayAudioItem( audioItem, _volumeFromScriptCall, Vector3.zero, null, 0, 0, false, this );
        //Debug.Log( "_PlayNextInLoopSequence: " + _lastChosenSubItemIndex );
    }

    private void _UpdateFadeVolume()
    {
        float fadeVolume = 1;

        if ( isFadingOut )
        {
            fadeVolume *= 1.0f - _GetFadeValue( (float)( AudioController.systemTime - _fadeOutStartTime ), _fadeOutTotalTime );

            if ( fadeVolume == 0 )
            {
                _Stop(); return;
            }
        }

        if ( isFadingIn )
        {
            fadeVolume *= _GetFadeValue( (float)( AudioController.systemTime - _fadeInStartTime ), _fadeInTotalTime );
        }

        if ( fadeVolume != _volumeFromFade )
        {
            _volumeFromFade = fadeVolume;
            _ApplyVolume();
        }
        
    }

	private float _GetFadeValue( float t, float dt )
	{
        if ( dt <= 0 )
        {
            return t > 0 ? 1.0f : 0;
        }
		return Mathf.Clamp( t / dt, 0.0f, 1.0f);
	}

	private void OnApplicationPause( bool b )
	{
		_applicationPaused = b;
	}
	
	/// <summary>
    /// Destroys the audio object (using <see cref="ObjectPoolController"/> if pooling is enabled)
	/// </summary>
	public void DestroyAudioObject()
	{
        if ( GetComponent<AudioSource>().isPlaying )
        {
            _Stop();
        }

		//Debug.Log( "Destroy:" + audio.clip.name );
#if AUDIO_TOOLKIT_DEMO
		GameObject.Destroy( gameObject );
#else
		ObjectPoolController.Destroy( gameObject );
#endif
		_destroyIfNotPlaying = false;
	}

    /// <summary>
    /// Transforms the volume to make it perceptually more intuitive to scale and cross-fade.
    /// </summary>
    /// <param name="volume">The volume to transform.</param>
    /// <returns>
    /// The transformed volume <c> = Pow( volume, 1.6 ) </c>
    /// </returns>
	static public float TransformVolume( float volume )
	{
		return Mathf.Pow( volume, 1.6f ); 
	}

    /// <summary>
    /// Transforms the pitch from semitones to a multiplicative factor
    /// </summary>
    /// <param name="pitchSemiTones">The pitch shift in semitones to transform.</param>
    /// <returns>
    /// The transformed pitch <c> = Pow( 2, pitch / 12 ) </c>
    /// </returns>
    static public float TransformPitch( float pitchSemiTones )
	{
        return Mathf.Pow( 2, pitchSemiTones / 12.0f );
	}


    /// <summary>
    /// Inverse pitch transformation: <see cref="TransformPitch"/>
    /// </summary>
    /// <param name="pitch">The transformed pitch</param>
    /// <returns>The pitch shift in semitones</returns>
    static public float InverseTransformPitch( float pitch )
    {
        return ( Mathf.Log( pitch ) / Mathf.Log( 2.0f ) ) * 12.0f;
    }

	internal void _ApplyVolume()
	{
        float volumeToSet = TransformVolume( volumeTotal );
		//Debug.Log( "volume set:" + volumeToSet );
		GetComponent<AudioSource>().volume = volumeToSet;
	}

    /// <summary>
    /// If pooling is enabled <c>OnDestroy</c> is called every  time the object is deactivated and moved to the pool.
    /// </summary>
	protected override void OnDestroy()
	{
        base.OnDestroy();
		//Debug.Log( "Destroy:" + audio.clip.name );

        var item = audioItem;

        if ( item != null )
        {
            if ( item.overrideAudioSourceSettings )  // restore audio source settings
            {
                _RestoreAudioSourceSettings();
            }
        }
	}

    private void _RestoreAudioSourceSettings()
    {
        GetComponent<AudioSource>().minDistance = _audioSource_MinDistance_Saved;
        GetComponent<AudioSource>().maxDistance = _audioSource_MaxDistance_Saved;
    }

#if !UNITY_FLASH

    internal void _SetNextClipForGaplessLoop()
    {
        var chosenSubItem = AudioController._ChooseSingleSubItem( subItem.item, subItem.item.SubItemPickMode, this );
        _audioTransition.nextClip = chosenSubItem.Clip;
        name = "AudioObject:" + _audioTransition.currentClip.name;
    }

    private void _OnGaplessAudioTransition()
    {
        _setNextGaplessLoopingClipNextFrame = true;
    }

    internal void _CreateGaplessAudioClipTransition( AudioSubItem subItem )
    {
        if ( _audioTransition == null )
        {
            _audioTransition = new GaplessAudioClipTransition( subItem.Clip, null ); // nextClip will be set later, this way it can not be null
            _audioTransition.onTransitionCallback = _OnGaplessAudioTransition;
        }
        else
            _audioTransition.currentClip = subItem.Clip;
        _SetNextClipForGaplessLoop();
        GetComponent<AudioSource>().clip = _audioTransition.CreateOutputClip( subItem.Clip.channels, subItem.Clip.frequency, true );
    }
#endif

    /// <summary>
    /// Checks if this <see cref="AudioObject"/> belongs to a specific category 
    /// </summary>
    /// <param name="categoryName">The name of the category</param>
    /// <returns><c>true</c> if the category with the specified name or one of its child categories 
    /// contains the <see cref="AudioItem"/> the <see cref="AudioObject"/> belongs to.</returns>
    public bool DoesBelongToCategory( string categoryName )
    {
        AudioCategory curCategory = category;

        while( curCategory != null )
        {
            if ( curCategory.Name == categoryName )
            {
                return true;
            }
            curCategory = curCategory.parentCategory;
        }
        return false;
    }
}


