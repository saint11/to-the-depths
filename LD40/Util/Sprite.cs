using System;
using System.Collections.Generic;
using Otter;
using System.Xml;
using System.Text.RegularExpressions;
using System.IO;

namespace LD40
{
    /// <summary>
    /// Graphic that is used for an animated sprite sheet.
    /// </summary>
    /// <typeparam name="TAnimType"></typeparam>
    public class Sprite : Graphic
    {

        public float Speed = 1f;

        public Dictionary<string, Animation> Animations;
        public Animation CurrentAnimation { get { return Animations[CurrentAnimationName]; } }

        private string CurrentAnimationName;
        public bool Flip;

        #region Public Properties

        /// <summary>
        /// The total number of frames on the sprite sheet.
        /// </summary>
        public int Frames { get; private set; }

        /// <summary>
        /// The total number of columns on the sprite sheet.
        /// </summary>
        public int Columns { get; private set; }

        /// <summary>
        /// The total number of rows on the spirte sheet.
        /// </summary>
        public int Rows { get; private set; }

        /// <summary>
        /// The current buffered animation.
        /// </summary>
        public Animation BufferedAnimation { get; private set; }

        /// <summary>
        /// Determines if the sprite is playing animations.
        /// </summary>
        public bool Active { get; private set; }

        /// <summary>
        /// Determines if the sprite is advancing its current animation.
        /// </summary>
        public bool Paused { get; private set; }

        /// <summary>
        /// The current frame of the animation on the sprite sheet.
        /// </summary>
        public int CurrentFrame
        {
            get
            {
                return CurrentAnimation.CurrentFrameIndex;
            }
            set
            {
                SetFrame(value);
            }
        }

        public bool IsPlaying { get { return CurrentAnimation.Active; } }


        #endregion

        internal static Sprite Load(XmlElement xml)
        {
            Sprite sprite = new Sprite();
            sprite.Animations = new Dictionary<string, Animation>();
            sprite.Name = xml.GetAttribute("name");

            sprite.OriginX = xml["Origin"].AttributeFloat("x");
            sprite.OriginY = xml["Origin"].AttributeFloat("y");

            foreach (XmlNode n in xml["Animations"])
            {
                if (n is XmlComment) continue;
                XmlElement animXml = n as XmlElement;

                Animation anim = new Animation();
                sprite.CurrentAnimationName = anim.Name = animXml.GetAttribute("name");
                var frames = ParseFrames(animXml);
                foreach (var frame in frames)
                {
                    string path = Global.imagePath + animXml.GetAttribute("textures");
                    string imageName = path + ".png";

                    if (!(frame == "0" && File.Exists(imageName)))
                    {
                        imageName = path + frame + ".png";
                        for (int i = 1; i < 6; i++)
                        {
                            if (File.Exists(imageName)) break;
                            else imageName = path + GenerateZeros(i) + frame + ".png";
                        }
                    }
                    if (!File.Exists(imageName)) throw new Exception("Cannot find image " + imageName + " for the animation " + animXml.GetAttribute("name") + " on " + sprite.Name);
                    anim.Frames.Add(new Image(imageName));
                    anim.FrameDelays.Add(animXml.AttributeFloat("delay") / 100);
                    anim.RepeatCount = animXml.AttributeInt("repeat", -1);
                }

                sprite.Animations.Add(anim.Name, anim);
            }
            return sprite;
        }

        internal static Sprite CopyOf(Sprite source)
        {
            Sprite sprite = new Sprite();
            sprite.Animations = new Dictionary<string, Animation>();
            sprite.Name = source.Name;

            sprite.OriginX = source.OriginX;
            sprite.OriginY = source.OriginY;

            foreach (var pair in source.Animations)
            {
                Animation animSource = pair.Value;
                Animation anim = new Animation();
                sprite.CurrentAnimationName = anim.Name = animSource.Name;

                for (int i = 0; i < animSource.Frames.Count; i++)
                {
                    /*Image image = new Image(animSource.Frames[i].Texture);
                    image.AtlasRegion = animSource.Frames[i].AtlasRegion;
                    image.ClippingRegion = animSource.Frames[i].ClippingRegion;
                    anim.Frames.Add(image);
                    anim.FrameDelays.Add(animSource.FrameDelays[i]);*/
                    anim.Frames.Add(animSource.Frames[i]);
                    anim.FrameDelays.Add(animSource.FrameDelays[i]);
                }

                anim.RepeatCount = animSource.RepeatCount;
                sprite.Animations.Add(anim.Name, anim);
            }
            return sprite;
        }

