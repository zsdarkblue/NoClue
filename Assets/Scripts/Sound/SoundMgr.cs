using System;
using UnityEngine;
using System.Collections.Generic;
using UObj = UnityEngine.Object;


namespace BeatsFever.Sound
{

    public class SoundMgr
    {
        private class LoopSoundEntity
        {
            public string name;
            private AudioSource source;

            public bool Load(GameObject audioGo, string name, float volume, AudioClip clip)
            {
                this.name = name;
                if (!clip.isReadyToPlay)
                {
                    Debug.LogWarning("clip is not ready to play: " + name);
                    return false;
                }

                source = audioGo.AddComponent<AudioSource>();
                source.volume = volume;
                source.loop = true;
                source.clip = clip;
                source.Play();

                return true;
            }

            public void Release()
            {
                if (source != null)
                {
                    source.Stop();
                    UnityEngine.Object.Destroy(source);
                    source = null;
                }
            }
        }


        private const float BkVolumn = 0.2f;
        private const string AudioPathPrefix = "Audio/";
        private GameObject audioGo;
        private bool soundEnable = true;
        private bool musicEnable = true;
        private string backgroundSound;

        private Dictionary<string, LoopSoundEntity> loopSounds = new Dictionary<string, LoopSoundEntity>();

        private Dictionary<string, Queue<AudioSource>> sounds = new Dictionary<string, Queue<AudioSource>>();

        /**the sound res dic */
        private Dictionary<string, AudioClip> clipDic = new Dictionary<string, AudioClip>();


        public bool SoundEnable
        {
            get { return soundEnable; }
            set
            {
                if (value == soundEnable)
                {
                    return;
                }

                soundEnable = value;

                if (!soundEnable)
                {
                    ClearLoopsAndStoped();
                    sounds.Clear();
                }
            }
        }

        public bool MusicEnable
        {
            get { return musicEnable; }
            set
            {
                if (value == musicEnable)
                {
                    return;
                }

                musicEnable = value;

                if (!musicEnable)
                {
                    StopLoop(backgroundSound);
                }
                else
                {
                    if (backgroundSound != null)
                    {
                        PlayInternal(backgroundSound, true, BkVolumn, false);
                    }
                }
            }
        }

        private void CreateAudioGo()
        {
            if (audioGo == null)
            {
                audioGo = new GameObject("SoundObj");
                UObj.DontDestroyOnLoad(audioGo);
            }
        }

        public void Start()
        {
            CreateAudioGo();
        }

        public void Shutdown()
        {
            Clear();

            UObj.Destroy(audioGo);
            audioGo = null;
        }

        public void Clear()
        {
            ClearLoopsAndStoped();

            AudioClip bkClip = null;
            if (backgroundSound != null && clipDic.ContainsKey(backgroundSound))
            {
                bkClip = clipDic[backgroundSound];
            }

            clipDic.Clear();
            if (bkClip != null)
            {
                clipDic[backgroundSound] = bkClip;
            }
        }

        public bool Register(string name)
        {
            if (clipDic.ContainsKey(name))
            {
                return true;
            }

            AudioClip clip = App.ResourceMgr.LoadRes(AudioPathPrefix + name) as AudioClip;
            if (null == clip)
            {
                Debug.LogWarning("sound is null: " + name);
                return false;
            }

            clipDic[name] = clip;
            return true;
        }

        /** unregister a named sound from the mgr
         *
         * @param[in] name the sound you want to unregister
         */
        public void UnRegister(AudioResources source)
        {
            string name = Enum.GetName(typeof(AudioResources), source);
            UnRegister(name);
        }

        public void UnRegister(String name)
        {
            if (clipDic.ContainsKey(name))
            {
                Resources.UnloadAsset(clipDic[name]);
                clipDic.Remove(name);
            }
        }

		public void UnloadSound(string name)
		{
			UnRegister(name);
		}

        public void Warm(string name)
        {
            if (!soundEnable)
            {
                return;
            }

            Register(name);
        }

        public void PlayBkMusic(MusicResources res)
        {
            if (!musicEnable)
            {
                return;
            }

            string name = Enum.GetName(typeof(MusicResources), res);
            PlayBkMusic(name);
        }

