/* PlayButton.cs
 * Copyright Eddie Cameron 2012 (See readme for licence)
 * ----------------------------
 */

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Chronos;

namespace cakeslice
{
    public class PlayButton : MonoBehaviour
    {
        //public InputVCR playbackCharacterPrefab;
        public InputVCR playerVCR;
        public Material ghostMaterial;

        [Header("Things To Add To New Clone")]
        public GameObject displayNameObject;
        public GameObject cloneDigitalCodeObject;

        //public RecordButton recordButton;

        //public Texture pauseTex;

        //private bool isRecording;
        private Vector3 recordingStartPos;
        private Quaternion recordingStartRot;
        //private Quaternion defaultStart;

        private bool isPlaying;
        private GameObject curPlayer;

        //private float camAngleX = 0;
        //private float camAngleZ = 0;

        //private Quaternion playerYRot;

        private int recPlayerNum = 0;

        //private GameObject playerObject;

        string saveRecordingPath = System.Environment.CurrentDirectory + @"\PlayerRecordings\" + "Recording";

        [HideInInspector] public int playFrom = 0;
        [HideInInspector] public bool rewindStart = false;
        [HideInInspector] public bool rewindEnd = false;

        private float recFPS;
        private bool triggeredEarly = false;

        private int rewindCountDown = 5;


        List<Recording> playerRecordings = new List<Recording>();

        [HideInInspector] public GameObject rewindCountDownObj;
        private bool countDownEarly = false;

        [HideInInspector] public float tempMaxFrames;

        [Header("Current Buffer & Active Ghosts")]
        public List<GameObject> allRecordedClones = new List<GameObject>();
        public List<GameObject> allRecordedClonesOfClones = new List<GameObject>();

        [HideInInspector]
        public Camera playerCamera;

        [Header("Time Rewind Ability Settings")]
        public int maxActiveClones = 3;
        public float maxRecordTime = 10;
        public float minRewind = 2;

        [Header("Ghost Sound & Effects Prefabs")]
        public AudioClip ghostSpawn;
        public GameObject tempAudio;

        private bool canDeactivate = false;
        private bool canActivate = true;
        private bool firstPress = true;
        private Coroutine minDelay;
        public bool haveAbility;
        private bool abilityFirstActivate = true;


        void Awake()
        {
            allRecordedClones.Clear();
            allRecordedClonesOfClones.Clear();
            rewindCountDownObj = GameObject.Find("CountDown");

            recFPS = playerVCR.recordingFrameRate;
            playerRecordings.Clear();
        }

        public void StartRecording()
        {
            playerVCR.NewRecording();
        }

        void Update()
        {
            if (haveAbility)
            {
                if (abilityFirstActivate)
                {
                    StartRecording();
                    abilityFirstActivate = false;
                }

                if (playerVCR.GetRecording() != null)
                {
                    if ((playerVCR.GetRecording().frames.Count / recFPS) < minRewind)
                        canActivate = false;
                    else
                        canActivate = true;
                }
                else
                {
                    Debug.Log("There is no player recording to access!");
                }

                if (allRecordedClones.Count < maxActiveClones && canActivate)
                {
                    if (hardInput.GetKeyDown("RecordPlayer"))
                    {
                        canDeactivate = false;

                        if (firstPress)
                        {
                            Clock globalClock = Timekeeper.instance.Clock("Root");
                            firstPress = false;
                            rewindStart = true;
                            countDownEarly = false;
                            playerVCR.Stop();
                            globalClock.localTimeScale = -1;
                            playerVCR.gameObject.GetComponent<CameraBehaviour>().isActive = false;

                            Debug.Log("Testing - There are " + playerVCR.GetRecording().recordingLength + " recorded! - Activated Start");
                            // Starts the rewinding of the frames
                            InvokeRepeating("RewindToFrame", 0.0f, 1.0f / recFPS);

                            playerVCR.gameObject.GetComponent<PlayerBehaviour>().stopKeyInput = true;
                            playerCamera.gameObject.GetComponent<ChangeCameraEffects>().ChangeFOV(100, 2);
                            playerCamera.gameObject.GetComponent<ChangeCameraEffects>().ChangeGrain(1, 2);
                            AudioManager.instance.Play("Ability_TimeRewind");

                            if (minDelay != null)
                                StopCoroutine(minDelay);

                            minDelay = StartCoroutine(MinRewindDelay(minRewind));
                        }
                    }
                    if (hardInput.GetKeyUp("RecordPlayer") && canDeactivate && haveAbility)
                    {
                        canDeactivate = false;

                        if (!triggeredEarly)
                        {
                            RewindStuff();
                        }
                        else
                        {
                            triggeredEarly = false;
                        }
                    }
                    if (!hardInput.GetKey("RecordPlayer") && canDeactivate && haveAbility)
                    {
                        canDeactivate = false;

                        if (!triggeredEarly)
                        {
                            RewindStuff();
                        }
                        else
                        {
                            triggeredEarly = false;
                        }
                    }
                }
                else
                {
                    if (hardInput.GetKeyDown("RecordPlayer"))
                    {
                        AudioManager.instance.Play("Ability_TimeRewindDisabled");

                        // Start new recording, to make sure it doesn't get stuck.
                        Debug.Log("There are " + playerVCR.GetRecording().frames.Count + " frames in current recording!");
                        if (playerVCR.GetRecording().recordingLength < 5)
                        {
                            StartRecording();
                        }
                    }
                }
            }
            else
            {
                if (hardInput.GetKeyDown("RecordPlayer"))
                {
                    AudioManager.instance.Play("Ability_TimeRewindDisabled");
                    Debug.Log("You do not have the ability!");
                }
            }

            // if (hardInput.GetKeyDown("SaveRecordings"))
            // {
            //     SaveRecordingsToFile();
            // }
        }

