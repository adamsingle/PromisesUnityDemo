using Newtonsoft.Json;
using RSG;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Code
{
    public class TutorialCamera : MonoBehaviour
    {
        /// <summary>
        /// In meters per second.
        /// </summary>
        public float cameraSpeed;

        /// <summary>
        /// Used to tell if the camera is close enough to the position it's moving to to snap to it and resolve the movement promise
        /// </summary>
        public float closeEnoughThreshold = 0.1f;

        /// <summary>
        /// When the camera is moving we want to hide the whole text panel
        /// </summary>
        public GameObject UIPanel;

        /// <summary>
        /// This is what we set from the data
        /// </summary>
        public Text UIText;

        /// <summary>
        /// Path to the data we will load that describes our tutorial stages
        /// </summary>
        private string filepath;

        private PromiseTimer promiseTimer = new PromiseTimer();

        public GameObject Player;

        void Start()
        {
            filepath = Application.dataPath + "/Data/tutorial.json";
            var jsonString = File.ReadAllText(filepath);
            var stages = JsonConvert.DeserializeObject<TutorialStage[]>(jsonString);
            RunTutorial(stages);
        }

        void Update()
        {
            //Update the promise timer so our promises get evaluated over time
            promiseTimer.Update(Time.deltaTime);
        }

        /// <summary>
        /// Move the camera to a specific position and return a promise that resolves when it's completed the move
        /// </summary>
        public IPromise Move(Vector3 endPos, Vector3 endOrientation)
        {
            endPos = Player.transform.position + endPos;
            UIPanel.SetActive(false);
            var startPos = transform.position;
            var distance = Vector3.Distance(endPos,startPos);
            var distanceStep = 1f / distance;
            
            var startOrientation = transform.localEulerAngles;
            var distanceTime = 0f;

            // Notice we don't call Done on this promise. Since we are returning it, whatever
            // calls this function can handle calling Done if appropriate, since if it's a part of 
            // a chain we don't want to do that.
            return promiseTimer.WaitUntil(timeData =>
            {
                distanceTime += timeData.deltaTime * distanceStep * cameraSpeed;
                transform.position = Vector3.Lerp(startPos, endPos, distanceTime);

                transform.localEulerAngles = Vector3.Lerp(startOrientation, endOrientation, distanceTime);

                return IsPositionCloseEnough(endPos);
            });
        }

        /// <summary>
        /// Returns true when the camera is considered "close enough" to the position passed in
        /// </summary>
        private bool IsPositionCloseEnough(Vector3 endPos)
        {
            return Vector3.Distance(transform.position, endPos) <= closeEnoughThreshold;
        }

        /// <summary>
        /// Wait for the user to press the specified key before resolving the returned promise
        /// </summary>
        private IPromise WaitForKeyInput(KeyCode key)
        {
            // As a convention, since we don't need to use the TimeData struct that is passed into
            // the WaitUntil, we use an underscore. This is purely convention and for speed of typing.
            // Why write it if you don't need it right?
            //We also need to have a small delay here so that the promises don't get called all in the same 
            //frame where the first space bar is pressed.
            return promiseTimer.WaitFor(0.2f)
            .Then(() => 
                promiseTimer.WaitUntil(_ =>
                {
                    return Input.GetKeyUp(key);
                })
            );
        }

        /// <summary>
        /// Returns a promise that will display our text in our UI text box and optionally wait for the 
        /// user to press space bar before resolving.
        /// </summary>
        private Func<IPromise> PrepTextBoxForStage(string text, bool waitForInput)
        {
            return () =>
            {
                if (waitForInput)
                {
                    text += "\n\nPress Space to continue";
                }

                UIText.text = text;
                if (!UIPanel.active)
                {
                    UIPanel.SetActive(true);
                }

                if (waitForInput)
                {
                    return WaitForKeyInput(KeyCode.Space);
                }

                //If we don't want to wait for input, then we can return an already resolved promise and the chain using this function
                //can continue straight away
                return Promise.Resolved();
            };
        }

        /// <summary>
        /// Here we prep the entire stage of the tutorial. We move the camera, then show the text boxes, waiting for input when
        /// necessary, then wait for the specific key
        /// </summary>
        private Func<IPromise> PrepTutorialStage(Vector3 cameraPosition, Vector3 cameraOrientation, string[] texts, KeyCode key)
        {
            var textBoxPromises = texts.Select((text, index) => PrepTextBoxForStage(text, index != texts.Length - 1));

            return () =>
            {
                return Move(cameraPosition, cameraOrientation)
                    .ThenSequence(() => textBoxPromises)
                    .Then(() => WaitForKeyInput(key));
            };
        }

        /// <summary>
        /// The final link, or perhaps more accurately, the first link that fires all the rest off. This once tiny
        /// function will run through our entire tutorial, waiting for input when needed.
        /// </summary>
        /// <returns></returns>
        private IPromise RunTutorial(IEnumerable<TutorialStage> tutorialData)
        {
            var tutorialStages = tutorialData.Select(data => PrepTutorialStage( BuildPositionFromData(data), BuildOrientationFromData(data), data.TextBoxes, data.Key));
            return Promise.Sequence(tutorialStages);
        }

        /// <summary>
        /// Little helper to build the Vector3 from the data
        /// </summary>
        private Vector3 BuildPositionFromData(TutorialStage data)
        {
            return new Vector3(data.CameraPositionX, data.CameraPositionY, data.CameraPositionZ);
        }

        private Vector3 BuildOrientationFromData(TutorialStage data)
        {
            return new Vector3(data.CameraOrientationX, data.CameraOrientationY, data.CameraOrientationZ);
        }
    }
}
