using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;



public class AddTaskScript : MonoBehaviour
{

    [Header("Hud")]
    public TMP_Text dateOut2;
    public TMP_Text dateOut;

    [Header("Addtask Panel")]
    public TMP_InputField catName;
    public TMP_InputField typeID;
    public TMP_InputField cdate;
    public TMP_InputField interval;
    public TMP_InputField isInterval;
    public Toggle isIntervalToggle;
    public TMP_InputField cnote;

    [Header("SetBackground")]
    public List<Button> bgButtons;

    [Header("HomePage")]
    public Transform DayTasks;
    public Transform DlTasks;
    public TMP_InputField Note;
    public GameObject catPrefab2;


    [Header("Calender")]
    public GameObject calenderTaskPrefab;
    public Transform taskTransform;


    [Header("History")]
    public TMP_InputField HistoryInput;

    [Header("TaskList Page")]
    public GameObject catPrefab;
    public Transform catTransform;


    string LastDoneTemp = "";
    string CAT_PREF = "tsk";
    string DUN_PREF = "dn";
    string NOTE_PREF = "nt";



    void Start()
    {
        ResetCalenderAndTaskList();
        SetBG();
    }
    //return DateTime as String 230101
    string getDateFrom(DateTime dt)
    {
        string myDate = dt.Year.ToString().Substring(2)
     + (dt.Month < 10 ? "0" + dt.Month.ToString() : dt.Month.ToString())
     + (dt.Day < 10 ? "0" + dt.Day.ToString() : dt.Day.ToString());

        return myDate;
    }
    public void ResetCalenderAndTaskList()
    {
        ClearTransform(DayTasks);
        ClearTransform(DlTasks);
        showTasks(CAT_PREF, catPrefab, catTransform);
        ShowDaysTasks();
        SetTodayNote();
    }
    private void Awake()
    {
        dateOut.text = getDateFrom(DateTime.Now);
        dateOut2.text = DateTime.Now.DayOfWeek.ToString().Substring(0, 3);

    }
    public void CheatButtonAdd()
    {
        List<List<string>> tasks = new List<List<string>>();
        for (int i = 0; i < 11; i++) {
            tasks.Add(new List<string>());
        }
        tasks[0] = new List<string> { "0", "Vacuum", "231206", "5" };
        tasks[1] = new List<string> { "1", "Dish-m", "231207", "3" };
        tasks[2] = new List<string> { "2", "Dish-d", "231208", "2" };
        tasks[3] = new List<string> { "3", "Self1", "231211", "7" };
        tasks[4] = new List<string> { "4", "Self2", "231215", "10" };
        tasks[5] = new List<string> { "5", "Laundry", "231207", "7" };
        tasks[6] = new List<string> { "6", "Cooking", "231208", "2" };
        tasks[7] = new List<string> { "7", "Balcony", "231207", "20" };
        tasks[8] = new List<string> { "8", "Toilet", "231212", "30" };
        tasks[9] = new List<string> { "9", "Handwash", "240105", "30" };
        tasks[10] = new List<string> { "10", "Grocery", "231207", "3" };
        //tasks[11] = new List<string>{ "111","test3","220606","4"};
        for (int i = 0; i < 11; i++)
        {
            TaskScript task = new TaskScript();
            task.taskID = CAT_PREF + int.Parse(tasks[i][0]);
            task.taskName = tasks[i][1];//"grocery";
            task.typeID = "0";
            task.startDate = int.Parse(tasks[i][2]);
            task.intervalDay = int.Parse(tasks[i][3]);
            task.isInterval = true;
            task.lastDone = 0;
            task.SaveTask();
        }
        ResetCalenderAndTaskList();
    }


