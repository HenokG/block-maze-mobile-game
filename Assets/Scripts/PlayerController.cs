using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    private float _xPosition;
    private float _yPosition;
    private int eatablesCount;
    private List<string> takenPath;
    private List<GameObject> trails;
    private bool IsUndoHorizontal;
    private int UndoToPosition;

    public LevelManager LevelManager;
    public GameObject trailPrefab;
    public GameObject multiTrailPrefab;
    public GameObject NormalGridsContainer;
    public GameObject NormalPositionersContainer;
    public GameObject MultiGridsContainer;
    public GameObject MultiPositionersContainer;
    protected bool IsAboveLevel31;

    public int EatablesCount { get; set; }

    public float XPosition
    {
        get { return _xPosition; }
        set
        {
            if (!IsAboveLevel31 && (value < 2 || value > 9)) return; //bound x position to grid
            if (IsAboveLevel31 && (value < 2 || value > 14)) return; //bound x position to grid
            string prefix = "p_";
            if (IsAboveLevel31)
            {
                prefix = "mp_";
            }
            string gameObjectName =  prefix + value + "" + _yPosition;
            if (takenPath.Count >= 1)
            {
                foreach (string s in takenPath)
                {
                    if (s.Equals(gameObjectName))
                    {
                        //check if player is moving backwards(that is allowed) its like undo action
                        //but first check if the player moved enough moves to do [undo]
                        if (takenPath.Count >= 2 && takenPath[takenPath.Count - 2].Equals(gameObjectName)
                        ) //if clicked position is behind by one
                        {
                            IsUndoHorizontal = false;
                            UndoToPosition = (int) _xPosition;
                            if (takenPath.Count != 1)
                                takenPath.RemoveAt(takenPath.Count - 1);
                            //undo everything done in moving a piece
                            trails[trails.Count - 1].transform.position =
                                new Vector3(1000, 1000, 1000); //for the sake of OnTriggerExit()
                            trails.RemoveAt(trails.Count - 1);
                            _xPosition = value;
                            Vector3 pos = GameObject.Find(takenPath[takenPath.Count - 1]).transform.position;
                            GetComponent<Rigidbody>().MovePosition(pos);
                            return;
                        }
                        return;
                    }
                }
            }

            takenPath.Add(gameObjectName);
            IsUndoHorizontal = false;
            UndoToPosition = (int) _xPosition;
            _xPosition = value;
            GameObject positioner = GameObject.Find(gameObjectName);
            //draw trail before moving object
            //average of former position and next position gives center of trail object
            if (positioner == null) return;

            GameObject smallOrNormalTrail;
            if (LevelManager.MyEventManager.Level >= 31)
                smallOrNormalTrail = multiTrailPrefab;

            else
                smallOrNormalTrail = trailPrefab;

            GameObject newTrail = Instantiate(smallOrNormalTrail, (transform.position + positioner.transform.position) / 2,
                Quaternion.Euler(0, 0, 90));
            newTrail.GetComponent<Renderer>().material.color = MyEventManager.ChoosenMainColor;
            trails.Add(newTrail);
            GetComponent<Rigidbody>().MovePosition(positioner.transform.position);
        }
    }

    public float YPosition
    {
        get { return _yPosition; }
        set
        {
            if (!IsAboveLevel31 && (value < 1 || value > 7)) return; //bound y position to grid 1-7
            if (IsAboveLevel31 && (value < 1 || value > 11)) return; //bound y position to grid 1-10 for multi puzzle stages

            string prefix = "p_";
            if (IsAboveLevel31)
            {
                prefix = "mp_";
            }
            string gameObjectName = prefix + _xPosition + "" + value;
            if (takenPath.Count >= 1)
            {
                foreach (string s in takenPath)
                {
                    if (s.Equals(gameObjectName))
                    {
                        //check if player is moving backwards(that is allowed) its like undo action
                        //but check if the player moved enough moves to do [undo]
                        if (takenPath.Count >= 2 && takenPath[takenPath.Count - 2].Equals(gameObjectName)
                        ) //if clicked position is behind by one
                        {

                            //special case when path length is 1 if we removed that then moveposition will call path of -1
                            IsUndoHorizontal = true;
                            UndoToPosition = (int) _yPosition;
                            if (takenPath.Count != 1)
                                takenPath.RemoveAt(takenPath.Count - 1);
                            //undo everything done on moving a piece
                            trails[trails.Count - 1].transform.position =
                                new Vector3(1000, 1000, 1000); //for the sake of trigger
                            trails.RemoveAt(trails.Count - 1);
                            _yPosition = value;
                            //special case
                            GetComponent<Rigidbody>()
                                .MovePosition(GameObject.Find(takenPath[takenPath.Count - 1]).transform.position);
                            return;
                        }

                        return;
                    }
                }
            }

            takenPath.Add(gameObjectName);
            IsUndoHorizontal = true;
            UndoToPosition = (int) _yPosition;
            _yPosition = value;
            GameObject positioner = GameObject.Find(gameObjectName);
            //draw trail before moving object
            //average of former position and next position gives center of trail object
            if (positioner == null) return;

            var smallOrNormalTrail = LevelManager.MyEventManager.Level >= 31 ? multiTrailPrefab : trailPrefab;

            GameObject newTrail = Instantiate(smallOrNormalTrail, (transform.position + positioner.transform.position) / 2,
                Quaternion.Euler(0, 0, 0));
            newTrail.GetComponent<Renderer>().material.color = MyEventManager.ChoosenMainColor;
            trails.Add(newTrail);
            GetComponent<Rigidbody>().MovePosition(positioner.transform.position);
        }
    }

    // Use this for initialization
    public void ManualStart()
    {
        IsAboveLevel31 = LevelManager.MyEventManager.Level >= 31;
        string prefix = "p_";
        if (IsAboveLevel31)
        {
            prefix = "mp_";
            NormalGridsContainer.SetActive(false);
            MultiGridsContainer.SetActive(true);
            NormalPositionersContainer.SetActive(false);
            MultiPositionersContainer.SetActive(true);
        }
        else
        {
            NormalGridsContainer.SetActive(true);
            MultiGridsContainer.SetActive(false);
            NormalPositionersContainer.SetActive(true);
            MultiPositionersContainer.SetActive(false);
        }

        trails = new List<GameObject>();
        switch (PlayerPrefs.GetInt(FinalConstants.LevelTag, 1))
        {
            case 18:
                _xPosition = FinalConstants.Level18Start[0];
                _yPosition = FinalConstants.Level18Start[1];
                break;
            case 19:
                _xPosition = FinalConstants.Level19Start[0];
                _yPosition = FinalConstants.Level19Start[1];
                break;
            case 20:
                _xPosition = FinalConstants.Level20Start[0];
                _yPosition = FinalConstants.Level20Start[1];
                break;
            case 21:
                _xPosition = FinalConstants.Level21Start[0];
                _yPosition = FinalConstants.Level21Start[1];
                break;
            case 22:
                _xPosition = FinalConstants.Level22Start[0];
                _yPosition = FinalConstants.Level22Start[1];
                break;
            case 23:
                _xPosition = FinalConstants.Level23Start[0];
                _yPosition = FinalConstants.Level23Start[1];
                break;
            case 25:
                _xPosition = FinalConstants.Level25Start[0];
                _yPosition = FinalConstants.Level25Start[1];
                break;
            case 28:
                _xPosition = FinalConstants.Level28Start[0];
                _yPosition = FinalConstants.Level28Start[1];
                break;
            case 30:
                _xPosition = FinalConstants.Level30Start[0];
                _yPosition = FinalConstants.Level30Start[1];
                break;
            case 31:
                _xPosition = FinalConstants.Level31Start[0];
                _yPosition = FinalConstants.Level31Start[1];
                break;
            case 32:
                _xPosition = FinalConstants.Level32Start[0];
                _yPosition = FinalConstants.Level32Start[1];
                break;
            case 33:
                _xPosition = FinalConstants.Level33Start[0];
                _yPosition = FinalConstants.Level33Start[1];
                break;
            case 35:
                _xPosition = FinalConstants.Level35Start[0];
                _yPosition = FinalConstants.Level35Start[1];
                break;
            case 36:
                _xPosition = FinalConstants.Level36Start[0];
                _yPosition = FinalConstants.Level36Start[1];
                break;
            case 37:
                _xPosition = FinalConstants.Level37Start[0];
                _yPosition = FinalConstants.Level37Start[1];
                break;
            case 38:
                _xPosition = FinalConstants.Level38Start[0];
                _yPosition = FinalConstants.Level38Start[1];
                break;
            case 39:
                _xPosition = FinalConstants.Level39Start[0];
                _yPosition = FinalConstants.Level39Start[1];
                break;
            default:
                _xPosition = 2;
                _yPosition = 1;
                break;
        }

        GetComponent<Rigidbody>().MovePosition(GameObject.Find(prefix+ XPosition + YPosition).transform.position);

        takenPath = new List<string>
        {
            prefix + XPosition + YPosition
        }; //p_ stand for positioner: since player is on 11 on start
    }

    // Update is called once per frame
    void Update()
    {
    	if (EventSystem.current.IsPointerOverGameObject()) return;
        if (Time.timeScale > 0 && Input.GetMouseButtonDown(0) && !(Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began && EventSystem.current.IsPointerOverGameObject(Input.touches[0].fingerId)))
        {
            float inputX = Input.mousePosition.x;
            float inputY = Input.mousePosition.y;
            float playerX = Camera.main.WorldToScreenPoint(transform.position).x;
            float playerY = Camera.main.WorldToScreenPoint(transform.position).y;
            float diffX = inputX - playerX;
            float diffY = inputY - playerY;

            if (Mathf.Abs(diffX) > Mathf.Abs(diffY) && (!IsAboveLevel31 && Math.Abs(XPosition - 10) > 0.5 || IsAboveLevel31 && Math.Abs(XPosition - 14) > 0.5))    //block player from moving in hori while at the most top element
            {
                if (diffX > 0)
                {
                    YPosition += 1;
                }
                else
                {
                    YPosition -= 1;
                }
            }
            else if (!IsAboveLevel31 && Math.Abs(YPosition - 8) > 0.5 || IsAboveLevel31 && Math.Abs(YPosition - 11) > 0.5)
            {
                if (diffY > 0)
                {
//                    if (XPosition >= 7) return;
                    XPosition += 1;
                }
                else
                {
                    XPosition -= 1;
                }
            }
        }
    }

    public void Reset()
    {
        ManualStart();
    }

    public void UndoMove()
    {
        if (IsUndoHorizontal)
            YPosition = UndoToPosition;
        else
            XPosition = UndoToPosition;
    }
}