        private static string GenerateZeros(int i)
        {
            string zeros = "";
            while (i-- > 0)
            {
                zeros += "0";
            }
            return zeros;
        }

        private static string[] ParseFrames(XmlElement animXml)
        {
            string frames = "";
            string rawFrames = animXml.AttributeString("frames");
            if (rawFrames.Contains("-"))
            {
                foreach (string element in rawFrames.Split(','))
                {
                    if (element.Contains("-"))
                    {
                        string min = element.Substring(0, element.IndexOf('-'));
                        string max = element.Substring(element.IndexOf('-') + 1);
                        for (int i = int.Parse(min); i <= int.Parse(max); i++)
                        {
                            frames += i.ToString() + ",";
                        }
                    }
                    else frames += element + ",";
                }
            }
            else frames = rawFrames;
            if (frames[frames.Length - 1] == ',') frames = frames.Substring(0, frames.Length - 1);
            return frames.Split(',');
        }

        public override void Update()
        {
            base.Update();

            if (!Active) return;
            CurrentAnimation.Update();

            if (Paused) return;

            CurrentAnimation.Update(Speed);
        }

        public override void Render(float x = 0, float y = 0)
        {
            if (!Visible) return;

            var img = CurrentAnimation.CurrentFrame;
            img.OriginX = OriginX;
            img.OriginY = OriginY;
            img.ScaleX = ScaleX * (Flip ? -1 : 1);
            img.ScaleY = ScaleY;
            img.Angle = Angle;

            float renderX = X + x;
            float renderY = Y + y;

            // Rounding here to fix 1 pixel offset problems (textures wrap around?)
            if (roundRendering)
            {
                renderX = Util.Round(renderX);
                renderY = Util.Round(renderY);
            }
            if (ScrollX != 1)
            {
                renderX = X + Draw.Target.CameraX * (1 - ScrollX) + x;
            }
            if (ScrollY != 1)
            {
                renderY = Y + Draw.Target.CameraY * (1 - ScrollY) + y;
            }

            img.Render(renderX, renderY);
        }

        #region Public Methods

        public void Play(string a)
        {
            if (!Animations.ContainsKey(a))
            {
                Debugger.Instance.Log("Cannot play animation " + a + " on " + Name);
                return;
            }
            Active = true;
            var pastAnim = CurrentAnimation.Name;

            CurrentAnimationName = a;
            if (CurrentAnimation != Animations[pastAnim])
            {
                CurrentAnimation.Reset();
            }
        }

        /// <summary>
        /// Plays an animation.  If no animation is specified, play the buffered animation.
        /// </summary>
        public void Play()
        {
            Active = true;

            if (BufferedAnimation != null)
            {
                Play(BufferedAnimation.Name);
            }
            else
            {
                CurrentAnimation.Reset();
            }
        }


        /// <summary>
        /// Buffers an animation but does not play it.  Call Play() with no arguments to play the buffered animation.
        /// </summary>
        /// <param name="a">The animation to buffer.</param>
        public void Buffer(Animation a)
        {
            BufferedAnimation = a;
        }

        /// <summary>
        /// Pause the playback of the animation.
        /// </summary>
        public void Pause()
        {
            Paused = true;
        }

        /// <summary>
        /// Resume the animation from the current position.
        /// </summary>
        public void Resume()
        {
            Paused = false;
        }

        /// <summary>
        /// Stop playback.  This will reset the animation to the first frame.
        /// </summary>
        public void Stop()
        {
            Active = false;
            CurrentAnimation.Stop();
        }

        /// <summary>
        /// Set the current animation to a specific frame.
        /// </summary>
        /// <param name="frame">The frame in terms of the animation.</param>
        public void SetFrame(int frame)
        {
            if (!Active) return;

            CurrentAnimation.CurrentFrameIndex = frame;
            CurrentAnimation.Reset();
        }

        /// <summary>
        /// Set the current animation to a specific frame and pause.
        /// </summary>
        /// <param name="frame">The frame in terms of the animation.</param>
        public void FreezeFrame(int frame)
        {
            if (!Active) return;

            Paused = true;
            CurrentAnimation.CurrentFrameIndex = frame;
        }