    //calender
    void ShowDaysTasks()//"clean slef1|0|220605|5|true|"
    {
        ClearTransform(taskTransform);
        int totalNumber = 26;
        DateTime dt = ChangeDate(0, dateOut.text);

        List<List<string>> dayTasksList = new List<List<string>>();
        for (int j = 0; j < totalNumber; j++)
        {
            dayTasksList.Add(new List<string>());
        }

        int id = GetLastUnusedID(CAT_PREF);
        for (int i = 0; i < id; i++)
        {
            TaskScript task = new TaskScript();
            task.GetTask(i);
            if (!task.isDeleted())
            {
                if (task.isInterval)
                {
                    task.lastDone = TaskDun(i);
                    DateTime startDT = ChangeDate(0, task.startDate.ToString());
                    DateTime lastDT = ChangeDate(0, task.lastDone.ToString());

                    DateTime DueDT = (startDT >= dt) ? startDT : (lastDT.Year == 1) ? dt : ((lastDT.AddDays(task.intervalDay)) >= dt) ? (lastDT.AddDays(task.intervalDay)) : dt;
                    if (DueDT == dt)
                        AddToHomeTasks(task);

                    for (int j = 0; j < totalNumber; j++)
                    {
                        DateTime newDay = dt.AddDays(j);
                        if (DueDT == newDay)
                        {
                            dayTasksList[j].Add(task.taskName);
                            DueDT = DueDT.AddDays(task.intervalDay);
                        }
                    }
                }
                else //Every not interval task is displayed in home page
                {
                    AddToHomeTasks(task);
                }
            }
        }
        for (int i = 0; i < totalNumber; i++)
        {
            DateTime tempDate = dt.AddDays(i);
            string DayTasksString =// "<b>" +
                tempDate.DayOfWeek.ToString().Substring(0, 3) + getDateFrom(tempDate).Substring(2) + "\n";
                //+ "</b>\n";
            for (int j = 0; j < dayTasksList[i].Count; j++)
            {
                DayTasksString += dayTasksList[i][j] + "\n";
            }

            GameObject CalenderTask = Instantiate<GameObject>(calenderTaskPrefab, taskTransform);
            TextMeshProUGUI taskText = CalenderTask.GetComponentInChildren<TextMeshProUGUI>();
            taskText.text = DayTasksString;
        }


    }
    //show all tasks in the tasklist page
    void showTasks(string prefix, GameObject prefab, Transform parentTransform)
    {
        ClearTransform(parentTransform);
        int id = GetLastUnusedID(prefix);
        for (int i = 0; i < id; i++)
        {
            if (!TaskScript.isDeleted(prefix + i))
            {
                GameObject task = Instantiate<GameObject>(prefab, parentTransform);
                task.GetComponent<TaskScript>().GetTask(i).SetText();
            }
        }

    }
    void AddToHomeTasks(TaskScript task)
    {
        Transform parentTransform;
        parentTransform = DayTasks;
        GameObject taskGameObject = Instantiate<GameObject>(catPrefab2, parentTransform);
        taskGameObject.GetComponent<TaskScript>().GetTask(task.taskID).SetShortText();
        if (!task.isInterval)
            taskGameObject.transform.SetAsFirstSibling();
    }

    //text with dates and tasks finished on that date
    //we have both interval and normal tasks so the
    public void ShowHistory()
    {
        int totalNumberDays = 100;
        int id = GetLastUnusedID(CAT_PREF);
        HistoryInput.text = "";
        DateTime todayDT = ChangeDate(0, dateOut.text);

        for (int j = 0; j <= totalNumberDays; j++)
        {
            DateTime newDay = todayDT.AddDays(-j);
            // string dayy = 
            int day = int.Parse(getDateFrom(newDay));

            HistoryInput.text += day + "\n";
            HistoryInput.text += GetNote(day) + "\n";
            for (int i = 0; i < id; i++)
            {
                if (PlayerPrefs.HasKey(DUN_PREF + CAT_PREF + i + day))
                {
                    HistoryInput.text += "\t" + new TaskScript().GetTask(i).taskName + "\n";
                }
            }
        }
    }