        private void StartPlay()
        {
            AudioManager.instance.Stop("Ability_TimeRewind");
            Debug.Log("Player Start Pos: " + playerVCR.gameObject.transform.position + " Player Start Rot: " + playerVCR.gameObject.transform.GetChild(1).transform.rotation);
            recordingStartPos = playerVCR.gameObject.transform.position;
            recordingStartRot = playerVCR.gameObject.transform.GetChild(1).transform.rotation;

            playerVCR.gameObject.GetComponent<CameraBehaviour>().isActive = true;
            StartCoroutine(Player());
        }

        private IEnumerator Player()
        {
            // Get the previous recording
            Recording recording = playerVCR.GetRecording();

            // Gets selected length and cuts it down if too long
            int framePos = recording.frames.Count - playFrom;

            if (framePos != 0)
            {
                recording.frames.RemoveRange(0, framePos);
            }

            playFrom = 0;

            Debug.Log("Testing - There are " + recording.frames.Count + " recorded! - Spawning");

            // Add previous recording to list
            playerRecordings.Add(recording);

            // Start new recording after delay
            StartCoroutine(StartRecordingDelay(2f));


            if (recording == null)
                yield break;

            Debug.Log("Recording Start Pos: " + recordingStartPos + " Recording Start Rot: " + recordingStartRot);
            Debug.Log("Num of Frames Rec: " + recording.frames.Count);

            // Creates the clone
            Debug.Log(recording.ToString());
            curPlayer = Instantiate(GameObject.Find("Player"));
            curPlayer.GetComponent<PlayerBehaviour>().isClone = true;

            Destroy(curPlayer.GetComponent<playerTutorial>());
            curPlayer.GetComponent<Timeline>().mode = TimelineMode.Global;
            curPlayer.GetComponent<Timeline>().globalClockKey = "RecordedClones";
            curPlayer.transform.Find("Animator").GetComponent<Timeline>().mode = TimelineMode.Global;
            curPlayer.transform.Find("Animator").GetComponent<Timeline>().globalClockKey = "RecordedClones";
            curPlayer.GetComponent<PlayerBehaviour>().ResetRigidBody();

            curPlayer.tag = "RecordedPlayer";

            curPlayer.transform.Find("Camera").gameObject.SetActive(false);
            Destroy(curPlayer.transform.Find("InputRecorder").gameObject);

            GameObject displayName, cloneDigitalCode;

            displayName = Instantiate(displayNameObject);
            displayName.transform.SetParent(curPlayer.transform);
            displayName.transform.rectTransform().localPosition = new Vector3(0, 2.079996f, 0);
            displayName.name = "DisplayInfoAbove";


            Material[] ghostMaterials = new Material[] { ghostMaterial, ghostMaterial, ghostMaterial, ghostMaterial, ghostMaterial, ghostMaterial, ghostMaterial };

            curPlayer.transform.FindDeepChild("VBOT_:VBOT_LOD3").transform.parent = curPlayer.transform.Find("Animator");
            curPlayer.transform.FindDeepChild("VBOT_:VBOT_LOD3").GetComponent<Renderer>().materials = ghostMaterials;
            curPlayer.transform.FindDeepChild("VBOT_:VBOT_LOD3").gameObject.AddComponent<Outline>();
            curPlayer.transform.FindDeepChild("VBOT_:VBOT_LOD3").name = "PlayerMesh";
            Destroy(curPlayer.transform.FindDeepChild("Mesh_LOD").gameObject);

            cloneDigitalCode = Instantiate(cloneDigitalCodeObject);
            cloneDigitalCode.transform.SetParent(curPlayer.gameObject.transform.FindDeepChild("PlayerMesh"), false);
            cloneDigitalCode.name = "DigitalCodeGhostParticles";


            // Sets the starting camera rotation
            float cameraX = playerVCR.gameObject.transform.FindDeepChild("Z").transform.rotation.x;
            float cameraZ = playerVCR.gameObject.transform.FindDeepChild("Y").transform.rotation.y;
            curPlayer.gameObject.GetComponent<CameraBehaviour>().SetRotationX(cameraX);
            curPlayer.gameObject.GetComponent<CameraBehaviour>().SetRotationZ(cameraZ);

            // Set Health & Energy
            curPlayer.gameObject.GetComponent<PlayerBehaviour>().life = playerVCR.gameObject.GetComponent<PlayerBehaviour>().life;
            curPlayer.gameObject.GetComponent<PlayerBehaviour>().energy = playerVCR.gameObject.GetComponent<PlayerBehaviour>().energy;


            curPlayer.GetComponent<InputVCR>().Play(Recording.ParseRecording(recording.ToString()));

            recPlayerNum++;
            curPlayer.name = "RecordedPlayer" + recPlayerNum;

            curPlayer.GetComponent<InputVCR>().recordingNumber = recPlayerNum;
            curPlayer.GetComponent<InputVCR>().startPos = recordingStartPos;
            curPlayer.GetComponent<InputVCR>().startRot = recordingStartRot;

            displayName.transform.FindDeepChild("RecordingNumber").GetComponent<FaceObject>().SetRecordingText(curPlayer.gameObject.GetComponent<InputVCR>().recordingNumber);

            CreateCurrentCloneCopy();

            float playTime = recording.recordingLength;
            float curTime = 0f;

            isPlaying = true;
            while (curTime < playTime)
            {
                if (isPlaying)
                    curTime += Time.deltaTime;

                yield return 0;
            }
        }

