using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class TaskScript : MonoBehaviour
{

    //clean slef1|0|220605|5|true|

    public string taskName;
         public GameObject taskGameObject;
    public string typeID = "";
    public string taskID = "";//CAT_PREF+#
    public int startDate = 220604;
    public int intervalDay = 1;
    public bool isInterval = true;
         public int lastDone = 0;
         public TMP_Text date;
         public GameObject DunImg;
         public GameObject TypeImg;
    public bool isDone = false;
    [Header("Category")]
    public TMP_InputField catName;
    public TMP_InputField typeIDInp;
    //public TMP_InputField cdate;
    public TMP_InputField interval;
    //public TMP_InputField isIntervalInp;
    public Toggle isIntervalToggle;
    public TMP_InputField cnote;

    string CAT_PREF = "tsk";
    string TYPE_PREF = "col";

    string DUN_PREF = "dn";

    class Task
    {
        public string Name;
        public string type = ""; //deleted task "-1"
        public string ID = "";//CAT_PREF+#
        public int start = 220604;
        public int interval = 1; //if !isInt, it should be init with 0 and the day it was finished
        public bool isInt = true;
    }
    class Type
    {
        public string Name;
        public string color = ""; //deleted task "-1"
        public string ID = "";//TYPE_PREF+#
        public string other = "";
        public bool isDisplay = true;
    }
    private void Start()
    {

       /// Debug.Log(date == null);


    }
    //public bool IsFinished(int today)
    //{
    //    return today == lastDone;
    //}
    public void ShowDoneBaG()
    {
        date = GameObject.FindGameObjectWithTag("today").GetComponent<TMP_Text>();

        if (!isInterval)
        {
            bool isFinish = (startDate != 0); //not interval task, has the pref, but should become done here on forever
            finishHim(isFinish);
        }
        else
        {
            bool isFinish = PlayerPrefs.HasKey(DUN_PREF + taskID + date.text);
          //  Debug.Log("one "+DUN_PREF + taskID + date.text);
            finishHim(isFinish);
        }
    }
    void finishHim(bool done)
    {
        if (DunImg != null)
        {
            DunImg.SetActive(done);
            isDone = done;

        }
    }
    public void SetText()
    {
        TextMeshProUGUI taskText = GetComponentInChildren<TextMeshProUGUI>();
        taskText.text = taskName + "\t" + startDate + "\n" + taskID + "\t\t" + typeID + "\n" + isInterval.ToString() + "\t\t" + intervalDay;
            //PlayerPrefs.GetString(taskID, " ");

    }
    public void SetShortText()
    {
        TextMeshProUGUI taskText = GetComponentInChildren<TextMeshProUGUI>();
        taskText.text = taskName;

    }
    //called from UI
    //adds task to PlayerPrefs : (DUN_PREF + taskID + date.text, "100")
    //chanege the startdate of task in PlayerPrefs
    public void done(bool finished)
    {
        if (finished)
        {
            PlayerPrefs.SetString(DUN_PREF + taskID + date.text, "100");
            if (!isInterval)
            {
                startDate = int.Parse(date.text);
                SaveTask();
            }
        }
        else
        {
            PlayerPrefs.DeleteKey(DUN_PREF + taskID + date.text);
            if (!isInterval)
            {
                startDate = 0;
                SaveTask();
            }
        }
      ShowDoneBaG();
    }
    public void ToggleFin()
    {
        done(!isDone);//if task not done, done(!false) == done(true) will done the task
    }
    public void SaveTask()
    {
        var task = new Task
        {
            Name = taskName,
            type = typeID,
            ID = taskID,
            start = startDate,
            interval = intervalDay,
            isInt = isInterval,
        };

        var jsonstring = JsonUtility.ToJson(task);
        PlayerPrefs.SetString(taskID, jsonstring);
        PlayerPrefs.Save();
        Debug.Log("save "+taskID );
    }
    public void RemoveTask()
    {
        typeID = "-1";
        SaveTask();
       // PlayerPrefs.DeleteKey(taskID);
    }
    public static bool isDeleted(string id)
    {
        if (PlayerPrefs.HasKey(id))
        {
            var jsonstring = PlayerPrefs.GetString(id);
           // Debug.Log(jsonstring);
            var task = JsonUtility.FromJson<Task>(jsonstring);
            if (task != null)
            {
                return task.type == "-1";
            }
        }
        return false;
    }
    public bool isDeleted()
    {
        return typeID == "-1";

    }
    public TaskScript GetTask(int ID)//GetLastUnusedID must be called before this function
    {
        SetDataFromJson(CAT_PREF + ID);
        ShowDoneBaG();
        ShowType();
        return this;
    }

    private void ShowType()
    {
     if(typeID != "0") {
            if (TypeImg != null)
                TypeImg.SetActive(true);
            //else
              //  Debug.Log("heeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeey");
        }  
    }

    public TaskScript GetTask(string id)//GetLastUnusedID must be called before this function
    {
        SetDataFromJson(id);
        ShowDoneBaG(); // ShowDaysTasks creates a taaskscript w/o a g.o so it doesnt havw an image and these two
        ShowType();    //--need to be checked afterwards.
        return this;
    }
    void SetDataFromJson(string id)
    {
        if (PlayerPrefs.HasKey(id))
        {
            var jsonstring = PlayerPrefs.GetString(id);
         //   Debug.Log(jsonstring);
            var task = JsonUtility.FromJson<Task>(jsonstring);
            if (task != null)
            {
              //  Debug.Log(task.Name + " " + task.start);


                taskName = task.Name;
                typeID = task.type;
                taskID = task.ID;//CAT_PREF+#
                startDate = task.start;
                intervalDay = task.interval;
                isInterval = task.isInt;
            }
        }
    }
    public void SaveEdits()
    {
       // int id = GetLastUnusedID(CAT_PREF);

        //TaskScript task = new TaskScript();
        //Name = taskName,
        //    type = typeID,
        //    ID = taskID,
        //    start = startDate,
        //    interval = intervalDay,
        //    isInt = isInterval,

       // taskID = taskID;//CAT_PREF + id;
        taskName = catName.text;
        typeID = typeIDInp.text;
        //startDate = int.Parse(cdate.text);
        intervalDay = int.Parse(interval.text);
        isInterval = isIntervalToggle.isOn;
        //bool.Parse(isInterval.text);
        SaveTask();

      //  Log("entry added");
    }
    public void LoadEditPopUp()
    {
        catName.text = taskName;
        typeIDInp.text = typeID;
        // cdate.text)=startDate
        interval.text = intervalDay.ToString();
        isIntervalToggle.isOn = isInterval;
    }


}