    //get date of when task was last done 
    public int TaskDun(int taskID)
    {
        int today = GetTodayDate();
        DateTime dt = ChangeDate(0, dateOut.text);
        int lastDone = 0;

        for (int i = -1; i > -30; i--)
        {
            DateTime newDay = dt.AddDays(i);
            string dayy = getDateFrom(newDay);
            int day = int.Parse(dayy);//today + i;
                                      //  Debug.Log("two "+DUN_PREF + CAT_PREF + taskID + day);
            if (PlayerPrefs.HasKey(DUN_PREF + CAT_PREF + taskID + day))
            {
                lastDone = day;//int.P=arse(PlayerPrefs.GetString(DUN_PREF + CAT_PREF + taskID + day));
                               // Log("this " + lastDone+" "+dayy);

                return lastDone;
            }
        }
        //  Log("this2 " + lastDone);

        return lastDone;
    }
    //public void AddTask()
    //{
    //    int id = GetLastUnusedID(TAKS_PREF);
    //    PlayerPrefs.SetString(TAKS_PREF + id, taskName.text + "|" + catID.text + "|" +
    //        note.text + "|" + date.text + "|" + done.text);
    //    Log("entry added");
    //    showTasks(TAKS_PREF, taskPrefab, taskTransform);

    //}
    public void AddCategory()
    {
        int id = GetLastUnusedID(CAT_PREF);

        TaskScript task = new TaskScript();
        task.taskID = CAT_PREF + id;
        task.taskName = catName.text;
        task.typeID = typeID.text;
        task.startDate = int.Parse(cdate.text);
        task.intervalDay = int.Parse(interval.text);
        task.isInterval = isIntervalToggle.isOn;
        //bool.Parse(isInterval.text);
        task.lastDone = 0;
        if (!task.isInterval)
        {
            task.startDate = 0;
        }
        task.SaveTask();

        Log("entry added");
        // showTasks(CAT_PREF, catPrefab, catTransform);
        ResetCalenderAndTaskList();
    }
    //get the last task id+1 to read other tasks from playerprefab
    int GetLastUnusedID(string prefix)
    {
        int i = 0;
        while (true)
        {

            if (!PlayerPrefs.HasKey(prefix + i))
            {

                return i;
            }
            i++;


        }
    }
    public void ChangeDate(int i)
    {

        DateTime dt = new DateTime();
        dt = ChangeDate(i, dateOut.text);
        dateOut2.text = dt.DayOfWeek.ToString().Substring(0, 3);
        dateOut.text = getDateFrom(dt);


        ShowHistory();
        ResetCalenderAndTaskList();
        SetTodayNote();



    }
    //Changes string Date to DateTime, +i days.
    DateTime ChangeDate(int i, string date)
    {
        DateTime dt = new DateTime();

        if (date.Length == 6)
        {
            string y = "20" + date.Substring(0, 2);
            string m = date.Substring(2, 2);
            string d = date.Substring(4, 2);

            int year = int.Parse(y);
            int month = int.Parse(m);
            int day = int.Parse(d);

            dt = new DateTime(year, month, day);
            dt = dt.AddDays(i);
        }
        else if (date.Length == 1)
        {
            dt = new DateTime(1, 1, 1);
        }
        else
        {
            Log("changeDate Exception" + date);
            dt = new DateTime(2022, 06, 06);
        }
        return dt;
    }
    public int GetTodayDate()
    {
        return int.Parse(dateOut.text);

    }
    public void ClearTransform(Transform transform)
    {
        while (transform.childCount > 0)
        {
            GameObject.DestroyImmediate(transform.GetChild(0).gameObject);//funny! destroy will make it stackoverflow
        }
    }
    public void Log(string msg)
    {
        Debug.Log(msg);
    }
    public void Exit()
    {
        Application.Quit();
    }
    public void SaveNote()
    {
        int today = GetTodayDate();
        PlayerPrefs.SetString(NOTE_PREF + today, Note.text);

    }
    public string GetNote(int day)
    {
        return PlayerPrefs.GetString(NOTE_PREF + day, "");
    }
    public void SetTodayNote()
    {
        int date = GetTodayDate();
        Note.text = GetNote(date);

    }
    public void SaveBG(int i)
    {
            PlayerPrefs.SetInt("BG" , i);
    }
    public void SetBG()
    {
        int i = PlayerPrefs.GetInt("BG");
        bgButtons[i].onClick.Invoke();
     }
}
