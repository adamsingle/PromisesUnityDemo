using Newtonsoft.Json;
using RSG;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Code
{
    public class TutorialStage
    {
        /// <summary>
        /// X Position to move the camera to
        /// </summary>
        public float CameraPositionX { get; set; }

        /// <summary>
        /// Y Position to move the camera to
        /// </summary>
        public float CameraPositionY { get; set; }

        /// <summary>
        /// Z Position to move the camera to
        /// </summary>
        public float CameraPositionZ { get; set; }

        /// <summary>
        /// X euler angle to rotate to
        /// </summary>
        public float CameraOrientationX { get; set; }

        /// <summary>
        /// Y euler angle to rotate to
        /// </summary>
        public float CameraOrientationY { get; set; }

        /// <summary>
        /// Z euler angle to rotate to
        /// </summary>
        public float CameraOrientationZ { get; set; }

        /// <summary>
        /// An array of text box messages to show the player for this stage. The player
        /// will be prompted to press space bar after each one to see the next one, 
        /// except the final string which will stay till overwritten by another stage.
        /// </summary>
        public string[] TextBoxes { get; set; }

        /// <summary>
        /// The key that this tutorial stage is prompting the user to press.
        /// </summary>
        public KeyCode Key { get; set; }
    }
}