        public void PlayBkMusic(string name)
        {
            if (!musicEnable)
            {
                backgroundSound = name;
                return;
            }

            if (name == backgroundSound || name == null)
            {
                return;
            }

            if (backgroundSound != null)
            {
                StopBkMusic();
            }

            PlayInternal(name, true, BkVolumn, false);
            backgroundSound = name;
        }

        public void StopBkMusic()
        {
            if (backgroundSound != null)
            {
                StopLoop(backgroundSound);
                backgroundSound = null;
            }
        }

		public void Play(AudioResources source, bool loop = false,float volume = 1f,bool onceAtATime = false)
        {
            if (!soundEnable)
            {
                return;
            }

            string name = Enum.GetName(typeof(AudioResources), source);
            PlayInternal(name, loop, volume, onceAtATime);
        }

        public void Play(string name, bool loop = false, float volume = 1f, bool onceAtATime = false)
        {
            if (!soundEnable)
            {
                return;
            }

            PlayInternal(name, loop, volume, onceAtATime);
        }

        private void PlayInternal(string name, bool loop, float volume, bool onceAtATime)
        {
            if (!Register(name))
            {
                //Debug.LogWarning("not registered audio: " + name);
				Register(name);
                return;
            }

            AudioClip clip = clipDic[name];
            if (clip == null)
            {
                UnRegister(name);
                return;
            }

            if (loop)
            {
                if (!loopSounds.ContainsKey(name))
                {
                    LoopSoundEntity entity = new LoopSoundEntity();
                    if (entity.Load(audioGo, name, volume, clip))
                    {
                        loopSounds[name] = entity;
                    }
                }
            }
            else
            {
                Queue<AudioSource> queue;
                sounds.TryGetValue(name, out queue);
                if (queue == null)
                {
                    queue = new Queue<AudioSource>();
                    sounds[name] = queue;
                }

                AudioSource sound = null;
                while (queue.Count > 0)
                {
                    var peekSound = queue.Peek();
                    if (peekSound.isPlaying)
                    {
                        if (onceAtATime)
                        {
                            return;
                        }

                        // If the sounds play within 0.1f, don't need to play another.
                        if (peekSound.time < 0.1f)
                        {
                            return;
                        }

                        break;
                    }

                    if (sound != null)
                    {
                        // Destory the prior stopped sound.
                        UnityEngine.Object.Destroy(sound);
                    }

                    sound = peekSound;
                    queue.Dequeue();
                }

                if (sound == null)
                {
                    sound = audioGo.AddComponent<AudioSource>();
                    sound.clip = clip;
                }

                sound.volume = volume;
                sound.Play();
                queue.Enqueue(sound);
            }
        }

        public void Stop(string name)
        {
            if (name == null) return;
            Queue<AudioSource> queue;
            sounds.TryGetValue(name, out queue);
            if (queue == null)
            {
                return;
            }

            if (queue.Count > 0)
            {
                var audio = queue.Dequeue();
                audio.Stop();
            }
        }

        private bool HasSound(string name)
        {
            Queue<AudioSource> queue;
            sounds.TryGetValue(name, out queue);
            if (queue == null)
            {
                return false;
            }

            return queue.Count > 0;
        }


        /** stop the specified playing sound
         * param[in] name the sound name to stop
         */
        public void StopLoop(AudioResources source)
        {
            string name = Enum.GetName(typeof(AudioResources), source);
            StopLoop(name);
        }

        public void StopLoop(string name)
        {
            if (name != null && loopSounds.ContainsKey(name))
            {
                loopSounds[name].Release();
                loopSounds.Remove(name);
            }
        }

        /** stop all the playing sound
         */
        public void ClearLoopsAndStoped()
        {
            LoopSoundEntity bkSound = null;
            foreach (var sound in loopSounds.Values)
            {
                if (sound.name != backgroundSound)
                {
                    sound.Release();
                }
                else
                {
                    bkSound = sound;
                }
            }

            loopSounds.Clear();
            if (bkSound != null)
            {
                loopSounds[bkSound.name] = bkSound;
            }

            foreach (var queue in sounds.Values)
            {
                while (queue.Count > 0)
                {
                    var sound = queue.Peek();
                    if (sound.isPlaying)
                    {
                        break;
                    }

                    queue.Dequeue();
                    UnityEngine.Object.Destroy(sound);
                }
            }
        }
    }
}