        /// <summary>
        /// Set the sprite to a frame on the sprite sheet itself.
        /// This will disable the current animation!
        /// </summary>
        /// <param name="frame">The global frame in terms of the sprite sheet.</param>
        public void SetGlobalFrame(int frame)
        {
            Active = false;
        }

        /// <summary>
        /// Resets the current animation back to the first frame.
        /// </summary>
        public void Reset()
        {
            CurrentAnimation.Reset();
        }
        #endregion
    }

    public class Animation
    {
        public string Name;

        #region Private Fields

        bool pingPong;
        int loopBack = 0;
        float timer;
        float delay;
        int direction;
        int repeatsCounted = 0;

        int currentFrame;

        #endregion

        #region Public Fields

        public Action OnComplete;
        public bool Active = true;

        #endregion

        #region Public Properties

        /// <summary>
        /// The overall playback speed of the animation.
        /// </summary>
        public float PlaybackSpeed { get; private set; }

        /// <summary>
        /// The repeat count of the animation.
        /// </summary>
        public int RepeatCount { get; set; }

        /// <summary>
        /// The frames used in the animation.
        /// </summary>
        public List<Image> Frames { get; private set; }

        /// <summary>
        /// The frame delays used in the animation.
        /// </summary>
        public List<float> FrameDelays { get; private set; }

        /// <summary>
        /// The total number of frames in this animation.
        /// </summary>
        public int FrameCount
        {
            get { return Frames.Count; }
        }

        /// <summary>
        /// The current frame of the animation.
        /// </summary>
        public Image CurrentFrame
        {
            get { return Frames[currentFrame]; }
        }

        public int CurrentFrameIndex
        {
            get { return currentFrame; }
            set { currentFrame = value; }
        }

        /// <summary>
        /// The total duration of the animation.
        /// </summary>
        public float TotalDuration
        {
            get
            {
                float delay = 0;
                foreach (float d in FrameDelays)
                {
                    delay += d;
                }
                return delay;
            }
        }

        #endregion

        /// <summary>
        /// Creates a new Anim with an array of ints for frames, and an array of floats for frameDelays.
        /// </summary>
        /// <param name="frames">The frames from the sprite sheet to display.</param>
        /// <param name="frameDelays">The time that each frame should be displayed.</param>
        public Animation()
        {
            RepeatCount = -1;
            timer = 0;
            delay = 0;
            direction = 1;
            PlaybackSpeed = 1;

            Frames = new List<Image>();
            FrameDelays = new List<float>();

            Active = true;
        }


        public void Update(float t = 1f)
        {
            if (Active)
            {
                timer += PlaybackSpeed * Game.Instance.DeltaTime * t;
            }

            delay = FrameDelays[currentFrame];

            while (timer >= delay)
            {
                timer -= delay;
                currentFrame += direction;

                if (currentFrame == Frames.Count)
                {
                    if (repeatsCounted < RepeatCount || RepeatCount < 0)
                    {
                        repeatsCounted++;
                        if (pingPong)
                        {
                            direction *= -1;
                            currentFrame = Frames.Count - 2;
                        }
                        else
                        {
                            currentFrame = loopBack;
                        }
                    }
                    else
                    {
                        if (pingPong)
                        {
                            direction *= -1;
                            currentFrame = Frames.Count - 2;
                        }
                        else
                        {
                            if (OnComplete != null)
                            {
                                OnComplete();
                            }
                            Stop();
                            currentFrame = Frames.Count - 1;
                        }
                    }
                }

                if (currentFrame < loopBack)
                {
                    if (pingPong)
                    {
                        if (repeatsCounted < RepeatCount || RepeatCount < 0)
                        {
                            repeatsCounted++;
                            direction *= -1;
                            currentFrame = loopBack + 1;
                        }
                        else
                        {
                            if (OnComplete != null)
                            {
                                OnComplete();
                            }
                            Stop();
                        }
                    }
                }
            }
        }

        public Animation Stop()
        {
            Active = false;
            currentFrame = 0;
            return this;
        }
        public Animation Reset()
        {
            Active = true;
            timer = 0;
            currentFrame = 0;
            return this;
        }
    }
}