        public void SaveRecordingsToFile()
        {
            if (!Directory.Exists(System.Environment.CurrentDirectory + @"\PlayerRecordings\"))
            {
                Debug.Log("Recording folder dosn't exist. Creating new recording folder!");
                Directory.CreateDirectory(System.Environment.CurrentDirectory + @"\PlayerRecordings\");
            }

            if (Directory.Exists(System.Environment.CurrentDirectory + @"\PlayerRecordings\"))
            {
                if (playerRecordings.Count != 0)
                {
                    for (int i = 0; i < playerRecordings.Count; i++)
                    {
                        System.IO.File.WriteAllText(saveRecordingPath + (i + 1).ToString() + ".txt", playerRecordings[i].ToString());
                        Debug.Log("Saving Recording " + (i + 1) + "...");
                    }
                    Debug.Log("Save Complete!");
                }
                else
                {
                    Debug.Log("There is no player recordings to save!");
                }
            }
            else
            {
                Debug.Log("Recording folder still dosn't exist. Please try saving again.");
            }
        }

        public void RewindToFrame()
        {
            // Sets current recorded frames
            tempMaxFrames = (maxRecordTime * recFPS);
            if (playerVCR.GetRecording().frames.Count < tempMaxFrames)
            {
                tempMaxFrames = playerVCR.GetRecording().frames.Count;
            }
            if (playFrom < tempMaxFrames)
            {
                playFrom++;
                rewindCountDown = Mathf.FloorToInt((tempMaxFrames - playFrom) / recFPS) + 1;
                rewindCountDownObj.GetComponent<Text>().text = rewindCountDown.ToString();

                //Debug.Log((tempMaxFrames - playFrom) / recFPS % 1);

                if (tempMaxFrames >= maxRecordTime * recFPS)
                {
                    countDownEarly = true;
                }
                if ((tempMaxFrames - playFrom) / recFPS % 1 == 0 && !countDownEarly)
                {
                    rewindCountDownObj.GetComponent<Animation>().Play();
                    countDownEarly = true;
                }
                if (!rewindCountDownObj.GetComponent<Animation>().isPlaying && countDownEarly)
                {
                    rewindCountDownObj.GetComponent<Animation>().Play();
                }
            }
            else
            {
                Debug.Log("Max Rewind Reached!");
                triggeredEarly = true;
                countDownEarly = false;
                RewindStuff();
            }
        }

        private void RewindStuff()
        {
            CancelInvoke("RewindToFrame");
            Clock globalClock = Timekeeper.instance.Clock("Root");
            firstPress = true;
            rewindEnd = true;
            globalClock.localTimeScale = 1;
            playerVCR.gameObject.GetComponent<PlayerBehaviour>().stopKeyInput = false;
            playerCamera.gameObject.GetComponent<ChangeCameraEffects>().ChangeFOV(80, 2);
            playerCamera.gameObject.GetComponent<ChangeCameraEffects>().ChangeGrain(0, 2);
            StartPlay();
        }

        private void CreateCurrentCloneCopy()
        {
            int tempCount = allRecordedClones.Count;
            allRecordedClones.Add(curPlayer);
            allRecordedClones[tempCount].tag = "RecordedPlayerTemp";
            Destroy(allRecordedClones[tempCount].transform.Find("InputRecorder").gameObject);
            Destroy(allRecordedClones[tempCount].transform.FindDeepChild("Mesh_LOD").gameObject);
            allRecordedClones[tempCount].SetActive(false);
            SpawnCloneCopy(curPlayer.name, recPlayerNum, 0, curPlayer.transform.position, curPlayer.transform.rotation);
        }

        public void SpawnCloneCopy(string name, int num, float time, Vector3 pos, Quaternion rot)
        {
            StartCoroutine(SpawnCloneCopyDelay(name, num, time, pos, rot));
        }

        private IEnumerator SpawnCloneCopyDelay(string name, int num, float time, Vector3 pos, Quaternion rot)
        {
            yield return new WaitForSeconds(time);

            GameObject spawnTemp = Instantiate(tempAudio, pos, rot);
            spawnTemp.GetComponent<AudioSource>().PlayOneShot(ghostSpawn, 1);
            Destroy(spawnTemp, 2f);

            int temp = allRecordedClonesOfClones.Count;

            allRecordedClonesOfClones.Add(Instantiate(allRecordedClones[num - 1]));

            allRecordedClonesOfClones[temp].tag = "RecordedPlayer";
            allRecordedClonesOfClones[temp].name = "RecordedPlayer" + num;
            allRecordedClonesOfClones[temp].GetComponent<InputVCR>().startPos = pos;
            allRecordedClonesOfClones[temp].GetComponent<InputVCR>().startRot = rot;
            allRecordedClonesOfClones[temp].SetActive(true);
            allRecordedClonesOfClones[temp].GetComponent<InputVCR>().Play(playerRecordings[num - 1]);
            allRecordedClonesOfClones[temp].GetComponent<PlayerBehaviour>().ResetRigidBody();
            allRecordedClonesOfClones[temp].GetComponent<Timeline>().time = 0;
        }

        private IEnumerator MinRewindDelay(float time)
        {
            yield return new WaitForSeconds(time);
            canDeactivate = true;
        }

        public void SetHaveAbility(bool answer)
        {
            haveAbility = answer;
        }

        public void DestroyClone(GameObject clone)
        {
            GameObject spawnTemp = Instantiate(tempAudio, clone.transform.position, clone.transform.rotation);
            spawnTemp.GetComponent<AudioSource>().PlayOneShot(ghostSpawn, 1);
            Destroy(spawnTemp, 2f);

            allRecordedClonesOfClones.Remove(clone);
            Destroy(clone, 0.1f);
        }

        private IEnumerator StartRecordingDelay(float time)
        {
            yield return new WaitForSeconds(time);
            StartRecording();
        }
    }
}