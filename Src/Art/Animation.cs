using System;
namespace DukeNukem3D.Art
{
    public class Animation
    {
        public Animation(int stored)
        {
            this.Stored = stored;
        }

        public readonly int Stored;

        /// <summary>
        /// 6 bit signed integer<br/>
        /// bits 0-6
        /// </summary>
        public byte NumberOfFrames
        {
            get
            {
                byte b = 0;
                for (int i = 0; i < 6; i++)
                    b += (byte)((Stored >> (0 + i) & 0x01) << i);
                return b;
            }
        }
        /// <summary>
        /// 2 bit signed integer<br/>
        /// bits 6-7
        /// </summary>
        public AnimationType Type
        {
            get
            {
                byte b = 0;
                for (int i = 0; i < 2; i++)
                    b += (byte)((Stored >> (6 + i) & 0x01) << i);
                return (AnimationType)b;
            }
        }
        /// <summary>
        /// 8 bit signed integer<br/>
        /// bits 8-15
        /// </summary>
        public sbyte CenterOffsetX
        {
            get
            {
                byte b = 0;
                for (int i = 0; i < 8; i++)
                    b += (byte)((Stored >> (8 + i) & 0x01) << i);
                return unchecked((sbyte)b);
            }
        }
        /// <summary>
        /// 8 bit signed integer<br/>
        /// bits 16-23
        /// </summary>
        public sbyte CenterOffsetY
        {
            get
            {
                byte b = 0;
                for (int i = 0; i < 8; i++)
                    b += (byte)((Stored >> (16 + i) & 0x01) << i);
                return unchecked((sbyte)b);
            }
        }
        /// <summary>
        /// 4 bit unsigned integer<br/>
        /// bits 24-27
        /// </summary>
        public byte AnimationSpeed
        {
            get
            {
                byte b = 0;
                for (int i = 0; i < 4; i++)
                    b += (byte)((Stored >> (24 + i) & 0x01) << i);
                return b;
            }
        }
        /// <summary>
        /// 4 bit<br/>
        /// bits 28-31
        /// </summary>
        public byte Unused
        {
            get
            {
                byte b = 0;
                for (int i = 0; i < 4; i++)
                    b += (byte)((Stored >> (28 + i) & 0x01) << i);
                return b;
            }
        }

        public enum AnimationType
        {
            NoAnimation = 0,            //0b00
            OscillatingAnimation = 1,   //0b01
            AnimateForward = 2,         //0b10
            AnimateBackward = 3,        //0b11
        }
    }
}